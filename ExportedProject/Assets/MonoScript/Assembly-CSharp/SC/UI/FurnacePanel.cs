using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class FurnacePanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillBagOreCell_003Ec__AnonStorey0
		{
			internal BagItem bagItemInfo;

			internal ItemCfg itemInfo;

			internal FurnacePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this.IsBurning)
				{
					AlertBox.Show(117);
					return;
				}
				Singleton<FurnaceMgr>.Ins.JiaShaA = bagItemInfo;
				Singleton<FurnaceMgr>.Ins.AiJiaShaJiaSha = _0024this.JiaKuangShi;
				ViewMgr.Ins.ShowView<BagSplitPanel>(bagItemInfo.instanceId, false);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBagFuelCell_003Ec__AnonStorey1
		{
			internal BagItem bagItemInfo;

			internal ItemCfg itemInfo;

			internal FurnacePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this.IsBurning)
				{
					AlertBox.Show(117);
					return;
				}
				Singleton<FurnaceMgr>.Ins.JiaShaA = bagItemInfo;
				Singleton<FurnaceMgr>.Ins.AiJiaShaJiaSha = _0024this.JiaRanLiao;
				ViewMgr.Ins.ShowView<BagSplitPanel>(bagItemInfo.instanceId, false);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillFuelCells_003Ec__AnonStorey2
		{
			internal int index;

			internal FurnacePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_0024this.IsBurning)
				{
					AlertBox.Show(117);
				}
				else
				{
					Singleton<FurnaceMgr>.Ins.SendGetFuelMsg(index);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillOreCell_003Ec__AnonStorey3
		{
			internal OreProductionMgr.Ore oreInfo;

			internal int index;

			internal FurnacePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this.IsBurning)
				{
					AlertBox.Show(117);
				}
				else if (Singleton<BagMgr>.Ins.IsHasCapacity(oreInfo.OreItem.id, oreInfo.OreItem.num))
				{
					Singleton<FurnaceMgr>.Ins.SendGetOreMsg(index);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillProductionCells_003Ec__AnonStorey4
		{
			internal int index;

			internal FurnacePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (!_0024this.IsBurning)
				{
					Singleton<FurnaceMgr>.Ins.SendGetProductionMsg(index, false);
				}
				else
				{
					AlertBox.Show(117);
				}
			}
		}

		private UIScrollPanel _bagOrePanel;

		private UIScrollPanel _bagFuelPanel;

		private readonly List<BagItem> _bagOreList = new List<BagItem>();

		private readonly List<BagItem> _bagFuelList = new List<BagItem>();

		private int _nextTimeUpdateFuel;

		private int _burnIndex = -1;

		private int _curOpenSmelterItemId;

		private UIScrollPanel _orePanel;

		private float _alpha = 0.5f;

		private int _burningSoundId;

		private bool _isBurning;

		private List<UseItem> _productionList;

		private OreProductionMgr _oreProductionData;

		private Button _btnGet;

		private GameObject _effectGo;

		private long _selfRoleId;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GFurnaceBagOreCell m_cell;

		private GameObject scp_ore;

		private List<GFurnaceFuelCell> m_fuellist = new List<GFurnaceFuelCell>();

		private GameObject[] m_fuel;

		private GameObject m_fuelObj;

		private GameObject btn_start;

		private GameObject btn_preview;

		private List<GFurnaceProductionCell> m_productionlist = new List<GFurnaceProductionCell>();

		private GameObject[] m_production;

		private GameObject m_productionObj;

		private GameObject btn_get;

		private GameObject m_icon_drag;

		private GameObject btn_cancel;

		private GameObject scp_bag_fuel;

		private GameObject scp_bag_ore;

		private GFurnaceBagFuelCell m_cell_0;

		private GameObject m_fire_effect;

		private GFurnaceOreCell m_cell_1;

		private GameObject btn_desc;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		private bool IsBurning
		{
			get
			{
				return _isBurning;
			}
			set
			{
				_isBurning = value;
				if (value && _burningSoundId <= 0)
				{
					_burningSoundId = SingletonMono<AudioManager>.Ins.Play2DLoop(479);
				}
				else if (!value)
				{
					if (_burningSoundId > 0)
					{
						SingletonMono<AudioManager>.Ins.StopMusic(_burningSoundId);
						_burningSoundId = 0;
					}
					bool flag = Singleton<FurnaceMgr>.Ins.ProductionCanGet();
					_btnGet.interactable = flag;
					if ((bool)_effectGo)
					{
						_effectGo.SetActiveBetter(flag);
					}
				}
			}
		}

		private void BagInit()
		{
			m_icon_drag.SetActive(false);
			_bagOrePanel = scp_bag_ore.GetComponent<UIScrollPanel>();
			_bagFuelPanel = scp_bag_fuel.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_back, string.Empty).onClick = _003CBagInit_003Em__0;
		}

		private void RefreshBagItems()
		{
			_bagOreList.Clear();
			_bagFuelList.Clear();
			List<BagItem> bagItems = Singleton<BagMgr>.Ins.BagItems;
			int i = 0;
			for (int count = bagItems.Count; i < count; i++)
			{
				if (SmelterCfg.Get(bagItems[i].itemId) != null)
				{
					_bagOreList.Add(bagItems[i]);
				}
				if (SmelterFuelCfg.Get(bagItems[i].itemId) != null)
				{
					_bagFuelList.Add(bagItems[i]);
				}
			}
		}

		private void UpdateBagItemScroll(bool isResetNos = true)
		{
			if (isResetNos)
			{
				_bagOrePanel.ResetNoPosClear(_bagOreList.Count, FillBagOreCell);
				_bagFuelPanel.ResetNoPosClear(_bagFuelList.Count, FillBagFuelCell);
			}
			else
			{
				_bagOrePanel.Reset(_bagOreList.Count, FillBagOreCell);
				_bagFuelPanel.Reset(_bagFuelList.Count, FillBagFuelCell);
			}
		}

		private void FillBagOreCell(GameObject go, int index)
		{
			GFurnaceBagOreCell component = go.GetComponent<GFurnaceBagOreCell>();
			if (_bagOreList.Count > index)
			{
				_003CFillBagOreCell_003Ec__AnonStorey0 _003CFillBagOreCell_003Ec__AnonStorey = new _003CFillBagOreCell_003Ec__AnonStorey0();
				_003CFillBagOreCell_003Ec__AnonStorey._0024this = this;
				component.m_nothing.SetActiveBetter(false);
				component.m_something.SetActiveBetter(true);
				component.m_select.SetActiveBetter(false);
				_003CFillBagOreCell_003Ec__AnonStorey.bagItemInfo = _bagOreList[index];
				_003CFillBagOreCell_003Ec__AnonStorey.itemInfo = ItemCfg.Get(_003CFillBagOreCell_003Ec__AnonStorey.bagItemInfo.itemId);
				View.SetItemSprite(component.m_icon, _003CFillBagOreCell_003Ec__AnonStorey.itemInfo.icon);
				View.SetLabelText(component.txt_numText, _003CFillBagOreCell_003Ec__AnonStorey.bagItemInfo.number);
				component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBagOreCell_003Ec__AnonStorey.bagItemInfo.instanceId));
				ClickListener.Get(go, string.Empty).onClick = _003CFillBagOreCell_003Ec__AnonStorey._003C_003Em__0;
				PressListener.Get(go).onPress = _003CFillBagOreCell_003Ec__AnonStorey._003C_003Em__1;
			}
			else
			{
				component.m_nothing.SetActiveBetter(true);
				component.m_something.SetActiveBetter(false);
				ClickListener.Get(go, string.Empty).onClick = null;
				PressListener.Get(go).onPress = null;
			}
		}

		private void JiaKuangShi()
		{
			BagItem jiaShaA = Singleton<FurnaceMgr>.Ins.JiaShaA;
			Dictionary<int, UseItem> dicIndex2UseItemInfo;
			if (jiaShaA != null && _oreProductionData.AddOreByInstanceId(jiaShaA, out dicIndex2UseItemInfo) && dicIndex2UseItemInfo.Count > 0)
			{
				Singleton<FurnaceMgr>.Ins.SendAddOreMsg(dicIndex2UseItemInfo);
			}
		}

		private void FillBagFuelCell(GameObject go, int index)
		{
			GFurnaceBagFuelCell component = go.GetComponent<GFurnaceBagFuelCell>();
			if (_bagFuelList.Count > index)
			{
				_003CFillBagFuelCell_003Ec__AnonStorey1 _003CFillBagFuelCell_003Ec__AnonStorey = new _003CFillBagFuelCell_003Ec__AnonStorey1();
				_003CFillBagFuelCell_003Ec__AnonStorey._0024this = this;
				component.m_nothing.SetActiveBetter(false);
				component.m_something.SetActiveBetter(true);
				component.m_select.SetActiveBetter(false);
				_003CFillBagFuelCell_003Ec__AnonStorey.bagItemInfo = _bagFuelList[index];
				_003CFillBagFuelCell_003Ec__AnonStorey.itemInfo = ItemCfg.Get(_003CFillBagFuelCell_003Ec__AnonStorey.bagItemInfo.itemId);
				View.SetItemSprite(component.m_icon, _003CFillBagFuelCell_003Ec__AnonStorey.itemInfo.icon);
				View.SetLabelText(component.txt_numText, _003CFillBagFuelCell_003Ec__AnonStorey.bagItemInfo.number);
				component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBagFuelCell_003Ec__AnonStorey.bagItemInfo.instanceId));
				ClickListener.Get(go, string.Empty).onClick = _003CFillBagFuelCell_003Ec__AnonStorey._003C_003Em__0;
				PressListener.Get(go).onPress = _003CFillBagFuelCell_003Ec__AnonStorey._003C_003Em__1;
			}
			else
			{
				component.m_nothing.SetActiveBetter(true);
				component.m_something.SetActiveBetter(false);
				ClickListener.Get(go, string.Empty).onClick = null;
				PressListener.Get(go).onPress = null;
			}
		}

		private void JiaRanLiao()
		{
			BagItem jiaShaA = Singleton<FurnaceMgr>.Ins.JiaShaA;
			if (jiaShaA == null)
			{
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(jiaShaA.itemId);
			Dictionary<int, UseItem> dictionary = new Dictionary<int, UseItem>();
			List<UseItem> fuelList = Singleton<FurnaceMgr>.Ins.FuelList;
			int num = jiaShaA.number;
			for (int i = 0; i < fuelList.Count; i++)
			{
				if (fuelList[i] == null)
				{
					dictionary.Add(i, new UseItem
					{
						id = jiaShaA.itemId,
						num = num,
						instanceId = jiaShaA.instanceId
					});
					break;
				}
				if (fuelList[i].id == jiaShaA.itemId)
				{
					int num2 = itemCfg.maxPileNum - fuelList[i].num;
					int num3 = ((num2 >= num) ? num : num2);
					if (num3 > 0)
					{
						dictionary.Add(i, new UseItem
						{
							id = jiaShaA.itemId,
							num = num3,
							instanceId = jiaShaA.instanceId
						});
						num -= num3;
					}
				}
				if (num <= 0)
				{
					break;
				}
			}
			if (dictionary.Count > 0)
			{
				Singleton<FurnaceMgr>.Ins.SendAddFuelMsg(dictionary);
			}
		}

		private void RefreshFuelCapacity()
		{
			int num = (int)ItemCfg.Get(_curOpenSmelterItemId).extras[1];
			for (int i = 0; i < m_fuel.Length; i++)
			{
				m_fuel[i].SetActiveBetter(num > i);
			}
		}

		private void UpdateFuels(int nowTime)
		{
			_burnIndex = -1;
			List<UseItem> fuelList = Singleton<FurnaceMgr>.Ins.FuelList;
			int i = 0;
			for (int count = fuelList.Count; i < count; i++)
			{
				UseItem useItem = fuelList[i];
				if (IsBurning && _burnIndex < 0 && useItem != null && useItem.num >= 0)
				{
					_burnIndex = i;
					if (useItem.fuelTime > 0)
					{
						_nextTimeUpdateFuel = SmelterFuelCfg.Get(fuelList[i].id).burnTime + nowTime - useItem.fuelTime;
						useItem.fuelTime = 0;
					}
					else if (useItem.num > 0)
					{
						useItem.num--;
						_nextTimeUpdateFuel = SmelterFuelCfg.Get(fuelList[i].id).burnTime + nowTime;
					}
					else if (useItem.num == 0)
					{
						fuelList[i] = null;
						useItem = null;
					}
				}
				FillFuelCells(m_fuellist[i], useItem, _burnIndex == i && IsBurning, i);
			}
		}

		private void StopSmeltFuel(int nowTime)
		{
			if (_burnIndex > -1)
			{
				List<UseItem> fuelList = Singleton<FurnaceMgr>.Ins.FuelList;
				UseItem useItem = fuelList[_burnIndex];
				if (useItem != null && useItem.num >= 0)
				{
					useItem.fuelTime = SmelterFuelCfg.Get(useItem.id).burnTime + nowTime - _nextTimeUpdateFuel;
				}
			}
			_nextTimeUpdateFuel = 0;
		}

		private void FillFuelCells(GFurnaceFuelCell cell, UseItem fuelInfo, bool burning, int index)
		{
			_003CFillFuelCells_003Ec__AnonStorey2 _003CFillFuelCells_003Ec__AnonStorey = new _003CFillFuelCells_003Ec__AnonStorey2();
			_003CFillFuelCells_003Ec__AnonStorey.index = index;
			_003CFillFuelCells_003Ec__AnonStorey._0024this = this;
			if (fuelInfo == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.m_burning.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
				return;
			}
			cell.m_icon.SetActiveBetter(true);
			cell.m_burning.SetActiveBetter(burning);
			cell.txt_num.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(fuelInfo.id);
			View.SetItemSprite(cell.m_icon, itemCfg.icon);
			View.SetLabelText(cell.txt_numText, fuelInfo.num);
			ClickListener.Get(cell.gameObject, string.Empty).onClick = _003CFillFuelCells_003Ec__AnonStorey._003C_003Em__0;
		}

		private void OreInit()
		{
			_orePanel = scp_ore.GetComponent<UIScrollPanel>();
		}

		private void UpdateOreScrollPanel(bool isNoPos)
		{
			if (isNoPos)
			{
				_orePanel.ResetNoPosClear((int)ItemCfg.Get(_curOpenSmelterItemId).extras[0], FillOreCell);
			}
			else
			{
				_orePanel.Reset((int)ItemCfg.Get(_curOpenSmelterItemId).extras[0], FillOreCell);
			}
		}

		private void FillOreCell(GameObject go, int index)
		{
			_003CFillOreCell_003Ec__AnonStorey3 _003CFillOreCell_003Ec__AnonStorey = new _003CFillOreCell_003Ec__AnonStorey3();
			_003CFillOreCell_003Ec__AnonStorey.index = index;
			_003CFillOreCell_003Ec__AnonStorey._0024this = this;
			_003CFillOreCell_003Ec__AnonStorey.oreInfo = null;
			if (_oreProductionData.OreList.Count > _003CFillOreCell_003Ec__AnonStorey.index)
			{
				_003CFillOreCell_003Ec__AnonStorey.oreInfo = _oreProductionData.OreList[_003CFillOreCell_003Ec__AnonStorey.index];
			}
			GFurnaceOreCell component = go.GetComponent<GFurnaceOreCell>();
			if (_003CFillOreCell_003Ec__AnonStorey.oreInfo == null || _003CFillOreCell_003Ec__AnonStorey.oreInfo.OreItem == null)
			{
				component.m_something.SetActiveBetter(false);
				component.m_nothing.SetActiveBetter(true);
				ClickListener.Get(go, string.Empty).onClick = null;
				return;
			}
			ItemCfg itemCfg = ItemCfg.Get(_003CFillOreCell_003Ec__AnonStorey.oreInfo.OreItem.id);
			component.m_something.SetActiveBetter(true);
			component.m_nothing.SetActiveBetter(false);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, _003CFillOreCell_003Ec__AnonStorey.oreInfo.OreItem.num);
			component.m_creating.SetActiveBetter(_003CFillOreCell_003Ec__AnonStorey.oreInfo._target != null && IsBurning);
			ClickListener.Get(go, string.Empty).onClick = _003CFillOreCell_003Ec__AnonStorey._003C_003Em__0;
		}

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			m_cell_0.gameObject.SetActiveBetter(false);
			m_cell_1.gameObject.SetActiveBetter(false);
			btn_cancel.SetActiveBetter(false);
			_selfRoleId = Singleton<RoleMgr>.Ins.info.roleId;
			_btnGet = btn_get.GetComponent<Button>();
			BagInit();
			OreInit();
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_get, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_preview, string.Empty).onClick = null;
			ClickListener.Get(btn_start, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener clickListener = ClickListener.Get(btn_desc, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__5;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			Singleton<ButtonEffectMgr>.Ins.AddGetEffect(btn_get, GetEffectGo);
		}

		private bool ProductionCanGet()
		{
			List<OreProductionMgr.Production> productionList = _oreProductionData.ProductionList;
			int i = 0;
			for (int count = productionList.Count; i < count; i++)
			{
				if (productionList[i] != null && productionList[i].ProductItem != null && productionList[i].ProductItem.num > 0)
				{
					return true;
				}
			}
			return false;
		}

		private void GetEffectGo(GameObject effectGo)
		{
			_effectGo = effectGo;
			_effectGo.SetActiveBetter(_btnGet.interactable);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			_oreProductionData = Singleton<FurnaceMgr>.Ins.OreProduct;
			OnMoneyChange();
			IsBurning = _oreProductionData.IsBurning;
			btn_cancel.SetActiveBetter(IsBurning);
			btn_start.SetActiveBetter(!IsBurning);
			RefreshBagItems();
			RefreshAllInfo(false, false);
			SingletonMono<AudioManager>.Ins.Play2D(478);
			FurnaceEvent.ResetOreProductionDataDelegate = (Action)Delegate.Combine(FurnaceEvent.ResetOreProductionDataDelegate, new Action(ResetOreProductionData));
			SSmelterInfo.handler = (SSmelterInfo.Handler)Delegate.Combine(SSmelterInfo.handler, new SSmelterInfo.Handler(OnSSmelterInfo));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
			if (PlayerPrefs.GetInt(FurnacePopupPanel.HadShowedParamName, 0) == 0)
			{
				ViewMgr.Ins.ShowTopView<FurnacePopupPanel>();
				PlayerPrefs.SetInt(FurnacePopupPanel.HadShowedParamName, 1);
			}
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			if (_selfRoleId != msg.operationRoleId && Singleton<FurnaceMgr>.Ins.IsCurOpen(msg.id))
			{
				Singleton<FurnaceMgr>.Ins.SendRequireSmelterInfoMsg();
			}
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			if (_burningSoundId > 0)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(_burningSoundId);
				_burningSoundId = 0;
			}
			FurnaceEvent.ResetOreProductionDataDelegate = (Action)Delegate.Remove(FurnaceEvent.ResetOreProductionDataDelegate, new Action(ResetOreProductionData));
			SSmelterInfo.handler = (SSmelterInfo.Handler)Delegate.Remove(SSmelterInfo.handler, new SSmelterInfo.Handler(OnSSmelterInfo));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void OnItemChange(int arg1, int arg2)
		{
			RefreshBagItems();
			UpdateBagItemScroll(false);
		}

		private void ResetOreProductionData()
		{
			_oreProductionData = Singleton<FurnaceMgr>.Ins.OreProduct;
		}

		protected void Update()
		{
			if (!IsBurning)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			int num = (int)realtimeSinceStartup;
			if (realtimeSinceStartup > (float)_nextTimeUpdateFuel)
			{
				if (realtimeSinceStartup - (float)_nextTimeUpdateFuel > 3f)
				{
					IsBurning = false;
					if (!ViewMgr.Ins.IsShow<MessageBoxPanel>())
					{
						MessageBoxPanel.ShowConfirm(141, Singleton<FurnaceMgr>.Ins.SendRequireSmelterInfoMsg);
					}
					return;
				}
				UpdateFuels(num);
				if (!_oreProductionData.IsCanSmelt(num))
				{
					AutoStopSmelt(num);
				}
			}
			if (!_oreProductionData.Update(realtimeSinceStartup))
			{
				return;
			}
			UpdateOreScrollPanel(false);
			UpdateProductionScroll();
			if (!_oreProductionData.IsCanSmelt(num))
			{
				AutoStopSmelt(num);
			}
			if (!_btnGet.interactable)
			{
				_btnGet.interactable = true;
				if ((bool)_effectGo)
				{
					_effectGo.SetActiveBetter(true);
				}
			}
		}

		private void AutoStopSmelt(int nowTime)
		{
			IsBurning = false;
			btn_cancel.SetActiveBetter(false);
			btn_start.SetActiveBetter(true);
			_oreProductionData.StopSmelt();
			StopSmeltFuel(nowTime);
			RefreshAllInfo(true, true);
		}

		private void OnSSmelterInfo(SSmelterInfo msg)
		{
			_curOpenSmelterItemId = msg.cfgId;
			IsBurning = msg.isStart;
			btn_cancel.SetActiveBetter(IsBurning);
			btn_start.SetActiveBetter(!IsBurning);
			RefreshAllInfo(false, false);
		}

		private void RefreshAllInfo(bool isBagNoPos, bool isOre)
		{
			_curOpenSmelterItemId = Singleton<FurnaceMgr>.Ins.CurOpenSmelterItemId();
			RefreshFuelCapacity();
			RefreshProductionCapacity();
			UpdateBagItemScroll(isBagNoPos);
			UpdateOreScrollPanel(isOre);
			UpdateFuels((int)Time.realtimeSinceStartup);
			UpdateProductionScroll();
		}

		private void RefreshProductionCapacity()
		{
			int num = (int)ItemCfg.Get(_curOpenSmelterItemId).extras[2];
			int i = 0;
			for (int num2 = m_production.Length; i < num2; i++)
			{
				m_production[i].SetActiveBetter(num > i);
			}
		}

		private void UpdateProductionScroll()
		{
			int num = (int)ItemCfg.Get(_curOpenSmelterItemId).extras[2];
			List<OreProductionMgr.Production> productionList = _oreProductionData.ProductionList;
			for (int i = 0; i < num; i++)
			{
				FillProductionCells(m_productionlist[i], (productionList.Count <= i) ? null : productionList[i], i);
			}
		}

		private void FillProductionCells(GFurnaceProductionCell cell, OreProductionMgr.Production productionInfo, int index)
		{
			_003CFillProductionCells_003Ec__AnonStorey4 _003CFillProductionCells_003Ec__AnonStorey = new _003CFillProductionCells_003Ec__AnonStorey4();
			_003CFillProductionCells_003Ec__AnonStorey.index = index;
			_003CFillProductionCells_003Ec__AnonStorey._0024this = this;
			cell.m_select.SetActiveBetter(false);
			if (productionInfo == null || productionInfo.ProductItem == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
				ClickListener.Get(cell.gameObject, string.Empty).onClick = null;
				return;
			}
			cell.m_icon.SetActiveBetter(true);
			cell.txt_num.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(productionInfo.ProductItem.id);
			View.SetItemSprite(cell.m_icon, itemCfg.icon);
			Image component = cell.m_icon.GetComponent<Image>();
			Color color = component.color;
			color.a = ((productionInfo.ProductItem.num != 0) ? 1f : _alpha);
			component.color = color;
			View.SetLabelText(cell.txt_numText, productionInfo.ProductItem.num);
			ClickListener.Get(cell.gameObject, string.Empty).onClick = _003CFillProductionCells_003Ec__AnonStorey._003C_003Em__0;
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
			m_cell = View.AddComponentIfNotExist<GFurnaceBagOreCell>(component.GameObjects[4].gameObject);
			scp_ore = component.GameObjects[5].gameObject;
			m_fuel = component.GameObjects[6].gameObject.GetComponent<UIGameObjectList>().objects;
			m_fuelObj = component.GameObjects[6].gameObject;
			if (m_fuellist.Count <= 0)
			{
				for (int i = 0; i < m_fuel.Length; i++)
				{
					m_fuellist.Add(View.AddComponentIfNotExist<GFurnaceFuelCell>(m_fuel[i].gameObject));
				}
			}
			btn_start = component.GameObjects[7].gameObject;
			btn_preview = component.GameObjects[8].gameObject;
			m_production = component.GameObjects[9].gameObject.GetComponent<UIGameObjectList>().objects;
			m_productionObj = component.GameObjects[9].gameObject;
			if (m_productionlist.Count <= 0)
			{
				for (int j = 0; j < m_production.Length; j++)
				{
					m_productionlist.Add(View.AddComponentIfNotExist<GFurnaceProductionCell>(m_production[j].gameObject));
				}
			}
			btn_get = component.GameObjects[10].gameObject;
			m_icon_drag = component.GameObjects[11].gameObject;
			btn_cancel = component.GameObjects[12].gameObject;
			scp_bag_fuel = component.GameObjects[13].gameObject;
			scp_bag_ore = component.GameObjects[14].gameObject;
			m_cell_0 = View.AddComponentIfNotExist<GFurnaceBagFuelCell>(component.GameObjects[15].gameObject);
			m_fire_effect = component.GameObjects[16].gameObject;
			m_cell_1 = View.AddComponentIfNotExist<GFurnaceOreCell>(component.GameObjects[17].gameObject);
			btn_desc = component.GameObjects[18].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003CBagInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (!Singleton<FurnaceMgr>.Ins.BagCapacityEnough())
			{
				AlertBox.Show(26);
			}
			else if ((!IsBurning && Singleton<FurnaceMgr>.Ins.ProductionCanGet()) || (IsBurning && ProductionCanGet()))
			{
				Singleton<FurnaceMgr>.Ins.SendGetProductionMsg(0, true);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (!IsBurning)
			{
				float fuelTime = Singleton<FurnaceMgr>.Ins.GetFuelTime(Singleton<FurnaceMgr>.Ins.FuelList);
				if (fuelTime <= 0f)
				{
					AlertBox.Show(215);
				}
				else if (_oreProductionData.StartSmelt(Singleton<FurnaceMgr>.Ins.OreList, Singleton<FurnaceMgr>.Ins.ProductionList, fuelTime, false))
				{
					Singleton<FurnaceMgr>.Ins.SendStartSmeltMsg();
					AlertBox.Show(152);
				}
				else
				{
					AlertBox.Show(216);
				}
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			if (IsBurning)
			{
				IsBurning = false;
				Singleton<FurnaceMgr>.Ins.SendCancelSmeltMsg();
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__5(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<FurnacePopupPanel>();
		}
	}
}
