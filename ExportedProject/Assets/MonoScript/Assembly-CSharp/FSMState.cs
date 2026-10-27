using System.Collections;

public abstract class FSMState
{
	protected StateID stateID;

	protected string m_TargetAimatorName;

	public FSMSystem fsm;

	public bool OpenLog;

	public bool CanBreak = true;

	public PlayerController Player;

	public MonsterController Monster;

	public MapObject Obj;

	public StateID ID
	{
		get
		{
			return stateID;
		}
	}

	public virtual void DoBeforeEntering(params object[] args)
	{
	}

	public virtual void DoBeforeLeaving()
	{
	}

	public virtual void Init()
	{
	}

	public abstract void Act();

	public abstract void Reason();

	public virtual void LateUpdate()
	{
	}

	public virtual IEnumerator ActCoroutine()
	{
		yield break;
	}

	public bool IsTargetAimatorPlayFinish()
	{
		if (Obj is PlayerController)
		{
			return ((PlayerController)Obj).IsPlayFinish(m_TargetAimatorName);
		}
		return ((MonsterController)Obj).IsPlayFinish(m_TargetAimatorName);
	}

	public IEnumerator WaitTargetAimatorFinish()
	{
		while (!IsTargetAimatorPlayFinish())
		{
			yield return null;
		}
	}
}
