sampler s0;
bool flashOn;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
	{
	float4 colour = tex2D(s0, coords);
	
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
			// colour.rgb = 0;
			}
		}

	return colour;
	}

technique Technique1
	{
	pass Pass1
		{
		PixelShader = compile ps_2_0 PixelShaderFunction();
		}
	}
