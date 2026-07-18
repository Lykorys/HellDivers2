sampler uImage0 : register(s0);
float3 uColor;
float uTime;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float alpha = uv.y;
    float pulse = (sin(uTime * 5.0) * 0.2) + 0.8;
    
    return float4(uColor * pulse, alpha * pulse);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};