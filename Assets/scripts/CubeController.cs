using UnityEngine;
using System.Collections;
using System.Linq;

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
        Vector4[] vertices = new Vector4[]
        {
            // 手前の面-w
            vec(-0.5f,  0.5f, -0.5f, -0.5f),
            vec( 0.5f,  0.5f, -0.5f, -0.5f),
            vec( 0.5f, -0.5f, -0.5f, -0.5f),
            vec(-0.5f, -0.5f, -0.5f, -0.5f),

            // 奥の面-w
            vec(-0.5f,  0.5f,  0.5f, -0.5f),
            vec( 0.5f,  0.5f,  0.5f, -0.5f),
            vec( 0.5f, -0.5f,  0.5f, -0.5f),
            vec(-0.5f, -0.5f,  0.5f, -0.5f),

            // 手前の面+w
            vec(-0.5f,  0.5f, -0.5f,  0.5f),
            vec( 0.5f,  0.5f, -0.5f,  0.5f),
            vec( 0.5f, -0.5f, -0.5f,  0.5f),
            vec(-0.5f, -0.5f, -0.5f,  0.5f),

            // 奥の面+w
            vec(-0.5f,  0.5f,  0.5f,  0.5f),
            vec( 0.5f,  0.5f,  0.5f,  0.5f),
            vec( 0.5f, -0.5f,  0.5f,  0.5f),
            vec(-0.5f, -0.5f,  0.5f,  0.5f),
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
            rgb(1.0f, 0.0f, 0.0f),
            rgb(0.0f, 1.0f, 0.0f),
            rgb(0.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 0.0f),

            rgb(0.0f, 1.0f, 1.0f),
            rgb(1.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 1.0f),
            rgb(0.0f, 0.0f, 0.0f),

            rgb(0.0f, 1.0f, 1.0f),
            rgb(1.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 1.0f),
            rgb(0.0f, 0.0f, 0.0f),

            rgb(1.0f, 0.0f, 0.0f),
            rgb(0.0f, 1.0f, 0.0f),
            rgb(0.0f, 0.0f, 1.0f),
            rgb(1.0f, 1.0f, 0.0f),
        };

        // 面(三角形分割)の設定
        // 時計回り方向が法線方向
        mesh_.triangles = new int[]
        {
            // -wの立方体

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

            // +wの立方体

            // 手前の面
            8, 9, 10,
            8, 10, 11,
            
            // 上面
            8, 12, 13,
            8, 13, 9,

            // 下面
            11, 14, 15,
            11, 10, 14,

            // 左面
            8, 15, 12,
            8, 11, 15,

            // 右面
            9, 13, 14,
            9, 14, 10,

            // 奥の面
            12, 15, 14,
            12, 14, 13,
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
        return new Color(r, g, b, 1.0f);
    }
}
