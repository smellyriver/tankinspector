sampler2D diffuse : register(S0)=
	sampler_state
{
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
};

struct VS_IN
{
	float4 pos : POSITION;
	float4 normal : NORMAL;
	float2 uv: TEXCOORD;
};

struct PS_IN
{
	float4 pos : POSITION;
	float4 normal : NORMAL;
	float2 uv: TEXCOORD;
};

float4x4 worldViewProj;

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = mul(input.pos, worldViewProj);
	output.normal = input.normal;
	output.uv = input.uv;
	return output;
}

float4 PS( PS_IN input ) : COLOR
{
	float4 texcolor = tex2D(diffuse,input.uv);
	return texcolor;
}


technique Main {
	pass P0 {
		AlphaBlendEnable = FALSE;
		VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
	}
}
