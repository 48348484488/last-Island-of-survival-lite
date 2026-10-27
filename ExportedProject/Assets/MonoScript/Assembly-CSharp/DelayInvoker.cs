using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelayInvoker : SingletonMono<DelayInvoker>
{
	public delegate void DelayFunction(object[] args);

	private class DelayHelper
	{
		public object name;

		public float delay;

		public DelayFunction func;

		public object[] args;

		public void Invoke()
		{
			if (func != null)
			{
				try
				{
					func(args);
				}
				catch (Exception ex)
				{
					Debug.LogError("DelayInvoker" + ex.Message + "   Invoke() Error:{0}\n{1}" + ex.StackTrace);
				}
			}
		}
	}

	private Dictionary<string, DelayHelper> m_dicHelper;

	private static WaitForEndOfFrame ms_waitForEndOfFrame = new WaitForEndOfFrame();

	private List<string> m_needRemoveKeys = new List<string>();

	private List<string> m_Keys = new List<string>();

	public static void DelayInvoke(string name, float delay, DelayFunction func, params object[] args)
	{
		SingletonMono<DelayInvoker>.Ins.DelayInvokeWorker(name, delay, func, args);
	}

	public new static void CancelInvoke(string name)
	{
		SingletonMono<DelayInvoker>.Ins.CancelInvokeWorker(name);
	}

	private void DelayInvokeWorker(string name, float delay, DelayFunction func, params object[] args)
	{
		if (m_dicHelper == null)
		{
			m_dicHelper = new Dictionary<string, DelayHelper>();
		}
		if (m_dicHelper.ContainsKey(name))
		{
			m_dicHelper[name].name = name;
			m_dicHelper[name].delay = delay;
			m_dicHelper[name].func = func;
			m_dicHelper[name].args = args;
		}
		else
		{
			DelayHelper delayHelper = new DelayHelper();
			delayHelper.name = name;
			delayHelper.delay = delay;
			delayHelper.func = func;
			delayHelper.args = args;
			m_dicHelper.Add(name, delayHelper);
		}
	}

	private void CancelInvokeWorker(string name)
	{
		if (m_dicHelper != null)
		{
			if (name == null)
			{
				m_dicHelper.Clear();
			}
			else
			{
				m_dicHelper.Remove(name);
			}
		}
	}

	private void Update()
	{
		if (m_dicHelper != null && m_dicHelper.Count > 0)
		{
			m_Keys = m_dicHelper.Keys.ToList();
			foreach (string key in m_Keys)
			{
				DelayHelper delayHelper = m_dicHelper[key];
				if (delayHelper != null)
				{
					delayHelper.delay -= Time.deltaTime;
					if (delayHelper.delay <= 0f)
					{
						m_needRemoveKeys.Add(key);
						delayHelper.Invoke();
					}
				}
			}
		}
		for (int num = m_needRemoveKeys.Count - 1; num >= 0; num--)
		{
			m_dicHelper.Remove(m_needRemoveKeys[num]);
			m_needRemoveKeys.RemoveAt(num);
		}
	}

	private void OnDisable()
	{
		CancelInvoke(null);
		StopAllCoroutines();
	}

	public static void DelayInvokerOnEndOfFrame(DelayFunction func, params object[] args)
	{
		SingletonMono<DelayInvoker>.Ins.StartCoroutine(DelayInvokerOnEndOfFrameWorker(func, args));
	}

	private static IEnumerator DelayInvokerOnEndOfFrameWorker(DelayFunction func, params object[] args)
	{
		yield return ms_waitForEndOfFrame;
		try
		{
			func(args);
		}
		catch (Exception ex)
		{
			Debug.LogError("DelayInvoker" + ex.Message + "    DelayInvokerOnEndOfFrame() Error:{0}\n{1}" + ex.StackTrace);
		}
	}
}
