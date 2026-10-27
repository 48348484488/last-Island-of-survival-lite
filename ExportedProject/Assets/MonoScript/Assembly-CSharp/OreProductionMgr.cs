using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.smelter.scmsg;

public class OreProductionMgr
{
	public class Ore
	{
		public int OnceSettleTime;

		public UseItem OreItem;

		public int OreType;

		public int OnceOutput;

		public int ProductionId;

		public int NextTimeSettle;

		public Production _target;

		public int Index;

		public void ResetOre(UseItem item, int index)
		{
			if (item != null)
			{
				OreItem = item;
				ItemCfg itemCfg = ItemCfg.Get(item.id);
				OreType = itemCfg.type;
				SmelterCfg smelterCfg = SmelterCfg.Get(item.id);
				ProductionId = smelterCfg.targetItemId;
				OnceSettleTime = smelterCfg.needTime;
				OnceOutput = 1;
			}
			Index = index;
		}

		public void Settle(Dictionary<int, List<Ore>> nextTimeToSettleOre, ObjectPool<List<Ore>> pool, List<Production> emptyPos, Dictionary<int, List<Production>> notFullDic, int nowTime, bool isFirstTime = false)
		{
			if (_target != null)
			{
				_target.ProduceFinish(this);
				OreItem.num--;
				if (OreItem.num <= 0)
				{
					Clear();
					return;
				}
			}
			bool flag = false;
			int type = SmelterCfg.Get(OreItem.id).type;
			List<Production> value;
			if (notFullDic.TryGetValue(type, out value))
			{
				int i = 0;
				for (int count = value.Count; i < count; i++)
				{
					if (value[i].AttachOreToThis(this))
					{
						_target = value[i];
						flag = true;
						break;
					}
				}
				if (!flag && emptyPos.Count > 0)
				{
					Production production = emptyPos[0];
					emptyPos.RemoveAt(0);
					production.ResetProduction(new UseItem
					{
						id = ProductionId,
						num = 0
					});
					value.Add(production);
					_target = production;
					production.AttachOreToThis(this);
					flag = true;
				}
			}
			else if (emptyPos.Count > 0)
			{
				Production production2 = emptyPos[0];
				emptyPos.RemoveAt(0);
				production2.ResetProduction(new UseItem
				{
					id = ProductionId,
					num = 0
				});
				value = (notFullDic[type] = new List<Production>());
				value.Add(production2);
				_target = production2;
				production2.AttachOreToThis(this);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			NextTimeSettle = nowTime + OnceSettleTime;
			if (isFirstTime)
			{
				NextTimeSettle -= OreItem.fuelTime;
				OreItem.fuelTime = 0;
			}
			if (NextTimeSettle <= nowTime)
			{
				_target.ProduceFinish(this);
				OreItem.num--;
				if (OreItem.num <= 0)
				{
					Clear();
				}
			}
			else
			{
				List<Ore> value2;
				if (!nextTimeToSettleOre.TryGetValue(NextTimeSettle, out value2))
				{
					value2 = pool.Get();
					nextTimeToSettleOre[NextTimeSettle] = value2;
				}
				value2.Add(this);
			}
		}

		public void OnStop(int stopTime)
		{
			OreItem.fuelTime = OnceSettleTime - NextTimeSettle + stopTime;
		}

		public void Clear()
		{
			OnceSettleTime = 0;
			OreItem = null;
			OreType = 0;
			OnceOutput = 0;
			ProductionId = 0;
			NextTimeSettle = 0;
			_target = null;
			Index = -1;
		}
	}

	public class Production
	{
		public UseItem ProductItem;

		private int _maxPileNum;

		private int _emptyCapacity;

		public void ResetProduction(UseItem item)
		{
			ProductItem = item;
			_emptyCapacity = (_maxPileNum = ItemCfg.Get(item.id).maxPileNum) - item.num;
		}

		public bool AttachOreToThis(Ore o)
		{
			if (o.ProductionId != ProductItem.id)
			{
				return false;
			}
			int onceOutput = o.OnceOutput;
			if (onceOutput > _emptyCapacity)
			{
				return false;
			}
			_emptyCapacity -= onceOutput;
			return true;
		}

		public void ProduceFinish(Ore o)
		{
			if (o.ProductionId == ProductItem.id)
			{
				ProductItem.num += o.OnceOutput;
			}
		}

		public bool IsFull()
		{
			return _maxPileNum <= ProductItem.num;
		}

