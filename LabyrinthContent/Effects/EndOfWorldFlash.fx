Texture2D SpriteTexture;
sampler s0;
bool flashOn;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
	{
	float4 colour = tex2D(s0, input.TextureCoordinates);
	
	if (colour.a > 0)
		{
		if (colour.r == 0 && colour.g == 0 && colour.b == 0)
			{
			// change black to yellow/blue
			if (flashOn)
				colour.rg = 1;
			else
				colour.b = 1;
			}
		else if (colour.r == 0 && colour.g == 0 && colour.b == 1)
			{
			// change blue to black
			colour.rgb = 0;
			}
		}

	return colour;
	}

technique Technique1
	{
	pass Pass1
		{
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
		}
	}
