using System;

public class Signal : SignalBase
{
	public Signal(int capacity)
		: base(capacity)
	{
	}

	public Signal()
	{
	}

	public static Signal operator +(Signal p1, Action p2)
	{
		p1.AddListener(p2);
		return p1;
	}

	public static Signal operator -(Signal p1, Action p2)
	{
		p1.RemoveListener(p2);
		return p1;
	}

	public void AddListener(Action a, bool bInsertAtFirst = false)
	{
		AddListener((Delegate)a, bInsertAtFirst);
	}

	public void RemoveListener(Action a)
	{
		RemoveListener((Delegate)a);
	}

	public void Invoke()
	{
		int methodCount = m_methodCount;
		for (int i = 0; i < methodCount; i++)
		{
			Delegate @delegate = m_methods[i];
			if ((object)@delegate != null)
			{
				Action action = @delegate as Action;
				action();
			}
		}
	}

	public void InvokeSafe()
	{
		try
		{
			int methodCount = m_methodCount;
			for (int i = 0; i < methodCount; i++)
			{
				Delegate @delegate = m_methods[i];
				if ((object)@delegate != null)
				{
					Action action = @delegate as Action;
					action();
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
public class Signal<T> : SignalBase
{
	public Signal(int capacity)
		: base(capacity)
	{
	}

	public Signal()
	{
	}

	public static Signal<T>operator +(Signal<T> p1, Action<T> p2)
	{
		p1.AddListener(p2);
		return p1;
	}

	public static Signal<T>operator -(Signal<T> p1, Action<T> p2)
	{
		p1.RemoveListener(p2);
		return p1;
	}

	public void AddListener(Action<T> a, bool bInsertAtFirst = false)
	{
		AddListener((Delegate)a, bInsertAtFirst);
	}

	public void RemoveListener(Action<T> a)
	{
		RemoveListener((Delegate)a);
	}

	public void Invoke(T t1)
	{
		int methodCount = m_methodCount;
		for (int i = 0; i < methodCount; i++)
		{
			Delegate @delegate = m_methods[i];
			if ((object)@delegate != null)
			{
				Action<T> action = @delegate as Action<T>;
				action(t1);
			}
		}
	}

	public void InvokeSafe(T t)
	{
		try
		{
			int methodCount = m_methodCount;
			for (int i = 0; i < methodCount; i++)
			{
				Delegate @delegate = m_methods[i];
				if ((object)@delegate != null)
				{
					Action<T> action = @delegate as Action<T>;
					action(t);
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
public class Signal<T1, T2> : SignalBase
{
	public Signal(int capacity)
		: base(capacity)
	{
	}

	public Signal()
	{
	}

	public static Signal<T1, T2>operator +(Signal<T1, T2> p1, Action<T1, T2> p2)
	{
		p1.AddListener(p2);
		return p1;
	}

	public static Signal<T1, T2>operator -(Signal<T1, T2> p1, Action<T1, T2> p2)
	{
		p1.RemoveListener(p2);
		return p1;
	}

	public void AddListener(Action<T1, T2> a, bool bInsertAtFirst = false)
	{
		AddListener((Delegate)a, bInsertAtFirst);
	}

	public void RemoveListener(Action<T1, T2> a)
	{
		RemoveListener((Delegate)a);
	}

	public void Invoke(T1 t1, T2 t2)
	{
		int methodCount = m_methodCount;
		for (int i = 0; i < methodCount; i++)
		{
			Delegate @delegate = m_methods[i];
			if ((object)@delegate != null)
			{
				Action<T1, T2> action = @delegate as Action<T1, T2>;
				action(t1, t2);
			}
		}
	}

	public void InvokeSafe(T1 t1, T2 t2)
	{
		try
		{
			int methodCount = m_methodCount;
			for (int i = 0; i < methodCount; i++)
			{
				Delegate @delegate = m_methods[i];
				if ((object)@delegate != null)
				{
					Action<T1, T2> action = @delegate as Action<T1, T2>;
					action(t1, t2);
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
public class Signal<T1, T2, T3> : SignalBase
{
	public Signal(int capacity)
		: base(capacity)
	{
	}

	public Signal()
	{
	}

	public static Signal<T1, T2, T3>operator +(Signal<T1, T2, T3> p1, Action<T1, T2, T3> p2)
	{
		p1.AddListener(p2);
		return p1;
	}

	public static Signal<T1, T2, T3>operator -(Signal<T1, T2, T3> p1, Action<T1, T2, T3> p2)
	{
		p1.RemoveListener(p2);
		return p1;
	}

	public void AddListener(Action<T1, T2, T3> a, bool bInsertAtFirst = false)
	{
		AddListener((Delegate)a, bInsertAtFirst);
	}

	public void RemoveListener(Action<T1, T2, T3> a)
	{
		RemoveListener((Delegate)a);
	}

	public void Invoke(T1 t1, T2 t2, T3 t3)
	{
		int methodCount = m_methodCount;
		for (int i = 0; i < methodCount; i++)
		{
			Delegate @delegate = m_methods[i];
			if ((object)@delegate != null)
			{
				Action<T1, T2, T3> action = @delegate as Action<T1, T2, T3>;
				action(t1, t2, t3);
			}
		}
	}

	public void InvokeSafe(T1 t1, T2 t2, T3 t3)
	{
		try
		{
			int methodCount = m_methodCount;
			for (int i = 0; i < methodCount; i++)
			{
				Delegate @delegate = m_methods[i];
				if ((object)@delegate != null)
				{
					Action<T1, T2, T3> action = @delegate as Action<T1, T2, T3>;
					action(t1, t2, t3);
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
