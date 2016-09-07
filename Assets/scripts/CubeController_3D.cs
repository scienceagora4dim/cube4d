using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 立方体コントローラークラス
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class CubeController_3D : MonoBehaviour
{
    /// <summary>
    /// 面のアルファ値
    /// </summary>
    private const float ALPHA = 0.3f;

    /// <summary>
    /// 線のみ描画するかどうかのフラグ
    /// </summary>
    [SerializeField, Tooltip("線のみ描画するかどうか")]
    private bool lines;
   
    [SerializeField, Tooltip("親オブジェクト")]
	public Transform_4D parent;

    [SerializeField, Tooltip("カメラオブジェクト")]
	public Transform_4D eye;

    /// <summary>
    /// W軸関連の回転角度
    /// </summary>
    //[SerializeField, Tooltip("W軸関連の回転角度")]
    //public Vector3 wAngles;

    // 生成されたメッシュオブジェクト
    private Mesh mesh_;

    // メッシュ描画設定
    private MeshRenderer meshRenderer_;

	// プロパティID
    // モデル原点のワールド座標
	private int cubePositionId_;
    // モデルの向きを定める回転角度
    private int cubeRotation1Id_;
    private int cubeRotation2Id_;
    // モデルの拡大率
	private int cubeScaleId_;
	// カメラのワールド座標
    private int cameraPositionId_;
	// カメラの向きを定める回転角度
    private int cameraRotation1Id_;
    private int cameraRotation2Id_;

    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    void Awake()
    {
        meshRenderer_ = GetComponent<MeshRenderer>();

        cubePositionId_ = Shader.PropertyToID("_CubePosition");
        cubeRotation1Id_ = Shader.PropertyToID("_CubeRotation1");
        cubeRotation2Id_ = Shader.PropertyToID("_CubeRotation2");
        cubeScaleId_ = Shader.PropertyToID("_CubeScale");
        cameraPositionId_ = Shader.PropertyToID("_CameraPosition");
        cameraRotation1Id_ = Shader.PropertyToID("_CameraRotation1");
        cameraRotation2Id_ = Shader.PropertyToID("_CameraRotation2");
 
        Vector4[] vertices = new Vector4[]
        {
            vec(-0.5f,  0.5f, -0.5f, 0.0f), //0
            vec( 0.5f,  0.5f, -0.5f, 0.0f), //1
            vec( 0.5f, -0.5f, -0.5f, 0.0f), //2
            vec(-0.5f, -0.5f, -0.5f, 0.0f), //3
            vec(-0.5f,  0.5f,  0.5f, 0.0f), //4
            vec( 0.5f,  0.5f,  0.5f, 0.0f), //5
            vec( 0.5f, -0.5f,  0.5f, 0.0f), //6
            vec(-0.5f, -0.5f,  0.5f, 0.0f), //7
        };

        mesh_ = new Mesh();

        // 頂点の設定(4次元座標の3次元分だけ設定)
        mesh_.vertices = vertices.Select(v => new Vector3(v.x, v.y, v.z)).ToArray();

        // 4つ目の座標軸の点の設定
        // UV座標で代用する。ここでのuの値がxyzwのwになる。
        mesh_.uv = vertices.Select(v => new Vector2(v.w, 0.0f)).ToArray();

        // 頂点色の設定
        mesh_.colors = new Color[]
        {
            rgba(1.0f, 0.0f, 0.0f, ALPHA), //red
            rgba(0.0f, 1.0f, 0.0f, ALPHA), //green
            rgba(0.0f, 0.0f, 1.0f, ALPHA), //blue
            rgba(1.0f, 1.0f, 0.0f, ALPHA), //yellow
            rgba(0.0f, 1.0f, 1.0f, ALPHA), //skyblue
            rgba(1.0f, 0.0f, 1.0f, ALPHA), //magenta
            rgba(1.0f, 1.0f, 1.0f, ALPHA), //white
            rgba(0.0f, 0.0f, 0.0f, ALPHA), //black
        };
        
        if(lines)
        {
            // 輪郭線の定義
            mesh_.SetIndices(new int[] {
                0, 1,
                1, 2,
                2, 3,
                3, 0,

                4, 5,
                5, 6,
                6, 7,
                7, 4,

                0, 4,
                1, 5,
                2, 6,
                3, 7,
            }, MeshTopology.Lines, 0);
        }
        else
        {
            // 面(三角形分割)の設定
            // 時計回り方向が法線方向
            mesh_.triangles = new int[]
            {
            // 手前の面
            0, 1, 2,
            0, 2, 3,
            
            // 上面
            0, 4, 5,
            0, 5, 1,

            // 下面
            3, 6, 7,
            3, 2, 6,

            // 左面
            0, 7, 4,
            0, 3, 7,

            // 右面
            1, 5, 6,
            1, 6, 2,

            // 奥の面
            4, 7, 6,
            4, 6, 5,
            };

            mesh_.RecalculateNormals();
        }
        mesh_.RecalculateBounds();
    }

    /// <summary>
    /// シーン開始時の処理
    /// </summary>
	void Start ()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh_;

        // ダミーオブジェクトを非表示にする		
		transform.FindChild("Dummy").gameObject.SetActive(false);
    }
	
    /// <summary>
    /// 毎フレームの処理
    /// </summary>
	void Update ()
    {
        meshRenderer_.material.SetVector(cubePositionId_, parent.Position);
        // Vector4 でないので以下は少し変
		meshRenderer_.material.SetVector(cubeRotation1Id_, parent.Rotation1);
        meshRenderer_.material.SetVector(cubeRotation2Id_, parent.Rotation2);
        meshRenderer_.material.SetVector(cubeScaleId_, parent.Scale);
        meshRenderer_.material.SetVector(cameraPositionId_, eye.Position);
        // Vector4 でないので以下は少し変
		meshRenderer_.material.SetVector(cameraRotation1Id_, eye.Rotation1);
        meshRenderer_.material.SetVector(cameraRotation2Id_, eye.Rotation2);
    }

    /// <summary>
    /// Vector4を生成する。
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <param name="z">Z座標</param>
    /// <param name="w">W座標</param>
    /// <returns>Vector4</returns>
    private static Vector4 vec(float x, float y, float z, float w)
    {
        return new Vector4(x, y, z, w);
    }

    /// <summary>
    /// Vector3を生成する。
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <param name="z">Z座標</param>
    /// <returns>Vector3</returns>
    private static Vector3 vec(float x, float y, float z)
    {
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Vector2を生成する。
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns>Vector2</returns>
    private static Vector2 vec(float x, float y)
    {
        return new Vector2(x, y);
    }

    /// <summary>
    /// Colorを生成する。各成分の値は[0.0f, 1.0f]
    /// </summary>
    /// <param name="r">赤成分</param>
    /// <param name="g">緑成分</param>
    /// <param name="b">青成分</param>
    /// <returns></returns>
    private static Color rgb(float r, float g, float b)
    {
        return rgba(r, g, b, 1.0f);
    }

    /// <summary>
    /// Colorを生成する。各成分の値は[0.0f, 1.0f]
    /// </summary>
    /// <param name="r">赤成分</param>
    /// <param name="g">緑成分</param>
    /// <param name="b">青成分</param>
    /// <param name="a">アルファ成分</param>
    /// <returns></returns>
    private static Color rgba(float r, float g, float b, float a)
    {
        return new Color(r, g, b, a);
    }
}
