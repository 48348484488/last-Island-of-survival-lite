using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.task.scmsg;

namespace SC.UI
{
	public class LitterTaskPage : MonoBehaviour, ITeamTaskPage
	{
		public GameObject m_content;

		public List<GLittleTaskItem> m_taskslist = new List<GLittleTaskItem>();

		public GameObject[] m_tasks;

		public GameObject m_tasksObj;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		public void OnInit()
		{
			ClickListener clickListener = ClickListener.Get(m_content, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__0;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
		}

		public void OnShow(object param)
		{
			TaskEvent.RefreshTask = (Utils.VoidDelegate)Delegate.Combine(TaskEvent.RefreshTask, new Utils.VoidDelegate(RefreshData));
			RefreshData();
		}

		public void OnHide()
		{
			TaskEvent.RefreshTask = (Utils.VoidDelegate)Delegate.Remove(TaskEvent.RefreshTask, new Utils.VoidDelegate(RefreshData));
		}

		private void RefreshData()
		{
			Singleton<TaskMgr>.Ins.TaskDailyList.Sort(Singleton<TaskMgr>.Ins.Sort);
			int num = ((Singleton<TaskMgr>.Ins.TaskDailyList.Count <= m_tasks.Length) ? Singleton<TaskMgr>.Ins.TaskDailyList.Count : m_tasks.Length);
			for (int i = 0; i < m_tasks.Length; i++)
			{
				if (i < Singleton<TaskMgr>.Ins.TaskDailyList.Count)
				{
					m_tasks[i].SetActiveBetter(true);
					int index = i;
					InitItemCallBack(m_taskslist[i], index);
				}
				else
				{
					m_tasks[i].SetActiveBetter(false);
				}
			}
		}

		private void InitItemCallBack(GLittleTaskItem gLittleTaskItem, int index)
		{
			TaskInfo taskInfo = Singleton<TaskMgr>.Ins.TaskDailyList[index];
			TaskCfg taskCfg = TaskCfg.Get(taskInfo.taskTypeId);
			if (taskCfg != null)
			{
				View.SetLabelText(gLittleTaskItem.txt_nameText, taskCfg.name);
				int num = ((taskInfo.progress <= taskInfo.progressMax) ? taskInfo.progress : taskInfo.progressMax);
				gLittleTaskItem.m_finish.SetActiveBetter(num >= taskInfo.progressMax);
				View.SetLabelText(gLittleTaskItem.txt_progressText, Utils.GetString(9, num, taskInfo.progressMax));
				View.SetLabelText(gLittleTaskItem.txt_descText, taskCfg.desc);
				View.SetLabelText(gLittleTaskItem.txt_desc_conditionText, taskCfg.executeDesc);
			}
		}

		private void Awake()
		{
			m_content = base.transform.Find("GameObject/Image (1)/m_tasks/m_content").gameObject;
			m_tasks = base.transform.Find("GameObject/Image (1)/m_tasks").gameObject.GetComponent<UIGameObjectList>().objects;
			m_tasksObj = base.transform.Find("GameObject/Image (1)/m_tasks").gameObject;
			if (m_taskslist.Count <= 0)
			{
				for (int i = 0; i < m_tasks.Length; i++)
				{
					m_taskslist.Add(View.AddComponentIfNotExist<GLittleTaskItem>(m_tasks[i].gameObject));
				}
			}
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__0(GameObject go)
		{
			ViewMgr.Ins.ShowView<LadderTaskPanel>(LadderTaskPanel.Tab2PageEnum.Task);
		}
	}
}
