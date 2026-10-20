// Круг дальности башни на плоском квадрате, лежащем на земле. Текущая дальность — заливка и сплошная граница; прирост
// до следующего уровня — более яркое кольцо и пунктирная внешняя граница. Обе границы окружены мягкой коричневой
// кромкой. Радиусы, толщины и длина штрихов — в мировых единицах, поэтому штрихи одной длины на любом круге.
// Смешивание premultiplied.
Shader "Project/RangeZone"
{
    Properties
    {
        _FillColor ("Fill Color", Color) = (0.45, 0.95, 0.35, 0.06)
        _GrowthFillColor ("Growth Fill Color", Color) = (0.45, 0.95, 0.35, 0.18)
        _LineColor ("Line Color", Color) = (0.78, 1, 0.6, 0.85)
        _RimColor ("Rim Color", Color) = (0.435, 0.212, 0.075, 0.35)
        _LineWidth ("Line Width", Float) = 0.034
        _RimWidth ("Rim Width", Float) = 0.023
        _DashLength ("Dash Length", Float) = 0.1
        _GapLength ("Gap Length", Float) = 0.1
        [HideInInspector] _Radius ("Radius", Float) = 1
        [HideInInspector] _OuterRadius ("Outer Radius", Float) = 1
        [HideInInspector] _Opacity ("Opacity", Range(0, 1)) = 0
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
            Name "RangeZone"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Наименьшее расстояние до центра, на которое делится при пересчёте сглаживания в доли штриха
            #define MIN_DISTANCE 0.001

            CBUFFER_START(UnityPerMaterial)
                half4 _FillColor;
                half4 _GrowthFillColor;
                half4 _LineColor;
                half4 _RimColor;
                float _LineWidth;
                float _RimWidth;
                float _DashLength;
                float _GapLength;
                float _Radius;
                float _OuterRadius;
                half _Opacity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                // Смещение точки от центра объекта по земле
                float2 offsetWS : TEXCOORD0;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 centerWS = TransformObjectToWorld(float3(0, 0, 0));

                output.positionHCS = TransformWorldToHClip(positionWS);
                output.offsetWS = positionWS.xz - centerWS.xz;
                return output;
            }

            // Доля пикселя внутри кольца [from, to]; aa — размер пикселя в мировых единицах
            float Band(float centerDistance, float from, float to, float aa)
            {
                return saturate((centerDistance - from) / aa + 0.5) * saturate((to - centerDistance) / aa + 0.5);
            }

            // Накладывает цвет с покрытием coverage поверх premultiplied-цвета
            half4 Over(half4 destination, half4 color, float coverage)
            {
                half alpha = color.a * coverage;
                return half4(color.rgb * alpha + destination.rgb * (1 - alpha), alpha + destination.a * (1 - alpha));
            }

            // 1 на штрихе, 0 в промежутке. Число штрихов целое, поэтому круг замыкается без обрубка
            float Dash(float2 offset, float centerDistance, float radius, float aa)
            {
                float period = _DashLength + _GapLength;
                float count = max(1, round(TWO_PI * radius / period));
                float phase = frac(atan2(offset.y, offset.x) / TWO_PI * count);
                float dashFraction = _DashLength / period;
                float aaPhase = aa * count / (TWO_PI * max(centerDistance, MIN_DISTANCE));

                return saturate(phase / aaPhase + 0.5) * saturate((dashFraction - phase) / aaPhase + 0.5);
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float centerDistance = length(input.offsetWS);
                float aa = max(fwidth(centerDistance), MIN_DISTANCE);

                // Прирост проявляется, пока внешняя граница отходит от текущей на толщину линии
                float growth = saturate((_OuterRadius - _Radius) / _LineWidth);
                float lineInner = _Radius - _LineWidth;
                float outerLineInner = _OuterRadius - _LineWidth;

                half4 color = 0;
                color = Over(color, _FillColor, saturate((_Radius - centerDistance) / aa + 0.5));
                color = Over(color, _GrowthFillColor, Band(centerDistance, _Radius, _OuterRadius, aa) * growth);

                color = Over(color, _RimColor, Band(centerDistance, lineInner - _RimWidth, lineInner, aa));
                color = Over(color, _RimColor, Band(centerDistance, _Radius, _Radius + _RimWidth, aa));
                color = Over(color, _LineColor, Band(centerDistance, lineInner, _Radius, aa));

                color = Over(color, _RimColor, Band(centerDistance, outerLineInner - _RimWidth, outerLineInner, aa) * growth);
                color = Over(color, _RimColor, Band(centerDistance, _OuterRadius, _OuterRadius + _RimWidth, aa) * growth);
                float dash = Dash(input.offsetWS, centerDistance, _OuterRadius, aa);
                color = Over(color, _LineColor, Band(centerDistance, outerLineInner, _OuterRadius, aa) * dash * growth);

                return color * _Opacity;
            }
            ENDHLSL
        }
    }
}
