using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class ActivityPanel : View
	{
		private enum TabTypeEnum
		{
			None = 0,
			SevenDay = 1,
			MonthSignIn = 2,
			DailyGift = 3,
			FirstCharge = 4
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal TabTypeEnum tab;

			internal ActivityPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this.OnClickBtn(tab, null);
			}
		}

		private Dictionary<TabTypeEnum, GameObject> _dicTab2Btn = new Dictionary<TabTypeEnum, GameObject>();

		private Dictionary<TabTypeEnum, IActivityPage> _dicTab2Page = new Dictionary<TabTypeEnum, IActivityPage>();

		private TabTypeEnum _curTab;

		private bool isAlreadyResetQueue;

		private ActivitySevenDayPage m_seven_day;

		private ActivityMonthSignInPage m_month_sign_in;

		private ActivityDailyGiftPage m_daily_gift;

		private ActivityFirstChargePage m_first_charge;

		private GameObject m_btns;

		private GameObject btn_first_charge;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_red_dot_charge;

		private GameObject btn_daily_gift;

		private GameObject m_red_dot_daily;

		private GameObject btn_seven_day;

		private GameObject m_red_dot_seven_day;

		private GameObject btn_month_sign_in;

		private GameObject m_red_dot_month;

		private GameObject btn_back;

		private GameObject m_bg;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		protected override void onInit()
		{
			base.onInit();
			m_seven_day.gameObject.SetActiveBetter(false);
			m_month_sign_in.gameObject.SetActiveBetter(false);
			m_daily_gift.gameObject.SetActiveBetter(false);
			m_first_charge.gameObject.SetActiveBetter(false);
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			_dicTab2Btn[TabTypeEnum.SevenDay] = btn_seven_day;
			_dicTab2Page[TabTypeEnum.SevenDay] = m_seven_day;
			_dicTab2Btn[TabTypeEnum.MonthSignIn] = btn_month_sign_in;
			_dicTab2Page[TabTypeEnum.MonthSignIn] = m_month_sign_in;
			_dicTab2Btn[TabTypeEnum.DailyGift] = btn_daily_gift;
			_dicTab2Page[TabTypeEnum.DailyGift] = m_daily_gift;
			_dicTab2Btn[TabTypeEnum.FirstCharge] = btn_first_charge;
			_dicTab2Page[TabTypeEnum.FirstCharge] = m_first_charge;
			for (int i = 0; i < Enum.GetNames(typeof(TabTypeEnum)).Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.tab = (TabTypeEnum)i;
				GameObject value;
				if (_dicTab2Btn.TryGetValue(_003ConInit_003Ec__AnonStorey.tab, out value))
				{
					ClickListener.Get(value, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
					IActivityPage value2;
					if (_dicTab2Page.TryGetValue(_003ConInit_003Ec__AnonStorey.tab, out value2))
					{
						value2.OnInit();
					}
				}
			}
		}

		private void OnClickBtn(TabTypeEnum tabType, object param)
		{
			if (_curTab != tabType)
			{
				IActivityPage value;
				if (_dicTab2Page.TryGetValue(_curTab, out value) && value.ThisGo.activeSelf)
				{
					value.ThisGo.SetActive(false);
					value.OnHide();
				}
				_curTab = tabType;
				if (_dicTab2Page.TryGetValue(tabType, out value) && !value.ThisGo.activeSelf)
				{
					RadioButton.ChooseBtn(_dicTab2Btn[tabType]);
					value.ThisGo.SetActive(true);
					value.OnShow(param);
				}
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			if (!isAlreadyResetQueue)
			{
				m_btns.SetActiveBetter(false);
				m_btns.SetActiveBetter(true);
				m_bg.SetActiveBetter(false);
				m_bg.SetActiveBetter(true);
			}
			_curTab = TabTypeEnum.None;
			OnClickBtn(TabTypeEnum.SevenDay, null);
			UpdateRedDot();
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Combine(ActivityEvent.UpdateActivityRedDot, new Action(UpdateRedDot));
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			IActivityPage value;
			if (_dicTab2Page.TryGetValue(_curTab, out value) && value.ThisGo.activeSelf)
			{
				value.ThisGo.SetActive(false);
				value.OnHide();
			}
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Remove(ActivityEvent.UpdateActivityRedDot, new Action(UpdateRedDot));
		}

		private void UpdateRedDot()
		{
			m_red_dot_charge.SetActiveBetter(Singleton<ActivityMgr>.Ins.IsShowFirstChargeRedDot());
			m_red_dot_daily.SetActiveBetter(Singleton<ActivityMgr>.Ins.IsShowDailyGiftRedDot());
			m_red_dot_month.SetActiveBetter(Singleton<ActivityMgr>.Ins.IsShowMonthSignInRedDot());
			m_red_dot_seven_day.SetActiveBetter(Singleton<ActivityMgr>.Ins.IsShowSevenDayRedDot());
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_seven_day = View.AddComponentIfNotExist<ActivitySevenDayPage>(component.GameObjects[0].gameObject);
			m_month_sign_in = View.AddComponentIfNotExist<ActivityMonthSignInPage>(component.GameObjects[1].gameObject);
			m_daily_gift = View.AddComponentIfNotExist<ActivityDailyGiftPage>(component.GameObjects[2].gameObject);
			m_first_charge = View.AddComponentIfNotExist<ActivityFirstChargePage>(component.GameObjects[3].gameObject);
			m_btns = component.GameObjects[4].gameObject;
			btn_first_charge = component.GameObjects[5].gameObject;
			txt_on = component.GameObjects[6].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[7].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_red_dot_charge = component.GameObjects[8].gameObject;
			btn_daily_gift = component.GameObjects[9].gameObject;
			m_red_dot_daily = component.GameObjects[10].gameObject;
			btn_seven_day = component.GameObjects[11].gameObject;
			m_red_dot_seven_day = component.GameObjects[12].gameObject;
			btn_month_sign_in = component.GameObjects[13].gameObject;
			m_red_dot_month = component.GameObjects[14].gameObject;
			btn_back = component.GameObjects[15].gameObject;
			m_bg = component.GameObjects[16].gameObject;
			txt_on_0 = component.GameObjects[17].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[18].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[19].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_1 = component.GameObjects[20].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[21].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_2 = component.GameObjects[22].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}
