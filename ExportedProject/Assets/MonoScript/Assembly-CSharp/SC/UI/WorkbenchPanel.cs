using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.workbench.scmsg;

namespace SC.UI
{
	public class WorkbenchPanel : View
	{
		public enum WorkbenchPageType
		{
			Bag = 0,
			Study = 1,
			Repair = 2,
			RepairItem = 3,
			RepairMaterial = 4,
			Upgrade = 5,
			None = 6
		}

		[CompilerGenerated]
		private sealed class _003CFillBagItemCell_003Ec__AnonStorey0
		{
			internal ItemCfg itemInfo;

			internal bool isInstanceInUse;

			internal BagItem bagItemInfo;

			internal WorkbenchPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				if (_0024this._curTabType == WorkbenchPageType.Repair)
				{
					IWorkbenchPage value;
					if (!isInstanceInUse && _0024this._dicTab2Page.TryGetValue(_0024this._curTabType, out value))
					{
						value.OnClickBagItem(bagItemInfo, bagItemInfo.number);
					}
				}
				else
				{
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
				}
			}

			internal void _003C_003Em__2(GameObject o)
			{
				_0024this.SetDragTarget(WorkbenchPageType.Bag);
			}

			internal void _003C_003Em__3(GameObject o)
			{
				_0024this.SetDragTarget(WorkbenchPageType.None);
			}

			internal void _003C_003Em__4(GameObject o)
			{
				_0024this._bagScrollRect.OnBeginDrag(UIEventListener.pointEventData);
			}

			internal void _003C_003Em__5(GameObject o)
			{
				_0024this._bagScrollRect.OnEndDrag(UIEventListener.pointEventData);
			}

			internal void _003C_003Em__6(GameObject o)
			{
				if (Time.realtimeSinceStartup < _0024this._startDragIconTime)
				{
					_0024this._isPointerDown = false;
				}
				_0024this._bagScrollRect.OnDrag(UIEventListener.pointEventData);
				_0024this.rectTrans.position = (Vector2)ViewMgr.Ins.UICamera.ScreenToWorldPoint(Input.mousePosition);
			}

			internal void _003C_003Em__7(GameObject o)
			{
				IWorkbenchPage value;
				if (!isInstanceInUse && _0024this.m_icon_drag.activeSelf && _0024this._targetPageType != WorkbenchPageType.None && _0024this._targetPageType != 0 && _0024this._dicTab2Page.TryGetValue(_0024this._curTabType, out value))
				{
					value.OnClickBagItem(bagItemInfo, bagItemInfo.number);
				}
				_0024this.m_icon_drag.SetActiveBetter(false);
				_0024this.ClearDragCache();
				_0024this._isPointerDown = false;
			}

			internal void _003C_003Em__8(GameObject o)
			{
				_0024this.SetDragFrom(bagItemInfo);
				_0024this._isPointerDown = true;
			}
		}

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_repair;

		private GameObject scp_bag;

		private GWorkbenchBagCell m_cell;

		private WorkbenchStudyPage m_stuty;

		private WorkbenchRepairPage m_repair;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_high;

		private GameObject btn_middle;

		private GameObject btn_primary;

		private WorkbenchUpgradePage m_upgrade;

		private GameObject m_icon_drag;

		private GameObject m_append_expression;

		private GameObject m_append_expression_2;

		private GameObject btn_study;

		private GameObject m_study_red;

		private GameObject m_repair_red;

		private GameObject btn_repair_2;

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

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		private GameObject txt_on_3;

		private Text txt_on_3Text;

		private GameObject txt_on_4;

		private Text txt_on_4Text;

		private GameObject txt_off_3;

		private Text txt_off_3Text;

		private GameObject txt_off_4;

		private Text txt_off_4Text;

		public const int WorkbenchHighLevel = 3;

		public const int WorkbenchMiddleLevel = 2;

		public const int WorkbenchPrimaryLevel = 1;

		private Dictionary<WorkbenchPageType, IWorkbenchPage> _dicTab2Page;

		private WorkbenchPageType _curTabType;

		private int _curLevel;

		private List<BagItem> _bagItems;

		private UIScrollPanel _bagItemPanel;

		private ScrollRect _bagScrollRect;

		private RectTransform rectTrans;

		private BagItem _beginDragInfo;

		private WorkbenchPageType _targetPageType;

		private float _startDragIconTime;

		private bool _bPointerDown;

		private Dictionary<int, HashSet<int>> _dicItemId2InUseItemId = new Dictionary<int, HashSet<int>>();

