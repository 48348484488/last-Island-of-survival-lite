using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeDown : MonoBehaviour
{
	public static Text mText;

	public Action downFinish;

	public Action<float> updateTimeString;

	public float remainTime;

	private Coroutine _setTimeTxtCoroutine;

	private void Start()
	{
		if (_setTimeTxtCoroutine != null)
		{
			StopCoroutine(_setTimeTxtCoroutine);
		}
		StartCoroutine(SetTimeTxt());
	}

	private void OnEnable()
	{
	}

	public static void ShowCountDown(float remainTime, GameObject go, GameObject text, Action<float> updateTime, Action downFinish = null)
	{
		TimeDown component = go.GetComponent<TimeDown>();
		if ((bool)component)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		go.AddComponent<TimeDown>();
		component = go.GetComponent<TimeDown>();
		component.downFinish = downFinish;
		component.updateTimeString = updateTime;
		component.remainTime = remainTime;
		mText = text.GetComponent<Text>();
	}

	public IEnumerator SetTimeTxt()
	{
		while (!(Math.Abs(remainTime) <= 0f))
		{
			updateTimeString(remainTime);
			yield return new WaitForSeconds(1f);
			remainTime -= 1f;
		}
		if (downFinish != null)
		{
			downFinish();
		}
	}
}
