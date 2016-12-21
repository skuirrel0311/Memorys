Shader "Custum/ImageOutLine" {
	// シェーダのプロパティ。必要なら追加する。
	Properties{
		// 入力画像がここにはいる
		_MainTex("Source", 2D) = "white" {}
		_LineColor("LineColor",Color) = (1,1,1,1)
		_Thickness("thickness",Float) = 1.0
	}
		SubShader{
		// 不必要な機能はすべて切る
		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode Off }

		Pass{
		// シェーダプログラムの本体
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
		return o;
	}

		sampler2D _MainTex;
		fixed4 _LineColor;
		fixed _Thickness;

		fixed4 frag(v2f i) : SV_TARGET{
			// ここに表示したい色の計算処理を書く

			float2 pixelx = float2(_Thickness / _ScreenParams.x,0.0f);
			float2 pixely = float2(0.0f, _Thickness /_ScreenParams.y);


			fixed4 colR = tex2D(_MainTex,i.uv+pixelx);
			fixed4 colL = tex2D(_MainTex, i.uv - pixelx);
			fixed4 colU = tex2D(_MainTex, i.uv + pixely);
			fixed4 colD= tex2D(_MainTex, i.uv - pixely);

			fixed4 fragColor = tex2D(_MainTex,i.uv)-((1-_LineColor)*(abs(colR - colL)+(abs(colU-colD))));
			return fragColor;
		}
		ENDCG
	}
	}
		FallBack Off
}
