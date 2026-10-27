using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using SC.UI;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.online.scmsg;
using gs.task.scmsg;

public class GuideMgr : Singleton<GuideMgr>
{
	public Transform UIRoot;

	public float NoviceProtectFinishTime;

	private readonly Dictionary<int, TaskInfo> _dicCfgId2TaskInfo = new Dictionary<int, TaskInfo>();

	private readonly Dictionary<long, TaskInfo> _dicInsId2TaskInfo = new Dictionary<long, TaskInfo>();

	private readonly Dictionary<int, int> _dicTaskCfgId2FirstGuideCfgId = new Dictionary<int, int>();

	private readonly Dictionary<int, List<int>> _dicMultiId2TaskCfgIds = new Dictionary<int, List<int>>();

	private readonly HashSet<int> _extraBtnShowId = new HashSet<int>();

	private int _firstNoviceTaskCfgId = -1;

	private bool _isInBuildMode;

	private GuideCfg _curStepCfgInfo;

	public int WeaponLayer;

	private bool _requiredNoviceMonster;

	private CRequestNewbeeMonster _cRequestNewbeeMonster = new CRequestNewbeeMonster();

	private long _lastAim = -1L;

	public GuideCfg CurStepCfgInfo
	{
		get
		{
			return _curStepCfgInfo;
		}
		private set
		{
			_curStepCfgInfo = value;
		}
	}

