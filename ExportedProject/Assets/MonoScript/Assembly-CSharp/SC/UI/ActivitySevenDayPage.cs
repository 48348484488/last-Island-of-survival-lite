using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.drop.scmsg;

namespace SC.UI
{
	public class ActivitySevenDayPage : MonoBehaviour, IActivityPage
	{
		[CompilerGenerated]
		private sealed class _003CFillSevenDayCell_003Ec__AnonStorey0
		{
			internal bool isAlreadyGot;

			internal List<DropMgr.DropDesInfo> infoList;

			internal int cfgId;

			internal void _003C_003Em__0(GameObject o)
			{
				if (!isAlreadyGot)
				{
					if (!ActivityViewTool.BagCapacityEnough(infoList))
					{
						AlertBox.Show(26);
					}
					else
					{
						Singleton<ActivityMgr>.Ins.SendGetSevenDayMsg(cfgId);
					}
				}
			}
		}

		private List<WeekSignInCfg> _allSevenDayInfoList;

		private UIScrollPanel _sevenDayScrollPanel;

		public GActivitySevenDayCell m_cell;

		public GameObject scp_seven_day;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		public GameObject ThisGo { get; private set; }

		public void OnInit()
		{
			txt_time.SetActiveBetter(false);
			ThisGo = base.gameObject;
			_allSevenDayInfoList = WeekSignInCfg.GetAllList();
			_sevenDayScrollPanel = scp_seven_day.GetComponent<UIScrollPanel>();
		}

		public void OnShow(object param)
		{
			UpdateSevenDayScroll(false);
			ActivityEvent.UpdateSevenDayBtnAction = (Action<DropDetail>)Delegate.Combine(ActivityEvent.UpdateSevenDayBtnAction, new Action<DropDetail>(UpdateSevenDayBtn));
		}

		public void OnHide()
		{
			ActivityEvent.UpdateSevenDayBtnAction = (Action<DropDetail>)Delegate.Remove(ActivityEvent.UpdateSevenDayBtnAction, new Action<DropDetail>(UpdateSevenDayBtn));
		}

		private void UpdateSevenDayBtn(DropDetail details)
		{
			Singleton<GainMgr>.Ins.Add(details);
			_sevenDayScrollPanel.UpdateAllCell(ResetSevenDayBtnCell);
		}

		private void ResetSevenDayBtnCell(GameObject go, int index)
		{
			GActivitySevenDayCell component = go.GetComponent<GActivitySevenDayCell>();
			WeekSignInCfg weekSignInCfg = _allSevenDayInfoList[index];
			int id = weekSignInCfg.id;
			bool flag = Singleton<ActivityMgr>.Ins.IsCanGetSevenDay(id);
			bool flag2 = Singleton<ActivityMgr>.Ins.IsAlreadyGotSevenDay(id);
			component.btn_get.SetActiveBetter(flag && !flag2);
			component.btn_already_got.SetActiveBetter(flag && flag2);
		}

		private void UpdateSevenDayScroll(bool isNoPos)
		{
			if (isNoPos)
			{
				_sevenDayScrollPanel.ResetNoPos(_allSevenDayInfoList.Count, FillSevenDayCell);
			}
			else
			{
				_sevenDayScrollPanel.Reset(_allSevenDayInfoList.Count, FillSevenDayCell);
			}
		}

		private void FillSevenDayCell(GameObject go, int index)
		{
			_003CFillSevenDayCell_003Ec__AnonStorey0 _003CFillSevenDayCell_003Ec__AnonStorey = new _003CFillSevenDayCell_003Ec__AnonStorey0();
			GActivitySevenDayCell component = go.GetComponent<GActivitySevenDayCell>();
			WeekSignInCfg weekSignInCfg = _allSevenDayInfoList[index];
			_003CFillSevenDayCell_003Ec__AnonStorey.cfgId = weekSignInCfg.id;
			_003CFillSevenDayCell_003Ec__AnonStorey.infoList = Singleton<DropMgr>.Ins.GetDropDetailInfo(weekSignInCfg.dropId);
			ActivityViewTool.SetActivitySevenDayItems(component.m_item_parent, _003CFillSevenDayCell_003Ec__AnonStorey.infoList);
			View.SetLabelText(component.txt_nameText, weekSignInCfg.name);
			bool flag = Singleton<ActivityMgr>.Ins.IsCanGetSevenDay(_003CFillSevenDayCell_003Ec__AnonStorey.cfgId);
			_003CFillSevenDayCell_003Ec__AnonStorey.isAlreadyGot = Singleton<ActivityMgr>.Ins.IsAlreadyGotSevenDay(_003CFillSevenDayCell_003Ec__AnonStorey.cfgId);
			component.btn_get.SetActiveBetter(flag && !_003CFillSevenDayCell_003Ec__AnonStorey.isAlreadyGot);
			component.btn_already_got.SetActiveBetter(flag && _003CFillSevenDayCell_003Ec__AnonStorey.isAlreadyGot);
			ClickListener.Get(component.btn_get, string.Empty).onClick = _003CFillSevenDayCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void Awake()
		{
			m_cell = View.AddComponentIfNotExist<GActivitySevenDayCell>(base.transform.Find("scp_seven_day/content/m_cell").gameObject);
			scp_seven_day = base.transform.Find("scp_seven_day").gameObject;
			txt_time = base.transform.Find("GameObject/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
