using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using client;
using gs.item.create.scmsg;
using gs.online.scmsg;

public class ScProduceMgr : Singleton<ScProduceMgr>
{
	private const float AimDistance = 14f;

	private const float IsMoveDistance = 0.125f;

	public Dictionary<int, List<DrawingCfg>> DicType2Cfgs = new Dictionary<int, List<DrawingCfg>>();

	public List<CreateInfo> CreatingItemsList = new List<CreateInfo>();

	public List<int> ShortcutItemList = new List<int>();

	public float CurItemFinishTime;

	public HashSet<int> CanProduce = new HashSet<int>();

	public HashSet<int> NewUnlock = new HashSet<int>();

	private HashSet<int> _learnedDrawingSet = new HashSet<int>();

	private Dictionary<int, List<DrawingCfg>> _dicLevel2DrawingCfgs = new Dictionary<int, List<DrawingCfg>>();

	public bool IsTest;

	public readonly Dictionary<int, int> DicProductionId2DrawId = new Dictionary<int, int>();

	private HashSet<long> _lastResult = new HashSet<long>();

	private Collider[] cos = new Collider[64];

	private Vector3 _oldPlayerPos;

	private bool _isLoadInfo;

	private string _newUnlockPath = "produce.oc";

	private ProduceLocalData _localNewUnlock = new ProduceLocalData();

	private bool _isRequiredData;

	private CCreateInfo _cCreateInfo = new CCreateInfo();

	private CCreateItem _cCreateItem = new CCreateItem();

	private CCancelCreate _cCancelCreate = new CCancelCreate();

	private CChangeCreateNum _cChangeCreateNum = new CChangeCreateNum();

	private CEditShortcutKey _cEditShortcutKey = new CEditShortcutKey();

	private CChangeCreateIndex _cChangeCreateIndex = new CChangeCreateIndex();

	private CLearnDrawing _cLearnDrawing = new CLearnDrawing();

	public int CountDown;

	public bool IsLockCreate;

	private CGetFinshCreate _cGetFinishCreate = new CGetFinshCreate();

