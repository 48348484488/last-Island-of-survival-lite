using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using cfg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class BattleDropMgr : Singleton<BattleDropMgr>
{
	[CompilerGenerated]
	private sealed class _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey4
	{
		internal Action<TrashcanInfo, GameObject> initObjAct;

		internal BattleDropMgr _0024this;
	}

	[CompilerGenerated]
	private sealed class _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2
	{
		private sealed class _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey3
		{
			internal GameObject o;

			internal _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey4 _003C_003Ef__ref_00244;

			internal _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2 _003C_003Ef__ref_00242;

			internal GameObject _003C_003Em__0()
			{
				return UnityEngine.Object.Instantiate(o);
			}
		}

		internal TrashcanInfo curTrashcanInfo;

		internal ObjectPool<GameObject> pool;

		internal _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey4 _003C_003Ef__ref_00244;

		private static ObjectPool<GameObject>.RecycleObject<GameObject> _003C_003Ef__am_0024cache0;

		internal void _003C_003Em__0(GameObject o)
		{
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey3 _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey = new _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey3();
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey._003C_003Ef__ref_00244 = _003C_003Ef__ref_00244;
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey._003C_003Ef__ref_00242 = this;
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.o = o;
			int num = -curTrashcanInfo.typeId;
			if (!_003C_003Ef__ref_00244._0024this._itemId2Pools.TryGetValue(num, out pool))
			{
				_003C_003Ef__ref_00244._0024this._itemId2Origin[num] = _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.o;
				int num2 = ((num != -20002 && num != -20001) ? 1 : 4);
				int capacity = num2 * _003C_003Ef__ref_00244._0024this.RandomBoxPoolSize;
				ObjectPool<GameObject>.CreateObject<GameObject> createFun = _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey._003C_003Em__0;
				ObjectPool<GameObject>.DestroyObject<GameObject> destroyFun = _003C_003Ef__ref_00244._0024this.DestroyFunc;
				if (_003C_003Ef__am_0024cache0 == null)
				{
					_003C_003Ef__am_0024cache0 = _003C_003Em__1;
				}
				pool = new ObjectPool<GameObject>(capacity, createFun, destroyFun, _003C_003Ef__am_0024cache0);
				_003C_003Ef__ref_00244._0024this._itemId2Pools.Set(num, pool);
				_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.o.SetActive(false);
			}
			else
			{
				pool.Recycle(_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.o);
			}
			GameObject gameObject = pool.Get();
			_003C_003Ef__ref_00244._0024this.InitLaJiZhanTong(gameObject, curTrashcanInfo);
			_003C_003Ef__ref_00244._0024this._instanceId2GameObject[curTrashcanInfo.instanceId] = gameObject;
			_003C_003Ef__ref_00244.initObjAct(curTrashcanInfo, gameObject);
		}

		private static void _003C_003Em__1(GameObject o2)
		{
			if ((bool)o2)
			{
				o2.transform.position = new Vector3(0f, -1000f, 0f);
				o2.SetActive(false);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CCreateItem_003Ec__AnonStorey5
	{
		internal int id;

		internal int poolSize;

		internal long objId;

		internal Vec3 vec;

		internal BattleDropMgr _0024this;
	}

	[CompilerGenerated]
	private sealed class _003CCreateItem_003Ec__AnonStorey6
	{
		private sealed class _003CCreateItem_003Ec__AnonStorey7
		{
			internal GameObject arg;

			internal _003CCreateItem_003Ec__AnonStorey5 _003C_003Ef__ref_00245;

			internal _003CCreateItem_003Ec__AnonStorey6 _003C_003Ef__ref_00246;

			internal GameObject _003C_003Em__0()
			{
				return UnityEngine.Object.Instantiate(arg);
			}
		}

		internal ObjectPool<GameObject> pool;

		internal _003CCreateItem_003Ec__AnonStorey5 _003C_003Ef__ref_00245;

		private static ObjectPool<GameObject>.RecycleObject<GameObject> _003C_003Ef__am_0024cache0;

		internal void _003C_003Em__0(GameObject o)
		{
			_003CCreateItem_003Ec__AnonStorey7 _003CCreateItem_003Ec__AnonStorey = new _003CCreateItem_003Ec__AnonStorey7();
			_003CCreateItem_003Ec__AnonStorey._003C_003Ef__ref_00245 = _003C_003Ef__ref_00245;
			_003CCreateItem_003Ec__AnonStorey._003C_003Ef__ref_00246 = this;
			o.transform.SetParent(_003C_003Ef__ref_00245._0024this.DropParent);
			o.SetActive(false);
			_003CCreateItem_003Ec__AnonStorey.arg = o;
			if (!_003C_003Ef__ref_00245._0024this._itemId2Pools.TryGetValue(_003C_003Ef__ref_00245.id, out pool))
			{
				int num = ((_003C_003Ef__ref_00245.id != -20002 && _003C_003Ef__ref_00245.id != -20001) ? 1 : 4);
				int capacity = num * _003C_003Ef__ref_00245.poolSize;
				ObjectPool<GameObject>.CreateObject<GameObject> createFun = _003CCreateItem_003Ec__AnonStorey._003C_003Em__0;
				ObjectPool<GameObject>.DestroyObject<GameObject> destroyFun = _003C_003Ef__ref_00245._0024this.DestroyFunc;
				if (_003C_003Ef__am_0024cache0 == null)
				{
					_003C_003Ef__am_0024cache0 = _003C_003Em__1;
				}
				pool = new ObjectPool<GameObject>(capacity, createFun, destroyFun, _003C_003Ef__am_0024cache0);
				_003C_003Ef__ref_00245._0024this._itemId2Origin[_003C_003Ef__ref_00245.id] = _003CCreateItem_003Ec__AnonStorey.arg;
				_003C_003Ef__ref_00245._0024this._itemId2Pools.Set(_003C_003Ef__ref_00245.id, pool);
			}
			else
			{
				pool.Recycle(_003CCreateItem_003Ec__AnonStorey.arg);
			}
			if (!_003C_003Ef__ref_00245._0024this._instanceId2Info.ContainsKey(_003C_003Ef__ref_00245.objId) && !_003C_003Ef__ref_00245._0024this._dInstanceId2Info.ContainsKey(_003C_003Ef__ref_00245.objId) && _003C_003Ef__ref_00245.id > 0)
			{
				return;
			}
			if (_003C_003Ef__ref_00245._0024this._instanceId2GameObject.ContainsKey(_003C_003Ef__ref_00245.objId))
			{
				GameObject go = _003C_003Ef__ref_00245._0024this._instanceId2GameObject[_003C_003Ef__ref_00245.objId];
				_003C_003Ef__ref_00245._0024this.InitGameObject(ref go, _003C_003Ef__ref_00245.objId, _003C_003Ef__ref_00245.id, _003C_003Ef__ref_00245.vec);
				return;
			}
			GameObject go2 = pool.Get();
			_003C_003Ef__ref_00245._0024this.InitGameObject(ref go2, _003C_003Ef__ref_00245.objId, _003C_003Ef__ref_00245.id, _003C_003Ef__ref_00245.vec);
			if (_003C_003Ef__ref_00245._0024this.CalDistance(_003C_003Ef__ref_00245.vec) < 6.25f)
			{
				_003C_003Ef__ref_00245._0024this.RefreshDropList();
			}
		}

		private static void _003C_003Em__1(GameObject o2)
		{
			if ((bool)o2)
			{
				o2.transform.position = new Vector3(0f, -1000f, 0f);
				o2.SetActive(false);
			}
		}
	}

	private readonly int DeadBoxPoolSize = 8;

	private readonly int NormalItemPoolSize = 8;

	private readonly int RandomBoxPoolSize = 8;

	private readonly Dictionary<long, GameObject> _instanceId2GameObject = new Dictionary<long, GameObject>();

	private Collider[] _drops;

	private Collider[] _newDrops;

	private readonly HashSet<long> _canPick = new HashSet<long>();

	private int _dropLayerMask;

	private int _groundLayerMask;

	private int _noPlayerGroundWaterLayerMask;

	private int _groundLayer;

	private int _dropLayer;

	private Coroutine _check;

	private Coroutine _flyObjCheck;

	private int _oldDropNum;

	private Vector3 _oldPos;

	private readonly LRU<int, ObjectPool<GameObject>> _itemId2Pools = new LRU<int, ObjectPool<GameObject>>(32);

	private readonly Dictionary<int, GameObject> _itemId2Origin = new Dictionary<int, GameObject>();

	private readonly Dictionary<long, SItemInfo> _instanceId2Info = new Dictionary<long, SItemInfo>();

	private readonly Dictionary<long, SItemBoxInfo> _dInstanceId2Info = new Dictionary<long, SItemBoxInfo>();

	public readonly List<long> OpenBoxIds = new List<long>();

	private readonly List<long> _closeBoxes = new List<long>();

	private readonly List<ItemInfo> _boxItemInfos = new List<ItemInfo>();

	private readonly HashSet<long> _boxIdCanSee = new HashSet<long>();

	private bool _isShowScroll = true;

	public Transform DropParent;

	public int DropNum;

	private byte _showPickPanelCode;

	public readonly HashSet<long> TrashcanIds = new HashSet<long>();

	private readonly CPickItem _cPickItem = new CPickItem();

	private readonly CPickItemFromBox _cPickItemFromBox = new CPickItemFromBox();

	public bool IsClickBox;

	private readonly Dictionary<long, DeadBoxAnim> _objId2AnimDead = new Dictionary<long, DeadBoxAnim>();

	private readonly Dictionary<int, List<float>> _skinId2OriginId = new Dictionary<int, List<float>>();

	private readonly Dictionary<int, int> _changedId2NormalId = new Dictionary<int, int>();

	private readonly HashSet<long> _objIdToRemoveAnim = new HashSet<long>();

	private bool _isFoolHoliday = true;

	private Dictionary<long, SItemInfo> _alwaysCache = new Dictionary<long, SItemInfo>();

	private readonly List<long> _objIdCreated = new List<long>();

	private Coroutine _dequeueCreateItem;

	private readonly Vector3 _upOffset = Vector3.up * 0.1f;

	private const string PlaneName = "Plane";

	private RaycastHit[] _hitInfos = new RaycastHit[1];

	private readonly Dictionary<long, long> _instanceId2BoxInstanceId = new Dictionary<long, long>();

	public bool IsShowScroll
	{
		get
		{
			return _isShowScroll;
		}
		set
		{
			_isShowScroll = value;
		}
	}

	public void Init()
	{
		_groundLayer = LayerMask.NameToLayer("Ground");
		_dropLayer = LayerMask.NameToLayer("DropItem");
		_dropLayerMask = 1 << _dropLayer;
		_groundLayerMask = 1 << _groundLayer;
		_noPlayerGroundWaterLayerMask = -1 ^ LayerMask.GetMask("Enemy", "OtherPlayerCollider", "Player", "SelfPlayer", "DropItem", "Ground", "Water");
		LRU<int, ObjectPool<GameObject>> itemId2Pools = _itemId2Pools;
		itemId2Pools.onRemoveEntry = (LRU<int, ObjectPool<GameObject>>.OnRemoveEntry)Delegate.Combine(itemId2Pools.onRemoveEntry, new LRU<int, ObjectPool<GameObject>>.OnRemoveEntry(OnRemoveCache));
		SItemInfo.handler = (SItemInfo.Handler)Delegate.Combine(SItemInfo.handler, new SItemInfo.Handler(OnSItemInfo));
		SItemDisappear.handler = (SItemDisappear.Handler)Delegate.Combine(SItemDisappear.handler, new SItemDisappear.Handler(OnSItemDisappear));
		SItemBoxDisappear.handler = (SItemBoxDisappear.Handler)Delegate.Combine(SItemBoxDisappear.handler, new SItemBoxDisappear.Handler(OnSItemBoxDisappear));
		SMapBoxItemChange.handler = (SMapBoxItemChange.Handler)Delegate.Combine(SMapBoxItemChange.handler, new SMapBoxItemChange.Handler(OnSMapBoxItemChange));
		SItemBoxInfo.handler = (SItemBoxInfo.Handler)Delegate.Combine(SItemBoxInfo.handler, new SItemBoxInfo.Handler(OnSItemBoxInfo));
		SceneEvent.InitLoadSceneFinish = (Utils.VoidDelegate)Delegate.Combine(SceneEvent.InitLoadSceneFinish, new Utils.VoidDelegate(StartBattle));
		BattleDropEvent.ClickBoxDelegate = (Utils.IntDelegate)Delegate.Combine(BattleDropEvent.ClickBoxDelegate, new Utils.IntDelegate(OnClickBox));
		SettingEvent.ExitCustomSaveSettingPanelDelegate = (Utils.VoidDelegate)Delegate.Combine(SettingEvent.ExitCustomSaveSettingPanelDelegate, new Utils.VoidDelegate(OnExitCustomSettingPanel));
		STrashCan.handler = (STrashCan.Handler)Delegate.Combine(STrashCan.handler, new STrashCan.Handler(OnSTrashCan));
		STrashCanDisappear.handler = (STrashCanDisappear.Handler)Delegate.Combine(STrashCanDisappear.handler, new STrashCanDisappear.Handler(OnSTrashCanDisappear));
		BattleDropEvent.LaJiZhanBaiXiangZiAction = (Action<List<TrashcanInfo>, Action<TrashcanInfo, GameObject>>)Delegate.Combine(BattleDropEvent.LaJiZhanBaiXiangZiAction, new Action<List<TrashcanInfo>, Action<TrashcanInfo, GameObject>>(OnLaJiZhanBaiXiangZi));
	}

	private void OnLaJiZhanBaiXiangZi(List<TrashcanInfo> trashcanInfos, Action<TrashcanInfo, GameObject> initObjAct)
	{
		_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey4 _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey = new _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey4();
		_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.initObjAct = initObjAct;
		_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey._0024this = this;
		int i = 0;
		for (int count = trashcanInfos.Count; i < count; i++)
		{
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2 _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2 = new _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2();
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2._003C_003Ef__ref_00244 = _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey;
			_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo = trashcanInfos[i];
			if (_itemId2Pools.TryGetValue(-_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo.typeId, out _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.pool))
			{
				GameObject gameObject = _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.pool.Get();
				_instanceId2GameObject[_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo.instanceId] = gameObject;
				InitLaJiZhanTong(gameObject, _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo);
				_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey.initObjAct(_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo, gameObject);
			}
			else
			{
				BoxCfg boxCfg = BoxCfg.Get(_003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo.typeId);
				if (boxCfg == null)
				{
					Debug.LogError("[BattleDropMgr.cs]BoxCfg not exist.Id : " + _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2.curTrashcanInfo.typeId);
				}
				else
				{
					ResMgr.Ins.CreateFromAB(boxCfg.prefabName, null, _003COnLaJiZhanBaiXiangZi_003Ec__AnonStorey2._003C_003Em__0);
				}
			}
		}
	}

	private void InitLaJiZhanTong(GameObject o, TrashcanInfo curTrashcanInfo)
	{
		TrashcanCfgInfo trashcanCfgInfo = o.GetComponent<TrashcanCfgInfo>();
		if (!trashcanCfgInfo)
		{
			trashcanCfgInfo = Utils.AddComponentIfNotExist<TrashcanCfgInfo>(o);
		}
		TrashcanIds.Add(curTrashcanInfo.instanceId);
		trashcanCfgInfo.InsId = curTrashcanInfo.instanceId;
		trashcanCfgInfo.CfgId = curTrashcanInfo.typeId;
		BoxCfg boxCfg = (trashcanCfgInfo.MyCfg = BoxCfg.Get(curTrashcanInfo.typeId));
		if (boxCfg != null)
		{
			trashcanCfgInfo.Hp = boxCfg.hp;
			trashcanCfgInfo.MaxHp = boxCfg.hp;
			trashcanCfgInfo.MapObjectName = boxCfg.name;
		}
		else
		{
			trashcanCfgInfo.Hp = 0f;
			trashcanCfgInfo.MaxHp = 0;
			trashcanCfgInfo.MapObjectName = string.Empty;
		}
		o.name = (-trashcanCfgInfo.CfgId).ToString();
		Battle.Ins.MapObjectDic.Add(trashcanCfgInfo.InsId, trashcanCfgInfo);
	}

	public void RecycleLaJiZhanLaJiTong(List<GameObject> trashcanList)
	{
		if (trashcanList == null || trashcanList.Count == 0)
		{
			return;
		}
		int i = 0;
		for (int count = trashcanList.Count; i < count; i++)
		{
			GameObject gameObject = trashcanList[i];
			if (!gameObject)
			{
				continue;
			}
			gameObject.transform.SetParent(DropParent);
			TrashcanCfgInfo component = gameObject.GetComponent<TrashcanCfgInfo>();
			if ((bool)component)
			{
				Battle.Ins.MapObjectDic.Remove(component.InsId);
				TrashcanIds.Add(component.InsId);
			}
			else
			{
				Debug.LogError("[BattleDropMgr.cs]No TrashcanInfo.");
			}
			string name = gameObject.name;
			if (!RoleMgr.IsInt(name.TrimStart('-')))
			{
				Debug.LogError("[BattleDropMgr.cs]GameObject name is not int.Name : " + name);
				DestroyFunc(gameObject);
				continue;
			}
			int key = -Convert.ToInt32(name);
			ObjectPool<GameObject> value;
			if (_itemId2Pools.TryGetValue(key, out value))
			{
				value.Recycle(gameObject);
			}
			else
			{
				DestroyFunc(gameObject);
			}
		}
	}

	public void ChangeTrashcanHp(long insId, int hp)
	{
		GameObject value;
		if (_instanceId2GameObject.TryGetValue(insId, out value))
		{
			MapObject component = value.GetComponent<MapObject>();
			if ((bool)component)
			{
				component.Hp = hp;
			}
		}
	}

	private void OnSTrashCanDisappear(STrashCanDisappear msg)
	{
		GameObject value;
		if (_instanceId2GameObject.TryGetValue(msg.objId, out value) && RoleMgr.IsInt(value.name.TrimStart('-')))
		{
			int num = Convert.ToInt32(value.name);
			BoxCfg boxCfg = BoxCfg.Get(-num);
			if (boxCfg != null)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(boxCfg.breakEffect, value.transform.position, value.transform.forward);
			}
		}
		TrashcanIds.Remove(msg.objId);
		Battle.Ins.LajitongDic.Remove(msg.objId);
		Battle.Ins.XiangziDic.Remove(msg.objId);
		DestroyItem(msg.objId);
	}

	private void OnSTrashCan(STrashCan msg)
	{
		BoxCfg boxCfg = BoxCfg.Get(msg.type);
		if (boxCfg == null)
		{
			Debug.LogError("[BattleDropMgr.cs]Can't find boxCfg with id : " + msg.type);
			return;
		}
		TrashcanIds.Add(msg.objId);
		CreateItem(msg.objId, -msg.type, msg.pos, boxCfg.prefabName, RandomBoxPoolSize);
		if (BattleMapSettingPanel.ContainLajitong(msg.type))
		{
			Battle.Ins.LajitongDic.Add(msg.objId, msg);
		}
		else if (BattleMapSettingPanel.ContainXiangzi(msg.type))
		{
			Battle.Ins.XiangziDic.Add(msg.objId, msg);
		}
	}

	public bool GetGameObject(long insId, out GameObject obj)
	{
		return _instanceId2GameObject.TryGetValue(insId, out obj);
	}

	public void SendPickItem(long instanceId, int num)
	{
		_cPickItem.objId = instanceId;
		_cPickItem.number = num;
		Client2Gs.Ins.Send(_cPickItem);
	}

	public void SendPickItemFromBox(long boxInstanceId, long instanceId, int num)
	{
		_cPickItemFromBox.boxId = boxInstanceId;
		_cPickItemFromBox.itemInstanceId = instanceId;
		_cPickItemFromBox.number = num;
		Client2Gs.Ins.Send(_cPickItemFromBox);
	}

	private void OnExitCustomSettingPanel()
	{
		_showPickPanelCode = 1;
	}

	private void OnClickBox(int index)
	{
		long instanceId = GetInstanceId(index);
		if (_boxIdCanSee.Contains(instanceId))
		{
			OpenBoxIds.Add(instanceId);
			_closeBoxes.Remove(instanceId);
			_boxItemInfos.Clear();
			for (int num = OpenBoxIds.Count - 1; num >= 0; num--)
			{
				_boxItemInfos.AddRange(_dInstanceId2Info[OpenBoxIds[num]].items);
			}
			IsClickBox = true;
			Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
			IsClickBox = false;
		}
	}

	private bool OnRemoveCache(int key, ObjectPool<GameObject> value)
	{
		ItemCfg itemCfg = ItemCfg.Get(key);
		if (itemCfg != null && (itemCfg.type == 24 || itemCfg.type == 34 || itemCfg.type == 35))
		{
			return false;
		}
		UnityEngine.Object.DestroyImmediate(_itemId2Origin[key]);
		_itemId2Origin.Remove(key);
		value.Clear();
		return true;
	}

	private void StartBattle()
	{
		if (_drops == null)
		{
			_drops = new Collider[100];
			_newDrops = new Collider[100];
		}
		SBloodAirportPlayerDie.handler = (SBloodAirportPlayerDie.Handler)Delegate.Combine(SBloodAirportPlayerDie.handler, new SBloodAirportPlayerDie.Handler(OnSBloodAirportPlayerDie));
		_dequeueCreateItem = Battle.StartConroutine(DequeueCreateItem());
		_check = Battle.StartConroutine(CheckForDropItem());
		GameObject gameObject = new GameObject("_DropParent_");
		DropParent = gameObject.transform;
		DropParent.position = Vector3.zero;
		DropParent.localScale = Vector3.one;
		IntSkinId2OriginDic();
		InitChangedId2NormalId();
	}

	private void IntSkinId2OriginDic()
	{
		if (_skinId2OriginId.Count > 0)
		{
			return;
		}
		List<ItemCfg> allList = ItemCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			if ((allList[i].type == 63 || allList[i].type == 23) && allList[i].extras.Count > 0)
			{
				List<float> extras = allList[i].extras;
				if (allList[i].type == 63)
				{
					_skinId2OriginId[(int)extras[0]] = extras;
				}
				else if (allList[i].type == 23)
				{
					_skinId2OriginId[allList[i].id] = extras;
				}
			}
		}
	}

	private void InitChangedId2NormalId()
	{
		if (_changedId2NormalId.Count > 0)
		{
			return;
		}
		List<MemberCardForBattleCfg> allList = MemberCardForBattleCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			if (allList[i].afterPickItemId != allList[i].afterDropItemId)
			{
				_changedId2NormalId[allList[i].afterPickItemId] = allList[i].afterDropItemId;
			}
		}
	}

	private void OnSBloodAirportPlayerDie(SBloodAirportPlayerDie msg)
	{
		Battle.StopConroutine(_check);
	}

	public void Clear()
	{
		Battle.StopConroutine(_check);
		Battle.StopConroutine(_dequeueCreateItem);
		Battle.StopConroutine(_flyObjCheck);
		_instanceId2GameObject.Clear();
		_itemId2Pools.Clear();
		_canPick.Clear();
		_dInstanceId2Info.Clear();
		OpenBoxIds.Clear();
		_closeBoxes.Clear();
		_boxItemInfos.Clear();
		_boxIdCanSee.Clear();
		_itemId2Origin.Clear();
		_instanceId2Info.Clear();
		_alwaysCache.Clear();
		_objIdCreated.Clear();
		_objId2AnimDead.Clear();
		_objIdToRemoveAnim.Clear();
		IsShowScroll = true;
		DropNum = 0;
		_oldDropNum = 0;
	}

	private IEnumerator CheckForDropItem()
	{
		while (true)
		{
			if (Battle.Ins.SelfPlayer != null)
			{
				if (Battle.Ins.SelfPlayer.InCar)
				{
					if (DropNum > 0)
					{
						for (int i = 0; i < _drops.Length; i++)
						{
							_drops[i] = null;
							_newDrops[i] = null;
						}
						DropNum = 0;
						_oldDropNum = 0;
						RefreshAllListAndSet();
						Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
					}
					yield return Utils.WaitForSeconds(1f);
				}
				else if (IsMove())
				{
					if (Battle.Ins.SelfPlayer.IsDie)
					{
						if (BattleDropEvent.HidePickPanel != null)
						{
							BattleDropEvent.HidePickPanel();
						}
						yield return Utils.WaitForSeconds(1f);
					}
					_oldDropNum = DropNum;
					DropNum = Physics.OverlapSphereNonAlloc(_oldPos, 2f, _newDrops, _dropLayerMask);
					if (DropNum != _oldDropNum || NotContainAll())
					{
						Exchange(ref _drops, ref _newDrops);
						SortArray();
						RefreshAllListAndSet();
						_oldDropNum = DropNum;
						PickPanel pickPanel = ViewMgr.Ins.GetView("PickPanel") as PickPanel;
						if ((bool)pickPanel && (bool)pickPanel.gameObject && pickPanel.gameObject.activeSelf)
						{
							Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
						}
						else if (BattleDropEvent.ShowPickPanel != null)
						{
							BattleDropEvent.ShowPickPanel(_showPickPanelCode);
							_showPickPanelCode = 0;
						}
					}
					if (DropNum > 0 && (bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer && !Battle.Ins.SelfPlayer.IsDie && Battle.Ins.SelfPlayer.HP > 0)
					{
						if (!ViewMgr.Ins.IsShow<PickPanel>())
						{
							ViewMgr.Ins.ShowView<PickPanel>(_showPickPanelCode, false);
						}
						_showPickPanelCode = 0;
					}
				}
			}
			yield return Utils.WaitForSeconds(0.2f);
		}
	}

	private void RefreshDropList()
	{
		if (!Battle.Ins || !Battle.Ins.SelfPlayer || ((bool)Battle.Ins.SelfPlayer && Battle.Ins.SelfPlayer.IsDie))
		{
			return;
		}
		DropNum = Physics.OverlapSphereNonAlloc(Battle.Ins.SelfPlayer.Pos, 2f, _newDrops, _dropLayerMask);
		Exchange(ref _newDrops, ref _drops);
		SortArray();
		RefreshAllListAndSet();
		_oldDropNum = DropNum;
		Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
		if (DropNum > 0 && (bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer && !Battle.Ins.SelfPlayer.IsDie && Battle.Ins.SelfPlayer.HP > 0)
		{
			if (!ViewMgr.Ins.IsShow<PickPanel>())
			{
				ViewMgr.Ins.ShowView<PickPanel>(_showPickPanelCode, false);
			}
			else if (BattleDropEvent.ShowPickPanel != null)
			{
				BattleDropEvent.ShowPickPanel(_showPickPanelCode);
			}
			_showPickPanelCode = 0;
		}
	}

	private void RefreshAllListAndSet()
	{
		RefreshCanSeeAndClose();
		RefreshCanPickSet();
		RefreshOpenListAndBoxItemInfos();
	}

	private bool IsMove()
	{
		if (CalDistance(Battle.Ins.SelfPlayer.Pos) > 0.01f)
		{
			_oldPos = Battle.Ins.SelfPlayer.Pos;
			return true;
		}
		return false;
	}

	private bool NotContainAll()
	{
		for (int i = 0; i < DropNum; i++)
		{
			if (!_canPick.Contains(GetInstanceId(i, _newDrops)))
			{
				return true;
			}
		}
		return false;
	}

	private void RefreshCanPickSet()
	{
		_canPick.Clear();
		for (int i = 0; i < DropNum; i++)
		{
			long instanceId = GetInstanceId(i, _drops);
			int itemIdBeforeBoxNumEnsured = GetItemIdBeforeBoxNumEnsured(i);
			ItemCfg itemCfg = ItemCfg.Get(itemIdBeforeBoxNumEnsured);
			_canPick.Add(instanceId);
			if (itemCfg != null && itemCfg.type == 34 && !_objId2AnimDead.ContainsKey(instanceId) && _isFoolHoliday)
			{
				_objId2AnimDead[instanceId] = _drops[i].GetComponent<DeadBoxAnim>();
				if (_objId2AnimDead[instanceId] == null)
				{
					_isFoolHoliday = false;
				}
				else
				{
					_objId2AnimDead[instanceId].Open();
				}
			}
		}
		if (!_isFoolHoliday)
		{
			return;
		}
		foreach (KeyValuePair<long, DeadBoxAnim> item in _objId2AnimDead)
		{
			if (!_canPick.Contains(item.Key))
			{
				item.Value.Close();
				_objIdToRemoveAnim.Add(item.Key);
			}
		}
		foreach (long item2 in _objIdToRemoveAnim)
		{
			_objId2AnimDead.Remove(item2);
		}
		_objIdToRemoveAnim.Clear();
	}

	private void RefreshCanSeeAndClose()
	{
		_boxIdCanSee.Clear();
		_closeBoxes.Clear();
		for (int i = 0; i < DropNum; i++)
		{
			int itemIdBeforeBoxNumEnsured = GetItemIdBeforeBoxNumEnsured(i);
			ItemCfg itemCfg = ItemCfg.Get(itemIdBeforeBoxNumEnsured);
			if (itemCfg == null || itemCfg.type == 34 || itemCfg.type == 35)
			{
				long instanceId = GetInstanceId(i, _drops);
				_boxIdCanSee.Add(instanceId);
				_closeBoxes.Add(instanceId);
				continue;
			}
			break;
		}
	}

	private void RefreshOpenListAndBoxItemInfos()
	{
		for (int i = 0; i < OpenBoxIds.Count; i++)
		{
			if (!_boxIdCanSee.Contains(OpenBoxIds[i]))
			{
				SItemBoxInfo value;
				if (!_dInstanceId2Info.TryGetValue(OpenBoxIds[i], out value))
				{
					continue;
				}
				List<ItemInfo> items = value.items;
				if (items != null)
				{
					for (int j = 0; j < items.Count; j++)
					{
						_boxItemInfos.Remove(items[j]);
					}
					OpenBoxIds.RemoveAt(i);
					i--;
				}
			}
			else
			{
				_closeBoxes.Remove(OpenBoxIds[i]);
			}
		}
	}

	private void SortArray()
	{
		for (int i = 0; i < DropNum - 1; i++)
		{
			bool flag = false;
			for (int j = 0; j < DropNum - 1 - i; j++)
			{
				if (SpecificSortMethod(GetItemIdBeforeBoxNumEnsured(j), GetItemIdBeforeBoxNumEnsured(j + 1)) > 0)
				{
					Exchange(ref _drops[j], ref _drops[j + 1]);
					flag = true;
				}
			}
			if (!flag)
			{
				break;
			}
		}
	}

	private int GetItemIdBeforeBoxNumEnsured(int index)
	{
		if (_drops[index].name.EndsWith("*"))
		{
			return ConstsBs.DEAD_BOX_ITEM_ID;
		}
		long num = Convert.ToInt64(_drops[index].name);
		SItemInfo value;
		if (_instanceId2Info.TryGetValue(num, out value))
		{
			return value.itemInfo.itemId;
		}
		SItemBoxInfo value2;
		if (_dInstanceId2Info.TryGetValue(num, out value2))
		{
			return ConstsBs.DEAD_BOX_ITEM_ID;
		}
		Debug.LogError("InstanceId : " + num + " not item not box.");
		return 0;
	}

	private int SpecificSortMethod(int itemId1, int itemId2)
	{
		if (itemId1 * itemId2 < 0)
		{
			return itemId1;
		}
		ItemCfg itemCfg = ItemCfg.Get(itemId1);
		ItemCfg itemCfg2 = ItemCfg.Get(itemId2);
		if (itemCfg == null || itemCfg2 == null)
		{
			return 0;
		}
		if (itemCfg.type == itemCfg2.type)
		{
			if (itemCfg2.quality != itemCfg.quality)
			{
				return itemCfg2.quality - itemCfg.quality;
			}
			return itemId1 - itemId2;
		}
		if (itemCfg.type == 35)
		{
			return -1;
		}
		if (itemCfg2.type == 35)
		{
			return 1;
		}
		if (itemCfg.type == 34)
		{
			return -1;
		}
		if (itemCfg2.type == 34)
		{
			return 1;
		}
		return itemCfg.type - itemCfg2.type;
	}

	private void Exchange<T>(ref T co1, ref T co2)
	{
		T val = co1;
		co1 = co2;
		co2 = val;
	}

	private void OnSItemBoxInfo(SItemBoxInfo msg)
	{
		msg.items.Sort(SortBoxContent);
		if (_dInstanceId2Info.ContainsKey(msg.objId))
		{
			_dInstanceId2Info[msg.objId] = msg;
			if (OpenBoxIds.Contains(msg.objId))
			{
				RefreshOpenList();
			}
			Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
		}
		else
		{
			_dInstanceId2Info.Add(msg.objId, msg);
			ItemCfg itemCfg = ItemCfg.Get(ConstsBs.DEAD_BOX_ITEM_ID);
			if (itemCfg == null)
			{
				Debug.LogError("No dead box info.");
				return;
			}
			CreateItem(msg.objId, ConstsBs.DEAD_BOX_ITEM_ID, msg.pos, itemCfg.dropModel, DeadBoxPoolSize);
		}
		int i = 0;
		for (int count = msg.items.Count; i < count; i++)
		{
			_instanceId2BoxInstanceId[msg.items[i].objId] = msg.objId;
		}
	}

	private void OnSMapBoxItemChange(SMapBoxItemChange msg)
	{
		SItemBoxInfo value;
		if (!_dInstanceId2Info.TryGetValue(msg.boxId, out value))
		{
			return;
		}
		List<ItemInfo> items = value.items;
		int i = 0;
		for (int count = items.Count; i < count; i++)
		{
			ItemInfo itemInfo = items[i];
			if (itemInfo.objId == msg.itemInstanceId)
			{
				itemInfo.number = msg.number;
				if (itemInfo.number <= 0)
				{
					items.RemoveAt(i);
				}
				break;
			}
		}
		if (OpenBoxIds.Contains(msg.boxId))
		{
			RefreshOpenList();
		}
		Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
	}

	private int SortBoxContent(ItemInfo info1, ItemInfo info2)
	{
		return SpecificSortMethod(info1.itemId, info2.itemId);
	}

	private void RefreshOpenList()
	{
		_boxItemInfos.Clear();
		for (int i = 0; i < OpenBoxIds.Count; i++)
		{
			_boxItemInfos.AddRange(_dInstanceId2Info[OpenBoxIds[i]].items);
		}
	}

	private void OnSItemDisappear(SItemDisappear msg)
	{
		_alwaysCache.Remove(msg.objId);
		DestroyItem(msg.objId);
		_instanceId2Info.Remove(msg.objId);
	}

	private void OnSItemBoxDisappear(SItemBoxDisappear msg)
	{
		DestroyItem(msg.objId);
		_dInstanceId2Info.Remove(msg.objId);
	}

	private void DestroyItem(long instanceId)
	{
		GameObject value;
		if (_instanceId2GameObject.TryGetValue(instanceId, out value))
		{
			Vector3 position = value.transform.position;
			int key = 0;
			SItemInfo value2;
			if (_instanceId2Info.TryGetValue(instanceId, out value2))
			{
				key = value2.itemInfo.itemId;
			}
			else if (value.name.EndsWith("*"))
			{
				key = ConstsBs.DEAD_BOX_ITEM_ID;
			}
			else
			{
				if (RoleMgr.IsInt(value.name.TrimStart('-')))
				{
					key = Convert.ToInt32(value.name);
				}
				Battle.Ins.MapObjectDic.Remove(instanceId);
			}
			ObjectPool<GameObject> value3;
			if (_itemId2Pools.TryGetValue(key, out value3))
			{
				value3.Recycle(value);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(value);
			}
			if (CalDistance(position) < 6.25f)
			{
				RefreshDropList();
			}
		}
		_instanceId2GameObject.Remove(instanceId);
	}

	private void OnSItemInfo(SItemInfo msg)
	{
		_alwaysCache[msg.itemInfo.objId] = msg;
	}

	private IEnumerator DequeueCreateItem()
	{
		while (true)
		{
			yield return Utils.WaitForSeconds(0.1f);
			int createOnceNum = 0;
			foreach (KeyValuePair<long, SItemInfo> item in _alwaysCache)
			{
				SItemInfo value = item.Value;
				ItemInfo itemInfo = value.itemInfo;
				_instanceId2Info[itemInfo.objId] = value;
				if (!_instanceId2GameObject.ContainsKey(itemInfo.objId))
				{
					ItemCfg itemCfg = ItemCfg.Get(itemInfo.itemId);
					if (itemCfg == null)
					{
						Debug.LogError("Can't find item with id : " + itemInfo.itemId);
					}
					else
					{
						CreateItem(itemInfo.objId, itemInfo.itemId, value.pos, itemCfg.dropModel, NormalItemPoolSize);
					}
				}
				else
				{
					_instanceId2GameObject[itemInfo.objId].name = GetSuitableName(itemInfo.objId, itemInfo.itemId);
					if (CalDistance(value.pos) < 6.25f)
					{
						Utils.TriggerEvent(BattleDropEvent.DropNumChangeDelegate);
					}
				}
				_objIdCreated.Add(item.Key);
				if (createOnceNum >= 5)
				{
					break;
				}
				createOnceNum++;
			}
			int i = 0;
			for (int count = _objIdCreated.Count; i < count; i++)
			{
				_alwaysCache.Remove(_objIdCreated[i]);
			}
			_objIdCreated.Clear();
		}
	}

	private float CalDistance(Vec3 pos)
	{
		float num = pos.x - _oldPos.x;
		float num2 = pos.y - _oldPos.y;
		float num3 = pos.z - _oldPos.z;
		return num * num + num3 * num3 + num2 * num2;
	}

	private float CalDistance(Vector3 pos)
	{
		float num = pos.x - _oldPos.x;
		float num2 = pos.y - _oldPos.y;
		float num3 = pos.z - _oldPos.z;
		return num * num + num3 * num3 + num2 * num2;
	}

	private void CreateItem(long objId, int id, Vec3 vec, string modelPath, int poolSize)
	{
		_003CCreateItem_003Ec__AnonStorey5 _003CCreateItem_003Ec__AnonStorey = new _003CCreateItem_003Ec__AnonStorey5();
		_003CCreateItem_003Ec__AnonStorey.id = id;
		_003CCreateItem_003Ec__AnonStorey.poolSize = poolSize;
		_003CCreateItem_003Ec__AnonStorey.objId = objId;
		_003CCreateItem_003Ec__AnonStorey.vec = vec;
		_003CCreateItem_003Ec__AnonStorey._0024this = this;
		if (string.IsNullOrEmpty(modelPath))
		{
			Debug.LogError("id : " + _003CCreateItem_003Ec__AnonStorey.id + " modelPath is null or empty.");
			return;
		}
		_003CCreateItem_003Ec__AnonStorey6 _003CCreateItem_003Ec__AnonStorey2 = new _003CCreateItem_003Ec__AnonStorey6();
		_003CCreateItem_003Ec__AnonStorey2._003C_003Ef__ref_00245 = _003CCreateItem_003Ec__AnonStorey;
		if (_itemId2Pools.TryGetValue(_003CCreateItem_003Ec__AnonStorey.id, out _003CCreateItem_003Ec__AnonStorey2.pool))
		{
			GameObject go = _003CCreateItem_003Ec__AnonStorey2.pool.Get();
			if ((bool)go)
			{
				InitGameObject(ref go, _003CCreateItem_003Ec__AnonStorey.objId, _003CCreateItem_003Ec__AnonStorey.id, _003CCreateItem_003Ec__AnonStorey.vec);
			}
			if (CalDistance(_003CCreateItem_003Ec__AnonStorey.vec) < 6.25f)
			{
				RefreshDropList();
			}
		}
		else
		{
			ResMgr.Ins.CreateFromAB(modelPath, null, _003CCreateItem_003Ec__AnonStorey2._003C_003Em__0);
		}
	}

	private void DestroyFunc(GameObject go)
	{
		try
		{
			go.SetActive(false);
			UnityEngine.Object.Destroy(go, 0.5f);
		}
		catch (Exception)
		{
		}
	}

	private void InitGameObject(ref GameObject go, long instanceId, int cfgId, Vec3 vec)
	{
		Vector3 vector = default(Vector3);
		vector.x = vec.x;
		vector.y = vec.y;
		vector.z = vec.z;
		Transform transform = go.transform;
		transform.SetParent(DropParent);
		transform.position = vector;
		if (cfgId > 0)
		{
			go.name = GetSuitableName(instanceId, cfgId);
			go.layer = _dropLayer;
			if ((int)transform.localEulerAngles.x == 0)
			{
				transform.Rotate(-90f, instanceId % 36 * 10, 0f);
			}
		}
		else
		{
			TrashcanCfgInfo trashcanCfgInfo = go.GetComponent<TrashcanCfgInfo>();
			if (!trashcanCfgInfo)
			{
				trashcanCfgInfo = Utils.AddComponentIfNotExist<TrashcanCfgInfo>(go);
			}
			trashcanCfgInfo.InsId = instanceId;
			trashcanCfgInfo.CfgId = -cfgId;
			BoxCfg boxCfg = (trashcanCfgInfo.MyCfg = BoxCfg.Get(-cfgId));
			if (boxCfg != null)
			{
				trashcanCfgInfo.Hp = boxCfg.hp;
				trashcanCfgInfo.MaxHp = boxCfg.hp;
				trashcanCfgInfo.MapObjectName = boxCfg.name;
			}
			else
			{
				trashcanCfgInfo.Hp = 0f;
				trashcanCfgInfo.MaxHp = 0;
				trashcanCfgInfo.MapObjectName = string.Empty;
			}
			go.name = cfgId.ToString();
			Battle.Ins.MapObjectDic.Add(instanceId, trashcanCfgInfo);
			if (instanceId <= 0)
			{
				go.SetActiveBetter(false);
				return;
			}
		}
		_instanceId2GameObject[instanceId] = go;
		Ray ray = new Ray(vector + _upOffset, Vector3.down);
		Ray ray2 = new Ray(vector + _upOffset, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 50f, _noPlayerGroundWaterLayerMask, QueryTriggerInteraction.Ignore) && !hitInfo.collider.name.Equals("Plane"))
		{
			if (hitInfo.distance > 0.2f)
			{
				transform.position = hitInfo.point;
			}
			if (hitInfo.collider.name.StartsWith("project_lajidui"))
			{
				TrashcanStationGameObject component = hitInfo.collider.GetComponent<TrashcanStationGameObject>();
				if ((bool)component && component.InsId > 0)
				{
					Singleton<TrashcanStationMgr>.Ins.AddGroundItem2TrashcanStation(component.InsId, transform);
				}
			}
			if (cfgId > 0)
			{
				transform.forward = hitInfo.normal;
			}
			else
			{
				transform.up = hitInfo.normal;
			}
		}
		else if (Physics.RaycastNonAlloc(ray2, _hitInfos, 500f, _groundLayerMask) > 0)
		{
			transform.position = _hitInfos[0].point;
			if (cfgId > 0)
			{
				transform.forward = _hitInfos[0].normal;
			}
			else
			{
				transform.up = _hitInfos[0].normal;
			}
		}
		go.SetActive(true);
	}

	public void SetObjectToGround(Transform trans)
	{
		Ray ray = new Ray(trans.position + _upOffset, Vector3.down);
		if (Physics.RaycastNonAlloc(ray, _hitInfos, 500f, _groundLayerMask) > 0)
		{
			trans.position = _hitInfos[0].point;
		}
	}

	private string GetSuitableName(long instanceId, int itemId)
	{
		if (itemId != ConstsBs.DEAD_BOX_ITEM_ID)
		{
			return instanceId.ToString();
		}
		return instanceId.ToString() + '*';
	}

	private long GetInstanceId(int index, Collider[] cols)
	{
		if (!cols[index].name.EndsWith("*"))
		{
			return Convert.ToInt64(cols[index].name);
		}
		return Convert.ToInt64(cols[index].name.TrimEnd('*'));
	}

	public long GetInstanceId(int index)
	{
		if (index < _boxItemInfos.Count)
		{
			return _boxItemInfos[index].objId;
		}
		if (index < _boxItemInfos.Count + _closeBoxes.Count)
		{
			return _closeBoxes[index - _boxItemInfos.Count];
		}
		if ((bool)_drops[index - _boxItemInfos.Count + OpenBoxIds.Count])
		{
			return Convert.ToInt64(_drops[index - _boxItemInfos.Count + OpenBoxIds.Count].name);
		}
		return -1L;
	}

	public bool GetIdAndNumber(int index, out int itemId, out int num, out long boxInstanceId)
	{
		boxInstanceId = 0L;
		if (index < _boxItemInfos.Count)
		{
			itemId = _boxItemInfos[index].itemId;
			num = _boxItemInfos[index].number;
			_instanceId2BoxInstanceId.TryGetValue(_boxItemInfos[index].objId, out boxInstanceId);
			return true;
		}
		int num2 = index - _boxItemInfos.Count + OpenBoxIds.Count;
		if (index < _boxItemInfos.Count + _closeBoxes.Count)
		{
			itemId = ConstsBs.DEAD_BOX_ITEM_ID;
			num = 1;
			return false;
		}
		if ((bool)_drops[num2])
		{
			long key = Convert.ToInt64(_drops[num2].name);
			itemId = _instanceId2Info[key].itemInfo.itemId;
			num = _instanceId2Info[key].itemInfo.number;
		}
		else
		{
			itemId = 0;
			num = 0;
		}
		return false;
	}

	public int GetTotalCellNum()
	{
		int num = 0;
		num += DropNum - OpenBoxIds.Count;
		return num + _boxItemInfos.Count;
	}

	public bool IsAllPicked()
	{
		return _instanceId2Info.Count <= 0;
	}
}
