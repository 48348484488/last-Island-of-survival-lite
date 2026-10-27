using UnityEngine;

public class SteelTrap : MonoBehaviour
{
	[SerializeField]
	private Animator _animCtrl;

	public Animator AnimCtrl
	{
		get
		{
			return _animCtrl;
		}
	}

	public void PlayOpenToCloseAnim()
	{
		if (!_animCtrl)
		{
			Debug.LogError("No Animator assigned.");
			return;
		}
		_animCtrl.SetBool("ToOpen", false);
		_animCtrl.SetBool("ToClose", true);
	}

	public void PlayCloseToOpenAnim()
	{
		if (!_animCtrl)
		{
			Debug.LogError("No Animator assigned.");
			return;
		}
		_animCtrl.SetBool("ToOpen", true);
		_animCtrl.SetBool("ToClose", false);
	}
}
