using UnityEngine;

public class JiXieDeng : MonoBehaviour, IElectricElement
{
	[SerializeField]
	private GameObject _lightEffectGo;

	public void OnStatusChange(int status)
	{
		if ((bool)_lightEffectGo)
		{
			_lightEffectGo.SetActiveBetter(ElectricTool.IsConnectToBattery(status));
		}
	}
}
