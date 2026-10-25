using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Project.Develop.Runtime.Utilities.Rendering
{
    // Склеивает части модели объекта в один SkinnedMeshRenderer: вместо вызова отрисовки на каждую часть — по одному
    // на материал. Части — кожа на костях (SkinnedMeshRenderer) и жёсткие детали (MeshRenderer прямо на кости кожи:
    // шляпа, шлем). Каждая часть сохраняет свою привязку к костям: кость, привязанная у частей по-разному, входит
    // в общий список костей несколько раз
    public static class SkinnedMeshCombiner
    {
        private const int MinPartsToCombine = 2;
        private const int UvChannel = 0;
        private const string CombinedMeshNameSuffix = "_Combined";

        // Только эти данные вершины переносятся в общий меш: часть с другими (цвет, второй UV) склейка потеряла бы
        private static readonly VertexAttribute[] _supportedAttributes =
        {
            VertexAttribute.Position,
            VertexAttribute.Normal,
            VertexAttribute.Tangent,
            VertexAttribute.TexCoord0,
            VertexAttribute.BlendWeight,
            VertexAttribute.BlendIndices
        };

        // Без них данные частей в общем меше разойдутся по длине
        private static readonly VertexAttribute[] _requiredAttributes =
        {
            VertexAttribute.Position,
            VertexAttribute.Normal,
            VertexAttribute.Tangent,
            VertexAttribute.TexCoord0
        };

        // Части — включённые рендереры на активных объектах под root; сам root может лежать под выключенным родителем.
        // Частям нужны одинаковые настройки отрисовки и читаемые меши: в сборке без Read/Write в импорте вершины не
        // прочитать, а в редакторе они читаются всегда, поэтому флаг проверяется явно. Если что-то не сходится, объект
        // не меняется.
        // Склеенный меш получает рендерер первой кожи вместе с её настройками, у остальных частей удаляются только
        // компоненты отрисовки — объекты частей остаются. Меш создаётся заново, уничтожает его вызывающий
        public static bool TryCombine(GameObject root, out Mesh combinedMesh)
        {
            combinedMesh = null;

            SkinnedMeshRenderer[] skins = GetActiveRenderers<SkinnedMeshRenderer>(root.transform);
            List<ModelPart> parts = GetParts(root.transform, skins);

            if (parts.Count < MinPartsToCombine || CanCombine(root, parts, skins[0]) == false)
                return false;

            SkinnedMeshRenderer combinedRenderer = skins[0];

            // Границы кожи заданы относительно корня костей, а без него — относительно её собственного объекта
            Transform boundsSpace = combinedRenderer.rootBone != null ? combinedRenderer.rootBone : combinedRenderer.transform;

            List<Transform> bones = new();
            List<Material> materials = new();

            combinedMesh = CreateMesh($"{root.name}{CombinedMeshNameSuffix}", parts, bones, materials);
            Bounds bounds = GetBounds(parts, boundsSpace);

            combinedRenderer.sharedMesh = combinedMesh;
            combinedRenderer.bones = bones.ToArray();
            combinedRenderer.sharedMaterials = materials.ToArray();
            combinedRenderer.localBounds = bounds;

            for (int i = 1; i < parts.Count; i++)
                parts[i].RemoveRenderer();

            return true;
        }

        private static T[] GetActiveRenderers<T>(Transform root) where T : Renderer
        {
            List<T> renderers = new();

            foreach (T renderer in root.GetComponentsInChildren<T>(true))
            {
                if (renderer.enabled && IsActiveUnder(renderer.transform, root))
                    renderers.Add(renderer);
            }

            return renderers.ToArray();
        }

        // activeInHierarchy не подходит: root может лежать под выключенным родителем
        private static bool IsActiveUnder(Transform transform, Transform root)
        {
            for (Transform current = transform; current != root; current = current.parent)
            {
                if (current.gameObject.activeSelf == false)
                    return false;
            }

            return true;
        }

        // Сначала кожа, потом детали на её костях: остальные MeshRenderer объекта к модели не относятся
        private static List<ModelPart> GetParts(Transform root, SkinnedMeshRenderer[] skins)
        {
            List<ModelPart> parts = new();
            HashSet<Transform> bones = new();

            foreach (SkinnedMeshRenderer skin in skins)
            {
                parts.Add(new SkinPart(skin));
                bones.UnionWith(skin.bones);
            }

            foreach (MeshRenderer renderer in GetActiveRenderers<MeshRenderer>(root))
            {
                if (bones.Contains(renderer.transform.parent))
                    parts.Add(new RigidPart(renderer, renderer.GetComponent<MeshFilter>()));
            }

            return parts;
        }

        private static bool CanCombine(GameObject root, List<ModelPart> parts, SkinnedMeshRenderer sample)
        {
            foreach (ModelPart part in parts)
            {
                string problem = FindCommonProblem(part, sample) ?? part.FindOwnProblem(sample);

                if (problem != null)
                {
                    Debug.LogWarning($"Части модели {root.name} не склеены, часть {part.Renderer.name}: {problem}", root);
                    return false;
                }
            }

            return true;
        }

        private static string FindCommonProblem(ModelPart part, Renderer sample)
        {
            Mesh mesh = part.Mesh;

            if (mesh == null)
                return "нет меша";

            if (mesh.isReadable == false)
                return $"меш {mesh.name} не читается — включите Read/Write в импорте модели";

            if (HasSupportedVertexData(mesh) == false)
                return $"у меша {mesh.name} данные вершины, которые склейка не переносит";

            if (HasOnlyTriangles(mesh) == false || mesh.subMeshCount != part.Renderer.sharedMaterials.Length)
                return $"подмеши меша {mesh.name} не из треугольников или не совпадают с материалами";

            if (HasSameRenderSettings(part.Renderer, sample) == false)
                return "другие настройки отрисовки";

            return null;
        }

        private static bool HasSupportedVertexData(Mesh mesh)
        {
            if (mesh.blendShapeCount > 0)
                return false;

            foreach (VertexAttributeDescriptor descriptor in mesh.GetVertexAttributes())
            {
                if (Array.IndexOf(_supportedAttributes, descriptor.attribute) < 0)
                    return false;
            }

            foreach (VertexAttribute attribute in _requiredAttributes)
            {
                if (mesh.HasVertexAttribute(attribute) == false)
                    return false;
            }

            return true;
        }

        private static bool HasOnlyTriangles(Mesh mesh)
        {
            for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                if (mesh.GetTopology(subMesh) != MeshTopology.Triangles)
                    return false;
            }

            return true;
        }

        private static bool HasSameRenderSettings(Renderer part, Renderer sample)
        {
            return part.gameObject.layer == sample.gameObject.layer
                && part.shadowCastingMode == sample.shadowCastingMode
                && part.receiveShadows == sample.receiveShadows;
        }

        private static Mesh CreateMesh(string name, List<ModelPart> parts, List<Transform> bones, List<Material> materials)
        {
            List<Vector3> vertices = new();
            List<Vector3> normals = new();
            List<Vector4> tangents = new();
            List<Vector2> uvs = new();
            List<BoneWeight> boneWeights = new();
            List<Matrix4x4> bindposes = new();
            List<List<int>> materialsIndices = new();

            foreach (ModelPart part in parts)
            {
                Mesh partMesh = part.Mesh;
                int vertexOffset = vertices.Count;

                vertices.AddRange(partMesh.vertices);
                normals.AddRange(partMesh.normals);
                tangents.AddRange(partMesh.tangents);
                uvs.AddRange(partMesh.uv);

                int[] boneMap = MapBones(part, bones, bindposes);

                foreach (BoneWeight boneWeight in part.GetBoneWeights())
                    boneWeights.Add(RemapBones(boneWeight, boneMap));

                AddIndices(part, vertexOffset, materials, materialsIndices);
            }

            Mesh mesh = new()
            {
                name = name,
                indexFormat = vertices.Count > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16
            };

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.SetUVs(UvChannel, uvs);
            mesh.boneWeights = boneWeights.ToArray();
            mesh.bindposes = bindposes.ToArray();
            mesh.subMeshCount = materialsIndices.Count;

            for (int subMesh = 0; subMesh < materialsIndices.Count; subMesh++)
                mesh.SetIndices(materialsIndices[subMesh], MeshTopology.Triangles, subMesh);

            return mesh;
        }

        // Индекс кости части → индекс в общем списке. Кость с той же привязкой берётся из списка, с другой — добавляется
        // ещё раз
        private static int[] MapBones(ModelPart part, List<Transform> bones, List<Matrix4x4> bindposes)
        {
            Transform[] partBones = part.GetBones();
            Matrix4x4[] partBindposes = part.GetBindposes();
            int[] boneMap = new int[partBones.Length];

            for (int i = 0; i < partBones.Length; i++)
                boneMap[i] = GetOrAddBone(partBones[i], partBindposes[i], bones, bindposes);

            return boneMap;
        }

        private static int GetOrAddBone(Transform bone, Matrix4x4 bindpose, List<Transform> bones, List<Matrix4x4> bindposes)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                if (bones[i] == bone && bindposes[i] == bindpose)
                    return i;
            }

            bones.Add(bone);
            bindposes.Add(bindpose);

            return bones.Count - 1;
        }

        private static BoneWeight RemapBones(BoneWeight boneWeight, int[] boneMap)
        {
            boneWeight.boneIndex0 = boneMap[boneWeight.boneIndex0];
            boneWeight.boneIndex1 = boneMap[boneWeight.boneIndex1];
            boneWeight.boneIndex2 = boneMap[boneWeight.boneIndex2];
            boneWeight.boneIndex3 = boneMap[boneWeight.boneIndex3];

            return boneWeight;
        }

        // Подмеши частей с одним материалом сливаются в один подмеш общего меша
        private static void AddIndices(
            ModelPart part,
            int vertexOffset,
            List<Material> materials,
            List<List<int>> materialsIndices)
        {
            Mesh partMesh = part.Mesh;
            Material[] partMaterials = part.Renderer.sharedMaterials;

            for (int subMesh = 0; subMesh < partMesh.subMeshCount; subMesh++)
            {
                int materialIndex = materials.IndexOf(partMaterials[subMesh]);

                if (materialIndex < 0)
                {
                    materials.Add(partMaterials[subMesh]);
                    materialsIndices.Add(new List<int>());
                    materialIndex = materials.Count - 1;
                }

                foreach (int index in partMesh.GetIndices(subMesh))
                    materialsIndices[materialIndex].Add(index + vertexOffset);
            }
        }

        private static Bounds GetBounds(List<ModelPart> parts, Transform boundsSpace)
        {
            Bounds bounds = parts[0].GetBoundsIn(boundsSpace);

            foreach (ModelPart part in parts)
                bounds.Encapsulate(part.GetBoundsIn(boundsSpace));

            return bounds;
        }

        private abstract class ModelPart
        {
            protected ModelPart(Renderer renderer, Mesh mesh)
            {
                Renderer = renderer;
                Mesh = mesh;
            }

            public Renderer Renderer { get; }

            public Mesh Mesh { get; }

            // Проверки, которые есть только у этого вида частей; меш уже проверен и есть
            public abstract string FindOwnProblem(SkinnedMeshRenderer sample);

            public abstract Transform[] GetBones();

            public abstract Matrix4x4[] GetBindposes();

            public abstract BoneWeight[] GetBoneWeights();

            public abstract Bounds GetBoundsIn(Transform boundsSpace);

            public abstract void RemoveRenderer();
        }

        private class SkinPart : ModelPart
        {
            private readonly SkinnedMeshRenderer _skin;

            public SkinPart(SkinnedMeshRenderer skin) : base(skin, skin.sharedMesh)
            {
                _skin = skin;
            }

            public override string FindOwnProblem(SkinnedMeshRenderer sample)
            {
                if (Mesh.HasVertexAttribute(VertexAttribute.BlendIndices) == false)
                    return $"у меша {Mesh.name} нет привязки к костям";

                if (_skin.rootBone != sample.rootBone || _skin.transform.parent != sample.transform.parent)
                    return "другой корень костей или родитель";

                if (_skin.quality != sample.quality || _skin.updateWhenOffscreen != sample.updateWhenOffscreen)
                    return "другие настройки отрисовки";

                return null;
            }

            public override Transform[] GetBones() => _skin.bones;

            public override Matrix4x4[] GetBindposes() => Mesh.bindposes;

            // Весов костей нет у кожи на одной кости — у неё только индекс кости, а Mesh.boneWeights отдаёт ей вес 1
            public override BoneWeight[] GetBoneWeights() => Mesh.boneWeights;

            // Корень костей у всей кожи общий, и её границы уже заданы относительно него
            public override Bounds GetBoundsIn(Transform boundsSpace) => _skin.localBounds;

            public override void RemoveRenderer() => GameObject.DestroyImmediate(_skin);
        }

        // Деталь склеивается как кожа на одной кости — родительской: все вершины целиком на ней, а привязка — положение
        // детали относительно кости
        private class RigidPart : ModelPart
        {
            private const float WholeWeight = 1;

            private readonly MeshFilter _meshFilter;

            public RigidPart(MeshRenderer renderer, MeshFilter meshFilter)
                : base(renderer, meshFilter != null ? meshFilter.sharedMesh : null)
            {
                _meshFilter = meshFilter;
            }

            public override string FindOwnProblem(SkinnedMeshRenderer sample) => null;

            public override Transform[] GetBones() => new[] { Renderer.transform.parent };

            public override Matrix4x4[] GetBindposes()
            {
                Transform transform = Renderer.transform;

                return new[] { Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale) };
            }

            public override BoneWeight[] GetBoneWeights()
            {
                BoneWeight[] boneWeights = new BoneWeight[Mesh.vertexCount];

                for (int i = 0; i < boneWeights.Length; i++)
                    boneWeights[i].weight0 = WholeWeight;

                return boneWeights;
            }

            // Углы границ меша переводятся в пространство корня костей по текущей позе
            public override Bounds GetBoundsIn(Transform boundsSpace)
            {
                Matrix4x4 toBoundsSpace = boundsSpace.worldToLocalMatrix * Renderer.transform.localToWorldMatrix;
                Bounds meshBounds = Mesh.bounds;
                Vector3[] extremes = { meshBounds.min, meshBounds.max };
                Bounds bounds = new(toBoundsSpace.MultiplyPoint3x4(meshBounds.center), Vector3.zero);

                foreach (Vector3 x in extremes)
                {
                    foreach (Vector3 y in extremes)
                    {
                        foreach (Vector3 z in extremes)
                            bounds.Encapsulate(toBoundsSpace.MultiplyPoint3x4(new Vector3(x.x, y.y, z.z)));
                    }
                }

                return bounds;
            }

            public override void RemoveRenderer()
            {
                GameObject.DestroyImmediate(Renderer);
                GameObject.DestroyImmediate(_meshFilter);
            }
        }
    }
}
