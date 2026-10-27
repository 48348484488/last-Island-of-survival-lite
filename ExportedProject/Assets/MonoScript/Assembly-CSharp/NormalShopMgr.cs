using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SC.UI;
using UnityEngine;
using cfg;
using gs.shop.scmsg;

public class NormalShopMgr : Singleton<NormalShopMgr>
{
	private float _receiveTime;

	private DateTime _nowTime;

	private SShopInfo _sShopInfoCache;

	private readonly CBuyShop _cBuyShop = new CBuyShop();

	private readonly Dictionary<int, List<ShopCfg>> _dicTabType2Cfg = new Dictionary<int, List<ShopCfg>>();

	private DateTime NowTime
	{
		get
		{
			return _nowTime.AddSeconds(Time.realtimeSinceStartup - _receiveTime);
		}
		set
		{
			_nowTime = value;
		}
	}

	public void Init()
	{
		_nowTime = DateTime.UtcNow.AddHours(8.0);
		SShopShangJia.handler = (SShopShangJia.Handler)Delegate.Combine(SShopShangJia.handler, new SShopShangJia.Handler(OnSShopShangJia));
		SShopXiaJia.handler = (SShopXiaJia.Handler)Delegate.Combine(SShopXiaJia.handler, new SShopXiaJia.Handler(OnSShopXiaJia));
		SShopInfo.handler = (SShopInfo.Handler)Delegate.Combine(SShopInfo.handler, new SShopInfo.Handler(OnSShopInfo));
		SServerRefreshDailyBuyInfo.handler = (SServerRefreshDailyBuyInfo.Handler)Delegate.Combine(SServerRefreshDailyBuyInfo.handler, new SServerRefreshDailyBuyInfo.Handler(OnSServerRefreshDailyBuyInfo));
		RechargeEvent.OnReceiveServerTimeAction = (Action<int>)Delegate.Combine(RechargeEvent.OnReceiveServerTimeAction, new Action<int>(SetNowTime));
		SBuyShop.handler = (SBuyShop.Handler)Delegate.Combine(SBuyShop.handler, new SBuyShop.Handler(OnSBuyShop));
	}

	private void OnSBuyShop(SBuyShop msg)
	{
		if (_sShopInfoCache == null)
		{
			Debug.LogError("No ShopInfo cache.");
			return;
		}
		if (_sShopInfoCache.daylyBuyInfo.ContainsKey(msg.shopId))
		{
			_sShopInfoCache.daylyBuyInfo[msg.shopId] += msg.shopNum;
		}
		else
		{
			_sShopInfoCache.daylyBuyInfo[msg.shopId] = msg.shopNum;
		}
		if (ShopEvent.NormalShopShangXiaJiaAction != null)
		{
			ShopEvent.NormalShopShangXiaJiaAction();
		}
	}

	public void SetNowTime(int second)
	{
		_receiveTime = Time.realtimeSinceStartup;
		_nowTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(second).AddHours(8.0);
	}

	private void OnSServerRefreshDailyBuyInfo(SServerRefreshDailyBuyInfo msg)
	{
		if (_sShopInfoCache == null)
		{
			Debug.LogError("No ShopInfo cache.");
		}
		else
		{
			_sShopInfoCache.daylyBuyInfo.Clear();
		}
	}

	private void OnSShopInfo(SShopInfo msg)
	{
		_sShopInfoCache = msg;
		InitShopList();
	}

	private void OnSShopXiaJia(SShopXiaJia msg)
	{
		if (_sShopInfoCache == null)
		{
			Debug.LogError("No ShopInfo cache.");
			return;
		}
		_sShopInfoCache.shangJiaShopIds.Remove(msg.shopId);
		InitShopList();
		if (ShopEvent.NormalShopShangXiaJiaAction != null)
		{
			ShopEvent.NormalShopShangXiaJiaAction();
		}
	}

	private void OnSShopShangJia(SShopShangJia msg)
	{
		if (_sShopInfoCache == null)
		{
			Debug.LogError("No ShopInfo cache.");
			return;
		}
		_sShopInfoCache.shangJiaShopIds.Add(msg.shopId);
		InitShopList();
		if (ShopEvent.NormalShopShangXiaJiaAction != null)
		{
			ShopEvent.NormalShopShangXiaJiaAction();
		}
	}

