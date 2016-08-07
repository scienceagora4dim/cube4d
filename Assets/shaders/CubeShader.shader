Shader "Unlit/CubeShader"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			/**
			 *	頂点データ構造体
			 *
			 *	スクリプトで設定したvertices・colors・uvの値が設定される。
			 */
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR0;
				float2 uv : TEXCOORD0;
			};

			/**
			 *	頂点シェーダーからフラグメントシェーダーに渡す中間データの構造体
			 *
			 *	フラグメントシェーダーでは、頂点からの距離に応じて補完された値が設定される。
			 */
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
				float2 uv : TEXCOORD0;
			};

			/**
			 *	頂点シェーダー
			 *
			 *	スクリプトで設定したvertices・colors・uvの値が渡される。
			 *	頂点毎に呼び出される。
			 *	頂点を座標変換し、フラグメントシェーダーに渡す。
			 */
			v2f vert (appdata v)
			{
				v2f o;

				// TODO: ここで4次元座標を3次元に投影する。
				o.uv = v.uv;

				// 通常の3次元MVP変換
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				// 頂点色の引継ぎ
				o.color = v.color;
				return o;
			}
			
			/**
			 *	フラグメントシェーダー
			 *
			 *	頂点シェーダーの出力結果をもとに、画素毎の色の計算を行う。
			 *	頂点シェーダーの出力結果を画素毎に線型補完した値が渡される。
			 */
			float4 frag (v2f i) : SV_Target
			{
				// 頂点色をそのまま返す。
				return i.color;
			}
			ENDCG
		}
	}
}
