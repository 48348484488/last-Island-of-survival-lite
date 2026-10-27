using System.Collections.Generic;
using UnityEngine;

public class TimerManager : SingletonMono<TimerManager>
{
	public delegate void TimerFinishHandler();

	public delegate void TimerCoolDownHandler();

	public delegate void TimerFinishHandlerArgs(params object[] args);

	private enum TIMER_MODE
	{
		NORMAL = 0,
		REPEAT = 1
	}

	private class Timer
	{
		private string m_Name;

		private TIMER_MODE m_Mode;

		private float m_StartTime;

		private float m_duration;

		private bool m_Break;

		private float m_BreakStart;

		private float m_BreakDuration;

		private float m_lastTimeTriggerCoolDownEvent;

		private TimerFinishHandler m_TimerFinishEvent;

		private TimerCoolDownHandler m_TimerCoolDownEvent;

		private TimerFinishHandlerArgs m_TimerFinishArgsEvent;

		private TimerManager m_Manger;

		private object[] m_Args;

		public float StartTime
		{
			get
			{
				return m_StartTime;
			}
			set
			{
				m_StartTime = value;
			}
		}

		public float TimeLeft
		{
			get
			{
				return Mathf.Max(0f, m_duration - (Time.time - m_StartTime) + m_BreakDuration);
			}
		}

		public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerFinishHandler handler, TimerManager manager)
		{
			m_Name = name;
			m_Mode = mode;
			m_StartTime = startTime;
			m_duration = duration;
			m_TimerFinishEvent = handler;
			m_Manger = manager;
		}

