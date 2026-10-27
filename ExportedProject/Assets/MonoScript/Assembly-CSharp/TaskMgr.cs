using System;
using System.Collections.Generic;
using SC.UI;
using cfg;
using gs.task.scmsg;

public class TaskMgr : Singleton<TaskMgr>
{
	public List<TaskInfo> TaskDailyList = new List<TaskInfo>();

	public List<TaskInfo> ChallengeList = new List<TaskInfo>();

	public List<TaskInfo> SuperList = new List<TaskInfo>();

	public bool IsReadTask;

	public bool HasDaySpecialTask;

	public TaskInfo DaySpecialTaskInfo;

	public const string Email = "EmailPanel";

	public const string Shop = "ShopPanel";

	public const string ShopDiamond = "ShopDiamond";

	private readonly CGetTaskReward _cGetTaskReward = new CGetTaskReward();

	private readonly CUploadItem _cUploadItem = new CUploadItem();

	public void Init()
	{
		SAllTasks.handler = (SAllTasks.Handler)Delegate.Combine(SAllTasks.handler, new SAllTasks.Handler(SAllTasksHandle));
		SAddTask.handler = (SAddTask.Handler)Delegate.Combine(SAddTask.handler, new SAddTask.Handler(SAddTaskHandle));
		SSyncTaskProgress.handler = (SSyncTaskProgress.Handler)Delegate.Combine(SSyncTaskProgress.handler, new SSyncTaskProgress.Handler(SSyncTaskProgressHandle));
		SGetTaskReward.handler = (SGetTaskReward.Handler)Delegate.Combine(SGetTaskReward.handler, new SGetTaskReward.Handler(SGetTaskRewardHandle));
		SRemoveTask.handler = (SRemoveTask.Handler)Delegate.Combine(SRemoveTask.handler, new SRemoveTask.Handler(SRemoveTaskHandle));
		STaskLevelUp.handler = (STaskLevelUp.Handler)Delegate.Combine(STaskLevelUp.handler, new STaskLevelUp.Handler(OnSTaskLevelUp));
		SUploadItem.handler = (SUploadItem.Handler)Delegate.Combine(SUploadItem.handler, new SUploadItem.Handler(SUploadItemHandle));
	}

	private void SUploadItemHandle(SUploadItem msg)
	{
		AlertBox.Show(320);
	}

	private void OnSTaskLevelUp(STaskLevelUp msg)
	{
		TaskInfo taskById = GetTaskById(msg.oldId);
		if (taskById != null)
		{
			taskById.progress = msg.progress;
			taskById.progressMax = msg.progressMax;
			taskById.rewarded = false;
			taskById.taskTypeId++;
			Utils.TriggerEvent(TaskEvent.RefreshTaskRed);
			Utils.TriggerEvent(TaskEvent.RefreshTask);
		}
	}

	private void SGetTaskRewardHandle(SGetTaskReward msg)
	{
		TaskInfo taskById = GetTaskById(msg.taskId);
		if (taskById != null)
		{
			Singleton<PlatformMgr>.Ins.onGetTaskAward(taskById.taskTypeId);
			ViewMgr.Ins.AddView<GainPanel>(msg.dropDetail, false);
			DeleteTask(msg.taskId);
			Utils.TriggerEvent(TaskEvent.RefreshTaskRed);
			Utils.TriggerEvent(TaskEvent.RefreshTask);
			Utils.TriggerEvent(LadderEvent.RefreshLevel);
		}
	}

	private void SRemoveTaskHandle(SRemoveTask s)
	{
		DeleteTask(s.taskId);
		Utils.TriggerEvent(TaskEvent.RefreshTaskRed);
		Utils.TriggerEvent(TaskEvent.RefreshTask);
	}

