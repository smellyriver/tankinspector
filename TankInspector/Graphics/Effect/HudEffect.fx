struct VS_IN
{
	float2 pos : POSITION0;
	float4 color : COLOR0;
};

struct PS_IN
{
    float4 pos : POSITION0;
	float4 color : COLOR0;
};

struct PS_OUT
{
    float4 color : COLOR0;
};


PS_IN VertexShaderFunction(VS_IN input)
{
	PS_IN output = (PS_IN)0;
    output.pos = float4(input.pos,1,1);
	output.color = input.color;
	return output;
}

PS_OUT PixelShaderFunction(VS_IN input)
{
	PS_OUT output = (PS_OUT)0;
	output.color = input.color;
    return output;
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();

		AlphaBlendEnable	= true;
		SrcBlend			= SrcAlpha;
		DestBlend			= InvSrcAlpha;

		 ZEnable = false;
    }
}
