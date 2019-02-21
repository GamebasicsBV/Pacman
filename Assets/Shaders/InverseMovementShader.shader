Shader "Hidden/InverseMovementShader" {
	Properties{
		_MainTex("Screen Texture", 2D) = "white" {}
		_MyNormalMap("Normal map", 2D) = "bump" {}
		_ColorMultiplier("Color Multiplier", Float) = 0.1
		_EffectOffset("Effect Offset", Color) = (0, 0, 0, 0)
		_Color("Color Multiplier", Color) = (1, 0, 0, 0)
	}
	SubShader {
		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex, _MyNormalMap;
			float4 _MainTex_TexelSize;
			float _OffsetMultiplier, _ColorMultiplier;
			float4 _Color, _EffectOffset;

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
				fixed4 colorBrightness = _ColorMultiplier * tex2D(_MyNormalMap, i.uv + float2(_EffectOffset.x, _EffectOffset.y)) * _Color;
				fixed4 tex_screen = tex2D(_MainTex, i.uv);
				return tex_screen + colorBrightness;
			}

			ENDCG
		}
	}
}
