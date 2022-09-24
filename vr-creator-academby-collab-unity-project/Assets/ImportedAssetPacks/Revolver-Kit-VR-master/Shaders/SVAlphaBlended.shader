
Shader "SpectacleVR/Particles/Alpha Blended"
{
	Properties
	{
	}

	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off
		Lighting Off
		ZWrite Off
		
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"
				
				//Base
				
				//Soft Particles
				sampler2D_float _CameraDepthTexture;
				
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
				};
				
				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					UNITY_FOG_COORDS(1)
				};
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					
					fixed4 col = i.color;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
