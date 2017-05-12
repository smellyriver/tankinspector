struct VertexShaderInput
{
    float Position : POSITION;
	float4 Color : COLOR;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Color : COLOR;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,0,0.5,1);
	output.Color = input.Color;
    return output;
}
struct PixelShaderOutput
{
    float4 Color : COLOR0;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
	output.Color = input.Color;
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