using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.task.scmsg;

namespace SC.UI
{
	public class GuidePage : MonoBehaviour, ITeamTaskPage
	{
		private const float PackUpX = -305f;

		private const float Duration = 0.5f;

		private readonly Dictionary<int, GGuideTaskConditionCell> _dicTaskCfgId2Cell = new Dictionary<int, GGuideTaskConditionCell>();

		private bool _curMulti;

		private RectTransform _rectTransform;

		public List<GGuideTaskConditionCell> m_condition_parentlist = new List<GGuideTaskConditionCell>();

		public GameObject[] m_condition_parent;

		public GameObject m_condition_parentObj;

		public GameObject m_tasks;

		public GameObject txt_desc_total;

		public Text txt_desc_totalText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_progress;

		public Text txt_progressText;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		public GameObject ThisGo { get; private set; }

		public void OnInit()
		{
			ThisGo = base.gameObject;
			_rectTransform = base.transform as RectTransform;
			ClickListener clickListener = ClickListener.Get(ThisGo, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__0;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
		}

		public void OnShow(object param)
		{
			StepIn();
			Vector2 vector = default(Vector2);
			vector.x = 0f;
			vector.y = _rectTransform.anchoredPosition.y;
			Vector2 anchoredPosition = vector;
			_rectTransform.anchoredPosition = anchoredPosition;
			GuideEvent.StepForwardAction = (Action)Delegate.Combine(GuideEvent.StepForwardAction, new Action(OnStepForward));
			GuideEvent.TaskProgressChangeAction = (Action<TaskInfo>)Delegate.Combine(GuideEvent.TaskProgressChangeAction, new Action<TaskInfo>(OnTaskProgressChange));
			GuideEvent.StepInNotMeetConditionTask = (Action)Delegate.Combine(GuideEvent.StepInNotMeetConditionTask, new Action(OnStepForward));
		}

		public void OnHide()
		{
			GuideEvent.StepForwardAction = (Action)Delegate.Remove(GuideEvent.StepForwardAction, new Action(OnStepForward));
			GuideEvent.TaskProgressChangeAction = (Action<TaskInfo>)Delegate.Remove(GuideEvent.TaskProgressChangeAction, new Action<TaskInfo>(OnTaskProgressChange));
			GuideEvent.StepInNotMeetConditionTask = (Action)Delegate.Remove(GuideEvent.StepInNotMeetConditionTask, new Action(OnStepForward));
		}

		private void StepIn()
		{
			GuideCfg curStepCfgInfo = Singleton<GuideMgr>.Ins.CurStepCfgInfo;
			if (curStepCfgInfo == null)
			{
				ThisGo.SetActiveBetter(false);
				return;
			}
			TaskCfg taskCfg = TaskCfg.Get(curStepCfgInfo.taskCfgId);
			int multiToOneDescId = taskCfg.multiToOneDescId;
			_curMulti = multiToOneDescId > 0;
			if (_curMulti)
			{
				List<int> taskCfgIds;
				Singleton<GuideMgr>.Ins.GetTaskCfgIdsByMultiId(multiToOneDescId, out taskCfgIds);
				ShowMultiTask(taskCfg.multiToOneDescId, taskCfgIds, taskCfg.name);
			}
			else
			{
				TaskInfo taskDataInfo;
				Singleton<GuideMgr>.Ins.GetTaskInfoByCfgId(taskCfg.id, out taskDataInfo);
				ResetSingleTask(taskCfg, taskDataInfo);
			}
		}

		private void OnTaskProgressChange(TaskInfo taskDataInfo)
		{
			string progress;
			bool isFinish;
			GetProgressStrFinishInfoByTaskInfo(taskDataInfo, out progress, out isFinish);
			if (_curMulti)
			{
				MultiTaskProgressChange(taskDataInfo.taskTypeId, progress, isFinish);
			}
			else
			{
				SetSingleTaskProgress(progress);
			}
		}

		private void GetProgressStrFinishInfoByTaskInfo(TaskInfo taskDataInfo, out string progress, out bool isFinish)
		{
			if (taskDataInfo == null)
			{
				progress = string.Empty;
				isFinish = true;
				return;
			}
			int progress2 = taskDataInfo.progress;
			int progressMax = taskDataInfo.progressMax;
			progress = Utils.GetString(343, Utils.GetString(9, progress2, progressMax));
			isFinish = progress2 >= progressMax;
		}

		private void OnStepForward()
		{
			StepIn();
		}

		private void ShowMultiTask(int multiCfgId, List<int> taskCfgIds, string taskName)
		{
			if (taskCfgIds == null || taskCfgIds.Count <= 0)
			{
				Debug.LogError("[GuidePage.cs]Unexpected null or empty list.");
				return;
			}
			if (Singleton<GuideMgr>.Ins.IsTaskTotalComplete())
			{
				Debug.LogError("[GuidePage.cs]Already finished task but showed.");
			}
			View.SetLabelText(txt_progressText, string.Empty);
			MultiToOneTaskDescCfg multiToOneTaskDescCfg = MultiToOneTaskDescCfg.Get(multiCfgId);
			View.SetLabelText(txt_nameText, taskName);
			View.SetLabelText(txt_desc_totalText, multiToOneTaskDescCfg.desc);
			int count = taskCfgIds.Count;
			int i = 0;
			for (int num = m_condition_parent.Length; i < num; i++)
			{
				if (i < count)
				{
					m_condition_parent[i].SetActiveBetter(true);
					int num2 = taskCfgIds[i];
					TaskCfg taskCfgInfo = TaskCfg.Get(num2);
					TaskInfo taskDataInfo;
					Singleton<GuideMgr>.Ins.GetTaskInfoByCfgId(num2, out taskDataInfo);
					FillMultiTaskConditions(m_condition_parentlist[i], taskCfgInfo, taskDataInfo);
				}
				else
				{
					m_condition_parent[i].SetActiveBetter(false);
				}
			}
		}

		private void FillMultiTaskConditions(GGuideTaskConditionCell cell, TaskCfg taskCfgInfo, TaskInfo taskDataInfo)
		{
			_dicTaskCfgId2Cell[taskCfgInfo.id] = cell;
			string progress;
			bool isFinish;
			GetProgressStrFinishInfoByTaskInfo(taskDataInfo, out progress, out isFinish);
			View.SetLabelText(cell.txt_descText, Utils.GetString(taskCfgInfo.desc, progress));
			cell.m_finish.SetActiveBetter(isFinish);
		}

		private void MultiTaskProgressChange(int taskCfgId, string newProgress, bool isFinish)
		{
			GGuideTaskConditionCell value;
			if (_dicTaskCfgId2Cell.TryGetValue(taskCfgId, out value))
			{
				TaskCfg taskCfg = TaskCfg.Get(taskCfgId);
				View.SetLabelText(value.txt_descText, Utils.GetString(taskCfg.desc, newProgress));
				value.m_finish.SetActiveBetter(isFinish);
			}
		}

		private void ResetSingleTask(TaskCfg taskCfgInfo, TaskInfo taskDataInfo)
		{
			_dicTaskCfgId2Cell.Clear();
			int i = 0;
			for (int num = m_condition_parent.Length; i < num; i++)
			{
				m_condition_parent[i].SetActiveBetter(false);
			}
			View.SetLabelText(txt_nameText, taskCfgInfo.name);
			View.SetLabelText(txt_desc_totalText, taskCfgInfo.desc);
			SetSingleTaskProgress(Utils.GetString(9, taskDataInfo.progress, taskDataInfo.progressMax));
		}

		private void SetSingleTaskProgress(string progress)
		{
			View.SetLabelText(txt_progressText, progress);
		}

		private void Awake()
		{
			m_condition_parent = base.transform.Find("GameObject/Image (1)/m_tasks/Content/m_condition_parent").gameObject.GetComponent<UIGameObjectList>().objects;
			m_condition_parentObj = base.transform.Find("GameObject/Image (1)/m_tasks/Content/m_condition_parent").gameObject;
			if (m_condition_parentlist.Count <= 0)
			{
				for (int i = 0; i < m_condition_parent.Length; i++)
				{
					m_condition_parentlist.Add(View.AddComponentIfNotExist<GGuideTaskConditionCell>(m_condition_parent[i].gameObject));
				}
			}
			m_tasks = base.transform.Find("GameObject/Image (1)/m_tasks").gameObject;
			txt_desc_total = base.transform.Find("GameObject/Image (1)/m_tasks/Content/m_condition_parent/txt_desc_total").gameObject;
			txt_desc_totalText = txt_desc_total.GetComponent<Text>();
			txt_name = base.transform.Find("GameObject/Image (1)/m_tasks/Content/m_condition_parent/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_progress = base.transform.Find("GameObject/Image (1)/m_tasks/Content/m_condition_parent/txt_name/txt_progress").gameObject;
			txt_progressText = txt_progress.GetComponent<Text>();
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__0(GameObject go)
		{
			if (GuideEvent.ShowGuideTipsAction != null)
			{
				GuideEvent.ShowGuideTipsAction();
			}
		}
	}
}
