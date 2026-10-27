using UnityEngine;

public class TweenTime : MonoBehaviour
{
	public Utils.VoidDelegate VoidDelegate;

	public Utils.VoidDelegate FinishDelegate;

	public float duration;

	public bool canExecute;

	public static void Begin(GameObject go, float duration, Utils.VoidDelegate voidDelegate, Utils.VoidDelegate finishDelegate = null)
	{
		TweenTime tweenTime = go.GetComponent<TweenTime>();
		if (!tweenTime)
		{
			tweenTime = go.AddComponent<TweenTime>();
		}
		tweenTime.VoidDelegate = voidDelegate;
		if (tweenTime.duration <= 0f)
		{
			tweenTime.canExecute = true;
			tweenTime.duration = duration;
			tweenTime.FinishDelegate = finishDelegate;
		}
	}

	public static void Quit(GameObject go)
	{
		TweenTime tweenTime = go.GetComponent<TweenTime>();
		if (!tweenTime)
		{
			tweenTime = go.AddComponent<TweenTime>();
		}
		tweenTime.canExecute = false;
		tweenTime.duration = 0f;
		tweenTime.VoidDelegate = null;
		tweenTime.FinishDelegate = null;
	}

	private void Update()
	{
		if (canExecute)
		{
			if (VoidDelegate != null)
			{
				VoidDelegate();
			}
			canExecute = false;
		}
		if (!(duration <= 0f))
		{
			duration -= Time.deltaTime;
			if (duration <= 0f)
			{
				Utils.TriggerEvent(FinishDelegate);
			}
		}
	}

	private void OnDisable()
	{
		duration = -1f;
	}
}
