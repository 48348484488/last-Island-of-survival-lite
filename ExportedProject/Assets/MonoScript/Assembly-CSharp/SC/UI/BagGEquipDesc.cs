using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BagGEquipDesc : MonoBehaviour
	{
		private List<BagItem> _bagItems;

		private BagPage _bagPage;

		private int _showDescFrom;

		private BagItem _bagItem;

		public GameObject btn_back_desc;

		public GameObject btn_combination;

		public GameObject btn_destroy;

		public GameObject btn_discard;

		public GameObject btn_disrobe;

		public GameObject btn_equip;

		public GameObject btn_out_shortcut_bar;

		public GameObject btn_sell;

		public GameObject btn_split;

		public GameObject btn_to_shortcut_bar;

		public GameObject btn_use;

		public GameObject m_durability;

		public GameObject m_durability_root;

		public GameObject m_icon;

		public GameObject txt_add_life;

		public Text txt_add_lifeText;

		public GameObject txt_defense;

		public Text txt_defenseText;

		public GameObject txt_desc_desc;

		public Text txt_desc_descText;

		public GameObject txt_durability;

		public Text txt_durabilityText;

		public GameObject txt_name;

		public Text txt_nameText;

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
			ClickListener.Get(btn_disrobe, string.Empty).onClick = _bagPage.OnClickRemoveEquip;
			ClickListener.Get(btn_destroy, string.Empty).onClick = _bagPage.OnClickDestroy;
		}

		public void OnShow()
		{
			BagEvent.PutSkinByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutSkinByItemId, new Utils.IntDelegate(PutSkinByItemIdDelegent));
			BagEvent.PutEquipByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutEquipByItemId, new Utils.IntDelegate(PutEquipByItemIdDelegent));
			BagEvent.RemoveSkinByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveSkinByItemId, new Utils.IntDelegate(RemoveSkinByItemIdDelegent));
			BagEvent.RemoveEquipByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveEquipByItemId, new Utils.IntDelegate(RemoveEquipByItemIdDelegent));
		}

		private void RemoveEquipByItemIdDelegent(int arg)
		{
			_bagPage.OnClickEquip(null);
		}

		private void RemoveSkinByItemIdDelegent(int arg)
		{
			_bagPage.OnClickEquip(null);
		}

		private void PutEquipByItemIdDelegent(int arg)
		{
			_bagPage.OnClickEquip(null);
		}

		private void PutSkinByItemIdDelegent(int arg)
		{
			_bagPage.OnClickEquip(null);
		}

		public void OnHide()
		{
			BagEvent.PutSkinByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.PutSkinByItemId, new Utils.IntDelegate(PutSkinByItemIdDelegent));
			BagEvent.PutEquipByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.PutEquipByItemId, new Utils.IntDelegate(PutEquipByItemIdDelegent));
			BagEvent.RemoveSkinByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.RemoveSkinByItemId, new Utils.IntDelegate(RemoveSkinByItemIdDelegent));
			BagEvent.RemoveEquipByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.RemoveEquipByItemId, new Utils.IntDelegate(RemoveEquipByItemIdDelegent));
		}

		public void RefreshData()
		{
			if (_bagItem == null)
			{
				_bagPage.OnClickBackDesc(null);
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(_bagItem.itemId);
			View.SetLabelText(txt_nameText, BagPage.GetNameForBinding(itemCfg.name, _bagItem.isBind));
			View.SetItemSprite(m_icon, itemCfg.icon);
			m_durability_root.SetActiveBetter(false);
			if (itemCfg.durability > 0)
			{
				m_durability_root.SetActiveBetter(true);
				View.SetSlider(m_durability, (float)_bagItem.duration * 1f / (float)itemCfg.durability);
				View.SetLabelText(txt_durabilityText, Utils.GetString(9, _bagItem.duration, itemCfg.durability));
			}
			View.SetLabelText(txt_desc_descText, itemCfg.desc);
			EquipCfg equipCfg = EquipCfg.Get(_bagItem.itemId);
			float num = equipCfg.props[0];
			float num2 = equipCfg.props[1] + equipCfg.props[2];
			View.SetLabelText(txt_add_lifeText, num);
			View.SetLabelText(txt_defenseText, num2);
			btn_sell.SetActiveBetter(itemCfg.sellType != 0);
			btn_split.SetActive(_bagItem.number > 1);
			btn_combination.SetActiveBetter(false);
			btn_sell.SetActiveBetter(false);
			bool flag = Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId);
			btn_to_shortcut_bar.SetActiveBetter(!flag && itemCfg.toQuickUse);
			btn_out_shortcut_bar.SetActiveBetter(flag && itemCfg.toQuickUse);
			btn_use.SetActiveBetter(itemCfg.canUse);
			btn_equip.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 1);
			btn_discard.SetActiveBetter((Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId)) && !_bagItem.isBind);
			btn_disrobe.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type) && _showDescFrom == 4);
			btn_destroy.SetActiveBetter((Singleton<BagMgr>.Ins.BagContainsInstanceId(_bagItem.instanceId) || Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(_bagItem.instanceId)) && _bagItem.isBind);
		}

		public void SetData(BagItem bagItem, int fromZone)
		{
			_showDescFrom = fromZone;
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(true);
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
			btn_disrobe = base.transform.Find("GameObject (2)/GameObject/btn_disrobe").gameObject;
			btn_equip = base.transform.Find("GameObject (2)/GameObject/btn_equip").gameObject;
			btn_out_shortcut_bar = base.transform.Find("GameObject (2)/GameObject/btn_out_shortcut_bar").gameObject;
			btn_sell = base.transform.Find("GameObject (2)/GameObject/btn_sell").gameObject;
			btn_split = base.transform.Find("GameObject (2)/GameObject/btn_split").gameObject;
			btn_to_shortcut_bar = base.transform.Find("GameObject (2)/GameObject/btn_to_shortcut_bar").gameObject;
			btn_use = base.transform.Find("GameObject (2)/GameObject/btn_use").gameObject;
			m_durability = base.transform.Find("GameObject (2)/Image/RawImage/m_durability_root/m_durability").gameObject;
			m_durability_root = base.transform.Find("GameObject (2)/Image/RawImage/m_durability_root").gameObject;
			m_icon = base.transform.Find("GameObject (2)/Image/RawImage/m_icon").gameObject;
			txt_add_life = base.transform.Find("GameObject (2)/tasks/content/Image/txt_add_life").gameObject;
			txt_add_lifeText = txt_add_life.GetComponent<Text>();
			txt_defense = base.transform.Find("GameObject (2)/tasks/content/Image (1)/txt_defense").gameObject;
			txt_defenseText = txt_defense.GetComponent<Text>();
			txt_desc_desc = base.transform.Find("GameObject (2)/tasks/content/Image (6)/txt_desc_desc").gameObject;
			txt_desc_descText = txt_desc_desc.GetComponent<Text>();
			txt_durability = base.transform.Find("GameObject (2)/Image/RawImage/m_durability_root/naijiu/txt_durability").gameObject;
			txt_durabilityText = txt_durability.GetComponent<Text>();
			txt_name = base.transform.Find("GameObject (2)/Image/RawImage/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