	public void SendBuyShopMsg(int shopCfgId, int num)
	{
		_cBuyShop.shopId = shopCfgId;
		_cBuyShop.shopNum = num;
		Client2Gs.Ins.Send(_cBuyShop);
	}

	public bool CheckMoneyEnough(int id, int cost)
	{
		ShopCfg shopCfg = ShopCfg.Get(id);
		if (shopCfg == null)
		{
			if (id > 0)
			{
				Debug.LogError("Can't get ShopCfg with id : " + id);
			}
			return false;
		}
		return Singleton<RoleMgr>.Ins.IsMoneyEnough(cost, shopCfg.moneyType, GoldNotEnoughCallBack, CouponNotEnoughCallBack);
	}

	public bool GetOneCost(int id, out int realCost, out int fullCost)
	{
		realCost = 0;
		fullCost = 0;
		ShopCfg shopCfg = ShopCfg.Get(id);
		if (shopCfg == null)
		{
			if (id > 0)
			{
				Debug.LogError("Can't get ShopCfg with id : " + id);
			}
			return false;
		}
		bool result = IsInDiscount(shopCfg);
		realCost = (int)Math.Round((float)shopCfg.price * (float)shopCfg.discount / 100f, 0);
		fullCost = shopCfg.price;
		return result;
	}

	private bool IsInDiscount(ShopCfg shopInfo)
	{
		DateTime nowTime = NowTime;
		if (shopInfo.isDiscountXianShi)
		{
			DateTime dateTimeByStr = GetDateTimeByStr(shopInfo.discountStartTime);
			DateTime dateTimeByStr2 = GetDateTimeByStr(shopInfo.discountEndTime);
			return nowTime > dateTimeByStr && nowTime <= dateTimeByStr2;
		}
		return shopInfo.discount < 100;
	}

