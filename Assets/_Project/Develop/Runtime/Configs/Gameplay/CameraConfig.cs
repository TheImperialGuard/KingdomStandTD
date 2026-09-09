using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/CameraConfig", fileName = "CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [field: SerializeField, Min(0.1f)] public float MoveSpeed = 5f;
    }
}
