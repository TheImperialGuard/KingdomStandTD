// Wind for Kenney flag-pennant: the pole runs along object Y, the cloth spreads towards -Z.
// Vertices bend sideways (object X) by a wave running from the pole to the tip.
#ifndef MENU_BACKDROP_FLAG_WIND_INCLUDED
#define MENU_BACKDROP_FLAG_WIND_INCLUDED

#define FLAG_WIND_POLE_RADIUS 0.05
#define FLAG_WIND_CLOTH_LENGTH 0.47
#define FLAG_WIND_AMPLITUDE 0.06
#define FLAG_WIND_LIFT_SHARE 0.25
#define FLAG_WIND_SPEED 6.0
#define FLAG_WIND_LIFT_SPEED_SHARE 1.3
#define FLAG_WIND_WAVE_NUMBER 9.0
#define FLAG_WIND_GUST_SPEED 0.7
#define FLAG_WIND_GUST_SHARE 0.25

float3 ApplyFlagWind(float3 positionOS)
{
    float distanceFromPole = -positionOS.z - FLAG_WIND_POLE_RADIUS;
    float weight = saturate(distanceFromPole / FLAG_WIND_CLOTH_LENGTH);

    float4x4 objectToWorld = GetObjectToWorldMatrix();
    float3 origin = float3(objectToWorld._m03, objectToWorld._m13, objectToWorld._m23);
    float phase = dot(origin, float3(0.37, 0.0, 0.61));

    float time = _Time.y;
    float gust = 1.0 - FLAG_WIND_GUST_SHARE + FLAG_WIND_GUST_SHARE * sin(time * FLAG_WIND_GUST_SPEED + phase);
    float wave = sin(time * FLAG_WIND_SPEED + phase - distanceFromPole * FLAG_WIND_WAVE_NUMBER);
    float lift = sin(time * FLAG_WIND_SPEED * FLAG_WIND_LIFT_SPEED_SHARE + phase - distanceFromPole * FLAG_WIND_WAVE_NUMBER);

    positionOS.x += wave * FLAG_WIND_AMPLITUDE * weight * gust;
    positionOS.y += lift * FLAG_WIND_AMPLITUDE * FLAG_WIND_LIFT_SHARE * weight * weight * gust;
    return positionOS;
}

#define FLAG_WIND_VERTEX(VertexFunction, PositionField) \
    Varyings FlagWind##VertexFunction(Attributes input) \
    { \
        UNITY_SETUP_INSTANCE_ID(input); \
        input.PositionField.xyz = ApplyFlagWind(input.PositionField.xyz); \
        return VertexFunction(input); \
    }

#endif
