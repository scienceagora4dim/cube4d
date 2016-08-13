Shader "Unlit/CubeShader"
{
	Properties
	{
		_CubeRotation ("Cube rotation", Vector) = (0, 0, 0, 0)
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

			/// 立方体のW軸が関わる回転
			float4 _CubeRotation;

			/// 投影用の行列
			float4x4 makeProjection()
			{
				// XYZ各座標にWの値を加算する。
				// Wは0にする。
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 1.0f,
					0.0f, 1.0f, 0.0f, 1.0f,
					0.0f, 0.0f, 1.0f, 1.0f,
					0.0f, 0.0f, 0.0f, 0.0f,
				};
				return result;
			}

			/// WX平面回転行列を生成する
			float4x4 makeRotateWX(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					   c, 0.0f, 0.0f,    s,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f, 1.0f, 0.0f,
					  -s, 0.0f, 0.0f,    c,
				};
				return result;
			}

			/// WY平面回転行列を生成する
			float4x4 makeRotateWY(float theta)
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

			/// WZ平面回転行列を生成する
			float4x4 makeRotateWZ(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f, c,    -s,
					0.0f, 0.0f, s,    c,
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
				float4x4 wx = makeRotateWX(radians(_CubeRotation.x));
				float4x4 wy = makeRotateWY(radians(_CubeRotation.y));
				float4x4 wz = makeRotateWZ(radians(_CubeRotation.z));

				// 立方体の頂点の座標を回転する(W軸が関わるものだけ)
				float4 vertex = float4(v.vertex.xyz, v.uv.x);
				vertex = mul(wz, mul(wy, mul(wx, vertex)));

				// W軸の値をxyzに加算して、W軸はつぶすことで投影を行う。
				// TODO: もっと別の方法を考える
				float4x4 p = makeProjection();
				vertex = mul(p, vertex);
				vertex.w = 1;

				// 3次元MVP変換(通常の3次元回転はこちらで行う)
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
