using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Worker
{
	public static readonly Worker Ins = new Worker();

	private Thread mThread;

	private readonly List<Utils.VoidDelegate> mQueue = new List<Utils.VoidDelegate>();

	private Worker()
	{
	}

	public void Init()
	{
		if (mThread == null)
		{
			mThread = new Thread(Run);
			mThread.Start();
		}
	}

	public bool Insert(Utils.VoidDelegate func, bool bNotAllowSameTask = false)
	{
		bool result = true;
		lock (mQueue)
		{
			if (bNotAllowSameTask)
			{
				if (mQueue.Contains(func))
				{
					result = false;
				}
				else
				{
					mQueue.Insert(0, func);
				}
			}
			else
			{
				mQueue.Insert(0, func);
			}
			if (mQueue.Count == 1)
			{
				Monitor.Pulse(mQueue);
				return result;
			}
			return result;
		}
	}

	public bool Execute(Utils.VoidDelegate func, bool bNotAllowSameTask = false)
	{
		bool result = true;
		lock (mQueue)
		{
			if (bNotAllowSameTask)
			{
				if (mQueue.Contains(func))
				{
					result = false;
				}
				else
				{
					mQueue.Add(func);
				}
			}
			else
			{
				mQueue.Add(func);
			}
			if (mQueue.Count == 1)
			{
				Monitor.Pulse(mQueue);
				return result;
			}
			return result;
		}
	}

	public void Destroy()
	{
		if (mThread != null)
		{
			mThread.Abort();
			while (mThread.IsAlive)
			{
				Thread.Sleep(1);
			}
			mThread = null;
		}
		lock (mQueue)
		{
			mQueue.Clear();
		}
	}

	private void Run()
	{
		while (true)
		{
			Utils.VoidDelegate voidDelegate = null;
			lock (mQueue)
			{
				if (mQueue.Count > 0)
				{
					voidDelegate = mQueue[0];
					mQueue.RemoveAt(0);
				}
				else
				{
					Monitor.Wait(mQueue);
				}
			}
			if (voidDelegate != null)
			{
				try
				{
					voidDelegate();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
	}
}
