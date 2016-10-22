Shader "Unlit/customtoon"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_ShadowCon("Shadow",2D) = "white"{}
		_Ramp("Ramp",2D) = "White"{}
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_Threshold("Threshold",Float) = 1.0
		_Strength("ShadowStrength",Float) = 1.0
	}
		SubShader
		{
			Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			//#pragma tessellate tess
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"


			// compile shader into multiple variants, with and without shadows
			// (we don't care about any lightmaps yet, so skip these variants)
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			// shadow helper functions and macros
			#include "AutoLight.cginc"

		struct v2f
		{
			float2 uv : TEXCOORD0;
			SHADOW_COORDS(1) // put shadows data into TEXCOORD1
		    fixed3 diff : COLOR1;
			fixed3 ambient : COLOR0;
			float4 pos : SV_POSITION;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;
			half3 worldNormal = UnityObjectToWorldNormal(v.normal);
			half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
			o.diff = nl * _LightColor0.rgb*0.5f+0.5f;
			o.ambient = ShadeSH9(half4(worldNormal,1));
			// compute shadows data
			TRANSFER_SHADOW(o)
				return o;
		}

		sampler2D _MainTex;
		sampler2D _ShadowCon;
		sampler2D _Ramp;
		fixed4 _Color;
		float _Threshold;
		float _Strength;

		fixed4 frag(v2f i) : SV_Target
		{
		fixed4 col = tex2D(_MainTex, i.uv);
		float diff = tex2D(_Ramp, fixed2(i.diff.x, 0)).r;
		fixed shadow = min(SHADOW_ATTENUATION(i) + (tex2D(_ShadowCon, i.uv).a),1.0f);
		if (shadow < diff)shadow *= diff;
		fixed3 lighting = shadow + i.ambient;
		col.rgb *= lighting*_Color;
		return col;
	}
		ENDCG
	}

	// shadow casting support
	UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}

