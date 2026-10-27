using System.Collections.Generic;
using SC.UI;

public class AlertMgr : Singleton<AlertMgr>
{
	private List<string> _strs = new List<string>();

	public bool _showing;

	private int _maxCount = 10;

	public int Count
	{
		get
		{
			return _strs.Count;
		}
	}

	public float SpaceTime
	{
		get
		{
			return 1.1f - (float)_strs.Count * 0.1f;
		}
	}

	public void Add(string alertStr)
	{
		if (!string.IsNullOrEmpty(alertStr))
		{
			if (_strs.Count >= _maxCount)
			{
				_strs.RemoveAt(0);
			}
			_strs.Add(alertStr);
			if (!_showing)
			{
				ShowAlertPanel();
			}
		}
	}

	public void ShowAlertPanel()
	{
		if (_strs.Count > 0)
		{
			ViewMgr.Ins.ShowTopView<AlertBox>(_strs[0]);
		}
	}

	public void CloseAlertPanel()
	{
		if (_strs.Count > 0)
		{
			_strs.RemoveAt(0);
		}
		if (_strs.Count <= 0)
		{
			_showing = false;
		}
		else
		{
			ShowAlertPanel();
		}
	}
}
