using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.item.create.scmsg;

namespace SC.UI
{
	public class BagPage : MonoBehaviour, IBagAndBuildPage
	{
		public enum BagPageType
		{
			Equipment = 0,
			Weapon = 1,
			EquipDesc = 2,
			ItemDesc = 3
		}

		[CompilerGenerated]
		private sealed class _003CFillBagItemData_003Ec__AnonStorey0
		{
			internal BagItem bagItem;

			internal GameObject go;

			internal bool isShowNewbieEffect;

			internal GBagItem gBagItem;

			internal ItemCfg itemCfg;

			internal int index;

			internal BagPage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._bagScrollRect.OnBeginDrag(DragListener.pointEventData);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this._bagScrollRect.OnEndDrag(DragListener.pointEventData);
			}

			internal void _003C_003Em__2(GameObject o)
			{
				_0024this._bagScrollRect.OnDrag(DragListener.pointEventData);
			}

			internal void _003C_003Em__3(GameObject o)
			{
				_0024this.OnEnter(1);
			}

			internal void _003C_003Em__4(GameObject o)
			{
				_0024this._isDrag = false;
				_0024this.DragBagItem = bagItem;
				_0024this._startDragTime = 0.2f;
				_0024this._offset = Input.mousePosition - go.GetComponent<RectTransform>().localPosition;
				_0024this._onDownX = Input.mousePosition.x;
				_0024this._onDownY = Input.mousePosition.y;
				if (isShowNewbieEffect)
				{
					gBagItem.m_novice_effect.SetActiveBetter(false);
					Utils.TriggerEvent(GuideEvent.StepCompleteAction);
				}
			}

			internal void _003C_003Em__5(GameObject o)
			{
				if (_0024this._isDrag)
				{
					_0024this.OnDragFromBagZone();
					_0024this._bagScrollRect.enabled = true;
					return;
				}
				if (Mathf.Abs(_0024this._onDownX - Input.mousePosition.x) < 10f && Mathf.Abs(_0024this._onDownY - Input.mousePosition.y) < 10f)
				{
					_0024this._currentInstanceId = bagItem.instanceId;
					if (itemCfg.type == 13)
					{
						_0024this.SetWeaponData(bagItem, 1);
					}
					else if (itemCfg.type == 119)
					{
						_0024this.DragFromZone = 1;
						_0024this.SetEquipDescItemDescData(bagItem, 1);
					}
					else
					{
						_0024this.DragFromZone = 1;
						_0024this.SetItemDescData(bagItem, 1);
					}
					_0024this.UpdateBagNoposData();
					_0024this.SetQuickUseData();
				}
				_0024this._startDragTime = 0f;
			}

			internal void _003C_003Em__6(GameObject o)
			{
				_0024this._bagScrollRect.OnBeginDrag(DragListener.pointEventData);
			}

			internal void _003C_003Em__7(GameObject o)
			{
				_0024this._bagScrollRect.OnEndDrag(DragListener.pointEventData);
			}

			internal void _003C_003Em__8(GameObject o)
			{
				_0024this._bagScrollRect.OnDrag(DragListener.pointEventData);
				_0024this.OnDragMask(o);
			}

			internal void _003C_003Em__9(GameObject o)
			{
				_0024this.OnEnter(1, index);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillQuickUseItemData_003Ec__AnonStorey2
		{
			internal BagItem bagItem;

			internal int index;

			internal BagPage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_0024this.m_quick_use_mask.activeInHierarchy)
				{
					_0024this.OnDragEndQuickUseZone();
				}
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this.OnEnter(2, index);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillQuickUseItemData_003Ec__AnonStorey1
		{
			internal ItemCfg itemCfg;

			internal _003CFillQuickUseItemData_003Ec__AnonStorey2 _003C_003Ef__ref_00242;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_003C_003Ef__ref_00242._0024this.m_quick_use_mask.activeInHierarchy)
				{
					_003C_003Ef__ref_00242._0024this.OnDragEndQuickUseZone();
					return;
				}
				if (itemCfg.type == 13)
				{
					_003C_003Ef__ref_00242._0024this.SetWeaponData(_003C_003Ef__ref_00242.bagItem, 2);
				}
				else if (itemCfg.type == 119)
				{
					_003C_003Ef__ref_00242._0024this.DragFromZone = 2;
					_003C_003Ef__ref_00242._0024this.SetEquipDescItemDescData(_003C_003Ef__ref_00242.bagItem, 2);
				}
				else
				{
					_003C_003Ef__ref_00242._0024this.DragFromZone = 2;
					_003C_003Ef__ref_00242._0024this.SetItemDescData(_003C_003Ef__ref_00242.bagItem, 2);
				}
				_003C_003Ef__ref_00242._0024this._currentInstanceId = _003C_003Ef__ref_00242.bagItem.instanceId;
				_003C_003Ef__ref_00242._0024this.SetBagNoposData();
				_003C_003Ef__ref_00242._0024this.SetQuickUseData();
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_003C_003Ef__ref_00242._0024this.DragBagItem = _003C_003Ef__ref_00242.bagItem;
				_003C_003Ef__ref_00242._0024this.OnBeginDrag(_003C_003Ef__ref_00242._0024this.DragBagItem, 2);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_003C_003Ef__ref_00242._0024this.OnDragFromQuickUseZone();
			}
		}

		private UIScrollPanel _bagScrollPanel;

		private ScrollRect _bagScrollRect;

		public List<BagItem> BagItems;

		public Dictionary<int, BagItem> QuickUseItems;

		public Dictionary<int, GunParts> _gunDic = new Dictionary<int, GunParts>();

		private int _currentInstanceId = -1;

		private float _startDragTime;

		private int _bagCapacity;

		private Canvas _canvas;

		public const int BagZone = 1;

		public const int QuickUseZone = 2;

		public const int GunPartZone = 3;

		public const int EquipmentZone = 4;

		private BagItem _currentDescBagItem;

