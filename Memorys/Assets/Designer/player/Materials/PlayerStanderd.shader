Shader "Custom/PlayerStanderd" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_HideColor("HideColor",Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("NormalMap", 2D) = "white" {}
		_HachingTex("Haching",2D) = "white"{}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
SubShader{
		Tags { "RenderType" = "Transparent""Queue" = "Transparent+1" }
		LOD 200
		Zwrite Off
		Ztest Greater
		 Lighting Off
		Offset  0,-1
	CGPROGRAM
		#pragma surface surf Lambert alpha
		#pragma target 3.0

		sampler2D _HachingTex;
		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 screenPos;
			float3 viewDir;
		};
		fixed4  _HideColor;
		void surf(Input IN, inout SurfaceOutput o) 
		{
			//fixed4 c = tex2D(_MainTex, IN.uv_MainTex)*0.1f;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));		
			//half rim = 1.0 - saturate(dot(normalize(IN.viewDir)*2.0f, o.Normal));
			//o.Alpha = rim;
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV *= float2(4+ cos(floor(_Time.x*120.0f)), 3+sin(floor(_Time.x*120.0f)));

			o.Alpha = 1.0f;
			o.Emission = tex2D(_HachingTex, screenUV).rgb *_HideColor;
		}
	ENDCG
		Zwrite on
		Ztest LEqual
	CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metallic*(c.a*0.2);
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}	
	ENDCG
}
			FallBack "Diffuse"
}
