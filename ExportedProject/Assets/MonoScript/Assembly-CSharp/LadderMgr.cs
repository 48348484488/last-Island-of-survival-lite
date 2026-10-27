using System;
using SC.UI;
using UnityEngine;
using cfg;
using gs.drop.scmsg;
using gs.ladder.scmsg;

public class LadderMgr : Singleton<LadderMgr>
{
	private LadderInfo _ladderInfo;

	private float _runTime;

	private CBuyLadderTask _cBuyLadderTask = new CBuyLadderTask();

	private CBuyLadderLevel _cBuyLadderLevel = new CBuyLadderLevel();

	private CGetLadderReward _cGetLadderReward = new CGetLadderReward();

	private CGetLadderBox _cGetLadderBox = new CGetLadderBox();

	public LadderInfo Info
	{
		get
		{
			return _ladderInfo;
		}
	}

	public int FinishTime
	{
		get
		{
			float num = Time.realtimeSinceStartup - _runTime;
			return _ladderInfo.timeToRefreshWeekTask - (int)num;
		}
	}

	public void Init()
	{
		SLadderInfo.handler = (SLadderInfo.Handler)Delegate.Combine(SLadderInfo.handler, new SLadderInfo.Handler(SLadderInfoHandle));
		SBuyLadderTask.handler = (SBuyLadderTask.Handler)Delegate.Combine(SBuyLadderTask.handler, new SBuyLadderTask.Handler(SBuyLadderTaskHandle));
		SBuyLadderLevel.handler = (SBuyLadderLevel.Handler)Delegate.Combine(SBuyLadderLevel.handler, new SBuyLadderLevel.Handler(SBuyLadderLevelHandle));
		SLadderExpChange.handler = (SLadderExpChange.Handler)Delegate.Combine(SLadderExpChange.handler, new SLadderExpChange.Handler(SLadderExpChangeHandle));
		SGetLadderReward.handler = (SGetLadderReward.Handler)Delegate.Combine(SGetLadderReward.handler, new SGetLadderReward.Handler(SGetLadderRewardHandle));
		SGetLadderBox.handler = (SGetLadderBox.Handler)Delegate.Combine(SGetLadderBox.handler, new SGetLadderBox.Handler(SGetLadderBoxHandle));
		SFinishTaskNumberChanged.handler = (SFinishTaskNumberChanged.Handler)Delegate.Combine(SFinishTaskNumberChanged.handler, new SFinishTaskNumberChanged.Handler(SFinishTaskNumberChangedHandle));
	}

	private void SFinishTaskNumberChangedHandle(SFinishTaskNumberChanged msg)
	{
		_ladderInfo.weekFinishTaskCount = msg.number;
		Utils.TriggerEvent(LadderEvent.FinishTaskNumberChanged);
		Utils.TriggerEvent(TaskEvent.RefreshTaskRed);
	}

	private void SGetLadderBoxHandle(SGetLadderBox msg)
	{
		_ladderInfo.isGetBox = true;
		DropDetail dropDetail = new DropDetail();
		dropDetail.dropItem.Add(31101, 1);
		dropDetail.dropItem.Add(31102, 1);
		ViewMgr.Ins.ShowView<GainPanel>(dropDetail, false);
		Utils.TriggerEvent(LadderEvent.RefreshLadderRed);
		Utils.TriggerEvent(LadderEvent.RefreshBox);
	}

	private void SGetLadderRewardHandle(SGetLadderReward msg)
	{
		if (msg.taskType == 0)
		{
			if (!_ladderInfo.rewardedNormalTaskIds.Contains(msg.level))
			{
				_ladderInfo.rewardedNormalTaskIds.Add(msg.level);
			}
		}
		else if ((msg.taskType == 1 || msg.taskType == 2) && !_ladderInfo.rewardedBuyedTaskIds.Contains(msg.level))
		{
			_ladderInfo.rewardedBuyedTaskIds.Add(msg.level);
		}
		ViewMgr.Ins.ShowTopView<GainPanel>(msg.dropDetail);
		Utils.TriggerEvent(LadderEvent.RefreshLadder);
		Utils.TriggerEvent(LadderEvent.RefreshLadderRed);
	}

	private void SLadderExpChangeHandle(SLadderExpChange msg)
	{
		int level = _ladderInfo.level;
		_ladderInfo.level = msg.level;
		_ladderInfo.exp = msg.exp;
		if (level < msg.level)
		{
			ViewMgr.Ins.AddView<LadderLevelUpPanel>(null, false);
		}
		Utils.TriggerEvent(LadderEvent.RefreshLevel);
		Utils.TriggerEvent(LadderEvent.RefreshLadderRed);
	}

	private void SBuyLadderLevelHandle(SBuyLadderLevel msg)
	{
		AlertBox.Show(142);
	}

	private void SBuyLadderTaskHandle(SBuyLadderTask msg)
	{
		_ladderInfo.taskType = msg.type;
		if (msg.dropDetail.dropProp.Count > 0 || msg.dropDetail.dropItem.Count > 0)
		{
			ViewMgr.Ins.ShowView<GainPanel>(msg.dropDetail);
		}
		Utils.TriggerEvent(LadderEvent.BuyTaskTypeSucess);
		AlertBox.Show(165);
	}

	private void SLadderInfoHandle(SLadderInfo msg)
	{
		_ladderInfo = msg.ladderInfo;
		foreach (int rewardedBuyedTaskId in _ladderInfo.rewardedBuyedTaskIds)
		{
			if (!_ladderInfo.rewardedNormalTaskIds.Contains(rewardedBuyedTaskId))
			{
				_ladderInfo.rewardedNormalTaskIds.Add(rewardedBuyedTaskId);
			}
		}
		_runTime = Time.realtimeSinceStartup;
	}

	public void BuyLadderTask(int type)
	{
		_cBuyLadderTask.type = type;
		Client2Gs.Ins.Send(_cBuyLadderTask);
	}

	public void BuyLadderLevel(int level)
	{
		_cBuyLadderLevel.level = level;
		Client2Gs.Ins.Send(_cBuyLadderLevel);
	}

	public bool GetLadderReward(int taskType, int level, int itemid, int num, int dropType)
	{
		if (dropType > 2 && !Singleton<BagMgr>.Ins.IsHasCapacity(itemid, num))
		{
			AlertBox.Show(26);
			return false;
		}
		_cGetLadderReward.taskType = taskType;
		_cGetLadderReward.level = level;
		Client2Gs.Ins.Send(_cGetLadderReward);
		return true;
	}

	public void GetLadderBox()
	{
		Client2Gs.Ins.Send(_cGetLadderBox);
	}

	public int WeekFinishNum()
	{
		return _ladderInfo.weekFinishTaskCount;
	}

	public bool IsShowRedDot()
	{
		for (int i = 1; i <= Singleton<RoleMgr>.Ins.info.level; i++)
		{
			if (IsShowRedDot(i))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsShowRedDot(int level)
	{
		if (level > Singleton<RoleMgr>.Ins.info.level)
		{
			return false;
		}
		if (_ladderInfo.taskType == 0)
		{
			return !_ladderInfo.rewardedNormalTaskIds.Contains(level);
		}
		if (_ladderInfo.taskType == 1 || _ladderInfo.taskType == 2)
		{
			return !_ladderInfo.rewardedBuyedTaskIds.Contains(level);
		}
		return false;
	}

	public bool IsShowBoxRedDot()
	{
		return _ladderInfo.weekFinishTaskCount > cfg.Consts.TASK_BOX_MAX_COUNT && !_ladderInfo.isGetBox;
	}
}
