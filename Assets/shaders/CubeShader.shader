Shader "Unlit/CubeShader"
{
	Properties
	{
		_MainTex("Main Texture (RGB)", 2D) = "white" {}
		_CubePosition ("Cube position", Vector) = (0, 0, 0, 0)
		_CubeRotation1 ("Cube rotation 1", Vector) = (0, 0, 0, 0)
		_CubeRotation2 ("Cube rotation 2", Vector) = (0, 0, 0, 0)
		_CubeScale ("Cube scale", Vector) = (0, 0, 0, 0)
		_CameraPosition ("Camera position", Vector) = (0, 0, 0, 0)
		_CameraRotation1 ("Camera rotation 1", Vector) = (0, 0, 0, 0)
		_CameraRotation2 ("Camera rotation 2", Vector) = (0, 0, 0, 0)
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

			float4 _CubePosition;
			float4 _CubeRotation1;
			float4 _CubeRotation2;
			float4 _CubeScale;
			float4 _CameraPosition;
			float4 _CameraRotation1;
			float4 _CameraRotation2;
			
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

			/// テクスチャID
			sampler2D _MainTex;

			/// XZ平面回転行列を生成する
			float4x4 makeRotateXZ(float theta)
			{
				float c = cos(theta);
				float s = sin(theta);
				const float4x4 result =
				{
					   c, 0.0f,    s, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					  -s, 0.0f,    c, 0.0f,
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


			// 以下実際の値を代入して、各フレームで一度だけ行う計算を終えておく。
			// 4次元回転行列の生成
			static float4x4 Mxy = makeRotateXY(radians(_CubeRotation1.z));
			static float4x4 Mxz = makeRotateXZ(radians(_CubeRotation1.y));
			static float4x4 Mxw = makeRotateXW(radians(_CubeRotation2.x));
			static float4x4 Myz = makeRotateYZ(radians(_CubeRotation1.x));
			static float4x4 Myw = makeRotateYW(radians(_CubeRotation2.y));
			static float4x4 Mzw = makeRotateZW(radians(_CubeRotation2.z));
			static float4x4 Rotation4D = 
					mul(Mzw, mul(Myw, mul(Myz, mul(Mxw, mul(Mxz, Mxy)))));

			// カメラの4次元回転行列の生成
			static float4x4 Cxy = makeRotateXY(radians(_CameraRotation1.z));
			static float4x4 Cxz = makeRotateXZ(radians(_CameraRotation1.y));
			static float4x4 Cxw = makeRotateXW(radians(_CameraRotation2.x));
			static float4x4 Cyz = makeRotateYZ(radians(_CameraRotation1.x));
			static float4x4 Cyw = makeRotateYW(radians(_CameraRotation2.y));
			static float4x4 Czw = makeRotateZW(radians(_CameraRotation2.z));
			static float4x4 CameraRotation4D = 
					mul(Czw, mul(Cyw, mul(Cyz, mul(Cxw, mul(Cxz, Cxy)))));


			// TODO: ちゃんと5x5の行列として扱う。
			// モデルビュー変換の回転部分
			static float4x4 RotationMV = mul(transpose(CameraRotation4D), Rotation4D);
					
			// モデルビュー変換の平行移動部分
			static float4 TranslationMV = 
					mul(transpose(CameraRotation4D), _CubePosition)-
					mul(transpose(CameraRotation4D), _CameraPosition); 

			// モデル原点の位置、およびそのwバッファ。
			static float4 ModelPosition = mul(RotationMV, _CubePosition)+TranslationMV;
			static float wBaffa = UNITY_MATRIX_P[1][1] * ModelPosition.w;


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
				float2 uv2 : TEXCOORD1;
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
				float2 uv : TEXCOORD1;
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

				float4 vertex = float4(v.vertex.xyz, v.uv.x);
				// w方向の見切れの処理
				// ToDo: もっと良い処理方法を考える
				if(wBaffa<=-3||wBaffa>=3) vertex=float4(0,0,0,0);

				// 4次元のモデル・ビュー変換を行う。
				vertex = mul(RotationMV, vertex)+TranslationMV;

				// w座標を単に落とすことで3次元に射影。
				vertex.w = 1;
				// 右手系→左手系
				vertex.z = -vertex.z;

				// 3次元投影変換（4次元投影変換→1次元分射影とするのと結果は同じ）
				o.vertex = mul(UNITY_MATRIX_P, vertex);

				// 頂点色の引継ぎ
				o.color = v.color;
				o.uv = v.uv2;
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
				return tex2D(_MainTex, i.uv.xy);
			}
			ENDCG
		}
	}
}
