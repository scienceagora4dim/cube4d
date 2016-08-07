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
	}
}
