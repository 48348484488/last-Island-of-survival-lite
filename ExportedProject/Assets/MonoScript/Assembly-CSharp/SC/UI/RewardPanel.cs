using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.task.scmsg;

namespace SC.UI
{
	public class RewardPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillRewardItemCell_003Ec__AnonStorey1
		{
			internal DropMgr.DropDesInfo info;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(info.itemId));
			}
		}

		private SGetTaskReward _msg;

		private GameObject m_recording;

		private List<GRewardShowItemCell> m_itemslist = new List<GRewardShowItemCell>();

		private GameObject[] m_items;

		private GameObject m_itemsObj;

		private GameObject btn_confirm;

		private GameObject txt_task_name;

		private Text txt_task_nameText;

		private GameObject m_bg;

		protected override void onInit()
		{
			UIEventListener.Get(btn_confirm, string.Empty).onClick = OnClickOk;
		}

		private IEnumerator WaitHide()
		{
			yield return Utils.WaitForSeconds(0.03f);
			Hide();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(352);
			_msg = param as SGetTaskReward;
			if (_msg == null)
			{
				StartCoroutine(WaitHide());
			}
			else
			{
				ShowItem(_msg);
			}
		}

		private void ShowItem(SGetTaskReward msg)
		{
			Animation();
			TaskInfo taskDataInfo;
			if (!Singleton<GuideMgr>.Ins.GetTaskInfoByInsId(msg.taskId, out taskDataInfo))
			{
				taskDataInfo = Singleton<TaskMgr>.Ins.GetTaskById(msg.taskId);
			}
			if (taskDataInfo != null)
			{
				TaskCfg taskCfg = TaskCfg.Get(taskDataInfo.taskTypeId);
				if (taskCfg != null)
				{
					View.SetLabelText(txt_task_nameText, Utils.GetString(364, taskCfg.name));
				}
				else
				{
					View.SetLabelText(txt_task_nameText, Utils.GetString(364, string.Empty));
				}
			}
			List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(msg.dropDetail);
			int i = 0;
			for (int num = m_items.Length; i < num; i++)
			{
				if (dropDetailInfo.Count > i)
				{
					m_items[i].SetActiveBetter(true);
					FillRewardItemCell(m_itemslist[i], dropDetailInfo[i]);
				}
				else
				{
					m_items[i].SetActiveBetter(false);
				}
			}
		}

		private void FillRewardItemCell(GRewardShowItemCell cell, DropMgr.DropDesInfo info)
		{
			_003CFillRewardItemCell_003Ec__AnonStorey1 _003CFillRewardItemCell_003Ec__AnonStorey = new _003CFillRewardItemCell_003Ec__AnonStorey1();
			_003CFillRewardItemCell_003Ec__AnonStorey.info = info;
			cell.m_bind.SetActiveBetter(_003CFillRewardItemCell_003Ec__AnonStorey.info.isBinding);
			View.SetItemSprite(cell.m_icon, _003CFillRewardItemCell_003Ec__AnonStorey.info.icon);
			View.SetLabelText(cell.txt_numText, _003CFillRewardItemCell_003Ec__AnonStorey.info.num);
			ClickListener.Get(cell.m_icon, string.Empty).onClick = _003CFillRewardItemCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void OnClickOk(GameObject go)
		{
			Hide();
			Singleton<RewardShowMgr>.Ins.ClosePanel(_msg);
		}

		protected override void onDestroy()
		{
			Singleton<RewardShowMgr>.Ins.ClearPanel();
		}

		private void Animation()
		{
			m_recording.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			Tweener tweener = m_recording.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_recording = component.GameObjects[0].gameObject;
			m_items = component.GameObjects[1].gameObject.GetComponent<UIGameObjectList>().objects;
			m_itemsObj = component.GameObjects[1].gameObject;
			if (m_itemslist.Count <= 0)
			{
				for (int i = 0; i < m_items.Length; i++)
				{
					m_itemslist.Add(View.AddComponentIfNotExist<GRewardShowItemCell>(m_items[i].gameObject));
				}
			}
			btn_confirm = component.GameObjects[2].gameObject;
			txt_task_name = component.GameObjects[3].gameObject;
			txt_task_nameText = txt_task_name.GetComponent<Text>();
			m_bg = component.GameObjects[4].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
