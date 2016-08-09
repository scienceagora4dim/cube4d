Shader "Unlit/CubeShader"
{
	Properties
	{
		_CameraPosition ("Camera position", Vector) = (0, 0, 0, 0)
		_CameraDirection ("Camera direction", Vector) = (0, 0, 1, 1)
		_CubePosition ("Cube position", Vector) = (0, 0, 4, 0)
		_ProjectionDistance ("Projection distance", Float) = 16.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Cull Off // ポリゴンを両面レンダリングする。
			Blend SrcAlpha OneMinusSrcAlpha // 一般的なアルファブレンドを行う。

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			/// カメラの位置
			float4 _CameraPosition;

			/// カメラの向き
			float4 _CameraDirection;

			/// 立方体の位置
			float4 _CubePosition;

			/// 投影時の係数
			float _ProjectionDistance;

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

				// 4次元座標を3次元に投影する。
				
				// 立方体の頂点の座標
				float4 position = float4(v.vertex.xyz, v.uv.x) + _CubePosition;

				// カメラから頂点への方向
				float4 r = position;

				// カメラから3次元空間への方向
				float4 n = _CameraDirection;

				// カメラから3次元空間に投影した頂点へのベクトルの係数を求める
				float l = _ProjectionDistance * dot(n, n) / dot(r, n);

				// 3次元空間に投影した頂点座標を求める
				float4 vertex = float4((l * r).xyz, 1);

				// 3次元VP変換
				o.vertex = mul(UNITY_MATRIX_VP, vertex);

				// 頂点色の引継ぎ
				o.color = v.color;
				o.uv = v.uv;
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