	public void Init()
	{
		SCreateInfo.handler = (SCreateInfo.Handler)Delegate.Combine(SCreateInfo.handler, new SCreateInfo.Handler(OnSCreateInfo));
		SOneCreateFinish.handler = (SOneCreateFinish.Handler)Delegate.Combine(SOneCreateFinish.handler, new SOneCreateFinish.Handler(OnSOneCreateFinish));
		SCreateItem.handler = (SCreateItem.Handler)Delegate.Combine(SCreateItem.handler, new SCreateItem.Handler(OnSCreateItem));
		SCancelCreate.handler = (SCancelCreate.Handler)Delegate.Combine(SCancelCreate.handler, new SCancelCreate.Handler(OnSCancelCreate));
		SChangeCreateNum.handler = (SChangeCreateNum.Handler)Delegate.Combine(SChangeCreateNum.handler, new SChangeCreateNum.Handler(OnSChangeCreateNum));
		SEditShortcutKey.handler = (SEditShortcutKey.Handler)Delegate.Combine(SEditShortcutKey.handler, new SEditShortcutKey.Handler(OnSEditShortcutKey));
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinished));
		RoleEvent.LevelChangeDelegate = (Utils.Int2Delegate)Delegate.Combine(RoleEvent.LevelChangeDelegate, new Utils.Int2Delegate(RefreshUnlockDrawingSet));
		SCreateItemError.handler = (SCreateItemError.Handler)Delegate.Combine(SCreateItemError.handler, new SCreateItemError.Handler(OnSCreateItemError));
		SChangeCreateIndex.handler = (SChangeCreateIndex.Handler)Delegate.Combine(SChangeCreateIndex.handler, new SChangeCreateIndex.Handler(OnSChangeCreateIndex));
		SLearnDrawing.handler = (SLearnDrawing.Handler)Delegate.Combine(SLearnDrawing.handler, new SLearnDrawing.Handler(OnSLearnDrawing));
		SGetFinshCreate.handler = (SGetFinshCreate.Handler)Delegate.Combine(SGetFinshCreate.handler, new SGetFinshCreate.Handler(OnSGetFinshCreate));
		if (IsTest)
		{
			TestCreateList();
		}
		ViewMgr.Ins.AddOnHideEvent<BagAndBuildPanel>(OnPanelHide);
	}

	private void OnPanelHide()
	{
		_oldPlayerPos.y = -1000f;
	}

	private void OnSGetFinshCreate(SGetFinshCreate msg)
	{
		IsLockCreate = false;
	}

	private void OnSLearnDrawing(SLearnDrawing msg)
	{
		_learnedDrawingSet.Add(msg.learnedDrawId);
		SaveNewUnlock(msg.learnedDrawId);
		Utils.TriggerEvent(ScProduceEvent.ItemNewUnlockChangeDelegate);
		SaveForAllSetChange(_learnedDrawingSet);
	}

	private void OnSChangeCreateIndex(SChangeCreateIndex msg)
	{
		CreatingItemsList = msg.ceateInfos;
		int num = ((_cChangeCreateIndex.targetIndex <= _cChangeCreateIndex.changeIndex) ? _cChangeCreateIndex.changeIndex : _cChangeCreateIndex.targetIndex);
		int num2 = ((_cChangeCreateIndex.targetIndex >= _cChangeCreateIndex.changeIndex) ? _cChangeCreateIndex.changeIndex : _cChangeCreateIndex.targetIndex);
		if (num > 0 && num2 == 0)
		{
			ChangeFinishTime();
		}
		Utils.TriggerEvent(ScProduceEvent.CreateListChangeDelegate);
	}

	private void TestCreateList()
	{
		for (int i = 0; i < 4; i++)
		{
			CreatingItemsList.Add(new CreateInfo
			{
				createId = i + 1,
				createNum = 1
			});
		}
	}

	private void OnSCreateItemError(SCreateItemError msg)
	{
		int num = 0;
		if (msg.code == 1)
		{
			num = 21;
		}
		else if (msg.code == 3)
		{
			num = 22;
		}
		else if (msg.code == 4)
		{
			num = 23;
		}
		else if (msg.code == 5)
		{
			num = 24;
		}
		else if (msg.code == 6)
		{
			num = 25;
		}
		else if (msg.code == 7)
		{
			num = 26;
		}
		else if (msg.code == 8)
		{
			num = 27;
		}
		else if (msg.code == 10)
		{
			num = 28;
		}
		else if (msg.code == 11)
		{
			num = 29;
		}
		if (num == 0)
		{
			Debug.LogError("[ScProduceMgr]SCreateItemError : code = " + msg.code);
		}
		else
		{
			AlertBox.Show(num);
		}
	}

	private void OnSLoginFinished(SLoginFinished msg)
	{
		_localNewUnlock.RoleId = Singleton<RoleMgr>.Ins.info.roleId;
		Utils.StartConroutine(LoadProduceLocalData());
		InitDic();
	}

	private void InitDic()
	{
		List<DrawingCfg> allList = DrawingCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			DrawingCfg drawingCfg = allList[i];
			DicProductionId2DrawId[drawingCfg.targetItemId] = drawingCfg.id;
		}
	}

	private void OnSEditShortcutKey(SEditShortcutKey msg)
	{
		ShortcutItemList = msg.shortcutKeys;
		Utils.TriggerEvent(ScProduceEvent.ChangeShortcutKeyDelegate);
	}

	private void OnSChangeCreateNum(SChangeCreateNum msg)
	{
		if (CreatingItemsList.Count > 0 && msg.ceateInfos.Count > 0)
		{
			CreateInfo createInfo = CreatingItemsList[0];
			int num = (int)(CurItemFinishTime - Time.realtimeSinceStartup);
			int num2 = DrawingCfg.Get(createInfo.createId).needTime;
			if (num2 <= 0)
			{
				num2 = 1;
			}
			int num3 = num / num2 + 1;
			CurItemFinishTime += (float)((msg.ceateInfos[0].createNum - ((num3 >= createInfo.createNum) ? createInfo.createNum : num3)) * DrawingCfg.Get(createInfo.createId).needTime) - 0.5f;
		}
		else if (msg.ceateInfos.Count > 0)
		{
			ChangeFinishTime();
		}
		OnCreateListChange(msg.ceateInfos);
	}

	private void OnSCancelCreate(SCancelCreate msg)
	{
		OnCreateListChange(msg.ceateInfos);
		if (msg.ceateInfos.Count > 0)
		{
			ChangeFinishTime();
		}
		Utils.TriggerEvent(ScProduceEvent.CreateListChangeDelegate);
	}

	private void OnSCreateItem(SCreateItem msg)
	{
		OnCreateListChange(msg.ceateInfos);
		if (msg.ceateInfos.Count == 1)
		{
			ChangeFinishTime();
		}
		Utils.TriggerEvent(ScProduceEvent.ItemListChangeDelegate);
	}

	private void OnSOneCreateFinish(SOneCreateFinish msg)
	{
		OnCreateListChange(msg.ceateInfos);
		if (msg.ceateInfos.Count > 0)
		{
			ChangeFinishTime();
		}
		Utils.TriggerEvent(ScProduceEvent.ItemListChangeDelegate);
	}

	private void ChangeFinishTime()
	{
		CurItemFinishTime = Time.realtimeSinceStartup + (float)(DrawingCfg.Get(CreatingItemsList[0].createId).needTime * CreatingItemsList[0].createNum) - 0.5f;
	}

	private void OnCreateListChange(List<CreateInfo> createInfos)
	{
		CreatingItemsList = createInfos;
		Utils.TriggerEvent(ScProduceEvent.CreateListChangeDelegate);
	}

	private void OnSCreateInfo(SCreateInfo msg)
	{
		IsLockCreate = false;
		_isRequiredData = true;
		_learnedDrawingSet = msg.learnedDrawings;
		ShortcutItemList = msg.shortcutKeys;
		CreatingItemsList = msg.ceateInfos;
		if (CreatingItemsList.Count > 0)
		{
			DrawingCfg drawingCfg = DrawingCfg.Get(CreatingItemsList[0].createId);
			CurItemFinishTime = Time.realtimeSinceStartup + (float)(drawingCfg.needTime * CreatingItemsList[0].createNum) - (float)msg.passTime / 1000f - 0.5f;
			if ((float)msg.passTime / 1000f >= (float)drawingCfg.needTime && drawingCfg.needTime > 0)
			{
				IsLockCreate = true;
				CountDown = drawingCfg.needTime * CreatingItemsList[0].createNum - (int)((float)msg.passTime / 1000f);
			}
		}
		InitItemLists();
		Utils.TriggerEvent(ScProduceEvent.CreateListChangeDelegate);
		Utils.TriggerEvent(ScProduceEvent.ChangeShortcutKeyDelegate);
		Utils.TriggerEvent(ScProduceEvent.ItemListChangeDelegate);
		Utils.TriggerEvent(ScProduceEvent.ItemCanProduceChangeDelegate);
		Utils.TriggerEvent(ScProduceEvent.ItemNewUnlockChangeDelegate);
		Utils.TriggerEvent(ScProduceEvent.UpdateAllBtnRedDotDelegate);
	}

	public void InitItemLists()
	{
		DicType2Cfgs.Clear();
		List<DrawingCfg> allList = DrawingCfg.GetAllList();
		DicType2Cfgs.Add(0, new List<DrawingCfg>());
		DicType2Cfgs.Add(1, new List<DrawingCfg>());
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			DrawingCfg drawingCfg = allList[i];
			List<DrawingCfg> value;
			if (!DicType2Cfgs.TryGetValue(drawingCfg.smallType, out value))
			{
				value = new List<DrawingCfg>();
				DicType2Cfgs.Add(drawingCfg.smallType, value);
			}
			value.Add(drawingCfg);
			if (drawingCfg.smallType != drawingCfg.bigType)
			{
				if (!DicType2Cfgs.TryGetValue(drawingCfg.bigType, out value))
				{
					value = new List<DrawingCfg>();
					DicType2Cfgs.Add(drawingCfg.bigType, value);
				}
				value.Add(drawingCfg);
			}
			if (drawingCfg.isNormal)
			{
				DicType2Cfgs[1].Add(drawingCfg);
			}
			List<DrawingCfg> value2;
			if (!_dicLevel2DrawingCfgs.TryGetValue(drawingCfg.levelLimit, out value2))
			{
				value2 = new List<DrawingCfg>();
				_dicLevel2DrawingCfgs.Add(drawingCfg.levelLimit, value2);
			}
			if (drawingCfg.isDefault)
			{
				value2.Add(drawingCfg);
			}
		}
	}

	public void RefreshCanProduceSet()
	{
		CanProduce.Clear();
		int level = Singleton<RoleMgr>.Ins.info.level;
		List<DrawingCfg> allList = DrawingCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			bool flag = IsLocked(allList[i], level);
			List<DrawingNeedMaterial> material = allList[i].material;
			bool flag2 = true;
			for (int j = 0; j < material.Count; j++)
			{
				int itemId = material[j].itemId;
				if (Singleton<BagMgr>.Ins.GetItemNum(itemId) < material[j].num || flag)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				CanProduce.Add(allList[i].id);
			}
		}
		Utils.TriggerEvent(ScProduceEvent.ItemCanProduceChangeDelegate);
	}

	public bool IsLocked(DrawingCfg drawingInfo, int roleLevel)
	{
		if (drawingInfo == null)
		{
			return true;
		}
		if (drawingInfo.levelLimit > roleLevel)
		{
			return true;
		}
		if (drawingInfo.isDefault)
		{
			return false;
		}
		return !IsLearned(drawingInfo.id);
	}

	public bool MeetWorkbenchCondition(DrawingCfg createInfo)
	{
		if (createInfo == null || createInfo.needWorkbench == 0)
		{
			return true;
		}
		if (!IsMove())
		{
			long num = ((long)createInfo.needWorkbench << 32) + createInfo.needWorkbenchLevel;
			return _lastResult.Contains(num) || _lastResult.Contains(num + 1) || _lastResult.Contains(num + 2) || _lastResult.Contains(num + 3);
		}
		_lastResult.Clear();
		int num2 = Physics.OverlapSphereNonAlloc(_oldPlayerPos, 14f, cos, (1 << BattleScMgr.DefaultLayer) | (1 << BattleScMgr.OutlineLayer), QueryTriggerInteraction.Ignore);
		if (num2 > 0)
		{
			for (int i = 0; i < num2; i++)
			{
				PartBehaviour componentInParent = cos[i].GetComponentInParent<PartBehaviour>();
				if (!componentInParent || componentInParent.MyCfg == null)
				{
					continue;
				}
				ItemCfg itemCfg = ItemCfg.Get(componentInParent.MyCfg.id);
				if (itemCfg != null)
				{
					switch (itemCfg.childType)
					{
					case 97:
						_lastResult.Add(4294967296L + componentInParent.MyCfg.lv);
						break;
					case 98:
						_lastResult.Add(17179869184L + componentInParent.MyCfg.lv);
						break;
					}
				}
			}
			long num3 = ((long)createInfo.needWorkbench << 32) + createInfo.needWorkbenchLevel;
			return _lastResult.Contains(num3) || _lastResult.Contains(num3 + 1) || _lastResult.Contains(num3 + 2) || _lastResult.Contains(num3 + 3);
		}
		return false;
	}

	private bool IsMove()
	{
		if ((bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer)
		{
			Vector3 pos = Battle.Ins.SelfPlayer.Pos;
			float num = pos.x - _oldPlayerPos.x;
			float num2 = pos.y - _oldPlayerPos.y;
			float num3 = pos.z - _oldPlayerPos.z;
			bool flag = num * num + num2 * num2 + num3 * num3 > 0.125f;
			if (flag)
			{
				_oldPlayerPos = pos;
			}
			return flag;
		}
		return false;
	}

	private void RefreshUnlockDrawingSet(int oldLevel, int curLevel)
	{
		if (_dicLevel2DrawingCfgs.Count == 0)
		{
			InitItemLists();
		}
		List<DrawingCfg> list = new List<DrawingCfg>();
		for (int i = oldLevel + 1; i < curLevel + 1; i++)
		{
			List<DrawingCfg> value;
			if (_dicLevel2DrawingCfgs.TryGetValue(i, out value))
			{
				list.AddRange(value);
			}
		}
		foreach (DrawingCfg item in list)
		{
			NewUnlock.Add(item.id);
		}
		SaveForAllSetChange(NewUnlock);
	}

	private IEnumerator LoadProduceLocalData()
	{
		if (_isLoadInfo)
		{
			yield break;
		}
		WWW www = new WWW(Utils.GetPersistentPathForWWW(_newUnlockPath));
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			MarshalProduceData(www.bytes);
		}
		else
		{
			try
			{
				_localNewUnlock.RoleId = Singleton<RoleMgr>.Ins.info.roleId;
				File.Delete(Utils.GetPersistentPath(_newUnlockPath));
			}
			catch
			{
			}
		}
		_isLoadInfo = true;
	}

	private void MarshalProduceData(byte[] bytes)
	{
		Octets oc = new Octets(bytes, bytes.Length);
		try
		{
			_localNewUnlock.unmarshal(oc);
		}
		catch (Exception)
		{
			_localNewUnlock = new ProduceLocalData();
			_localNewUnlock.RoleId = Singleton<RoleMgr>.Ins.info.roleId;
			try
			{
				File.Delete(Utils.GetPersistentPath(_newUnlockPath));
			}
			catch
			{
			}
		}
		if (_localNewUnlock.RoleId != Singleton<RoleMgr>.Ins.info.roleId)
		{
			_localNewUnlock.RoleId = Singleton<RoleMgr>.Ins.info.roleId;
			_localNewUnlock.NewUnlock.Clear();
			Save(false);
		}
		NewUnlock = _localNewUnlock.NewUnlock;
	}

	public void SaveForAllSetChange(HashSet<int> allNewUnlock, bool isUpdateScroll = true)
	{
		_localNewUnlock.NewUnlock = allNewUnlock;
		Save(isUpdateScroll);
	}

	public void SaveNewUnlock(int newUnlockId)
	{
		NewUnlock.Add(newUnlockId);
		_localNewUnlock.NewUnlock.Add(newUnlockId);
		Save();
	}

	public void DeleteNewUnlock(int noticedUnlockId)
	{
		NewUnlock.Remove(noticedUnlockId);
		_localNewUnlock.NewUnlock.Remove(noticedUnlockId);
		Save();
	}

	public void Save(bool isUpdateScroll = true)
	{
		Octets oc = new Octets();
		oc = _localNewUnlock.marshal(oc);
		Stream stream = new FileStream(Utils.GetPersistentPath(_newUnlockPath), FileMode.OpenOrCreate);
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(oc.getBytes());
		binaryWriter.Close();
		stream.Close();
		if (isUpdateScroll)
		{
			Utils.TriggerEvent(ScProduceEvent.ItemNewUnlockChangeDelegate);
		}
	}

	public bool IsLearned(int drawingId)
	{
		DrawingCfg drawingCfg = DrawingCfg.Get(drawingId);
		if (drawingCfg != null && drawingCfg.isDefault)
		{
			return true;
		}
		return _learnedDrawingSet != null && _learnedDrawingSet.Contains(drawingId);
	}

	public bool IsShortcut(int drawingId)
	{
		int i = 0;
		for (int count = ShortcutItemList.Count; i < count; i++)
		{
			if (ShortcutItemList[i] == drawingId)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCreating(int drawingId)
	{
		for (int i = 0; i < CreatingItemsList.Count; i++)
		{
			if (CreatingItemsList[i].createId == drawingId)
			{
				return true;
			}
		}
		return false;
	}

	public void SendRequireCreateInfoMsg(bool forceRequire = false)
	{
		if (!_isRequiredData || forceRequire)
		{
			Client2Gs.Ins.Send(_cCreateInfo);
		}
	}

	public bool SendCreateItemMsg(int drawingId, int num)
	{
		DrawingCfg drawingCfg = DrawingCfg.Get(drawingId);
		if (drawingCfg == null)
		{
			Debug.LogError("[ScProduceMgr]:No such drawing.DrawingId : " + drawingId);
			return false;
		}
		if (num == 0)
		{
			return false;
		}
		_cCreateItem.item.createId = drawingId;
		_cCreateItem.item.createNum = num;
		Client2Gs.Ins.Send(_cCreateItem);
		return true;
	}

	public bool SendCancelCreateMsg(int index)
	{
		if (CreatingItemsList.Count < index || index < 0)
		{
			return false;
		}
		_cCancelCreate.cancelIndex = index;
		Client2Gs.Ins.Send(_cCancelCreate);
		return true;
	}

	public bool SendChangeCreateNumMsg(int index, int changeNum)
	{
		if (CreatingItemsList.Count < index)
		{
			return false;
		}
		_cChangeCreateNum.changeIndex = index;
		_cChangeCreateNum.changeNum = changeNum;
		Client2Gs.Ins.Send(_cChangeCreateNum);
		return true;
	}

	public void SendEditShortcutKeyMsg(int shortcutId, int editType)
	{
		_cEditShortcutKey.shortcutId = shortcutId;
		_cEditShortcutKey.editType = editType;
		Client2Gs.Ins.Send(_cEditShortcutKey);
	}

	public void SendChangeIndexMsg(int fromIndex, int targetIndex)
	{
		if (IsTest)
		{
			Debug.Log("[send change msg]fromIndex : " + fromIndex + " targetIndex : " + targetIndex);
		}
		else
		{
			_cChangeCreateIndex.changeIndex = fromIndex;
			_cChangeCreateIndex.targetIndex = targetIndex;
			Client2Gs.Ins.Send(_cChangeCreateIndex);
		}
	}

	public void SendLearnDrawingMsg(int cfgId)
	{
		_cLearnDrawing.drawingId = cfgId;
		Client2Gs.Ins.Send(_cLearnDrawing);
	}

	public void SendCGetFinishCreateMsg()
	{
		Client2Gs.Ins.Send(_cGetFinishCreate);
	}
}