		private Dictionary<WorkbenchPageType, List<int>> _dicType2PileItemIds = new Dictionary<WorkbenchPageType, List<int>>();

		private Dictionary<WorkbenchPageType, List<int>> _dicType2NonPileItemIds = new Dictionary<WorkbenchPageType, List<int>>();

		private long _selfRoleId;

		private float _upgradeEndTime;

		private bool _isPointerDown
		{
			get
			{
				return _bPointerDown;
			}
			set
			{
				_bPointerDown = value;
				if (_bPointerDown)
				{
					_startDragIconTime = Time.realtimeSinceStartup + 0.1f;
				}
				else if (!_bPointerDown && !_bagScrollRect.enabled)
				{
					_bagScrollRect.enabled = true;
					_startDragIconTime = 0f;
				}
			}
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
			btn_repair = component.GameObjects[4].gameObject;
			scp_bag = component.GameObjects[5].gameObject;
			m_cell = View.AddComponentIfNotExist<GWorkbenchBagCell>(component.GameObjects[6].gameObject);
			m_stuty = View.AddComponentIfNotExist<WorkbenchStudyPage>(component.GameObjects[7].gameObject);
			m_repair = View.AddComponentIfNotExist<WorkbenchRepairPage>(component.GameObjects[8].gameObject);
			txt_on = component.GameObjects[9].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[10].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_high = component.GameObjects[11].gameObject;
			btn_middle = component.GameObjects[12].gameObject;
			btn_primary = component.GameObjects[13].gameObject;
			m_upgrade = View.AddComponentIfNotExist<WorkbenchUpgradePage>(component.GameObjects[14].gameObject);
			m_icon_drag = component.GameObjects[15].gameObject;
			m_append_expression = component.GameObjects[16].gameObject;
			m_append_expression_2 = component.GameObjects[17].gameObject;
			btn_study = component.GameObjects[18].gameObject;
			m_study_red = component.GameObjects[19].gameObject;
			m_repair_red = component.GameObjects[20].gameObject;
			btn_repair_2 = component.GameObjects[21].gameObject;
			txt_on_0 = component.GameObjects[22].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[23].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_0 = component.GameObjects[24].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_2 = component.GameObjects[25].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_1 = component.GameObjects[26].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_off_2 = component.GameObjects[27].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			txt_on_3 = component.GameObjects[28].gameObject;
			txt_on_3Text = txt_on_3.GetComponent<Text>();
			txt_on_4 = component.GameObjects[29].gameObject;
			txt_on_4Text = txt_on_4.GetComponent<Text>();
			txt_off_3 = component.GameObjects[30].gameObject;
			txt_off_3Text = txt_off_3.GetComponent<Text>();
			txt_off_4 = component.GameObjects[31].gameObject;
			txt_off_4Text = txt_off_4.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			m_icon_drag.SetActiveBetter(false);
			_selfRoleId = Singleton<RoleMgr>.Ins.info.roleId;
			_dicTab2Page = new Dictionary<WorkbenchPageType, IWorkbenchPage>();
			_dicTab2Page[WorkbenchPageType.Study] = m_stuty;
			_dicTab2Page[WorkbenchPageType.Repair] = m_repair;
			_dicTab2Page[WorkbenchPageType.Upgrade] = m_upgrade;
			m_stuty.gameObject.SetActiveBetter(false);
			m_repair.gameObject.SetActiveBetter(false);
			m_upgrade.gameObject.SetActiveBetter(false);
			foreach (IWorkbenchPage value2 in _dicTab2Page.Values)
			{
				value2.OnInit();
			}
			_bagItems = new List<BagItem>();
			_bagItemPanel = scp_bag.GetComponent<UIScrollPanel>();
			_bagScrollRect = scp_bag.GetComponent<ScrollRect>();
			rectTrans = m_icon_drag.GetComponent<RectTransform>();
			List<WorkbenchRepairCfg> allList = WorkbenchRepairCfg.GetAllList();
			int i = 0;
			for (int count = allList.Count; i < count; i++)
			{
				ItemCfg itemInfo = ItemCfg.Get(allList[i].itemId);
				InitType2IdsDic(WorkbenchPageType.RepairItem, itemInfo);
			}
			List<int> value;
			if (_dicType2PileItemIds.TryGetValue(WorkbenchPageType.RepairItem, out value))
			{
				value.Sort();
			}
			if (_dicType2NonPileItemIds.TryGetValue(WorkbenchPageType.RepairItem, out value))
			{
				value.Sort();
			}
			int j = 0;
			for (int count2 = allList.Count; j < count2; j++)
			{
				List<DrawingNeedMaterial> material = allList[j].material;
				int k = 0;
				for (int count3 = material.Count; k < count3; k++)
				{
					ItemCfg itemInfo2 = ItemCfg.Get(material[k].itemId);
					InitType2IdsDic(WorkbenchPageType.RepairMaterial, itemInfo2);
				}
			}
			if (_dicType2PileItemIds.TryGetValue(WorkbenchPageType.RepairMaterial, out value))
			{
				value.Sort();
			}
			if (_dicType2NonPileItemIds.TryGetValue(WorkbenchPageType.RepairMaterial, out value))
			{
				value.Sort();
			}
			List<WorkbenchDevelopmentCfg> allList2 = WorkbenchDevelopmentCfg.GetAllList();
			int l = 0;
			for (int count4 = allList2.Count; l < count4; l++)
			{
				ItemCfg itemInfo3 = ItemCfg.Get(allList2[l].assistId);
				InitType2IdsDic(WorkbenchPageType.Study, itemInfo3);
				itemInfo3 = ItemCfg.Get(allList2[l].requisiteId);
				InitType2IdsDic(WorkbenchPageType.Study, itemInfo3);
			}
			List<WorkbenchLevelUpCfg> allList3 = WorkbenchLevelUpCfg.GetAllList();
			int m = 0;
			for (int count5 = allList3.Count; m < count5; m++)
			{
				int n = 0;
				for (int count6 = allList3[m].material.Count; n < count6; n++)
				{
					ItemCfg itemInfo4 = ItemCfg.Get(allList3[m].material[n].itemId);
					InitType2IdsDic(WorkbenchPageType.Upgrade, itemInfo4);
				}
			}
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_repair, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_high, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_middle, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_primary, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_study, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_repair, string.Empty).onClick = _003ConInit_003Em__6;
			m_study_red.SetActiveBetter(false);
			m_repair_red.SetActiveBetter(false);
		}

		private void InitType2IdsDic(WorkbenchPageType type, ItemCfg itemInfo)
		{
			List<int> value;
			if (itemInfo.isPileAble)
			{
				if (!_dicType2PileItemIds.TryGetValue(type, out value))
				{
					value = new List<int>();
					_dicType2PileItemIds[type] = value;
				}
			}
			else if (!_dicType2NonPileItemIds.TryGetValue(type, out value))
			{
				value = new List<int>();
				_dicType2NonPileItemIds[type] = value;
			}
			if (!value.Contains(itemInfo.id))
			{
				value.Add(itemInfo.id);
			}
		}

		private void OnClickBtn(int level)
		{
			bool isPrimary2High;
			if (Singleton<WorkbenchPanelMgr>.Ins.IsLevelToUpgrade(level, out isPrimary2High))
			{
				ChangePage(WorkbenchPageType.Upgrade, level);
			}
			else if (isPrimary2High)
			{
				RadioButton.ChooseBtn(btn_middle);
				ChangePage(WorkbenchPageType.Upgrade, 2);
			}
			else
			{
				ChangePage(WorkbenchPageType.Study, level);
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			WorkbenchEvent.SetInUseItemInstanceIdDelegate = (Action<int, int>)Delegate.Combine(WorkbenchEvent.SetInUseItemInstanceIdDelegate, new Action<int, int>(SetInUseItemInstanceId));
			WorkbenchEvent.RemoveInUseItemIdDelegate = (Action<int>)Delegate.Combine(WorkbenchEvent.RemoveInUseItemIdDelegate, new Action<int>(RemoveInUseItemId));
			WorkbenchEvent.RemoveAllInUseItemDelegate = (Action)Delegate.Combine(WorkbenchEvent.RemoveAllInUseItemDelegate, new Action(RemoveAllInUseItem));
			OnMoneyChange();
			ChangePage(WorkbenchPageType.Study, 1);
			btn_high.SetActiveBetter(Singleton<WorkbenchPanelMgr>.Ins.HighBtnShow());
			RadioButton.ChooseBtn(btn_study);
			RadioButton.ChooseBtn(btn_primary);
			SingletonMono<AudioManager>.Ins.Play2D(474);
			WorkbenchEvent.ChangePageDelegate = (Action<WorkbenchPageType, int>)Delegate.Combine(WorkbenchEvent.ChangePageDelegate, new Action<WorkbenchPageType, int>(ChangePage));
			WorkbenchEvent.SetDragFromDelegate = (Action<BagItem>)Delegate.Combine(WorkbenchEvent.SetDragFromDelegate, new Action<BagItem>(SetDragFrom));
			WorkbenchEvent.SetDragTargetDelegate = (Action<WorkbenchPageType>)Delegate.Combine(WorkbenchEvent.SetDragTargetDelegate, new Action<WorkbenchPageType>(SetDragTarget));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			WorkbenchEvent.AutoAddRepairMaterialDelegate = (Action<WorkbenchRepairCfg>)Delegate.Combine(WorkbenchEvent.AutoAddRepairMaterialDelegate, new Action<WorkbenchRepairCfg>(OnAutoAddRepairMaterial));
			SWorkbenchInfo.handler = (SWorkbenchInfo.Handler)Delegate.Combine(SWorkbenchInfo.handler, new SWorkbenchInfo.Handler(OnSWorkbenchInfo));
			SRepair.handler = (SRepair.Handler)Delegate.Combine(SRepair.handler, new SRepair.Handler(OnSRepair));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			if (_selfRoleId != msg.operationRoleId && Singleton<WorkbenchPanelMgr>.Ins.CurOpenWorkbench == msg.id)
			{
				Singleton<WorkbenchPanelMgr>.Ins.SendRequireWorkbenchInfoMsg();
			}
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			WorkbenchEvent.SetInUseItemInstanceIdDelegate = (Action<int, int>)Delegate.Remove(WorkbenchEvent.SetInUseItemInstanceIdDelegate, new Action<int, int>(SetInUseItemInstanceId));
			WorkbenchEvent.RemoveInUseItemIdDelegate = (Action<int>)Delegate.Remove(WorkbenchEvent.RemoveInUseItemIdDelegate, new Action<int>(RemoveInUseItemId));
			WorkbenchEvent.RemoveAllInUseItemDelegate = (Action)Delegate.Remove(WorkbenchEvent.RemoveAllInUseItemDelegate, new Action(RemoveAllInUseItem));
			ChangePage(WorkbenchPageType.None);
			WorkbenchEvent.ChangePageDelegate = (Action<WorkbenchPageType, int>)Delegate.Remove(WorkbenchEvent.ChangePageDelegate, new Action<WorkbenchPageType, int>(ChangePage));
			WorkbenchEvent.SetDragFromDelegate = (Action<BagItem>)Delegate.Remove(WorkbenchEvent.SetDragFromDelegate, new Action<BagItem>(SetDragFrom));
			WorkbenchEvent.SetDragTargetDelegate = (Action<WorkbenchPageType>)Delegate.Remove(WorkbenchEvent.SetDragTargetDelegate, new Action<WorkbenchPageType>(SetDragTarget));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			WorkbenchEvent.AutoAddRepairMaterialDelegate = (Action<WorkbenchRepairCfg>)Delegate.Remove(WorkbenchEvent.AutoAddRepairMaterialDelegate, new Action<WorkbenchRepairCfg>(OnAutoAddRepairMaterial));
			SWorkbenchInfo.handler = (SWorkbenchInfo.Handler)Delegate.Remove(SWorkbenchInfo.handler, new SWorkbenchInfo.Handler(OnSWorkbenchInfo));
			SRepair.handler = (SRepair.Handler)Delegate.Remove(SRepair.handler, new SRepair.Handler(OnSRepair));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnSRepair(SRepair msg)
		{
			FiltrateItem();
			UpdateBagItems(true);
		}

		private void OnSWorkbenchInfo(SWorkbenchInfo msg)
		{
			btn_high.SetActiveBetter(msg.workbenchLevel >= 2);
		}

		private void OnAutoAddRepairMaterial(WorkbenchRepairCfg obj)
		{
			int num = 0;
			List<DrawingNeedMaterial> material = obj.material;
			HashSet<DrawingNeedMaterial> hashSet = new HashSet<DrawingNeedMaterial>();
			int i = 0;
			for (int count = _bagItems.Count; i < count; i++)
			{
				BagItem bagItem = _bagItems[i];
				int j = 0;
				for (int count2 = material.Count; j < count2; j++)
				{
					if (material[j].itemId == bagItem.itemId)
					{
						m_repair.OnClickBagItem(bagItem, bagItem.number);
						num++;
						hashSet.Add(material[j]);
						if (num == count2)
						{
							return;
						}
						break;
					}
				}
			}
			int k = 0;
			for (int count3 = material.Count; k < count3; k++)
			{
				if (!hashSet.Contains(material[k]))
				{
					BagItem bagItem2 = new BagItem();
					bagItem2.itemId = material[k].itemId;
					bagItem2.instanceId = material[k].itemId;
					m_repair.AddRepairMaterial(bagItem2, false);
				}
			}
		}

		private void OnItemChange(int arg1, int arg2)
		{
			FiltrateItem();
			UpdateBagItems(true);
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void ChangePage(WorkbenchPageType tabType, int studyLevel = 0)
		{
			m_append_expression.SetActiveBetter(tabType == WorkbenchPageType.Repair);
			m_append_expression_2.SetActiveBetter(tabType != WorkbenchPageType.Repair);
			if (tabType == _curTabType && studyLevel == _curLevel)
			{
				return;
			}
			_curLevel = studyLevel;
			_dicItemId2InUseItemId.Clear();
			if (tabType == _curTabType && tabType == WorkbenchPageType.Study && WorkbenchEvent.ChangeStudyLevelDelegate != null)
			{
				WorkbenchEvent.ChangeStudyLevelDelegate(studyLevel);
				return;
			}
			IWorkbenchPage value;
			if (_dicTab2Page.TryGetValue(_curTabType, out value))
			{
				value.ThisGo.SetActiveBetter(false);
				value.OnHide();
			}
			_curTabType = tabType;
			if (_dicTab2Page.TryGetValue(tabType, out value))
			{
				value.ThisGo.SetActiveBetter(true);
				value.OnShow(studyLevel);
				FiltrateItem();
				UpdateBagItems(false);
			}
		}

		private void FiltrateItem()
		{
			_bagItems.Clear();
			if (_curTabType != WorkbenchPageType.Repair)
			{
				AddItemToList(_curTabType);
				_bagItems.Sort(SortBagItems);
			}
			else
			{
				AddItemToList(WorkbenchPageType.RepairItem);
				AddItemToList(WorkbenchPageType.RepairMaterial);
			}
		}

		private int SortBagItems(BagItem item1, BagItem item2)
		{
			if (item1.itemId == item2.itemId)
			{
				return -1;
			}
			return item1.itemId - item2.itemId;
		}

		private void AddItemToList(WorkbenchPageType tabType)
		{
			List<int> value;
			if (_dicType2NonPileItemIds.TryGetValue(tabType, out value))
			{
				int i = 0;
				for (int count = value.Count; i < count; i++)
				{
					List<BagItem> allItemByItemId = Singleton<BagMgr>.Ins.GetAllItemByItemId(value[i], _curTabType == WorkbenchPageType.Repair);
					if (_curTabType == WorkbenchPageType.Repair)
					{
						int j = 0;
						for (int num = allItemByItemId.Count; j < num; j++)
						{
							ItemCfg itemCfg = ItemCfg.Get(allItemByItemId[j].itemId);
							if (itemCfg.durability <= 0 || itemCfg.durability == allItemByItemId[j].duration)
							{
								allItemByItemId.RemoveAt(j--);
								num--;
							}
						}
					}
					_bagItems.AddRange(allItemByItemId);
				}
			}
			if (!_dicType2PileItemIds.TryGetValue(tabType, out value))
			{
				return;
			}
			int k = 0;
			for (int count2 = value.Count; k < count2; k++)
			{
				BagItem bagItem = new BagItem();
				bagItem.itemId = value[k];
				bagItem.instanceId = bagItem.itemId;
				bagItem.number = Singleton<BagMgr>.Ins.GetItemNum(bagItem.itemId, _curTabType == WorkbenchPageType.Repair);
				if (bagItem.number > 0)
				{
					_bagItems.Add(bagItem);
				}
			}
		}

		private void SetInUseItemInstanceId(int itemId, int saveItemId)
		{
			HashSet<int> value;
			if (!_dicItemId2InUseItemId.TryGetValue(itemId, out value))
			{
				value = new HashSet<int>();
				_dicItemId2InUseItemId[itemId] = value;
			}
			value.Add(saveItemId);
			UpdateBagItems(true);
		}

		private void RemoveAllInUseItem()
		{
			_dicItemId2InUseItemId.Clear();
			UpdateBagItems(true);
		}

		private void RemoveInUseItemId(int itemId)
		{
			HashSet<int> value;
			if (_dicItemId2InUseItemId.TryGetValue(itemId, out value))
			{
				value.Clear();
				UpdateBagItems(true);
			}
		}

		private void UpdateBagItems(bool isNoPos)
		{
			if (isNoPos)
			{
				_bagItemPanel.ResetNoPosClear(Singleton<BagMgr>.Ins.BagCapacity, FillBagItemCell);
			}
			else
			{
				_bagItemPanel.Reset(Singleton<BagMgr>.Ins.BagCapacity, FillBagItemCell);
			}
		}

		private void FillBagItemCell(GameObject go, int index)
		{
			_003CFillBagItemCell_003Ec__AnonStorey0 _003CFillBagItemCell_003Ec__AnonStorey = new _003CFillBagItemCell_003Ec__AnonStorey0();
			_003CFillBagItemCell_003Ec__AnonStorey._0024this = this;
			GWorkbenchBagCell component = go.GetComponent<GWorkbenchBagCell>();
			if (_bagItems.Count <= index)
			{
				component.m_something.SetActiveBetter(false);
				component.m_nothing.SetActiveBetter(true);
				return;
			}
			component.m_something.SetActiveBetter(true);
			component.m_nothing.SetActiveBetter(false);
			_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo = _bagItems[index];
			_003CFillBagItemCell_003Ec__AnonStorey.itemInfo = ItemCfg.Get(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId);
			View.SetItemSprite(component.m_icon, _003CFillBagItemCell_003Ec__AnonStorey.itemInfo.icon);
			_003CFillBagItemCell_003Ec__AnonStorey.isInstanceInUse = false;
			HashSet<int> value;
			if (_dicItemId2InUseItemId.TryGetValue(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId, out value))
			{
				_003CFillBagItemCell_003Ec__AnonStorey.isInstanceInUse = value.Contains(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.instanceId);
			}
			component.m_select_material.SetActiveBetter(_003CFillBagItemCell_003Ec__AnonStorey.isInstanceInUse);
			component.m_select_pointer.SetActiveBetter(_beginDragInfo != null && _beginDragInfo.instanceId == _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.instanceId);
			component.m_new.SetActiveBetter(false);
			component.m_durability.SetActiveBetter(_003CFillBagItemCell_003Ec__AnonStorey.itemInfo.durability > 0);
			if (component.m_durability.activeSelf)
			{
				Slider component2 = component.m_durability.GetComponent<Slider>();
				component2.maxValue = _003CFillBagItemCell_003Ec__AnonStorey.itemInfo.durability;
				component2.value = _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.duration;
			}
			View.SetLabelText(component.txt_numText, _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.number);
			if (!Singleton<WorkbenchPanelMgr>.Ins.IsOpenDrag)
			{
				PressListener.Get(go).onPress = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__0;
				ClickListener.Get(go, string.Empty).onClick = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__1;
			}
			else if (Singleton<WorkbenchPanelMgr>.Ins.IsOpenDrag)
			{
				UIEventListener uIEventListener = UIEventListener.Get(go, string.Empty);
				uIEventListener.onEnter = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__2;
				uIEventListener.onExit = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__3;
				uIEventListener.onBeginDrag = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__4;
				uIEventListener.onEndDrag = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__5;
				uIEventListener.onDrag = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__6;
				uIEventListener.onUp = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__7;
				uIEventListener.onDown = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__8;
			}
		}

		protected void Update()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (_isPointerDown && realtimeSinceStartup > _startDragIconTime && _bagScrollRect.enabled)
			{
				_bagScrollRect.enabled = false;
				m_icon_drag.SetActiveBetter(true);
				View.SetItemSprite(m_icon_drag, ItemCfg.Get(_beginDragInfo.itemId).icon);
				rectTrans.position = (Vector2)ViewMgr.Ins.UICamera.ScreenToWorldPoint(Input.mousePosition);
			}
		}

		private void SetDragTarget(WorkbenchPageType tabType)
		{
			_targetPageType = tabType;
		}

		private void SetDragFrom(BagItem fromInfo)
		{
			_beginDragInfo = fromInfo;
		}

		private void ClearDragCache()
		{
			_targetPageType = WorkbenchPageType.None;
			_beginDragInfo = null;
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			ChangePage(WorkbenchPageType.Repair);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			OnClickBtn(3);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			OnClickBtn(2);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			OnClickBtn(1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			RadioButton.ChooseBtn(btn_primary);
			OnClickBtn(1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			RadioButton.ChooseBtn(btn_repair_2);
			ChangePage(WorkbenchPageType.Repair);
		}
	}
}
