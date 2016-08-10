using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 立方体コントローラークラス
/// </summary>
public class CubeController : MonoBehaviour
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
            vec(-0.5f,  0.5f, -0.5f, 0.0f),
            vec( 0.5f,  0.5f, -0.5f, 0.0f),
            vec( 0.5f, -0.5f, -0.5f, 0.0f),
            vec(-0.5f, -0.5f, -0.5f, 0.0f),

            // 奥の面-w
            vec(-0.5f,  0.5f,  0.5f, 0.0f),
            vec( 0.5f,  0.5f,  0.5f, 0.0f),
            vec( 0.5f, -0.5f,  0.5f, 0.0f),
            vec(-0.5f, -0.5f,  0.5f, 0.0f),

            // 手前の面+w
            vec(-0.5f,  0.5f, -0.5f, 1.0f),
            vec( 0.5f,  0.5f, -0.5f, 1.0f),
            vec( 0.5f, -0.5f, -0.5f, 1.0f),
            vec(-0.5f, -0.5f, -0.5f, 1.0f),

            // 奥の面+w
            vec(-0.5f,  0.5f,  0.5f, 1.0f),
            vec( 0.5f,  0.5f,  0.5f, 1.0f),
            vec( 0.5f, -0.5f,  0.5f, 1.0f),
            vec(-0.5f, -0.5f,  0.5f, 1.0f),
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
            rgba(1.0f, 0.0f, 0.0f, ALPHA),
            rgba(0.0f, 1.0f, 0.0f, ALPHA),
            rgba(0.0f, 0.0f, 1.0f, ALPHA),
            rgba(1.0f, 1.0f, 0.0f, ALPHA),

            rgba(0.0f, 1.0f, 1.0f, ALPHA),
            rgba(1.0f, 0.0f, 1.0f, ALPHA),
            rgba(1.0f, 1.0f, 1.0f, ALPHA),
            rgba(0.0f, 0.0f, 0.0f, ALPHA),

            rgba(0.0f, 1.0f, 1.0f, ALPHA),
            rgba(1.0f, 0.0f, 1.0f, ALPHA),
            rgba(1.0f, 1.0f, 1.0f, ALPHA),
            rgba(0.0f, 0.0f, 0.0f, ALPHA),

            rgba(1.0f, 0.0f, 0.0f, ALPHA),
            rgba(0.0f, 1.0f, 0.0f, ALPHA),
            rgba(0.0f, 0.0f, 1.0f, ALPHA),
            rgba(1.0f, 1.0f, 0.0f, ALPHA),
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

                8, 9,
                9, 10,
                10, 11,
                11, 8,

                12, 13,
                13, 14,
                14, 15,
                15, 12,

                8, 12,
                9, 13,
                10, 14,
                11, 15,

                0, 8,
                1, 9,
                2, 10,
                3, 11,

                4, 12,
                5, 13,
                6, 14,
                7, 15,
            }, MeshTopology.Lines, 0);
        }
        else
        {
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

            // 2つの立方体をつなぐ-y側の面

            // 手前の面
            11, 10, 2,
            11, 2, 3,

            // 右の面
            10, 14, 6,
            10, 6, 2,

            // 左の面
            11, 3, 7,
            11, 7, 15,

            // 奥の面
            14, 15, 7,
            14, 7, 6,

            // 2つの立方体をつなぐ+y側の面

            // 手前の面
            0, 1, 9,
            1, 9, 8,

            // 右の面
            1, 5, 13,
            1, 13, 9,
            
            // 左の面
            0, 12, 4,
            0, 8, 12,

            // 奥の面
            5, 4, 12,
            5, 12, 13,

            // 2つの立方体をつなぐ縦の面

            // 左前の面
            0, 8, 11,
            0, 11, 3,

            // 右前の面
            1, 2, 10,
            1, 10, 9,

            // 左後の面
            4, 7, 15,
            4, 15, 12,

            // 右後の面
            5, 13, 14,
            5, 14, 6,
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
