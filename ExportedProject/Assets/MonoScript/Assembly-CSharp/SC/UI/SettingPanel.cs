using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class SettingPanel : View
	{
		public enum SettingPage
		{
			None = 0,
			Basic = 1,
			Operation = 2,
			Pickup = 3,
			Drive = 4,
			Trusteeship = 5,
			Pic = 6
		}

		private SettingPage _settingPage;

		private SettingGBasicPage BasicPage;

		private SettingGPicPage PicPage;

		private SettingTrusteeshipPage TrusteeshipPage;

		public static readonly string CONTROL_CAR_BY_JOYSTICK = "CONTROL_CAR_BY_JOYSTICK";

		private readonly List<Utils.VoidDelegate> _onShowDelegates = new List<Utils.VoidDelegate>();

		private readonly List<Utils.VoidDelegate> _onHideDelegates = new List<Utils.VoidDelegate>();

		private readonly Dictionary<SettingPage, GameObject> _dicPageType2Btn = new Dictionary<SettingPage, GameObject>();

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject txt_on;

		private Text txt_onText;

		private SettingGBasicPage m_basic_page;

		private GameObject btn_normal;

		private GameObject btn_trusteeship;

		private SettingTrusteeshipPage m_trusteeship;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_pic;

		private SettingGPicPage m_pic;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		protected override void onInit()
		{
			ClickListener.Get(btn_back, "ui_close").onClick = OnClose;
			BasicPage = m_basic_page;
			TrusteeshipPage = m_trusteeship;
			BasicPage.OnInit();
			TrusteeshipPage.OnInit();
			PicPage = m_pic;
			PicPage.OnInit();
			RegistDelegate(BasicPage.OnShow, BasicPage.OnHide);
			RegistDelegate(TrusteeshipPage.OnShow, TrusteeshipPage.OnHide);
			RegistDelegate(PicPage.OnShow, PicPage.OnHide);
			_dicPageType2Btn[SettingPage.Basic] = btn_normal;
			_dicPageType2Btn[SettingPage.Trusteeship] = btn_trusteeship;
			_dicPageType2Btn[SettingPage.Pic] = btn_pic;
			ClickListener.Get(btn_normal, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_trusteeship, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_pic, string.Empty).onClick = _003ConInit_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			if (param == null)
			{
				SetPage(SettingPage.Basic);
			}
			else if (param is SettingPage)
			{
				SetPage((SettingPage)param);
			}
			else
			{
				SetPage(SettingPage.Basic);
			}
			GameObject value;
			if (!_dicPageType2Btn.TryGetValue(_settingPage, out value))
			{
				value = btn_normal;
			}
			RadioButton.ChooseBtn(value);
			for (int num = _onShowDelegates.Count - 1; num >= 0; num--)
			{
				_onShowDelegates[num]();
			}
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			OnMoneyChange();
		}

		protected override void onHide(string childView = null)
		{
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			_settingPage = SettingPage.None;
			for (int num = _onHideDelegates.Count - 1; num >= 0; num--)
			{
				_onHideDelegates[num]();
			}
		}

		protected override void onDestroy()
		{
		}

		private void SetPage(SettingPage settingPage)
		{
			if (_settingPage != settingPage)
			{
				_settingPage = settingPage;
				m_basic_page.gameObject.SetActive(_settingPage == SettingPage.Basic);
				m_trusteeship.gameObject.SetActive(_settingPage == SettingPage.Trusteeship);
				m_pic.gameObject.SetActive(_settingPage == SettingPage.Pic);
			}
		}

		public void OnClose(GameObject go)
		{
			Utils.TriggerEvent(SettingEvent.ExitSettingPanelDelegate);
			Hide();
		}

		public void RegistDelegate(Utils.VoidDelegate onShowDel, Utils.VoidDelegate onHideDel)
		{
			_onShowDelegates.Add(onShowDel);
			_onHideDelegates.Add(onHideDel);
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_blood = component.GameObjects[1].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[2].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[3].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			m_basic_page = View.AddComponentIfNotExist<SettingGBasicPage>(component.GameObjects[5].gameObject);
			btn_normal = component.GameObjects[6].gameObject;
			btn_trusteeship = component.GameObjects[7].gameObject;
			m_trusteeship = View.AddComponentIfNotExist<SettingTrusteeshipPage>(component.GameObjects[8].gameObject);
			txt_off = component.GameObjects[9].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_pic = component.GameObjects[10].gameObject;
			m_pic = View.AddComponentIfNotExist<SettingGPicPage>(component.GameObjects[11].gameObject);
			txt_on_0 = component.GameObjects[12].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[13].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_0 = component.GameObjects[14].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_2 = component.GameObjects[15].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_1 = component.GameObjects[16].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			SetPage(SettingPage.Basic);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			SetPage(SettingPage.Trusteeship);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			SetPage(SettingPage.Pic);
		}
	}
}
