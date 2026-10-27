using System;

public class SignalBase
{
	protected int m_methodCount;

	protected Delegate[] m_methods;

	public SignalBase(int capacity)
	{
		m_methods = new Delegate[capacity];
		m_methodCount = 0;
	}

	public SignalBase()
	{
		m_methods = new Delegate[10];
		m_methodCount = 0;
	}

	protected void AddListener(Delegate del, bool bInsertAtFirst)
	{
		if (bInsertAtFirst)
		{
			if (m_methods[0] == del)
			{
				return;
			}
			Delegate[] array = ((m_methodCount + 1 <= m_methods.Length) ? new Delegate[m_methods.Length] : new Delegate[m_methods.Length * 2]);
			bool flag = false;
			int num = 0;
			array[num++] = del;
			for (int i = 0; i < m_methodCount; i++)
			{
				if (m_methods[i] == del)
				{
					flag = true;
				}
				else
				{
					array[num++] = m_methods[i];
				}
			}
			if (!flag)
			{
				m_methodCount++;
			}
			m_methods = array;
			return;
		}
		for (int j = 0; j < m_methodCount; j++)
		{
			if (m_methods[j] == del)
			{
				return;
			}
		}
		if (m_methodCount + 1 > m_methods.Length)
		{
			Delegate[] array2 = new Delegate[m_methods.Length * 2];
			Array.Copy(m_methods, 0, array2, 0, m_methodCount);
			m_methods = array2;
		}
		m_methods[m_methodCount] = del;
		m_methodCount++;
	}

	protected void RemoveListener(Delegate del)
	{
		int num = 0;
		int methodCount = m_methodCount;
		for (int i = 0; i < methodCount; i++)
		{
			if (m_methods[i] == del)
			{
				m_methodCount--;
				m_methods[i] = null;
				for (int j = i; j < m_methodCount; j++)
				{
					m_methods[j] = m_methods[j + 1];
				}
				break;
			}
		}
	}

	public void RemoveAllListeners()
	{
		for (int i = 0; i < m_methodCount; i++)
		{
			m_methods[i] = null;
		}
		m_methodCount = 0;
	}

	public int GetListenerCount()
	{
		return m_methodCount;
	}
}
