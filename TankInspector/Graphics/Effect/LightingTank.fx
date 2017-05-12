float3 cameraPosition = {0,0,-7};
	float specularHardness = 48;

struct Lighting
{
	float3 Diffuse;
	float3 Specular;
};

struct PointLight
{
	float3 position;
	float3 diffuseColor;
	float  diffusePower;
	float3 specularColor;
	float  specularPower;
};

struct DirectionalLight
{
	float3 Direction;
	float3 diffuseColor;
	float  diffusePower;
	float3 specularColor;
	float  specularPower;
};

Lighting GetPointLight( PointLight light, float3 pos3D, float3 viewDir, float3 normal )
{
	Lighting OUT;
	if( light.diffusePower > 0 )
	{
		float3 lightDir = light.position - pos3D; //3D position in space of the surface
			float distance = length( lightDir );
		lightDir = lightDir / distance; // = normalize( lightDir );
		distance = distance * distance; //This line may be optimised using Inverse square root

		//Intensity of the diffuse light. Saturate to keep within the 0-1 range.
		float NdotL = dot( normal, lightDir );
		float intensity = saturate( NdotL );

		// Calculate the diffuse light factoring in light color, power and the attenuation
		OUT.Diffuse = intensity * light.diffuseColor * light.diffusePower / distance; 

		//Calculate the half vector between the light vector and the view vector.
		//This is faster than calculating the actual reflective vector.
		float3 H = normalize( lightDir + viewDir );

			//Intensity of the specular light
			float NdotH = dot( normal, H );
		intensity = pow( saturate( NdotH ), specularHardness );

		//Sum up the specular light factoring
		OUT.Specular = intensity * light.specularColor * light.specularPower / distance; 
	}
	return OUT;
}





Lighting GetDirectionalLight(DirectionalLight light, float3 viewDir, float3 normal )
{
	Lighting OUT;
	if( light.diffusePower > 0 )
	{
		float3 lightDir = normalize(light.Direction);

			//Intensity of the diffuse light. Saturate to keep within the 0-1 range.
			float NdotL = dot( -normal, lightDir );
		float intensity = saturate( NdotL );

		// Calculate the diffuse light factoring in light color, power and the attenuation
		OUT.Diffuse = intensity * light.diffuseColor * light.diffusePower; 

		//Calculate the half vector between the light vector and the view vector.
		//This is faster than calculating the actual reflective vector.
		float3 H = normalize( lightDir + normalize(viewDir) );

			//Intensity of the specular light
			float NdotH = dot(-normal, H );
		intensity = pow( saturate( NdotH ), specularHardness );

		//Sum up the specular light factoring
		OUT.Specular = intensity * light.specularColor * light.specularPower ; 
	}
	return OUT;
}


//PointLight light1 = { float3(0,5,0), float3(1,1,1),40,float3(1,1,1), 40 }; 
DirectionalLight mainLight = {float3(0.12,-1.1,0.55), float3(0.88,0.92,0.94),1.0,float3(1,1,1),0.8};  //main light

DirectionalLight sideLight = {float3(1,-0.5,0.2), float3(1.0,0.95,0.9),0.73,float3(1,1,1),0.1};  //side light

//DirectionalLight sideLight = {float3(1,0,0), float3(1.0,0.95,0.9),3.0,float3(1,1,1),1};  //side light

DirectionalLight fillLight = {float3(-0.5,0.1,0), float3(0.94,1.0,0.93),0.2,float3(1,1,1),0.05};  //fill light

DirectionalLight highLight = {float3(0.05,0.0,0.5), float3(1.0,1.0,1.00),0.4,float3(0.8,0.85,0.93),1.2};  //high light

// texture we are rendering
sampler2D diffuseSampler : register(S0)=
	sampler_state
{
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;

	AddressU = Wrap;
	AddressV = Wrap;
};

