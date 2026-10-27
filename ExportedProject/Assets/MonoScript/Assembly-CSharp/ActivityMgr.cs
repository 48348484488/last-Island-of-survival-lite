using System;
using UnityEngine;
using cfg;
using gs.activity.scmsg;
using gs.online.scmsg;
using gs.shop.scmsg;

public class ActivityMgr : Singleton<ActivityMgr>
{
	private SWeekSignInfo _selfWeekSignInfo;

	private SMonthSignInfo _selfMonthSignInfo;

	private SDailyBuyGiftInfo _selfSDailyBuyGiftInfo;

	private SChargeInfo _selfChargeInfo;

	private float _receiveTime;

	private DateTime _nowTime;

	private bool _isSeeDailyGift;

	private bool _isSeeFirstCharge;

	private int _dailyGiftCount;

	private readonly CGetWeekSignReward _cGetWeekSignReward = new CGetWeekSignReward();

	private readonly CSigninMonth _cSigninMonth = new CSigninMonth();

	private readonly CBuyDailyGift _cBuyDailyGift = new CBuyDailyGift();

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

	public int AlreadySignInDayCount
	{
		get
		{
			return _selfMonthSignInfo.signedDays;
		}
	}

	public int CurDayId
	{
		get
		{
			return NowTime.Day;
		}
	}

