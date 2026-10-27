using System;
using System.Collections.Generic;

public class EventHandle
{
	private static Dictionary<string, Delegate> m_GlobalEventTable = new Dictionary<string, Delegate>();

	public static void RegisterEvent(string eventName, Action handler)
	{
		RegisterEvent(eventName, (Delegate)handler);
	}

	public static void RegisterEvent<T>(string eventName, Action<T> handler)
	{
		RegisterEvent(eventName, (Delegate)handler);
	}

	private static void RegisterEvent(string eventName, Delegate handler)
	{
		Delegate value;
		if (m_GlobalEventTable.TryGetValue(eventName, out value))
		{
			m_GlobalEventTable[eventName] = Delegate.Combine(value, handler);
		}
		else
		{
			m_GlobalEventTable.Add(eventName, handler);
		}
	}

	public static void UnregisterEvent(string eventName, Action handler)
	{
		UnregisterEvent(eventName, (Delegate)handler);
	}

	public static void UnregisterEvent<T>(string eventName, Action<T> handler)
	{
		UnregisterEvent(eventName, (Delegate)handler);
	}

	private static void UnregisterEvent(string eventName, Delegate handler)
	{
		Delegate value;
		if (m_GlobalEventTable.TryGetValue(eventName, out value))
		{
			m_GlobalEventTable[eventName] = Delegate.Remove(value, handler);
		}
	}

	private static Delegate GetDelegate(string eventName)
	{
		Delegate value;
		if (m_GlobalEventTable.TryGetValue(eventName, out value))
		{
			return value;
		}
		return null;
	}

	public static void ExecuteEvent(string eventName)
	{
		Action action = GetDelegate(eventName) as Action;
		if (action != null)
		{
			action();
		}
	}

	public static void ExecuteEvent<T>(string eventName, T arg1)
	{
		Action<T> action = GetDelegate(eventName) as Action<T>;
		if (action != null)
		{
			action(arg1);
		}
	}
}
