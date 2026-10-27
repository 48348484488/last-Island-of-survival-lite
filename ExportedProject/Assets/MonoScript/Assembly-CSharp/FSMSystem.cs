using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMSystem
{
	private Dictionary<StateID, FSMState> m_StatesDic;

	private FSMState currentState;

	private Coroutine m_CurrentStateActCoroutine;

	public Utils.VoidDelegate Ticker;

	public bool CanRun = true;

	private MapObject m_mapObj;

	public FSMState CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public FSMSystem(MapObject obj)
	{
		m_StatesDic = new Dictionary<StateID, FSMState>();
		m_mapObj = obj;
	}

	public FSMState GetState(StateID stateID)
	{
		return m_StatesDic[stateID];
	}

	public void AddState(FSMState state)
	{
		if (state == null)
		{
			Debug.LogError("The state you want to add is null");
			return;
		}
		if (m_StatesDic.ContainsKey(state.ID))
		{
			Debug.LogError(string.Concat("The state", state.ID, "yosu want to add has already been added"));
			return;
		}
		state.fsm = this;
		state.Obj = m_mapObj;
		if (m_mapObj is PlayerController)
		{
			state.Player = m_mapObj as PlayerController;
		}
		else if (m_mapObj is MonsterController)
		{
			state.Monster = m_mapObj as MonsterController;
		}
		m_StatesDic.Add(state.ID, state);
		state.Init();
	}

	public void SwitchState(StateID id, params object[] args)
	{
		if (currentState == null || currentState.ID == id || !currentState.CanBreak)
		{
			return;
		}
		FSMState value;
		m_StatesDic.TryGetValue(id, out value);
		if (value == null)
		{
			Debug.LogError("SwitchState id = null  " + id);
			return;
		}
		Battle.StopConroutine(m_CurrentStateActCoroutine);
		m_CurrentStateActCoroutine = null;
		currentState.DoBeforeLeaving();
		currentState = value;
		currentState.DoBeforeEntering(args);
		m_CurrentStateActCoroutine = Battle.StartConroutine(currentState.ActCoroutine());
		try
		{
			if (BattleEvent.OnSwitchState != null)
			{
				BattleEvent.OnSwitchState(id, currentState.ID, m_mapObj.InsId);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public void SwtichStateReStart(StateID id, params object[] args)
	{
		if (currentState != null && currentState.CanBreak)
		{
			FSMState value;
			m_StatesDic.TryGetValue(id, out value);
			Battle.StopConroutine(m_CurrentStateActCoroutine);
			m_CurrentStateActCoroutine = null;
			currentState.DoBeforeLeaving();
			currentState = value;
			currentState.DoBeforeEntering(args);
			m_CurrentStateActCoroutine = Battle.StartConroutine(currentState.ActCoroutine());
		}
	}

	public void StartFsmSystem(StateID id, params object[] args)
	{
		FSMState value;
		if (m_StatesDic.TryGetValue(id, out value))
		{
			value.DoBeforeEntering(args);
			Battle.StopConroutine(m_CurrentStateActCoroutine);
			m_CurrentStateActCoroutine = Battle.StartConroutine(value.ActCoroutine());
			currentState = value;
		}
		else
		{
			Debug.LogError(string.Concat("The state ", id, "is not exit in the fsm!!!"));
		}
	}

	public void Update()
	{
		if (CanRun)
		{
			Utils.TriggerEvent(Ticker);
			currentState.Act();
			currentState.Reason();
		}
	}

	public void LateUpdate()
	{
		if (CanRun)
		{
			currentState.LateUpdate();
		}
	}

	public void Stop()
	{
		CanRun = false;
	}

	public void Clear()
	{
		Stop();
		Battle.StopConroutine(m_CurrentStateActCoroutine);
		m_CurrentStateActCoroutine = null;
		m_StatesDic.Clear();
		Ticker = null;
	}
}
