using UnityEngine;

public class DayNightSystem : SingletonMono<DayNightSystem>
{
	public Animator myAnimator;

	public const float ArtTime = 24f;

	private double _gameTime;

	private double _lastHour = -1.0;

	private const float TimeRate = 24f;

	public Color HuanjingLight;

	public Color fog;

	public float fogStartDistance;

	public float fogEndDistance;

	public Color waterfog;

	public float waterfogStartDistance;

	public float waterfogEndDistance;

	public Color skyBoxColor;

	public float exposure;

	private float _timeSpace = 0.5f;

	[HideInInspector]
	public bool InWater;

	public bool IsNight
	{
		get
		{
			if (GetGameTime() / 3600.0 >= 21.0 || GetGameTime() / 3600.0 <= 5.0)
			{
				return true;
			}
			return false;
		}
	}

	private new void Awake()
	{
		PlayCurAim();
	}

	public void PlayCurAim()
	{
		myAnimator.Play("tianqixitong_01", 0, GetPassDayPercent());
	}

	public float GetPassDayPercent()
	{
		return (float)(GetGameTime() / 86400.0);
	}

	public double GetGameTime()
	{
		_gameTime = (float)Singleton<RechargeMgr>.Ins.realSencondFromServer * 24f % 86400f;
		return _gameTime;
	}

	public void CheckQDHN()
	{
		double num = GetGameTime() / 3600.0;
		if (num >= 21.0 && _lastHour < 21.0)
		{
			if (_lastHour != 0.0)
			{
				_lastHour = 0.0;
				Utils.TriggerEvent(BattleEvent.OnChangeNight);
			}
		}
		else if (num >= 19.0 && _lastHour < 19.0)
		{
			_lastHour = num;
			Utils.TriggerEvent(BattleEvent.OnChangeHuanghun);
		}
		else if (num >= 7.0 && _lastHour < 7.0)
		{
			_lastHour = num;
			Utils.TriggerEvent(BattleEvent.OnChangeDay);
		}
		else if (num >= 5.0 && _lastHour < 5.0)
		{
			_lastHour = num;
			Utils.TriggerEvent(BattleEvent.OnChangeQingchen);
		}
		else if (num >= 0.0 && _lastHour < 0.0)
		{
			_lastHour = num;
			Utils.TriggerEvent(BattleEvent.OnChangeNight);
		}
	}

	private void Update()
	{
		_timeSpace -= Time.deltaTime;
		if (_timeSpace < 0f)
		{
			RenderSettings.ambientLight = HuanjingLight;
			RenderSettings.skybox.SetColor("_Tint", skyBoxColor);
			RenderSettings.skybox.SetFloat("_Exposure", exposure);
			if (!InWater)
			{
				RenderSettings.fogColor = fog;
				RenderSettings.fogStartDistance = fogStartDistance;
				RenderSettings.fogEndDistance = fogEndDistance;
			}
			else
			{
				RenderSettings.fogColor = waterfog;
				RenderSettings.fogStartDistance = waterfogStartDistance;
				RenderSettings.fogEndDistance = waterfogEndDistance;
			}
			_timeSpace = 0.5f;
		}
	}
}
