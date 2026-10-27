using System;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.cook.scmsg;
using gs.online.scmsg;
using gs.smelter.scmsg;

public class CookMgr : Singleton<CookMgr>
{
	private struct CookbookKey
	{
		public ulong Key1;

		public ulong Key2;
	}

	public struct CookMaterial
	{
		public BagItem bagItemInfo;

		public int OnceConsumeNum;

		public CookMaterial(BagItem mBagItemInfo, int mTotalNum)
		{
			bagItemInfo = mBagItemInfo;
			OnceConsumeNum = 1;
		}
	}

	private const ulong One = 1uL;

	private int _nextSignIndex;

	private Dictionary<int, int> _dicItemId2SignIndex = new Dictionary<int, int>();

	private Dictionary<CookbookKey, int> _dicMaterial2Food = new Dictionary<CookbookKey, int>();

	private long _curOpenKitchenInstanceId;

	private LinkedList<CookMaterial> _allMaterials = new LinkedList<CookMaterial>();

	private Dictionary<int, LinkedListNode<CookMaterial>> _dicItemId2MaterialInfo = new Dictionary<int, LinkedListNode<CookMaterial>>();

	public int CanAddFuelNum;

	private CDoStart _cDoStart = new CDoStart();

	private CCampFireOrFirePlaceInfo _cCampFireOrFirePlaceInfo = new CCampFireOrFirePlaceInfo();

	private CDoAddFuel _cAddFuel = new CDoAddFuel();

	private CDoStop _cStop = new CDoStop();

	private CDoGetFuel _cGetFuel = new CDoGetFuel();

	private CDoGetFinished _cGetFinished = new CDoGetFinished();

