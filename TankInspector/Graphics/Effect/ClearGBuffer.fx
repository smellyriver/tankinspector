struct VertexShaderInput
{
    float3 Position : POSITION0;
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
	output.UV.x = input.Position.x;
	output.UV.y = - input.Position.y;
    return output;
}
struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
    //black color
    output.Color = float4(0.0,0.0,0.0,0.0);
    //when transforming 0.5f into [-1,1], we will get 0.0f
    output.Normal = float4(0.5,0.5,0.5,0.0);
    //max depth
    output.Depth = float4(input.UV.x*100,input.UV.y*100,1000,1.0);
    return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();

		ZEnable = false;        
        StencilEnable = false;
    }
}