using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GoOverAction : TrusteeshipAction
{
	private const float WaitMovementFinishTime = 0.5f;

	private const float PreStopDistance = 4f;

	private const float NearDistance = 1f;

	private const float MaxRunTime = 15f;

	private float _waitTime;

	private float _runTime;

	private bool _isMove;

	private bool _isEndMove;

	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.GoOver;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return TargetTrans.position - SingletonMono<TrusteeshipMgr>.Ins.SelfPos;
		}
	}

	protected override bool NeedTurnCamera
	{
		get
		{
			return true;
		}
	}

	protected override float WaitTurnFinishTime
	{
		get
		{
			return 0.5f;
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
		_isEndMove = false;
		_isMove = false;
		_runTime = 0f;
		SingletonMono<TrusteeshipMgr>.Ins.UpdateEveryFrame = true;
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(GetNextActionByScriptType());
	}

	private TrusteeshipActionType GetNextActionByScriptType()
	{
		MapObject component = TargetTrans.GetComponent<MapObject>();
		if (component is TrashcanCfgInfo)
		{
			return TrusteeshipActionType.HitTrashcan;
		}
		if (component is TreeInfo)
		{
			return TrusteeshipActionType.CutDownATree;
		}
		if (component is Mine)
		{
			return TrusteeshipActionType.Mining;
		}
		if (component is PlantInfo)
		{
			return TrusteeshipActionType.CollectPlant;
		}
		if (!component)
		{
			Debug.LogError("[GoOverAction.cs]" + TargetTrans.name + " no MapObject find.");
		}
		else
		{
			Debug.Log("[GoOverAction.cs]Unknown type with object : " + TargetTrans.name);
		}
		return TrusteeshipActionType.WrongTypeStop;
	}

	protected override void StopAction()
	{
		SingletonMono<TrusteeshipMgr>.Ins.UpdateEveryFrame = false;
		StopMove();
	}

	private void StopMove()
	{
		_isMove = false;
		_isEndMove = true;
		CrossPlatformInputManager.SetAxis(KeyName.Vertical, 0f);
	}

	private void StartMove()
	{
		_isMove = true;
		CrossPlatformInputManager.SetAxis(KeyName.Vertical, 1f);
	}

	protected override bool NeedChangeAction()
	{
		float deltaTime = Time.deltaTime;
		_runTime += deltaTime;
		if (_runTime > 15f)
		{
			SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.FindTarget);
			return true;
		}
		if (_isMove && CalDistance(TargetTrans.position, SingletonMono<TrusteeshipMgr>.Ins.SelfPos) < 4f)
		{
			_waitTime = 0f;
			StopMove();
			return false;
		}
		if (!_isMove && !_isEndMove)
		{
			if (!(CalDistance(TargetTrans.position, SingletonMono<TrusteeshipMgr>.Ins.SelfPos) > 1f))
			{
				return true;
			}
			StartMove();
		}
		if (!_isMove && _isEndMove)
		{
			_waitTime += deltaTime;
			return _waitTime > 0.5f;
		}
		return false;
	}

	private float CalDistance(Vector3 pos1, Vector3 pos2)
	{
		float num = pos1.x - pos2.x;
		float num2 = pos1.z - pos2.z;
		return num * num + num2 * num2;
	}
}
