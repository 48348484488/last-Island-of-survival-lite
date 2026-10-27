using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using cfg;
using gs.battle.drop.scmsg;

public class TrashcanStationMgr : Singleton<TrashcanStationMgr>
{
	[CompilerGenerated]
	private sealed class _003CCreateItem_003Ec__AnonStorey0
	{
		internal int typeId;

		internal long instanceId;

		internal SShowTrashcanStation showTrashcanData;

		internal TrashcanStationMgr _0024this;
	}

	[CompilerGenerated]
	private sealed class _003CCreateItem_003Ec__AnonStorey1
	{
		private sealed class _003CCreateItem_003Ec__AnonStorey2
		{
			internal GameObject o;

			internal _003CCreateItem_003Ec__AnonStorey0 _003C_003Ef__ref_00240;

			internal _003CCreateItem_003Ec__AnonStorey1 _003C_003Ef__ref_00241;

			internal TrashcanStationGameObject _003C_003Em__0()
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(o);
				TrashcanStationGameObject trashcanStationGameObject = gameObject.GetComponent<TrashcanStationGameObject>();
				if (trashcanStationGameObject == null)
				{
					trashcanStationGameObject = gameObject.AddComponent<TrashcanStationGameObject>();
				}
				trashcanStationGameObject.RecycleTrashGameObject();
				return trashcanStationGameObject;
			}

			internal void _003C_003Em__1(TrashcanStationGameObject o2)
			{
				if ((bool)o2)
				{
					o2.transform.position = new Vector3(0f, -1000f, 0f);
					o2.gameObject.SetActive(false);
					TrashcanStationGameObject component = o2.GetComponent<TrashcanStationGameObject>();
					if (component != null)
					{
						_003C_003Ef__ref_00240._0024this.OnStationDestroySetGroundItem(component.InsId);
					}
				}
			}
		}

		internal ObjectPool<TrashcanStationGameObject> pool;

		internal _003CCreateItem_003Ec__AnonStorey0 _003C_003Ef__ref_00240;

