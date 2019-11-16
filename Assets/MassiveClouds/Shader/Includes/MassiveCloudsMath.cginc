#ifndef MASSIVE_CLOUDS_MATH_INCLUDED
#define MASSIVE_CLOUDS_MATH_INCLUDED

float4 mulQuaternion(float4 q1, float4 q2) {
    return float4(
        q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
        q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
        q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
        q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
    );
}

float3 rotate(float4 q, float3 v) {
    float4 qv = mulQuaternion(float4(-q.x, -q.y, -q.z, q.w), float4(v, 0.0));
    return mulQuaternion(qv, q).xyz;
}

float lamp(float v, float f)
{
    float a = step(v, 0.5) * v;
    float b = step(0.5, v) * v;
    
    a = a * 2;
    b = 1 - (b - 0.5) * 2;
    
    f = 1 + f;
    a = pow(a, f);
    b = pow(b, f);

    a = a / 2;
    b = (1 - b) / 2 + 0.5;

    return a * step(a, 0.5)
        + b * step(0.5, b);
}

#endif