	private void InitMaterial2Food()
	{
		List<CookbookCfg> allList = CookbookCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			CookbookKey cookbookIdByMaterials = GetCookbookIdByMaterials(allList[i].material);
			int value;
			if (_dicMaterial2Food.TryGetValue(cookbookIdByMaterials, out value))
			{
				Debug.LogError("Key Repeat : id1 - " + value + "   id2 - " + allList[i].id);
				break;
			}
			_dicMaterial2Food[cookbookIdByMaterials] = allList[i].id;
		}
	}

	private CookbookKey GetCookbookIdByMaterials<T>(Dictionary<int, T> materials)
	{
		CookbookKey result = default(CookbookKey);
		if (materials != null && materials.Count > 0)
		{
			foreach (int key in materials.Keys)
			{
				int value;
				if (!_dicItemId2SignIndex.TryGetValue(key, out value))
				{
					value = Singleton<CookMgr>.Ins._nextSignIndex;
					_dicItemId2SignIndex[key] = Singleton<CookMgr>.Ins._nextSignIndex++;
				}
				if (value < 63)
				{
					result.Key1 |= (ulong)(1L << value);
				}
				else if (value < 127)
				{
					result.Key2 |= (ulong)(1L << value - 64);
				}
				else
				{
					Debug.LogError("Key Not Enough.");
					AlertBox.Show("CookMgr:Key Not Enough.");
				}
			}
			return result;
		}
		Debug.LogError("CookMgr : Material is null or empty.");
		return result;
	}

	public void Init()
	{
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnUseBuild));
		SCampFireOrFirePlaceInfo.handler = (SCampFireOrFirePlaceInfo.Handler)Delegate.Combine(SCampFireOrFirePlaceInfo.handler, new SCampFireOrFirePlaceInfo.Handler(OnSCampFireOrFirePlaceInfo));
		SCampFireOrFirePlaceError.handler = (SCampFireOrFirePlaceError.Handler)Delegate.Combine(SCampFireOrFirePlaceError.handler, new SCampFireOrFirePlaceError.Handler(OnSCampFireOrFirePlaceError));
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinished));
	}

	private void OnSCampFireOrFirePlaceError(SCampFireOrFirePlaceError msg)
	{
		int num = 0;
		switch (msg.code)
		{
		case 1:
			num = 191;
			break;
		case 2:
			num = 192;
			break;
		case 3:
			num = 193;
			break;
		case 4:
			num = 194;
			break;
		case 5:
			num = 195;
			break;
		case 6:
			num = 196;
			break;
		case 7:
			num = 197;
			break;
		case 8:
			num = 198;
			break;
		}
		if (num > 0)
		{
			AlertBox.Show(num);
		}
	}

	private void OnSCampFireOrFirePlaceInfo(SCampFireOrFirePlaceInfo msg)
	{
		if (_curOpenKitchenInstanceId == msg.instanceId)
		{
			ViewMgr.Ins.ShowView<FirePanel>(msg, false);
		}
	}

	private void OnUseBuild(int itemId, long instanceId)
	{
		if (ItemCfg.Get(itemId).childType == 101)
		{
			_curOpenKitchenInstanceId = instanceId;
			SendRequireKitchenMsg();
		}
	}

	private void OnSLoginFinished(SLoginFinished msg)
	{
		InitMaterial2Food();
	}

	public bool GetCookbookCfgId(out int maxCookNum, out int cookbookCfgId)
	{
		if (_dicItemId2MaterialInfo.Count <= 0)
		{
			maxCookNum = 0;
			cookbookCfgId = cfg.Consts.COOK_DARK_DISHES_ITEMID;
			return true;
		}
		if (_dicMaterial2Food.TryGetValue(GetCookbookIdByMaterials(_dicItemId2MaterialInfo), out cookbookCfgId))
		{
			CookbookCfg cookbookCfg = CookbookCfg.Get(cookbookCfgId);
			maxCookNum = ItemCfg.Get(cookbookCfgId).maxPileNum / cookbookCfg.outNum;
			Dictionary<int, int> material = cookbookCfg.material;
			foreach (KeyValuePair<int, int> item in material)
			{
				LinkedListNode<CookMaterial> value;
				if (!_dicItemId2MaterialInfo.TryGetValue(item.Key, out value))
				{
					maxCookNum = 0;
					Debug.LogError("CookMgr : Cookbook key value not match.");
					return true;
				}
				CookMaterial value2 = value.Value;
				value2.OnceConsumeNum = item.Value;
				value.Value = value2;
				int num = value.Value.bagItemInfo.number / item.Value;
				maxCookNum = ((maxCookNum >= num) ? num : maxCookNum);
			}
			if (maxCookNum > 0)
			{
				AlertBox.Show(144);
				return true;
			}
		}
		cookbookCfgId = cfg.Consts.COOK_DARK_DISHES_ITEMID;
		maxCookNum = ItemCfg.Get(cookbookCfgId).maxPileNum;
		foreach (LinkedListNode<CookMaterial> value4 in _dicItemId2MaterialInfo.Values)
		{
			int number = value4.Value.bagItemInfo.number;
			maxCookNum = ((number >= maxCookNum) ? maxCookNum : number);
			CookMaterial value3 = value4.Value;
			value3.OnceConsumeNum = 1;
			value4.Value = value3;
		}
		return false;
	}

	public LinkedListNode<CookMaterial> GetFirstMaterial()
	{
		return _allMaterials.First;
	}

	public LinkedList<CookMaterial> GetMaterialNotCook()
	{
		return _allMaterials;
	}

	public void AddMaterial(BagItem bagItemInfo, int maxMaterialNum)
	{
		if (_dicItemId2MaterialInfo.Count < maxMaterialNum)
		{
			CookMaterial value = new CookMaterial(bagItemInfo, bagItemInfo.number);
			LinkedListNode<CookMaterial> linkedListNode = new LinkedListNode<CookMaterial>(value);
			_dicItemId2MaterialInfo[bagItemInfo.itemId] = linkedListNode;
			_allMaterials.AddLast(linkedListNode);
			if (CookEvent.OnAddMaterialAction != null)
			{
				CookEvent.OnAddMaterialAction(linkedListNode, _dicItemId2MaterialInfo.Count - 1);
			}
		}
		else
		{
			AlertBox.Show(143);
		}
	}

	public void RemoveMaterial(int itemId)
	{
		LinkedListNode<CookMaterial> node = _dicItemId2MaterialInfo[itemId];
		_allMaterials.Remove(node);
		_dicItemId2MaterialInfo.Remove(itemId);
		if (CookEvent.OnRemoveMaterialAction != null)
		{
			CookEvent.OnRemoveMaterialAction();
		}
	}

	public void AllMaterialReduce(int num)
	{
		for (LinkedListNode<CookMaterial> linkedListNode = _allMaterials.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.bagItemInfo.number -= num;
		}
	}

	public bool IsInUse(int itemId)
	{
		return _dicItemId2MaterialInfo.ContainsKey(itemId);
	}

	public void ClearAllData()
	{
		_allMaterials.Clear();
		_dicItemId2MaterialInfo.Clear();
	}

	public void SendDoStartMsg(int num)
	{
		_cDoStart.instanceId = _curOpenKitchenInstanceId;
		_cDoStart.rawMaterials = GetMaterialUsed(num);
		if (_cDoStart.rawMaterials.Count == 0)
		{
			num = 0;
		}
		else if (num <= 0)
		{
			return;
		}
		_cDoStart.cookNum = num;
		Client2Gs.Ins.Send(_cDoStart);
	}

	private Dictionary<int, UseItem> GetMaterialUsed(int cookNum)
	{
		Dictionary<int, UseItem> dictionary = new Dictionary<int, UseItem>();
		LinkedListNode<CookMaterial> linkedListNode = _allMaterials.First;
		int num = 0;
		while (linkedListNode != null)
		{
			UseItem useItem = new UseItem();
			useItem.id = linkedListNode.Value.bagItemInfo.itemId;
			useItem.num = linkedListNode.Value.OnceConsumeNum * cookNum;
			dictionary.Add(num++, useItem);
			linkedListNode = linkedListNode.Next;
		}
		return dictionary;
	}

	public void SendRequireKitchenMsg()
	{
		_cCampFireOrFirePlaceInfo.instanceId = _curOpenKitchenInstanceId;
		Client2Gs.Ins.Send(_cCampFireOrFirePlaceInfo);
	}

	public void SendAddFuelMsg(UseItem fuel)
	{
		_cAddFuel.instanceId = _curOpenKitchenInstanceId;
		_cAddFuel.fuels = fuel;
		Client2Gs.Ins.Send(_cAddFuel);
	}

	public void SendDoStopMsg()
	{
		_cStop.instanceId = _curOpenKitchenInstanceId;
		Client2Gs.Ins.Send(_cStop);
	}

	public void SendGetFuel()
	{
		_cGetFuel.instanceId = _curOpenKitchenInstanceId;
		Client2Gs.Ins.Send(_cGetFuel);
	}

	public void SendGetFinished()
	{
		_cGetFinished.instanceId = _curOpenKitchenInstanceId;
		Client2Gs.Ins.Send(_cGetFinished);
	}
}
