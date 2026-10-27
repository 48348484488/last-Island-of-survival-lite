using UnityEngine;

public class CarInfo : MonoBehaviour
{
	[Header("座位挂点")]
	public Transform[] seats;

	[Header("探身挂点")]
	public Transform[] Tanshen;

	[Header("方向盘左手挂点")]
	public Transform leftHandAttachPoint;

	[Header("方向盘右手挂点")]
	public Transform rightHandAttachPoint;

	[Header("左前轮模型")]
	public Transform frontLeftWheelModel;

	[Header("右前轮模型")]
	public Transform frontRightWheelModel;

	[Header("左后轮模型")]
	public Transform rearLeftWheelModel;

	[Header("右后轮模型")]
	public Transform rearRightWheelModel;

	[Header("左前轮碰撞体")]
	public WheelCollider frontLeftWheelCollider;

	[Header("右前轮碰撞体")]
	public WheelCollider frontRightWheelCollider;

	[Header("左后轮碰撞体")]
	public WheelCollider rearLeftWheelCollider;

	[Header("右后轮碰撞体")]
	public WheelCollider rearRightWheelCollider;

	[Header("玻璃")]
	public Transform[] glasses;

	[Header("冒烟特效位置")]
	public Vector3 smokeEffectPos;

	[Header("燃烧特效位置")]
	public Vector3 burnEffectPos;

	[Header("爆炸特效位置")]
	public Vector3 explodeEffectPos;

	[Header("氮气加速特效位置")]
	public Vector3 nitrogenEffectPos;
}
