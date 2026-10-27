using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.cook.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class FirePanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillBagItemCell_003Ec__AnonStorey1
		{
			internal int itemId;

			internal ItemCfg itemInfo;

			internal BagItem bagItemInfo;

			internal FirePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this.IsCook || _0024this._curKitchenInfo.finisheds.num > 0)
				{
					return;
				}
				if (SmelterFuelCfg.Get(itemId) != null)
				{
					if (_0024this._curKitchenInfo.fuels.num > 0 && _0024this._curKitchenInfo.fuels.id != itemId)
					{
						AlertBox.Show(146);
						return;
					}
					int num = itemInfo.maxPileNum - _0024this._curKitchenInfo.fuels.num - ((_0024this._curKitchenInfo.fuels.fuelTime > 0) ? 1 : 0);
					if (num <= 0)
					{
						AlertBox.Show(147);
						return;
					}
					Singleton<CookMgr>.Ins.CanAddFuelNum = num;
					ViewMgr.Ins.ShowView<BagSplitPanel>(bagItemInfo.instanceId, false);
				}
				else if (!Singleton<CookMgr>.Ins.IsInUse(itemId))
				{
					Singleton<CookMgr>.Ins.AddMaterial(bagItemInfo, _0024this.m_materials.Length);
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillMaterialCellNotCook_003Ec__AnonStorey2
		{
			internal ItemCfg itemInfo;

			internal FirePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (!_0024this.IsCook)
				{
					Singleton<CookMgr>.Ins.RemoveMaterial(itemInfo.id);
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		private int _maxCanCook;

		private int _cookbookId;

		private bool _isNotDarkDishes;

		private float _totalFinishTime;

		private float _totalFuelTime;

		private float _nextTimeCutMaterial;

		private float _nextTimeCutFuel;

		private InputField _inpNum;

		private UIScrollPanel _bagScrollPanel;

		private SCampFireOrFirePlaceInfo _curKitchenInfo;

		private List<BagItem> _bagItems = new List<BagItem>();

		private int _cookSoundId;

		private int _onlyFireSoundId;

		private const float _delay = 0.9f;

		private bool _isCook;

		private HashSet<int> _cookMaterialItemIds = new HashSet<int>();

		private HashSet<int> _cookFuelItemIds = new HashSet<int>();

		private long _selfRoleId;

		private float _intervalUpdateCountDown = 0.2f;

		private float _addUpTime;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject txt_title;

		private Text txt_titleText;

		private GameObject scp_bag;

		private GCookBagCell m_cell;

		private GameObject m_dishes_question;

		private GameObject m_dishes_icon;

		private GameObject m_fuel_icon;

		private GameObject btn_preview;

		private GameObject inp_num;

		private GameObject btn_reduce;

		private GameObject btn_add;

		private GameObject btn_max;

		private GameObject m_time;

		private GameObject txt_time;

		private Text txt_timeText;

		private GameObject btn_cancel;

		private GameObject btn_create;

		private List<GCookMaterialCell> m_materialslist = new List<GCookMaterialCell>();

		private GameObject[] m_materials;

		private GameObject m_materialsObj;

		private GameObject txt_dishes_num;

		private Text txt_dishes_numText;

		private GameObject txt_fuel_num;

		private Text txt_fuel_numText;

		private GameObject m_hou_jia_gong_neng;

		private GameObject btn_get;

		private GameObject txt_aaaaaaa;

		private Text txt_aaaaaaaText;

		private GameObject m_fuel;

		private GameObject m_fire_effect;

		private GameObject m_middle_line;

		private GameObject txt_nothing_add;

		private Text txt_nothing_addText;

		private GameObject m_right_arrows;

		private GameObject txt_peng_ren_cheng_pin;

		private Text txt_peng_ren_cheng_pinText;

		private GameObject m_time_fuel;

		private GameObject txt_time_fuel;

		private Text txt_time_fuelText;

		private GameObject m_dishes_parent;

		private GameObject btn_desc;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		private bool IsCook
		{
			get
			{
				return _isCook;
			}
			set
			{
				if (value)
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					_totalFuelTime = realtimeSinceStartup + (float)(SmelterFuelCfg.Get(_curKitchenInfo.fuels.id).burnTime * _curKitchenInfo.fuels.num) - (float)_curKitchenInfo.fuels.fuelTime + 0.9f - (float)_curKitchenInfo.fire;
					if (IsOnlyFireCook())
					{
						_totalFinishTime = _totalFuelTime;
						if (_onlyFireSoundId <= 0)
						{
							_onlyFireSoundId = SingletonMono<AudioManager>.Ins.Play2DLoop(470);
						}
						txt_peng_ren_cheng_pin.SetActiveBetter(false);
						SetRedundanceObj(false);
					}
					else
					{
						UpdateAllMaterialCook();
						_inpNum.text = _curKitchenInfo.cookCount.ToString();
						CookbookCfg cookbookCfg = CookbookCfg.Get(_curKitchenInfo.finisheds.id);
						int num = ((cookbookCfg == null) ? cfg.Consts.COOK_DARK_DISHES_TIME : cookbookCfg.time);
						_nextTimeCutMaterial = realtimeSinceStartup + (float)num - (float)(_curKitchenInfo.fire % num);
						_totalFinishTime = realtimeSinceStartup + (float)(_curKitchenInfo.cookCount * num) - (float)_curKitchenInfo.fire + 0.9f;
						if (_cookSoundId <= 0)
						{
							_cookSoundId = SingletonMono<AudioManager>.Ins.Play2DLoop(469);
						}
						txt_peng_ren_cheng_pin.SetActiveBetter(true);
						SetRedundanceObj(true);
					}
					View.SetLabelText(txt_timeText, Utils.GetCountDownTime((int)(_totalFinishTime - realtimeSinceStartup)));
					View.SetLabelText(txt_time_fuelText, Utils.GetCountDownTime((int)(_totalFuelTime - realtimeSinceStartup)));
					_nextTimeCutFuel = realtimeSinceStartup + (float)SmelterFuelCfg.Get(_curKitchenInfo.fuels.id).burnTime - (float)_curKitchenInfo.fuels.fuelTime;
				}
				else
				{
					UpdateAllMaterialNotCook(false);
					if (_onlyFireSoundId > 0)
					{
						SingletonMono<AudioManager>.Ins.StopMusic(_onlyFireSoundId);
						_onlyFireSoundId = 0;
					}
					if (_cookSoundId > 0)
					{
						SingletonMono<AudioManager>.Ins.StopMusic(_cookSoundId);
						_cookSoundId = 0;
					}
					txt_peng_ren_cheng_pin.SetActiveBetter(false);
					SetRedundanceObj(true);
				}
				_inpNum.interactable = !value;
				int dishesItemId;
				SetDishesIcon(IsOnlyFireCook() && value, value, HasFinished(out dishesItemId), dishesItemId);
				View.SetLabelText(txt_dishes_numText, _curKitchenInfo.finisheds.num);
				_isCook = value;
				btn_cancel.SetActiveBetter(_isCook);
				bool flag = _curKitchenInfo.finisheds.num > 0 && !value;
				btn_get.SetActiveBetter(flag);
				m_time.SetActiveBetter(!flag && (value || _curKitchenInfo.fuels.num > 0) && !IsOnlyFireCook());
				m_time_fuel.SetActiveBetter(_curKitchenInfo.fuels.num > 0);
				btn_create.SetActiveBetter(!_isCook && !btn_get.activeSelf);
				m_fire_effect.SetActiveBetter(value);
			}
		}

		private void SetRedundanceObj(bool isShow)
		{
			m_middle_line.SetActiveBetter(isShow);
			m_materialsObj.SetActiveBetter(isShow);
		}

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			m_fire_effect.SetActiveBetter(false);
			txt_nothing_add.SetActiveBetter(false);
			_selfRoleId = Singleton<RoleMgr>.Ins.info.roleId;
			_inpNum = inp_num.GetComponent<InputField>();
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_inpNum.onValueChanged.AddListener(_003ConInit_003Em__0);
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_add, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_reduce, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_max, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_create, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener.Get(btn_get, string.Empty).onClick = (ClickListener.Get(m_dishes_icon, string.Empty).onClick = _003ConInit_003Em__7);
			ClickListener.Get(m_fuel_icon, string.Empty).onClick = _003ConInit_003Em__8;
			ClickListener clickListener = ClickListener.Get(btn_desc, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__9;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			InitCookShowIds();
			m_fuel.SetActiveBetter(false);
			m_materialsObj.SetActiveBetter(false);
			Singleton<ButtonEffectMgr>.Ins.AddGetEffect(btn_get);
		}

		private void InitCookShowIds()
		{
			List<CookbookCfg> allList = CookbookCfg.GetAllList();
			int i = 0;
			for (int count = allList.Count; i < count; i++)
			{
				Dictionary<int, int> material = allList[i].material;
				foreach (int key in material.Keys)
				{
					_cookMaterialItemIds.Add(key);
				}
			}
			List<SmelterFuelCfg> allList2 = SmelterFuelCfg.GetAllList();
			int j = 0;
			for (int count2 = allList2.Count; j < count2; j++)
			{
				_cookFuelItemIds.Add(allList2[j].id);
			}
		}

		private IEnumerator WaitHide()
		{
			yield return new WaitForSeconds(0.1f);
			Hide();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			m_fuel.SetActiveBetter(true);
			m_materialsObj.SetActiveBetter(true);
			if (param == null)
			{
				StartCoroutine(WaitHide());
				return;
			}
			OnMoneyChange();
			_curKitchenInfo = param as SCampFireOrFirePlaceInfo;
			View.SetLabelText(txt_titleText, ItemCfg.Get(_curKitchenInfo.cfgId).name);
			if (_curKitchenInfo.fuels.fuelTime > 0)
			{
				_curKitchenInfo.fuels.num--;
			}
			if (!_curKitchenInfo.isStart)
			{
				UpdateAllMaterialNotCook(false);
				View.SetLabelText(txt_dishes_numText, _curKitchenInfo.finisheds.num);
				SetCookTimeNotCook(0);
				View.SetLabelText(txt_aaaaaaaText, Utils.GetString((_curKitchenInfo.fuels.num <= 0) ? 211 : 212));
			}
			else
			{
				InitData();
			}
			_inpNum.text = _curKitchenInfo.cookCount.ToString();
			IsCook = _curKitchenInfo.isStart;
			SetFuelIconNum();
			FiltrateBagItem();
			UpdateBagItem(false);
			if (!IsCook || IsOnlyFireCook())
			{
				m_hou_jia_gong_neng.SetActiveBetter(false);
			}
			CookEvent.OnAddMaterialAction = (Action<LinkedListNode<CookMgr.CookMaterial>, int>)Delegate.Combine(CookEvent.OnAddMaterialAction, new Action<LinkedListNode<CookMgr.CookMaterial>, int>(OnAddMaterial));
			CookEvent.OnRemoveMaterialAction = (Action)Delegate.Combine(CookEvent.OnRemoveMaterialAction, new Action(OnRemoveMaterial));
			SCampFireOrFirePlaceInfo.handler = (SCampFireOrFirePlaceInfo.Handler)Delegate.Combine(SCampFireOrFirePlaceInfo.handler, new SCampFireOrFirePlaceInfo.Handler(OnSCampFireOrFirePlaceInfo));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
			if (PlayerPrefs.GetInt(FirePopupPanel.HadShowedParamName, 0) == 0)
			{
				ViewMgr.Ins.ShowTopView<FirePopupPanel>();
				PlayerPrefs.SetInt(FirePopupPanel.HadShowedParamName, 1);
			}
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			if (_selfRoleId != msg.operationRoleId && _curKitchenInfo.instanceId == msg.id)
			{
				Singleton<CookMgr>.Ins.SendRequireKitchenMsg();
			}
		}

		private void InitData()
		{
			_curKitchenInfo.finisheds.id = _curKitchenInfo.cookId;
			if (_curKitchenInfo.cookId == cfg.Consts.COOK_DARK_DISHES_ITEMID)
			{
				_curKitchenInfo.finisheds.num = _curKitchenInfo.fire / cfg.Consts.COOK_DARK_DISHES_TIME;
			}
			else
			{
				CookbookCfg cookbookCfg = CookbookCfg.Get(_curKitchenInfo.cookId);
				if (cookbookCfg != null)
				{
					_curKitchenInfo.finisheds.num = _curKitchenInfo.fire / cookbookCfg.time * cookbookCfg.outNum;
				}
			}
			SmelterFuelCfg smelterFuelCfg = SmelterFuelCfg.Get(_curKitchenInfo.fuels.id);
			_curKitchenInfo.fuels.num -= _curKitchenInfo.fire / smelterFuelCfg.burnTime - ((_curKitchenInfo.fuels.fuelTime > 0) ? 1 : 0);
			_curKitchenInfo.fuels.fuelTime = _curKitchenInfo.fire % smelterFuelCfg.burnTime;
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			Singleton<CookMgr>.Ins.ClearAllData();
			if (_onlyFireSoundId > 0)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(_onlyFireSoundId);
				_onlyFireSoundId = 0;
			}
			if (_cookSoundId > 0)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(_cookSoundId);
				_cookSoundId = 0;
			}
			CookEvent.OnAddMaterialAction = (Action<LinkedListNode<CookMgr.CookMaterial>, int>)Delegate.Remove(CookEvent.OnAddMaterialAction, new Action<LinkedListNode<CookMgr.CookMaterial>, int>(OnAddMaterial));
			CookEvent.OnRemoveMaterialAction = (Action)Delegate.Remove(CookEvent.OnRemoveMaterialAction, new Action(OnRemoveMaterial));
			SCampFireOrFirePlaceInfo.handler = (SCampFireOrFirePlaceInfo.Handler)Delegate.Remove(SCampFireOrFirePlaceInfo.handler, new SCampFireOrFirePlaceInfo.Handler(OnSCampFireOrFirePlaceInfo));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnItemChange(int arg1, int arg2)
		{
			FiltrateBagItem();
			UpdateBagItem(true);
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void SetFuelIconNum()
		{
			if (_curKitchenInfo.fuels.num <= 0 && _curKitchenInfo.fuels.fuelTime <= 0)
			{
				m_fuel_icon.SetActiveBetter(false);
				txt_fuel_num.SetActiveBetter(false);
				return;
			}
			m_fuel_icon.SetActiveBetter(true);
			txt_fuel_num.SetActiveBetter(true);
			View.SetItemSprite(m_fuel_icon, ItemCfg.Get(_curKitchenInfo.fuels.id).icon);
			View.SetLabelText(txt_fuel_numText, _curKitchenInfo.fuels.num + ((_curKitchenInfo.fuels.fuelTime > 0) ? 1 : 0));
		}

		protected void Update()
		{
			if (Input.GetKeyUp(KeyCode.A))
			{
				ViewMgr.Ins.ShowTopView<FirePopupPanel>();
			}
			if (!IsCook)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup <= _totalFinishTime)
			{
				if (realtimeSinceStartup >= _nextTimeCutFuel)
				{
					_nextTimeCutFuel = realtimeSinceStartup + (float)SmelterFuelCfg.Get(_curKitchenInfo.fuels.id).burnTime;
					View.SetLabelText(txt_fuel_numText, --_curKitchenInfo.fuels.num);
				}
				_addUpTime += Time.deltaTime;
				if (_addUpTime >= _intervalUpdateCountDown)
				{
					_addUpTime = 0f;
					View.SetLabelText(txt_timeText, Utils.GetCountDownTime((int)(_totalFinishTime - realtimeSinceStartup)));
					View.SetLabelText(txt_time_fuelText, Utils.GetCountDownTime((int)(_totalFuelTime - realtimeSinceStartup)));
				}
				if (!(realtimeSinceStartup >= _nextTimeCutMaterial))
				{
					return;
				}
				CookbookCfg cookbookCfg = CookbookCfg.Get(_curKitchenInfo.finisheds.id);
				Dictionary<int, UseItem> rawMaterials = _curKitchenInfo.rawMaterials;
				int num = 1;
				int num2 = 1;
				if (cookbookCfg == null)
				{
					if (_curKitchenInfo.finisheds.id != cfg.Consts.COOK_DARK_DISHES_ITEMID)
					{
						_nextTimeCutMaterial = float.MaxValue;
						return;
					}
					num = cfg.Consts.COOK_DARK_DISHES_TIME;
					foreach (UseItem value2 in rawMaterials.Values)
					{
						value2.num--;
					}
				}
				else
				{
					num = cookbookCfg.time;
					num2 = cookbookCfg.outNum;
					foreach (UseItem value3 in rawMaterials.Values)
					{
						int value;
						cookbookCfg.material.TryGetValue(value3.id, out value);
						value3.num -= value;
					}
				}
				_nextTimeCutMaterial = realtimeSinceStartup + (float)num;
				UpdateAllMaterialCook();
				_curKitchenInfo.finisheds.num += num2;
				View.SetLabelText(txt_dishes_numText, _curKitchenInfo.finisheds.num);
				if (_curKitchenInfo.finisheds.num == num2)
				{
					m_dishes_icon.SetActiveBetter(true);
					m_dishes_question.SetActiveBetter(false);
				}
			}
			else
			{
				IsCook = false;
				Singleton<CookMgr>.Ins.SendRequireKitchenMsg();
			}
		}

		private void UpdateBagItem(bool isNoPos)
		{
			if (isNoPos)
			{
				_bagScrollPanel.ResetNoPosClear(Singleton<BagMgr>.Ins.BagCapacity, FillBagItemCell);
			}
			else
			{
				_bagScrollPanel.Reset(Singleton<BagMgr>.Ins.BagCapacity, FillBagItemCell);
			}
		}

		private void FiltrateBagItem()
		{
			_bagItems.Clear();
			foreach (int cookMaterialItemId in _cookMaterialItemIds)
			{
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(cookMaterialItemId, false);
				if (itemNum > 0)
				{
					BagItem bagItem = new BagItem();
					bagItem.itemId = cookMaterialItemId;
					bagItem.number = itemNum;
					_bagItems.Add(bagItem);
				}
			}
			foreach (int cookFuelItemId in _cookFuelItemIds)
			{
				_bagItems.AddRange(Singleton<BagMgr>.Ins.GetAllItemByItemId(cookFuelItemId, false));
			}
		}

		private void FillBagItemCell(GameObject go, int index)
		{
			_003CFillBagItemCell_003Ec__AnonStorey1 _003CFillBagItemCell_003Ec__AnonStorey = new _003CFillBagItemCell_003Ec__AnonStorey1();
			_003CFillBagItemCell_003Ec__AnonStorey._0024this = this;
			GCookBagCell component = go.GetComponent<GCookBagCell>();
			if (_bagItems.Count <= index)
			{
				component.m_something.SetActiveBetter(false);
				component.m_nothing.SetActiveBetter(true);
				return;
			}
			component.m_something.SetActiveBetter(true);
			component.m_nothing.SetActiveBetter(false);
			_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo = _bagItems[index];
			_003CFillBagItemCell_003Ec__AnonStorey.itemId = _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId;
			_003CFillBagItemCell_003Ec__AnonStorey.itemInfo = ItemCfg.Get(_003CFillBagItemCell_003Ec__AnonStorey.itemId);
			component.m_mark.SetActiveBetter(!IsCook && Singleton<CookMgr>.Ins.IsInUse(_003CFillBagItemCell_003Ec__AnonStorey.itemId));
			component.m_new.SetActiveBetter(false);
			View.SetItemSprite(component.m_icon, ItemCfg.Get(_003CFillBagItemCell_003Ec__AnonStorey.itemId).icon);
			View.SetLabelText(component.txt_numText, _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.number);
			ClickListener.Get(go, string.Empty).onClick = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__0;
			PressListener.Get(go).onPress = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__1;
		}

		private bool HasFinished(out int dishesItemId)
		{
			dishesItemId = 0;
			if (_curKitchenInfo != null && _curKitchenInfo.finisheds.num > 0)
			{
				dishesItemId = _curKitchenInfo.finisheds.id;
				return true;
			}
			return false;
		}

		private bool IsOnlyFireCook()
		{
			return _curKitchenInfo != null && _curKitchenInfo.finisheds.num <= 0 && _curKitchenInfo.rawMaterials.Count <= 0;
		}

		private bool IsOnlyFireNotCook()
		{
			return Singleton<CookMgr>.Ins.GetFirstMaterial() == null && _curKitchenInfo.finisheds.num <= 0;
		}

		private void OnSCampFireOrFirePlaceInfo(SCampFireOrFirePlaceInfo msg)
		{
			msg.finisheds.id = msg.cookId;
			if (msg.isStart)
			{
				Singleton<CookMgr>.Ins.ClearAllData();
			}
			UpdateBagItem(true);
			_curKitchenInfo = msg;
			if (_curKitchenInfo.fuels.fuelTime > 0)
			{
				_curKitchenInfo.fuels.num--;
			}
			IsCook = msg.isStart;
			SetFuelIconNum();
			SetDishesIcon(IsOnlyFireCook(), msg.isStart, msg.finisheds.num > 0, msg.finisheds.id);
			if (!IsCook)
			{
				Singleton<CookMgr>.Ins.GetCookbookCfgId(out _maxCanCook, out _cookbookId);
				SetCookTimeNotCook(1);
			}
			View.SetLabelText(txt_aaaaaaaText, Utils.GetString((!IsOnlyFireNotCook()) ? 211 : 212));
			bool trueOrFalse = _curKitchenInfo.finisheds.num > 0 && !IsCook;
			btn_get.SetActiveBetter(trueOrFalse);
			LinkedList<CookMgr.CookMaterial> materialNotCook = Singleton<CookMgr>.Ins.GetMaterialNotCook();
			if (Convert.ToInt32(_inpNum.text) == 0 && !IsCook && _curKitchenInfo.finisheds.num == 0 && materialNotCook != null && materialNotCook.Count > 0)
			{
				_inpNum.text = "1";
			}
		}

		private void SetDishesIcon(bool isOnlyFire, bool isCook, bool hasFinish, int dishesItemId)
		{
			m_dishes_icon.SetActiveBetter(hasFinish);
			m_dishes_question.SetActiveBetter(isCook && !hasFinish && !isOnlyFire);
			ItemCfg itemCfg = ItemCfg.Get(dishesItemId);
			if (itemCfg != null)
			{
				View.SetItemSprite(m_dishes_icon, itemCfg.icon);
			}
			if (isCook || !hasFinish || itemCfg == null || dishesItemId == cfg.Consts.COOK_DARK_DISHES_ITEMID)
			{
				return;
			}
			CookbookCfg cookbookCfg = CookbookCfg.Get(dishesItemId);
			if (cookbookCfg == null)
			{
				return;
			}
			Dictionary<int, int> material = cookbookCfg.material;
			int num = 0;
			foreach (KeyValuePair<int, int> item in material)
			{
				GCookMaterialCell gCookMaterialCell = m_materialslist[num];
				if (material.Count > num)
				{
					gCookMaterialCell.m_icon.SetActiveBetter(true);
					gCookMaterialCell.txt_num.SetActiveBetter(true);
					gCookMaterialCell.m_gray.SetActiveBetter(true);
					View.SetItemSprite(gCookMaterialCell.m_icon, ItemCfg.Get(item.Key).icon);
					View.SetLabelText(gCookMaterialCell.txt_numText, 0);
				}
				num++;
			}
		}

		private void SetCookTimeNotCook(int numCook)
		{
			SmelterFuelCfg smelterFuelCfg = SmelterFuelCfg.Get(_curKitchenInfo.fuels.id);
			if (smelterFuelCfg != null)
			{
				m_time_fuel.SetActiveBetter(true);
				View.SetLabelText(txt_time_fuelText, Utils.GetCountDownTime(smelterFuelCfg.burnTime * _curKitchenInfo.fuels.num + smelterFuelCfg.burnTime - _curKitchenInfo.fuels.fuelTime));
				if (IsOnlyFireNotCook())
				{
					m_time.SetActiveBetter(false);
				}
				else if (Singleton<CookMgr>.Ins.GetMaterialNotCook().Count > 0)
				{
					m_time.SetActiveBetter(true);
					View.SetLabelText(txt_timeText, Utils.GetCountDownTime(numCook * ((!_isNotDarkDishes) ? cfg.Consts.COOK_DARK_DISHES_TIME : CookbookCfg.Get(_cookbookId).time)));
				}
				else
				{
					m_time.SetActiveBetter(false);
				}
			}
			else
			{
				m_time.SetActiveBetter(false);
				m_time_fuel.SetActiveBetter(false);
			}
		}

		private void OnAddMaterial(LinkedListNode<CookMgr.CookMaterial> node, int index)
		{
			FillMaterialCellNotCook(m_materialslist[index], node);
			RefreshMaterialAfterChange();
			UpdateBagItem(true);
			m_hou_jia_gong_neng.SetActiveBetter(true);
			View.SetLabelText(txt_aaaaaaaText, Utils.GetString((!IsOnlyFireNotCook()) ? 211 : 212));
			txt_peng_ren_cheng_pin.SetActiveBetter(true);
		}

		private void OnRemoveMaterial()
		{
			RefreshMaterialAfterChange();
			UpdateAllMaterialNotCook(false);
			UpdateBagItem(true);
			if (Singleton<CookMgr>.Ins.GetMaterialNotCook().Count <= 0)
			{
				m_hou_jia_gong_neng.SetActiveBetter(false);
				txt_peng_ren_cheng_pin.SetActiveBetter(false);
			}
			else
			{
				txt_peng_ren_cheng_pin.SetActiveBetter(true);
			}
			View.SetLabelText(txt_aaaaaaaText, Utils.GetString((!IsOnlyFireNotCook()) ? 211 : 212));
		}

		private void RefreshMaterialAfterChange()
		{
			_isNotDarkDishes = Singleton<CookMgr>.Ins.GetCookbookCfgId(out _maxCanCook, out _cookbookId);
			if (_maxCanCook > 0)
			{
				int num = ((_curKitchenInfo.fuels.num > 0) ? (SmelterFuelCfg.Get(_curKitchenInfo.fuels.id).burnTime * _curKitchenInfo.fuels.num - _curKitchenInfo.fuels.fuelTime) : 0);
				int num2 = num / ((!_isNotDarkDishes) ? cfg.Consts.COOK_DARK_DISHES_TIME : CookbookCfg.Get(_cookbookId).time);
				_maxCanCook = ((_maxCanCook >= num2) ? num2 : _maxCanCook);
			}
			if (Convert.ToInt32(_inpNum.text) == 1)
			{
				UpdateAllMaterialNotCook(true);
				SetCookTimeNotCook(1);
			}
			_inpNum.text = "1";
		}

		private void UpdateAllMaterialCook()
		{
			Dictionary<int, UseItem> rawMaterials = _curKitchenInfo.rawMaterials;
			int num = 0;
			foreach (UseItem value in rawMaterials.Values)
			{
				FillMaterialCellCook(m_materialslist[num++], value);
			}
		}

		private void FillMaterialCellCook(GCookMaterialCell cell, UseItem useItemInfo)
		{
			ItemCfg itemCfg = ItemCfg.Get(useItemInfo.id);
			View.SetItemSprite(cell.m_icon, itemCfg.icon);
			View.SetLabelText(cell.txt_numText, useItemInfo.num);
		}

		private void UpdateAllMaterialNotCook(bool isOnlyUpdateUseNum)
		{
			LinkedListNode<CookMgr.CookMaterial> linkedListNode = Singleton<CookMgr>.Ins.GetFirstMaterial();
			Action<GCookMaterialCell, LinkedListNode<CookMgr.CookMaterial>> action = ((!isOnlyUpdateUseNum) ? new Action<GCookMaterialCell, LinkedListNode<CookMgr.CookMaterial>>(FillMaterialCellNotCook) : new Action<GCookMaterialCell, LinkedListNode<CookMgr.CookMaterial>>(RefreshMaterialUseNumNotCook));
			int i = 0;
			for (int num = m_materials.Length; i < num; i++)
			{
				if (linkedListNode != null)
				{
					action(m_materialslist[i], linkedListNode);
					linkedListNode = linkedListNode.Next;
				}
				else
				{
					action(m_materialslist[i], null);
				}
			}
		}

		private void FillMaterialCellNotCook(GCookMaterialCell cell, LinkedListNode<CookMgr.CookMaterial> node)
		{
			_003CFillMaterialCellNotCook_003Ec__AnonStorey2 _003CFillMaterialCellNotCook_003Ec__AnonStorey = new _003CFillMaterialCellNotCook_003Ec__AnonStorey2();
			_003CFillMaterialCellNotCook_003Ec__AnonStorey._0024this = this;
			cell.m_gray.SetActiveBetter(false);
			if (node == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
				ClickListener.Get(cell.gameObject, string.Empty).onClick = null;
				PressListener.Get(cell.gameObject).onPress = null;
				return;
			}
			cell.m_icon.SetActiveBetter(true);
			cell.txt_num.SetActiveBetter(true);
			CookMgr.CookMaterial value = node.Value;
			_003CFillMaterialCellNotCook_003Ec__AnonStorey.itemInfo = ItemCfg.Get(value.bagItemInfo.itemId);
			View.SetItemSprite(cell.m_icon, _003CFillMaterialCellNotCook_003Ec__AnonStorey.itemInfo.icon);
			View.SetLabelText(cell.txt_numText, Utils.GetString(9, value.bagItemInfo.number, value.OnceConsumeNum * Convert.ToInt32(_inpNum.text)));
			ClickListener.Get(cell.gameObject, string.Empty).onClick = _003CFillMaterialCellNotCook_003Ec__AnonStorey._003C_003Em__0;
			PressListener.Get(cell.gameObject).onPress = _003CFillMaterialCellNotCook_003Ec__AnonStorey._003C_003Em__1;
		}

		private void RefreshMaterialUseNumNotCook(GCookMaterialCell cell, LinkedListNode<CookMgr.CookMaterial> node)
		{
			cell.m_gray.SetActiveBetter(false);
			if (node == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
				return;
			}
			cell.m_icon.SetActiveBetter(true);
			cell.txt_num.SetActiveBetter(true);
			CookMgr.CookMaterial value = node.Value;
			View.SetLabelText(cell.txt_numText, Utils.GetString(9, value.bagItemInfo.number, value.OnceConsumeNum * Convert.ToInt32(_inpNum.text)));
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
			txt_title = component.GameObjects[4].gameObject;
			txt_titleText = txt_title.GetComponent<Text>();
			scp_bag = component.GameObjects[5].gameObject;
			m_cell = View.AddComponentIfNotExist<GCookBagCell>(component.GameObjects[6].gameObject);
			m_dishes_question = component.GameObjects[7].gameObject;
			m_dishes_icon = component.GameObjects[8].gameObject;
			m_fuel_icon = component.GameObjects[9].gameObject;
			btn_preview = component.GameObjects[10].gameObject;
			inp_num = component.GameObjects[11].gameObject;
			btn_reduce = component.GameObjects[12].gameObject;
			btn_add = component.GameObjects[13].gameObject;
			btn_max = component.GameObjects[14].gameObject;
			m_time = component.GameObjects[15].gameObject;
			txt_time = component.GameObjects[16].gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
			btn_cancel = component.GameObjects[17].gameObject;
			btn_create = component.GameObjects[18].gameObject;
			m_materials = component.GameObjects[19].gameObject.GetComponent<UIGameObjectList>().objects;
			m_materialsObj = component.GameObjects[19].gameObject;
			if (m_materialslist.Count <= 0)
			{
				for (int i = 0; i < m_materials.Length; i++)
				{
					m_materialslist.Add(View.AddComponentIfNotExist<GCookMaterialCell>(m_materials[i].gameObject));
				}
			}
			txt_dishes_num = component.GameObjects[20].gameObject;
			txt_dishes_numText = txt_dishes_num.GetComponent<Text>();
			txt_fuel_num = component.GameObjects[21].gameObject;
			txt_fuel_numText = txt_fuel_num.GetComponent<Text>();
			m_hou_jia_gong_neng = component.GameObjects[22].gameObject;
			btn_get = component.GameObjects[23].gameObject;
			txt_aaaaaaa = component.GameObjects[24].gameObject;
			txt_aaaaaaaText = txt_aaaaaaa.GetComponent<Text>();
			m_fuel = component.GameObjects[25].gameObject;
			m_fire_effect = component.GameObjects[26].gameObject;
			m_middle_line = component.GameObjects[27].gameObject;
			txt_nothing_add = component.GameObjects[28].gameObject;
			txt_nothing_addText = txt_nothing_add.GetComponent<Text>();
			m_right_arrows = component.GameObjects[29].gameObject;
			txt_peng_ren_cheng_pin = component.GameObjects[30].gameObject;
			txt_peng_ren_cheng_pinText = txt_peng_ren_cheng_pin.GetComponent<Text>();
			m_time_fuel = component.GameObjects[31].gameObject;
			txt_time_fuel = component.GameObjects[32].gameObject;
			txt_time_fuelText = txt_time_fuel.GetComponent<Text>();
			m_dishes_parent = component.GameObjects[33].gameObject;
			btn_desc = component.GameObjects[34].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(string str)
		{
			if (!IsCook)
			{
				int num = Convert.ToInt32(str);
				num = ((num >= _maxCanCook) ? _maxCanCook : num);
				num = ((num > 0) ? num : 0);
				UpdateAllMaterialNotCook(true);
				SetCookTimeNotCook(num);
				_inpNum.text = num.ToString();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (!IsCook)
			{
				int num = Convert.ToInt32(_inpNum.text);
				_inpNum.text = (num + 1).ToString();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (!IsCook)
			{
				int num = Convert.ToInt32(_inpNum.text);
				_inpNum.text = (num - 1).ToString();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			if (!IsCook)
			{
				_inpNum.text = _maxCanCook.ToString();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			if (IsCook)
			{
				Singleton<CookMgr>.Ins.SendDoStopMsg();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			if (_curKitchenInfo.fuels.num <= 0 && _curKitchenInfo.fuels.fuelTime <= 0)
			{
				AlertBox.Show(151);
				return;
			}
			int num = Convert.ToInt32(_inpNum.text);
			if (!IsCook && (num > 0 || IsOnlyFireNotCook()))
			{
				Singleton<CookMgr>.Ins.SendDoStartMsg(num);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			if (!IsCook)
			{
				int num = _curKitchenInfo.finisheds.num;
				int remainCapacity = Singleton<BagMgr>.Ins.GetRemainCapacity(_curKitchenInfo.finisheds.id, num);
				if (remainCapacity < num)
				{
					AlertBox.Show(26);
				}
				else
				{
					Singleton<CookMgr>.Ins.SendGetFinished();
				}
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			if (!IsCook)
			{
				Singleton<CookMgr>.Ins.SendGetFuel();
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__9(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<FirePopupPanel>();
		}
	}
}
