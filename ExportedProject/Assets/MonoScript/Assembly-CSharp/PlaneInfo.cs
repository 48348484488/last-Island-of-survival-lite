using UnityEngine;

public class PlaneInfo : MonoBehaviour
{
	[Header("座位挂点")]
	public Transform[] seats;

	[Header("方向盘左手挂点")]
	public Transform leftHandAttachPoint;

	[Header("方向盘右手挂点")]
	public Transform rightHandAttachPoint;

	[Header("螺旋桨模型")]
	public Transform heliRotorModel;

	[Header("尾部螺旋桨模型")]
	public Transform subHeliRotorModel;

	[Header("顶端旋转中心点")]
	public Transform topTurnCenter;

	[Header("旋转中心点")]
	public Transform turnCenter;

	[Header("冒烟特效位置")]
	public Vector3 smokeEffectPos;

	[Header("燃烧特效位置")]
	public Vector3 burnEffectPos;

	[Header("爆炸特效位置")]
	public Vector3 explodeEffectPos;

	[Header("飞行螺旋桨特效位置")]
	public Vector3 flyRotorEffectPos;

	[Header("玻璃")]
	public Transform[] glasses;

	[Header("起落架")]
	public Transform undercarriages;
}
