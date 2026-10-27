using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.task.scmsg;

namespace SC.UI
{
	public class GTaskPage : MonoBehaviour, IBagAndBuildPage
	{
		private enum TaskPageType
		{
			None = 0,
			Day = 1,
			Challenge = 2,
			SuperTask = 3
		}

		[CompilerGenerated]
		private sealed class _003CInitItemCallBack_003Ec__AnonStorey1
		{
			internal bool isJingying;

			internal List<DropMgr.DropDesInfo> list;

			internal TaskInfo info;

			internal TaskCfg taskCfg;

			private static Utils.VoidDelegate _003C_003Ef__am_0024cache0;

			internal void _003C_003Em__0(GameObject go)
			{
				if (isJingying && Singleton<LadderMgr>.Ins.Info.taskType == 0)
				{
					if (_003C_003Ef__am_0024cache0 == null)
					{
						_003C_003Ef__am_0024cache0 = _003C_003Em__3;
					}
					MessageBoxPanel.Show(354, _003C_003Ef__am_0024cache0);
					return;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].itemId > 0 && !Singleton<BagMgr>.Ins.IsHasCapacity(list[i].itemId, list[i].num))
					{
						AlertBox.Show(26);
						return;
					}
				}
				Singleton<TaskMgr>.Ins.GetTaskReward(info.taskId);
			}

			internal void _003C_003Em__1(GameObject go)
			{
				TaskMgr.GoPanel(taskCfg.dest);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				TaskCfg taskCfg = TaskCfg.Get(this.taskCfg.id);
				int num = taskCfg.args[1];
				ItemCfg itemCfg = ItemCfg.Get(num);
				if (itemCfg.isPileAble)
				{
					Singleton<TaskMgr>.Ins.UploadItem(0, num, taskCfg.args[2] - info.progress);
				}
				else
				{
					ViewMgr.Ins.ShowView<LadderItemSelectPanel>(num);
				}
			}

