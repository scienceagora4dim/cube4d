using UnityEngine;
using System.Collections;

/// <summary>
/// 最初のシーンのコントローラークラス
/// </summary>
public class StartSceneController : MonoBehaviour
{
    /// <summary>
    /// X軸回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("X軸回転用スライダー")]
    private LabeledSliderController rotateXSlider;

    /// <summary>
    /// Y軸回転用スライダー
    /// </summary>
    [SerializeField, Tooltip("Y軸回転用スライダー")]
    private LabeledSliderController rotateYSlider;

    /// <summary>
    /// Z軸回転用スライダー
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

    /// <summary>
    /// 超立方体オブジェクト
    /// </summary>
    [SerializeField, Tooltip("超立方体")]
    private CubeController cube;

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
        cube.gameObject.transform.eulerAngles = new Vector3(rotateXSlider.value, rotateYSlider.value, rotateZSlider.value) * 360.0f;
        cube.wAngles = new Vector3(rotateWXSlider.value, rotateWYSlider.value, rotateWZSlider.value) * 360.0f;
	}
}
