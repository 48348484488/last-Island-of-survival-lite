using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
	private float fEllapseTime;

	private float fFps;

	private float fLastRealtime;

	private int m_iLastFrameCount;

	private Text _fps;

	private Ping lastPing;

	private int time;

	private void Start()
	{
		_fps = GameObject.Find("FPS").GetComponent<Text>();
		StartCoroutine(Tick());
	}

	private void Update()
	{
		if (fLastRealtime == 0f)
		{
			fLastRealtime = Time.realtimeSinceStartup;
		}
		float num = Time.realtimeSinceStartup - fLastRealtime;
		fEllapseTime += num;
		if (fEllapseTime > 1f)
		{
			fFps = Mathf.CeilToInt((float)(Time.frameCount - m_iLastFrameCount) / fEllapseTime);
			m_iLastFrameCount = Time.frameCount;
			fEllapseTime = 0f;
			_fps.text = fFps.ToString() + "  " + time;
		}
		fLastRealtime = Time.realtimeSinceStartup;
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			TickDelay();
			yield return new WaitForSeconds(1f);
		}
	}

	private void TickDelay()
	{
	}
}
