using UnityEngine;

public class HongWaiXianKaiGuan : MonoBehaviour, IElectricElement
{
	[SerializeField]
	public Transform Line;

	[SerializeField]
	public LineRenderer LineR;

	[SerializeField]
	public GameObject RedLight;

	[SerializeField]
	public GameObject GreenLight;

	[HideInInspector]
	public GameObject LineGo;

	protected void Awake()
	{
		if ((bool)Line)
		{
			LineGo = Line.gameObject;
		}
		OnStatusChange(1);
	}

	public void OnStatusChange(int status)
	{
		bool flag = ElectricTool.IsConnectToBattery(status);
		bool flag2 = ElectricTool.IsSwitchOn(status);
		if (flag)
		{
			Line.gameObject.SetActiveBetter(true);
			RedLight.SetActiveBetter(!flag2);
			GreenLight.SetActiveBetter(flag2);
		}
		else
		{
			Line.gameObject.SetActiveBetter(false);
			RedLight.SetActiveBetter(false);
			GreenLight.SetActiveBetter(false);
		}
	}
}