	public void Init()
	{
		if ((bool)ViewMgr.Ins.UICamera)
		{
			UIRoot = ViewMgr.Ins.UICamera.transform.parent;
		}
		if ((bool)UIRoot)
		{
			WeaponLayer = LayerMask.NameToLayer("Weapon");
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(InitData));
			SAllTasks.handler = (SAllTasks.Handler)Delegate.Combine(SAllTasks.handler, new SAllTasks.Handler(OnSAllTasks));
			BattleEvent.OnEnteredBuildState = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnteredBuildState, new Utils.VoidDelegate(OnEnterBuildMode));
			BattleEvent.OnExitedBuildState = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnExitedBuildState, new Utils.VoidDelegate(OnExitedBuildMode));
			EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimStructure));
			EventHandlers.OnAddExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnAddExtraBtn, new Utils.IntDelegate(OnAddBtn));
			SNewbeeTime.handler = (SNewbeeTime.Handler)Delegate.Combine(SNewbeeTime.handler, new SNewbeeTime.Handler(OnSNewbeeTime));
			ViewMgr.Ins.AddOnShowEvent<BattlePanel>(ShowGuide);
		}
	}

	private void OnSNewbeeTime(SNewbeeTime msg)
	{
		NoviceProtectFinishTime = Time.realtimeSinceStartup + (float)msg.timeLeft;
		if (GuideEvent.UpdateProtectTimeAction != null)
		{
			GuideEvent.UpdateProtectTimeAction(NoviceProtectFinishTime);
		}
	}

	private void ShowGuide()
	{
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", ShowGuide);
		if (CurStepCfgInfo != null)
		{
			ViewMgr.Ins.ShowView<GuidePanel>();
		}
	}

	private void OnSAllTasks(SAllTasks msg)
	{
		List<TaskInfo> tasks = msg.tasks;
		int i = 0;
		for (int count = tasks.Count; i < count; i++)
		{
			TaskInfo taskInfo = tasks[i];
			TaskCfg taskCfg = TaskCfg.Get(taskInfo.taskTypeId);
			if (taskCfg != null && taskCfg.type == 8)
			{
				_dicCfgId2TaskInfo[taskCfg.id] = taskInfo;
				_dicInsId2TaskInfo[taskInfo.taskId] = taskInfo;
			}
		}
	}

	private void InitData(SLoginFinished msg)
	{
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Remove(SLoginFinished.handler, new SLoginFinished.Handler(InitData));
		InitDices();
		StartGuide();
	}

	private void StartGuide()
	{
		CurStepCfgInfo = GetFirstGuideByTask();
		if (CurStepCfgInfo != null)
		{
			GuideEvent.StepCompleteAction = (Utils.VoidDelegate)Delegate.Combine(GuideEvent.StepCompleteAction, new Utils.VoidDelegate(CompleteCurStepNotChangeTask));
			SGetTaskReward.handler = (SGetTaskReward.Handler)Delegate.Combine(SGetTaskReward.handler, new SGetTaskReward.Handler(OnSGetTaskReward));
			SSyncTaskProgress.handler = (SSyncTaskProgress.Handler)Delegate.Combine(SSyncTaskProgress.handler, new SSyncTaskProgress.Handler(OnSSyncTaskProgress));
		}
		else
		{
			BattleEvent.OnEnteredBuildState = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnEnteredBuildState, new Utils.VoidDelegate(OnEnterBuildMode));
			BattleEvent.OnExitedBuildState = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnExitedBuildState, new Utils.VoidDelegate(OnExitedBuildMode));
			EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Remove(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimStructure));
			EventHandlers.OnAddExtraBtn = (Utils.IntDelegate)Delegate.Remove(EventHandlers.OnAddExtraBtn, new Utils.IntDelegate(OnAddBtn));
		}
	}

	public void SendRequestNewbeeMonsterMsg()
	{
		if (!_requiredNoviceMonster)
		{
			_requiredNoviceMonster = true;
			Client2Gs.Ins.Send(_cRequestNewbeeMonster);
		}
	}

	private void OnAddBtn(int arg)
	{
		_extraBtnShowId.Add(arg);
		GuideCondition value;
		if (CurStepCfgInfo != null && CurStepCfgInfo.guideConditions.TryGetValue(8, out value) && value.conditionIntArg == arg)
		{
			CompleteCurStepNotChangeTask();
		}
	}

	private void OnAimStructure(long arg)
	{
		if (_lastAim != arg)
		{
			_extraBtnShowId.Clear();
			_lastAim = arg;
			if (_lastAim < 0)
			{
				ConditionChangeToSetEffectShowHide(9);
			}
		}
	}

	private void OnExitedBuildMode()
	{
		_isInBuildMode = false;
	}

	private void OnEnterBuildMode()
	{
		_isInBuildMode = true;
	}

	private void OnSSyncTaskProgress(SSyncTaskProgress msg)
	{
		TaskInfo taskDataInfo;
		if (!GetTaskInfoByInsId(msg.taskId, out taskDataInfo))
		{
			return;
		}
		taskDataInfo.progress = msg.progress;
		if (taskDataInfo.progress >= taskDataInfo.progressMax && !taskDataInfo.rewarded)
		{
			Singleton<TaskMgr>.Ins.GetTaskReward(taskDataInfo.taskId);
			if (!IsTaskTotalComplete(taskDataInfo.taskTypeId) && GuideEvent.TaskProgressChangeAction != null)
			{
				GuideEvent.TaskProgressChangeAction(taskDataInfo);
			}
		}
		else if (GuideEvent.TaskProgressChangeAction != null)
		{
			GuideEvent.TaskProgressChangeAction(taskDataInfo);
		}
	}

	private void OnSGetTaskReward(SGetTaskReward msg)
	{
		TaskInfo value;
		if (!_dicInsId2TaskInfo.TryGetValue(msg.taskId, out value))
		{
			return;
		}
		Singleton<PlatformMgr>.Ins.onGetTaskAward(value.taskTypeId);
		Singleton<RewardShowMgr>.Ins.Add(msg);
		value.rewarded = true;
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
			return;
		}
		TaskCfg taskCfg = TaskCfg.Get(value.taskTypeId);
		if ((taskCfg != null && taskCfg.multiToOneDescId != TaskCfg.Get(CurStepCfgInfo.taskCfgId).multiToOneDescId) || !IsTaskTotalComplete())
		{
			return;
		}
		CurStepCfgInfo = StepIn(GuideCfg.Get(CurStepCfgInfo.passStepNextId));
		int num = 0;
		while (IsTaskRewarded())
		{
			PassStep();
			num++;
			if (num > 1000)
			{
				Debug.LogError("[Guide.cs]Endless loop.Last id : " + CurStepCfgInfo.id);
				TaskFinishGetOneStep();
				break;
			}
		}
		if (CurStepCfgInfo != null && GuideEvent.StepForwardAction != null)
		{
			GuideEvent.StepForwardAction();
		}
	}

	private void InitDices()
	{
		List<GuideCfg> allList = GuideCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			GuideCfg guideCfg = allList[i];
			int value;
			if (!_dicTaskCfgId2FirstGuideCfgId.TryGetValue(guideCfg.taskCfgId, out value) || value > guideCfg.id)
			{
				_dicTaskCfgId2FirstGuideCfgId[guideCfg.taskCfgId] = guideCfg.id;
			}
		}
		List<int> list = new List<int>();
		List<TaskCfg> allList2 = TaskCfg.GetAllList();
		int j = 0;
		for (int count2 = allList2.Count; j < count2; j++)
		{
			TaskCfg taskCfg = allList2[j];
			int multiToOneDescId = taskCfg.multiToOneDescId;
			if (multiToOneDescId > 0)
			{
				List<int> value2;
				if (!_dicMultiId2TaskCfgIds.TryGetValue(multiToOneDescId, out value2))
				{
					value2 = new List<int>();
					_dicMultiId2TaskCfgIds[multiToOneDescId] = value2;
				}
				value2.Add(taskCfg.id);
			}
		}
		foreach (TaskInfo value5 in _dicInsId2TaskInfo.Values)
		{
			if (value5.rewarded)
			{
				continue;
			}
			if (value5.progress >= value5.progressMax && IsTaskTotalComplete(value5.taskTypeId))
			{
				GetOneTaskReward(value5.taskId);
				continue;
			}
			list.Add(value5.taskTypeId);
			if (_dicTaskCfgId2FirstGuideCfgId.ContainsKey(value5.taskTypeId))
			{
				continue;
			}
			TaskCfg taskCfg2 = TaskCfg.Get(value5.taskTypeId);
			List<int> value3;
			if (taskCfg2 != null && _dicMultiId2TaskCfgIds.TryGetValue(taskCfg2.multiToOneDescId, out value3))
			{
				int k = 0;
				for (int count3 = value3.Count; k < count3; k++)
				{
					int value4;
					if (_dicTaskCfgId2FirstGuideCfgId.TryGetValue(value3[k], out value4))
					{
						_dicTaskCfgId2FirstGuideCfgId[value5.taskTypeId] = value4;
						break;
					}
				}
			}
			else if (taskCfg2 == null)
			{
				Debug.LogError("[GuideMgr.cs]TaskCfgInfo can't find.Id : " + value5.taskTypeId + ".");
			}
			else
			{
				Debug.LogError("[GuideMgr.cs]Novice task not exist in GuideCfg.Id : " + value5.taskTypeId + ".");
			}
		}
		if (list.Count > 0)
		{
			list.Sort();
			_firstNoviceTaskCfgId = list[0];
		}
	}

	private GuideCfg GetFirstGuideByTask()
	{
		int value;
		if (_dicTaskCfgId2FirstGuideCfgId.TryGetValue(_firstNoviceTaskCfgId, out value))
		{
			return StepIn(GuideCfg.Get(value));
		}
		return null;
	}

	public bool IsBagItemEffectShow(int curCellItemId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && CurStepCfgInfo.guideConditions.TryGetValue(2, out value) && value.conditionIntArg == curCellItemId;
	}

	public bool IsShortcutEffectShow(int itemId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && ((CurStepCfgInfo.guideConditions.TryGetValue(1, out value) && value.conditionIntArg == itemId) || TaskFinishCheckNextStepContain(itemId));
	}

	private bool TaskFinishCheckNextStepContain(int itemId)
	{
		GuideCfg guideCfg = GuideCfg.Get(CurStepCfgInfo.finishStepNextId);
		GuideCondition value;
		if (guideCfg != null)
		{
			return guideCfg.guideConditions.TryGetValue(1, out value) && value.conditionIntArg == itemId;
		}
		return false;
	}

	public bool IsExtraBtnEffectShow(int btnId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && CurStepCfgInfo.guideConditions.TryGetValue(9, out value) && value.conditionIntArg == btnId;
	}

	public bool IsBuildModeBagBuildEffectShow(int itemId)
	{
		if (CurStepCfgInfo == null)
		{
			return false;
		}
		GuideCfg guideCfg = CurStepCfgInfo;
		if (!string.IsNullOrEmpty(guideCfg.effectBtnName))
		{
			guideCfg = GuideCfg.Get(guideCfg.finishStepNextId);
		}
		GuideCondition value;
		return guideCfg != null && !IsTaskRewarded() && guideCfg.guideConditions.TryGetValue(18, out value) && value.conditionIntArg == itemId;
	}

	public bool IsAllNoviceTaskFinish()
	{
		foreach (TaskInfo value in _dicInsId2TaskInfo.Values)
		{
			if (!value.rewarded)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsProduceEffectShow(int drawingItemId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && CurStepCfgInfo.guideConditions.TryGetValue(14, out value) && drawingItemId == value.conditionIntArg;
	}

	private bool IsGuideEffectAttachToBtn()
	{
		if (CurStepCfgInfo == null)
		{
			return false;
		}
		return IsGuideEffectAttachToBtn(CurStepCfgInfo.effectBtnName);
	}

	public bool IsShowPickEffect(int itemId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && CurStepCfgInfo.guideConditions.TryGetValue(16, out value) && value.conditionStrArg.Equals("PickPanel") && (itemId == ConstsBs.DEAD_BOX_ITEM_ID || itemId == cfg.Consts.GUIDE_PICK_ITEM_ID);
	}

	public bool IsShowToolboxEffect(int itemId)
	{
		GuideCondition value;
		return CurStepCfgInfo != null && (CurStepCfgInfo.targetItemId == itemId || (CurStepCfgInfo.guideConditions.TryGetValue(2, out value) && value.conditionIntArg == itemId));
	}

	public bool IsGuideEffectAttachToBtn(string btnName)
	{
		return !string.IsNullOrEmpty(btnName);
	}

	public void CompleteCurStepNotChangeTask()
	{
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
			return;
		}
		GuideCfg nextStep = GetNextStep();
		if (nextStep == null)
		{
			TaskFinishGetOneStep();
		}
		else if (nextStep.taskCfgId == CurStepCfgInfo.nextTaskCfgId)
		{
			if (CurStepCfgInfo.specialFinishCondition == 0)
			{
				TaskInfo value;
				if (_dicCfgId2TaskInfo.TryGetValue(CurStepCfgInfo.taskCfgId, out value) && value.progress >= value.progressMax && !value.rewarded)
				{
					Singleton<TaskMgr>.Ins.GetTaskReward(value.taskId);
				}
				else if (GuideEvent.GuideEffectActiveNeedChangeAction != null)
				{
					GuideEvent.GuideEffectActiveNeedChangeAction(false);
				}
			}
		}
		else if (nextStep.taskCfgId == CurStepCfgInfo.taskCfgId)
		{
			CurStepCfgInfo = nextStep;
			if (GuideEvent.StepForwardAction != null)
			{
				GuideEvent.StepForwardAction();
			}
		}
	}

	private GuideCfg StepIn(GuideCfg guideCfgInfo)
	{
		int num = 0;
		int nextId;
		while (!MeetTotalCondition(guideCfgInfo, out nextId))
		{
			num++;
			guideCfgInfo = GuideCfg.Get(nextId);
			if (num > 100)
			{
				Debug.LogError("[GuideMgr.cs]Loop.The Last Id : " + guideCfgInfo.id + ".");
				return null;
			}
		}
		if (guideCfgInfo != null)
		{
			switch (guideCfgInfo.specialFinishCondition)
			{
			case 10:
				ViewMgr.Ins.AddOnShowEvent<LockPanel>(OnLockPanelShow);
				break;
			case 15:
				ViewMgr.Ins.AddOnShowEvent<PickPanel>(OnPickPanelShow);
				break;
			case 12:
				ViewMgr.Ins.AddOnShowEvent<WorkbenchPanel>(OnWorkbenchPanelShow);
				break;
			case 20:
				ViewMgr.Ins.AddOnShowEvent<ToolboxPanel>(OnToolboxPanelShow);
				break;
			}
		}
		return guideCfgInfo;
	}

	private void OnToolboxPanelShow()
	{
		Utils.TriggerEvent(GuideEvent.StepCompleteAction);
		ViewMgr.Ins.RemoveOnShowEvent("ToolboxPanel", OnToolboxPanelShow);
	}

	private void OnWorkbenchPanelShow()
	{
		Utils.TriggerEvent(GuideEvent.StepCompleteAction);
		ViewMgr.Ins.RemoveOnShowEvent("WorkbenchPanel", OnWorkbenchPanelShow);
	}

	private void OnPickPanelShow()
	{
		Utils.TriggerEvent(GuideEvent.StepCompleteAction);
		ViewMgr.Ins.RemoveOnShowEvent("PickPanel", OnPickPanelShow);
	}

	private void OnLockPanelShow()
	{
		Utils.TriggerEvent(GuideEvent.StepCompleteAction);
		ViewMgr.Ins.RemoveOnShowEvent("LockPanel", OnLockPanelShow);
	}

	private bool MeetTotalCondition(GuideCfg guideCfgInfo, out int nextId)
	{
		nextId = -1;
		if (guideCfgInfo == null)
		{
			return true;
		}
		Dictionary<int, GuideCondition> guideConditions = guideCfgInfo.guideConditions;
		foreach (KeyValuePair<int, GuideCondition> item in guideConditions)
		{
			GuideCondition value = item.Value;
			if (!MeetCondition(item.Key, value.conditionIntArg, value.conditionStrArg))
			{
				nextId = value.nextIdNotMeetCondition;
				return false;
			}
		}
		return true;
	}

	private GuideCfg GetNextStep()
	{
		return GetNextStep(CurStepCfgInfo.finishStepNextId);
	}

	private GuideCfg GetNextStep(int finishStepNextId)
	{
		GuideCfg guideCfgInfo = GuideCfg.Get(finishStepNextId);
		return StepIn(guideCfgInfo);
	}

	public bool GetTaskInfoByCfgId(int cfgId, out TaskInfo taskDataInfo)
	{
		return _dicCfgId2TaskInfo.TryGetValue(cfgId, out taskDataInfo);
	}

	public bool GetTaskInfoByInsId(long insId, out TaskInfo taskDataInfo)
	{
		return _dicInsId2TaskInfo.TryGetValue(insId, out taskDataInfo);
	}

	public bool GetTaskCfgIdsByMultiId(int multiCfgId, out List<int> taskCfgIds)
	{
		return _dicMultiId2TaskCfgIds.TryGetValue(multiCfgId, out taskCfgIds);
	}

	private void CompleteTaskByClickBtn()
	{
		if (CurStepCfgInfo == null)
		{
			return;
		}
		TaskCfg taskCfg = TaskCfg.Get(CurStepCfgInfo.taskCfgId);
		List<int> value;
		if (_dicMultiId2TaskCfgIds.TryGetValue(taskCfg.multiToOneDescId, out value))
		{
			int i = 0;
			for (int count = value.Count; i < count; i++)
			{
				GetOneTaskReward(value[i]);
			}
		}
		else
		{
			GetOneTaskReward(CurStepCfgInfo.taskCfgId);
		}
	}

	public bool IsTaskTotalComplete()
	{
		if (CurStepCfgInfo == null)
		{
			return false;
		}
		return IsTaskTotalComplete(CurStepCfgInfo.taskCfgId);
	}

	public bool IsTaskTotalComplete(int taskCfgId)
	{
		TaskCfg taskCfgInfo = TaskCfg.Get(taskCfgId);
		return IsTaskTotalComplete(taskCfgInfo);
	}

	public bool IsTaskTotalComplete(TaskCfg taskCfgInfo)
	{
		bool result = true;
		List<int> value;
		if (_dicMultiId2TaskCfgIds.TryGetValue(taskCfgInfo.multiToOneDescId, out value))
		{
			int i = 0;
			for (int count = value.Count; i < count; i++)
			{
				if (!IsTaskComplete(value[i]))
				{
					result = false;
					break;
				}
			}
		}
		else
		{
			result = IsTaskComplete(taskCfgInfo.id);
		}
		return result;
	}

	private void GetOneTaskReward(int taskCfgId)
	{
		TaskInfo value;
		if (_dicCfgId2TaskInfo.TryGetValue(taskCfgId, out value))
		{
			GetOneTaskReward(value.taskId);
		}
	}

	private void GetOneTaskReward(long taskId)
	{
		Singleton<TaskMgr>.Ins.GetTaskReward(taskId);
	}

	private void PassStep()
	{
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
		}
		else if (CurStepCfgInfo.passStepNextId > 0)
		{
			CurStepCfgInfo = GuideCfg.Get(CurStepCfgInfo.passStepNextId);
		}
		else
		{
			TaskFinishGetOneStep();
		}
	}

	private bool IsTaskRewarded()
	{
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
			return false;
		}
		TaskInfo value;
		if (_dicCfgId2TaskInfo.TryGetValue(CurStepCfgInfo.taskCfgId, out value))
		{
			return value.rewarded || value.finishedBefore;
		}
		return true;
	}

	private bool IsTaskComplete(int taskCfgId)
	{
		TaskInfo value;
		if (_dicCfgId2TaskInfo.TryGetValue(taskCfgId, out value))
		{
			return value.progress >= value.progressMax;
		}
		return true;
	}

	private void TaskFinishGetOneStep()
	{
		int num = int.MaxValue;
		foreach (TaskInfo value2 in _dicInsId2TaskInfo.Values)
		{
			if (!value2.rewarded && num > value2.taskTypeId)
			{
				num = value2.taskTypeId;
			}
		}
		int value;
		if (_dicTaskCfgId2FirstGuideCfgId.TryGetValue(num, out value))
		{
			CurStepCfgInfo = GuideCfg.Get(value);
			if (GuideEvent.StepInNotMeetConditionTask != null)
			{
				GuideEvent.StepInNotMeetConditionTask();
			}
		}
		else
		{
			TotalEnd();
		}
	}

	private void TotalEnd()
	{
		Singleton<PlatformMgr>.Ins.OnFinishNewBee();
		if (GuideEvent.GuideTotalFinishAction != null)
		{
			GuideEvent.GuideTotalFinishAction();
		}
		CurStepCfgInfo = null;
		if (Time.realtimeSinceStartup > NoviceProtectFinishTime)
		{
			ViewMgr.Ins.Destroy<GuidePanel>();
		}
		GuideEvent.StepCompleteAction = (Utils.VoidDelegate)Delegate.Remove(GuideEvent.StepCompleteAction, new Utils.VoidDelegate(CompleteCurStepNotChangeTask));
		SGetTaskReward.handler = (SGetTaskReward.Handler)Delegate.Remove(SGetTaskReward.handler, new SGetTaskReward.Handler(OnSGetTaskReward));
		SSyncTaskProgress.handler = (SSyncTaskProgress.Handler)Delegate.Remove(SSyncTaskProgress.handler, new SSyncTaskProgress.Handler(OnSSyncTaskProgress));
		BattleEvent.OnEnteredBuildState = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnEnteredBuildState, new Utils.VoidDelegate(OnEnterBuildMode));
		BattleEvent.OnExitedBuildState = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnExitedBuildState, new Utils.VoidDelegate(OnExitedBuildMode));
		EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Remove(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimStructure));
		EventHandlers.OnAddExtraBtn = (Utils.IntDelegate)Delegate.Remove(EventHandlers.OnAddExtraBtn, new Utils.IntDelegate(OnAddBtn));
		ViewMgr.Ins.ShowTopView<LadderPopupPanel>();
	}

	private void ConditionChangeToSetEffectShowHide(int conditionType)
	{
		GuideCondition value;
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
		}
		else if (CurStepCfgInfo.guideConditions.TryGetValue(conditionType, out value) && GuideEvent.GuideEffectActiveNeedChangeAction != null)
		{
			int nextId;
			GuideEvent.GuideEffectActiveNeedChangeAction(MeetTotalCondition(CurStepCfgInfo, out nextId));
		}
	}

	private void ConditionChangeToRollBack(int conditionType)
	{
		GuideCondition value;
		if (CurStepCfgInfo == null)
		{
			TaskFinishGetOneStep();
		}
		else if (CurStepCfgInfo.guideConditions.TryGetValue(conditionType, out value))
		{
			CurStepCfgInfo = GuideCfg.Get(value.nextIdNotMeetCondition);
			CurStepCfgInfo = StepIn(CurStepCfgInfo);
			if (GuideEvent.StepForwardAction != null)
			{
				GuideEvent.StepForwardAction();
			}
		}
	}

	private bool MeetCondition(int type, int intArg, string strArg)
	{
		switch (type)
		{
		case 1:
			return IsInQuickUse(intArg);
		case 2:
			return IsInBag(intArg);
		case 3:
			return IsEquipItem(intArg);
		case 4:
			return IsInBuildMode();
		case 5:
			return false;
		case 6:
			return true;
		case 7:
			return !IsInBuildMode();
		case 8:
			return !IsExtraBtnShow(intArg);
		case 9:
			return IsExtraBtnShow(intArg);
		case 11:
			return IsInQuickUse(intArg);
		case 13:
			return !Singleton<ScProduceMgr>.Ins.IsLearned(intArg);
		case 14:
			return Singleton<ScProduceMgr>.Ins.IsLearned(intArg);
		case 16:
			return ViewMgr.Ins.IsShow(strArg);
		case 18:
			return IsInQuickUse(intArg) || IsInBag(intArg);
		case 19:
			return !ViewMgr.Ins.IsShow(strArg);
		default:
			return true;
		}
	}

	private bool IsExtraBtnShow(int btnId)
	{
		return _extraBtnShowId.Contains(btnId);
	}

	private bool IsEquipItem(int itemId)
	{
		BagItem bagItemByInstanceId = Singleton<BagMgr>.Ins.GetBagItemByInstanceId(Singleton<BagMgr>.Ins.HandInstanceId);
		if (bagItemByInstanceId != null && bagItemByInstanceId.itemId == itemId)
		{
			return true;
		}
		List<BagItem> equipItems = Singleton<BagMgr>.Ins.EquipItems;
		int i = 0;
		for (int count = equipItems.Count; i < count; i++)
		{
			if (equipItems[i].itemId == itemId)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsInBuildMode()
	{
		return _isInBuildMode;
	}

	private bool IsInQuickUse(int itemId)
	{
		Dictionary<int, BagItem> quickUseItems = Singleton<BagMgr>.Ins.QuickUseItems;
		foreach (KeyValuePair<int, BagItem> item in quickUseItems)
		{
			if (item.Value.itemId == itemId)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsInBag(int itemId)
	{
		List<BagItem> bagItems = Singleton<BagMgr>.Ins.BagItems;
		int i = 0;
		for (int count = bagItems.Count; i < count; i++)
		{
			if (bagItems[i].itemId == itemId)
			{
				return true;
			}
		}
		return false;
	}
}
