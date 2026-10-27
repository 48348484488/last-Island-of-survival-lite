using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ButtonEffectMgr : Singleton<ButtonEffectMgr>
{
	[CompilerGenerated]
	private sealed class _003CAddGetEffect_003Ec__AnonStorey0
	{
		internal Action<GameObject> callBack;

		internal GameObject parent;

		internal ButtonEffectMgr _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if ((bool)_0024this._getEffectOrigin)
			{
				_0024this.InitCloneEffect(go, callBack);
				return;
			}
			_0024this._getEffectOrigin = go;
			_0024this._getEffectOrigin.SetActiveBetter(false);
			_0024this.InitCloneEffect(UnityEngine.Object.Instantiate(_0024this._getEffectOrigin, parent.transform), callBack);
		}
	}

	private readonly string GetEffectPath = "effect/ui_lingqu_01.ab";

	private GameObject _getEffectOrigin;

	public void AddGetEffect(GameObject parent, Action<GameObject> callBack = null)
	{
		_003CAddGetEffect_003Ec__AnonStorey0 _003CAddGetEffect_003Ec__AnonStorey = new _003CAddGetEffect_003Ec__AnonStorey0();
		_003CAddGetEffect_003Ec__AnonStorey.callBack = callBack;
		_003CAddGetEffect_003Ec__AnonStorey.parent = parent;
		_003CAddGetEffect_003Ec__AnonStorey._0024this = this;
		if ((bool)_getEffectOrigin)
		{
			InitCloneEffect(UnityEngine.Object.Instantiate(_getEffectOrigin, _003CAddGetEffect_003Ec__AnonStorey.parent.transform), _003CAddGetEffect_003Ec__AnonStorey.callBack);
		}
		else
		{
			ResMgr.Ins.CreateFromAB(GetEffectPath, null, _003CAddGetEffect_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private void InitCloneEffect(GameObject cloneEffect, Action<GameObject> callBack = null)
	{
		Transform transform = cloneEffect.transform;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
		cloneEffect.SetActiveBetter(true);
		if (callBack != null)
		{
			callBack(cloneEffect);
		}
	}
}
