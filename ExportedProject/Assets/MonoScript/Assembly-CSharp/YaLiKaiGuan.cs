using UnityEngine;

public class YaLiKaiGuan : MonoBehaviour, IElectricElement
{
	[SerializeField]
	private Animator _animCtrl;

	[SerializeField]
	private GameObject _redLight;

	[SerializeField]
	private GameObject _greenLight;

	public void OnStatusChange(int status)
	{
		bool isConnect = ElectricTool.IsConnectToBattery(status);
		if (ElectricTool.IsSwitchOn(status))
		{
			OnSwitchOn(isConnect);
		}
		else
		{
			OnSwitchOff(isConnect);
		}
	}

	private void OnSwitchOn(bool isConnect)
	{
		_animCtrl.SetBool("ToOpen", true);
		_animCtrl.SetBool("ToClose", false);
		_redLight.SetActiveBetter(false);
		_greenLight.SetActiveBetter(isConnect);
	}

	private void OnSwitchOff(bool isConnect)
	{
		_animCtrl.SetBool("ToOpen", false);
		_animCtrl.SetBool("ToClose", true);
		_redLight.SetActiveBetter(isConnect);
		_greenLight.SetActiveBetter(false);
	}
}
