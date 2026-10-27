using System;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using cfg;
using gs.workbench.scmsg;

public class WorkbenchPanelMgr : Singleton<WorkbenchPanelMgr>
{
	private bool _isOpenDrag;

	public long CurOpenWorkbench;

	private Dictionary<long, SWorkbenchInfo> _dicInstanceId2WorkbenchInfos = new Dictionary<long, SWorkbenchInfo>();

	private CWorkbenchInfo _cWorkbenchInfo = new CWorkbenchInfo();

	private CWorkbenchLevelUp _cUpgradeWorkbench = new CWorkbenchLevelUp();

	private CRepair _cRepair = new CRepair();

	private CDevelopment _cDevelopment = new CDevelopment();

	private CGetItem _cGetItem = new CGetItem();

	private CCancelDevelopment _cCancelDevelopment = new CCancelDevelopment();

	private CCancelWorkbenchLevelUp _cCancelWorkbenchLevelUp = new CCancelWorkbenchLevelUp();

	public bool IsOpenDrag
	{
		get
		{
			return _isOpenDrag;
		}
	}

	public int WorkbenchLevel
	{
		get
		{
			SWorkbenchInfo value;
			if (_dicInstanceId2WorkbenchInfos.TryGetValue(CurOpenWorkbench, out value))
			{
				return value.workbenchLevel;
			}
			return 0;
		}
	}

