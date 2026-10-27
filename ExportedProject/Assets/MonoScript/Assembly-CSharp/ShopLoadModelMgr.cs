using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class ShopLoadModelMgr : Singleton<ShopLoadModelMgr>
{
	[CompilerGenerated]
	private sealed class _003CGetModel_003Ec__AnonStorey0
	{
		internal int itemId;

		internal Action<int, GameObject> callBack;

		internal ShopLoadModelMgr _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			_0024this._itemId2ModelLru.Set(itemId, go);
			callBack(itemId, go);
		}
	}

	private readonly LRU<int, GameObject> _itemId2ModelLru = new LRU<int, GameObject>();

	private readonly HashSet<int> _setInLoading = new HashSet<int>();

	public void GetModel(int itemId, Action<int, GameObject> callBack)
	{
		ItemCfg itemInfo = ItemCfg.Get(itemId);
		GetModel(itemInfo, callBack);
	}

	public void GetModel(ItemCfg itemInfo, Action<int, GameObject> callBack)
	{
		_003CGetModel_003Ec__AnonStorey0 _003CGetModel_003Ec__AnonStorey = new _003CGetModel_003Ec__AnonStorey0();
		_003CGetModel_003Ec__AnonStorey.callBack = callBack;
		_003CGetModel_003Ec__AnonStorey._0024this = this;
		if (itemInfo == null)
		{
			Debug.LogError("[ShopLoadModelMgr.cs]GetModel:itemInfo is null.");
			return;
		}
		_003CGetModel_003Ec__AnonStorey.itemId = itemInfo.id;
		GameObject value;
		if (_itemId2ModelLru.TryGetValue(_003CGetModel_003Ec__AnonStorey.itemId, out value))
		{
			_003CGetModel_003Ec__AnonStorey.callBack(_003CGetModel_003Ec__AnonStorey.itemId, value);
		}
		else if (!_setInLoading.Contains(_003CGetModel_003Ec__AnonStorey.itemId))
		{
			_setInLoading.Add(_003CGetModel_003Ec__AnonStorey.itemId);
			ResMgr.Ins.CreateFromAB(itemInfo.modelPath, null, _003CGetModel_003Ec__AnonStorey._003C_003Em__0);
		}
	}
}