		public BagItem DragBagItem;

		public int DragToZone = -1;

		public int DragToZoneIndex = -1;

		public int DragFromZone = -1;

		public int DragEndZone = -1;

		private float _onDownX;

		private float _onDownY;

		private bool _isDrag;

		private Vector3 _offset;

		public List<Transform> TargetTransforms = new List<Transform>();

		public List<Transform> TargetParentTransforms = new List<Transform>();

		public BagPageType _currentBagPageType;

		public GameObject m_bag_zone;

		public GBagItem m_cell;

		public BagGDesc m_desc;

		public GameObject m_drag_Canvas;

		public GameObject m_drag_icon;

		public BagGEquipDesc m_equip_desc;

		public BagGEquipment m_equipment;

		public GameObject m_quick_use_mask;

		public List<GShortcutBarItem> m_shortcut_barlist = new List<GShortcutBarItem>();

		public GameObject[] m_shortcut_bar;

		public GameObject m_shortcut_barObj;

		public BagGWeapon m_weapon;

		public GameObject m_zuo;

		public GameObject scp_bag;

		public GameObject txt_bag_num;

		public Text txt_bag_numText;

		public object context;

		public bool IsShow
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public void OnEnter(int enterZone, int enterIndex)
		{
			DragEndZone = enterZone;
			DragToZoneIndex = enterIndex;
		}

		public void OnEnter(int enterZone)
		{
			OnEnter(enterZone, -1);
		}

		public void OnDragEndQuickUseZone()
		{
			switch (DragFromZone)
			{
			case 1:
				Singleton<BagMgr>.Ins.SetQuickUseItem(_currentInstanceId, DragToZoneIndex);
				break;
			case 3:
			{
				int partItemIdByInstanceId = Singleton<BagMgr>.Ins.GetPartItemIdByInstanceId(_currentInstanceId, DragBagItem.instanceId);
				Singleton<BagMgr>.Ins.RemoveGunPart(_currentInstanceId, partItemIdByInstanceId, false);
				break;
			}
			}
			ClearDrag();
		}

		public void OnDragFromGunPartZone()
		{
			switch (DragEndZone)
			{
			case 1:
			{
				ItemCfg itemCfg = ItemCfg.Get(DragBagItem.itemId);
				if (itemCfg.type == 24)
				{
					Singleton<BagMgr>.Ins.RemoveGunBullet(_currentInstanceId, DragBagItem.instanceId, DragBagItem.number);
					break;
				}
				int partItemIdByInstanceId = Singleton<BagMgr>.Ins.GetPartItemIdByInstanceId(_currentInstanceId, DragBagItem.instanceId);
				Singleton<BagMgr>.Ins.RemoveGunPart(_currentInstanceId, partItemIdByInstanceId, true);
				break;
			}
			case 2:
			{
				int partItemIdByInstanceId = Singleton<BagMgr>.Ins.GetPartItemIdByInstanceId(_currentInstanceId, DragBagItem.instanceId);
				Singleton<BagMgr>.Ins.RemoveGunPart(_currentInstanceId, partItemIdByInstanceId, false);
				break;
			}
			}
			ClearDrag();
		}

		public void OnDragFromQuickUseZone()
		{
			switch (DragEndZone)
			{
			case 1:
				if (DragToZoneIndex > -1)
				{
					BagItem bagItem = BagItems[DragToZoneIndex];
					if (bagItem.itemId == DragBagItem.itemId)
					{
						ItemCfg itemCfg2 = ItemCfg.Get(bagItem.itemId);
						int num = itemCfg2.maxPileNum - bagItem.number;
						int num2 = ((DragBagItem.number <= num) ? DragBagItem.number : num);
						Singleton<BagMgr>.Ins.PutTogether(bagItem.instanceId, DragBagItem.instanceId, num2, false);
					}
					else
					{
						Singleton<BagMgr>.Ins.SetQuickUseItem(DragBagItem.instanceId, -1);
					}
				}
				else
				{
					Singleton<BagMgr>.Ins.SetQuickUseItem(DragBagItem.instanceId, DragToZoneIndex);
				}
				break;
			case 2:
				Singleton<BagMgr>.Ins.SetQuickUseItem(DragBagItem.instanceId, DragToZoneIndex);
				break;
			case 3:
				Singleton<BagMgr>.Ins.PutGunPart(_currentInstanceId, DragBagItem.instanceId);
				break;
			case 4:
			{
				ItemCfg itemCfg = ItemCfg.Get(DragBagItem.itemId);
				if (BagMgr.IsEquipType(itemCfg.type))
				{
					Singleton<BagMgr>.Ins.PutEquip(DragBagItem.instanceId);
				}
				else if (BagMgr.IsSkinType(itemCfg.type))
				{
					Singleton<BagMgr>.Ins.PutSkin(DragBagItem.instanceId);
				}
				break;
			}
			}
			ClearDrag();
		}