	private void SAllTasksHandle(SAllTasks s)
	{
		IsReadTask = s.isReadTask;
		TaskDailyList.Clear();
		List<TaskInfo> tasks = s.tasks;
		int i = 0;
		for (int count = tasks.Count; i < count; i++)
		{
			TaskCfg taskCfg = TaskCfg.Get(tasks[i].taskTypeId);
			if (taskCfg == null)
			{
				continue;
			}
			if (taskCfg.type == 1 || taskCfg.type == 4 || taskCfg.type == 5 || taskCfg.type == 7)
			{
				if (tasks[i].progress < tasks[i].progressMax || !tasks[i].rewarded)
				{
					TaskDailyList.Add(tasks[i]);
				}
			}
			else if (taskCfg.type == 1 || taskCfg.type == 4 || taskCfg.type == 5 || taskCfg.type == 7)
			{
				if (tasks[i].progress < tasks[i].progressMax || !tasks[i].rewarded)
				{
					HasDaySpecialTask = true;
					DaySpecialTaskInfo = tasks[i];
					TaskDailyList.Add(tasks[i]);
				}
			}
			else if (taskCfg.type == 1 || taskCfg.type == 4 || taskCfg.type == 5 || taskCfg.type == 7)
			{
				TaskDailyList.Add(tasks[i]);
			}
			else if (taskCfg.type == 2)
			{
				if (tasks[i].progress < tasks[i].progressMax || !tasks[i].rewarded)
				{
					ChallengeList.Add(tasks[i]);
				}
			}
			else if (taskCfg.type == 3 && (tasks[i].progress < tasks[i].progressMax || !tasks[i].rewarded))
			{
				SuperList.Add(tasks[i]);
			}
		}
		TaskDailyList.Sort(Sort);
		ChallengeList.Sort(Sort);
		SuperList.Sort(Sort);
	}

	public void ReadTask()
	{
		CIsReadTask msg = new CIsReadTask();
		Client2Gs.Ins.Send(msg);
		IsReadTask = true;
	}

	public TaskInfo GetDaySpecialTask()
	{
		int i = 0;
		for (int count = TaskDailyList.Count; i < count; i++)
		{
			TaskCfg taskCfg = TaskCfg.Get(TaskDailyList[i].taskTypeId);
			if ((taskCfg.type == 1 || taskCfg.type == 4 || taskCfg.type == 5 || taskCfg.type == 7) && (TaskDailyList[i].progress < TaskDailyList[i].progressMax || !TaskDailyList[i].rewarded))
			{
				return TaskDailyList[i];
			}
		}
		return null;
	}

	private void SAddTaskHandle(SAddTask s)
	{
		TaskCfg taskCfg = TaskCfg.Get(s.task.taskTypeId);
		if (taskCfg == null)
		{
			return;
		}
		if (taskCfg.type == 1 || taskCfg.type == 4 || taskCfg.type == 5 || taskCfg.type == 7)
		{
			for (int i = 0; i < TaskDailyList.Count; i++)
			{
				if (TaskDailyList[i].taskId == s.task.taskId)
				{
					TaskDailyList[i] = s.task;
					return;
				}
			}
			TaskDailyList.Add(s.task);
			TaskDailyList.Sort(Sort);
		}
		else if (taskCfg.type == 2)
		{
			ChallengeList.Add(s.task);
			ChallengeList.Sort(Sort);
		}
		else if (taskCfg.type == 3)
		{
			SuperList.Add(s.task);
			SuperList.Sort(Sort);
		}
		Utils.TriggerEvent(TaskEvent.RefreshTask);
	}

	private void SSyncTaskProgressHandle(SSyncTaskProgress s)
	{
		TaskInfo taskById = GetTaskById(s.taskId);
		if (taskById != null)
		{
			taskById.progress = s.progress;
		}
		Utils.TriggerEvent(TaskEvent.RefreshTask);
	}

	public void DeleteTask(long id)
	{
		TaskInfo taskById = GetTaskById(id);
		if (taskById != null)
		{
			if (TaskDailyList.Contains(taskById))
			{
				TaskDailyList.Remove(taskById);
			}
			if (ChallengeList.Contains(taskById))
			{
				ChallengeList.Remove(taskById);
			}
			if (SuperList.Contains(taskById))
			{
				SuperList.Remove(taskById);
			}
		}
	}

