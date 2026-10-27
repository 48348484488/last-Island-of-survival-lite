using DG.Tweening;
using UnityEngine;

public class JiGuanMen : MonoBehaviour, IElectricElement
{
	private const float OpenYOffset = 5f;

	private const float CloseYOffset = 2f;

	private const string OpenActionStateName = "ToOpen";

	private const string CloseActionStateName = "ToClose";

	private const float ClipLength = 1f;

	[SerializeField]
	private Animator _animCtrl;

	[SerializeField]
	private Transform _doorCol;

	public void OnStatusChange(int status)
	{
		if (status > 0)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	private void Open()
	{
		AnimatorStateInfo currentAnimatorStateInfo = _animCtrl.GetCurrentAnimatorStateInfo(0);
		float value;
		if (currentAnimatorStateInfo.IsName("ToClose"))
		{
			value = currentAnimatorStateInfo.normalizedTime;
			_doorCol.DOKill();
			_doorCol.DOLocalMoveY(5f, 1f * (1f - currentAnimatorStateInfo.normalizedTime));
		}
		else
		{
			value = 0f;
			_doorCol.DOLocalMoveY(5f, 1f);
		}
		_animCtrl.SetFloat("Blend", value);
		_animCtrl.Play("ToOpen");
	}

	private void Close()
	{
		AnimatorStateInfo currentAnimatorStateInfo = _animCtrl.GetCurrentAnimatorStateInfo(0);
		float value;
		if (currentAnimatorStateInfo.IsName("ToOpen"))
		{
			value = currentAnimatorStateInfo.normalizedTime;
			_doorCol.DOKill();
			_doorCol.DOLocalMoveY(2f, 1f * (1f - currentAnimatorStateInfo.normalizedTime));
		}
		else
		{
			value = 0f;
			_doorCol.DOLocalMoveY(2f, 1f);
		}
		_animCtrl.SetFloat("Blend", value);
		_animCtrl.Play("ToClose");
	}
}