		public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerFinishHandlerArgs handler, TimerManager manager, params object[] args)
		{
			m_Name = name;
			m_Mode = mode;
			m_StartTime = startTime;
			m_duration = duration;
			m_TimerFinishArgsEvent = handler;
			m_Manger = manager;
			m_Args = args;
		}

		public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerFinishHandler handler, TimerManager manager, TimerCoolDownHandler coolDownHandler)
		{
			m_Name = name;
			m_Mode = mode;
			m_StartTime = startTime;
			m_duration = duration;
			m_TimerFinishEvent = handler;
			m_Manger = manager;
			m_TimerCoolDownEvent = coolDownHandler;
		}

		public void Run()
		{
			if (m_Break)
			{
				return;
			}
			if (TimeLeft - m_lastTimeTriggerCoolDownEvent >= 1f && m_TimerCoolDownEvent != null)
			{
				m_lastTimeTriggerCoolDownEvent = TimeLeft;
				m_TimerCoolDownEvent();
			}
			if (!(TimeLeft > 0f))
			{
				if (m_TimerFinishEvent != null)
				{
					m_TimerFinishEvent();
				}
				if (m_TimerFinishArgsEvent != null)
				{
					m_TimerFinishArgsEvent(m_Args);
				}
				if (m_Mode == TIMER_MODE.NORMAL)
				{
					m_Manger.Destroy(m_Name);
					return;
				}
				m_StartTime = Time.time;
				m_BreakDuration = 0f;
			}
		}

		public void Break()
		{
			if (!m_Break)
			{
				m_Break = true;
				m_BreakStart = Time.time;
			}
		}

		public void Resume()
		{
			if (m_Break)
			{
				m_BreakDuration += Time.time - m_BreakStart;
				m_Break = false;
			}
		}
	}

	private Dictionary<string, Timer> m_TimerList = new Dictionary<string, Timer>();

	private Dictionary<string, Timer> m_AddTimerList = new Dictionary<string, Timer>();

	private List<string> m_DestroyTimerList = new List<string>();

	public override void Init()
	{
	}

	private void Update()
	{
		if (m_DestroyTimerList.Count > 0)
		{
			foreach (string destroyTimer in m_DestroyTimerList)
			{
				m_TimerList.Remove(destroyTimer);
			}
			m_DestroyTimerList.Clear();
		}
		if (m_AddTimerList.Count > 0)
		{
			foreach (KeyValuePair<string, Timer> addTimer in m_AddTimerList)
			{
				if (addTimer.Value != null)
				{
					if (m_TimerList.ContainsKey(addTimer.Key))
					{
						m_TimerList[addTimer.Key] = addTimer.Value;
					}
					else
					{
						m_TimerList.Add(addTimer.Key, addTimer.Value);
					}
				}
			}
			m_AddTimerList.Clear();
		}
		if (m_TimerList.Count <= 0)
		{
			return;
		}
		foreach (Timer value in m_TimerList.Values)
		{
			if (value == null)
			{
				break;
			}
			value.Run();
		}
	}

	public bool AddTimer(string key, float duration, TimerFinishHandler handler)
	{
		return Internal_AddTimer(key, TIMER_MODE.NORMAL, duration, handler);
	}

	public bool AddTimerCoolDown(string key, float duration, TimerFinishHandler handler, TimerCoolDownHandler coolDownHandler)
	{
		return Internal_AddTimer(key, TIMER_MODE.NORMAL, duration, handler);
	}

	public bool AddTimerRepeat(string key, float duration, TimerFinishHandler handler)
	{
		return Internal_AddTimer(key, TIMER_MODE.REPEAT, duration, handler);
	}

	public bool AddTimer(string key, float duration, TimerFinishHandlerArgs handler, params object[] args)
	{
		return Internal_AddTimer(key, TIMER_MODE.NORMAL, duration, handler, args);
	}

	public bool AddTimerRepeat(string key, float duration, TimerFinishHandlerArgs handler, params object[] args)
	{
		return Internal_AddTimer(key, TIMER_MODE.REPEAT, duration, handler, args);
	}

	public void BreakTimerWithPrefix(string prefix)
	{
		if (m_TimerList == null || m_TimerList.Count <= 0)
		{
			return;
		}
		string[] array = new string[m_TimerList.Count];
		m_TimerList.Keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith(prefix))
			{
				BreakTimer(array[i]);
			}
		}
	}

	public void BreakTimer(string key)
	{
		if (m_TimerList.ContainsKey(key))
		{
			Timer timer = m_TimerList[key];
			timer.Break();
		}
	}

	public void ResumeTimerWithPrefix(string prefix)
	{
		if (m_TimerList == null || m_TimerList.Count <= 0)
		{
			return;
		}
		string[] array = new string[m_TimerList.Count];
		m_TimerList.Keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith(prefix))
			{
				ResumeTimer(array[i]);
			}
		}
	}

	public void ResumeTimer(string key)
	{
		if (m_TimerList.ContainsKey(key))
		{
			Timer timer = m_TimerList[key];
			timer.Resume();
		}
	}

	public void ClearTimerWithPrefix(string prefix)
	{
		if (m_TimerList == null || m_TimerList.Count <= 0)
		{
			return;
		}
		foreach (string key in m_TimerList.Keys)
		{
			if (key.StartsWith(prefix))
			{
				Destroy(key);
			}
		}
	}

	public bool Destroy(string key)
	{
		if (!m_TimerList.ContainsKey(key))
		{
			return false;
		}
		if (!m_DestroyTimerList.Contains(key))
		{
			m_DestroyTimerList.Add(key);
		}
		return true;
	}

	private bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerFinishHandler handler)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (duration < 0f)
		{
			return false;
		}
		Timer value = new Timer(key, mode, Time.time, duration, handler, this);
		if (m_AddTimerList.ContainsKey(key))
		{
			m_AddTimerList[key] = value;
		}
		else
		{
			m_AddTimerList.Add(key, value);
		}
		return true;
	}

	private bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerFinishHandlerArgs handler, params object[] args)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (duration < 0f)
		{
			return false;
		}
		Timer value = new Timer(key, mode, Time.time, duration, handler, this, args);
		if (m_AddTimerList.ContainsKey(key))
		{
			m_AddTimerList[key] = value;
		}
		else
		{
			m_AddTimerList.Add(key, value);
		}
		return true;
	}

	private bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerFinishHandler handler, TimerCoolDownHandler coolDownHandler)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (duration < 0f)
		{
			return false;
		}
		Timer value = new Timer(key, mode, Time.time, duration, handler, this, coolDownHandler);
		if (m_AddTimerList.ContainsKey(key))
		{
			m_AddTimerList[key] = value;
		}
		else
		{
			m_AddTimerList.Add(key, value);
		}
		return true;
	}

	public bool IsRunning(string key)
	{
		return m_TimerList.ContainsKey(key);
	}

	public float GetTimerLeft(string key)
	{
		if (!m_TimerList.ContainsKey(key))
		{
			return 0f;
		}
		Timer timer = m_TimerList[key];
		return timer.TimeLeft;
	}

	public float GetTimerLeftWithPrefix(string prefix)
	{
		if (m_TimerList != null && m_TimerList.Count > 0)
		{
			string[] array = new string[m_TimerList.Count];
			m_TimerList.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].StartsWith(prefix))
				{
					return GetTimerLeft(array[i]);
				}
			}
		}
		return 0f;
	}
}
