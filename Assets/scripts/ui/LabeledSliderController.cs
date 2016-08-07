using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// ラベル付きスライダーのコントローラークラス
/// </summary>
public class LabeledSliderController : MonoBehaviour
{
    /// <summary>
    /// ラベル
    /// </summary>
    [SerializeField, Tooltip("スライダーのラベル")]
    public string label;

    // 子要素のラベル
    private Text label_;

    // 子要素のスライダー
    private Slider slider_;

    /// <summary>
    /// スライダーの値
    /// </summary>
    public float value
    {
        get { return slider_.value; }
        set { slider_.value = value; }
    }

    /// <summary>
    /// シーン開始時の処理
    /// </summary>
	void Start ()
    {
        // 子要素を取得する
        label_ = GetComponentInChildren<Text>();
        slider_ = GetComponentInChildren<Slider>();
	}
	
    /// <summary>
    /// 毎フレームの処理
    /// </summary>
	void Update ()
    {
        label_.text = label;
    }
}
