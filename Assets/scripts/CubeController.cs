using UnityEngine;
using System.Collections;
using System.Linq;
using cube4d.hobj;

/// <summary>
/// 立方体コントローラークラス
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class CubeController : MonoBehaviour
{

    /// <summary>
    /// 面のアルファ値
    /// </summary>
    private const float ALPHA = 0.3f;

    /// <summary>
    /// オブジェクト形状のソースコード
    /// </summary>
    [SerializeField, Tooltip("オブジェクト形状のソースコード")]
    public TextAsset objectSource;

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

        // オブジェクト形状の読み込み
        HigherObject hobj = HigherObjects.ReadFromSource(objectSource.text).First().Value;
        mesh_ = HigherObjects.MakeMesh(hobj);

        // TODO: 頂点色もファイルで設定できるようにする
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

            rgba(0.0f, 1.0f, 1.0f, ALPHA), //skyblue
            rgba(1.0f, 0.0f, 1.0f, ALPHA), //magenta
            rgba(1.0f, 1.0f, 1.0f, ALPHA), //white
            rgba(0.0f, 0.0f, 0.0f, ALPHA), //black

            rgba(1.0f, 0.0f, 0.0f, ALPHA), //red
            rgba(0.0f, 1.0f, 0.0f, ALPHA), //green
            rgba(0.0f, 0.0f, 1.0f, ALPHA), //blue
            rgba(1.0f, 1.0f, 0.0f, ALPHA), //yellow
        };
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
