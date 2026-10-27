using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.ladder.scmsg;

namespace SC.UI
{
	public class LadderTaskPanel : View
	{
		public enum Tab2PageEnum
		{
			Empty = 0,
			Ladder = 1,
			Task = 2
		}

		[CompilerGenerated]
		private sealed class _003CAttachPage2Btn_003Ec__AnonStorey0
		{
			internal Tab2PageEnum tabID;

			internal LadderTaskPanel _0024this;

			internal void _003C_003Em__0(bool isOn)
			{
				if (isOn)
				{
					_0024this._currentPage = tabID;
					if (!_0024this._dicPages[tabID].IsShow)
					{
						_0024this._dicPages[tabID].OnShow();
					}
				}
				else if (_0024this._dicPages[tabID].IsShow)
				{
					_0024this._dicPages[tabID].OnHide();
				}
			}
		}

		private readonly Dictionary<Tab2PageEnum, RadioButton> _dicTabs = new Dictionary<Tab2PageEnum, RadioButton>();

		private readonly Dictionary<Tab2PageEnum, IBagAndBuildPage> _dicPages = new Dictionary<Tab2PageEnum, IBagAndBuildPage>();

		private Tab2PageEnum _currentPage;

		private LadderInfo _info;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_ladder;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_ladder_red;

		private GameObject btn_task;

		private GameObject m_task_red;

		private GLadderPage m_Ladder_page;

		private GTaskPage m_task_page;

		private GameObject m_icon;

		private GameObject txt_level;

		private Text txt_levelText;

		private GameObject btn_buy_ladder_level;

		private GameObject txt_exp;

		private Text txt_expText;

		private GameObject txt_ladder_desc;

		private Text txt_ladder_descText;

		private GameObject txt_ladder_time;

		private Text txt_ladder_timeText;

