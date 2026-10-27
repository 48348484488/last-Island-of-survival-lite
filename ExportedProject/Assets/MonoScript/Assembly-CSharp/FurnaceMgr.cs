using System;
using System.Collections.Generic;
using SC.UI;
using cfg;
using gs.bag.scmsg;
using gs.smelter.scmsg;

public class FurnaceMgr : Singleton<FurnaceMgr>
{
	private long _curOpenSmelterInstanceId;

	private SSmelterInfo _curOpenSmelter;

	public List<UseItem> FuelList = new List<UseItem>();

	public List<UseItem> OreList = new List<UseItem>();

	public List<UseItem> ProductionList = new List<UseItem>();

	public OreProductionMgr OreProduct;

	private CSmelterInfo _cSmelterInfo = new CSmelterInfo();

	private CStart _cStart = new CStart();

	private CStop _cStop = new CStop();

	private CAddFuel _cAddFuel = new CAddFuel();

	private CAddRawMaterial _cAddOre = new CAddRawMaterial();

	private CGetRawMaterial _cGetRawMaterial = new CGetRawMaterial();

	private CGetFuel _cGetFuel = new CGetFuel();

	private CGetFinished _cGetFinished = new CGetFinished();

	public BagItem JiaShaA;

	public Action AiJiaShaJiaSha;

