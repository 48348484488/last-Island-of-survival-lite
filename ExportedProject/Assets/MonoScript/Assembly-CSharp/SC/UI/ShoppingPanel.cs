using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class ShoppingPanel : View
	{
		public enum ShopPageEnum
		{
			TimeShop = 0,
			NormalShop = 1,
			Recharge = 2,
			None = 3
		}

		private readonly Dictionary<ShopPageEnum, IShopPage> _dicTab2Page = new Dictionary<ShopPageEnum, IShopPage>();

		private ShopPageEnum _curTab;

		private GameObject btn_back;

		private GameObject txt_gold;

		private Text txt_goldText;

		private GameObject txt_coupon;

		private Text txt_couponText;

		private GameObject btn_time_shop;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_time_shop_red;

		private GameObject btn_normal_shop;

		private GameObject m_normal_shop_red;

		private GameObject btn_recharge;

		private GameObject m_recharge_red;

		private ShopTimePage m_time_shop;

		private ShopNormalPage m_normal_shop;

		private ShopRechargePage m_recharge;

		private GameObject btn_add_coupon;

		private GameObject btn_change;

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

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		protected override void onInit()
		{
			base.onInit();
			_curTab = ShopPageEnum.None;
			_dicTab2Page.Add(ShopPageEnum.TimeShop, m_time_shop);
			_dicTab2Page.Add(ShopPageEnum.NormalShop, m_normal_shop);
			_dicTab2Page.Add(ShopPageEnum.Recharge, m_recharge);
			foreach (IShopPage value in _dicTab2Page.Values)
			{
				value.OnInit();
				value.ThisGo.SetActiveBetter(false);
			}
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener clickListener = ClickListener.Get(btn_change, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__1;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener.Get(btn_time_shop, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_normal_shop, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_recharge, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener clickListener2 = ClickListener.Get(btn_add_coupon, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003ConInit_003Em__5;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			OnRedDotChange();
			if (param == null)
			{
				RadioButton.ChooseBtn(btn_time_shop);
				ChangePage(ShopPageEnum.TimeShop);
			}
			else
			{
				int num = (int)param;
				if (num > 0)
				{
					RadioButton.ChooseBtn(btn_normal_shop);
					ChangePage(ShopPageEnum.NormalShop, num);
				}
				else
				{
					RadioButton.ChooseBtn(btn_recharge);
					ChangePage(ShopPageEnum.Recharge);
				}
			}
			OnTokenMoneyChange();
			ShopEvent.RedDotChangeAction = (Action)Delegate.Combine(ShopEvent.RedDotChangeAction, new Action(OnRedDotChange));
			RoleEvent.TokenMoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.TokenMoneyChangeDelegate, new Utils.VoidDelegate(OnTokenMoneyChange));
			ShopEvent.JumpToRechargePage = (Action)Delegate.Combine(ShopEvent.JumpToRechargePage, new Action(OnJumpToRechargePage));
		}

		private void OnJumpToRechargePage()
		{
			RadioButton.ChooseBtn(btn_recharge);
			ChangePage(ShopPageEnum.Recharge);
		}

		private void OnTokenMoneyChange()
		{
			View.SetLabelText(txt_couponText, Singleton<RoleMgr>.Ins.Coupons);
			View.SetLabelText(txt_goldText, Singleton<RoleMgr>.Ins.Gold);
		}

		private void OnRedDotChange()
		{
			m_time_shop_red.SetActiveBetter(false);
			m_normal_shop_red.SetActiveBetter(false);
			m_recharge_red.SetActiveBetter(false);
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			IShopPage value;
			if (_dicTab2Page.TryGetValue(_curTab, out value))
			{
				value.ThisGo.SetActiveBetter(false);
				value.OnHide();
				_curTab = ShopPageEnum.None;
			}
			ShopEvent.RedDotChangeAction = (Action)Delegate.Remove(ShopEvent.RedDotChangeAction, new Action(OnRedDotChange));
			RoleEvent.TokenMoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.TokenMoneyChangeDelegate, new Utils.VoidDelegate(OnTokenMoneyChange));
			ShopEvent.JumpToRechargePage = (Action)Delegate.Remove(ShopEvent.JumpToRechargePage, new Action(OnJumpToRechargePage));
		}

		private void ChangePage(ShopPageEnum tab, int selectId = 0)
		{
			if (_curTab != tab)
			{
				IShopPage value;
				if (_dicTab2Page.TryGetValue(_curTab, out value))
				{
					value.ThisGo.SetActiveBetter(false);
					value.OnHide();
				}
				_curTab = tab;
				if (_dicTab2Page.TryGetValue(tab, out value))
				{
					value.ThisGo.SetActiveBetter(true);
					value.OnShow(selectId);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_gold = component.GameObjects[1].gameObject;
			txt_goldText = txt_gold.GetComponent<Text>();
			txt_coupon = component.GameObjects[2].gameObject;
			txt_couponText = txt_coupon.GetComponent<Text>();
			btn_time_shop = component.GameObjects[3].gameObject;
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[5].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_time_shop_red = component.GameObjects[6].gameObject;
			btn_normal_shop = component.GameObjects[7].gameObject;
			m_normal_shop_red = component.GameObjects[8].gameObject;
			btn_recharge = component.GameObjects[9].gameObject;
			m_recharge_red = component.GameObjects[10].gameObject;
			m_time_shop = View.AddComponentIfNotExist<ShopTimePage>(component.GameObjects[11].gameObject);
			m_normal_shop = View.AddComponentIfNotExist<ShopNormalPage>(component.GameObjects[12].gameObject);
			m_recharge = View.AddComponentIfNotExist<ShopRechargePage>(component.GameObjects[13].gameObject);
			btn_add_coupon = component.GameObjects[14].gameObject;
			btn_change = component.GameObjects[15].gameObject;
			txt_on_0 = component.GameObjects[16].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[17].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_0 = component.GameObjects[18].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_2 = component.GameObjects[19].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_1 = component.GameObjects[20].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__1(GameObject go)
		{
			ViewMgr.Ins.ShowView<ExchangePanel>();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			ChangePage(ShopPageEnum.TimeShop);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			ChangePage(ShopPageEnum.NormalShop);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			ChangePage(ShopPageEnum.Recharge);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__5(GameObject go)
		{
			Singleton<RechargeMgr>.Ins.ShowRecharge();
		}
	}
}
