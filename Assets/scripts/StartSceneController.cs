using UnityEngine;
using System.Collections;

/// <summary>
/// 最初のシーンのコントローラークラス
/// </summary>
public class StartSceneController : MonoBehaviour
{
    private const int CAMERA_WIDTH = 320;
    private const int CAMERA_HEIGHT = 240;
    private const int FPS = 30;

    /// 4次元の回転角 ij平面：xy(12), xz(13), xw(14), yz(23), yw(24), zw(34)
    /// n次元のij平面回転角が第m成分（ここでは使わない）: m = n(i-1)-i(i+1)/2+j-1;
    public float[] Angles = new float[6];

    /// <summary>
    /// Webカメラテクスチャ
    /// </summary>
    public WebCamTexture webCameraTexture;

	/*
    /// <summary>
    /// X軸/YZ平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("X軸回転用スライダー")]
    private LabeledSliderController rotateXSlider;

    /// <summary>
    /// Y軸/XZ平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("Y軸回転用スライダー")]
    private LabeledSliderController rotateYSlider;

    /// <summary>
    /// Z軸/XY平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("Z軸回転用スライダー")]
    private LabeledSliderController rotateZSlider;

    /// <summary>
    /// WX平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("WX平面回転用スライダー")]
    private LabeledSliderController rotateWXSlider;

    /// <summary>
    /// WY平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("WY平面回転用スライダー")]
    private LabeledSliderController rotateWYSlider;

    /// <summary>
    /// WZ平面回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("WZ平面回転用スライダー")]
    private LabeledSliderController rotateWZSlider;
	*/

    /// <summary>
    /// 超立方体オブジェクト
    /// </summary>
    //[SerializeField, Tooltip("超立方体")]
    //private CubeController cube;
	public CubeController cube;
	public CubeController cubeleft;
    /// <summary>
    /// シーン開始時の処理
    /// </summary>
	void Start ()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        // display all cameras
        for (var i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
        }

        if (devices.Length > 0)
        {
            webCameraTexture = new WebCamTexture(devices[0].name, CAMERA_WIDTH, CAMERA_HEIGHT, FPS);
            webCameraTexture.Play();
        }
    }
	
    /// <summary>
    /// 毎フレームの処理
    /// </summary>
	void Update ()
    {
		/*
		/// Slider使用時
		Angles[0] = rotateZSlider.value * 360.0f;
		Angles[1] = rotateYSlider.value * 360.0f;
		Angles[2] = rotateWXSlider.value * 360.0f;
		Angles[3] = rotateXSlider.value * 360.0f;
		Angles[4] = rotateWYSlider.value * 360.0f;
		Angles[5] = rotateWZSlider.value * 360.0f;
		*/

		float rotationSpeed = 3f;
		Angles[0] = Angles[0] + Input.GetAxis("R-Horizontal") * rotationSpeed;
		Angles[2] = Angles[2] + Input.GetAxis("LT/RT") * rotationSpeed;
		Angles[3] = Angles[3] + Input.GetAxis("R-Vertical") * rotationSpeed;
		Angles[4] = Angles[4] + Input.GetAxis("Horizontal") * rotationSpeed;
		Angles[5] = Angles[5] + Input.GetAxis("Vertical") * rotationSpeed;

        cube.gameObject.transform.eulerAngles
				= new Vector3(Angles[3], Angles[1], Angles[0]);
        cube.wAngles 
				= new Vector3(Angles[2], Angles[4], Angles[5]);
        cubeleft.gameObject.transform.eulerAngles
				= new Vector3(Angles[3], Angles[1], Angles[0]);
        cubeleft.wAngles 
				= new Vector3(Angles[2], Angles[4], Angles[5]);
	}
}
