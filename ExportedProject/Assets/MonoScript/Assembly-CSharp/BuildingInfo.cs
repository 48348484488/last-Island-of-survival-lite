using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
	[HideInInspector]
	public int id;

	[Header("房子类型ID")]
	public int type;

	[Header("掉落点列表")]
	public Transform[] dropPoints;

	[Header("房顶掉落点列表")]
	public Transform[] roofDropPoints;

	[Header("门列表")]
	public Transform[] doors;

	[Header("窗列表")]
	public Transform[] windows;

	[Header("保险箱")]
	public Transform[] baoxianxiang;
}
