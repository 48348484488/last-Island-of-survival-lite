using System;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using cfg;
using gs.shop.scmsg;

public class TimeShopMgr : Singleton<TimeShopMgr>
{
	private SOpenRandomShop _sOpenRandomShopCache;

	private readonly Dictionary<int, RandomShopRefreshInfo> _dicId2LimitInfo = new Dictionary<int, RandomShopRefreshInfo>();

	private readonly COpenRandomShop _cOpenRandomShop = new COpenRandomShop();

	private CBuyRandomShop _cBuyRandomShop = new CBuyRandomShop();

	private CRefreshRandomShop _cRefreshRandomShop = new CRefreshRandomShop();

	public void Init()
	{
		SOpenRandomShop.handler = (SOpenRandomShop.Handler)Delegate.Combine(SOpenRandomShop.handler, new SOpenRandomShop.Handler(OnSOpenRandomShop));
		SBuyRandomShop.handler = (SBuyRandomShop.Handler)Delegate.Combine(SBuyRandomShop.handler, new SBuyRandomShop.Handler(OnSBuyRandomShop));
		SRefreshRandomShop.handler = (SRefreshRandomShop.Handler)Delegate.Combine(SRefreshRandomShop.handler, new SRefreshRandomShop.Handler(OnSRefreshRandomShop));
		SServerRefreshRandomShop.handler = (SServerRefreshRandomShop.Handler)Delegate.Combine(SServerRefreshRandomShop.handler, new SServerRefreshRandomShop.Handler(OnSServerRefreshRandomShop));
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(OnLogin);
	}

	private void OnSServerRefreshRandomShop(SServerRefreshRandomShop msg)
	{
		SendOpenRandomShopMsg();
	}

	private void OnLogin()
	{
		SendOpenRandomShopMsg();
		ViewMgr.Ins.RemoveOnShowEvent("TeamTaskPanel", OnLogin);
	}

	private void OnSRefreshRandomShop(SRefreshRandomShop msg)
	{
		if (_sOpenRandomShopCache == null)
		{
			Debug.LogError("No SOpenRandomShop received,but SBuyRandomShop received.");
			return;
		}
		_sOpenRandomShopCache.randomShopItemInfos = msg.randomShopItemInfos;
		_sOpenRandomShopCache.refreshNumber++;
		if (ShopEvent.TimeShopRefreshAllAction != null)
		{
			ShopEvent.TimeShopRefreshAllAction();
		}
	}

	private void OnSBuyRandomShop(SBuyRandomShop msg)
	{
		if (_sOpenRandomShopCache == null)
		{
			Debug.LogError("No SOpenRandomShop received,but SBuyRandomShop received.");
			return;
		}
		if (_sOpenRandomShopCache.randomShopItemInfos.Count < msg.index)
		{
			Debug.LogError("SOpenRandomShop.randomShopItemInfos.Count is less than SBuyRandomShop.index.");
			return;
		}
		_sOpenRandomShopCache.randomShopItemInfos[msg.index].number -= msg.number;
		if (ShopEvent.TimeShopRefreshIndexAction != null)
		{
			ShopEvent.TimeShopRefreshIndexAction(msg.index);
		}
	}

	private void OnSOpenRandomShop(SOpenRandomShop msg)
	{
		_sOpenRandomShopCache = msg;
		msg.timeToRefresh += (int)Time.realtimeSinceStartup + 1;
		_dicId2LimitInfo.Clear();
		List<RandomShopRefreshCfg> allList = RandomShopRefreshCfg.GetAllList();
		List<RandomShopItemInfo> randomShopItemInfos = msg.randomShopItemInfos;
		int i = 0;
		for (int count = randomShopItemInfos.Count; i < count; i++)
		{
			RandomShopRefreshCfg randomShopRefreshCfg = allList[i];
			List<RandomShopRefreshInfo> refreshInfos = randomShopRefreshCfg.refreshInfos;
			for (int j = 0; j < refreshInfos.Count; j++)
			{
				RandomShopRefreshInfo randomShopRefreshInfo = refreshInfos[j];
				if (randomShopRefreshInfo.shopId == randomShopItemInfos[i].shopId)
				{
					_dicId2LimitInfo.Add(i + 1, randomShopRefreshInfo);
					break;
				}
			}
		}
		if (ShopEvent.TimeShopRefreshAllAction != null)
		{
			ShopEvent.TimeShopRefreshAllAction();
		}
	}

	private void SendOpenRandomShopMsg()
	{
		Client2Gs.Ins.Send(_cOpenRandomShop);
	}

