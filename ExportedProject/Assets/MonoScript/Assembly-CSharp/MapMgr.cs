using System;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;

public class MapMgr : Singleton<MapMgr>
{
	public Vector3 StartPos;

	public Vector3 EndPos;

	public Vector3 FlyDir;

	public bool IsFlyBattlePlane;

	public Vector3 LastCarPos;

	public int LastVehicleId;

	public bool IsShowLastCarPos;

	public Vector3 SelfDiePos = Vector3.zero;

	public SFlyPlaneOther FlyPlaneOther;

	public const int MapSize = 6000;

	public Vector3 MilitaryBasePos = new Vector3(-1772.6f, 0f, -318.9f);

	private SDropedItemsDuringOffline _sDropedItemsDuringOffline;

	private float _dropedItemsRealTime;

	private Vector3 _otherFlyDir = Vector3.zero;

	public Vector3 PlaneDistance = Vector3.zero;

	public List<Vector3> SecondPos = new List<Vector3>();

	private const string ShowToolBoxInMapKey = "ShowToolBoxInMap";

	private const string SecondBattleKey = "SecondBattle";

	private const string TeamPosKey = "TeamMarkPos";

	private const string AirDropPosKey = "AirDropPos";

	private const string DiePosKey = "DiePos";

	private const string ShowToolBoxInLittleMapKey = "ShowToolBoxInLittleMap";

	private const string SecondBattleLittleMapKey = "SecondBattleLittleMap";

	private const string TeamPosLittleMapKey = "TeamMarkLittleMapPos";

	private const string AirDropPosLittleMapKey = "AirDropPosLittleMap";

	private const string DiePosLittleMapKey = "DiePosLittleMap";

	private List<int> _mains = new List<int>();

	private List<int> _trees = new List<int>();

	private List<int> _plant = new List<int>();

	private List<int> _lajitong = new List<int>();

	private List<int> _xiangzi = new List<int>();

	private const string MainLittleMapKey = "MainLittleMap";

	private const string TreeLittleMapKey = "TreeLittleMap";

	private const string PlantLittleMapKey = "PlantLittleMap";

	private const string LajitongLittleMapKey = "LajitongLittleMap";

	private const string XiangziLittleMapKey = "XiangziLittleMap";

	public static bool ShowToolBoxInMap
	{
		get
		{
			return GetBool("ShowToolBoxInMap");
		}
		set
		{
			SetBool("ShowToolBoxInMap", value, MapEvent.ToolBoxDelegate);
		}
	}

	public static bool SecondBattle
	{
		get
		{
			return false;
		}
		set
		{
			SetBool("SecondBattle", value, MapEvent.SecondBattlePosDelegate);
		}
	}

	public static bool TeamMarkPos
	{
		get
		{
			return GetBool("TeamMarkPos");
		}
		set
		{
			SetBool("TeamMarkPos", value, MapEvent.TeamPosDelegate);
		}
	}

	public static bool AirDropPos
	{
		get
		{
			return GetBool("AirDropPos");
		}
		set
		{
			SetBool("AirDropPos", value, MapEvent.AirDropPosDelegate);
		}
	}

	public static bool DiePos
	{
		get
		{
			return GetBool("DiePos");
		}
		set
		{
			SetBool("DiePos", value, MapEvent.DiePosDelegate);
		}
	}

	public static bool ShowToolBoxInLittleMap
	{
		get
		{
			return GetBool("ShowToolBoxInLittleMap");
		}
		set
		{
			SetBool("ShowToolBoxInLittleMap", value, MapEvent.ToolBoxLittleMapDelegate);
		}
	}

	public static bool SecondBattleLittleMap
	{
		get
		{
			return false;
		}
		set
		{
			SetBool("SecondBattleLittleMap", value, MapEvent.SecondBattlePosLittleMapDelegate);
		}
	}

	public static bool TeamMarkPosLittleMap
	{
		get
		{
			return GetBool("TeamMarkLittleMapPos");
		}
		set
		{
			SetBool("TeamMarkLittleMapPos", value, MapEvent.TeamPosDelegate);
		}
	}