		internal void _003C_003Em__0(GameObject o)
		{
			_003CCreateItem_003Ec__AnonStorey2 _003CCreateItem_003Ec__AnonStorey = new _003CCreateItem_003Ec__AnonStorey2();
			_003CCreateItem_003Ec__AnonStorey._003C_003Ef__ref_00240 = _003C_003Ef__ref_00240;
			_003CCreateItem_003Ec__AnonStorey._003C_003Ef__ref_00241 = this;
			_003CCreateItem_003Ec__AnonStorey.o = o;
			_003CCreateItem_003Ec__AnonStorey.o.SetActive(false);
			GameObject gameObject = UnityEngine.Object.Instantiate(_003CCreateItem_003Ec__AnonStorey.o);
			TrashcanStationGameObject trashcanStationGameObject = gameObject.GetComponent<TrashcanStationGameObject>();
			if (trashcanStationGameObject == null)
			{
				trashcanStationGameObject = gameObject.AddComponent<TrashcanStationGameObject>();
			}
			if (!_003C_003Ef__ref_00240._0024this._itemId2Pools.TryGetValue(_003C_003Ef__ref_00240.typeId, out pool))
			{
				pool = new ObjectPool<TrashcanStationGameObject>(_003C_003Ef__ref_00240._0024this._poolSize, _003CCreateItem_003Ec__AnonStorey._003C_003Em__0, _003C_003Ef__ref_00240._0024this.DestroyFunc, _003CCreateItem_003Ec__AnonStorey._003C_003Em__1);
				_003C_003Ef__ref_00240._0024this._itemId2Pools.Set(_003C_003Ef__ref_00240.typeId, pool);
			}
			if (!_003C_003Ef__ref_00240._0024this.TrashcanStationData.ContainsKey(_003C_003Ef__ref_00240.instanceId) && !_003C_003Ef__ref_00240._0024this.TrashcanStationData.ContainsKey(_003C_003Ef__ref_00240.instanceId) && _003C_003Ef__ref_00240.typeId > 0)
			{
				pool.Recycle(trashcanStationGameObject);
			}
			else
			{
				_003C_003Ef__ref_00240._0024this.InitData(trashcanStationGameObject, _003C_003Ef__ref_00240.showTrashcanData);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CSBlowTrashcanStationHandle_003Ec__AnonStorey3
	{
		internal SBlowTrashcanStation msg;

		internal TrashcanStationMgr _0024this;

		internal void _003C_003Em__0()
		{
			_0024this.RemoveTrashcanStation(msg.instanceId);
		}
	}

	private string _pathFormat = "model/{0}.ab";

	private readonly LRU<int, ObjectPool<TrashcanStationGameObject>> _itemId2Pools = new LRU<int, ObjectPool<TrashcanStationGameObject>>(32);

	public Dictionary<long, SShowTrashcanStation> TrashcanStationData = new Dictionary<long, SShowTrashcanStation>();

	public Dictionary<long, TrashcanStationGameObject> TranscanStationGameObject = new Dictionary<long, TrashcanStationGameObject>();

	private int _poolSize = 16;

	private readonly Dictionary<long, List<Transform>> _dicStationInsId2GroundItem = new Dictionary<long, List<Transform>>();

	public void Init()
	{
		SShowTrashcanStation.handler = (SShowTrashcanStation.Handler)Delegate.Combine(SShowTrashcanStation.handler, new SShowTrashcanStation.Handler(SShowTrashcanStationHandle));
		SHideTrashcanStation.handler = (SHideTrashcanStation.Handler)Delegate.Combine(SHideTrashcanStation.handler, new SHideTrashcanStation.Handler(SHideTrashcanStationHandle));
		SBlowTrashcanStation.handler = (SBlowTrashcanStation.Handler)Delegate.Combine(SBlowTrashcanStation.handler, new SBlowTrashcanStation.Handler(SBlowTrashcanStationHandle));
		TrashcanStationEvent.DestoryTrashcanStation = (Utils.LongDelegate)Delegate.Combine(TrashcanStationEvent.DestoryTrashcanStation, new Utils.LongDelegate(DestoryTrashcanStation));
		LRU<int, ObjectPool<TrashcanStationGameObject>> itemId2Pools = _itemId2Pools;
		itemId2Pools.onRemoveEntry = (LRU<int, ObjectPool<TrashcanStationGameObject>>.OnRemoveEntry)Delegate.Combine(itemId2Pools.onRemoveEntry, new LRU<int, ObjectPool<TrashcanStationGameObject>>.OnRemoveEntry(OnRemoveCache));
	}

	private bool OnRemoveCache(int typeId, ObjectPool<TrashcanStationGameObject> value)
	{
		value.Clear();
		return true;
	}

	public void Clear()
	{
		_itemId2Pools.Clear();
	}

	private void CreateItem(SShowTrashcanStation showTrashcanData)
	{
		_003CCreateItem_003Ec__AnonStorey0 _003CCreateItem_003Ec__AnonStorey = new _003CCreateItem_003Ec__AnonStorey0();
		_003CCreateItem_003Ec__AnonStorey.showTrashcanData = showTrashcanData;
		_003CCreateItem_003Ec__AnonStorey._0024this = this;
		_003CCreateItem_003Ec__AnonStorey.typeId = _003CCreateItem_003Ec__AnonStorey.showTrashcanData.typeId;
		_003CCreateItem_003Ec__AnonStorey.instanceId = _003CCreateItem_003Ec__AnonStorey.showTrashcanData.instanceId;
		TrashcanStationCfg trashcanStationCfg = TrashcanStationCfg.Get(_003CCreateItem_003Ec__AnonStorey.typeId);
		if (string.IsNullOrEmpty(trashcanStationCfg.name))
		{
			Debug.LogError("typeId : " + _003CCreateItem_003Ec__AnonStorey.typeId + " modelPath is null or empty.");
			return;
		}
		_003CCreateItem_003Ec__AnonStorey1 _003CCreateItem_003Ec__AnonStorey2 = new _003CCreateItem_003Ec__AnonStorey1();
		_003CCreateItem_003Ec__AnonStorey2._003C_003Ef__ref_00240 = _003CCreateItem_003Ec__AnonStorey;
		if (_itemId2Pools.TryGetValue(_003CCreateItem_003Ec__AnonStorey.typeId, out _003CCreateItem_003Ec__AnonStorey2.pool))
		{
			TrashcanStationGameObject trashcanStationGameObject = _003CCreateItem_003Ec__AnonStorey2.pool.Get();
			if ((bool)trashcanStationGameObject)
			{
				InitData(trashcanStationGameObject, _003CCreateItem_003Ec__AnonStorey.showTrashcanData);
			}
		}
		else
		{
			string abPath = string.Format(_pathFormat, trashcanStationCfg.name);
			ResMgr.Ins.CreateFromAB(abPath, null, _003CCreateItem_003Ec__AnonStorey2._003C_003Em__0);
		}
	}

	private void DestroyFunc(TrashcanStationGameObject go)
	{
		try
		{
			go.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(go.gameObject, 0.5f);
		}
		catch (Exception)
		{
		}
	}

	private void DestoryTrashcanStation(long instanceId)
	{
		RemoveTrashcanStationData(instanceId);
	}

	private void SBlowTrashcanStationHandle(SBlowTrashcanStation msg)
	{
		_003CSBlowTrashcanStationHandle_003Ec__AnonStorey3 _003CSBlowTrashcanStationHandle_003Ec__AnonStorey = new _003CSBlowTrashcanStationHandle_003Ec__AnonStorey3();
		_003CSBlowTrashcanStationHandle_003Ec__AnonStorey.msg = msg;
		_003CSBlowTrashcanStationHandle_003Ec__AnonStorey._0024this = this;
		TrashcanStationGameObject value;
		if (TranscanStationGameObject.TryGetValue(_003CSBlowTrashcanStationHandle_003Ec__AnonStorey.msg.instanceId, out value))
		{
			Transform transform = value.transform;
			float y = transform.position.y;
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos((value.CfgId >= 30007) ? "effect/ef_lajidui_xiao.ab" : "effect/ef_lajidui_da.ab", transform.position);
			transform.DOMoveY(y - 1f, 1f).OnComplete(_003CSBlowTrashcanStationHandle_003Ec__AnonStorey._003C_003Em__0);
		}
		else
		{
			RemoveTrashcanStation(_003CSBlowTrashcanStationHandle_003Ec__AnonStorey.msg.instanceId);
		}
	}

	private void SHideTrashcanStationHandle(SHideTrashcanStation msg)
	{
		RemoveTrashcanStation(msg.instanceId);
	}

	private void RemoveTrashcanStation(long instanceId)
	{
		SShowTrashcanStation value;
		if (!TrashcanStationData.TryGetValue(instanceId, out value))
		{
			return;
		}
		TrashcanStationData.Remove(instanceId);
		TrashcanStationGameObject value2;
		if (TranscanStationGameObject.TryGetValue(instanceId, out value2))
		{
			ObjectPool<TrashcanStationGameObject> value3;
			if (_itemId2Pools.TryGetValue(value.typeId, out value3))
			{
				value2.RecycleTrashGameObject();
				value3.Recycle(value2);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(value2.gameObject);
			}
			TranscanStationGameObject.Remove(instanceId);
		}
	}

	private void RemoveTrashcanStationData(long instanceId)
	{
	}

	private void SShowTrashcanStationHandle(SShowTrashcanStation msg)
	{
		TrashcanStationData[msg.instanceId] = msg;
		if (TranscanStationGameObject.ContainsKey(msg.instanceId))
		{
			TrashcanStationGameObject component = TranscanStationGameObject[msg.instanceId].GetComponent<TrashcanStationGameObject>();
			component.SetData(msg);
		}
		else
		{
			CreateItem(msg);
		}
	}

	private void SetData(GameObject go, SShowTrashcanStation data)
	{
		TrashcanStationGameObject trashcanStationGameObject = go.GetComponent<TrashcanStationGameObject>();
		if (trashcanStationGameObject == null)
		{
			trashcanStationGameObject = go.AddComponent<TrashcanStationGameObject>();
		}
		trashcanStationGameObject.SetData(data);
	}

	private void InitData(TrashcanStationGameObject trashcanStationGameObject, SShowTrashcanStation data)
	{
		trashcanStationGameObject.gameObject.SetActiveBetter(true);
		TranscanStationGameObject[data.instanceId] = trashcanStationGameObject;
		trashcanStationGameObject.SetData(data);
	}

	public void AddGroundItem2TrashcanStation(long stationInsId, Transform groundItem)
	{
		List<Transform> value;
		if (!_dicStationInsId2GroundItem.TryGetValue(stationInsId, out value))
		{
			value = new List<Transform>();
			_dicStationInsId2GroundItem[stationInsId] = value;
		}
		value.Add(groundItem);
	}

	private void OnStationDestroySetGroundItem(long stationInsId)
	{
		List<Transform> value;
		if (!_dicStationInsId2GroundItem.TryGetValue(stationInsId, out value))
		{
			return;
		}
		int i = 0;
		for (int count = value.Count; i < count; i++)
		{
			Transform transform = value[i];
			if ((bool)transform)
			{
				Singleton<BattleDropMgr>.Ins.SetObjectToGround(transform);
			}
		}
		_dicStationInsId2GroundItem.Remove(stationInsId);
	}
}
