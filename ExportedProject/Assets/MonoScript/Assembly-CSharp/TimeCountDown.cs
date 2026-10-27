using System;
using UnityEngine;

public class TimeCountDown
{
	public float remainTime;

	public TimeManager.CallFunc callfunc;

	public GameObject TimeGameObject;

	public TimeCountDown(float time, TimeManager.CallFunc func)
	{
		remainTime = time;
		callfunc = func;
	}

	public bool checkAlive()
	{
		return TimeGameObject != null;
	}

	public bool CheckTime()
	{
		remainTime -= Time.deltaTime;
		if (remainTime < 0f)
		{
			try
			{
				if (callfunc != null)
				{
					callfunc();
				}
			}
			catch (Exception message)
			{
				Debug.Log(message);
				return true;
			}
			return true;
		}
		return false;
	}
}
