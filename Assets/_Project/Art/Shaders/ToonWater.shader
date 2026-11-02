// Мультяшная поверхность воды: ровный цвет и белые штрихи-барашки.
// Рисунок строится по мировым XZ, поэтому плитки воды сходятся без швов при любом повороте и раскладке.
// Каждый штрих колышется в своей фазе: покачивается на месте, выгибается сильнее и слабее,
// то набирает толщину, то истончается до исчезновения.
// Освещение — как у Lit без бликов: основной свет с тенями плюс окружение.
Shader "Project/ToonWater"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.46, 0.75, 0.91, 1)
        _StrokeColor ("Stroke Color", Color) = (1, 1, 1, 1)
        _StrokeSpacing ("Stroke Spacing", Float) = 0.6
        _StrokeLength ("Stroke Length", Range(0.05, 0.4)) = 0.2
        _StrokeWidth ("Stroke Width", Range(0.005, 0.1)) = 0.03
        _StrokeBend ("Stroke Bend", Range(0, 3)) = 1.4
        _StrokeCoverage ("Stroke Coverage", Range(0, 1)) = 0.75
        _StrokeCycle ("Stroke Cycle (s)", Float) = 5
        _StrokeAlpha ("Stroke Alpha", Range(0, 1)) = 0.85
        _SwayAmplitude ("Sway Amplitude", Range(0, 0.1)) = 0.04
        _SwaySpeed ("Sway Speed", Float) = 1.3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _StrokeColor;
            float _StrokeSpacing;
            float _StrokeLength;
            float _StrokeWidth;
            float _StrokeBend;
            half _StrokeCoverage;
            float _StrokeCycle;
            half _StrokeAlpha;
            float _SwayAmplitude;
            float _SwaySpeed;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "ToonWater"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                half fogFactor : TEXCOORD2;
            };

            float Hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            // Штрих-барашек: по одному на ячейку, ряды сдвинуты, чтобы не читалась сетка.
            // Штрих — дуга вдоль X, часть ячеек пустует.
            float StrokeMask(float2 positionXZ)
            {
                float2 p = positionXZ / _StrokeSpacing;
                float row = floor(p.y);
                p.x += Hash(float2(row, 7.0)) * 3.0;

                float2 cell = floor(p);
                float2 local = frac(p) - 0.5;

                float seed = Hash(cell);
                float seedOffset = Hash(cell + 31.7);

                if (seed > _StrokeCoverage)
                    return 0;

                // Запас под длину и покачивание, чтобы штрих не обрезался границей ячейки
                float2 freeSpace = float2(1 - 2 * _StrokeLength, 0.6) - 2 * _SwayAmplitude;
                float2 center = (float2(seed / max(_StrokeCoverage, 0.001), seedOffset) - 0.5) * max(freeSpace, 0);

                float phase = seedOffset * TWO_PI;
                float swayTime = _Time.y * _SwaySpeed + phase;
                float2 sway = float2(cos(swayTime * 0.7), sin(swayTime)) * _SwayAmplitude;
                float2 q = local - center - sway;

                float pulse = sin(_Time.y * TWO_PI / max(_StrokeCycle, 0.01) + phase) * 0.5 + 0.5;
                float growth = smoothstep(0.2, 0.7, pulse);

                float halfLength = _StrokeLength * lerp(0.6, 1.0, seedOffset) * lerp(0.7, 1.0, growth);
                float bend = _StrokeBend * lerp(0.6, 1.2, sin(swayTime * 1.3) * 0.5 + 0.5);

                // Дуга: середина приподнята, концы опущены; толщина сходит на нет к концам
                float along = saturate(abs(q.x) / halfLength);
                float distanceToArc = abs(q.y + bend * q.x * q.x);
                float width = _StrokeWidth * (1 - along * along) * growth;
                float edge = fwidth(distanceToArc);
                float shape = 1 - smoothstep(width - edge, width + edge, distanceToArc);
                shape *= step(abs(q.x), halfLength);

                return shape * saturate(growth * 2);
            }

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);

                half3 albedo = lerp(_BaseColor.rgb, _StrokeColor.rgb, StrokeMask(input.positionWS.xz) * _StrokeAlpha);

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                half diffuse = saturate(dot(normalWS, mainLight.direction)) * mainLight.shadowAttenuation;
                half3 lighting = mainLight.color * diffuse + SampleSH(normalWS);

                half3 color = MixFog(albedo * lighting, input.fogFactor);
                return half4(color, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            float4 Vertex(float4 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS.xyz);
            }

            half Fragment() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