	private DateTime GetDateTimeByStr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return NowTime;
		}
		Regex regex = new Regex("\\d*");
		MatchCollection matchCollection = regex.Matches(str);
		List<int> list = new List<int>();
		IEnumerator enumerator = matchCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Match match = (Match)enumerator.Current;
				if (!string.IsNullOrEmpty(match.Value))
				{
					list.Add(int.Parse(match.Value));
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		if (list.Count == 3)
		{
			return new DateTime(list[0], list[1], list[2]);
		}
		if (list.Count == 6)
		{
			if (list[3] >= 24)
			{
				list[3] = 0;
			}
			return new DateTime(list[0], list[1], list[2], list[3], list[4], list[5]);
		}
		Debug.LogError("Error Time : " + str);
		return NowTime;
	}

	public void SetCellObjs(ShopItem cell, ShopCfg shopInfo, bool isNormalShop = true)
	{
		if (isNormalShop)
		{
			ShopBuyCondition condition;
			GetDailyNumLimit(shopInfo, out condition);
			if (condition == null)
			{
				cell.m_sell_out.SetActiveBetter(false);
				cell.txt_limit.SetActiveBetter(false);
			}
			else
			{
				cell.txt_limit.SetActiveBetter(true);
				int value;
				_sShopInfoCache.daylyBuyInfo.TryGetValue(shopInfo.id, out value);
				View.SetLabelText(cell.txt_numText, Utils.GetString(9, value, condition.arg1));
				cell.m_sell_out.SetActiveBetter(value >= condition.arg1);
			}
		}
		int realCost;
		int fullCost;
		if (GetOneCost(shopInfo.id, out realCost, out fullCost))
		{
			cell.m_discount.SetActiveBetter(true);
			cell.txt_full_price.SetActiveBetter(true);
			float num = (float)shopInfo.discount / 10f;
			View.SetLabelText(cell.txt_discount, Utils.GetString(266, (!(num - (float)(int)num > 0f)) ? ((double)num) : Math.Round(num, 1)));
			View.SetLabelText(cell.txt_full_priceText, fullCost);
		}
		else
		{
			cell.m_discount.SetActiveBetter(false);
			cell.txt_full_price.SetActiveBetter(false);
		}
		ItemCfg itemCfg = ItemCfg.Get(shopInfo.id);
		View.SetLabelText(cell.txt_nameText, itemCfg.name);
		View.SetLabelText(cell.txt_real_priceText, realCost);
		View.SetItemSprite(cell.m_icon, shopInfo.icon);
		string limitStr;
		if (SellTimeLimit(shopInfo, out limitStr))
		{
			cell.txt_extra_string.SetActiveBetter(true);
			View.SetLabelText(cell.txt_extra_stringText, limitStr);
		}
		else
		{
			cell.txt_extra_string.SetActiveBetter(false);
		}
		SetMoneyIcon(cell.m_price_icon, shopInfo.moneyType);
	}

	public void GetDailyNumLimit(ShopCfg shopInfo, out ShopBuyCondition condition)
	{
		List<ShopBuyCondition> buyConditions = shopInfo.buyConditions;
		int i = 0;
		for (int count = buyConditions.Count; i < count; i++)
		{
			condition = buyConditions[i];
			if (condition.conditionType == "EveryDayNum")
			{
				return;
			}
		}
		condition = null;
	}

	public void SetMoneyIcon(GameObject imageGo, int moneyType)
	{
		string text;
		switch (moneyType)
		{
		case 1:
			text = cfg.Consts.GOLD_ICON;
			break;
		case 2:
			text = cfg.Consts.COUPON_ICON;
			break;
		default:
			Debug.LogError("Wrong MoneyType : " + moneyType);
			return;
		}
		View.SetItemSprite(imageGo, "common/" + text);
	}

	private bool SellTimeLimit(ShopCfg shopInfo, out string limitStr)
	{
		if (shopInfo.isXianShiChuShou)
		{
			DateTime dateTimeByStr = GetDateTimeByStr(shopInfo.shangJiaDate);
			DateTime dateTimeByStr2 = GetDateTimeByStr(shopInfo.xiaJiaDate);
			DateTime nowTime = NowTime;
			bool flag = nowTime > dateTimeByStr && nowTime <= dateTimeByStr2;
			if (flag)
			{
				TimeSpan timeSpan = dateTimeByStr2 - nowTime;
				int days = timeSpan.Days;
				int hours = timeSpan.Hours;
				if (days > 0)
				{
					limitStr = Utils.GetString(267, Utils.GetString(167, days));
				}
				else
				{
					limitStr = Utils.GetString(267, Utils.GetString(18, hours));
				}
			}
			else
			{
				limitStr = string.Empty;
			}
			return flag;
		}
		limitStr = string.Empty;
		return false;
	}

	private void InitShopList()
	{
		_dicTabType2Cfg.Clear();
		List<ShopCfg> allList = ShopCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			ShopCfg shopCfg = allList[i];
			if (shopCfg.isShowInNormal && ShangJiaZhong(shopCfg))
			{
				List<ShopCfg> value;
				if (!_dicTabType2Cfg.TryGetValue(shopCfg.itemSmallType, out value))
				{
					value = new List<ShopCfg>();
					_dicTabType2Cfg[shopCfg.itemSmallType] = value;
				}
				value.Add(shopCfg);
			}
		}
	}

	private bool ShangJiaZhong(ShopCfg shopInfo)
	{
		return ShangJiaZhong(shopInfo.id);
	}

	private bool ShangJiaZhong(int shopId)
	{
		return _sShopInfoCache != null && _sShopInfoCache.shangJiaShopIds.Contains(shopId);
	}

	public List<ShopCfg> GetGoodsList(int shopSmallType)
	{
		List<ShopCfg> value;
		_dicTabType2Cfg.TryGetValue(shopSmallType, out value);
		return value;
	}

	public List<ShopCfg> GetGoodsList()
	{
		List<ShopCfg> list = new List<ShopCfg>();
		List<ShopCfg> allList = ShopCfg.GetAllList();
		for (int i = 0; i < allList.Count; i++)
		{
			if (_sShopInfoCache.shangJiaShopIds.Contains(allList[i].id))
			{
				list.Add(allList[i]);
			}
		}
		return list;
	}

	public void GoldNotEnoughCallBack()
	{
	}

	public void CouponNotEnoughCallBack()
	{
		if (!ViewMgr.Ins.IsShow<ShoppingPanel>())
		{
			ViewMgr.Ins.ShowView<ShoppingPanel>(-1, false);
		}
		else if (ShopEvent.JumpToRechargePage != null)
		{
			ShopEvent.JumpToRechargePage();
		}
	}
}
