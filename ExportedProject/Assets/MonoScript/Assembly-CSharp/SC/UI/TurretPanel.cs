using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.map.turret.scmsg;
using gs.battle.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class TurretPanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal int index;

			internal TurretPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (!_0024this._isPress)
				{
					_0024this._currentSelectIndex = index + 1;
					_0024this.RefreshSelect();
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this.OnEnter(2, index + 1);
			}

			internal void _003C_003Em__2(GameObject o)
			{
				_0024this._drag = true;
				_0024this._dragIndex = index + 1;
				if (_0024this._turretBuildingInfo.bullets.ContainsKey(index + 1))
				{
					_0024this._dragUseItem = _0024this._turretBuildingInfo.bullets[index + 1];
					_0024this.OnBeginDrag(_0024this._dragUseItem, 2);
				}
			}

			internal void _003C_003Em__3(GameObject o)
			{
				_0024this.OnDragFromBullletZone();
			}

			internal void _003C_003Em__4(GameObject o)
			{
				_0024this._isPress = true;
			}

			internal void _003C_003Em__5(GameObject g)
			{
				_0024this._isPress = false;
				_0024this._drag = false;
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBoxBagItemData_003Ec__AnonStorey1
		{
			internal int index;

			internal BagItem bagItem;

			internal ItemCfg itemCfg;

			internal TurretPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this.OnEnter(1, index);
			}

			internal void _003C_003Em__1(GameObject g)
			{
				if (!_0024this._drag && !_0024this._isPress)
				{
					TurretSplitPanel.ShowSplitPanel(bagItem, _0024this._currentSelectIndex, _0024this.LoadBullet);
				}
			}

			internal void _003C_003Em__2(GameObject o)
			{
				_0024this._drag = true;
				_0024this._dragBagItem = bagItem;
				_0024this.OnBeginDrag(_0024this._dragBagItem, 1);
			}

			internal void _003C_003Em__3(GameObject o)
			{
				_0024this.OnDragFromBagZone();
			}

			internal void _003C_003Em__4(GameObject o)
			{
				_0024this._isPress = true;
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemCfg);
			}

			internal void _003C_003Em__5(GameObject g)
			{
				_0024this._isPress = false;
				_0024this._drag = false;
			}
		}

		private UIScrollPanel _bagScrollPanel;

		private List<BagItem> _bagItems = new List<BagItem>();

		private float _startPressTime = 1f;

		private bool _isPress;

		private bool _drag;

		private int _capacity = 25;

		public List<int> DrawOriginalIds = new List<int>();

		private STurretBuildingInfo _turretBuildingInfo;

		private int _currentSelectIndex = 1;

		private BagItem _dragBagItem;

		private UseItem _dragUseItem;

		private int _dragIndex;

		private int _enterIndex;

		private int _dragEndZone;

		private int _dragFromeZone;

		private Canvas _canvas;

		public const int BagZone = 1;

		public const int BulletZone = 2;

		private bool _isOpen;

		private RadioButton _turretSwitchOn;

		private RadioButton _turretSwitchOff;

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

		private GTurretBagItem m_cell;

		private GameObject txt_bag_num;

		private Text txt_bag_numText;

		private GameObject m_gun_frame;

		private GameObject m_gun_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_duration;

		private Text txt_durationText;

		private GameObject txt_desc;

		private Text txt_descText;

		private List<GTurretBulletItem> m_bulletslist = new List<GTurretBulletItem>();

		private GameObject[] m_bullets;

		private GameObject m_bulletsObj;

		private GTurretBulletItem m_bullet_item;

		private GameObject btn_battle_friend;

		private GameObject btn_put_all_bullet;

		private GameObject btn_set_power;

		private GameObject ckb_team;

		private GameObject m_drag_Canvas;

		private GameObject m_drag_icon;

		private GameObject m_turrent_switch;

		private GameObject m_turrent_switch_on;

		private GameObject m_turrent_switch_off;

		private GTurretBulletItem m_bullet_item_0;

		private GTurretBulletItem m_bullet_item_1;

		private GTurretBulletItem m_bullet_item_2;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			_turretSwitchOn = m_turrent_switch_on.GetComponent<RadioButton>();
			_turretSwitchOff = m_turrent_switch_off.GetComponent<RadioButton>();
			_canvas = GameObject.Find("UIRootCanvas").GetComponent<Canvas>();
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_bagScrollPanel.Clear();
			m_cell.gameObject.SetActiveBetter(false);
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(m_turrent_switch_on, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(m_turrent_switch_off, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_set_power, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(ckb_team, string.Empty).onClick = _003ConInit_003Em__4;
			for (int i = 0; i < m_bullets.Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.index = i;
				GameObject go = m_bullets[_003ConInit_003Ec__AnonStorey.index];
				ClickListener.Get(go, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
				UIEventListener.Get(go, string.Empty).onEnter = _003ConInit_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(go).onBeginDrag = _003ConInit_003Ec__AnonStorey._003C_003Em__2;
				DragListener.Get(go).onDrag = OnDragMask;
				DragListener.Get(go).onEndDrag = _003ConInit_003Ec__AnonStorey._003C_003Em__3;
				PressListener.Get(go).onPress = _003ConInit_003Ec__AnonStorey._003C_003Em__4;
				DownUpListener.Get(go).onDown = _003ConInit_003Ec__AnonStorey._003C_003Em__5;
			}
		}

		private void RefreshSelect()
		{
			SetTurretBulles();
		}

		public void OnEnter(int enterZone, int enterIndex)
		{
			_dragEndZone = enterZone;
			_enterIndex = enterIndex;
		}

		private void UpdateSelectIndex()
		{
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_currentSelectIndex = 1;
			_turretBuildingInfo = param as STurretBuildingInfo;
			RefreshIsOpen();
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			STurretBuildingInfo.handler = (STurretBuildingInfo.Handler)Delegate.Combine(STurretBuildingInfo.handler, new STurretBuildingInfo.Handler(STurretBuildingInfoHandle));
			SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(SBuildingStatusChangeHandle));
			RefreshData();
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(_turretBuildingInfo.turretId);
			View.SetLabelText(txt_durationText, partByInsID.Hp);
		}

		public void RefreshIsOpen()
		{
			_isOpen = Singleton<TurretMgr>.Ins.IsTurrentOpen(_turretBuildingInfo.turretId);
			_turretSwitchOff.isChecked = !_isOpen;
			_turretSwitchOn.isChecked = _isOpen;
		}

		private void RefreshData()
		{
			m_drag_icon.SetActiveBetter(false);
			GunCfg gunCfg = GunCfg.Get(Singleton<TurretMgr>.Ins.GetTurretGunId(_turretBuildingInfo.turretBaseId));
			DrawOriginalIds.Clear();
			for (int i = 0; i < gunCfg.bulletIds.Count; i++)
			{
				if (!DrawOriginalIds.Contains(gunCfg.bulletIds[i]))
				{
					DrawOriginalIds.Add(gunCfg.bulletIds[i]);
				}
			}
			OnMoneyChange();
			_bagItems = GetDrawPanelItems();
			SetBagData();
			SetTurretInfo();
			SetTurretBulles();
			View.SetCheckbox(ckb_team, _turretBuildingInfo.isAttackCompanions);
		}

		private void STurretBuildingInfoHandle(STurretBuildingInfo msg)
		{
			_turretBuildingInfo = msg;
			RefreshData();
		}

		public List<BagItem> GetDrawPanelItems()
		{
			List<BagItem> list = new List<BagItem>();
			for (int i = 0; i < DrawOriginalIds.Count; i++)
			{
				BagItem bagItem = new BagItem();
				bagItem.itemId = DrawOriginalIds[i];
				bagItem.instanceId = bagItem.itemId;
				bagItem.number = Singleton<BagMgr>.Ins.GetItemNum(bagItem.itemId, false);
				if (bagItem.number > 0)
				{
					list.Add(bagItem);
				}
			}
			return list;
		}

		private void SItemChangedHandle(SItemChanged msg)
		{
			_bagItems = GetDrawPanelItems();
			SetBagNoposData();
		}

		private void SetBagNoposData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.ResetNoPos(_capacity, FillBoxBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, _bagItems.Count, _capacity));
		}

		private void SetBagData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_capacity, FillBoxBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, _bagItems.Count, _capacity));
		}

		private void FillBoxBagItemData(GameObject go, int index)
		{
			_003CFillBoxBagItemData_003Ec__AnonStorey1 _003CFillBoxBagItemData_003Ec__AnonStorey = new _003CFillBoxBagItemData_003Ec__AnonStorey1();
			_003CFillBoxBagItemData_003Ec__AnonStorey.index = index;
			_003CFillBoxBagItemData_003Ec__AnonStorey._0024this = this;
			GTurretBagItem component = go.GetComponent<GTurretBagItem>();
			UIEventListener.Get(go, string.Empty).onEnter = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__0;
			if (_003CFillBoxBagItemData_003Ec__AnonStorey.index >= _bagItems.Count)
			{
				component.m_no_has.SetActiveBetter(true);
				component.m_has.SetActiveBetter(false);
				return;
			}
			component.m_no_has.SetActiveBetter(false);
			component.m_has.SetActiveBetter(true);
			_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem = _bagItems[_003CFillBoxBagItemData_003Ec__AnonStorey.index];
			_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg = ItemCfg.Get(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.itemId);
			View.SetItemSprite(component.m_icon, _003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.icon);
			View.SetLabelText(component.txt_num, _003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.number);
			component.m_select.SetActiveBetter(false);
			component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.instanceId));
			if (_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.durability > 0)
			{
				component.m_durability.SetActiveBetter(true);
				View.SetSlider(component.m_durability, (float)_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.durability);
			}
			else
			{
				component.m_durability.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__1;
			DragListener.Get(go).onBeginDrag = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__2;
			DragListener.Get(go).onDrag = OnDragMask;
			DragListener.Get(go).onEndDrag = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__3;
			PressListener.Get(go).onPress = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__4;
			DownUpListener.Get(go).onDown = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__5;
		}

		private void OnDragFromBagZone()
		{
			m_drag_icon.SetActiveBetter(false);
			int dragEndZone = _dragEndZone;
			if (dragEndZone != 1 && dragEndZone == 2)
			{
				LoadBullet(_dragBagItem);
			}
			ClearDrag();
		}

		private void OnDragFromBullletZone()
		{
			m_drag_icon.SetActiveBetter(false);
			switch (_dragEndZone)
			{
			case 1:
				if (_turretBuildingInfo.bullets.ContainsKey(_dragIndex))
				{
					RemoveBullet(_turretBuildingInfo.bullets[_dragIndex]);
				}
				break;
			case 2:
			{
				if (_dragIndex == _enterIndex || !_turretBuildingInfo.bullets.ContainsKey(_dragIndex))
				{
					break;
				}
				UseItem useItem = _turretBuildingInfo.bullets[_dragIndex];
				int clipItemId = GetClipItemId(_enterIndex);
				if (clipItemId == useItem.id)
				{
					if (!IsFillClip(_enterIndex))
					{
						ChangeBullet(_turretBuildingInfo.bullets[_dragIndex], _dragIndex, _enterIndex);
					}
				}
				else
				{
					ChangeBullet(_turretBuildingInfo.bullets[_dragIndex], _dragIndex, _enterIndex);
				}
				break;
			}
			}
			ClearDrag();
		}

		private void ClearDrag()
		{
			_dragBagItem = null;
			_dragUseItem = null;
			_dragIndex = 0;
			_enterIndex = 0;
			_dragEndZone = 0;
			_dragFromeZone = 0;
			m_drag_icon.SetActiveBetter(false);
		}

		public void OnBeginDrag(BagItem bagItem, int form)
		{
			m_drag_icon.SetActiveBetter(true);
			SetIcon(m_drag_icon, bagItem.itemId);
			OnDragMask(null);
		}

		public void OnBeginDrag(UseItem useItem, int form)
		{
			m_drag_icon.SetActiveBetter(true);
			SetIcon(m_drag_icon, useItem.id);
			OnDragMask(null);
		}

		private void SetIcon(GameObject go, int itemId)
		{
			ItemCfg itemCfg = ItemCfg.Get(itemId);
			if (itemCfg == null)
			{
				go.SetActiveBetter(false);
				return;
			}
			go.SetActiveBetter(true);
			View.SetItemSprite(go, itemCfg.icon, true);
		}

		public void OnDragMask(GameObject go)
		{
			Vector2 localPoint;
			if (m_drag_icon.activeSelf && !(_canvas == null) && RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, DragListener.pointEventData.position, _canvas.worldCamera, out localPoint))
			{
				m_drag_icon.GetComponent<RectTransform>().anchoredPosition = localPoint;
			}
		}

		private void UndateBagNoposData()
		{
			_bagScrollPanel.UpdateAllCell(UpdateCellData);
		}

		private void UpdateCellData(GameObject go, int index)
		{
			GTurretBagItem component = go.GetComponent<GTurretBagItem>();
			if (index >= _bagItems.Count)
			{
				component.m_no_has.SetActiveBetter(true);
				component.m_has.SetActiveBetter(false);
				return;
			}
			component.m_no_has.SetActiveBetter(false);
			component.m_has.SetActiveBetter(true);
			BagItem bagItem = _bagItems[index];
			component.m_select.SetActiveBetter(false);
			component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(bagItem.instanceId));
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		protected override void onHide(string childView = null)
		{
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			STurretBuildingInfo.handler = (STurretBuildingInfo.Handler)Delegate.Combine(STurretBuildingInfo.handler, new STurretBuildingInfo.Handler(STurretBuildingInfoHandle));
			SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(SBuildingStatusChangeHandle));
			Utils.TriggerEvent(TurrentEvent.CloseTurrentDelegate, _turretBuildingInfo.turretId);
		}

		private void SBuildingStatusChangeHandle(SBuildingStatusChange msg)
		{
			RefreshIsOpen();
		}

		protected override void onDestroy()
		{
		}

		private void SetTurretInfo()
		{
			ItemCfg itemCfg = ItemCfg.Get(_turretBuildingInfo.turretBaseId);
			View.SetItemSprite(m_gun_icon, itemCfg.icon);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_descText, itemCfg.desc);
		}

		private void SetTurretBulles()
		{
			for (int i = 0; i < m_bullets.Length; i++)
			{
				int key = i + 1;
				GTurretBulletItem gTurretBulletItem = m_bulletslist[i];
				gTurretBulletItem.m_select.SetActiveBetter(_currentSelectIndex - 1 == i);
				UseItem value;
				if (_turretBuildingInfo.bullets.TryGetValue(key, out value))
				{
					ItemCfg itemCfg = ItemCfg.Get(value.id);
					if (itemCfg == null)
					{
						gTurretBulletItem.m_icon.SetActiveBetter(false);
						gTurretBulletItem.txt_num.SetActiveBetter(false);
						continue;
					}
					gTurretBulletItem.m_icon.SetActiveBetter(true);
					gTurretBulletItem.txt_num.SetActiveBetter(true);
					View.SetItemSprite(gTurretBulletItem.m_icon, itemCfg.icon);
					View.SetLabelText(gTurretBulletItem.txt_numText, value.num);
				}
				else
				{
					gTurretBulletItem.m_icon.SetActiveBetter(false);
					gTurretBulletItem.txt_num.SetActiveBetter(false);
				}
			}
		}

		private void OnClickAllBullet(GameObject go)
		{
		}

		private void OnClickPermissions(GameObject go)
		{
		}

		private void LoadBullet(BagItem bagItem)
		{
			int clipItemId = GetClipItemId(_enterIndex);
			if (clipItemId == bagItem.itemId)
			{
				if (!IsFillClip(_enterIndex))
				{
					TurretSplitPanel.ShowSplitPanel(bagItem, _enterIndex, LoadBullet);
				}
			}
			else
			{
				TurretSplitPanel.ShowSplitPanel(bagItem, _enterIndex, LoadBullet);
			}
		}

		private void LoadBullet(long instanceId, int itemId, int splitNum, int index)
		{
			ItemCfg itemCfg = ItemCfg.Get(_turretBuildingInfo.turretBaseId);
			int num = (int)itemCfg.extras[index];
			int num2 = 0;
			if (_turretBuildingInfo.bullets.ContainsKey(index))
			{
				UseItem useItem = _turretBuildingInfo.bullets[index];
				if (itemId == useItem.id)
				{
					int num3 = num - useItem.num;
					num2 = ((splitNum <= num3) ? splitNum : num3);
				}
				else
				{
					num2 = ((splitNum <= num) ? splitNum : num);
				}
			}
			else
			{
				num2 = ((splitNum <= num) ? splitNum : num);
			}
			Dictionary<int, UseItem> dictionary = new Dictionary<int, UseItem>();
			UseItem useItem2 = new UseItem();
			useItem2.id = itemId;
			useItem2.num = num2;
			dictionary[index] = useItem2;
			Singleton<TurretMgr>.Ins.AddBullets(_turretBuildingInfo.turretId, dictionary);
		}

		private void RemoveBullet(UseItem useItem)
		{
			RemoveBullet(useItem.instanceId, useItem.id, useItem.num, _dragIndex);
		}

		private void RemoveBullet(UseItem useItem, int index)
		{
			TurretSplitPanel.ShowSplitPanel(useItem, index, RemoveBullet);
		}

		private void RemoveBullet(long instanceId, int itemId, int splitNum, int index)
		{
			Dictionary<int, UseItem> dictionary = new Dictionary<int, UseItem>();
			UseItem useItem = new UseItem();
			useItem.instanceId = (int)instanceId;
			useItem.id = itemId;
			useItem.num = splitNum;
			dictionary[index] = useItem;
			Singleton<TurretMgr>.Ins.RemoveBulletsToBag(_turretBuildingInfo.turretId, dictionary);
		}

		private void RemoveBullet(BagItem bagItem, int splitNum, int index)
		{
			Dictionary<int, UseItem> dictionary = new Dictionary<int, UseItem>();
			UseItem useItem = new UseItem();
			useItem.id = bagItem.itemId;
			useItem.num = splitNum;
			dictionary[index] = useItem;
			Singleton<TurretMgr>.Ins.RemoveBulletsToBag(_turretBuildingInfo.turretId, dictionary);
		}

		private void ChangeBullet(UseItem useItem, int dragIndex, int enterIndex)
		{
			Singleton<TurretMgr>.Ins.ReportManagementTurret(_turretBuildingInfo.turretId, dragIndex, enterIndex);
		}

		private bool IsFillClip(int index)
		{
			ItemCfg itemCfg = ItemCfg.Get(_turretBuildingInfo.turretBaseId);
			int num = (int)itemCfg.extras[index];
			if (_turretBuildingInfo.bullets.ContainsKey(index))
			{
				UseItem useItem = _turretBuildingInfo.bullets[index];
				if (useItem.num >= num)
				{
					return true;
				}
			}
			return false;
		}

		private int GetClipItemId(int index)
		{
			if (_turretBuildingInfo.bullets.ContainsKey(index))
			{
				UseItem useItem = _turretBuildingInfo.bullets[index];
				return useItem.id;
			}
			return 0;
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
			m_cell = View.AddComponentIfNotExist<GTurretBagItem>(component.GameObjects[7].gameObject);
			txt_bag_num = component.GameObjects[8].gameObject;
			txt_bag_numText = txt_bag_num.GetComponent<Text>();
			m_gun_frame = component.GameObjects[9].gameObject;
			m_gun_icon = component.GameObjects[10].gameObject;
			txt_name = component.GameObjects[11].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_duration = component.GameObjects[12].gameObject;
			txt_durationText = txt_duration.GetComponent<Text>();
			txt_desc = component.GameObjects[13].gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			m_bullets = component.GameObjects[14].gameObject.GetComponent<UIGameObjectList>().objects;
			m_bulletsObj = component.GameObjects[14].gameObject;
			if (m_bulletslist.Count <= 0)
			{
				for (int i = 0; i < m_bullets.Length; i++)
				{
					m_bulletslist.Add(View.AddComponentIfNotExist<GTurretBulletItem>(m_bullets[i].gameObject));
				}
			}
			m_bullet_item = View.AddComponentIfNotExist<GTurretBulletItem>(component.GameObjects[15].gameObject);
			btn_battle_friend = component.GameObjects[16].gameObject;
			btn_put_all_bullet = component.GameObjects[17].gameObject;
			btn_set_power = component.GameObjects[18].gameObject;
			ckb_team = component.GameObjects[19].gameObject;
			m_drag_Canvas = component.GameObjects[20].gameObject;
			m_drag_icon = component.GameObjects[21].gameObject;
			m_turrent_switch = component.GameObjects[22].gameObject;
			m_turrent_switch_on = component.GameObjects[23].gameObject;
			m_turrent_switch_off = component.GameObjects[24].gameObject;
			m_bullet_item_0 = View.AddComponentIfNotExist<GTurretBulletItem>(component.GameObjects[25].gameObject);
			m_bullet_item_1 = View.AddComponentIfNotExist<GTurretBulletItem>(component.GameObjects[26].gameObject);
			m_bullet_item_2 = View.AddComponentIfNotExist<GTurretBulletItem>(component.GameObjects[27].gameObject);
			txt_on_0 = component.GameObjects[28].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off = component.GameObjects[29].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_on_1 = component.GameObjects[30].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_0 = component.GameObjects[31].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
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
			Singleton<TurretMgr>.Ins.ChangeTurretStatus(_turretBuildingInfo.turretId, true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Singleton<TurretMgr>.Ins.ChangeTurretStatus(_turretBuildingInfo.turretId, false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			ViewMgr.Ins.ShowView<TurretPermitPanel>(_turretBuildingInfo, false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			Singleton<TurretMgr>.Ins.EditorTurret(_turretBuildingInfo.turretId, _turretBuildingInfo.isAttackAllies, View.IsCheckboxChecked(ckb_team));
		}
	}
}
