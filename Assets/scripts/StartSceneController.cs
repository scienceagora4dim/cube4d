using UnityEngine;
using UnityEngine.VR;
using System.Collections;

/// <summary>
/// 最初のシーンのコントローラークラス
/// </summary>
public class StartSceneController : MonoBehaviour
{
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
    [SerializeField, Tooltip("超立方体")]
	public Transform_4D hypercube;

    /// <summary>
    /// シーン開始時の処理
    /// </summary>
	void Start ()
    {
	
	}
	
    /// <summary>
    /// 毎フレームの処理
    /// </summary>
	void Update ()
    {
		/*
		/// Slider使用時
		hypercube.Rotation1.x = rotateZSlider.value * 360.0f;
		hypercube.Rotation1.y = rotateYSlider.value * 360.0f;
		hypercube.Rotation1.z = rotateWXSlider.value * 360.0f;
		hypercube.Rotation2.x = rotateXSlider.value * 360.0f;
		hypercube.Rotation2.y = rotateWYSlider.value * 360.0f;
		hypercube.Rotation2.z = rotateWZSlider.value * 360.0f;
		*/

		float rotationSpeed = 3f;
		hypercube.Rotation1.x += Input.GetAxis("R-Horizontal") * rotationSpeed;
		hypercube.Rotation1.y += Input.GetAxis("LT/RT") * rotationSpeed;
		hypercube.Rotation1.z += Input.GetAxis("R-Vertical") * rotationSpeed;
		hypercube.Rotation2.x += Input.GetAxis("Horizontal") * rotationSpeed;
		hypercube.Rotation2.y += Input.GetAxis("Vertical") * rotationSpeed;
	}
}