		public void OnDragFromBagZone()
		{
			ItemCfg itemCfg = ItemCfg.Get(DragBagItem.itemId);
			switch (DragEndZone)
			{
			case 1:
				if (DragToZoneIndex > -1)
				{
					BagItem bagItem = BagItems[DragToZoneIndex];
					if (bagItem.itemId == DragBagItem.itemId)
					{
						ItemCfg itemCfg3 = ItemCfg.Get(bagItem.itemId);
						int num2 = itemCfg3.maxPileNum - bagItem.number;
						int num3 = ((DragBagItem.number <= num2) ? DragBagItem.number : num2);
						Singleton<BagMgr>.Ins.PutTogether(bagItem.instanceId, DragBagItem.instanceId, num3, true);
					}
				}
				break;
			case 2:
				if (QuickUseItems.ContainsKey(DragToZoneIndex))
				{
					BagItem quickUseItemByIndex = Singleton<BagMgr>.Ins.GetQuickUseItemByIndex(DragToZoneIndex);
					if (quickUseItemByIndex.itemId == DragBagItem.itemId && itemCfg.isPileAble && itemCfg.maxPileNum > 1)
					{
						int num = itemCfg.maxPileNum - quickUseItemByIndex.number;
						num = ((num <= DragBagItem.number) ? num : DragBagItem.number);
						if (num > 0)
						{
							Singleton<BagMgr>.Ins.PutTogether(quickUseItemByIndex.instanceId, DragBagItem.instanceId, num, true);
							break;
						}
					}
				}
				Singleton<BagMgr>.Ins.SetQuickUseItem(DragBagItem.instanceId, DragToZoneIndex);
				break;
			case 3:
				if (itemCfg.type == 24)
				{
					Singleton<BagMgr>.Ins.LoadGunBullet(_currentInstanceId, DragBagItem.instanceId, DragBagItem.number);
				}
				else
				{
					Singleton<BagMgr>.Ins.PutGunPart(_currentInstanceId, DragBagItem.instanceId);
				}
				break;
			case 4:
			{
				ItemCfg itemCfg2 = ItemCfg.Get(DragBagItem.itemId);
				if (BagMgr.IsEquipType(itemCfg2.type))
				{
					Singleton<BagMgr>.Ins.PutEquip(DragBagItem.instanceId);
				}
				else if (BagMgr.IsSkinType(itemCfg2.type))
				{
					Singleton<BagMgr>.Ins.PutSkin(DragBagItem.instanceId);
				}
				break;
			}
			default:
				AlertBox.Show(327);
				break;
			}
			ClearDrag();
		}

		public void OnDragFromEquipmentZone()
		{
			switch (DragEndZone)
			{
			case 1:
			case 2:
				RemoveSkinOrEquip(DragBagItem);
				break;
			}
			ClearDrag();
		}

		public void OnInit()
		{
			_canvas = GameObject.Find("UIRootCanvas").GetComponent<Canvas>();
			ClickListener.Get(m_quick_use_mask, string.Empty).onClick = _003COnInit_003Em__0;
			m_quick_use_mask.SetActiveBetter(false);
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_bagScrollRect = scp_bag.GetComponent<ScrollRect>();
			BagItems = Singleton<BagMgr>.Ins.BagItems;
			QuickUseItems = Singleton<BagMgr>.Ins.QuickUseItems;
			_bagCapacity = Singleton<BagMgr>.Ins.BagCapacity;
			_gunDic = Singleton<BagMgr>.Ins.GunDic;
			m_drag_icon.SetActiveBetter(false);
			m_desc.gameObject.SetActiveBetter(false);
			m_desc.Init(this);
			m_weapon.Init(this);
			m_equipment.Init(this);
			m_equip_desc.Init(this);
			OnClickBackDesc(null);
			m_bag_zone.SetActiveBetter(false);
			m_weapon.m_weapon_zone.SetActiveBetter(false);
			m_equipment.m_equipment_zone.SetActiveBetter(false);
		}

