using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BagGDesc : MonoBehaviour
	{
		private List<BagItem> _bagItems;

		private BagPage _bagPage;

		private int _showDescFrom;

		private BagItem _bagItem;

		public GameObject btn_back_desc;

		public GameObject btn_combination;

		public GameObject btn_destroy;

		public GameObject btn_discard;

		public GameObject btn_disrobe_gun_part;

		public GameObject btn_disrobe_skin;

		public GameObject btn_equip;

		public GameObject btn_out_shortcut_bar;

		public GameObject btn_sell;

		public GameObject btn_split;

		public GameObject btn_to_shortcut_bar;

		public GameObject btn_tohand_equip;

		public GameObject btn_unload_bullet;

		public GameObject btn_use;

		public GameObject m_durability_desc;

		public GameObject m_durability_root;

		public GameObject m_icon_desc;

		public GameObject txt_desc_desc;

		public Text txt_desc_descText;

		public GameObject txt_durability_desc;

		public Text txt_durability_descText;

		public GameObject txt_name_desc;

		public Text txt_name_descText;

		public GameObject txt_num_desc;

		public Text txt_num_descText;

		public object context;

		public void Init(BagPage bagPage)
		{
			_bagPage = bagPage;
			ClickListener.Get(btn_back_desc, string.Empty).onClick = _bagPage.OnClickBackDesc;
			ClickListener.Get(btn_sell, string.Empty).onClick = _bagPage.OnClickSell;
			ClickListener.Get(btn_discard, string.Empty).onClick = _bagPage.OnClickDiscard;
			ClickListener.Get(btn_split, string.Empty).onClick = _bagPage.OnClickSplit;
			ClickListener.Get(btn_combination, string.Empty).onClick = _bagPage.OnClickCombination;
			ClickListener.Get(btn_to_shortcut_bar, string.Empty).onClick = _bagPage.OnClickToShortBar;
			ClickListener.Get(btn_out_shortcut_bar, string.Empty).onClick = _bagPage.OnClickOutShortBar;
			ClickListener.Get(btn_use, string.Empty).onClick = _bagPage.OnClickUse;
			ClickListener.Get(btn_equip, string.Empty).onClick = _bagPage.OnClickEquip;
			ClickListener.Get(btn_disrobe_skin, string.Empty).onClick = _bagPage.OnClickRemoveEquip;
			ClickListener.Get(btn_disrobe_gun_part, string.Empty).onClick = _bagPage.OnClickRemoveGunPart;
			ClickListener.Get(btn_tohand_equip, string.Empty).onClick = _bagPage.OnClickToHand;
			ClickListener.Get(btn_unload_bullet, string.Empty).onClick = _bagPage.OnUnloadBullet;
			ClickListener.Get(btn_destroy, string.Empty).onClick = _bagPage.OnClickDestroy;
		}

		public void OnShow()
		{
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUse));
		}

		private void RefreshQuickUse()
		{
			if (_bagItem == null)
			{
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(_bagItem.itemId);
			if (itemCfg != null)
			{
				View.SetLabelText(txt_name_descText, BagPage.GetNameForBinding(itemCfg.name, _bagItem.isBind));
				View.SetItemSprite(m_icon_desc, itemCfg.icon);
				View.SetLabelText(txt_num_descText, _bagItem.number);
				if (itemCfg.durability > 0)
				{
					m_durability_desc.SetActiveBetter(true);
					txt_durability_desc.SetActiveBetter(true);
					View.SetSlider(m_durability_desc, (float)_bagItem.duration * 1f / (float)itemCfg.durability);
					View.SetLabelText(txt_durability_descText, Utils.GetString(9, _bagItem.duration, itemCfg.durability));
				}
				else
				{
					m_durability_desc.SetActiveBetter(false);
					txt_durability_desc.SetActiveBetter(false);
				}
				View.SetLabelText(txt_desc_descText, itemCfg.desc);
				if (_bagItem.instanceId == 9999)
				{
					RefreshGunBulletBtn();
					return;
				}
				btn_sell.SetActiveBetter(itemCfg.sellType != 0);
				btn_split.SetActive(_bagItem.number > 1);
				btn_combination.SetActiveBetter(false);
				btn_sell.SetActiveBetter(false);
				bool flag = Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId);
				btn_to_shortcut_bar.SetActiveBetter(!flag && itemCfg.toQuickUse);
				btn_out_shortcut_bar.SetActiveBetter(flag && itemCfg.toQuickUse);
				btn_use.SetActiveBetter(itemCfg.canUse);
				btn_equip.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 1);
				btn_disrobe_skin.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 4);
				btn_disrobe_gun_part.SetActiveBetter(_showDescFrom == 3);
				btn_discard.SetActiveBetter(Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId));
				btn_tohand_equip.SetActiveBetter(Singleton<BagMgr>.Ins.ToHandTypes.Contains(itemCfg.type));
				btn_unload_bullet.SetActiveBetter(false);
				btn_destroy.SetActiveBetter((Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId)) && _bagItem.isBind);
			}
		}

		public void OnHide()
		{
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUse));
		}

		public void RefreshData()
		{
			if (_bagItem == null)
			{
				_bagPage.OnClickBackDesc(null);
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(_bagItem.itemId);
			View.SetLabelText(txt_name_descText, BagPage.GetNameForBinding(itemCfg.name, _bagItem.isBind));
			View.SetItemSprite(m_icon_desc, itemCfg.icon);
			View.SetLabelText(txt_num_descText, _bagItem.number);
			if (itemCfg.durability > 0)
			{
				m_durability_root.SetActiveBetter(true);
				m_durability_desc.SetActiveBetter(true);
				txt_durability_desc.SetActiveBetter(true);
				View.SetSlider(m_durability_desc, (float)_bagItem.duration * 1f / (float)itemCfg.durability);
				View.SetLabelText(txt_durability_descText, Utils.GetString(9, _bagItem.duration, itemCfg.durability));
			}
			else
			{
				m_durability_root.SetActiveBetter(false);
				m_durability_desc.SetActiveBetter(false);
				txt_durability_desc.SetActiveBetter(false);
			}
			View.SetLabelText(txt_desc_descText, itemCfg.desc);
			if (_bagItem.instanceId == 9999)
			{
				RefreshGunBulletBtn();
				return;
			}
			btn_sell.SetActiveBetter(itemCfg.sellType != 0);
			btn_split.SetActive(_bagItem.number > 1);
			btn_combination.SetActiveBetter(false);
			btn_sell.SetActiveBetter(false);
			bool flag = Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId);
			btn_to_shortcut_bar.SetActiveBetter(!flag && itemCfg.toQuickUse);
			btn_out_shortcut_bar.SetActiveBetter(flag && itemCfg.toQuickUse);
			btn_use.SetActiveBetter(itemCfg.canUse);
			btn_equip.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 1);
			btn_disrobe_skin.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 4);
			btn_disrobe_gun_part.SetActiveBetter(_showDescFrom == 3);
			btn_discard.SetActiveBetter((Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId)) && !_bagItem.isBind);
			btn_tohand_equip.SetActiveBetter(Singleton<BagMgr>.Ins.ToHandTypes.Contains(itemCfg.type));
			btn_unload_bullet.SetActiveBetter(false);
			btn_destroy.SetActiveBetter((Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId)) && _bagItem.isBind);
		}

		public void RefreshGunBulletBtn()
		{
			btn_sell.SetActiveBetter(false);
			btn_split.SetActive(false);
			btn_combination.SetActiveBetter(false);
			btn_sell.SetActiveBetter(false);
			btn_to_shortcut_bar.SetActiveBetter(false);
			btn_out_shortcut_bar.SetActiveBetter(false);
			btn_use.SetActiveBetter(false);
			btn_equip.SetActiveBetter(false);
			btn_disrobe_skin.SetActiveBetter(false);
			btn_disrobe_gun_part.SetActiveBetter(false);
			btn_discard.SetActiveBetter(false);
			btn_tohand_equip.SetActiveBetter(false);
			btn_unload_bullet.SetActiveBetter(true);
			btn_destroy.SetActiveBetter(false);
		}

		public void SetData(BagItem bagItem, int fromZone)
		{
			_showDescFrom = fromZone;
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_desc.gameObject.SetActiveBetter(true);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(false);
			_bagItem = bagItem;
			RefreshData();
		}

		private void Awake()
		{
			btn_back_desc = base.transform.Find("GameObject (2)/btn_back_desc").gameObject;
			btn_combination = base.transform.Find("GameObject (2)/GameObject/btn_combination").gameObject;
			btn_destroy = base.transform.Find("GameObject (2)/GameObject/btn_destroy").gameObject;
			btn_discard = base.transform.Find("GameObject (2)/GameObject/btn_discard").gameObject;
			btn_disrobe_gun_part = base.transform.Find("GameObject (2)/GameObject/btn_disrobe_gun_part").gameObject;
			btn_disrobe_skin = base.transform.Find("GameObject (2)/GameObject/btn_disrobe_skin").gameObject;
			btn_equip = base.transform.Find("GameObject (2)/GameObject/btn_equip").gameObject;
			btn_out_shortcut_bar = base.transform.Find("GameObject (2)/GameObject/btn_out_shortcut_bar").gameObject;
			btn_sell = base.transform.Find("GameObject (2)/GameObject/btn_sell").gameObject;
			btn_split = base.transform.Find("GameObject (2)/GameObject/btn_split").gameObject;
			btn_to_shortcut_bar = base.transform.Find("GameObject (2)/GameObject/btn_to_shortcut_bar").gameObject;
			btn_tohand_equip = base.transform.Find("GameObject (2)/GameObject/btn_tohand_equip").gameObject;
			btn_unload_bullet = base.transform.Find("GameObject (2)/GameObject/btn_unload_bullet").gameObject;
			btn_use = base.transform.Find("GameObject (2)/GameObject/btn_use").gameObject;
			m_durability_desc = base.transform.Find("GameObject (2)/Image/RawImage/naijiu/m_durability_desc").gameObject;
			m_durability_root = base.transform.Find("GameObject (2)/Image/RawImage/naijiu/m_durability_root").gameObject;
			m_icon_desc = base.transform.Find("GameObject (2)/Image/RawImage/m_icon_desc").gameObject;
			txt_desc_desc = base.transform.Find("GameObject (2)/Image (6)/txt_desc_desc").gameObject;
			txt_desc_descText = txt_desc_desc.GetComponent<Text>();
			txt_durability_desc = base.transform.Find("GameObject (2)/Image/RawImage/naijiu/m_durability_root/txt_durability_desc").gameObject;
			txt_durability_descText = txt_durability_desc.GetComponent<Text>();
			txt_name_desc = base.transform.Find("GameObject (2)/Image/RawImage/txt_name_desc").gameObject;
			txt_name_descText = txt_name_desc.GetComponent<Text>();
			txt_num_desc = base.transform.Find("GameObject (2)/Image/RawImage/naijiu/shuliang/txt_num_desc").gameObject;
			txt_num_descText = txt_num_desc.GetComponent<Text>();
		}
	}
}
