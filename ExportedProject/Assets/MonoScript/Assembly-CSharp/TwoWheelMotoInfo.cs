using UnityEngine;

public class TwoWheelMotoInfo : MonoBehaviour
{
	[Header("座位挂点")]
	public Transform[] seats;

	[Header("方向盘左手挂点")]
	public Transform leftHandAttachPoint;

	[Header("方向盘右手挂点")]
	public Transform rightHandAttachPoint;

	[Header("前轮模型")]
	public Transform frontWheelModel;

	[Header("后轮模型")]
	public Transform rearWheelModel;

	[Header("前轮碰撞体")]
	public WheelCollider frontWheelCollider;

	[Header("后轮碰撞体")]
	public WheelCollider rearWheelCollider;

	[Header("冒烟特效位置")]
	public Vector3 smokeEffectPos;

	[Header("燃烧特效位置")]
	public Vector3 burnEffectPos;

	[Header("爆炸特效位置")]
	public Vector3 explodeEffectPos;

	[Header("氮气加速特效位置")]
	public Vector3 nitrogenEffectPos;

	[Header("车把模型")]
	public Transform handTransform;

	[Header("车体")]
	public Transform chassis;

	[Header("重心")]
	public Transform com;
}