		public void OnShow(object param = null)
		{
			BagEvent.RefreshBag = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshBag, new Utils.VoidDelegate(RefreshBagDelegate));
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUseDelegate));
			BagEvent.RefreshQuickUseByIndex = (Utils.IntDelegate)Delegate.Combine(BagEvent.RefreshQuickUseByIndex, new Utils.IntDelegate(RefreshQuickUseByIndexDelegate));
			BagEvent.SetQuickDelegate = (Utils.VoidDelegate)Delegate.Combine(BagEvent.SetQuickDelegate, new Utils.VoidDelegate(SetQuickDelegate));
			BagEvent.OutQuickDelegate = (Utils.VoidDelegate)Delegate.Combine(BagEvent.OutQuickDelegate, new Utils.VoidDelegate(OutQuickDelegate));
			BagEvent.RefreshBullet = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshBullet, new Utils.VoidDelegate(RefreshBulletDelegate));
			SLearnDrawing.handler = (SLearnDrawing.Handler)Delegate.Combine(SLearnDrawing.handler, new SLearnDrawing.Handler(OnSLearnDrawing));
			BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Combine(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeHandInstanceIdDelegate));
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			BagEvent.DiscardDelegate = (Utils.VoidDelegate)Delegate.Combine(BagEvent.DiscardDelegate, new Utils.VoidDelegate(DiscardDelegate));
			m_weapon.OnShow();
			m_equipment.OnShow();
			m_desc.OnShow();
			SetBagData();
			SetQuickUseData();
			m_equipment.SetData(null);
			base.gameObject.SetActiveBetter(true);
		}

		private void DiscardDelegate()
		{
			OnClickBackDesc(null);
		}

		private void SItemChangedHandle(SItemChanged msg)
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(msg.instanceId);
			if (allItemByInstanceId == null)
			{
				OnClickBackDesc(null);
			}
		}

		private void ChangeHandInstanceIdDelegate(long arg)
		{
			OnClickBackDesc(null);
		}

		private void RefreshBulletDelegate()
		{
			UpdateBagWeaponNoposData();
			UpdateQuickUseWeaponData();
		}

		private void OnSLearnDrawing(SLearnDrawing msg)
		{
			ViewMgr.Ins.ShowView<DrawDescribePanel>(msg.learnedDrawId, false);
		}

		private void OutQuickDelegate()
		{
			_currentInstanceId = -1;
			m_equipment.SetData(null);
		}

		private void SetQuickDelegate()
		{
			_currentInstanceId = -1;
			m_equipment.SetData(null);
			m_weapon.gameObject.SetActiveBetter(false);
		}

		private void RefreshQuickUseByIndexDelegate(int arg)
		{
			m_equipment.SetEquipment();
			m_equipment.OnClickEuipmentR(null);
		}

		private void RefreshQuickUseDelegate()
		{
			SetQuickUseData();
		}

		private void RefreshBagDelegate()
		{
			SetBagNoposData();
		}

		public void OnHide()
		{
			BagEvent.RefreshBag = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshBag, new Utils.VoidDelegate(RefreshBagDelegate));
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUseDelegate));
			BagEvent.RefreshQuickUseByIndex = (Utils.IntDelegate)Delegate.Remove(BagEvent.RefreshQuickUseByIndex, new Utils.IntDelegate(RefreshQuickUseByIndexDelegate));
			BagEvent.SetQuickDelegate = (Utils.VoidDelegate)Delegate.Remove(BagEvent.SetQuickDelegate, new Utils.VoidDelegate(SetQuickDelegate));
			BagEvent.OutQuickDelegate = (Utils.VoidDelegate)Delegate.Remove(BagEvent.OutQuickDelegate, new Utils.VoidDelegate(OutQuickDelegate));
			SLearnDrawing.handler = (SLearnDrawing.Handler)Delegate.Remove(SLearnDrawing.handler, new SLearnDrawing.Handler(OnSLearnDrawing));
			BagEvent.RefreshBullet = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshBullet, new Utils.VoidDelegate(RefreshBulletDelegate));
			BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Remove(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeHandInstanceIdDelegate));
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Remove(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			BagEvent.DiscardDelegate = (Utils.VoidDelegate)Delegate.Remove(BagEvent.DiscardDelegate, new Utils.VoidDelegate(DiscardDelegate));
			m_weapon.OnHide();
			m_equipment.OnHide();
			m_desc.OnHide();
			ClearDrag();
			base.gameObject.SetActiveBetter(false);
			_currentInstanceId = -1;
		}

		private void SetBagData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_bagCapacity, FillBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, BagItems.Count, _bagCapacity));
		}

		private void SetBagNoposData()
		{
			_bagScrollPanel.ResetNoPos(_bagCapacity, FillBagItemData);
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, BagItems.Count, _bagCapacity));
		}

		private void UpdateBagNoposData()
		{
			_bagScrollPanel.UpdateAllCell(UpdateItemData);
		}

		private void UpdateItemData(GameObject go, int index)
		{
			GBagItem component = go.GetComponent<GBagItem>();
			if (index < BagItems.Count)
			{
				BagItem bagItem = BagItems[index];
				component.m_select.SetActiveBetter(bagItem.instanceId == _currentInstanceId);
				component.m_equip_select.SetActiveBetter(false);
				component.m_binding.SetActiveBetter(bagItem.isBind);
			}
		}

		private void UpdateBagWeaponNoposData()
		{
			_bagScrollPanel.UpdateAllCell(UpdateWeaponItemData);
		}

		private void UpdateWeaponItemData(GameObject go, int index)
		{
			GBagItem component = go.GetComponent<GBagItem>();
			if (index >= BagItems.Count)
			{
				return;
			}
			BagItem bagItem = BagItems[index];
			component.m_binding.SetActiveBetter(bagItem.isBind);
			component.m_select.SetActiveBetter(bagItem.instanceId == _currentInstanceId);
			component.m_equip_select.SetActiveBetter(false);
			ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
			if (itemCfg == null)
			{
				return;
			}
			if (itemCfg.type == 13)
			{
				component.txt_bullet_num.SetActiveBetter(true);
				component.m_gun_part.SetActiveBetter(true);
				View.SetLabelText(component.txt_bullet_numText, _gunDic[bagItem.instanceId].bulletNumber);
				GunCfg gunCfg = GunCfg.Get(itemCfg.id);
				int num = 0;
				if (gunCfg.muzzleParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.aimParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.clipParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.qiangbaParts.Count > 0)
				{
					num++;
				}
				for (int i = 0; i < component.m_gun_part_num_bgs.Length; i++)
				{
					component.m_gun_part_num_bgs[i].SetActiveBetter(i < num);
				}
				for (int j = 0; j < component.m_gun_part_nums.Length; j++)
				{
					component.m_gun_part_nums[j].SetActiveBetter(j < _gunDic[bagItem.instanceId].parts.Count);
				}
			}
			else
			{
				component.m_gun_part.SetActiveBetter(false);
				component.txt_bullet_num.SetActiveBetter(false);
			}
		}

		private void FillBagItemData(GameObject go, int index)
		{
			_003CFillBagItemData_003Ec__AnonStorey0 _003CFillBagItemData_003Ec__AnonStorey = new _003CFillBagItemData_003Ec__AnonStorey0();
			_003CFillBagItemData_003Ec__AnonStorey.go = go;
			_003CFillBagItemData_003Ec__AnonStorey.index = index;
			_003CFillBagItemData_003Ec__AnonStorey._0024this = this;
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem = _003CFillBagItemData_003Ec__AnonStorey.go.GetComponent<GBagItem>();
			if (_003CFillBagItemData_003Ec__AnonStorey.index >= BagItems.Count)
			{
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_no_has.SetActiveBetter(true);
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_has.SetActiveBetter(false);
				DownUpListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onDown = null;
				DownUpListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onUp = null;
				DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onBeginDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onEndDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__2;
				UIEventListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go, string.Empty).onEnter = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__3;
				return;
			}
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_no_has.SetActiveBetter(false);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_has.SetActiveBetter(true);
			_003CFillBagItemData_003Ec__AnonStorey.bagItem = BagItems[_003CFillBagItemData_003Ec__AnonStorey.index];
			_003CFillBagItemData_003Ec__AnonStorey.itemCfg = ItemCfg.Get(_003CFillBagItemData_003Ec__AnonStorey.bagItem.itemId);
			_003CFillBagItemData_003Ec__AnonStorey.isShowNewbieEffect = Singleton<GuideMgr>.Ins.IsBagItemEffectShow(_003CFillBagItemData_003Ec__AnonStorey.bagItem.itemId);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_novice_effect.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.isShowNewbieEffect);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_binding.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.bagItem.isBind);
			if (_003CFillBagItemData_003Ec__AnonStorey.itemCfg == null)
			{
				Debug.LogError("itemCfg is null,id:" + _003CFillBagItemData_003Ec__AnonStorey.bagItem.itemId);
				return;
			}
			if (_003CFillBagItemData_003Ec__AnonStorey.itemCfg.type == 13)
			{
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.txt_bullet_num.SetActiveBetter(true);
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part.SetActiveBetter(true);
				View.SetLabelText(_003CFillBagItemData_003Ec__AnonStorey.gBagItem.txt_bullet_numText, _gunDic[_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId].bulletNumber);
				GunCfg gunCfg = GunCfg.Get(_003CFillBagItemData_003Ec__AnonStorey.itemCfg.id);
				int num = 0;
				if (gunCfg.muzzleParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.aimParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.clipParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.qiangbaParts.Count > 0)
				{
					num++;
				}
				for (int i = 0; i < _003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part_num_bgs.Length; i++)
				{
					_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part_num_bgs[i].SetActiveBetter(i < num);
				}
				for (int j = 0; j < _003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part_nums.Length; j++)
				{
					_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part_nums[j].SetActiveBetter(j < _gunDic[_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId].parts.Count);
				}
			}
			else
			{
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_gun_part.SetActiveBetter(false);
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.txt_bullet_num.SetActiveBetter(false);
			}
			View.SetItemSprite(_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_icon, _003CFillBagItemData_003Ec__AnonStorey.itemCfg.icon);
			View.SetLabelText(_003CFillBagItemData_003Ec__AnonStorey.gBagItem.txt_numText, _003CFillBagItemData_003Ec__AnonStorey.bagItem.number);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.txt_num.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.itemCfg.type != 13);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId));
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_select.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId == _currentInstanceId);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_equip_select.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId == Singleton<BagMgr>.Ins.HandInstanceId);
			_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_equip_select.SetActiveBetter(false);
			if (_003CFillBagItemData_003Ec__AnonStorey.itemCfg.durability > 0)
			{
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_durability.SetActiveBetter(true);
				View.SetSlider(_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_durability, (float)_003CFillBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)_003CFillBagItemData_003Ec__AnonStorey.itemCfg.durability);
			}
			else
			{
				_003CFillBagItemData_003Ec__AnonStorey.gBagItem.m_durability.SetActiveBetter(false);
			}
			DownUpListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onDown = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__4;
			DownUpListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onUp = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__5;
			DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onBeginDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__6;
			DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onEndDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__7;
			DragListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go).onDrag = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__8;
			UIEventListener.Get(_003CFillBagItemData_003Ec__AnonStorey.go, string.Empty).onEnter = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__9;
		}

		private void SetQuickUseData()
		{
			for (int i = 0; i < m_shortcut_bar.Length; i++)
			{
				FillQuickUseItemData(m_shortcut_barlist[i], i);
			}
		}

		private void UpdateQuickUseWeaponData()
		{
			for (int i = 0; i < m_shortcut_bar.Length; i++)
			{
				UpdateQuickUseWeaponItemData(m_shortcut_barlist[i], i);
			}
		}

		private void UpdateQuickUseWeaponItemData(GShortcutBarItem gShortcutBarItem, int index)
		{
			BagItem quickUseItemByIndex = Singleton<BagMgr>.Ins.GetQuickUseItemByIndex(index);
			gShortcutBarItem.m_gun_part.SetActiveBetter(false);
			if (quickUseItemByIndex == null)
			{
				return;
			}
			gShortcutBarItem.txt_num.SetActiveBetter(true);
			gShortcutBarItem.m_select.SetActiveBetter(quickUseItemByIndex.instanceId == _currentInstanceId);
			gShortcutBarItem.m_equip_select.SetActiveBetter(false);
			ItemCfg itemCfg = ItemCfg.Get(quickUseItemByIndex.itemId);
			if (itemCfg == null)
			{
				return;
			}
			if (itemCfg.type == 13)
			{
				gShortcutBarItem.txt_num.SetActiveBetter(false);
				gShortcutBarItem.m_gun_part.SetActiveBetter(true);
				gShortcutBarItem.txt_bullet_num.SetActiveBetter(true);
				View.SetLabelText(gShortcutBarItem.txt_bullet_numText, _gunDic[quickUseItemByIndex.instanceId].bulletNumber);
				GunCfg gunCfg = GunCfg.Get(itemCfg.id);
				int num = 0;
				if (gunCfg.muzzleParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.aimParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.clipParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.qiangbaParts.Count > 0)
				{
					num++;
				}
				for (int i = 0; i < gShortcutBarItem.m_gun_part_num_bgs.Length; i++)
				{
					gShortcutBarItem.m_gun_part_num_bgs[i].SetActiveBetter(i < num);
				}
				for (int j = 0; j < gShortcutBarItem.m_gun_part_nums.Length; j++)
				{
					gShortcutBarItem.m_gun_part_nums[j].SetActiveBetter(j < _gunDic[quickUseItemByIndex.instanceId].parts.Count);
				}
			}
			else
			{
				gShortcutBarItem.m_gun_part.SetActiveBetter(false);
				gShortcutBarItem.txt_bullet_num.SetActiveBetter(false);
				gShortcutBarItem.txt_num.SetActiveBetter(true);
			}
		}

		private void FillQuickUseItemData(GShortcutBarItem gShortcutBarItem, int index)
		{
			_003CFillQuickUseItemData_003Ec__AnonStorey2 _003CFillQuickUseItemData_003Ec__AnonStorey = new _003CFillQuickUseItemData_003Ec__AnonStorey2();
			_003CFillQuickUseItemData_003Ec__AnonStorey.index = index;
			_003CFillQuickUseItemData_003Ec__AnonStorey._0024this = this;
			_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem = Singleton<BagMgr>.Ins.GetQuickUseItemByIndex(_003CFillQuickUseItemData_003Ec__AnonStorey.index);
			gShortcutBarItem.m_gun_part.SetActiveBetter(false);
			if (_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem != null)
			{
				_003CFillQuickUseItemData_003Ec__AnonStorey1 _003CFillQuickUseItemData_003Ec__AnonStorey2 = new _003CFillQuickUseItemData_003Ec__AnonStorey1();
				_003CFillQuickUseItemData_003Ec__AnonStorey2._003C_003Ef__ref_00242 = _003CFillQuickUseItemData_003Ec__AnonStorey;
				gShortcutBarItem.m_shortcut_bar_icon.SetActiveBetter(true);
				gShortcutBarItem.txt_num.SetActiveBetter(true);
				gShortcutBarItem.m_select.SetActiveBetter(_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.instanceId == _currentInstanceId);
				gShortcutBarItem.m_equip_select.SetActiveBetter(false);
				gShortcutBarItem.m_bind.SetActiveBetter(_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.isBind);
				_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg = ItemCfg.Get(_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.itemId);
				if (_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg == null)
				{
					return;
				}
				if (_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.type == 13)
				{
					gShortcutBarItem.txt_num.SetActiveBetter(false);
					gShortcutBarItem.m_gun_part.SetActiveBetter(true);
					gShortcutBarItem.txt_bullet_num.SetActiveBetter(true);
					View.SetLabelText(gShortcutBarItem.txt_bullet_numText, _gunDic[_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.instanceId].bulletNumber);
					GunCfg gunCfg = GunCfg.Get(_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.id);
					int num = 0;
					if (gunCfg.muzzleParts.Count > 0)
					{
						num++;
					}
					if (gunCfg.aimParts.Count > 0)
					{
						num++;
					}
					if (gunCfg.clipParts.Count > 0)
					{
						num++;
					}
					if (gunCfg.qiangbaParts.Count > 0)
					{
						num++;
					}
					for (int i = 0; i < gShortcutBarItem.m_gun_part_num_bgs.Length; i++)
					{
						gShortcutBarItem.m_gun_part_num_bgs[i].SetActiveBetter(i < num);
					}
					for (int j = 0; j < gShortcutBarItem.m_gun_part_nums.Length; j++)
					{
						gShortcutBarItem.m_gun_part_nums[j].SetActiveBetter(j < _gunDic[_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.instanceId].parts.Count);
					}
					gShortcutBarItem.m_gun_part_red.SetActiveBetter(Singleton<BagMgr>.Ins.GunHasPartCanUse(_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.id, _gunDic[_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.instanceId].parts));
				}
				else
				{
					gShortcutBarItem.m_gun_part.SetActiveBetter(false);
					gShortcutBarItem.txt_bullet_num.SetActiveBetter(false);
					gShortcutBarItem.txt_num.SetActiveBetter(true);
					gShortcutBarItem.m_gun_part_red.SetActiveBetter(false);
				}
				View.SetItemSprite(gShortcutBarItem.m_shortcut_bar_icon, _003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.icon);
				View.SetLabelText(gShortcutBarItem.txt_num, _003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.number);
				if (_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.durability > 0)
				{
					gShortcutBarItem.m_durability.SetActiveBetter(true);
					View.SetSlider(gShortcutBarItem.m_durability, (float)_003CFillQuickUseItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)_003CFillQuickUseItemData_003Ec__AnonStorey2.itemCfg.durability);
				}
				else
				{
					gShortcutBarItem.m_durability.SetActiveBetter(false);
				}
				DownUpListener.Get(gShortcutBarItem.gameObject).onDown = _003CFillQuickUseItemData_003Ec__AnonStorey2._003C_003Em__0;
				DragListener.Get(gShortcutBarItem.gameObject).onBeginDrag = _003CFillQuickUseItemData_003Ec__AnonStorey2._003C_003Em__1;
				DragListener.Get(gShortcutBarItem.gameObject).onDrag = OnDragMask;
				DragListener.Get(gShortcutBarItem.gameObject).onEndDrag = _003CFillQuickUseItemData_003Ec__AnonStorey2._003C_003Em__2;
			}
			else
			{
				gShortcutBarItem.m_shortcut_bar_icon.SetActiveBetter(false);
				gShortcutBarItem.m_durability.SetActiveBetter(false);
				gShortcutBarItem.txt_num.SetActiveBetter(false);
				gShortcutBarItem.m_select.SetActiveBetter(false);
				gShortcutBarItem.txt_bullet_num.SetActiveBetter(false);
				gShortcutBarItem.m_gun_part_red.SetActiveBetter(false);
				gShortcutBarItem.m_bind.SetActiveBetter(false);
				View.SetCheckbox(gShortcutBarItem.gameObject, false);
				DownUpListener.Get(gShortcutBarItem.gameObject).onDown = _003CFillQuickUseItemData_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(gShortcutBarItem.gameObject).onBeginDrag = null;
				DragListener.Get(gShortcutBarItem.gameObject).onDrag = null;
				DragListener.Get(gShortcutBarItem.gameObject).onEndDrag = null;
			}
			UIEventListener.Get(gShortcutBarItem.gameObject, string.Empty).onEnter = _003CFillQuickUseItemData_003Ec__AnonStorey._003C_003Em__1;
		}

		public void SetItemDescData(BagItem bagItem, int fromZone)
		{
			DragFromZone = fromZone;
			_currentDescBagItem = bagItem;
			m_desc.SetData(bagItem, fromZone);
		}

		public void SetEquipDescItemDescData(BagItem bagItem, int fromZone)
		{
			DragFromZone = fromZone;
			_currentDescBagItem = bagItem;
			m_equip_desc.SetData(bagItem, fromZone);
		}

		public void SetWeaponData(BagItem bagItem, int fromZone)
		{
			DragFromZone = fromZone;
			m_weapon.SetData(bagItem);
		}

		private void ClearDrag()
		{
			_startDragTime = 0f;
			m_quick_use_mask.SetActiveBetter(false);
			m_bag_zone.SetActiveBetter(false);
			m_weapon.m_weapon_zone.SetActiveBetter(false);
			m_equipment.m_equipment_zone.SetActiveBetter(false);
			m_drag_icon.SetActiveBetter(false);
			ClearTarget();
		}

		private void RefreshPage()
		{
			DragBagItem = null;
			_startDragTime = 0f;
			m_quick_use_mask.SetActiveBetter(false);
			m_bag_zone.SetActiveBetter(false);
			m_weapon.m_weapon_zone.SetActiveBetter(false);
			m_equipment.m_equipment_zone.SetActiveBetter(false);
			m_drag_icon.SetActiveBetter(false);
			m_equipment.SetData(null);
		}

		public void OnClickBackDesc(GameObject og)
		{
			m_desc.gameObject.SetActiveBetter(false);
			m_equip_desc.gameObject.SetActiveBetter(false);
			m_weapon.gameObject.SetActiveBetter(false);
			m_equipment.gameObject.SetActiveBetter(true);
		}

		public void OnClickSell(GameObject og)
		{
		}

		public void OnClickDiscard(GameObject og)
		{
			Singleton<BagMgr>.Ins.DiscardItem(_currentInstanceId);
			RefreshPage();
		}

		public void OnClickDestroy(GameObject go)
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_currentInstanceId);
			if (allItemByInstanceId != null)
			{
				MessageBoxPanel.Show(Utils.GetString(380, ItemCfg.Get(allItemByInstanceId.itemId).name), _003COnClickDestroy_003Em__1);
			}
		}

		public void OnClickSplit(GameObject og)
		{
			ViewMgr.Ins.ShowView<BagSplitPanel>(_currentInstanceId, false);
		}

		public void OnClickCombination(GameObject og)
		{
		}

		public void OnClickToShortBar(GameObject og)
		{
			DragBagItem = Singleton<BagMgr>.Ins.GetBagItemByInstanceId(_currentInstanceId);
			if (!Singleton<BagMgr>.Ins.AutoQuickUse(DragBagItem))
			{
				AlertBox.Show(130);
			}
		}

		public void OnClickOutShortBar(GameObject og)
		{
			Singleton<BagMgr>.Ins.SetQuickUseItem(_currentInstanceId, -1);
		}

		public void OnClickUse(GameObject og)
		{
			Singleton<BagMgr>.Ins.OnClickUseItem(_currentInstanceId);
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_currentInstanceId);
			if (allItemByInstanceId != null && allItemByInstanceId.number <= 1)
			{
				OnClickBackDesc(null);
			}
		}

		public void OnClickEquip(GameObject og)
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_currentInstanceId);
			ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
			if (BagMgr.IsEquipType(itemCfg.type))
			{
				Singleton<BagMgr>.Ins.PutEquip(_currentInstanceId);
			}
			else if (BagMgr.IsSkinType(itemCfg.type))
			{
				Singleton<BagMgr>.Ins.PutSkin(_currentInstanceId);
			}
		}

		public void OnClickToHand(GameObject og)
		{
			if (Singleton<BagMgr>.Ins.BagContainsInstanceId(_currentInstanceId))
			{
				for (int i = 0; i < m_shortcut_bar.Length; i++)
				{
					if (!QuickUseItems.ContainsKey(i))
					{
						Singleton<BagMgr>.Ins.SetQuickUseItem(_currentInstanceId, i);
						Singleton<BagMgr>.Ins.PutToHand(_currentInstanceId);
						return;
					}
				}
				AlertBox.Show(130);
			}
			else
			{
				Singleton<BagMgr>.Ins.PutToHand(_currentInstanceId);
			}
		}

		public void OnClickOutHand(GameObject og)
		{
			Singleton<BagMgr>.Ins.PutToHand(-1);
		}

		public void OnUnloadBullet(GameObject og)
		{
			Singleton<BagMgr>.Ins.RemoveGunBullet(BagGWeapon.GunBagItem);
			m_weapon.gameObject.SetActiveBetter(true);
			m_desc.gameObject.SetActiveBetter(false);
			m_equipment.gameObject.SetActiveBetter(false);
		}

		public void OnClickRemoveEquip(GameObject og)
		{
			RemoveSkinOrEquip(_currentDescBagItem);
		}

		public void RemoveSkinOrEquip(BagItem bagItem)
		{
			if (m_equipment.CurrentEquipType == BagGEquipment.EquipType.Equip)
			{
				if (Singleton<BagMgr>.Ins.IsHasCapacity(bagItem.itemId))
				{
					Singleton<BagMgr>.Ins.RemoveEquip(bagItem.instanceId);
				}
				else
				{
					AlertBox.Show(26);
				}
			}
			else if (m_equipment.CurrentEquipType == BagGEquipment.EquipType.Skin)
			{
				if (Singleton<BagMgr>.Ins.IsHasCapacity(bagItem.itemId))
				{
					Singleton<BagMgr>.Ins.RemoveSkin(bagItem.instanceId);
				}
				else
				{
					AlertBox.Show(26);
				}
			}
		}

		public void OnClickRemoveGunPart(GameObject og)
		{
			Singleton<BagMgr>.Ins.UnLoadPart(_currentDescBagItem.instanceId);
		}

		public void SetTarget(BagItem bagItem, int from)
		{
			ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
			switch (from)
			{
			case 1:
			{
				AddTargetTransForm(m_zuo.transform, m_zuo.transform.parent);
				if (itemCfg.toQuickUse)
				{
					AddTargetTransForm(m_shortcut_barObj.transform, m_shortcut_barObj.transform.parent);
				}
				if (Singleton<BagMgr>.Ins.PartTypes.Contains(itemCfg.type) && Singleton<BagMgr>.Ins.IsGunContainPart(_currentInstanceId, bagItem.itemId))
				{
					AddTargetTransForm(m_weapon.m_gun_partandbullet.transform, m_weapon.m_gun_partandbullet.transform.parent);
				}
				if (itemCfg.type == 24 && Singleton<BagMgr>.Ins.IsGunContainBullet(_currentInstanceId, bagItem.itemId))
				{
					AddTargetTransForm(m_weapon.m_gun_partandbullet.transform, m_weapon.m_gun_partandbullet.transform.parent);
				}
				int equipIndex = Singleton<BagMgr>.Ins.GetEquipIndex(itemCfg.id);
				if (equipIndex > -1 && m_equipment.m_equipments.Length > equipIndex)
				{
					AddTargetTransForm(m_equipment.m_equipments[equipIndex].transform, m_equipment.m_equipments[equipIndex].transform.parent);
				}
				int skinIndex = Singleton<BagMgr>.Ins.GetSkinIndex(itemCfg.id);
				if (skinIndex > -1 && m_equipment.m_equipments_r.Length > skinIndex)
				{
					AddTargetTransForm(m_equipment.m_equipments_r[skinIndex].transform, m_equipment.m_equipments_r[skinIndex].transform.parent);
				}
				break;
			}
			case 2:
				AddTargetTransForm(m_shortcut_barObj.transform, m_shortcut_barObj.transform.parent);
				AddTargetTransForm(m_zuo.transform, m_zuo.transform.parent);
				break;
			case 3:
			case 4:
				AddTargetTransForm(m_zuo.transform, m_zuo.transform.parent);
				break;
			}
			m_quick_use_mask.SetActiveBetter(true);
		}

		private void AddTargetTransForm(Transform target, Transform targetParene)
		{
			TargetTransforms.Add(target);
			TargetParentTransforms.Add(targetParene);
			target.SetParent(m_quick_use_mask.transform);
		}

		public void ClearTarget()
		{
			for (int i = 0; i < TargetTransforms.Count; i++)
			{
				TargetTransforms[i].SetParent(TargetParentTransforms[i]);
			}
			m_quick_use_mask.SetActiveBetter(false);
			m_quick_use_mask.transform.SetAsLastSibling();
			m_drag_Canvas.transform.SetAsLastSibling();
			TargetTransforms.Clear();
			TargetParentTransforms.Clear();
			m_zuo.transform.SetParent(base.transform);
		}

		public void OnBeginDrag(BagItem bagItem, int from)
		{
			ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
			if (itemCfg.type == 119)
			{
				m_equipment.SetData(BagGEquipment.EquipType.Equip);
			}
			else if (itemCfg.type == 9)
			{
				m_equipment.SetData(BagGEquipment.EquipType.Skin);
			}
			SetTarget(bagItem, from);
			m_drag_icon.SetActiveBetter(true);
			SetIcon(m_drag_icon, bagItem.itemId);
			OnDragMask(null);
			m_equipment.m_equipment_zone.SetActiveBetter(true);
			m_weapon.m_weapon_zone.SetActiveBetter(true);
		}

		public void OnDragMask(GameObject go)
		{
			Vector2 localPoint;
			if (m_drag_icon.activeSelf && !(_canvas == null) && RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, DragListener.pointEventData.position, _canvas.worldCamera, out localPoint))
			{
				m_drag_icon.GetComponent<RectTransform>().anchoredPosition = localPoint;
			}
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

		protected void Update()
		{
			if (_startDragTime > 0f)
			{
				_startDragTime -= Time.deltaTime;
				if (_startDragTime <= 0f && Mathf.Abs(_onDownX - Input.mousePosition.x) < 10f && Mathf.Abs(_onDownY - Input.mousePosition.y) < 10f)
				{
					_isDrag = true;
					OnBeginDrag(DragBagItem, 1);
					_bagScrollRect.enabled = false;
				}
			}
		}

		public static string GetNameForBinding(string normalName, bool isBinding)
		{
			return (!isBinding) ? normalName : Utils.GetString(381, normalName);
		}

		public void ChangePage(BagPageType bagPageType)
		{
		}

		private void Awake()
		{
			m_bag_zone = base.transform.Find("m_zuo/m_bag_zone").gameObject;
			m_cell = View.AddComponentIfNotExist<GBagItem>(base.transform.Find("m_zuo/scp_bag/content/m_cell").gameObject);
			m_desc = View.AddComponentIfNotExist<BagGDesc>(base.transform.Find("m_desc").gameObject);
			m_drag_Canvas = base.transform.Find("m_drag_Canvas").gameObject;
			m_drag_icon = base.transform.Find("m_drag_Canvas/m_drag_icon").gameObject;
			m_equip_desc = View.AddComponentIfNotExist<BagGEquipDesc>(base.transform.Find("m_equip_desc").gameObject);
			m_equipment = View.AddComponentIfNotExist<BagGEquipment>(base.transform.Find("m_equipment").gameObject);
			m_quick_use_mask = base.transform.Find("m_quick_use_mask").gameObject;
			m_shortcut_bar = base.transform.Find("m_shortcut_bar").gameObject.GetComponent<UIGameObjectList>().objects;
			m_shortcut_barObj = base.transform.Find("m_shortcut_bar").gameObject;
			if (m_shortcut_barlist.Count <= 0)
			{
				for (int i = 0; i < m_shortcut_bar.Length; i++)
				{
					m_shortcut_barlist.Add(View.AddComponentIfNotExist<GShortcutBarItem>(m_shortcut_bar[i].gameObject));
				}
			}
			m_weapon = View.AddComponentIfNotExist<BagGWeapon>(base.transform.Find("m_weapon").gameObject);
			m_zuo = base.transform.Find("m_zuo").gameObject;
			scp_bag = base.transform.Find("m_zuo/scp_bag").gameObject;
			txt_bag_num = base.transform.Find("m_zuo/Image/txt_bag_num").gameObject;
			txt_bag_numText = txt_bag_num.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			ClearTarget();
		}

		[CompilerGenerated]
		private void _003COnClickDestroy_003Em__1()
		{
			Singleton<BagMgr>.Ins.DiscardItem(_currentInstanceId);
			RefreshPage();
		}
	}
}
