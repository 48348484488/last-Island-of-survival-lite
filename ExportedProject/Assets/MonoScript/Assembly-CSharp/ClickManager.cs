using UnityEngine;

public class ClickManager : SingletonMono<ClickManager>
{
	public float LimitTime;

	private void Update()
	{
		if (LimitTime > 0f)
		{
			LimitTime -= Time.deltaTime;
		}
	}
}