	public static bool AirDropLittleMapPos
	{
		get
		{
			return GetBool("AirDropPosLittleMap");
		}
		set
		{
			SetBool("AirDropPosLittleMap", value, MapEvent.AirDropPosLittleMapDelegate);
		}
	}

	public static bool DiePosLittleMap
	{
		get
		{
			return GetBool("DiePosLittleMap");
		}
		set
		{
			SetBool("DiePosLittleMap", value, MapEvent.DiePosLittleMapDelegate);
		}
	}

	public List<int> Mains
	{
		get
		{
			return _mains;
		}
	}

	public List<int> Trees
	{
		get
		{
			return _trees;
		}
	}

	public List<int> Plant
	{
		get
		{
			return _plant;
		}
	}

	public List<int> Lajitong
	{
		get
		{
			return _lajitong;
		}
	}

	public List<int> Xiangzi
	{
		get
		{
			return _xiangzi;
		}
	}

	public void Init()
	{
		SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Combine(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(SEnterRoomhandle));
		SDropedItemsDuringOffline.handler = (SDropedItemsDuringOffline.Handler)Delegate.Combine(SDropedItemsDuringOffline.handler, new SDropedItemsDuringOffline.Handler(SDropedItemsDuringOfflineHandle));
		SSelfDie.handler = (SSelfDie.Handler)Delegate.Combine(SSelfDie.handler, new SSelfDie.Handler(SSelfDieHandle));
		InitResource();
		SecondPos.Add(new Vector3(2013f, 58f, -1721f));
		SecondPos.Add(new Vector3(1827f, 21f, -1655f));
	}

	private void SSelfDieHandle(SSelfDie msg)
	{
		_sDropedItemsDuringOffline = new SDropedItemsDuringOffline();
		_sDropedItemsDuringOffline.boxLeftTime = msg.boxLeftTime;
		_sDropedItemsDuringOffline.diePosX = msg.pos.x;
		_sDropedItemsDuringOffline.diePosY = msg.pos.y;
		_sDropedItemsDuringOffline.diePosZ = msg.pos.z;
		_dropedItemsRealTime = Time.realtimeSinceStartup;
		Utils.TriggerEvent(MapEvent.OnDropedItemsDuringOfflineDelegate);
	}

	private void SDropedItemsDuringOfflineHandle(SDropedItemsDuringOffline msg)
	{
		_sDropedItemsDuringOffline = msg;
		_dropedItemsRealTime = Time.realtimeSinceStartup;
		Utils.TriggerEvent(MapEvent.OnDropedItemsDuringOfflineDelegate);
	}

	public bool TryGetDropedItemsVector3(out Vector3 pos)
	{
		pos = Vector3.zero;
		if (_sDropedItemsDuringOffline == null)
		{
			return false;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - _dropedItemsRealTime > (float)_sDropedItemsDuringOffline.boxLeftTime)
		{
			return false;
		}
		pos.Set(_sDropedItemsDuringOffline.diePosX, _sDropedItemsDuringOffline.diePosY, _sDropedItemsDuringOffline.diePosZ);
		return true;
	}

