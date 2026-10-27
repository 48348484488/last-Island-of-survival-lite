using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LadderBuyTypePanel : View
	{
		private int _specialConsume;

		private int _superConsume;

		private GameObject btn_back;

		private GameObject txt_ladder_special_desc;

		private Text txt_ladder_special_descText;

		private GameObject btn_buy_special;

		private GameObject txt_buy_special_consume;

		private Text txt_buy_special_consumeText;

		private GameObject m_discount_special;

		private GameObject txt_ladder_super_desc;

		private Text txt_ladder_super_descText;

		private GameObject btn_buy_super;

		private GameObject txt_origain_consume;

		private Text txt_origain_consumeText;

		private GameObject txt_buy_super_consume;

		private Text txt_buy_super_consumeText;

		private GameObject m_discount_super;

		private GameObject txt_award;

		private Text txt_awardText;

		private GameObject txt_award_0;

		private Text txt_award_0Text;

		private GameObject txt_award_1;

		private Text txt_award_1Text;

		private GameObject txt_award_2;

		private Text txt_award_2Text;

		private GameObject txt_award_3;

		private Text txt_award_3Text;

		private GameObject txt_award_4;

		private Text txt_award_4Text;

		private GameObject txt_award_5;

		private Text txt_award_5Text;

		private GameObject txt_award_6;

		private Text txt_award_6Text;

		private GameObject txt_award_7;

		private Text txt_award_7Text;

		private GameObject txt_award_8;

		private Text txt_award_8Text;

		protected override void onInit()
		{
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_buy_special, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_buy_super, string.Empty).onClick = _003ConInit_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			View.SetLabelText(txt_buy_special_consumeText, cfg.Consts.BUY_SPECIAL_LADDER_TASK_PRICE);
			View.SetLabelText(txt_buy_super_consume, cfg.Consts.BUY_SUPER_LADDER_TASK_PRICE);
			_specialConsume = cfg.Consts.BUY_SPECIAL_LADDER_TASK_PRICE;
			_superConsume = cfg.Consts.BUY_SUPER_LADDER_TASK_PRICE;
			LadderEvent.BuyTaskTypeSucess = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.BuyTaskTypeSucess, new Utils.VoidDelegate(BuyTaskTypeSucess));
		}

		private void BuyTaskTypeSucess()
		{
			Hide();
		}

		protected override void onHide(string childView = null)
		{
			LadderEvent.BuyTaskTypeSucess = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.BuyTaskTypeSucess, new Utils.VoidDelegate(BuyTaskTypeSucess));
		}

		protected override void onDestroy()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_ladder_special_desc = component.GameObjects[1].gameObject;
			txt_ladder_special_descText = txt_ladder_special_desc.GetComponent<Text>();
			btn_buy_special = component.GameObjects[2].gameObject;
			txt_buy_special_consume = component.GameObjects[3].gameObject;
			txt_buy_special_consumeText = txt_buy_special_consume.GetComponent<Text>();
			m_discount_special = component.GameObjects[4].gameObject;
			txt_ladder_super_desc = component.GameObjects[5].gameObject;
			txt_ladder_super_descText = txt_ladder_super_desc.GetComponent<Text>();
			btn_buy_super = component.GameObjects[6].gameObject;
			txt_origain_consume = component.GameObjects[7].gameObject;
			txt_origain_consumeText = txt_origain_consume.GetComponent<Text>();
			txt_buy_super_consume = component.GameObjects[8].gameObject;
			txt_buy_super_consumeText = txt_buy_super_consume.GetComponent<Text>();
			m_discount_super = component.GameObjects[9].gameObject;
			txt_award = component.GameObjects[10].gameObject;
			txt_awardText = txt_award.GetComponent<Text>();
			txt_award_0 = component.GameObjects[11].gameObject;
			txt_award_0Text = txt_award_0.GetComponent<Text>();
			txt_award_1 = component.GameObjects[12].gameObject;
			txt_award_1Text = txt_award_1.GetComponent<Text>();
			txt_award_2 = component.GameObjects[13].gameObject;
			txt_award_2Text = txt_award_2.GetComponent<Text>();
			txt_award_3 = component.GameObjects[14].gameObject;
			txt_award_3Text = txt_award_3.GetComponent<Text>();
			txt_award_4 = component.GameObjects[15].gameObject;
			txt_award_4Text = txt_award_4.GetComponent<Text>();
			txt_award_5 = component.GameObjects[16].gameObject;
			txt_award_5Text = txt_award_5.GetComponent<Text>();
			txt_award_6 = component.GameObjects[17].gameObject;
			txt_award_6Text = txt_award_6.GetComponent<Text>();
			txt_award_7 = component.GameObjects[18].gameObject;
			txt_award_7Text = txt_award_7.GetComponent<Text>();
			txt_award_8 = component.GameObjects[19].gameObject;
			txt_award_8Text = txt_award_8.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			if (Singleton<RoleMgr>.Ins.Gold < _specialConsume)
			{
				AlertBox.Show(155);
				return;
			}
			Singleton<LadderMgr>.Ins.BuyLadderTask(1);
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (Singleton<RoleMgr>.Ins.Gold < _superConsume)
			{
				AlertBox.Show(155);
				return;
			}
			Singleton<LadderMgr>.Ins.BuyLadderTask(2);
			Hide();
		}
	}
}
