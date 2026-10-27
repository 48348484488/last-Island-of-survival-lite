using UnityEngine;

public abstract class TrusteeshipAction
{
	private const float TurnTotalTime = 1f;

	protected Transform TargetTrans;

	protected GameObject TargetGo;

	private float _passTime;

	private bool _isTurn;

	private Vector3 _selfPos;

	private Vector3 _lookDir;

	protected abstract float WaitTurnFinishTime { get; }

	protected abstract Vector3 LookAtDir { get; }

	protected abstract bool NeedTurnCamera { get; }

	public abstract TrusteeshipActionType SelfType { get; }

	public void OnEnter(Transform trans = null)
	{
		TargetTrans = trans;
		if (TargetTrans != null)
		{
			TargetGo = TargetTrans.gameObject;
		}
		if (!NeedTurnCamera)
		{
			StartActionImmediately();
			return;
		}
		_passTime = 0f;
		_isTurn = true;
		SingletonMono<TrusteeshipMgr>.Ins.UpdateEveryFrame = true;
		_lookDir = LookAtDir;
		TrusteeshipEvent.UpdateFinishDelegate = StartActionImmediately;
		TrusteeshipEvent.UpdateNeedChangeDelegate = UpdateTurn;
	}

	private void StartActionImmediately()
	{
		_passTime = 0f;
		_isTurn = false;
		SingletonMono<TrusteeshipMgr>.Ins.UpdateEveryFrame = false;
		TrusteeshipEvent.UpdateFinishDelegate = ActionFinish;
		TrusteeshipEvent.UpdateNeedChangeDelegate = NeedChangeAction;
		AttachEvent();
		StartAction(SingletonMono<TrusteeshipMgr>.Ins.SelfPos);
	}

	public void OnExit()
	{
		TrusteeshipEvent.UpdateFinishDelegate = null;
		TrusteeshipEvent.UpdateNeedChangeDelegate = null;
		DetachEvent();
		StopAction();
	}

	private void ChangeCameraRotation()
	{
		if (1f > _passTime && Vector3.Angle(SingletonMono<TrusteeshipMgr>.Ins.SelfMainCameraTrans.forward, LookAtDir) > 0.01f)
		{
			SingletonMono<TrusteeshipMgr>.Ins.LookAtVector(Vector3.Lerp(_lookDir, SingletonMono<TrusteeshipMgr>.Ins.SelfMainCameraTrans.forward, 1f - _passTime));
			return;
		}
		SingletonMono<TrusteeshipMgr>.Ins.LookAtVector(_lookDir);
		_isTurn = false;
		_passTime = 0f;
	}

	private bool UpdateTurn()
	{
		_passTime += Time.deltaTime;
		if (_isTurn)
		{
			ChangeCameraRotation();
			return false;
		}
		if (_passTime > WaitTurnFinishTime)
		{
			return true;
		}
		return false;
	}

	protected abstract void AttachEvent();

	protected abstract void StartAction(Vector3 selfPos);

	protected abstract void DetachEvent();

	protected abstract void StopAction();

	protected abstract bool NeedChangeAction();

	private void ActionFinish()
	{
		SingletonMono<TrusteeshipMgr>.Ins.ChangeTrusteeshipAction(TargetTrans);
	}
}
