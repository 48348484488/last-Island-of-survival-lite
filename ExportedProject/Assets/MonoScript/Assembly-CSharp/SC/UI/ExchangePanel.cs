using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class ExchangePanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal int index;

			internal ExchangePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				int num = _0024this.BuyNum * 10 + index;
				if (num <= Singleton<RoleMgr>.Ins.Coupons)
				{
					_0024this.BuyNum = num;
				}
				else
				{
					AlertBox.Show(308);
				}
			}
		}

		private int _buyNum;

		private GameObject btn_close;

		private GameObject btn_ok;

		private GameObject txt_current_stamps;

		private Text txt_current_stampsText;

		private GameObject txt_diamond;

		private Text txt_diamondText;

		private GameObject txt_use_stamps;

		private Text txt_use_stampsText;

		private GameObject[] m_num_btns;

		private GameObject m_num_btnsObj;

		private GameObject btn_delect;

		private int BuyNum
		{
			get
			{
				return _buyNum;
			}
			set
			{
				_buyNum = value;
				View.SetLabelText(txt_use_stampsText, _buyNum);
				View.SetLabelText(txt_diamondText, _buyNum * cfg.Consts.CUPON_TO_DIAMOND);
			}
		}

		protected override void onInit()
		{
			ClickListener.Get(btn_close, string.Empty).onClick = OnClickClose;
			ClickListener.Get(btn_ok, string.Empty).onClick = OnClickOK;
			for (int i = 0; i < m_num_btns.Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.index = i;
				ClickListener.Get(m_num_btns[i], string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
			}
			ClickListener.Get(btn_delect, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(MoneyChangeDelegate));
			RefreshCurrentCoupons();
		}

		private void MoneyChangeDelegate()
		{
			RefreshCurrentCoupons();
		}

		protected override void onHide(string childView = null)
		{
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(MoneyChangeDelegate));
		}

		protected override void onDestroy()
		{
		}

		private void OnClickClose(GameObject go)
		{
			Hide();
		}

		private void OnClickOK(GameObject go)
		{
			Singleton<RoleMgr>.Ins.BuyDiamondHandle(BuyNum);
		}

		private void RefreshCurrentCoupons()
		{
			View.SetLabelText(txt_current_stampsText, Singleton<RoleMgr>.Ins.Coupons);
			BuyNum = 0;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			btn_ok = component.GameObjects[1].gameObject;
			txt_current_stamps = component.GameObjects[2].gameObject;
			txt_current_stampsText = txt_current_stamps.GetComponent<Text>();
			txt_diamond = component.GameObjects[3].gameObject;
			txt_diamondText = txt_diamond.GetComponent<Text>();
			txt_use_stamps = component.GameObjects[4].gameObject;
			txt_use_stampsText = txt_use_stamps.GetComponent<Text>();
			m_num_btns = component.GameObjects[5].gameObject.GetComponent<UIGameObjectList>().objects;
			m_num_btnsObj = component.GameObjects[5].gameObject;
			btn_delect = component.GameObjects[6].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			if (BuyNum > 0)
			{
				BuyNum /= 10;
			}
		}
	}
}
