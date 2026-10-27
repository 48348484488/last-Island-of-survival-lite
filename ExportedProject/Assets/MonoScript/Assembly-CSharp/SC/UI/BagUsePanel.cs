using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BagUsePanel : View
	{
		private int _instanceId;

		private int _useNum;

		private BagItem _bagItem;

		private Slider _slider;

		private GameObject btn_close;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_has_num;

		private Text txt_has_numText;

		private GameObject txt_split_num;

		private Text txt_split_numText;

		private GameObject m_split_sld;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject btn_confirm;

		private GameObject btn_confirm_box_consume;

		private GameObject txt_consume;

		private Text txt_consumeText;

		protected override void onInit()
		{
			_slider = m_split_sld.GetComponent<Slider>();
			_slider.onValueChanged.AddListener(OnSldValueChange);
			_slider.wholeNumbers = true;
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_confirm_box_consume, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_up, string.Empty).onClick = OnClickUp;
			ClickListener.Get(btn_down, string.Empty).onClick = OnClickDown;
			ClickListener.Get(btn_close, string.Empty).onClick = OnClose;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_instanceId = (int)param;
			_bagItem = Singleton<BagMgr>.Ins.GetBagItemByInstanceId(_instanceId);
			ItemCfg itemCfg = ItemCfg.Get(_bagItem.itemId);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_has_numText, _bagItem.number);
			View.SetItemSprite(m_icon, itemCfg.icon);
			_slider.maxValue = _bagItem.number;
			_slider.value = _bagItem.number;
			BagEvent.UseItemSucess = (Utils.VoidDelegate)Delegate.Combine(BagEvent.UseItemSucess, new Utils.VoidDelegate(UseItemSucessDelegent));
			_useNum = _bagItem.number;
			View.SetLabelText(txt_split_numText, _useNum);
			btn_confirm.SetActiveBetter(true);
			btn_confirm_box_consume.SetActiveBetter(false);
			if (itemCfg.type == 123)
			{
				OpenableBoxCfg openableBoxCfg = OpenableBoxCfg.Get(_bagItem.itemId);
				if (openableBoxCfg.moneyType == 1 && openableBoxCfg.money > 0)
				{
					btn_confirm_box_consume.SetActiveBetter(true);
					btn_confirm.SetActiveBetter(false);
					View.SetLabelText(txt_consumeText, openableBoxCfg.money * _useNum);
				}
			}
		}

		private void UseItemSucessDelegent()
		{
			Hide();
		}

		protected override void onHide(string childView = null)
		{
			BagEvent.UseItemSucess = (Utils.VoidDelegate)Delegate.Remove(BagEvent.UseItemSucess, new Utils.VoidDelegate(UseItemSucessDelegent));
		}

		protected override void onDestroy()
		{
		}

		private void OnClickConfirm(GameObject go)
		{
			Singleton<BagMgr>.Ins.UseItem(_instanceId, _useNum);
			Hide();
		}

		private void OnClickUp(GameObject go)
		{
			if (_useNum < _bagItem.number)
			{
				_useNum++;
				View.SetSlider(m_split_sld, _useNum);
				View.SetLabelText(txt_split_numText, _useNum);
				RefreshConsume();
			}
		}

		private void RefreshConsume()
		{
			OpenableBoxCfg openableBoxCfg = OpenableBoxCfg.Get(_bagItem.itemId);
			if (openableBoxCfg != null && openableBoxCfg.money > 0)
			{
				View.SetLabelText(txt_consumeText, openableBoxCfg.money * _useNum);
			}
		}

		private void OnClickDown(GameObject go)
		{
			if (_useNum > 0)
			{
				_useNum--;
				View.SetSlider(m_split_sld, _useNum);
				View.SetLabelText(txt_split_numText, _useNum);
				RefreshConsume();
			}
		}

		private void OnSldValueChange(float value)
		{
			_useNum = (int)value;
			View.SetLabelText(txt_split_numText, _useNum);
			RefreshConsume();
		}

		private void OnClose(GameObject go)
		{
			Hide();
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			m_icon = component.GameObjects[1].gameObject;
			txt_name = component.GameObjects[2].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_has_num = component.GameObjects[3].gameObject;
			txt_has_numText = txt_has_num.GetComponent<Text>();
			txt_split_num = component.GameObjects[4].gameObject;
			txt_split_numText = txt_split_num.GetComponent<Text>();
			m_split_sld = component.GameObjects[5].gameObject;
			btn_up = component.GameObjects[6].gameObject;
			btn_down = component.GameObjects[7].gameObject;
			btn_confirm = component.GameObjects[8].gameObject;
			btn_confirm_box_consume = component.GameObjects[9].gameObject;
			txt_consume = component.GameObjects[10].gameObject;
			txt_consumeText = txt_consume.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			OpenableBoxCfg openableBoxCfg = OpenableBoxCfg.Get(_bagItem.itemId);
			if (openableBoxCfg.moneyType == 1)
			{
				if (Singleton<RoleMgr>.Ins.Gold < openableBoxCfg.money)
				{
					AlertBox.Show(155);
					return;
				}
			}
			else if (openableBoxCfg.moneyType == 2 && Singleton<RoleMgr>.Ins.Coupons < openableBoxCfg.money)
			{
				AlertBox.Show(157);
				return;
			}
			Singleton<BagMgr>.Ins.UseItem(_instanceId, _useNum);
			Hide();
		}
	}
}
