using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;

namespace SC.UI
{
	public class BoxBagPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillBoxBagItemData_003Ec__AnonStorey0
		{
			internal BagItem bagItem;

			internal BoxBagPanel _0024this;

			internal void _003C_003Em__0(GameObject g)
			{
				if (!_0024this._isPress)
				{
					if (bagItem.isBind)
					{
						AlertBox.Show(379);
					}
					else
					{
						Singleton<BoxMgr>.Ins.PutBox(_0024this._currentId, bagItem.instanceId, bagItem.number);
					}
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this._isPress = true;
			}

			internal void _003C_003Em__2(GameObject g)
			{
				_0024this._isPress = false;
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBoxItemData_003Ec__AnonStorey1
		{
			internal BagItem bagItem;

			internal BoxBagPanel _0024this;

			internal void _003C_003Em__0(GameObject g)
			{
				if (!_0024this._isPress)
				{
					Singleton<BoxMgr>.Ins.OutBox(_0024this._currentId, bagItem.instanceId, bagItem.number);
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this._isPress = true;
			}

			internal void _003C_003Em__2(GameObject g)
			{
				_0024this._isPress = false;
			}
		}

		private UIScrollPanel _bagScrollPanel;

		private UIScrollPanel _boxScrollPanel;

		private int _bagCapacity;

		private int _boxCapacity;

		private List<BagItem> bagItems = new List<BagItem>();

		private List<BagItem> boxItems = new List<BagItem>();

		private long _currentId;

		private float _startPressTime = 1f;

		private bool _isPress;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject m_bag_page;

		private GameObject scp_bag;

		private GBoxBagBagItem m_cell;

		private GameObject txt_bag_num;

		private Text txt_bag_numText;

		private GameObject scp_box;

		private GameObject txt_box_num;

		private Text txt_box_numText;

		private GBoxBagBoxItem m_cell_0;

		protected override void onInit()
		{
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_boxScrollPanel = scp_box.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(471);
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			SBoxItemChanged.handler = (SBoxItemChanged.Handler)Delegate.Combine(SBoxItemChanged.handler, new SBoxItemChanged.Handler(SBoxItemChangedHandle));
			_currentId = (long)param;
			boxItems = Singleton<BoxMgr>.Ins.GetBoxBagItems(_currentId);
			bagItems = Singleton<BagMgr>.Ins.BagItems;
			_bagCapacity = Singleton<BagMgr>.Ins.BagCapacity;
			int buildItemId = Singleton<BagMgr>.Ins.GetBuildItemId(_currentId);
			ItemCfg itemCfg = ItemCfg.Get(buildItemId);
			_boxCapacity = (int)itemCfg.extras[0];
			SetBagData();
			SetBoxData();
			OnMoneyChange();
			SOpenBox.handler = (SOpenBox.Handler)Delegate.Combine(SOpenBox.handler, new SOpenBox.Handler(OnSOpenBox));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			if (msg.operationRoleId != Singleton<RoleMgr>.Ins.info.roleId && msg.id == _currentId)
			{
				Singleton<BoxMgr>.Ins.OpenBox(_currentId);
			}
		}

		private void OnSOpenBox(SOpenBox msg)
		{
			boxItems = Singleton<BoxMgr>.Ins.GetBoxBagItems(_currentId);
			SetBoxData();
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void SBoxItemChangedHandle(SBoxItemChanged msg)
		{
			if (msg.boxId == _currentId)
			{
				SetBoxNoposData();
			}
		}

		private void SItemChangedHandle(SItemChanged msg)
		{
			SetBagNoposData();
		}

		protected override void onHide(string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(472);
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Remove(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			SBoxItemChanged.handler = (SBoxItemChanged.Handler)Delegate.Remove(SBoxItemChanged.handler, new SBoxItemChanged.Handler(SBoxItemChangedHandle));
			SOpenBox.handler = (SOpenBox.Handler)Delegate.Remove(SOpenBox.handler, new SOpenBox.Handler(OnSOpenBox));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		protected override void onDestroy()
		{
		}

		private void SetBagData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_bagCapacity, FillBoxBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, bagItems.Count, _bagCapacity));
		}

		private void SetBagNoposData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.ResetNoPos(_bagCapacity, FillBoxBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, bagItems.Count, _bagCapacity));
		}

		private void FillBoxBagItemData(GameObject go, int index)
		{
			_003CFillBoxBagItemData_003Ec__AnonStorey0 _003CFillBoxBagItemData_003Ec__AnonStorey = new _003CFillBoxBagItemData_003Ec__AnonStorey0();
			_003CFillBoxBagItemData_003Ec__AnonStorey._0024this = this;
			GBoxBagBagItem component = go.GetComponent<GBoxBagBagItem>();
			if (index >= bagItems.Count)
			{
				component.m_no_has.SetActiveBetter(true);
				component.m_has.SetActiveBetter(false);
				ClickListener.Get(go, string.Empty).onClick = null;
				PressListener.Get(go).onPress = null;
				DownUpListener.Get(go).onDown = null;
				return;
			}
			component.m_no_has.SetActiveBetter(false);
			component.m_has.SetActiveBetter(true);
			_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem = bagItems[index];
			component.m_tag.SetActiveBetter(false);
			component.m_binding.SetActiveBetter(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.isBind);
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.itemId);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_num, _003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.number);
			if (itemCfg.durability > 0)
			{
				component.m_durability.SetActiveBetter(true);
				View.SetSlider(component.m_durability, (float)_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
			}
			else
			{
				component.m_durability.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__0;
			PressListener.Get(go).onPress = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__1;
			DownUpListener.Get(go).onDown = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__2;
		}

		private void SetBoxData()
		{
			_boxScrollPanel.Clear();
			_boxScrollPanel.Reset(_boxCapacity, FillBoxItemData);
			View.SetLabelText(txt_box_numText, Utils.GetString(9, boxItems.Count, _boxCapacity));
		}

		private void SetBoxNoposData()
		{
			_boxScrollPanel.Clear();
			_boxScrollPanel.ResetNoPos(_boxCapacity, FillBoxItemData);
			View.SetLabelText(txt_box_numText, Utils.GetString(9, boxItems.Count, _boxCapacity));
		}

		private void FillBoxItemData(GameObject go, int index)
		{
			_003CFillBoxItemData_003Ec__AnonStorey1 _003CFillBoxItemData_003Ec__AnonStorey = new _003CFillBoxItemData_003Ec__AnonStorey1();
			_003CFillBoxItemData_003Ec__AnonStorey._0024this = this;
			GBoxBagBoxItem component = go.GetComponent<GBoxBagBoxItem>();
			if (index >= boxItems.Count)
			{
				component.m_no_has.SetActiveBetter(true);
				component.m_has.SetActiveBetter(false);
				return;
			}
			component.m_no_has.SetActiveBetter(false);
			component.m_has.SetActiveBetter(true);
			_003CFillBoxItemData_003Ec__AnonStorey.bagItem = boxItems[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBoxItemData_003Ec__AnonStorey.bagItem.itemId);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_num, _003CFillBoxItemData_003Ec__AnonStorey.bagItem.number);
			if (itemCfg.durability > 0)
			{
				component.m_durability.SetActiveBetter(true);
				View.SetSlider(component.m_durability, (float)_003CFillBoxItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
			}
			else
			{
				component.m_durability.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillBoxItemData_003Ec__AnonStorey._003C_003Em__0;
			PressListener.Get(go).onPress = _003CFillBoxItemData_003Ec__AnonStorey._003C_003Em__1;
			DownUpListener.Get(go).onDown = _003CFillBoxItemData_003Ec__AnonStorey._003C_003Em__2;
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
			m_bag_page = component.GameObjects[5].gameObject;
			scp_bag = component.GameObjects[6].gameObject;
			m_cell = View.AddComponentIfNotExist<GBoxBagBagItem>(component.GameObjects[7].gameObject);
			txt_bag_num = component.GameObjects[8].gameObject;
			txt_bag_numText = txt_bag_num.GetComponent<Text>();
			scp_box = component.GameObjects[9].gameObject;
			txt_box_num = component.GameObjects[10].gameObject;
			txt_box_numText = txt_box_num.GetComponent<Text>();
			m_cell_0 = View.AddComponentIfNotExist<GBoxBagBoxItem>(component.GameObjects[11].gameObject);
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}
