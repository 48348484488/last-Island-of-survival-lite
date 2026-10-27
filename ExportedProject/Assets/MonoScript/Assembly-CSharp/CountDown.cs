using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
	private Utils.VoidDelegate _delegate;

	private float _time = -1f;

	private bool _isCountDown;

	private Text _text;

	public static void RegistTime(GameObject go, float time, Utils.VoidDelegate voiddelegate = null)
	{
		CountDown countDown = go.GetComponent<CountDown>();
		if (countDown == null)
		{
			countDown = go.AddComponent<CountDown>();
		}
		countDown.RegistCountDown(time, voiddelegate);
	}

	public static void CancleTime(GameObject go)
	{
		CountDown countDown = go.GetComponent<CountDown>();
		if (countDown == null)
		{
			countDown = go.AddComponent<CountDown>();
		}
		countDown.CloseCountDown();
	}

	private void Awake()
	{
		_text = GetComponent<Text>();
	}

	public void RegistCountDown(float time, Utils.VoidDelegate voiddelegate = null)
	{
		_time = time;
		_delegate = voiddelegate;
		if (_time > 0f)
		{
			_isCountDown = true;
		}
	}

	public void CloseCountDown()
	{
		_time = -1f;
	}

	private void Update()
	{
		if (_time > 0f)
		{
			_time -= Time.deltaTime;
			if (_time <= 0f)
			{
				Utils.TriggerEvent(_delegate);
			}
		}
		_text.text = ((int)_time).ToString();
	}
}