	public void Init()
	{
		RechargeEvent.OnReceiveServerTimeAction = (Action<int>)Delegate.Combine(RechargeEvent.OnReceiveServerTimeAction, new Action<int>(OnReceiveServerTime));
		SWeekSignInfo.handler = (SWeekSignInfo.Handler)Delegate.Combine(SWeekSignInfo.handler, new SWeekSignInfo.Handler(OnSWeekSignInfo));
		SGetWeekSignReward.handler = (SGetWeekSignReward.Handler)Delegate.Combine(SGetWeekSignReward.handler, new SGetWeekSignReward.Handler(OnSGetWeekSignReward));
		SMonthSignInfo.handler = (SMonthSignInfo.Handler)Delegate.Combine(SMonthSignInfo.handler, new SMonthSignInfo.Handler(OnSMonthSignInfo));
		SSigninMonth.handler = (SSigninMonth.Handler)Delegate.Combine(SSigninMonth.handler, new SSigninMonth.Handler(OnSSigninMonth));
		SDailyBuyGiftInfo.handler = (SDailyBuyGiftInfo.Handler)Delegate.Combine(SDailyBuyGiftInfo.handler, new SDailyBuyGiftInfo.Handler(OnSDailyBuyGiftInfo));
		SBuyDailyGift.handler = (SBuyDailyGift.Handler)Delegate.Combine(SBuyDailyGift.handler, new SBuyDailyGift.Handler(OnSBuyDailyGift));
		SGetFirstChargeGift.handler = (SGetFirstChargeGift.Handler)Delegate.Combine(SGetFirstChargeGift.handler, new SGetFirstChargeGift.Handler(OnSGetFirstChargeGift));
		SChargeInfo.handler = (SChargeInfo.Handler)Delegate.Combine(SChargeInfo.handler, new SChargeInfo.Handler(OnSChargeInfo));
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnCfgLoadFinish));
	}

	private void OnCfgLoadFinish(SLoginFinished msg)
	{
		_dailyGiftCount = DaylyGiftCfg.GetAllList().Count;
	}

	private void OnSChargeInfo(SChargeInfo msg)
	{
		_selfChargeInfo = msg;
	}

	private void OnSGetFirstChargeGift(SGetFirstChargeGift msg)
	{
		Singleton<GainMgr>.Ins.Add(msg.dropDetail);
		if (ActivityEvent.UpdateActivityRedDot != null)
		{
			ActivityEvent.UpdateActivityRedDot();
		}
	}

	private void OnReceiveServerTime(int iTimeInSecond)
	{
		_receiveTime = Time.realtimeSinceStartup;
		_nowTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(iTimeInSecond);
	}

	private void OnSBuyDailyGift(SBuyDailyGift msg)
	{
		_selfSDailyBuyGiftInfo.buyedGiftIds.Add(msg.index);
		if (ActivityEvent.OnBuyDailyGiftAction != null)
		{
			ActivityEvent.OnBuyDailyGiftAction(msg.dropDetail);
		}
		if (ActivityEvent.UpdateActivityRedDot != null)
		{
			ActivityEvent.UpdateActivityRedDot();
		}
	}

	private void OnSDailyBuyGiftInfo(SDailyBuyGiftInfo msg)
	{
		_selfSDailyBuyGiftInfo = msg;
	}

	private void OnSSigninMonth(SSigninMonth msg)
	{
		_selfMonthSignInfo.signedDays++;
		_selfMonthSignInfo.canSignIn = false;
		if (ActivityEvent.OnMonthSignInChangeAction != null)
		{
			ActivityEvent.OnMonthSignInChangeAction(msg.dropDetail);
		}
		if (ActivityEvent.UpdateActivityRedDot != null)
		{
			ActivityEvent.UpdateActivityRedDot();
		}
	}

	private void OnSMonthSignInfo(SMonthSignInfo msg)
	{
		_selfMonthSignInfo = msg;
	}

	private void OnSGetWeekSignReward(SGetWeekSignReward msg)
	{
		_selfWeekSignInfo.rewardedIndexes.Add(msg.day);
		if (ActivityEvent.UpdateSevenDayBtnAction != null)
		{
			ActivityEvent.UpdateSevenDayBtnAction(msg.dropDetail);
		}
		if (ActivityEvent.UpdateActivityRedDot != null)
		{
			ActivityEvent.UpdateActivityRedDot();
		}
	}

	private void OnSWeekSignInfo(SWeekSignInfo msg)
	{
		_selfWeekSignInfo = msg;
	}

	public bool IsAlreadyGotSevenDay(int cfgId)
	{
		return _selfWeekSignInfo.rewardedIndexes.Contains(cfgId);
	}

	public bool IsCanGetSevenDay(int cfgId)
	{
		return _selfWeekSignInfo.signinDays >= cfgId;
	}

	public void SendGetSevenDayMsg(int cfgId)
	{
		_cGetWeekSignReward.day = cfgId;
		Client2Gs.Ins.Send(_cGetWeekSignReward);
	}

	public bool IsShowSevenDayRedDot()
	{
		return _selfWeekSignInfo.signinDays > _selfWeekSignInfo.rewardedIndexes.Count;
	}

	public bool IsAlreadyGotMonthSignIn(int cfgId)
	{
		return _selfMonthSignInfo.signedDays >= cfgId;
	}

	public bool IsCanSignIn(int cfgId)
	{
		return _selfMonthSignInfo.canSignIn && _selfMonthSignInfo.signedDays + 1 == cfgId;
	}

	public void SendMonthSignInMsg()
	{
		Client2Gs.Ins.Send(_cSigninMonth);
	}

	public bool IsShowMonthSignInRedDot()
	{
		return _selfMonthSignInfo != null && _selfMonthSignInfo.canSignIn;
	}

	public bool IsAlreadyGotDailyGift(int cfgId)
	{
		return _selfSDailyBuyGiftInfo.buyedGiftIds.Contains(cfgId);
	}

	public void SendBuyDailyGiftMsg(int cfgId)
	{
		_cBuyDailyGift.index = cfgId;
		Client2Gs.Ins.Send(_cBuyDailyGift);
	}

	public bool IsShowDailyGiftRedDot()
	{
		return !_isSeeDailyGift && _selfSDailyBuyGiftInfo.buyedGiftIds.Count < _dailyGiftCount;
	}

	public void SetSeeDailyGiftTrue()
	{
		_isSeeDailyGift = true;
	}

	public bool IsAlreadyGotFirstCharge()
	{
		return _selfChargeInfo != null && _selfChargeInfo.usedFirstIds.Count > 0;
	}

	public bool IsShowFirstChargeRedDot()
	{
		return !_isSeeFirstCharge && !IsAlreadyGotFirstCharge();
	}

	public void SetSeeFirstChargeTrue()
	{
		_isSeeFirstCharge = true;
	}

	public bool ShowActivityRedDot()
	{
		return IsShowMonthSignInRedDot() || IsShowSevenDayRedDot();
	}
}
