float4x4 viewProj;
float4x4 world;

bool useDiffuse = false;
bool useNormal = false;
bool useSpecular = false;
bool useArmor = false;
bool spacingArmor = false;
bool useNormalPackDXT1 = false;
bool useMetallicDetail = false;
bool useNormalBC1 = true;

int maxAnisotropy = 16;
float armorValue;
float armorChanceToHitByProjectile;

bool alphaTestEnable = false;
float alphaReference;

float4 detailUVTiling;
float detailPower;

float4 diffuseColor = {1.0f, 1.0f, 1.0f, 1.0f};
float4 specularColor = {0.1f, 0.1f, 0.1f, 1.0f};

sampler2D diffuseSampler : register(S0)=
	sampler_state
{
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = maxAnisotropy;
	AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D normalSampler : register(S1)=
	sampler_state
{
	MAGFILTER = ANISOTROPIC;
    MINFILTER = ANISOTROPIC;
    MIPFILTER = ANISOTROPIC;
	MaxAnisotropy = maxAnisotropy;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D specularSampler : register(S2)=
	sampler_state
{
    MAGFILTER = ANISOTROPIC;
    MINFILTER = ANISOTROPIC;
    MIPFILTER = ANISOTROPIC;
	MaxAnisotropy = maxAnisotropy;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D metallicDetailSampler : register(S3)=
	sampler_state
{
    MAGFILTER = ANISOTROPIC;
    MINFILTER = ANISOTROPIC;
    MIPFILTER = ANISOTROPIC;
	MaxAnisotropy = maxAnisotropy;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_IN
{
	float4 pos : POSITION0;
	float3 normal : NORMAL0;
	float2 uv: TEXCOORD0;
	float3 tangent : TANGENT0;
	float3 binormal : BINORMAL0;
};

struct TRACER_VS_IN
{
	float4 pos : POSITION0;
	float color : COLOR0;
};

struct PS_IN
{
    float4 pos : POSITION0;
    float2 uv  : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 pointPosition : TEXCOORD2;
	float3x3 tangentToWorld : TEXCOORD3;
};

struct TRACER_PS_IN
{
	float4 pos : POSITION0;
	float color : COLOR0;
	float3 pointPosition : TEXCOORD0;
};

struct PS_OUT
{
    half4 color : COLOR0;
    half4 normal : COLOR1;
    half4 position : COLOR2;
};


TRACER_PS_IN TracerVertexShaderFunction(TRACER_VS_IN input)
{
	TRACER_PS_IN output = (TRACER_PS_IN)0;
	float4 worldPosition = mul(input.pos, world);
    output.pos = mul(worldPosition, viewProj);
	output.pointPosition = worldPosition;
	output.color = input.color;
	return output;
}



PS_OUT TracerPixelShaderFunction(TRACER_PS_IN input)
{
	PS_OUT output = (PS_OUT)0;

	float3 color;
	color.r = input.color;
	color.g = 0;

	if(useArmor)
	{
		color.b = 0.5;
	}
	else
	{
		color = 0.5;
	}


	float3 specular = 0.0;

	output.color.rgb = color;
	output.color.a = specular.r;

	float3 normal = float3(0,0,-1);

	normal = normalize(normal);
	output.normal.rgb = normal;
	output.normal.a = specular.g;

	output.position.rgb  = input.pointPosition;                  
	output.position.a = specular.b;

    return output;
}

PS_IN VertexShaderFunction(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    float4 worldPosition = mul(input.pos, world);
    output.pos = mul(worldPosition, viewProj);

    output.uv = input.uv;

	output.normal = mul(input.normal,world);

	output.pointPosition = worldPosition;

	if(useNormal)
	{
		float3x3 tangentToObject;
		tangentToObject[0] = input.tangent;
		tangentToObject[1] = input.binormal;
		tangentToObject[2] = input.normal;

		output.tangentToWorld = mul(tangentToObject, world);
	}

    return output;
}

PS_OUT PixelShaderFunction(PS_IN input)
{
    PS_OUT output = (PS_OUT)0;

	float3 color = 0;

	if(useDiffuse)
	{
		
		float4 diffuseColor = tex2D(diffuseSampler,input.uv);            //output Color
		if( alphaTestEnable && diffuseColor.a < alphaReference )
			discard;

		color = diffuseColor.rgb;
	}
	else if(useArmor)
	{
		if(spacingArmor)
		{
			color.g = armorValue;
			color.r = 0;
		}
		else
		{
			color.r = armorValue;
			color.g = 0;
		}
		color.b = spacingArmor ? 1.0 : 0.0;
	}
	else
	{
		color = diffuseColor.rgb;
	}

	float3 specular;
	if(useSpecular)
		specular = tex2D(specularSampler,input.uv);            
	else
		specular = specularColor.rgb;

	if(useMetallicDetail)
	{
		float detail = tex2D(metallicDetailSampler, input.uv * detailUVTiling);
		specular.b = pow((detail-0.5),detailPower) + 0.5;
	}
	else
	{
		specular.g = 0;
	}



	output.color.rgb = color;
	output.color.a = specular.r;

	float3 normal;
	if(useNormal)
    {
		float3 normalMap;
		//We sample the normal map
		if(useNormalBC1)
		{
			float4 normalPackMap = tex2D(normalSampler, input.uv);
			normalPackMap = 2.0f * normalPackMap - 1.0f;
			normalMap = float3(normalPackMap.r, normalPackMap.g, normalPackMap.b);
		}
		else
		{
			float4 normalPackMap = tex2D(normalSampler, input.uv);
			if (alphaTestEnable && normalPackMap.r < alphaReference)
				discard;
			normalPackMap = 2.0f * normalPackMap - 1.0f;
			normalMap = float3(normalPackMap.a, normalPackMap.g, sqrt(1 - normalPackMap.a*normalPackMap.a - normalPackMap.g*normalPackMap.g));
		}

		//normalMap = normalize(normalMap);
		//And use the TangentToWorld matrix to transform the sample to world space
		normal = normalize(mul(normalMap, input.tangentToWorld));
	}
	else
		normal = input.normal;

	normal = normalize(normal);
	output.normal.rgb = normal;
	output.normal.a = specular.g;

	output.position.rgb  = input.pointPosition;                  
	output.position.a = specular.b;

    return output;
}


technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();

		ZEnable = true;        
        StencilEnable = true;
        StencilPass = REPLACE;
        StencilRef = 1;
    }

	pass Pass1
    {
        VertexShader = compile vs_3_0 TracerVertexShaderFunction();
        PixelShader = compile ps_3_0 TracerPixelShaderFunction();

		ZEnable = true;  
        StencilEnable = true;
        StencilPass = REPLACE;
        StencilRef = 1;
    }
}
