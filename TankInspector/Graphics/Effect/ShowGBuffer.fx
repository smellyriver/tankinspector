sampler2D gbuffer1 : register(S0);


struct VertexShaderInput
{
    float3 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
	output.UV = input.UV;
    return output;
}
struct PixelShaderOutput
{
    float4 Color : COLOR0;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
	output.Color = tex2D(gbuffer1,input.UV);
	output.Color.a = 1;
    return output;
}

technique Technique1
{
    pass Pass1
    {	
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}