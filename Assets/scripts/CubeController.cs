using UnityEngine;
using System.Collections;

/// <summary>
/// 立方体コントローラークラス
/// </summary>
public class CubeController : MonoBehaviour
{
    // 生成されたメッシュオブジェクト
    private Mesh mesh_;

    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    void Awake()
    {
        mesh_ = new Mesh();

        // 頂点の設定
        mesh_.vertices = new Vector3[]
        {
            // 手前の面
            vec(-0.5f,  0.5f, -0.5f),
            vec( 0.5f,  0.5f, -0.5f),
            vec( 0.5f, -0.5f, -0.5f),
            vec(-0.5f, -0.5f, -0.5f),

            // 奥の面
            vec(-0.5f,  0.5f,  0.5f),
            vec( 0.5f,  0.5f,  0.5f),
            vec( 0.5f, -0.5f,  0.5f),
            vec(-0.5f, -0.5f,  0.5f),
        };

        // 4つ目の座標軸の点の設定
        // UV座標で代用する。ここでのuの値がxyzwのwになる。
        mesh_.uv = new Vector2[]
        {
            // 手前の面
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
            
            // 奥の面
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
            vec(0.0f,  0.0f),
        };

        // 頂点色の設定
        mesh_.colors = new Color[]
        {
            rgb(1.0f, 0.0f, 0.0f),
            rgb(0.0f, 1.0f, 0.0f),
            rgb(0.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 0.0f),

            rgb(0.0f, 1.0f, 1.0f),
            rgb(1.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 1.0f),
            rgb(0.0f, 0.0f, 0.0f),
        };

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
        return new Color(r, g, b, 1.0f);
    }
}
