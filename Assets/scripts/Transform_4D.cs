using UnityEngine;
using System.Collections;

public class Transform_4D : MonoBehaviour {

	// モデル原点のワールド座標
	[SerializeField, Tooltip("モデル原点のワールド座標")]
	public Vector4 Position;
	// モデルの向きを定める角度
	[SerializeField, Tooltip("モデルの向きを定める角度 XY XZ XW")]
	public Vector3 Rotation1;
	[SerializeField, Tooltip("モデルの向きを定める角度 YZ YW ZW")]
	public Vector3 Rotation2;
	// モデル各軸方向の拡大率
	[SerializeField, Tooltip("拡大率")]
	public Vector4 Scale;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
