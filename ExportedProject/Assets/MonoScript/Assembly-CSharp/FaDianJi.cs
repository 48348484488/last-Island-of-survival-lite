using UnityEngine;

public class FaDianJi : MonoBehaviour, IElectricElement
{
	[SerializeField]
	private Animator _animCtrl;

	[SerializeField]
	private GameObject _openEffect;

	public void OnStatusChange(int status)
	{
		OpenClose(status > 0);
	}

	private void OpenClose(bool isOpen)
	{
		_animCtrl.SetBool("Open", isOpen);
		_animCtrl.SetBool("Close", !isOpen);
		_openEffect.SetActiveBetter(isOpen);
	}
}
