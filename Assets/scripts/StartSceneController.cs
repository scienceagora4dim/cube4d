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
    [SerializeField, Tooltip("立方体")]
	public Transform_4D cube;
    [SerializeField, Tooltip("右目")]
	public Transform_4D RightEyeCamera;
    [SerializeField, Tooltip("左目")]
	public Transform_4D LeftEyeCamera;
    [SerializeField, Tooltip("4次元視の度合い")]
	public int squint;

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

		float Speed = 0.1f;
		float rotationSpeed = 3f;
		float squintDegree = 5f;
		
		/// オブジェクトの移動
		hypercube.Position.x += Input.GetAxis("Horizontal") * Speed;
		hypercube.Position.y -= Input.GetAxis("Vertical") * Speed;
		
		/// オブジェクトの回転
		hypercube.Rotation1.y += Input.GetAxis("HorizontalTurn") * rotationSpeed;
		hypercube.Rotation1.z += Input.GetAxis("VerticalTurn") * rotationSpeed;
		// hypercube.Rotation2.x += Input.GetAxis("HorizontalTurn") * rotationSpeed;
		// hypercube.Rotation2.z += Input.GetAxis("VerticalTurn") * rotationSpeed;
		
		/*
		cube.Rotation1.y += Input.GetAxis("HorizontalTurn") * rotationSpeed;
		cube.Rotation1.z += Input.GetAxis("Horizontal") * rotationSpeed;
		cube.Rotation2.x += Input.GetAxis("VerticalTurn") * rotationSpeed;
		cube.Rotation2.z += Input.GetAxis("Vertical") * rotationSpeed;
		*/

		/// カメラの回転
		/*
		RightEyeCamera.Rotation2.x += Input.GetAxis("HorizontalTurn") * rotationSpeed;
		RightEyeCamera.Rotation2.y += Input.GetAxis("VerticalTurn") * rotationSpeed;
		RightEyeCamera.Rotation2.z += Input.GetAxis("Vertical") * rotationSpeed;
		LeftEyeCamera.Rotation2.x += Input.GetAxis("HorizontalTurn") * rotationSpeed;
		LeftEyeCamera.Rotation2.y += Input.GetAxis("VerticalTurn") * rotationSpeed;
		LeftEyeCamera.Rotation2.z += Input.GetAxis("Vertical") * rotationSpeed;
		*/

		RightEyeCamera.Position = 
				InputTracking.GetLocalPosition(VRNode.RightEye)
				+RightEyeCamera.gameObject.transform.parent.localPosition;
		RightEyeCamera.Rotation1 = 
				InputTracking.GetLocalRotation(VRNode.RightEye).eulerAngles;
		
		LeftEyeCamera.Position = 
				InputTracking.GetLocalPosition(VRNode.LeftEye)
				+LeftEyeCamera.gameObject.transform.parent.localPosition;
		LeftEyeCamera.Rotation1 = 
				InputTracking.GetLocalRotation(VRNode.LeftEye).eulerAngles;

		/// 4次元立体視の度合い(0～4)
		if(Input.GetButtonDown("RightBumper") && squint<4)
		{
			squint++;
			RightEyeCamera.Rotation2.x += squintDegree/2;
			LeftEyeCamera.Rotation2.x -= squintDegree/2;

		}
		if(Input.GetButtonDown("LeftBumper") && squint>0)
		{
			squint--;
			RightEyeCamera.Rotation2.x -= squintDegree/2;
			LeftEyeCamera.Rotation2.x += squintDegree/2;

		}
	}
}
