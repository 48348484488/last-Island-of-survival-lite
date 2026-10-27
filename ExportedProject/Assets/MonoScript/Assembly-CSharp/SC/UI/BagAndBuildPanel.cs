using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class BagAndBuildPanel : View
	{
		public enum Tab2PageEnum
		{
			Empty = 0,
			Bag = 1,
			Produce = 2
		}

		[CompilerGenerated]
		private sealed class _003CAttachPage2Btn_003Ec__AnonStorey0
		{
			internal Tab2PageEnum tabID;

			internal BagAndBuildPanel _0024this;

			internal void _003C_003Em__0(bool isOn)
			{
				if (isOn)
				{
					_0024this.currentPage = tabID;
					if (!_0024this.dicPages[tabID].IsShow)
					{
						_0024this.dicPages[tabID].OnShow(_0024this.ToBuildItemId);
					}
					_0024this.ToBuildItemId = 0;
				}
				else if (_0024this.dicPages[tabID].IsShow)
				{
					_0024this.dicPages[tabID].OnHide();
				}
				_0024this.m_bag_bg.SetActiveBetter(_0024this.currentPage == Tab2PageEnum.Bag);
				_0024this.m_buid_bg.SetActiveBetter(_0024this.currentPage == Tab2PageEnum.Produce);
			}
		}

		private Dictionary<Tab2PageEnum, RadioButton> dicTabs = new Dictionary<Tab2PageEnum, RadioButton>();

		private Dictionary<Tab2PageEnum, IBagAndBuildPage> dicPages = new Dictionary<Tab2PageEnum, IBagAndBuildPage>();

		private Tab2PageEnum currentPage;

		public int ToBuildItemId;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_bag;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_Build;

		private BagPage m_bag_page;

		private BagProducePage m_produce_page;

		private GameObject m_bag_bg;

		private GameObject m_buid_bg;

		private GameObject m_bag_red;

		private GameObject m_build_red;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			m_bag_page.gameObject.SetActiveBetter(false);
			m_produce_page.gameObject.SetActiveBetter(false);
			dicTabs.Add(Tab2PageEnum.Bag, btn_bag.GetComponent<RadioButton>());
			dicPages.Add(Tab2PageEnum.Bag, m_bag_page);
			dicTabs.Add(Tab2PageEnum.Produce, btn_Build.GetComponent<RadioButton>());
			dicPages.Add(Tab2PageEnum.Produce, m_produce_page);
			foreach (RadioButton value in dicTabs.Values)
			{
				value.isChecked = false;
			}
			for (byte b = 1; b < Enum.GetNames(typeof(Tab2PageEnum)).Length; b = (byte)(b + 1))
			{
				Tab2PageEnum tab2PageEnum = (Tab2PageEnum)b;
				try
				{
					if (dicPages.ContainsKey(tab2PageEnum))
					{
						dicPages[tab2PageEnum].OnInit();
					}
				}
				catch
				{
					Debug.LogError("no excute init()");
				}
				if (currentPage == Tab2PageEnum.Empty)
				{
					currentPage = tab2PageEnum;
				}
				AttachPage2Btn(tab2PageEnum);
			}
			ClickListener.Get(btn_back, "ui_close").onClick = _003ConInit_003Em__0;
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
				currentPage = Tab2PageEnum.Bag;
				RadioButton.ChooseBtn(dicTabs[currentPage].gameObject);
			}
			else if (param is int)
			{
				int itemId = (int)param;
				JumpToProduce(itemId);
			}
			else
			{
				currentPage = Tab2PageEnum.Bag;
				RadioButton.ChooseBtn(dicTabs[currentPage].gameObject);
			}
			OnMoneyChange();
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			m_bag_red.SetActiveBetter(false);
			m_build_red.SetActiveBetter(false);
			ScProduceEvent.ItemInfoWayJumpDelegate = (Utils.IntDelegate)Delegate.Combine(ScProduceEvent.ItemInfoWayJumpDelegate, new Utils.IntDelegate(JumpToProduce));
		}

		private void JumpToProduce(int itemId)
		{
			int value;
			Singleton<ScProduceMgr>.Ins.DicProductionId2DrawId.TryGetValue(itemId, out value);
			ToBuild(value);
		}

		public void ToBuild(int drawItemId)
		{
			ToBuildItemId = drawItemId;
			RadioButton.ChooseBtn(dicTabs[Tab2PageEnum.Produce].gameObject);
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			dicTabs[currentPage].isChecked = false;
			dicPages[currentPage].OnHide();
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			ScProduceEvent.ItemInfoWayJumpDelegate = (Utils.IntDelegate)Delegate.Remove(ScProduceEvent.ItemInfoWayJumpDelegate, new Utils.IntDelegate(ToBuild));
		}

		private void AttachPage2Btn(Tab2PageEnum tabID)
		{
			_003CAttachPage2Btn_003Ec__AnonStorey0 _003CAttachPage2Btn_003Ec__AnonStorey = new _003CAttachPage2Btn_003Ec__AnonStorey0();
			_003CAttachPage2Btn_003Ec__AnonStorey.tabID = tabID;
			_003CAttachPage2Btn_003Ec__AnonStorey._0024this = this;
			dicTabs[_003CAttachPage2Btn_003Ec__AnonStorey.tabID].OnValueChanged = _003CAttachPage2Btn_003Ec__AnonStorey._003C_003Em__0;
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
			btn_bag = component.GameObjects[4].gameObject;
			txt_on = component.GameObjects[5].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[6].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_Build = component.GameObjects[7].gameObject;
			m_bag_page = View.AddComponentIfNotExist<BagPage>(component.GameObjects[8].gameObject);
			m_produce_page = View.AddComponentIfNotExist<BagProducePage>(component.GameObjects[9].gameObject);
			m_bag_bg = component.GameObjects[10].gameObject;
			m_buid_bg = component.GameObjects[11].gameObject;
			m_bag_red = component.GameObjects[12].gameObject;
			m_build_red = component.GameObjects[13].gameObject;
			txt_on_0 = component.GameObjects[14].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[15].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
			Utils.TriggerEvent(BagEvent.CloseBagPanel);
		}
	}
}