		private GameObject m_exp_sld;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			m_Ladder_page.gameObject.SetActiveBetter(false);
			m_task_page.gameObject.SetActiveBetter(false);
			_dicTabs.Add(Tab2PageEnum.Ladder, btn_ladder.GetComponent<RadioButton>());
			_dicPages.Add(Tab2PageEnum.Ladder, m_Ladder_page);
			_dicTabs.Add(Tab2PageEnum.Task, btn_task.GetComponent<RadioButton>());
			_dicPages.Add(Tab2PageEnum.Task, m_task_page);
			foreach (RadioButton value in _dicTabs.Values)
			{
				value.isChecked = false;
			}
			for (byte b = 1; b < Enum.GetNames(typeof(Tab2PageEnum)).Length; b = (byte)(b + 1))
			{
				Tab2PageEnum tab2PageEnum = (Tab2PageEnum)b;
				try
				{
					if (_dicPages.ContainsKey(tab2PageEnum))
					{
						_dicPages[tab2PageEnum].OnInit();
					}
				}
				catch
				{
					Debug.LogError("no excute init()");
				}
				if (_currentPage == Tab2PageEnum.Empty)
				{
					_currentPage = tab2PageEnum;
				}
				AttachPage2Btn(tab2PageEnum);
			}
			ClickListener.Get(btn_back, "ui_close").onClick = _003ConInit_003Em__0;
			_info = Singleton<LadderMgr>.Ins.Info;
			ClickListener.Get(btn_buy_ladder_level, string.Empty).onClick = OnClickBuyLadderLevel;
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			if (param == null)
			{
				_currentPage = Tab2PageEnum.Ladder;
			}
			else
			{
				try
				{
					_currentPage = (Tab2PageEnum)param;
				}
				catch (Exception)
				{
					_currentPage = Tab2PageEnum.Ladder;
				}
			}
			OnMoneyChange();
			if (_currentPage > Tab2PageEnum.Empty)
			{
				RadioButton.ChooseBtn(_dicTabs[_currentPage].gameObject);
			}
			m_task_red.SetActiveBetter(false);
			btn_ladder_red.SetActiveBetter(false);
			SetLadderBasicInfo();
			RefreshTaskRed();
			RefreshLadderRed();
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevel));
			TaskEvent.RefreshTaskRed = (Utils.VoidDelegate)Delegate.Combine(TaskEvent.RefreshTaskRed, new Utils.VoidDelegate(RefreshTaskRed));
			LadderEvent.RefreshLadderRed = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLadderRed, new Utils.VoidDelegate(RefreshLadderRed));
		}

		private void RefreshLevel()
		{
			SetLadderBasicInfo();
		}

		private void SetLadderBasicInfo()
		{
			View.SetLabelText(txt_levelText, Utils.GetString(149, _info.level));
			LadderCfg ladderCfg = LadderCfg.Get(_info.level);
			int exp = ladderCfg.exp;
			View.SetSlider(m_exp_sld, (float)_info.exp * 1f / (float)exp);
			if (exp > 0)
			{
				View.SetLabelText(txt_expText, Utils.GetString(9, _info.exp, exp));
				btn_buy_ladder_level.SetActive(true);
			}
			else
			{
				btn_buy_ladder_level.SetActive(false);
				View.SetLabelText(txt_expText, Utils.GetString(349));
				View.SetSlider(m_exp_sld, 1f);
			}
		}

		private void OnClickBuyLadderLevel(GameObject go)
		{
			ViewMgr.Ins.ShowView<LadderBuyLevelPanel>(null, false);
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			_dicTabs[_currentPage].isChecked = false;
			_dicPages[_currentPage].OnHide();
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevel));
			TaskEvent.RefreshTaskRed = (Utils.VoidDelegate)Delegate.Remove(TaskEvent.RefreshTaskRed, new Utils.VoidDelegate(RefreshTaskRed));
			LadderEvent.RefreshLadderRed = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLadderRed, new Utils.VoidDelegate(RefreshLadderRed));
		}

		private void AttachPage2Btn(Tab2PageEnum tabID)
		{
			_003CAttachPage2Btn_003Ec__AnonStorey0 _003CAttachPage2Btn_003Ec__AnonStorey = new _003CAttachPage2Btn_003Ec__AnonStorey0();
			_003CAttachPage2Btn_003Ec__AnonStorey.tabID = tabID;
			_003CAttachPage2Btn_003Ec__AnonStorey._0024this = this;
			_dicTabs[_003CAttachPage2Btn_003Ec__AnonStorey.tabID].OnValueChanged = _003CAttachPage2Btn_003Ec__AnonStorey._003C_003Em__0;
		}

		private void RefreshLadderRed()
		{
			btn_ladder_red.SetActive(Singleton<LadderMgr>.Ins.IsShowRedDot());
		}

		private void RefreshTaskRed()
		{
			m_task_red.SetActive(Singleton<TaskMgr>.Ins.IsShowRedDot() || Singleton<LadderMgr>.Ins.IsShowBoxRedDot());
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
			btn_ladder = component.GameObjects[4].gameObject;
			txt_on = component.GameObjects[5].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[6].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_ladder_red = component.GameObjects[7].gameObject;
			btn_task = component.GameObjects[8].gameObject;
			m_task_red = component.GameObjects[9].gameObject;
			m_Ladder_page = View.AddComponentIfNotExist<GLadderPage>(component.GameObjects[10].gameObject);
			m_task_page = View.AddComponentIfNotExist<GTaskPage>(component.GameObjects[11].gameObject);
			m_icon = component.GameObjects[12].gameObject;
			txt_level = component.GameObjects[13].gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
			btn_buy_ladder_level = component.GameObjects[14].gameObject;
			txt_exp = component.GameObjects[15].gameObject;
			txt_expText = txt_exp.GetComponent<Text>();
			txt_ladder_desc = component.GameObjects[16].gameObject;
			txt_ladder_descText = txt_ladder_desc.GetComponent<Text>();
			txt_ladder_time = component.GameObjects[17].gameObject;
			txt_ladder_timeText = txt_ladder_time.GetComponent<Text>();
			m_exp_sld = component.GameObjects[18].gameObject;
			txt_on_0 = component.GameObjects[19].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[20].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}
