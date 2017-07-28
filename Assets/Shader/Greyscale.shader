 Shader "Custom/Greyscale" {
 	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;

	struct v2f{
		float4 pos:SV_POSITION;
		float2 uv:TEXCOORD;
	};

	v2f vert(appdata_base i){
		v2f o;
		o.pos = UnityObjectToClipPos(i.vertex);
		o.uv = i.texcoord.xy;
		return o;
	}

	float4 frag(v2f i):COLOR{
		float4 c = tex2D(_MainTex,i.uv);
		c.rgb = dot(c.rgb, half3(0.3, 0.59, 0.11));

		
		return c;
	}

	ENDCG
	
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	} 
FallBack "Diffuse"
}
