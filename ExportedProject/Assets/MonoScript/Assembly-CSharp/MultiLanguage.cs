using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiLanguage : MonoBehaviour
{
	private Text[] _labels;

	private string[] _labelsKeys;

	private Dropdown[] _dropdowns;

	private int _length;

	private void Awake()
	{
		_labels = base.gameObject.GetComponentsInChildren<Text>(true);
		_length = _labels.Length;
		_labelsKeys = new string[_length];
		for (int i = 0; i < _length; i++)
		{
			_labelsKeys[i] = _labels[i].text;
		}
		SetLable();
		_dropdowns = base.gameObject.GetComponentsInChildren<Dropdown>(true);
		SetDropdowns();
		MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Combine(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(RefreshFixedLableDelegate));
	}

	private void RefreshFixedLableDelegate()
	{
		SetLable();
		SetDropdowns();
	}

	public void SetLable()
	{
		for (int i = 0; i < _length; i++)
		{
			View.SetLabelText(_labels[i], _labelsKeys[i]);
		}
	}

	public void SetDropdowns()
	{
		for (int i = 0; i < _dropdowns.Length; i++)
		{
			for (int j = 0; j < _dropdowns[i].options.Count; j++)
			{
				_dropdowns[i].options[j].text = Singleton<MultiLanguageMgr>.Ins.GetLanguage(_dropdowns[i].options[j].text);
			}
		}
	}

	private void Destory()
	{
		MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Remove(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(RefreshFixedLableDelegate));
	}
}
