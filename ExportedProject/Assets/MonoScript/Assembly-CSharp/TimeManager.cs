using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
	public delegate void CallFunc();

	private static GameObject gtime;

	private static Dictionary<string, TimeCountDown> TimeCountDownDic = new Dictionary<string, TimeCountDown>();

	private static List<GameObject> TimeCountDownGOList = new List<GameObject>();

	private static Queue<string> RemoveQueue = new Queue<string>();

	public static void Init()
	{
		gtime = new GameObject("TimeManager");
		gtime.AddComponent<TimeManager>();
	}

	private void Update()
	{
		if (TimeCountDownDic.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, TimeCountDown> item in TimeCountDownDic)
		{
			if (item.Value.CheckTime())
			{
				RemoveQueue.Enqueue(item.Key);
			}
		}
		while (RemoveQueue.Count > 0)
		{
			string text = RemoveQueue.Dequeue();
			if (TimeCountDownDic.ContainsKey(text))
			{
				UnregisterCountDown(text);
			}
		}
	}

	public static void RegisterCountDown(string clockName, float remainTime, CallFunc func)
	{
		if (gtime == null)
		{
			gtime = new GameObject("TimeManager");
			gtime.AddComponent<TimeManager>();
		}
		if (!TimeCountDownDic.ContainsKey(clockName))
		{
			TimeCountDown value = new TimeCountDown(remainTime, func);
			TimeCountDownDic.Add(clockName, value);
		}
		else
		{
			TimeCountDown value = TimeCountDownDic[clockName];
			value.remainTime = remainTime;
			value.callfunc = func;
		}
	}

	public static void ExecuteTimeFunc(string clockName)
	{
		TimeCountDown timeCountDown = TimeCountDownByClockName(clockName);
		if (timeCountDown != null)
		{
			if (timeCountDown.callfunc != null)
			{
				timeCountDown.callfunc();
			}
			TimeCountDownDic.Remove(clockName);
		}
	}

	public static void UnregisterCountDown(string clockName)
	{
		if (TimeCountDownDic.ContainsKey(clockName))
		{
			TimeCountDownDic.Remove(clockName);
		}
	}

	public static TimeCountDown TimeCountDownByClockName(string clockName)
	{
		TimeCountDown value;
		if (TimeCountDownDic.TryGetValue(clockName, out value))
		{
			return value;
		}
		return null;
	}

	public static void ShowCountDown(float remainTime, GameObject txtCountDown)
	{
	}

	public static void ShowCountDown(string key, GameObject txtCountDown)
	{
		if (TimeCountDownDic.ContainsKey(key))
		{
			txtCountDown.GetComponent<Text>().text = ((int)TimeCountDownDic[key].remainTime).ToString();
		}
	}

	public static int ShowCountDown(string key, Text txtCountDown, int stringIndex)
	{
		int result = 0;
		if (TimeCountDownDic.ContainsKey(key))
		{
			result = (int)TimeCountDownDic[key].remainTime;
			txtCountDown.text = Utils.GetString(stringIndex, result.ToString());
		}
		return result;
	}

	public static void Clear()
	{
		TimeCountDownDic.Clear();
	}
}
