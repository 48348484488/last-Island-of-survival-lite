using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.facilitybuilding.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class FenjiejiPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillMaterialCell_003Ec__AnonStorey0
		{
			internal UseItem materialDataInfo;

			internal int index;

			internal FenjiejiPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (Singleton<BagMgr>.Ins.IsHasCapacity(materialDataInfo.id, materialDataInfo.num))
				{
					Singleton<FacilityOneToMultiMgr>.Ins.SendGetRawMaterialMsg(_0024this._totalInfo.facilityBuildingId, index);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillProductionCells_003Ec__AnonStorey1
		{
			internal UseItem productionInfo;

			internal int index;

			internal FenjiejiPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (Singleton<BagMgr>.Ins.GetRemainCapacity(productionInfo.id, productionInfo.num) < productionInfo.num)
				{
					AlertBox.Show(26);
				}
				else
				{
					Singleton<FacilityOneToMultiMgr>.Ins.SendGetFinishedMsg(_0024this._totalInfo.facilityBuildingId, index);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBagItemCell_003Ec__AnonStorey2
		{
			internal BagItem bagItemInfo;

			internal ItemCfg itemInfo;

			internal FenjiejiPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA = bagItemInfo;
				Singleton<FacilityOneToMultiMgr>.Ins.AiJiaShaJiaSha = _0024this.JiaYuanLiao;
				ViewMgr.Ins.ShowView<BagSplitPanel>(bagItemInfo.instanceId, false);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		private readonly List<BagItem> _bagItems = new List<BagItem>();

		private UIScrollPanel _bagItemPanel;

		private UIScrollPanel _materialPanel;

		private GNSSCapacityCfg _curCapacityCfgInfo;

		private SFacilityBuildingInfo _totalInfo;

		private readonly Dictionary<int, UseItem> _dicIndex2MaterialToAdd = new Dictionary<int, UseItem>();

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GDisintegrateItemCell m_cell;

		private GameObject btn_start;

		private GameObject btn_cancel;

		private GameObject btn_desc;

		private List<GDisintegrateProductCell> m_productionlist = new List<GDisintegrateProductCell>();

		private GameObject[] m_production;

		private GameObject m_productionObj;

		private GameObject btn_get;

		private GameObject scp_bag_item;

		private GameObject scp_material;

		private GDisintegrateMaterialCell m_cell_0;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			m_cell_0.gameObject.SetActiveBetter(false);
			btn_desc.SetActive(false);
			_bagItemPanel = scp_bag_item.GetComponent<UIScrollPanel>();
			_materialPanel = scp_material.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_start, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_get, string.Empty).onClick = _003ConInit_003Em__3;
		}

		public bool BagCapacityEnough()
		{
			int num = 0;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int i = 0;
			for (int count = _totalInfo.finisheds.Count; i < count; i++)
			{
				UseItem useItem = _totalInfo.finisheds[i];
				if (useItem == null)
				{
					continue;
				}
				int value;
				if (!dictionary.TryGetValue(useItem.id, out value))
				{
					dictionary[useItem.id] = useItem.num;
					continue;
				}
				int maxPileNum = ItemCfg.Get(useItem.id).maxPileNum;
				value += useItem.num;
				if (value >= maxPileNum)
				{
					num++;
					value -= maxPileNum;
				}
				dictionary[useItem.id] = value;
			}
			int num2 = Singleton<BagMgr>.Ins.BagCapacity - Singleton<BagMgr>.Ins.BagItems.Count - num;
			if (num2 < 0)
			{
				return false;
			}
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				if (item.Value > 0 && Singleton<BagMgr>.Ins.GetRemainCapacityForRonglu(item.Key, item.Value) < item.Value)
				{
					num2--;
					if (num2 < 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			OnMoneyChange();
			_totalInfo = param as SFacilityBuildingInfo;
			if (_totalInfo != null)
			{
				_curCapacityCfgInfo = GNSSCapacityCfg.Get(_totalInfo.cfgId);
				RefreshAllData(_totalInfo, true, false);
				SetProductionCellShow();
				btn_start.SetActiveBetter(!_totalInfo.isStart);
				btn_cancel.SetActiveBetter(_totalInfo.isStart);
				RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
				SFacilityBuildingInfo.handler = (SFacilityBuildingInfo.Handler)Delegate.Combine(SFacilityBuildingInfo.handler, new SFacilityBuildingInfo.Handler(OnSFacilityBuildingInfo));
				SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
			}
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SFacilityBuildingInfo.handler = (SFacilityBuildingInfo.Handler)Delegate.Remove(SFacilityBuildingInfo.handler, new SFacilityBuildingInfo.Handler(OnSFacilityBuildingInfo));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			Singleton<FacilityOneToMultiMgr>.Ins.SendFacilityBuildingInfoMsg(_totalInfo.facilityBuildingId);
		}

		private void OnSFacilityBuildingInfo(SFacilityBuildingInfo msg)
		{
			RefreshAllData(msg, true);
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void RefreshAllData(SFacilityBuildingInfo newInfo, bool isReFiltrate, bool isNoPos = true)
		{
			_totalInfo = newInfo;
			btn_start.SetActiveBetter(!_totalInfo.isStart);
			btn_cancel.SetActiveBetter(_totalInfo.isStart);
			UpdateBagItemScroll(isReFiltrate, isNoPos);
			UpdateMaterialScroll(isNoPos);
			UpdateProductionScroll();
		}

		private void UpdateMaterialScroll(bool isNoPos = true)
		{
			if (isNoPos)
			{
				_materialPanel.ResetNoPos(_curCapacityCfgInfo.materialCapacity, FillMaterialCell);
			}
			else
			{
				_materialPanel.Reset(_curCapacityCfgInfo.materialCapacity, FillMaterialCell);
			}
		}

		private void FillMaterialCell(GameObject go, int index)
		{
			_003CFillMaterialCell_003Ec__AnonStorey0 _003CFillMaterialCell_003Ec__AnonStorey = new _003CFillMaterialCell_003Ec__AnonStorey0();
			_003CFillMaterialCell_003Ec__AnonStorey.index = index;
			_003CFillMaterialCell_003Ec__AnonStorey._0024this = this;
			GDisintegrateMaterialCell component = go.GetComponent<GDisintegrateMaterialCell>();
			_totalInfo.rawMaterials.TryGetValue(_003CFillMaterialCell_003Ec__AnonStorey.index, out _003CFillMaterialCell_003Ec__AnonStorey.materialDataInfo);
			if (_003CFillMaterialCell_003Ec__AnonStorey.materialDataInfo == null)
			{
				component.m_something.SetActiveBetter(false);
				component.m_nothing.SetActiveBetter(true);
				ClickListener.Get(go, string.Empty).onClick = null;
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(_003CFillMaterialCell_003Ec__AnonStorey.materialDataInfo.id);
			component.m_something.SetActiveBetter(true);
			component.m_nothing.SetActiveBetter(false);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, _003CFillMaterialCell_003Ec__AnonStorey.materialDataInfo.num);
			component.m_creating.SetActiveBetter((_curCapacityCfgInfo.isVolume || FirstIndex(_003CFillMaterialCell_003Ec__AnonStorey.index, _totalInfo.rawMaterials)) && _totalInfo.isStart);
			ClickListener.Get(go, string.Empty).onClick = _003CFillMaterialCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private bool FirstIndex(int index, Dictionary<int, UseItem> materials)
		{
			int num = int.MaxValue;
			foreach (KeyValuePair<int, UseItem> material in materials)
			{
				if (num > material.Key)
				{
					num = material.Key;
				}
			}
			return num == index;
		}

		private void SetProductionCellShow()
		{
			int i = 0;
			for (int num = m_production.Length; i < num; i++)
			{
				m_production[i].SetActiveBetter(_curCapacityCfgInfo.finishedCapacity > i);
			}
		}

		private void UpdateProductionScroll()
		{
			List<UseItem> finisheds = _totalInfo.finisheds;
			for (int i = 0; i < _curCapacityCfgInfo.finishedCapacity; i++)
			{
				FillProductionCells(m_productionlist[i], (finisheds.Count <= i) ? null : finisheds[i], i);
			}
		}

		private void FillProductionCells(GDisintegrateProductCell cell, UseItem productionInfo, int index)
		{
			_003CFillProductionCells_003Ec__AnonStorey1 _003CFillProductionCells_003Ec__AnonStorey = new _003CFillProductionCells_003Ec__AnonStorey1();
			_003CFillProductionCells_003Ec__AnonStorey.productionInfo = productionInfo;
			_003CFillProductionCells_003Ec__AnonStorey.index = index;
			_003CFillProductionCells_003Ec__AnonStorey._0024this = this;
			cell.m_select.SetActiveBetter(false);
			if (_003CFillProductionCells_003Ec__AnonStorey.productionInfo == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
				ClickListener.Get(cell.gameObject, string.Empty).onClick = null;
				return;
			}
			cell.m_icon.SetActiveBetter(true);
			cell.txt_num.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(_003CFillProductionCells_003Ec__AnonStorey.productionInfo.id);
			View.SetItemSprite(cell.m_icon, itemCfg.icon);
			View.SetLabelText(cell.txt_numText, _003CFillProductionCells_003Ec__AnonStorey.productionInfo.num);
			ClickListener.Get(cell.gameObject, string.Empty).onClick = _003CFillProductionCells_003Ec__AnonStorey._003C_003Em__0;
		}

		private void FiltrateBagItem()
		{
			_bagItems.Clear();
			GNSSMaterialCfg gNSSMaterialCfg = GNSSMaterialCfg.Get(_totalInfo.cfgId);
			Dictionary<int, GNSSMaterialInfo> materialInfos = gNSSMaterialCfg.materialInfos;
			List<BagItem> bagItems = Singleton<BagMgr>.Ins.BagItems;
			int i = 0;
			for (int count = bagItems.Count; i < count; i++)
			{
				BagItem bagItem = bagItems[i];
				if (!bagItem.isBind && materialInfos.ContainsKey(bagItem.itemId))
				{
					_bagItems.Add(bagItem);
				}
			}
			Dictionary<int, BagItem>.ValueCollection values = Singleton<BagMgr>.Ins.QuickUseItems.Values;
			foreach (BagItem item in values)
			{
				if (item != null && !item.isBind && materialInfos.ContainsKey(item.itemId))
				{
					_bagItems.Add(item);
				}
			}
		}

		private void UpdateBagItemScroll(bool isRefreshBagItem = false, bool isResetNos = true)
		{
			if (isRefreshBagItem)
			{
				FiltrateBagItem();
				isResetNos = false;
			}
			if (isResetNos)
			{
				_bagItemPanel.ResetNoPosClear(_bagItems.Count, FillBagItemCell);
			}
			else
			{
				_bagItemPanel.Reset(_bagItems.Count, FillBagItemCell);
			}
		}

		private void FillBagItemCell(GameObject go, int index)
		{
			GDisintegrateItemCell component = go.GetComponent<GDisintegrateItemCell>();
			if (_bagItems.Count > index)
			{
				_003CFillBagItemCell_003Ec__AnonStorey2 _003CFillBagItemCell_003Ec__AnonStorey = new _003CFillBagItemCell_003Ec__AnonStorey2();
				_003CFillBagItemCell_003Ec__AnonStorey._0024this = this;
				component.m_nothing.SetActiveBetter(false);
				component.m_something.SetActiveBetter(true);
				component.m_select.SetActiveBetter(false);
				_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo = _bagItems[index];
				_003CFillBagItemCell_003Ec__AnonStorey.itemInfo = ItemCfg.Get(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId);
				View.SetItemSprite(component.m_icon, _003CFillBagItemCell_003Ec__AnonStorey.itemInfo.icon);
				View.SetLabelText(component.txt_numText, _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.number);
				component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.instanceId));
				ClickListener.Get(go, string.Empty).onClick = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__0;
				PressListener.Get(go).onPress = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__1;
			}
			else
			{
				component.m_nothing.SetActiveBetter(true);
				component.m_something.SetActiveBetter(false);
				ClickListener.Get(go, string.Empty).onClick = null;
				PressListener.Get(go).onPress = null;
			}
		}

		private void JiaYuanLiao()
		{
			BagItem jiaShaA = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA;
			if (jiaShaA == null)
			{
				return;
			}
			_dicIndex2MaterialToAdd.Clear();
			int num = jiaShaA.number;
			if (num <= 0)
			{
				return;
			}
			Dictionary<int, UseItem> rawMaterials = _totalInfo.rawMaterials;
			int i = 0;
			for (int materialCapacity = _curCapacityCfgInfo.materialCapacity; i < materialCapacity; i++)
			{
				UseItem value;
				if (rawMaterials.TryGetValue(i, out value))
				{
					if (jiaShaA.itemId != value.id)
					{
						continue;
					}
					ItemCfg itemCfg = ItemCfg.Get(jiaShaA.itemId);
					if (value.num < itemCfg.maxPileNum)
					{
						UseItem useItem = new UseItem();
						useItem.id = jiaShaA.itemId;
						int num2 = itemCfg.maxPileNum - value.num;
						int num3 = (useItem.num = ((num2 >= num) ? num : num2));
						useItem.instanceId = jiaShaA.instanceId;
						_dicIndex2MaterialToAdd.Add(i, useItem);
						num -= num3;
						if (num <= 0)
						{
							break;
						}
					}
					continue;
				}
				UseItem useItem2 = new UseItem();
				useItem2.id = jiaShaA.itemId;
				useItem2.num = num;
				useItem2.instanceId = jiaShaA.instanceId;
				_dicIndex2MaterialToAdd.Add(i, useItem2);
				break;
			}
			if (_dicIndex2MaterialToAdd.Count > 0)
			{
				Singleton<FacilityOneToMultiMgr>.Ins.SendAddRawMaterialMsg(_totalInfo.facilityBuildingId, _dicIndex2MaterialToAdd);
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
			m_cell = View.AddComponentIfNotExist<GDisintegrateItemCell>(component.GameObjects[4].gameObject);
			btn_start = component.GameObjects[5].gameObject;
			btn_cancel = component.GameObjects[6].gameObject;
			btn_desc = component.GameObjects[7].gameObject;
			m_production = component.GameObjects[8].gameObject.GetComponent<UIGameObjectList>().objects;
			m_productionObj = component.GameObjects[8].gameObject;
			if (m_productionlist.Count <= 0)
			{
				for (int i = 0; i < m_production.Length; i++)
				{
					m_productionlist.Add(View.AddComponentIfNotExist<GDisintegrateProductCell>(m_production[i].gameObject));
				}
			}
			btn_get = component.GameObjects[9].gameObject;
			scp_bag_item = component.GameObjects[10].gameObject;
			scp_material = component.GameObjects[11].gameObject;
			m_cell_0 = View.AddComponentIfNotExist<GDisintegrateMaterialCell>(component.GameObjects[12].gameObject);
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
			Singleton<FacilityOneToMultiMgr>.Ins.SendStartMsg(_totalInfo.facilityBuildingId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Singleton<FacilityOneToMultiMgr>.Ins.SendStopMsg(_totalInfo.facilityBuildingId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (!BagCapacityEnough())
			{
				AlertBox.Show(26);
			}
			else if (_totalInfo.finisheds.Count > 0)
			{
				Singleton<FacilityOneToMultiMgr>.Ins.SendGetFinishedMsg(_totalInfo.facilityBuildingId, 0, true);
			}
		}
	}
}
