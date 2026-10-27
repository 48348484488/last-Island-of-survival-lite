using System;
using System.Collections.Generic;
using UnityEngine;

public class FacilityStatusEffectData : MonoBehaviour
{
	[Serializable]
	public class MyEffectInfos
	{
		[SerializeField]
		private int _status;

		[SerializeField]
		private List<GameObject> _levelEffects;

		public int Status
		{
			get
			{
				return _status;
			}
		}

		public List<GameObject> LevelEffects
		{
			get
			{
				return _levelEffects;
			}
		}
	}

	private readonly Dictionary<int, List<GameObject>> _dicStatus2Effects = new Dictionary<int, List<GameObject>>();

	private GameObject _lastEffect;

	[SerializeField]
	private List<MyEffectInfos> _effectInfos;

	private void Awake()
	{
		if (_effectInfos != null)
		{
			int i = 0;
			for (int count = _effectInfos.Count; i < count; i++)
			{
				MyEffectInfos myEffectInfos = _effectInfos[i];
				_dicStatus2Effects.Add(myEffectInfos.Status, myEffectInfos.LevelEffects);
			}
		}
	}

	public void StatusChange(int status, int index)
	{
		if ((bool)_lastEffect)
		{
			_lastEffect.SetActiveBetter(false);
		}
		List<GameObject> value;
		if (_dicStatus2Effects.TryGetValue(status, out value) && index > -1 && value.Count > index)
		{
			value[index].SetActiveBetter(true);
			_lastEffect = value[index];
		}
	}
}