	public void Init()
	{
		SSmelterInfo.handler = (SSmelterInfo.Handler)Delegate.Combine(SSmelterInfo.handler, new SSmelterInfo.Handler(OnSSmelterInfo));
		SSmelterError.handler = (SSmelterError.Handler)Delegate.Combine(SSmelterError.handler, new SSmelterError.Handler(OnSSmelterError));
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OpenSmelter));
	}

	public void ClearAllData()
	{
		FuelList.Clear();
		OreList.Clear();
		ProductionList.Clear();
		OreProduct = null;
		_curOpenSmelter = null;
	}

	private void OnSSmelterError(SSmelterError msg)
	{
		int num = 0;
		switch (msg.code)
		{
		case 1:
			num = 182;
			break;
		case 2:
			num = 183;
			break;
		case 3:
			num = 184;
			break;
		case 4:
			num = 185;
			break;
		case 5:
			num = 186;
			break;
		case 6:
			num = 187;
			break;
		case 7:
			num = 188;
			break;
		case 8:
			num = 189;
			break;
		case 9:
			num = 190;
			break;
		}
		if (num > 0)
		{
			AlertBox.Show(num);
		}
	}

	private void OnSSmelterInfo(SSmelterInfo msg)
	{
		if (_curOpenSmelterInstanceId != msg.smelterId)
		{
			return;
		}
		_curOpenSmelter = msg;
		if (OreProduct == null)
		{
			OreProduct = new OreProductionMgr();
			if (FurnaceEvent.ResetOreProductionDataDelegate != null)
			{
				FurnaceEvent.ResetOreProductionDataDelegate();
			}
		}
		OreProduct.Clear();
		OreList.Clear();
		FuelList.Clear();
		ProductionList.Clear();
		int num = (int)ItemCfg.Get(msg.cfgId).extras[0];
		int num2 = (int)ItemCfg.Get(msg.cfgId).extras[1];
		int num3 = (int)ItemCfg.Get(msg.cfgId).extras[2];
		for (int i = 0; i < num; i++)
		{
			UseItem value = null;
			msg.rawMaterials.TryGetValue(i, out value);
			if (value != null && value.num > 0)
			{
				OreList.Add(value);
			}
			else
			{
				OreList.Add(null);
			}
		}
		for (int j = 0; j < num2; j++)
		{
			UseItem value2 = null;
			msg.fuels.TryGetValue(j, out value2);
			if (value2 != null && value2.num > 0 && value2.fuelTime > 0)
			{
				value2.num--;
			}
			FuelList.Add(value2);
		}
		for (int k = 0; k < num3; k++)
		{
			if (msg.finisheds.Count > k)
			{
				ProductionList.Add(msg.finisheds[k]);
			}
			else
			{
				ProductionList.Add(null);
			}
		}
		if (msg.isStart)
		{
			OreProduct.StartSmelt(OreList, ProductionList, GetFuelTime(FuelList));
			OreProduct.IsBurning = true;
		}
		else
		{
			OreProduct.SmeltStopState(OreList, ProductionList);
			OreProduct.IsBurning = false;
		}
		ViewMgr.Ins.ShowView<FurnacePanel>(null, false);
	}

	public float GetFuelTime(List<UseItem> fuels)
	{
		float num = 0f;
		int i = 0;
		for (int count = fuels.Count; i < count; i++)
		{
			UseItem useItem = fuels[i];
			if (useItem != null)
			{
				SmelterFuelCfg smelterFuelCfg = SmelterFuelCfg.Get(useItem.id);
				num += (float)(smelterFuelCfg.burnTime * useItem.num);
				if (useItem.fuelTime > 0)
				{
					num += (float)(smelterFuelCfg.burnTime - useItem.num);
				}
			}
		}
		return num;
	}

	public void OpenSmelter(long instanceId)
	{
		if (OreProduct == null)
		{
			OreProduct = new OreProductionMgr();
			if (FurnaceEvent.ResetOreProductionDataDelegate != null)
			{
				FurnaceEvent.ResetOreProductionDataDelegate();
			}
		}
		OreProduct.Clear();
		_curOpenSmelterInstanceId = instanceId;
		Singleton<FurnaceMgr>.Ins.SendRequireSmelterInfoMsg();
	}

	private void OpenSmelter(int itemId, long instanceId)
	{
		if (ItemCfg.Get(itemId).childType == 98)
		{
			OpenSmelter(instanceId);
		}
	}

	public int CurOpenSmelterItemId()
	{
		if (_curOpenSmelter != null)
		{
			return _curOpenSmelter.cfgId;
		}
		return 0;
	}

	public void SendRequireSmelterInfoMsg()
	{
		_cSmelterInfo.smelterId = _curOpenSmelterInstanceId;
		Client2Gs.Ins.Send(_cSmelterInfo);
	}

	public void SendStartSmeltMsg()
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cStart.smelterId = _curOpenSmelterInstanceId;
			Client2Gs.Ins.Send(_cStart);
		}
	}

	public void SendCancelSmeltMsg()
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cStop.smelterId = _curOpenSmelterInstanceId;
			Client2Gs.Ins.Send(_cStop);
		}
	}

	public void SendAddFuelMsg(Dictionary<int, UseItem> dicIndex2Fuel)
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cAddFuel.smelterId = _curOpenSmelterInstanceId;
			_cAddFuel.fuels = dicIndex2Fuel;
			Client2Gs.Ins.Send(_cAddFuel);
		}
	}

	public void SendAddOreMsg(Dictionary<int, UseItem> dicIndex2Ore)
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cAddOre.smelterId = _curOpenSmelterInstanceId;
			_cAddOre.rawMaterials = dicIndex2Ore;
			Client2Gs.Ins.Send(_cAddOre);
		}
	}

	public void SendGetOreMsg(int index, bool isAllGet = false)
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cGetRawMaterial.smelterId = _curOpenSmelterInstanceId;
			_cGetRawMaterial.getIndex = index;
			_cGetRawMaterial.isAll = isAllGet;
			Client2Gs.Ins.Send(_cGetRawMaterial);
		}
	}

	public void SendGetFuelMsg(int index, bool isAllGet = false)
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cGetFuel.smelterId = _curOpenSmelterInstanceId;
			_cGetFuel.getIndex = index;
			_cGetFuel.isAll = isAllGet;
			Client2Gs.Ins.Send(_cGetFuel);
		}
	}

	public void SendGetProductionMsg(int index, bool isAllGet)
	{
		if (_curOpenSmelterInstanceId > 0)
		{
			_cGetFinished.smelterId = _curOpenSmelterInstanceId;
			_cGetFinished.getIndex = 0;
			_cGetFinished.isAll = isAllGet;
			Client2Gs.Ins.Send(_cGetFinished);
		}
	}

	public bool BagCapacityEnough()
	{
		int num = 0;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int i = 0;
		for (int count = ProductionList.Count; i < count; i++)
		{
			UseItem useItem = ProductionList[i];
			if (useItem == null)
			{
				continue;
			}
			int value;
			if (!dictionary.TryGetValue(useItem.id, out value))
			{
				dictionary[useItem.id] = useItem.num;
				continue;
			}
			int maxPileNum = ItemCfg.Get(useItem.id).maxPileNum;
			value += useItem.num;
			if (value >= maxPileNum)
			{
				num++;
				value -= maxPileNum;
			}
			dictionary[useItem.id] = value;
		}
		int num2 = Singleton<BagMgr>.Ins.BagCapacity - Singleton<BagMgr>.Ins.BagItems.Count - num;
		if (num2 < 0)
		{
			return false;
		}
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			if (item.Value > 0 && Singleton<BagMgr>.Ins.GetRemainCapacityForRonglu(item.Key, item.Value) < item.Value)
			{
				num2--;
				if (num2 < 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool ProductionCanGet()
	{
		int i = 0;
		for (int count = ProductionList.Count; i < count; i++)
		{
			UseItem useItem = ProductionList[i];
			if (useItem != null && useItem.num > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCurOpen(long insId)
	{
		return insId == _curOpenSmelterInstanceId;
	}
}
