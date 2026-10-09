// Круглая зона на плоском квадрате: полупрозрачная заливка, волны от центра к краю и кант по радиусу.
// Смешивание premultiplied: _Additive = 1 даёт чистое сложение цветов, 0 — обычную прозрачность.
Shader "Project/WarCryZone"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.165, 0.102, 1)
        _Additive ("Additive", Range(0, 1)) = 1
        _Fill ("Fill Alpha", Range(0, 1)) = 0.18
        _WaveCount ("Wave Count", Float) = 3
        _WaveSpeed ("Wave Speed", Float) = 0.6
        _WaveAlpha ("Wave Alpha", Range(0, 1)) = 0.45
        _WaveWidth ("Wave Width", Range(0.01, 1)) = 0.3
        _RimAlpha ("Rim Alpha", Range(0, 1)) = 0.75
        _RimWidth ("Rim Width", Range(0.001, 0.5)) = 0.04
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.015
        [HideInInspector] _Opacity ("Opacity", Range(0, 1)) = 0
        [HideInInspector] _StartTime ("Start Time", Float) = 0
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
            Name "WarCryZone"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Доля ширины волны, на которой она достигает пика: передний фронт короче заднего
            #define WAVE_PEAK_POSITION 0.35
            // Доля радиуса, до которой волна набирает яркость: в центре она только зарождается
            #define WAVE_GROW_RADIUS 0.35
            // Кант нарастает от (1 − RIM_RAMP_START × ширина) до (1 − ширина) радиуса
            #define RIM_RAMP_START 2.0

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _Additive;
                half _Fill;
                float _WaveCount;
                float _WaveSpeed;
                half _WaveAlpha;
                half _WaveWidth;
                half _RimAlpha;
                half _RimWidth;
                half _EdgeSoftness;
                half _Opacity;
                float _StartTime;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                // 0 в центре, 1 на радиусе; за радиусом edgeSoft = 0 и зона прозрачна
                float r = length(input.uv * 2 - 1);
                float time = _Time.y - _StartTime;
                float edgeSoft = 1 - smoothstep(1 - _EdgeSoftness, 1, r);

                float alpha = _Fill * edgeSoft;

                // Фаза растёт к центру и сдвигается со временем, поэтому волны рождаются в центре и бегут к краю
                float phase = frac(r * _WaveCount - time * _WaveSpeed);
                float peak = _WaveWidth * WAVE_PEAK_POSITION;
                float ring = smoothstep(0, peak, phase) * (1 - smoothstep(peak, _WaveWidth, phase));
                alpha += _WaveAlpha * ring * smoothstep(0, WAVE_GROW_RADIUS, r) * edgeSoft;

                alpha += _RimAlpha * smoothstep(1 - _RimWidth * RIM_RAMP_START, 1 - _RimWidth, r) * edgeSoft;
                alpha = saturate(alpha) * _Opacity;

                return half4(_Color.rgb * alpha, alpha * (1 - _Additive));
            }
            ENDHLSL
        }
    }
}
