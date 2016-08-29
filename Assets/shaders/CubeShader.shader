Shader "Unlit/CubeShader"
{
	Properties
	{
		_CubeRotation1 ("Cube rotation 1", Vector) = (0, 0, 0, 0)
		_CubeRotation2 ("Cube rotation 2", Vector) = (1, 0, 0, 0)
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

			/// 回転
			float4 _CubeRotation1;
			float4 _CubeRotation2;

			/// 投影用の行列
			float4x4 makeProjection()
			{
				// XYZ各座標にWの値を加算する。
				// Wは0にする。
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f, 1.0f, 0.0f,
					0.0f, 0.0f, 0.0f, 0.0f, // この行はなんでもよい
				};
				return result;
			}

			/// XY平面回転行列を生成する
			float4x4 makeRotateXY(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					   c,   -s, 0.0f, 0.0f,
					   s,    c, 0.0f, 0.0f,
					0.0f, 0.0f, 1.0f, 0.0f,
					0.0f, 0.0f, 0.0f, 1.0f,
				};
				return result;
			}

			/// XZ平面回転行列を生成する
			float4x4 makeRotateXZ(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					   c, 0.0f,   -s, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					   s, 0.0f,    c, 0.0f,
					0.0f, 0.0f, 0.0f, 1.0f,
				};
				return result;
			}

			/// XW平面回転行列を生成する
			float4x4 makeRotateXW(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					   c, 0.0f, 0.0f,   -s,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f, 1.0f, 0.0f,
					   s, 0.0f, 0.0f,    c,
				};
				return result;
			}

			/// YZ平面回転行列を生成する
			float4x4 makeRotateYZ(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f,    c,   -s, 0.0f,
					0.0f,    s,    c, 0.0f,
					0.0f, 0.0f, 0.0f, 1.0f,
				};
				return result;
			}

			/// YW平面回転行列を生成する
			float4x4 makeRotateYW(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f,    c, 0.0f,   -s,
					0.0f, 0.0f, 1.0f, 0.0f,
					0.0f,    s, 0.0f,    c,
				};
				return result;
			}

			/// ZW平面回転行列を生成する
			float4x4 makeRotateZW(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f,    c,   -s,
					0.0f, 0.0f,    s,    c,
				};
				return result;
			}

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
			};

			/**
			 *	頂点シェーダー
			 *
			 *	スクリプトで設定したvertices・colors・uvの値が渡される。
			 *	頂点毎に呼び出される。
			 *	頂点を座標変換し、フラグメントシェーダーに渡す。
			 */
			v2f vert(appdata v)
			{
				v2f o;

				// 4次元回転行列
				float4x4 Rxy = makeRotateXY(radians(_CubeRotation1.x));
				float4x4 Rxz = makeRotateXZ(radians(_CubeRotation1.y));
				float4x4 Rxw = makeRotateXW(radians(_CubeRotation1.z));
				float4x4 Ryz = makeRotateYZ(radians(_CubeRotation2.x));
				float4x4 Ryw = makeRotateYW(radians(_CubeRotation2.y));
				float4x4 Rzw = makeRotateZW(radians(_CubeRotation2.z));

				// 立方体の頂点の座標を回転する
				float4 vertex = float4(v.vertex.xyz, v.uv.x);
				vertex = mul(Rxy, mul(Rxz, mul(Rxw, mul(Ryz, mul(Ryw, mul(Rzw, vertex))))));

				// 投影を行う。
				float4x4 p = makeProjection();
				vertex = mul(p, vertex);
				vertex.w = 1;

				// 3次元MVP変換
				o.vertex = mul(UNITY_MATRIX_MVP, vertex);

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
			float4 frag(v2f i) : SV_Target
			{
				// 頂点色をそのまま返す。
				return i.color;
			}
			ENDCG
		}
	}
}