	private void SEnterRoomhandle(SBattleLoginFinish handler)
	{
		SFlyBattlePlane.handler = (SFlyBattlePlane.Handler)Delegate.Combine(SFlyBattlePlane.handler, new SFlyBattlePlane.Handler(SFlyBattlePlaneHandle));
		BattleEvent.OnDestoryStartPlane = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnDestoryStartPlane, new Utils.VoidDelegate(OnDestoryStartPlane));
		BattleEvent.onGetOutOfVehicle = (BattleEvent.OnGetOutOfVehicle)Delegate.Combine(BattleEvent.onGetOutOfVehicle, new BattleEvent.OnGetOutOfVehicle(OnGetOutOfVehicle));
		BattleEvent.OnGetInVehicle = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnGetInVehicle, new Utils.VoidDelegate(OnGetInVehicle));
		BattleEvent.OnOtherGetInVehicle = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnOtherGetInVehicle, new Utils.IntDelegate(OnOtherGetInVehicle));
		BattleEvent.OnVehicleDisappear = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnVehicleDisappear, new Utils.IntDelegate(OnVehicleDisappear));
		BattleEvent.onSelfDie = (Utils.Vector3Delegate)Delegate.Combine(BattleEvent.onSelfDie, new Utils.Vector3Delegate(OnSelfDie));
		SFlyPlaneOther.handler = (SFlyPlaneOther.Handler)Delegate.Combine(SFlyPlaneOther.handler, new SFlyPlaneOther.Handler(SFlyPlaneOtherHandle));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(SPlayerDieHandle));
		SBloodAirportPlayerDie.handler = (SBloodAirportPlayerDie.Handler)Delegate.Combine(SBloodAirportPlayerDie.handler, new SBloodAirportPlayerDie.Handler(OnBloodAirPlayerDie));
		SelfDiePos = new Vector3(handler.playerInfo.pos.x, handler.playerInfo.pos.y, handler.playerInfo.pos.z);
	}

	private void OnBloodAirPlayerDie(SBloodAirportPlayerDie msg)
	{
	}

	private void SPlayerDieHandle(SPlayerDie msg)
	{
	}

	private void SFlyPlaneOtherHandle(SFlyPlaneOther msg)
	{
		FlyPlaneOther = msg;
		PlaneDistance = StartPos - new Vector3(msg.startPos.x, 0f, msg.startPos.y);
		Utils.TriggerEvent(MapEvent.OnBigPlaneOtherStart);
	}

	private void OnSelfDie(Vector3 vector3)
	{
		SelfDiePos = vector3;
	}

	private void OnVehicleDisappear(int arg)
	{
		if (arg == LastVehicleId)
		{
			IsShowLastCarPos = false;
			Utils.TriggerEvent(MapEvent.OnHideLastCarPos);
		}
	}

	private void OnOtherGetInVehicle(int arg)
	{
		if (arg == LastVehicleId)
		{
			IsShowLastCarPos = false;
			Utils.TriggerEvent(MapEvent.OnHideLastCarPos);
		}
	}

	private void OnGetInVehicle()
	{
		IsShowLastCarPos = false;
		Utils.TriggerEvent(MapEvent.OnHideLastCarPos);
	}

	private void OnGetOutOfVehicle(int vehicleId, Vector3 pos)
	{
		LastVehicleId = vehicleId;
		LastCarPos = pos;
		IsShowLastCarPos = true;
		Utils.TriggerEvent(MapEvent.OnShowLastCarPos);
	}

	private void OnDestoryStartPlane()
	{
		IsFlyBattlePlane = false;
		Utils.TriggerEvent(MapEvent.OnBigPlaneEnd);
		Utils.TriggerEvent(MapEvent.OnBigPlaneOtherEnd);
	}

	private void SFlyBattlePlaneHandle(SFlyBattlePlane msg)
	{
		IsFlyBattlePlane = true;
		StartPos = new Vector3(msg.startPos.x, 0f, msg.startPos.z);
		EndPos = new Vector3(msg.endPos.x, 0f, msg.endPos.z);
		Utils.TriggerEvent(MapEvent.OnBigPlaneStart);
	}

	public void Clear()
	{
		SFlyBattlePlane.handler = (SFlyBattlePlane.Handler)Delegate.Remove(SFlyBattlePlane.handler, new SFlyBattlePlane.Handler(SFlyBattlePlaneHandle));
		BattleEvent.OnDestoryStartPlane = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnDestoryStartPlane, new Utils.VoidDelegate(OnDestoryStartPlane));
		BattleEvent.onGetOutOfVehicle = (BattleEvent.OnGetOutOfVehicle)Delegate.Remove(BattleEvent.onGetOutOfVehicle, new BattleEvent.OnGetOutOfVehicle(OnGetOutOfVehicle));
		BattleEvent.OnGetInVehicle = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnGetInVehicle, new Utils.VoidDelegate(OnGetInVehicle));
		BattleEvent.OnOtherGetInVehicle = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnOtherGetInVehicle, new Utils.IntDelegate(OnOtherGetInVehicle));
		BattleEvent.OnVehicleDisappear = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnVehicleDisappear, new Utils.IntDelegate(OnVehicleDisappear));
		BattleEvent.onSelfDie = (Utils.Vector3Delegate)Delegate.Remove(BattleEvent.onSelfDie, new Utils.Vector3Delegate(OnSelfDie));
		SFlyPlaneOther.handler = (SFlyPlaneOther.Handler)Delegate.Remove(SFlyPlaneOther.handler, new SFlyPlaneOther.Handler(SFlyPlaneOtherHandle));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Remove(SPlayerDie.handler, new SPlayerDie.Handler(SPlayerDieHandle));
		SBloodAirportPlayerDie.handler = (SBloodAirportPlayerDie.Handler)Delegate.Remove(SBloodAirportPlayerDie.handler, new SBloodAirportPlayerDie.Handler(OnBloodAirPlayerDie));
		IsFlyBattlePlane = false;
		LastVehicleId = -1;
		IsShowLastCarPos = false;
	}

	public static void SetBool(string key, bool value, Utils.BoolDelegate callBack)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack, value);
	}

	public static bool GetBool(string key, bool defaultValue = true)
	{
		return GetInt(key, defaultValue ? 1 : 0) == 1;
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		return PlayerPrefs.GetInt(key);
	}

	public void InitResource()
	{
		_mains = GetList("MainLittleMap");
		_trees = GetList("TreeLittleMap");
		_plant = GetList("PlantLittleMap");
		_lajitong = GetList("LajitongLittleMap");
		_xiangzi = GetList("XiangziLittleMap");
	}

	public void SetMainResource(int itemId)
	{
		if (!_mains.Contains(itemId))
		{
			_mains.Add(itemId);
			SetList("MainLittleMap", _mains);
		}
	}

	public void RemoveMainResource(int itemId)
	{
		_mains.Remove(itemId);
		SetList("MainLittleMap", _mains);
	}

	public bool IsContainMainResource(int itemId)
	{
		return _mains.Contains(itemId);
	}

	public void SetMainResource(int[] itemIds)
	{
		_mains.Clear();
		_mains = itemIds.vToList();
		SetList("MainLittleMap", _mains);
	}

	public void RemoveAllMainResource()
	{
		_mains.Clear();
		SetList("MainLittleMap", _mains);
	}

	public void SetTreeResource(int itemId)
	{
		if (!_trees.Contains(itemId))
		{
			_trees.Add(itemId);
			SetList("TreeLittleMap", _trees);
		}
	}

	public void RemoveTreeResource(int itemId)
	{
		_trees.Remove(itemId);
		SetList("TreeLittleMap", _trees);
	}

	public bool IsContainTreeResource(int type)
	{
		return _trees.Contains(type);
	}

	public void SetTreeResource(int[] itemIds)
	{
		_trees.Clear();
		_trees = itemIds.vToList();
		SetList("TreeLittleMap", _trees);
	}

	public void RemoveAllTreeResource()
	{
		_trees.Clear();
		SetList("TreeLittleMap", _trees);
	}

	public void SetPlantResource(int itemId)
	{
		if (!_plant.Contains(itemId))
		{
			_plant.Add(itemId);
			SetList("PlantLittleMap", _plant);
		}
	}

	public void RemovePlantResource(int itemId)
	{
		_plant.Remove(itemId);
		SetList("PlantLittleMap", _plant);
	}

	public bool IsContainPlantResource(int type)
	{
		return _plant.Contains(type);
	}

	public void SetPlantResource(int[] itemIds)
	{
		_plant.Clear();
		_plant = itemIds.vToList();
		SetList("PlantLittleMap", _plant);
	}

	public void RemoveAllPlantResource()
	{
		_plant.Clear();
		SetList("PlantLittleMap", _plant);
	}

	public void SetLajitongResource(int itemId)
	{
		if (!_lajitong.Contains(itemId))
		{
			_lajitong.Add(itemId);
			SetList("LajitongLittleMap", _lajitong);
		}
	}

	public void RemoveLajitongResource(int itemId)
	{
		_lajitong.Remove(itemId);
		SetList("LajitongLittleMap", _lajitong);
	}

	public bool IsContainLajitongResource(int type)
	{
		return _lajitong.Contains(type);
	}

	public void SetLajitongResource(int[] itemIds)
	{
		_lajitong.Clear();
		_lajitong = itemIds.vToList();
		SetList("LajitongLittleMap", _lajitong);
	}

	public void RemoveAllLajitongResource()
	{
		_lajitong.Clear();
		SetList("LajitongLittleMap", _lajitong);
	}

	public void SetXiangziResource(int itemId)
	{
		if (!_xiangzi.Contains(itemId))
		{
			_xiangzi.Add(itemId);
			SetList("XiangziLittleMap", _xiangzi);
		}
	}

	public void RemoveXiangziResource(int itemId)
	{
		_xiangzi.Remove(itemId);
		SetList("XiangziLittleMap", _xiangzi);
	}

	public bool IsContainXiangziResource(int type)
	{
		return _xiangzi.Contains(type);
	}

	public void SetXiangziResource(int[] itemIds)
	{
		_xiangzi.Clear();
		_xiangzi = itemIds.vToList();
		SetList("XiangziLittleMap", _xiangzi);
	}

	public void RemoveAllXiangziResource()
	{
		_xiangzi.Clear();
		SetList("XiangziLittleMap", _xiangzi);
	}

	public void SetList(string key, List<int> values, Utils.VoidDelegate callBack = null)
	{
		string text = string.Empty;
		for (int i = 0; i < values.Count; i++)
		{
			text = text + values[i] + ",";
		}
		PlayerPrefs.SetString(key, text);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack);
	}

	public List<int> GetList(string key)
	{
		List<int> list = new List<int>();
		string empty = string.Empty;
		empty = PlayerPrefs.GetString(key);
		string[] array = empty.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				list.Add(int.Parse(array[i]));
			}
		}
		return list;
	}

	public string GetPlantIconName(int typeId)
	{
		CutPlantCfg cutPlantCfg = CutPlantCfg.Get(typeId);
		if (cutPlantCfg != null)
		{
			return cutPlantCfg.littleBattleMapIcon;
		}
		return string.Empty;
	}

	public string GetMineIconName(int typeId)
	{
		ItemCfg itemCfg = ItemCfg.Get(typeId);
		if (itemCfg != null)
		{
			return itemCfg.extrasstring[0];
		}
		return string.Empty;
	}

	public string GetTreeIconName(int levellId)
	{
		switch (levellId)
		{
		case 1:
			return cfg.Consts.TREE1_ICON_IN_MAP;
		case 2:
			return cfg.Consts.TREE2_ICON_IN_MAP;
		case 3:
			return cfg.Consts.TREE3_ICON_IN_MAP;
		default:
			return string.Empty;
		}
	}

	public string GetLajitongIconName(int levellId)
	{
		switch (levellId)
		{
		case 1:
			return cfg.Consts.LAJITONG1_ICON_IN_MAP;
		case 2:
			return cfg.Consts.LAJITONG2_ICON_IN_MAP;
		default:
			return string.Empty;
		}
	}

	public string GetXiangziIconName(int levellId)
	{
		switch (levellId)
		{
		case 1:
			return cfg.Consts.XIANGZI1_ICON_IN_MAP;
		case 2:
			return cfg.Consts.XIANGZI2_ICON_IN_MAP;
		case 3:
			return cfg.Consts.XIANGZI3_ICON_IN_MAP;
		default:
			return string.Empty;
		}
	}
}
