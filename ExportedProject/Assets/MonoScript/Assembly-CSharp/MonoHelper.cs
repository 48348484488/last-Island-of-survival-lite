using System;
using System.Collections;
using UnityEngine;

public class MonoHelper : SingletonMono<MonoHelper>
{
	public delegate void MonoUpdaterEvent();

	private event MonoUpdaterEvent UpdateEvent;

	private event MonoUpdaterEvent FixedUpdateEvent;

	public static void AddUpdateListener(MonoUpdaterEvent listener)
	{
		if (SingletonMono<MonoHelper>.Ins != null)
		{
			SingletonMono<MonoHelper>.Ins.UpdateEvent += listener;
		}
	}

	public static void RemoveUpdateListener(MonoUpdaterEvent listener)
	{
		if (SingletonMono<MonoHelper>.Ins != null)
		{
			SingletonMono<MonoHelper>.Ins.UpdateEvent -= listener;
		}
	}

	public static void AddFixedUpdateListener(MonoUpdaterEvent listener)
	{
		if (SingletonMono<MonoHelper>.Ins != null)
		{
			SingletonMono<MonoHelper>.Ins.FixedUpdateEvent += listener;
		}
	}

	public static void RemoveFixedUpdateListener(MonoUpdaterEvent listener)
	{
		if (SingletonMono<MonoHelper>.Ins != null)
		{
			SingletonMono<MonoHelper>.Ins.FixedUpdateEvent -= listener;
		}
	}

	private void Update()
	{
		if (this.UpdateEvent != null)
		{
			try
			{
				this.UpdateEvent();
			}
			catch (Exception ex)
			{
				Debug.LogError("MonoHelperUpdate() Error" + ex.Message + "   StackTrace  " + ex.StackTrace);
			}
		}
	}

	private void FixedUpdate()
	{
		if (this.FixedUpdateEvent != null)
		{
			try
			{
				this.FixedUpdateEvent();
			}
			catch (Exception ex)
			{
				Debug.LogError("MonoHelperFixedUpdate() Error" + ex.Message + "   StackTrace  " + ex.StackTrace);
			}
		}
	}

	public new static void StartCoroutine(IEnumerator routine)
	{
		MonoBehaviour ins = SingletonMono<MonoHelper>.Ins;
		ins.StartCoroutine(routine);
	}
}