sampler2D normalSampler : register(S1)=
	sampler_state
{
	MAGFILTER = Linear;
	MINFILTER = Linear;
	MIPFILTER = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler2D positionSampler : register(S2)=
	sampler_state
{
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler2D randomSampler : register(S3)=
	sampler_state
{
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler1D armorColorSampler : register(S4)=
	sampler_state
{
	AddressU = Clamp;
};
bool useSSAO;
float random_size;  
float g_sample_rad = 0.5;  
float g_intensity = 1.0;  
float g_scale = 1.0;  
float g_bias = 0.3; 
int g_haltonIterations = 32;
const float2 g_halton[32] = 			
{
	float2(-0.353553, 0.612372),
	float2(-0.25, -0.433013),
	float2(0.663414, 0.55667),
	float2(-0.332232, 0.120922),
	float2(0.137281, -0.778559),
	float2(0.106337, 0.603069),
	float2(-0.879002, -0.319931),
	float2(0.191511, -0.160697),
	float2(0.729784, 0.172962),
	float2(-0.383621, 0.406614),
	float2(-0.258521, -0.86352),
	float2(0.258577, 0.34733),
	float2(-0.82355, 0.0962588),
	float2(0.261982, -0.607343),
	float2(-0.0562987, 0.966608),
	float2(-0.147695, -0.0971404),
	float2(0.651341, -0.327115),
	float2(0.47392, 0.238012),
	float2(-0.738474, 0.485702),
	float2(-0.0229837, -0.394616),
	float2(0.320861, 0.74384),
	float2(-0.633068, -0.0739953),
	float2(0.568478, -0.763598),
	float2(-0.0878153, 0.293323),
	float2(-0.528785, -0.560479),
	float2(0.570498, -0.13521),
	float2(0.915797, 0.0711813),
	float2(-0.264538, 0.385706),
	float2(-0.365725, -0.76485),
	float2(0.488794, 0.479406),
	float2(-0.948199, 0.263949),
	float2(0.0311802, -0.121049)			
};  

float3 getPosition(in float2 uv)  
{  
	return tex2D(positionSampler,uv).xyz;  
}  

float2 getRandom(in float2 uv)  
{  
	return normalize(tex2D(randomSampler,uv*8).xy * 2.0f - 1.0f);  
}  

float doAmbientOcclusion(in float2 tcoord,in float2 uv, in float3 p, in float3 cnorm )  
{  
	float3 diff = getPosition(tcoord + uv) - p;  
		const float3 v = normalize(diff);  
	const float d = length(diff)*g_scale;  
	return max(0.0,dot(cnorm,v)-g_bias)*(1.0/(1.0+d))*g_intensity;  
}  

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
	PixelShaderOutput output = (PixelShaderOutput)0;	
	float3 specular;
	float4 outfloat;

	outfloat = tex2D(positionSampler,input.UV);
	float3 position = outfloat.rgb;
		specular.b = outfloat.a;

	outfloat = tex2D(diffuseSampler,input.UV);
	float3 diffuse = outfloat.rgb;
		specular.r = outfloat.a;

	outfloat = tex2D(normalSampler,input.UV);
	float3 normal = outfloat.rgb;
		specular.g = outfloat.a;

	Lighting mainLighting = GetDirectionalLight(mainLight,position-cameraPosition,normal);
	Lighting sideLighting = GetDirectionalLight(sideLight,position-cameraPosition,normal);
	Lighting highLighting = GetDirectionalLight(highLight,position-cameraPosition,normal);
	Lighting fillLighting = GetDirectionalLight(fillLight,position-cameraPosition,normal);


	float  detailFactor = specular.g;
	float  detail = specular.b;

	float3 specularRate = specular.r;

		diffuse = (1 - detailFactor)*diffuse + detailFactor * ( detail - 0.5 );

	float3 main = mainLighting.Diffuse*diffuse + mainLighting.Specular*specularRate ;
		float3 side = sideLighting.Diffuse*diffuse + sideLighting.Specular*specularRate ;
		float3 high = highLighting.Diffuse*diffuse + highLighting.Specular*specularRate ;
		float3 fill = fillLighting.Diffuse*diffuse + fillLighting.Specular*specularRate ;

		float ao = 0.0f;  

	if(useSSAO)
	{
		float2 theta = getRandom(input.UV);
		float rad = g_sample_rad / length(position-cameraPosition);  
		float2x2 rot 	= float2x2(theta.x,theta.y,-theta.y,theta.x);

		for (int j = 0; j < g_haltonIterations; ++j)  
		{  		
			float2 coord1 = mul(rot,g_halton[j])*rad;
			ao += doAmbientOcclusion(input.UV,coord1, position, normal);  
		}   

		ao *= 0.03125;
	}
	output.Color.a = 1;
	output.Color.rgb =  main + side + high + fill - ao ;
	//output.Color.rgb = side - ao;
	//output.Color.rgb = (normal + 1) / 2.0;
	//output.Color.rgb = specularRate;
	return output;
}



float tankThickestArmor;
float tankThinnestArmor;     
float tankThickestSpacingArmor;
float tankThinnestSpacingArmor;

float regularArmorValueSelectionMax;
float regularArmorValueSelectionMin;

float spacingArmorValueSelectionMax;
float spacingArmorValueSelectionMin;


bool hasRegularArmorHintValue;
bool hasSpacingArmorHintValue;

float regularArmorHintValue;
float spacingArmorHintValue;

float highLightValue;

bool useBlackEdge;


float3 ArmorTone(float3 PackedArmor,float2 uv,float3 lookDirection,float3 normal)
{
	float3 outColor;
	float armor;
	if(PackedArmor.b == 0.5)
	{
		armor = PackedArmor.r;
		if(armor < 0.0)
		{
			outColor = float3(0.0,0.7,1.0);
		}
		else
		{
			float armorIndex = (1-saturate (( armor - tankThinnestArmor)  / ( tankThickestArmor - tankThinnestArmor))) * 0.5 ;
			outColor = tex1D(armorColorSampler,armorIndex);
		}
	}
	else if(PackedArmor.b < 0.5)
	{		
		armor = PackedArmor.r;
		if(armor < regularArmorValueSelectionMin || armor > regularArmorValueSelectionMax)
		{
			outColor = 1;
		}
		else
		{
			float armorIndex = (1-saturate (( armor - tankThinnestArmor)  / ( tankThickestArmor - tankThinnestArmor))) * 0.5 ;
			outColor = tex1D(armorColorSampler,armorIndex);
		}

		if(hasRegularArmorHintValue)
		{
			if( abs(armor - regularArmorHintValue) < 0.5 )
			{
				outColor += highLightValue;
			}
		}

		if(useBlackEdge)
		{
			float rad = 0.0005 / length(lookDirection);  

			const float2 vec[8] = {float2(1,0),float2(-1,0),float2(0,1),float2(0,-1),float2(0.707,0.707),float2(-0.707,-0.707),float2(-0.707,0.707),float2(0.707,-0.707)};  
			int iterations = 8;

			for (int j = 0; j < iterations; ++j)  
			{  
				float2 coord = vec[j]*rad;
					float3 packedNearArmor = tex2D(diffuseSampler,coord+uv);
					float nearArmor =  packedNearArmor.r;
				if( nearArmor != armor)
				{
					return 0.12f;
				}
			}   
		}
	}
	else
	{
		armor = PackedArmor.g;

		if(armor < spacingArmorValueSelectionMin || armor > spacingArmorValueSelectionMax)
		{
			outColor = 1;
		}
		else
		{
			float spacingIndex = (1-saturate((armor - tankThinnestSpacingArmor)  / ( tankThickestSpacingArmor - tankThinnestSpacingArmor))) * 0.5 + 0.5;
			outColor = tex1D(armorColorSampler,spacingIndex);
		}

		if (hasSpacingArmorHintValue)
		{
			if( abs(armor - spacingArmorHintValue) < 0.5 )
			{
				outColor += highLightValue;
			}
		}

		if(useBlackEdge)
		{
			float rad = 0.0005 / length(lookDirection);  

			const float2 vec[8] = {float2(1,0),float2(-1,0),float2(0,1),float2(0,-1),float2(0.707,0.707),float2(-0.707,-0.707),float2(-0.707,0.707),float2(0.707,-0.707)};  
			int iterations = 8;

			for (int j = 0; j < iterations; ++j)  
			{  
				float2 coord = vec[j]*rad;
					float3 packedNearSpacingArmor = tex2D(diffuseSampler,coord+uv);
					float nearArmor = packedNearSpacingArmor.g;
				if( nearArmor != armor)
				{
					return 0.12f;
				}
			}   
		}
	}

	return  outColor;
}

PixelShaderOutput ArmorTonePixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;	
	float3 specular;
	float4 outfloat;

	outfloat = tex2D(positionSampler,input.UV);
	float3 position = outfloat.rgb;
		specular.b = outfloat.a;

	outfloat = tex2D(normalSampler,input.UV);
	float3 normal = outfloat.rgb;
		specular.g = outfloat.a;
	float lookDirection = length(position-cameraPosition);
	outfloat = tex2D(diffuseSampler,input.UV);
	float3 diffuse = 0.8*ArmorTone(outfloat.rgb,input.UV,lookDirection,normal); // 装甲模型色彩转换～
		specular.r = outfloat.a;
	Lighting mainLighting = GetDirectionalLight(mainLight,position-cameraPosition,normal);
	Lighting sideLighting = GetDirectionalLight(sideLight,position-cameraPosition,normal);
	Lighting highLighting = GetDirectionalLight(highLight,position-cameraPosition,normal);
	Lighting fillLighting = GetDirectionalLight(fillLight,position-cameraPosition,normal);
	float3 main = mainLighting.Diffuse*diffuse+mainLighting.Specular*specular;
	float3 side = sideLighting.Diffuse*diffuse+sideLighting.Specular*specular;
	float3 high = highLighting.Diffuse*diffuse+highLighting.Specular*specular;
	float3 fill = fillLighting.Diffuse*diffuse+fillLighting.Specular*specular;
	float ao = 0.0f;  
	if(useSSAO)
	{
		float2 theta = getRandom(input.UV);
		float rad = g_sample_rad / length(position-cameraPosition);  
		float2x2 rot 	= float2x2(theta.x,theta.y,-theta.y,theta.x);

		for (int j = 0; j < g_haltonIterations; ++j)  
		{  		
			float2 coord1 = mul(rot,g_halton[j])*rad;
			ao += doAmbientOcclusion(input.UV,coord1, position, normal);  
		}   

		ao *= 0.05;
	}
	output.Color.a = 1;
	output.Color.rgb = diffuse + 0.25 * high - 1.1* ao;

	return output;
}


technique Technique1
{
	pass Pass1
	{	
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();

		ZEnable = false;
		StencilEnable = true;
		StencilPass = KEEP;
		StencilFunc = EQUAL;
		StencilRef = 1;
	}
}

technique Technique2
{
	pass Pass1
	{	
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 ArmorTonePixelShaderFunction();

		ZEnable = false;
		StencilEnable = true;
		StencilPass = KEEP;
		StencilFunc = EQUAL;
		StencilRef = 1;
	}
}