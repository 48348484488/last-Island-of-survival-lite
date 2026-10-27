using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.drop.scmsg;

namespace SC.UI
{
	public class ActivityMonthSignInPage : MonoBehaviour, IActivityPage
	{
		[CompilerGenerated]
		private sealed class _003COnSignInChange_003Ec__AnonStorey0
		{
			internal bool isCanSignIn;

			internal List<DropMgr.DropDesInfo> dropInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				if (isCanSignIn && !ActivityViewTool.BagCapacityEnough(dropInfo))
				{
					AlertBox.Show(26);
				}
				else if (isCanSignIn)
				{
					Singleton<ActivityMgr>.Ins.SendMonthSignInMsg();
				}
				else
				{
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(dropInfo[0].itemId));
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillMonthSignInCell_003Ec__AnonStorey1
		{
			internal bool isCanSignIn;

			internal List<DropMgr.DropDesInfo> dropInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				if (isCanSignIn && !ActivityViewTool.BagCapacityEnough(dropInfo))
				{
					AlertBox.Show(26);
				}
				else if (isCanSignIn)
				{
					Singleton<ActivityMgr>.Ins.SendMonthSignInMsg();
				}
				else
				{
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(dropInfo[0].itemId));
				}
			}
		}

		private List<MonthSignInCfg> _allMonthSignInInfoList;

		private UIScrollPanel _monthSignInScrollPanel;

		public GActivityMonthSignInCell m_cell;

		public GameObject scp_month_sign_in;

		public GameObject txt_time;

		public Text txt_timeText;

		public GameObject txt_total_day;

		public Text txt_total_dayText;

		public object context;

		public GameObject ThisGo { get; private set; }

		public void OnInit()
		{
			txt_time.SetActiveBetter(false);
			ThisGo = base.gameObject;
			_allMonthSignInInfoList = MonthSignInCfg.GetAllList();
			_monthSignInScrollPanel = scp_month_sign_in.GetComponent<UIScrollPanel>();
		}

		public void OnShow(object param)
		{
			SetTotalSignInDay();
			UpdateMonthSignInScroll();
			ActivityEvent.OnMonthSignInChangeAction = (Action<DropDetail>)Delegate.Combine(ActivityEvent.OnMonthSignInChangeAction, new Action<DropDetail>(OnMonthSignInChange));
		}

		public void OnHide()
		{
			ActivityEvent.OnMonthSignInChangeAction = (Action<DropDetail>)Delegate.Remove(ActivityEvent.OnMonthSignInChangeAction, new Action<DropDetail>(OnMonthSignInChange));
		}

		private void OnMonthSignInChange(DropDetail details)
		{
			Singleton<GainMgr>.Ins.Add(details);
			SetTotalSignInDay();
			_monthSignInScrollPanel.UpdateAllCell(OnSignInChange);
		}

		private void OnSignInChange(GameObject go, int index)
		{
			_003COnSignInChange_003Ec__AnonStorey0 _003COnSignInChange_003Ec__AnonStorey = new _003COnSignInChange_003Ec__AnonStorey0();
			GActivityMonthSignInCell component = go.GetComponent<GActivityMonthSignInCell>();
			MonthSignInCfg monthSignInCfg = _allMonthSignInInfoList[index];
			int id = monthSignInCfg.id;
			bool trueOrFalse = Singleton<ActivityMgr>.Ins.IsAlreadyGotMonthSignIn(id);
			_003COnSignInChange_003Ec__AnonStorey.isCanSignIn = Singleton<ActivityMgr>.Ins.IsCanSignIn(id);
			component.m_already_got.SetActiveBetter(trueOrFalse);
			component.m_sign_in.SetActiveBetter(_003COnSignInChange_003Ec__AnonStorey.isCanSignIn);
			component.m_buQian.SetActiveBetter(false);
			_003COnSignInChange_003Ec__AnonStorey.dropInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(monthSignInCfg.dropId);
			ClickListener.Get(go, string.Empty).onClick = _003COnSignInChange_003Ec__AnonStorey._003C_003Em__0;
		}

		private void SetTotalSignInDay()
		{
			View.SetLabelText(txt_total_dayText, Utils.GetString(9, Singleton<ActivityMgr>.Ins.AlreadySignInDayCount, _allMonthSignInInfoList.Count));
		}

		private void UpdateMonthSignInScroll()
		{
			_monthSignInScrollPanel.Reset(_allMonthSignInInfoList.Count, FillMonthSignInCell);
		}

		private void FillMonthSignInCell(GameObject go, int index)
		{
			_003CFillMonthSignInCell_003Ec__AnonStorey1 _003CFillMonthSignInCell_003Ec__AnonStorey = new _003CFillMonthSignInCell_003Ec__AnonStorey1();
			GActivityMonthSignInCell component = go.GetComponent<GActivityMonthSignInCell>();
			MonthSignInCfg monthSignInCfg = _allMonthSignInInfoList[index];
			int id = monthSignInCfg.id;
			bool trueOrFalse = Singleton<ActivityMgr>.Ins.IsAlreadyGotMonthSignIn(id);
			_003CFillMonthSignInCell_003Ec__AnonStorey.isCanSignIn = Singleton<ActivityMgr>.Ins.IsCanSignIn(id);
			component.m_already_got.SetActiveBetter(trueOrFalse);
			component.m_sign_in.SetActiveBetter(_003CFillMonthSignInCell_003Ec__AnonStorey.isCanSignIn);
			component.m_buQian.SetActiveBetter(false);
			_003CFillMonthSignInCell_003Ec__AnonStorey.dropInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(monthSignInCfg.dropId);
			component.m_bind.SetActiveBetter(_003CFillMonthSignInCell_003Ec__AnonStorey.dropInfo[0].isBinding);
			View.SetItemSprite(component.m_icon, _003CFillMonthSignInCell_003Ec__AnonStorey.dropInfo[0].icon);
			View.SetLabelText(component.txt_dayText, id);
			View.SetLabelText(component.txt_numText, Utils.GetString(281, _003CFillMonthSignInCell_003Ec__AnonStorey.dropInfo[0].num));
			ClickListener.Get(go, string.Empty).onClick = _003CFillMonthSignInCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void Awake()
		{
			m_cell = View.AddComponentIfNotExist<GActivityMonthSignInCell>(base.transform.Find("scp_month_sign_in/content/m_cell").gameObject);
			scp_month_sign_in = base.transform.Find("scp_month_sign_in").gameObject;
			txt_time = base.transform.Find("GameObject/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
			txt_total_day = base.transform.Find("GameObject (2)/GameObject/Image/txt_total_day").gameObject;
			txt_total_dayText = txt_total_day.GetComponent<Text>();
		}
	}
}