		public void Clear()
		{
			ProductItem = null;
			_maxPileNum = 0;
			_emptyCapacity = 0;
		}
	}

	public List<Production> ProductionList;

	public List<Ore> OreList;

	public bool IsBurning;

	private ObjectPool<Production> _productionPool;

	private ObjectPool<Ore> _orePool;

	private ObjectPool<List<Ore>> _oreListPool;

	private ObjectPool<List<Production>> _productionListPool;

	private Dictionary<int, List<Ore>> _dicTime2UpdateOre = new Dictionary<int, List<Ore>>();

	private Dictionary<int, List<Production>> _dicType2NotFullProduction = new Dictionary<int, List<Production>>();

	private List<Production> _emptyProductionList = new List<Production>();

	private Dictionary<int, int> _dicProductId2OreType = new Dictionary<int, int>();

	private Dictionary<int, List<Ore>> _dicOreItemId2Ores = new Dictionary<int, List<Ore>>();

	private List<Ore> _emptyOrePos = new List<Ore>();

	private const float IntervalTime = 0.9f;

	private float _passTimeFromLastUpdate;

	private float _canSmeltTime;

	private float _startSmeltTime;

	[CompilerGenerated]
	private static ObjectPool<Production>.CreateObject<Production> _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static ObjectPool<Production>.RecycleObject<Production> _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static ObjectPool<Ore>.CreateObject<Ore> _003C_003Ef__am_0024cache2;

	[CompilerGenerated]
	private static ObjectPool<Ore>.RecycleObject<Ore> _003C_003Ef__am_0024cache3;

	[CompilerGenerated]
	private static ObjectPool<List<Ore>>.CreateObject<List<Ore>> _003C_003Ef__am_0024cache4;

	[CompilerGenerated]
	private static ObjectPool<List<Ore>>.RecycleObject<List<Ore>> _003C_003Ef__am_0024cache5;

	[CompilerGenerated]
	private static ObjectPool<List<Production>>.CreateObject<List<Production>> _003C_003Ef__am_0024cache6;

	[CompilerGenerated]
	private static ObjectPool<List<Production>>.RecycleObject<List<Production>> _003C_003Ef__am_0024cache7;

	public OreProductionMgr()
	{
		ProductionList = new List<Production>();
		OreList = new List<Ore>();
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003COreProductionMgr_003Em__0;
		}
		ObjectPool<Production>.CreateObject<Production> createFun = _003C_003Ef__am_0024cache0;
		if (_003C_003Ef__am_0024cache1 == null)
		{
			_003C_003Ef__am_0024cache1 = _003COreProductionMgr_003Em__1;
		}
		_productionPool = new ObjectPool<Production>(8, createFun, null, _003C_003Ef__am_0024cache1);
		if (_003C_003Ef__am_0024cache2 == null)
		{
			_003C_003Ef__am_0024cache2 = _003COreProductionMgr_003Em__2;
		}
		ObjectPool<Ore>.CreateObject<Ore> createFun2 = _003C_003Ef__am_0024cache2;
		if (_003C_003Ef__am_0024cache3 == null)
		{
			_003C_003Ef__am_0024cache3 = _003COreProductionMgr_003Em__3;
		}
		_orePool = new ObjectPool<Ore>(15, createFun2, null, _003C_003Ef__am_0024cache3);
		if (_003C_003Ef__am_0024cache4 == null)
		{
			_003C_003Ef__am_0024cache4 = _003COreProductionMgr_003Em__4;
		}
		ObjectPool<List<Ore>>.CreateObject<List<Ore>> createFun3 = _003C_003Ef__am_0024cache4;
		if (_003C_003Ef__am_0024cache5 == null)
		{
			_003C_003Ef__am_0024cache5 = _003COreProductionMgr_003Em__5;
		}
		_oreListPool = new ObjectPool<List<Ore>>(16, createFun3, null, _003C_003Ef__am_0024cache5);
		if (_003C_003Ef__am_0024cache6 == null)
		{
			_003C_003Ef__am_0024cache6 = _003COreProductionMgr_003Em__6;
		}
		ObjectPool<List<Production>>.CreateObject<List<Production>> createFun4 = _003C_003Ef__am_0024cache6;
		if (_003C_003Ef__am_0024cache7 == null)
		{
			_003C_003Ef__am_0024cache7 = _003COreProductionMgr_003Em__7;
		}
		_productionListPool = new ObjectPool<List<Production>>(8, createFun4, null, _003C_003Ef__am_0024cache7);
		InitProductId2OreType();
	}

