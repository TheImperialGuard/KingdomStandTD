// Корка инея поверх модели: рисуется вторым материалом того же меша, родной материал остаётся под ней.
// Граница корки идёт по высоте в мире от _FrostBaseY на _FrostHeight вверх и рвётся шумом:
// _FrostAmount = 0 — инея нет, 1 — покрыта вся модель. Рост и таяние — один и тот же параметр,
// поэтому корка нарастает снизу вверх и сходит сверху вниз.
Shader "Project/TowerFrost"
{
    Properties
    {
        _FrostColor ("Frost Color", Color) = (0.82, 0.93, 1, 1)
        _FrostAlpha ("Frost Alpha", Range(0, 1)) = 0.6
        _RimColor ("Rim Color", Color) = (0.55, 0.85, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.5
        _RimAlpha ("Rim Alpha", Range(0, 1)) = 0.35
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0.001, 0.3)) = 0.06
        _NoiseScale ("Noise Scale", Float) = 6
        _NoiseAmplitude ("Noise Amplitude", Range(0, 0.5)) = 0.15
        _FrostSoftness ("Frost Softness", Range(0.001, 0.2)) = 0.02
        _Ambient ("Ambient", Range(0, 1)) = 0.55
        [HideInInspector] _FrostAmount ("Frost Amount", Range(0, 1)) = 0
        [HideInInspector] _FrostBaseY ("Frost Base Y", Float) = 0
        [HideInInspector] _FrostHeight ("Frost Height", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "TowerFrost"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _FrostColor;
                half _FrostAlpha;
                half4 _RimColor;
                half _RimPower;
                half _RimAlpha;
                half4 _EdgeColor;
                half _EdgeWidth;
                float _NoiseScale;
                half _NoiseAmplitude;
                half _FrostSoftness;
                half _Ambient;
                half _FrostAmount;
                float _FrostBaseY;
                float _FrostHeight;
            CBUFFER_END

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
            };

            float Hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            // Сглаженный шум по решётке: 0..1
            float ValueNoise(float3 p)
            {
                float3 cell = floor(p);
                float3 t = frac(p);
                t = t * t * (3 - 2 * t);

                return lerp(
                    lerp(lerp(Hash(cell), Hash(cell + float3(1, 0, 0)), t.x),
                         lerp(Hash(cell + float3(0, 1, 0)), Hash(cell + float3(1, 1, 0)), t.x), t.y),
                    lerp(lerp(Hash(cell + float3(0, 0, 1)), Hash(cell + float3(1, 0, 1)), t.x),
                         lerp(Hash(cell + float3(0, 1, 1)), Hash(cell + float3(1, 1, 1)), t.x), t.y),
                    t.z);
            }

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));

                // Высота точки в долях модели, сдвинутая шумом: граница корки получается рваной
                float height = (input.positionWS.y - _FrostBaseY) / max(_FrostHeight, 0.001);
                float noise = ValueNoise(input.positionWS * _NoiseScale) * 2 - 1;
                float noisyHeight = height + noise * _NoiseAmplitude;

                // Фронт растягивается на запас шума и кромки, чтобы 0 и 1 гарантированно давали «ничего» и «всё»
                float reserve = _NoiseAmplitude + _EdgeWidth + _FrostSoftness;
                float frontier = lerp(-reserve, 1 + reserve, _FrostAmount);
                float depth = frontier - noisyHeight;

                float coverage = smoothstep(0, _FrostSoftness, depth);
                float edge = coverage * (1 - smoothstep(0, _EdgeWidth, depth - _FrostSoftness));

                Light mainLight = GetMainLight();
                half diffuse = _Ambient + (1 - _Ambient) * saturate(dot(normalWS, mainLight.direction));
                half rim = pow(1 - saturate(dot(normalWS, viewDirWS)), _RimPower);

                half3 color = _FrostColor.rgb * diffuse;
                color = lerp(color, _RimColor.rgb, rim);
                color = lerp(color, _EdgeColor.rgb, edge);

                half alpha = saturate(_FrostAlpha + rim * _RimAlpha + edge) * coverage;

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