	public void Init()
	{
		SWorkbenchError.handler = (SWorkbenchError.Handler)Delegate.Combine(SWorkbenchError.handler, new SWorkbenchError.Handler(OnSWorkbenchError));
		SWorkbenchInfo.handler = (SWorkbenchInfo.Handler)Delegate.Combine(SWorkbenchInfo.handler, new SWorkbenchInfo.Handler(OnSWorkbenchInfo));
		SCancelDevelopment.handler = (SCancelDevelopment.Handler)Delegate.Combine(SCancelDevelopment.handler, new SCancelDevelopment.Handler(OnSCancelDevelopment));
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OpenWorkbenchPanel));
		SGetItem.handler = (SGetItem.Handler)Delegate.Combine(SGetItem.handler, new SGetItem.Handler(OnSGetItem));
	}

	private void OnSGetItem(SGetItem msg)
	{
		try
		{
			_dicInstanceId2WorkbenchInfos[CurOpenWorkbench].developments.Remove(msg.developmentId);
		}
		catch
		{
		}
	}

	private void OnSCancelDevelopment(SCancelDevelopment msg)
	{
		SWorkbenchInfo value;
		if (_dicInstanceId2WorkbenchInfos.TryGetValue(CurOpenWorkbench, out value))
		{
			value.developments.Remove(msg.developmentId);
			if (WorkbenchEvent.RefreshPageDelegate != null && CurOpenWorkbench == value.workbenchId)
			{
				WorkbenchEvent.RefreshPageDelegate(value);
			}
		}
	}

	public void Test()
	{
	}

	public void OpenWorkbenchPanel(long instanceId)
	{
	}

	public void OpenWorkbenchPanel(int itemId, long workbenchInstanceId)
	{
		if (ItemCfg.Get(itemId).childType == 97)
		{
			CurOpenWorkbench = workbenchInstanceId;
			Singleton<WorkbenchPanelMgr>.Ins.SendRequireWorkbenchInfoMsg();
		}
	}

	private void OnSWorkbenchInfo(SWorkbenchInfo msg)
	{
		int num = (int)Time.realtimeSinceStartup;
		msg.finishTime += num;
		foreach (Development value in msg.developments.Values)
		{
			if (!value.itemIsGet)
			{
				value.finishTime += num;
			}
		}
		if (_dicInstanceId2WorkbenchInfos.Count > 10)
		{
			_dicInstanceId2WorkbenchInfos.Clear();
		}
		_dicInstanceId2WorkbenchInfos[msg.workbenchId] = msg;
		if (WorkbenchEvent.RefreshPageDelegate != null && CurOpenWorkbench == msg.workbenchId)
		{
			WorkbenchEvent.RefreshPageDelegate(msg);
		}
		if (!ViewMgr.Ins.IsShow<WorkbenchPanel>())
		{
			ViewMgr.Ins.ShowView<WorkbenchPanel>(null, false);
		}
	}

	private void OnSWorkbenchError(SWorkbenchError msg)
	{
		int num = 0;
		switch (msg.code)
		{
		case 1:
			num = 85;
			break;
		case 2:
			num = 86;
			break;
		case 3:
			num = 87;
			break;
		case 4:
			num = 88;
			break;
		case 5:
			num = 89;
			break;
		case 6:
			num = 90;
			break;
		case 7:
			num = 91;
			break;
		case 8:
			num = 92;
			break;
		case 9:
			num = 93;
			break;
		case 10:
			num = 94;
			break;
		case 11:
			num = 95;
			break;
		case 12:
			num = 96;
			break;
		case 13:
			num = 97;
			break;
		}
		if (num > 0)
		{
			AlertBox.Show(num);
		}
	}

	public bool GetDevelopInfo(long instanceId, int level, out Development developInfo)
	{
		developInfo = null;
		SWorkbenchInfo value;
		if (_dicInstanceId2WorkbenchInfos.TryGetValue(instanceId, out value) && value.developments.TryGetValue(level, out developInfo))
		{
			return true;
		}
		return false;
	}

	public bool IsCurOpenUpgrading()
	{
		SWorkbenchInfo value;
		if (_dicInstanceId2WorkbenchInfos.TryGetValue(CurOpenWorkbench, out value))
		{
			return (float)value.finishTime > Time.realtimeSinceStartup;
		}
		return false;
	}

	public bool IsLevelToUpgrade(int level, out bool isPrimary2High)
	{
		isPrimary2High = false;
		SWorkbenchInfo value;
		if (_dicInstanceId2WorkbenchInfos.TryGetValue(CurOpenWorkbench, out value))
		{
			if (level == 3 && value.workbenchLevel == 1)
			{
				AlertBox.Show(39);
				isPrimary2High = true;
				return false;
			}
			return value.workbenchLevel < level;
		}
		return true;
	}

	public bool HighBtnShow()
	{
		SWorkbenchInfo value;
		if (_dicInstanceId2WorkbenchInfos.TryGetValue(CurOpenWorkbench, out value))
		{
			return value.workbenchLevel >= 2;
		}
		return false;
	}

	public string GetWorkbenchIcon(int level)
	{
		switch (level)
		{
		case 1:
			return "texture/" + cfg.Consts.WORKBENCH_LEVEL_ONE_ICON;
		case 2:
			return "texture/" + cfg.Consts.WORKBENCH_LEVEL_TWO_ICON;
		case 3:
			return "texture/" + cfg.Consts.WORKBENCH_LEVEL_THREE_ICON;
		default:
			return string.Empty;
		}
	}

	public void SendRequireWorkbenchInfoMsg()
	{
		_cWorkbenchInfo.workbenchId = CurOpenWorkbench;
		Client2Gs.Ins.Send(_cWorkbenchInfo);
	}

	public void SendUpgradeWorkbenchMsg()
	{
		_cUpgradeWorkbench.workbenchId = CurOpenWorkbench;
		Client2Gs.Ins.Send(_cUpgradeWorkbench);
	}

	public void SendRepairItemMsg(int repairItemInstanceId)
	{
		_cRepair.workbenchId = CurOpenWorkbench;
		_cRepair.repairId = repairItemInstanceId;
		Client2Gs.Ins.Send(_cRepair);
	}

	public void SendDevelopMsg(int developId, bool isUseAssist)
	{
		_cDevelopment.workbenchId = CurOpenWorkbench;
		_cDevelopment.developmentId = developId;
		_cDevelopment.isUsedAssist = isUseAssist;
		Client2Gs.Ins.Send(_cDevelopment);
	}

	public void SendGetItemMsg(int workbenchLevel)
	{
		_cGetItem.workbenchId = CurOpenWorkbench;
		_cGetItem.workbenchLevel = workbenchLevel;
		Client2Gs.Ins.Send(_cGetItem);
	}

	public void SendCancelDevelop(int level)
	{
		if (level >= 1 && level <= 3)
		{
			_cCancelDevelopment.workbenchId = CurOpenWorkbench;
			_cCancelDevelopment.developmentId = level;
			Client2Gs.Ins.Send(_cCancelDevelopment);
		}
	}

	public void SendCancelUpgradeMsg()
	{
		_cCancelWorkbenchLevelUp.workbenchId = CurOpenWorkbench;
		Client2Gs.Ins.Send(_cCancelWorkbenchLevelUp);
	}
}