	private void InitProductId2OreType()
	{
		List<SmelterCfg> allList = SmelterCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			SmelterCfg smelterCfg = allList[i];
			_dicProductId2OreType[smelterCfg.targetItemId] = smelterCfg.type;
		}
	}

	public bool GetOreToBag(int index)
	{
		if (OreList.Count > index)
		{
			_orePool.Recycle(OreList[index]);
			return true;
		}
		return false;
	}

	public bool GetProduction(int index)
	{
		if (ProductionList.Count > index)
		{
			_productionPool.Recycle(ProductionList[index]);
			return true;
		}
		return false;
	}

	public bool AddOre(BagItem oreBagItemInfo, out Dictionary<int, UseItem> dicIndex2UseItemInfo)
	{
		dicIndex2UseItemInfo = new Dictionary<int, UseItem>();
		int num = oreBagItemInfo.number;
		ItemCfg itemCfg = ItemCfg.Get(oreBagItemInfo.itemId);
		List<Ore> value;
		if (_dicOreItemId2Ores.TryGetValue(oreBagItemInfo.itemId, out value))
		{
			int i = 0;
			for (int count = value.Count; i < count; i++)
			{
				Ore ore = value[i];
				if (itemCfg.isPileAble && itemCfg.maxPileNum > ore.OreItem.num)
				{
					int num2 = itemCfg.maxPileNum - ore.OreItem.num;
					if (num2 >= num)
					{
						dicIndex2UseItemInfo[ore.Index] = new UseItem
						{
							id = itemCfg.id,
							num = num
						};
						return true;
					}
					dicIndex2UseItemInfo[ore.Index] = new UseItem
					{
						id = itemCfg.id,
						num = num2
					};
					num -= num2;
				}
			}
		}
		if (_emptyOrePos.Count > 0)
		{
			dicIndex2UseItemInfo.Add(_emptyOrePos[0].Index, new UseItem
			{
				id = oreBagItemInfo.itemId,
				num = num
			});
			_emptyOrePos.RemoveAt(0);
			return true;
		}
		return false;
	}

	public bool AddOreByInstanceId(BagItem oreBagItemInfo, out Dictionary<int, UseItem> dicIndex2UseItemInfo)
	{
		dicIndex2UseItemInfo = new Dictionary<int, UseItem>();
		if (_emptyOrePos.Count > 0)
		{
			dicIndex2UseItemInfo.Add(_emptyOrePos[0].Index, new UseItem
			{
				id = oreBagItemInfo.itemId,
				num = oreBagItemInfo.number,
				instanceId = oreBagItemInfo.instanceId
			});
			_emptyOrePos.RemoveAt(0);
			return true;
		}
		return false;
	}

	public void SmeltStopState(List<UseItem> ores, List<UseItem> products)
	{
		ProductionList.Clear();
		OreList.Clear();
		_dicOreItemId2Ores.Clear();
		_emptyOrePos.Clear();
		int i = 0;
		for (int count = products.Count; i < count; i++)
		{
			Production production = _productionPool.Get();
			if (products[i] != null)
			{
				production.ResetProduction(products[i]);
			}
			ProductionList.Add(production);
		}
		int j = 0;
		for (int count2 = ores.Count; j < count2; j++)
		{
			Ore ore = _orePool.Get();
			UseItem useItem = ores[j];
			if (useItem != null)
			{
				ore.ResetOre(useItem, j);
				List<Ore> value;
				if (!_dicOreItemId2Ores.TryGetValue(useItem.id, out value))
				{
					value = new List<Ore>();
					_dicOreItemId2Ores[useItem.id] = value;
				}
				value.Add(ore);
			}
			else
			{
				ore.ResetOre(null, j);
				_emptyOrePos.Add(ore);
			}
			OreList.Add(ore);
		}
	}

	public bool StartSmelt(List<UseItem> ores, List<UseItem> products, float fuelEndTime, bool isNotTestCanSmelt = true)
	{
		Clear();
		_canSmeltTime = fuelEndTime;
		_startSmeltTime = Time.realtimeSinceStartup;
		int i = 0;
		for (int count = products.Count; i < count; i++)
		{
			Production production = _productionPool.Get();
			if (products[i] != null)
			{
				production.ResetProduction(products[i]);
				if (!production.IsFull())
				{
					int key = _dicProductId2OreType[production.ProductItem.id];
					List<Production> value;
					if (!_dicType2NotFullProduction.TryGetValue(key, out value))
					{
						value = _productionListPool.Get();
						_dicType2NotFullProduction[key] = value;
					}
					value.Add(production);
				}
			}
			else
			{
				_emptyProductionList.Add(production);
			}
			ProductionList.Add(production);
		}
		int j = 0;
		for (int count2 = ores.Count; j < count2; j++)
		{
			Ore ore = _orePool.Get();
			UseItem useItem = ores[j];
			if (useItem != null && useItem.num > 0)
			{
				ore.ResetOre(useItem, j);
				ore.Settle(_dicTime2UpdateOre, _oreListPool, _emptyProductionList, _dicType2NotFullProduction, (int)_startSmeltTime, isNotTestCanSmelt);
			}
			else
			{
				ore.ResetOre(null, j);
				_emptyOrePos.Add(ore);
			}
			OreList.Add(ore);
		}
		return _dicTime2UpdateOre.Count > 0;
	}

	public void StopSmelt()
	{
		_canSmeltTime -= Time.realtimeSinceStartup - _startSmeltTime;
		int stopTime = (int)Time.realtimeSinceStartup;
		int i = 0;
		for (int count = OreList.Count; i < count; i++)
		{
			Ore ore = OreList[i];
			if (ore != null && ore._target != null)
			{
				ore.OnStop(stopTime);
			}
		}
	}

	public bool Update(float timeNow)
	{
		if (timeNow < _canSmeltTime + _startSmeltTime)
		{
			_passTimeFromLastUpdate += Time.deltaTime;
			if (_passTimeFromLastUpdate >= 0.9f)
			{
				_passTimeFromLastUpdate = 0f;
				int num = (int)timeNow;
				List<Ore> value;
				if (_dicTime2UpdateOre.TryGetValue(num, out value))
				{
					int i = 0;
					for (int count = value.Count; i < count; i++)
					{
						Ore ore = value[i];
						ore.Settle(_dicTime2UpdateOre, _oreListPool, _emptyProductionList, _dicType2NotFullProduction, num);
					}
					_oreListPool.Recycle(value);
					_dicTime2UpdateOre.Remove(num);
					return true;
				}
			}
		}
		return false;
	}

	public bool IsCanSmelt(int timeNow)
	{
		return _dicTime2UpdateOre.Count > 0 && (float)timeNow < _canSmeltTime + (float)(int)_startSmeltTime;
	}

	public void Clear()
	{
		_passTimeFromLastUpdate = 0f;
		int i = 0;
		for (int count = ProductionList.Count; i < count; i++)
		{
			_productionPool.Recycle(ProductionList[i]);
		}
		int j = 0;
		for (int count2 = OreList.Count; j < count2; j++)
		{
			_orePool.Recycle(OreList[j]);
		}
		foreach (List<Ore> value in _dicTime2UpdateOre.Values)
		{
			_oreListPool.Recycle(value);
		}
		foreach (List<Production> value2 in _dicType2NotFullProduction.Values)
		{
			_productionListPool.Recycle(value2);
		}
		ProductionList.Clear();
		OreList.Clear();
		_dicTime2UpdateOre.Clear();
		_dicType2NotFullProduction.Clear();
		_emptyProductionList.Clear();
		_dicOreItemId2Ores.Clear();
		_emptyOrePos.Clear();
	}

	[CompilerGenerated]
	private static Production _003COreProductionMgr_003Em__0()
	{
		return new Production();
	}

	[CompilerGenerated]
	private static void _003COreProductionMgr_003Em__1(Production pro)
	{
		pro.Clear();
	}

	[CompilerGenerated]
	private static Ore _003COreProductionMgr_003Em__2()
	{
		return new Ore();
	}

	[CompilerGenerated]
	private static void _003COreProductionMgr_003Em__3(Ore o)
	{
		o.Clear();
	}

	[CompilerGenerated]
	private static List<Ore> _003COreProductionMgr_003Em__4()
	{
		return new List<Ore>();
	}

	[CompilerGenerated]
	private static void _003COreProductionMgr_003Em__5(List<Ore> list)
	{
		list.Clear();
	}

	[CompilerGenerated]
	private static List<Production> _003COreProductionMgr_003Em__6()
	{
		return new List<Production>();
	}

	[CompilerGenerated]
	private static void _003COreProductionMgr_003Em__7(List<Production> list)
	{
		list.Clear();
	}
}
