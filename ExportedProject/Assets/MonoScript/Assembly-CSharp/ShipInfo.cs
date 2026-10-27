using UnityEngine;

public class ShipInfo : MonoBehaviour
{
	[Header("座位挂点")]
	public Transform[] seats;

	[Header("方向盘左手挂点")]
	public Transform leftHandAttachPoint;

	[Header("方向盘右手挂点")]
	public Transform rightHandAttachPoint;

	[Header("浮力作用点")]
	public Transform[] buoyancePoints;

	[Header("旋转中心点")]
	public Transform turnCenter;

	[Header("尾部")]
	public Transform tail;

	[Header("玻璃")]
	public Transform[] glasses;

	[Header("冒烟特效位置")]
	public Vector3 smokeEffectPos;

	[Header("燃烧特效位置")]
	public Vector3 burnEffectPos;

	[Header("爆炸特效位置")]
	public Vector3 explodeEffectPos;

	[Header("浪花特效位置")]
	public Vector3 splashEffectPos;
}