			private static void _003C_003Em__3()
			{
				ViewMgr.Ins.ShowView<LadderBuyTypePanel>(null, false);
			}
		}

		private TaskPageType _currentType;

		private UIScrollPanel _uiScrollPanel;

		private List<TaskInfo> _currentTaskList;

		public int WeekBoxConditionMax = 10;

		private int _finishTime;

		private WaitForSeconds WaitSeconds1;

		public GameObject btn_already_finish;

		public GameObject btn_challenge;

		public GameObject btn_day;

		public GameObject btn_get;

		public GameObject btn_no_finish;

		public GameObject btn_super_task;

		public GTaskItem m_cell;

		public GameObject m_challenge_red;

		public GameObject m_day_red;

		public GameObject m_super_red;

		public GameObject m_task;

		public GameObject m_task_normal;

		public GameObject m_task_senior;

		public GameObject m_taskbox;

		public GameObject scp_task;

		public GameObject txt_condition;

		public Text txt_conditionText;

		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_off_0;

		public Text txt_off_0Text;

		public GameObject txt_off_1;

		public Text txt_off_1Text;

		public GameObject txt_on;

		public Text txt_onText;

		public GameObject txt_on_0;

		public Text txt_on_0Text;

		public GameObject txt_on_1;

		public Text txt_on_1Text;

		public GameObject txt_remain_time;

		public Text txt_remain_timeText;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		public bool IsShow
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public void OnInit()
		{
			WaitSeconds1 = Utils.WaitForSeconds(1f);
			_uiScrollPanel = scp_task.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_get, string.Empty).onClick = OnClickGetBox;
			ClickListener.Get(btn_day, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener.Get(btn_challenge, string.Empty).onClick = _003COnInit_003Em__1;
			ClickListener.Get(btn_super_task, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener clickListener = ClickListener.Get(m_task_normal, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__3;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener clickListener2 = ClickListener.Get(m_task_senior, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003COnInit_003Em__4;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
			m_cell.gameObject.SetActiveBetter(false);
			WeekBoxConditionMax = cfg.Consts.TASK_BOX_MAX_COUNT;
		}

		public void OnShow(object param = null)
		{
			TaskEvent.RefreshTask = (Utils.VoidDelegate)Delegate.Combine(TaskEvent.RefreshTask, new Utils.VoidDelegate(RefreshTask));
			LadderEvent.FinishTaskNumberChanged = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.FinishTaskNumberChanged, new Utils.VoidDelegate(FinishTaskNumberChanged));
			LadderEvent.RefreshBox = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshBox, new Utils.VoidDelegate(SetBoxData));
			ChangeType(TaskPageType.Day);
			RefreshData();
			SetBoxData();
			base.gameObject.SetActiveBetter(true);
			StartCoroutine(Tick1());
			_finishTime = Singleton<LadderMgr>.Ins.FinishTime;
			RefreshTiemCountDown();
		}

		private void FinishTaskNumberChanged()
		{
			SetBoxData();
		}

		private void RefreshTask()
		{
			RefreshData();
		}

		public void OnHide()
		{
			TaskEvent.RefreshTask = (Utils.VoidDelegate)Delegate.Remove(TaskEvent.RefreshTask, new Utils.VoidDelegate(RefreshTask));
			LadderEvent.FinishTaskNumberChanged = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.FinishTaskNumberChanged, new Utils.VoidDelegate(FinishTaskNumberChanged));
			LadderEvent.RefreshBox = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshBox, new Utils.VoidDelegate(SetBoxData));
			base.gameObject.SetActiveBetter(false);
			RadioButton.ChooseBtn(btn_day);
		}

		private void ChangeType(TaskPageType taskPageType)
		{
			if (_currentType != taskPageType)
			{
				_currentType = taskPageType;
				RefreshData();
			}
		}

		private void RefreshData()
		{
			RefreshRed();
			_uiScrollPanel.Clear();
			if (_currentType == TaskPageType.Day)
			{
				_currentTaskList = Singleton<TaskMgr>.Ins.TaskDailyList;
			}
			else if (_currentType == TaskPageType.Challenge)
			{
				_currentTaskList = Singleton<TaskMgr>.Ins.ChallengeList;
			}
			else if (_currentType == TaskPageType.SuperTask)
			{
				_currentTaskList = Singleton<TaskMgr>.Ins.SuperList;
			}
			_currentTaskList.Sort(Singleton<TaskMgr>.Ins.Sort);
			_uiScrollPanel.Reset(_currentTaskList.Count, InitItemCallBack);
		}

		private void RefreshRed()
		{
			m_day_red.SetActiveBetter(Singleton<TaskMgr>.Ins.DailyListHasAward());
			m_challenge_red.SetActiveBetter(Singleton<TaskMgr>.Ins.ChallengeHasAward());
			m_super_red.SetActiveBetter(Singleton<TaskMgr>.Ins.SuperHasAward());
		}

		private void InitItemCallBack(GameObject cell, int index)
		{
			_003CInitItemCallBack_003Ec__AnonStorey1 _003CInitItemCallBack_003Ec__AnonStorey = new _003CInitItemCallBack_003Ec__AnonStorey1();
			GTaskItem component = cell.GetComponent<GTaskItem>();
			if (_currentTaskList.Count < 1)
			{
				return;
			}
			_003CInitItemCallBack_003Ec__AnonStorey.info = _currentTaskList[index];
			_003CInitItemCallBack_003Ec__AnonStorey.taskCfg = TaskCfg.Get(_003CInitItemCallBack_003Ec__AnonStorey.info.taskTypeId);
			if (_003CInitItemCallBack_003Ec__AnonStorey.taskCfg == null)
			{
				return;
			}
			_003CInitItemCallBack_003Ec__AnonStorey.isJingying = _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.type == 3;
			if (_003CInitItemCallBack_003Ec__AnonStorey.isJingying)
			{
				component.m_bg_jingying.SetActiveBetter(true);
				component.m_bg.SetActiveBetter(false);
			}
			else
			{
				component.m_bg_jingying.SetActiveBetter(false);
				component.m_bg.SetActiveBetter(true);
			}
			component.m_normal.SetActiveBetter(_currentType == TaskPageType.Day);
			component.m_jingying.SetActiveBetter(_003CInitItemCallBack_003Ec__AnonStorey.isJingying);
			component.m_challenge.SetActiveBetter(_currentType == TaskPageType.Challenge && !_003CInitItemCallBack_003Ec__AnonStorey.isJingying);
			View.SetItemSprite(component.m_task_icon, _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.icon);
			View.SetLabelText(component.txt_task_descText, _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.desc);
			int num = 0;
			int dropTableId = ((!_003CInitItemCallBack_003Ec__AnonStorey.info.finishedBefore) ? _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.firstDropId : _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.normalDropId);
			_003CInitItemCallBack_003Ec__AnonStorey.list = Singleton<DropMgr>.Ins.GetDropDetailInfo(dropTableId, (num < 1) ? 1 : num);
			for (int i = 0; i < component.m_awardslist.Count; i++)
			{
				if (i < _003CInitItemCallBack_003Ec__AnonStorey.list.Count)
				{
					component.m_awardslist[i].gameObject.SetActiveBetter(true);
					View.SetItemSprite(component.m_awardslist[i].m_ward_icon, _003CInitItemCallBack_003Ec__AnonStorey.list[i].icon);
					View.SetLabelText(component.m_awardslist[i].txt_award_numText, _003CInitItemCallBack_003Ec__AnonStorey.list[i].num);
				}
				else
				{
					component.m_awardslist[i].gameObject.SetActiveBetter(false);
				}
			}
			int num2 = ((_003CInitItemCallBack_003Ec__AnonStorey.info.progress <= _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax) ? _003CInitItemCallBack_003Ec__AnonStorey.info.progress : _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax);
			View.SetLabelText(component.txt_conditionText, Utils.GetString(9, num2, _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax));
			ClickListener.Get(component.btn_get, "ui_reward").onClick = _003CInitItemCallBack_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(component.btn_go, string.Empty).onClick = _003CInitItemCallBack_003Ec__AnonStorey._003C_003Em__1;
			bool flag = _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.className == "UploadItem";
			ClickListener.Get(component.btn_hand_in, string.Empty).onClick = _003CInitItemCallBack_003Ec__AnonStorey._003C_003Em__2;
			component.btn_get.SetActive(_003CInitItemCallBack_003Ec__AnonStorey.info.progress >= _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax && !_003CInitItemCallBack_003Ec__AnonStorey.info.rewarded);
			component.btn_go.SetActive(_003CInitItemCallBack_003Ec__AnonStorey.info.progress < _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax && !_003CInitItemCallBack_003Ec__AnonStorey.info.rewarded && !flag);
			component.btn_already_get.SetActive(_003CInitItemCallBack_003Ec__AnonStorey.info.progress >= _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax && _003CInitItemCallBack_003Ec__AnonStorey.info.rewarded);
			if (flag)
			{
				int itemId = _003CInitItemCallBack_003Ec__AnonStorey.taskCfg.args[1];
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(itemId);
				component.btn_hand_in.SetActiveBetter(!component.btn_get.activeSelf);
				component.btn_hand_in.GetComponent<Button>().interactable = itemNum + _003CInitItemCallBack_003Ec__AnonStorey.info.progress >= _003CInitItemCallBack_003Ec__AnonStorey.info.progressMax;
			}
			else
			{
				component.btn_hand_in.SetActiveBetter(false);
			}
		}

		private void SetBoxData()
		{
			RefreshTiemCountDown();
			int num = ((Singleton<LadderMgr>.Ins.WeekFinishNum() <= WeekBoxConditionMax) ? Singleton<LadderMgr>.Ins.WeekFinishNum() : WeekBoxConditionMax);
			View.SetLabelText(txt_conditionText, Utils.GetString(9, num, WeekBoxConditionMax));
			btn_already_finish.SetActiveBetter(Singleton<LadderMgr>.Ins.Info.isGetBox);
			btn_no_finish.SetActiveBetter(!Singleton<LadderMgr>.Ins.Info.isGetBox && num < WeekBoxConditionMax);
			btn_get.SetActiveBetter(!Singleton<LadderMgr>.Ins.Info.isGetBox && num >= WeekBoxConditionMax);
		}

		private void OnClickGetBox(GameObject go)
		{
			if (!Singleton<BagMgr>.Ins.IsHasCapacity(31101, 1) || !Singleton<BagMgr>.Ins.IsHasCapacity(31102, 1))
			{
				AlertBox.Show(26);
			}
			else
			{
				Singleton<LadderMgr>.Ins.GetLadderBox();
			}
		}

		private void RefreshTiemCountDown()
		{
			if (_finishTime > 0)
			{
				_finishTime--;
				string text = Utils.CountDownTime(_finishTime);
				View.SetLabelText(txt_remain_timeText, text);
			}
		}

		private IEnumerator Tick1()
		{
			while (true)
			{
				yield return WaitSeconds1;
				RefreshTiemCountDown();
			}
		}

		private void Awake()
		{
			btn_already_finish = base.transform.Find("m_taskbox/GameObject/btn_already_finish").gameObject;
			btn_challenge = base.transform.Find("m_task/Image/radio_btn/btn_challenge").gameObject;
			btn_day = base.transform.Find("m_task/Image/radio_btn/btn_day").gameObject;
			btn_get = base.transform.Find("m_taskbox/GameObject/btn_get").gameObject;
			btn_no_finish = base.transform.Find("m_taskbox/GameObject/btn_no_finish").gameObject;
			btn_super_task = base.transform.Find("m_task/Image/radio_btn/btn_super_task").gameObject;
			m_cell = View.AddComponentIfNotExist<GTaskItem>(base.transform.Find("m_task/Image/scp_task/content/m_cell").gameObject);
			m_challenge_red = base.transform.Find("m_task/Image/radio_btn/btn_challenge/m_challenge_red").gameObject;
			m_day_red = base.transform.Find("m_task/Image/radio_btn/btn_day/m_day_red").gameObject;
			m_super_red = base.transform.Find("m_task/Image/radio_btn/btn_super_task/m_super_red").gameObject;
			m_task = base.transform.Find("m_task").gameObject;
			m_task_normal = base.transform.Find("m_taskbox/m_task_normal").gameObject;
			m_task_senior = base.transform.Find("m_taskbox/m_task_senior").gameObject;
			m_taskbox = base.transform.Find("m_taskbox").gameObject;
			scp_task = base.transform.Find("m_task/Image/scp_task").gameObject;
			txt_condition = base.transform.Find("m_taskbox/GameObject/RawImage/txt_condition").gameObject;
			txt_conditionText = txt_condition.GetComponent<Text>();
			txt_off = base.transform.Find("m_task/Image/radio_btn/btn_super_task/Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_off_0 = base.transform.Find("m_task/Image/radio_btn/btn_challenge/Off/txt_off").gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_off_1 = base.transform.Find("m_task/Image/radio_btn/btn_day/Off/txt_off").gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on = base.transform.Find("m_task/Image/radio_btn/btn_super_task/On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_on_0 = base.transform.Find("m_task/Image/radio_btn/btn_day/On/txt_on").gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = base.transform.Find("m_task/Image/radio_btn/btn_challenge/On/txt_on").gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_remain_time = base.transform.Find("m_taskbox/GameObject/RawImage/Image/txt_remain_time").gameObject;
			txt_remain_timeText = txt_remain_time.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			ChangeType(TaskPageType.Day);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__1(GameObject go)
		{
			ChangeType(TaskPageType.Challenge);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			ChangeType(TaskPageType.SuperTask);
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__3(GameObject go)
		{
			ViewMgr.Ins.ShowView<LadderBoxPanel>(31101, false);
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__4(GameObject go)
		{
			ViewMgr.Ins.ShowView<LadderBoxPanel>(31102, false);
		}
	}
}
