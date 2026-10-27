using UnityEngine;

public class WrongTypeStopAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.WrongTypeStop;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return SingletonMono<TrusteeshipMgr>.Ins.SelfMainCameraTrans.forward;
		}
	}

	protected override bool NeedTurnCamera
	{
		get
		{
			return false;
		}
	}

	protected override float WaitTurnFinishTime
	{
		get
		{
			return 0.05f;
		}
	}

	protected override void AttachEvent()
	{
	}

	protected override void DetachEvent()
	{
	}

	protected override void StartAction(Vector3 selfPos)
	{
		SingletonMono<TrusteeshipMgr>.Ins.StopTrusteeship();
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		return false;
	}
}