	public void SendBuyRandomShopMsg(int index, int num)
	{
		_cBuyRandomShop.index = index;
		_cBuyRandomShop.number = num;
		Client2Gs.Ins.Send(_cBuyRandomShop);
	}

	public void SendRefreshRandomShopMsg()
	{
		Client2Gs.Ins.Send(_cRefreshRandomShop);
	}

	public bool CheckMoneyEnough(int index, int cost)
	{
		if (_sOpenRandomShopCache == null || _sOpenRandomShopCache.randomShopItemInfos.Count <= index)
		{
			return false;
		}
		return Singleton<NormalShopMgr>.Ins.CheckMoneyEnough(_sOpenRandomShopCache.randomShopItemInfos[index].shopId, cost);
	}

	public bool GetOneCost(int index, out int realCost, out int fullCost)
	{
		realCost = 0;
		fullCost = 0;
		if (_sOpenRandomShopCache == null || _sOpenRandomShopCache.randomShopItemInfos.Count <= index)
		{
			return false;
		}
		return Singleton<NormalShopMgr>.Ins.GetOneCost(_sOpenRandomShopCache.randomShopItemInfos[index].shopId, out realCost, out fullCost);
	}

	public int GetIndexLimitNum(int index)
	{
		if (_sOpenRandomShopCache == null || _sOpenRandomShopCache.randomShopItemInfos.Count <= index)
		{
			return 0;
		}
		RandomShopItemInfo randomShopItemInfo = _sOpenRandomShopCache.randomShopItemInfos[index];
		return randomShopItemInfo.number;
	}

	public List<RandomShopItemInfo> GetItemList()
	{
		if (_sOpenRandomShopCache == null)
		{
			return null;
		}
		return _sOpenRandomShopCache.randomShopItemInfos;
	}

	public void SetCellObjs(ShopItem cell, ShopCfg shopInfo, int index)
	{
		Singleton<NormalShopMgr>.Ins.SetCellObjs(cell, shopInfo, false);
		List<RandomShopItemInfo> randomShopItemInfos = _sOpenRandomShopCache.randomShopItemInfos;
		if (randomShopItemInfos.Count <= index)
		{
			Debug.LogError("[TimeShopMgr.cs]SetCellObjs:Index is out of range.");
			return;
		}
		RandomShopItemInfo randomShopItemInfo = randomShopItemInfos[index];
		RandomShopRefreshInfo value;
		if (!_dicId2LimitInfo.TryGetValue(index + 1, out value))
		{
			Debug.LogError("index not one less than cfgId.");
			return;
		}
		cell.m_sell_out.SetActiveBetter(randomShopItemInfo.number < 1);
		View.SetLabelText(cell.txt_numText, Utils.GetString(9, value.number - randomShopItemInfo.number, value.number));
	}

	public void GetFirstCanBuy(out int index, out ShopCfg shopCfg)
	{
		index = 0;
		shopCfg = null;
		if (_sOpenRandomShopCache == null)
		{
			return;
		}
		List<RandomShopItemInfo> randomShopItemInfos = _sOpenRandomShopCache.randomShopItemInfos;
		int i = 0;
		for (int count = randomShopItemInfos.Count; i < count; i++)
		{
			RandomShopRefreshInfo value;
			if (!_dicId2LimitInfo.TryGetValue(i + 1, out value))
			{
				return;
			}
			if (randomShopItemInfos[i].number > 0)
			{
				index = i;
				shopCfg = ShopCfg.Get(value.shopId);
				return;
			}
		}
		if (shopCfg == null && randomShopItemInfos.Count > 0)
		{
			shopCfg = ShopCfg.Get(randomShopItemInfos[0].shopId);
		}
	}

	public int GetRefreshIndex()
	{
		if (_sOpenRandomShopCache != null)
		{
			return _sOpenRandomShopCache.refreshNumber;
		}
		return 0;
	}

	public string GetNextTimeRefresh()
	{
		return (_sOpenRandomShopCache != null) ? Utils.GetString(271, Utils.GetCountDownTime(_sOpenRandomShopCache.timeToRefresh - (int)Time.realtimeSinceStartup)) : string.Empty;
	}

	public ShopCfg GetShopCfgByIndex(int index)
	{
		if (_sOpenRandomShopCache == null || _sOpenRandomShopCache.randomShopItemInfos.Count <= index)
		{
			return null;
		}
		return ShopCfg.Get(_sOpenRandomShopCache.randomShopItemInfos[index].shopId);
	}
}
