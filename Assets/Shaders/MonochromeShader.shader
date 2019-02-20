Shader "Hidden/MonochromeShader" {
	Properties{
		_MainTex("Screen Texture", 2D) = "white" {}
	}
		SubShader{
			Pass {

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_TexelSize, _XOffsets, _YOffsets;
				float _OffsetMultiplier;

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv     : TEXCOORD0;
				};

				v2f vert(appdata_img v) {
					v2f o;
					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
							v.texcoord.y = 1 - v.texcoord.y;
					#endif

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;

					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 c = tex2D(_MainTex, i.uv);
				float avg = (c.x + c.y + c.z) / 3;
					fixed4 tex_screen = fixed4(avg, avg, avg, 1);
					return tex_screen;
				}

				ENDCG
			}
		}
}
