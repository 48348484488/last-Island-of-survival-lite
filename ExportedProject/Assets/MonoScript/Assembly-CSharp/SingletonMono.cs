using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
	private static T m_Instance;

	public static T Ins
	{
		get
		{
			if ((Object)m_Instance == (Object)null)
			{
				m_Instance = Object.FindObjectOfType(typeof(T)) as T;
				if ((Object)m_Instance == (Object)null)
				{
					m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
					m_Instance.Init();
					Object.DontDestroyOnLoad(m_Instance);
				}
			}
			return m_Instance;
		}
	}

	protected virtual void Awake()
	{
		if ((Object)m_Instance == (Object)null)
		{
			m_Instance = this as T;
			m_Instance.Init();
		}
	}

	public virtual void Init()
	{
	}

	private void OnDestroy()
	{
		m_Instance = (T)null;
	}

	private void OnApplicationQuit()
	{
		m_Instance = (T)null;
	}
}