	public TaskInfo GetTaskById(long id)
	{
		int i = 0;
		for (int count = TaskDailyList.Count; i < count; i++)
		{
			if (TaskDailyList[i].taskId == id)
			{
				return TaskDailyList[i];
			}
		}
		int j = 0;
		for (int count2 = ChallengeList.Count; j < count2; j++)
		{
			if (ChallengeList[j].taskId == id)
			{
				return ChallengeList[j];
			}
		}
		int k = 0;
		for (int count3 = SuperList.Count; k < count3; k++)
		{
			if (SuperList[k].taskId == id)
			{
				return SuperList[k];
			}
		}
		return null;
	}

	public bool DailyListHasAward()
	{
		int i = 0;
		for (int count = TaskDailyList.Count; i < count; i++)
		{
			if (TaskDailyList[i].progress >= TaskDailyList[i].progressMax && !TaskDailyList[i].rewarded)
			{
				return true;
			}
		}
		return false;
	}

	public bool ChallengeHasAward()
	{
		int i = 0;
		for (int count = ChallengeList.Count; i < count; i++)
		{
			TaskInfo taskInfo = ChallengeList[i];
			if (taskInfo.progress >= taskInfo.progressMax && !taskInfo.rewarded)
			{
				return true;
			}
		}
		return false;
	}

	public bool SuperHasAward()
	{
		int i = 0;
		for (int count = SuperList.Count; i < count; i++)
		{
			TaskInfo taskInfo = SuperList[i];
			if (taskInfo.progress >= taskInfo.progressMax && !taskInfo.rewarded)
			{
				return true;
			}
		}
		return false;
	}

	public int Sort(TaskInfo info1, TaskInfo info2)
	{
		TaskCfg taskCfg = TaskCfg.Get(info1.taskTypeId);
		TaskCfg taskCfg2 = TaskCfg.Get(info2.taskTypeId);
		int num = CanGetAward(info1);
		int num2 = CanGetAward(info2);
		if (num != num2)
		{
			return num2 - num;
		}
		if (taskCfg.type != taskCfg2.type)
		{
			return taskCfg.type - taskCfg2.type;
		}
		return (int)(info1.taskId - info2.taskId);
	}

	public int CanGetAward(TaskInfo info)
	{
		return (info.progress < info.progressMax || info.rewarded) ? 1 : 2;
	}

	public void TaskLevelUp(int taskId)
	{
		TaskInfo taskById = GetTaskById(taskId);
		TaskCfg taskCfg = TaskCfg.Get(taskById.taskTypeId);
		int flag = taskCfg.flag;
		int num = taskCfg.level++;
		List<TaskCfg> allList = TaskCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			if (allList[i].flag != flag || allList[i].level == num)
			{
			}
		}
	}

	public static void GoPanel(int type)
	{
		switch (type)
		{
		case 2:
			break;
		case 3:
			break;
		case 1:
			break;
		case 4:
			break;
		}
	}

	public void GetTaskReward(long taskId)
	{
		_cGetTaskReward.taskId = taskId;
		Client2Gs.Ins.Send(_cGetTaskReward);
	}

	public void UploadItem(int instanceId, int itemId, int number)
	{
		_cUploadItem.instanceId = instanceId;
		_cUploadItem.itemId = itemId;
		_cUploadItem.number = number;
		Client2Gs.Ins.Send(_cUploadItem);
	}

	public void UploadItem(int taskId)
	{
		TaskCfg taskCfg = TaskCfg.Get(taskId);
		int num = taskCfg.args[1];
		ItemCfg itemCfg = ItemCfg.Get(num);
		if (itemCfg.isPileAble)
		{
			_cUploadItem.instanceId = 0;
			_cUploadItem.itemId = itemCfg.id;
			_cUploadItem.number = taskCfg.args[2];
			Client2Gs.Ins.Send(_cUploadItem);
		}
		else
		{
			ViewMgr.Ins.ShowView<LadderItemSelectPanel>(num);
		}
	}

	public bool IsShowRedDot()
	{
		return DailyListHasAward() || ChallengeHasAward() || SuperHasAward();
	}
}
