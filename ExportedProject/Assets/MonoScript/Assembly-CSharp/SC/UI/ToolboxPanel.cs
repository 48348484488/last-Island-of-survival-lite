using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using Share;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;

namespace SC.UI
{
	public class ToolboxPanel : View
	{
		private class OnlyAddList<TKey, TValue>
		{
			private readonly List<TValue> _values = new List<TValue>();

			private readonly Dictionary<TKey, int> _key2Index = new Dictionary<TKey, int>();

			public int Count
			{
				get
				{
					return _values.Count;
				}
			}

			public TValue this[TKey key]
			{
				get
				{
					int value;
					if (!_key2Index.TryGetValue(key, out value))
					{
						throw new KeyNotFoundException();
					}
					return _values[value];
				}
				set
				{
					int value2;
					if (_key2Index.TryGetValue(key, out value2))
					{
						_values[value2] = value;
						return;
					}
					_values.Add(value);
					_key2Index.Add(key, _values.Count - 1);
				}
			}

			public TValue GetValue(int index)
			{
				return _values[index];
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				int value2;
				if (!_key2Index.TryGetValue(key, out value2))
				{
					value = default(TValue);
					return false;
				}
				value = _values[value2];
				return true;
			}

			public void RemoveKey(TKey key)
			{
				int value;
				if (_key2Index.TryGetValue(key, out value))
				{
					_values.RemoveAt(value);
					_key2Index.Remove(key);
				}
			}

			public void GetSortedList(List<TValue> sortedList, Comparison<TValue> sortFunc)
			{
				sortedList.Clear();
				int i = 0;
				for (int count = _values.Count; i < count; i++)
				{
					sortedList.Add(_values[i]);
				}
				sortedList.Sort(sortFunc);
			}

			public void Clear()
			{
				_values.Clear();
				_key2Index.Clear();
			}
		}

		private class UpgradeInfo
		{
			public PartBehaviour Part;

			public int PartNum;

			public int NeedItemNum;
		}

		private class RepairInfo
		{
			public readonly int ItemId;

			public int ItemNum;

			public RepairInfo(int itemId)
			{
				ItemId = itemId;
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBagItemCell_003Ec__AnonStorey2
		{
			internal BagItem bagItemInfo;

			internal bool isShowNoviceEffect;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowView<BagSplitPanel>(bagItemInfo.instanceId, false);
				if (isShowNoviceEffect && GuideEvent.StepCompleteAction != null)
				{
					GuideEvent.StepCompleteAction();
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillManorCell_003Ec__AnonStorey3
		{
			internal ManorChestItem chestItemInfo;

			internal ToolboxPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				Singleton<ManorChestMgr>.Ins.SendGetManorChestItemMsg(_0024this._curOpenInfo.instanceId, _0024this.GetIndexByManorChestItem(chestItemInfo), chestItemInfo.number, chestItemInfo.itemId);
			}
		}

		private SManorChestInfo _curOpenInfo;

		private readonly List<ManorChestItem> _allItemList = new List<ManorChestItem>();

		private readonly Queue<int> _emptyIndex = new Queue<int>();

		private readonly HashSet<int> _showMaterialItemIds = new HashSet<int>();

		private UIScrollPanel _bagScrollPanel;

		private UIScrollPanel _manorScrollPanel;

		private readonly List<BagItem> _bagItems = new List<BagItem>();

		private Color _numNormalColor;

		private Color _normalColorExpends;

		private UIScrollPanel _repairScrollPanel;

		private UIScrollPanel _upgradeScrollPanel;

		private const float UpdateDataIntervalTime = 120f;

		private float _nextTimeUpdateRepairDataTime;

		private readonly Dictionary<int, float> _nextTimeUpdateUpgradeDataTimes = new Dictionary<int, float>();

		private readonly Dictionary<int, OnlyAddList<int, UpgradeInfo>> _dicLevel2UpgradeList = new Dictionary<int, OnlyAddList<int, UpgradeInfo>>();

		private readonly List<UpgradeInfo> _upgradeItemList = new List<UpgradeInfo>();

		private readonly Dictionary<int, int> _totalUpgradeNum = new Dictionary<int, int>();

		private readonly List<RepairInfo> _repairList = new List<RepairInfo>();

		private int _totalRepairNum;

		private readonly HashSet<int> _repairNotEnoughItem = new HashSet<int>();

		private readonly Dictionary<int, HashSet<int>> _upgradeNotEnoughItem = new Dictionary<int, HashSet<int>>();

		private int _curOpenLevel;

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

		private GManorChestItemCell m_cell;

		private GameObject txt_bag_num;

		private Text txt_bag_numText;

		private GameObject txt_remain_time;

		private Text txt_remain_timeText;

		private GameObject scp_ling_di_gui;

		private List<GManorChestExpendCell> m_expendslist = new List<GManorChestExpendCell>();

		private GameObject[] m_expends;

		private GameObject m_expendsObj;

		private GameObject btn_permit;

		private GameObject btn_repair;

		private GameObject m_repair_red_dot;

		private GameObject btn_upgrade;

		private GameObject m_upgrade_red_dot;

		private GameObject m_upgrade;

		private GameObject scp_upgrade;

		private GameObject txt_upgrade_total_num;

		private Text txt_upgrade_total_numText;

		private GameObject m_to_stone;

		private GameObject btn_up_stone;

		private GameObject m_to_iron;

		private GameObject btn_up_iron;

		private GameObject m_to_steel;

		private GameObject btn_up_steel;

		private GameObject btn_to_stone;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_to_iron;

		private GameObject btn_to_steel;

		private GameObject btn_back_upgrade;

		private GameObject m_repair;

		private GameObject scp_repair;

		private GameObject txt_repair_total_num;

		private Text txt_repair_total_numText;

		private GameObject btn_repair_confirm;

		private GameObject btn_back_repair;

		private GameObject m_red_dot_stone;

		private GameObject m_red_dot_iron;

		private GameObject m_red_dot_steel;

		private GManorChestUpgradeCell m_cell_0;

		private GameObject txt_upgrade_stone_need;

		private Text txt_upgrade_stone_needText;

		private GameObject txt_upgrade_iron_need;

		private Text txt_upgrade_iron_needText;

		private GameObject txt_upgrade_steel_need;

		private Text txt_upgrade_steel_needText;

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

		private GManorChestRepairCell m_cell_1;

		private GManorChestBagCell m_cell_2;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActive(false);
			m_cell_0.gameObject.SetActive(false);
			m_cell_1.gameObject.SetActive(false);
			m_cell_2.gameObject.SetActive(false);
			_numNormalColor = txt_upgrade_stone_needText.color;
			_normalColorExpends = m_expendslist[0].txt_nameText.color;
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_manorScrollPanel = scp_ling_di_gui.GetComponent<UIScrollPanel>();
			_repairScrollPanel = scp_repair.GetComponent<UIScrollPanel>();
			_upgradeScrollPanel = scp_upgrade.GetComponent<UIScrollPanel>();
			List<BuildPart> allList = BuildPart.GetAllList();
			int i = 0;
			for (int count = allList.Count; i < count; i++)
			{
				BuildPart buildPart = allList[i];
				List<FixBuildMaterialInfo> fixMaterialInfos = buildPart.fixMaterialInfos;
				int j = 0;
				for (int count2 = fixMaterialInfos.Count; j < count2; j++)
				{
					FixBuildMaterialInfo fixBuildMaterialInfo = fixMaterialInfos[j];
					_showMaterialItemIds.Add(fixBuildMaterialInfo.itemId);
				}
			}
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_permit, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_repair, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_back_repair, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_upgrade, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_to_stone, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_to_iron, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener.Get(btn_to_steel, string.Empty).onClick = _003ConInit_003Em__7;
			ClickListener.Get(btn_back_upgrade, string.Empty).onClick = _003ConInit_003Em__8;
			ClickListener.Get(btn_repair_confirm, string.Empty).onClick = _003ConInit_003Em__9;
			ClickListener.Get(btn_up_stone, string.Empty).onClick = _003ConInit_003Em__A;
			ClickListener.Get(btn_up_iron, string.Empty).onClick = _003ConInit_003Em__B;
			ClickListener.Get(btn_up_steel, string.Empty).onClick = _003ConInit_003Em__C;
		}

		private void OnClickUpgradeConfirm(int level)
		{
			if (_upgradeItemList.Count <= 0)
			{
				int needItemId;
				switch (level)
				{
				case 1:
					needItemId = cfg.Consts.STONE_ITEM_ID;
					break;
				case 2:
					needItemId = cfg.Consts.IRON_ITEM_ID;
					break;
				case 3:
					needItemId = cfg.Consts.STEEL_ITEM_ID;
					break;
				default:
					needItemId = cfg.Consts.STONE_ITEM_ID;
					break;
				}
				AlertBox.Show(390, GetExpendNameForAlert(needItemId));
			}
			else
			{
				_nextTimeUpdateRepairDataTime = 0f;
				HashSet<int> value;
				if (_upgradeNotEnoughItem.TryGetValue(level, out value) && value.Count > 0)
				{
					ShowNotEnoughItemName(value, true);
					return;
				}
				Singleton<ManorChestMgr>.Ins.SendUpgradeAllBuildingMsg(_curOpenInfo.instanceId, level);
				_nextTimeUpdateUpgradeDataTimes.Remove(level);
				_dicLevel2UpgradeList.Remove(level);
				_upgradeNotEnoughItem.Remove(level);
				_totalUpgradeNum.Remove(level);
				UpdateUpgradePageNotRefreshData(level);
				UpdateAllRedDot();
				SetUpgradeTotalNeedNum(level, _numNormalColor, 0, 0);
				AlertBox.Show(359);
			}
		}

		private void ShowNotEnoughItemName(HashSet<int> itemIds, bool isUpgrade)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int itemId in itemIds)
			{
				ItemCfg itemCfg = ItemCfg.Get(itemId);
				stringBuilder.Append(Utils.GetString(itemCfg.name));
				stringBuilder.Append(',');
			}
			string text = stringBuilder.ToString();
			AlertBox.Show(Utils.GetString((!isUpgrade) ? 388 : 387, text.TrimEnd(',')));
		}

		private void RefreshUpgradeData(int level)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float value;
			if (_nextTimeUpdateUpgradeDataTimes.TryGetValue(level, out value) && realtimeSinceStartup < value)
			{
				return;
			}
			_nextTimeUpdateUpgradeDataTimes[level] = realtimeSinceStartup + 120f;
			_totalUpgradeNum[level] = 0;
			HashSet<PartBehaviour> parts;
			if (!Singleton<ManorChestMgr>.Ins.GetToolBoxAllPart(_curOpenInfo.instanceId, out parts))
			{
				return;
			}
			OnlyAddList<int, UpgradeInfo> value2;
			if (!_dicLevel2UpgradeList.TryGetValue(level, out value2))
			{
				value2 = new OnlyAddList<int, UpgradeInfo>();
				_dicLevel2UpgradeList[level] = value2;
			}
			else
			{
				value2.Clear();
			}
			foreach (PartBehaviour item in parts)
			{
				if (item.MyCfg.canAutoUpgrade && item.MyCfg.canUpgrade && item.MyCfg.lv == level)
				{
					_totalUpgradeNum[level]++;
					UpgradeInfo value3;
					if (!value2.TryGetValue(item.MyCfg.id, out value3))
					{
						UpgradeInfo upgradeInfo = new UpgradeInfo();
						upgradeInfo.Part = item;
						upgradeInfo.NeedItemNum = 0;
						upgradeInfo.PartNum = 0;
						value3 = upgradeInfo;
						value2[item.MyCfg.id] = value3;
					}
					value3.NeedItemNum += item.MyNextLvCfg.materialInfo.needNum;
					value3.PartNum++;
				}
			}
			HashSet<int> value4;
			if (_upgradeNotEnoughItem.TryGetValue(level, out value4))
			{
				value4.Clear();
			}
			int needItemId;
			GetNeedItemIdFromList(value2, out needItemId);
			int num = 0;
			int i = 0;
			for (int count = value2.Count; i < count; i++)
			{
				UpgradeInfo value5 = value2.GetValue(i);
				num += value5.NeedItemNum;
			}
			int num2 = 0;
			if (needItemId > 0)
			{
				num2 = Singleton<BagMgr>.Ins.GetItemNum(needItemId);
				if (num > num2)
				{
					if (value4 == null)
					{
						value4 = new HashSet<int>();
						_upgradeNotEnoughItem[level] = value4;
					}
					value4.Add(needItemId);
				}
			}
			Color co = ((value4 != null && value4.Count != 0) ? Color.red : _numNormalColor);
			SetUpgradeTotalNeedNum(level, co, num, num2);
		}

		private bool GetNeedItemIdFromList(OnlyAddList<int, UpgradeInfo> list, out int needItemId)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				BuildPart myNextLvCfg = list.GetValue(i).Part.MyNextLvCfg;
				if (myNextLvCfg != null)
				{
					needItemId = myNextLvCfg.materialInfo.itemId;
					return true;
				}
			}
			needItemId = -1;
			return false;
		}

		private int SortUpgrade(UpgradeInfo info1, UpgradeInfo info2)
		{
			return info1.Part.MyCfg.id - info2.Part.MyCfg.id;
		}

		private void GetUpgradeData(int level)
		{
			OnlyAddList<int, UpgradeInfo> value;
			if (_dicLevel2UpgradeList.TryGetValue(level, out value))
			{
				value.GetSortedList(_upgradeItemList, SortUpgrade);
			}
			else
			{
				_upgradeItemList.Clear();
			}
		}

		private void UpdateUpgradePage(int level)
		{
			_curOpenLevel = level;
			RefreshUpgradeData(level);
			UpdateUpgradePageNotRefreshData(level);
		}

		private void UpdateUpgradePageNotRefreshData(int level)
		{
			m_to_stone.SetActiveBetter(level == 1);
			m_to_iron.SetActiveBetter(level == 2);
			m_to_steel.SetActiveBetter(level == 3);
			GetUpgradeData(level);
			int value;
			View.SetLabelText(txt_upgrade_total_numText, Utils.GetString(358, _totalUpgradeNum.TryGetValue(level, out value) ? value : 0));
			_upgradeScrollPanel.Reset((_upgradeItemList != null && _upgradeItemList.Count >= 4) ? _upgradeItemList.Count : 4, FillUpgradeCell);
		}

		private void SetUpgradeTotalNeedNum(int level, Color co, int needItemNum, int ownNum)
		{
			switch (level)
			{
			case 1:
				txt_upgrade_stone_needText.color = co;
				View.SetLabelText(txt_upgrade_stone_needText, Utils.GetString(9, needItemNum, ownNum));
				break;
			case 2:
				txt_upgrade_iron_needText.color = co;
				View.SetLabelText(txt_upgrade_iron_needText, Utils.GetString(9, needItemNum, ownNum));
				break;
			case 3:
				txt_upgrade_steel_needText.color = co;
				View.SetLabelText(txt_upgrade_steel_needText, Utils.GetString(9, needItemNum, ownNum));
				break;
			}
		}

		private void FillUpgradeCell(GameObject go, int index)
		{
			GManorChestUpgradeCell component = go.GetComponent<GManorChestUpgradeCell>();
			if (_upgradeItemList != null && _upgradeItemList.Count > index)
			{
				component.txt_expend_name.SetActiveBetter(true);
				component.txt_name.SetActiveBetter(true);
				component.txt_num.SetActiveBetter(true);
				UpgradeInfo upgradeInfo = _upgradeItemList[index];
				int itemId = upgradeInfo.Part.MyNextLvCfg.materialInfo.itemId;
				View.SetLabelText(component.txt_nameText, Utils.GetString(upgradeInfo.Part.MyCfg.name) + Utils.GetString(281, upgradeInfo.PartNum));
				View.SetLabelText(component.txt_expend_nameText, GetExpendName(itemId));
				View.SetLabelText(component.txt_numText, upgradeInfo.NeedItemNum);
			}
			else
			{
				component.txt_expend_name.SetActiveBetter(false);
				component.txt_name.SetActiveBetter(false);
				component.txt_num.SetActiveBetter(false);
			}
		}

		private string GetExpendNameForAlert(int needItemId)
		{
			if (needItemId == cfg.Consts.STONE_ITEM_ID)
			{
				return Utils.GetString(396);
			}
			if (needItemId == cfg.Consts.IRON_ITEM_ID)
			{
				return Utils.GetString(397);
			}
			if (needItemId == cfg.Consts.STEEL_ITEM_ID)
			{
				return Utils.GetString(398);
			}
			return string.Empty;
		}

		private string GetExpendName(int needItemId)
		{
			if (needItemId == cfg.Consts.STONE_ITEM_ID)
			{
				return Utils.GetString(355);
			}
			if (needItemId == cfg.Consts.IRON_ITEM_ID)
			{
				return Utils.GetString(356);
			}
			if (needItemId == cfg.Consts.STEEL_ITEM_ID)
			{
				return Utils.GetString(357);
			}
			return string.Empty;
		}

		private void RefreshRepairData()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup < _nextTimeUpdateRepairDataTime)
			{
				return;
			}
			_nextTimeUpdateRepairDataTime = realtimeSinceStartup + 120f;
			_totalRepairNum = 0;
			HashSet<PartBehaviour> parts;
			if (Singleton<ManorChestMgr>.Ins.GetToolBoxAllPart(_curOpenInfo.instanceId, out parts))
			{
				OnlyAddList<int, RepairInfo> onlyAddList = new OnlyAddList<int, RepairInfo>();
				foreach (PartBehaviour item in parts)
				{
					if (!item.MyCfg.canFix)
					{
						continue;
					}
					float num = ((float)item.MaxHp - item.Hp) / (float)item.MaxHp;
					if (num <= 0f)
					{
						continue;
					}
					_totalRepairNum++;
					List<FixBuildMaterialInfo> fixMaterialInfos = item.MyCfg.fixMaterialInfos;
					int i = 0;
					for (int count = fixMaterialInfos.Count; i < count; i++)
					{
						FixBuildMaterialInfo fixBuildMaterialInfo = fixMaterialInfos[i];
						RepairInfo value;
						if (!onlyAddList.TryGetValue(fixBuildMaterialInfo.itemId, out value))
						{
							value = new RepairInfo(fixBuildMaterialInfo.itemId);
							onlyAddList[fixBuildMaterialInfo.itemId] = value;
						}
						value.ItemNum += (int)Math.Ceiling(num * (float)fixBuildMaterialInfo.needNum);
					}
				}
				onlyAddList.GetSortedList(_repairList, SortRepair);
			}
			else
			{
				_repairList.Clear();
			}
			_repairNotEnoughItem.Clear();
			int j = 0;
			for (int count2 = _repairList.Count; j < count2; j++)
			{
				RepairInfo repairInfo = _repairList[j];
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(repairInfo.ItemId);
				if (repairInfo.ItemNum > itemNum)
				{
					_repairNotEnoughItem.Add(repairInfo.ItemId);
				}
			}
		}

		private int SortRepair(RepairInfo info1, RepairInfo info2)
		{
			return info1.ItemId - info2.ItemId;
		}

		private void UpdateRepairPage()
		{
			RefreshRepairData();
			UpdateRepairPageNotRefreshData();
		}

		private void UpdateRepairPageNotRefreshData()
		{
			View.SetLabelText(txt_repair_total_numText, Utils.GetString(353, _totalRepairNum));
			_repairScrollPanel.Reset((_repairList.Count >= 4) ? _repairList.Count : 4, FillRepairCell);
		}

		private void FillRepairCell(GameObject go, int index)
		{
			GManorChestRepairCell component = go.GetComponent<GManorChestRepairCell>();
			if (_repairList.Count > index)
			{
				component.txt_num.SetActiveBetter(true);
				component.txt_name.SetActiveBetter(true);
				RepairInfo repairInfo = _repairList[index];
				ItemCfg itemCfg = ItemCfg.Get(repairInfo.ItemId);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(repairInfo.ItemId);
				View.SetLabelText(component.txt_nameText, itemCfg.name);
				View.SetLabelText(component.txt_numText, Utils.GetString(9, repairInfo.ItemNum, itemNum));
				if (repairInfo.ItemNum > itemNum)
				{
					component.txt_numText.color = Color.red;
				}
				else
				{
					component.txt_numText.color = _numNormalColor;
				}
			}
			else
			{
				component.txt_num.SetActiveBetter(false);
				component.txt_name.SetActiveBetter(false);
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			m_repair.SetActiveBetter(false);
			m_upgrade.SetActiveBetter(false);
			if (param == null)
			{
				StartCoroutine(WaitHide());
				btn_permit.SetActiveBetter(false);
				return;
			}
			_curOpenInfo = param as SManorChestInfo;
			btn_permit.SetActiveBetter(Singleton<FriendPermitMgr>.Ins.IsMine(_curOpenInfo.instanceId));
			RefreshItemList();
			FiltrateBagItem();
			UpdateBagItem(false);
			UpdateManorItems(false);
			UpdateRemainTime();
			UpdateExpands();
			OnMoneyChange();
			SetBagCapacity();
			SGetManorChestItem.handler = (SGetManorChestItem.Handler)Delegate.Combine(SGetManorChestItem.handler, new SGetManorChestItem.Handler(OnSGetManorChestItem));
			SPutInManorChestItem.handler = (SPutInManorChestItem.Handler)Delegate.Combine(SPutInManorChestItem.handler, new SPutInManorChestItem.Handler(OnSPutInManorChestItem));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			BagEvent.OnClickLingDiGuiBtn = (Action<BagItem, int>)Delegate.Combine(BagEvent.OnClickLingDiGuiBtn, new Action<BagItem, int>(OnAddItem));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
			ManorChestEvent.OnBuildingToolBoxIdChangeEvent = (Action<long, long, long>)Delegate.Combine(ManorChestEvent.OnBuildingToolBoxIdChangeEvent, new Action<long, long, long>(OnSBuildingToolBoxIdChanged));
			EventHandlers.OnDestroyedPart += OnBuildDestroy;
			SUpgradeAllBuilding.handler = (SUpgradeAllBuilding.Handler)Delegate.Combine(SUpgradeAllBuilding.handler, new SUpgradeAllBuilding.Handler(OnSUpgradeAllBuilding));
			m_red_dot_iron.SetActiveBetter(false);
			m_red_dot_steel.SetActiveBetter(false);
			m_red_dot_stone.SetActiveBetter(false);
			m_repair_red_dot.SetActiveBetter(false);
			m_upgrade_red_dot.SetActiveBetter(false);
			StartCoroutine(RefreshAllData());
		}

		private void OnSUpgradeAllBuilding(SUpgradeAllBuilding msg)
		{
			if (msg.toolBoxId != _curOpenInfo.instanceId)
			{
				return;
			}
			int num = msg.level + 1;
			if (num <= 3)
			{
				_nextTimeUpdateUpgradeDataTimes.Remove(num);
				if (_curOpenLevel == num)
				{
					UpdateUpgradePage(num);
				}
				else
				{
					RefreshUpgradeData(num);
				}
				UpdateAllRedDot();
			}
		}

		private void UpdateAllRedDot()
		{
			m_repair_red_dot.SetActiveBetter(_repairNotEnoughItem.Count <= 0 && _totalRepairNum > 0);
			m_upgrade_red_dot.SetActiveBetter(IsShowUpgradeRedDot());
			m_red_dot_stone.SetActiveBetter(IsShowUpgradeRedDotByLevel(1));
			m_red_dot_iron.SetActiveBetter(IsShowUpgradeRedDotByLevel(2));
			m_red_dot_steel.SetActiveBetter(IsShowUpgradeRedDotByLevel(3));
		}

		private bool IsShowUpgradeRedDot()
		{
			for (int i = 1; i < 4; i++)
			{
				if (IsShowUpgradeRedDotByLevel(i))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsShowUpgradeRedDotByLevel(int level)
		{
			OnlyAddList<int, UpgradeInfo> value;
			HashSet<int> value2;
			return _dicLevel2UpgradeList.TryGetValue(level, out value) && value.Count > 0 && (!_upgradeNotEnoughItem.TryGetValue(level, out value2) || value2.Count <= 0);
		}

		private void OnBuildDestroy(PartBehaviour part)
		{
			if (part.ToolBoxId == _curOpenInfo.instanceId)
			{
				UpdatePageInfo(part, false);
			}
		}

		private void OnSBuildingToolBoxIdChanged(long buildingInsId, long oldToolBoxId, long newToolBoxId)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(buildingInsId);
			if (oldToolBoxId == _curOpenInfo.instanceId)
			{
				UpdatePageInfo(partByInsID, false);
			}
			if (newToolBoxId == _curOpenInfo.instanceId)
			{
				UpdatePageInfo(partByInsID, true);
			}
		}

		private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
			if (partByInsID.ToolBoxId == _curOpenInfo.instanceId)
			{
				UpdatePageInfo(partByInsID, true);
			}
		}

		private void UpdatePageInfo(PartBehaviour part, bool isAdd)
		{
			OnBuildChangeRefreshRepairData(part, isAdd);
			if (m_repair.activeSelf)
			{
				UpdateRepairPageNotRefreshData();
			}
			OnBuildChangeRefreshUpgradeData(part, isAdd);
			if (m_upgrade.activeSelf)
			{
				UpdateUpgradePageNotRefreshData(_curOpenLevel);
			}
			UpdateAllRedDot();
		}

		private void OnBuildChangeRefreshRepairData(PartBehaviour part, bool isAdd)
		{
			List<FixBuildMaterialInfo> fixMaterialInfos = part.MyCfg.fixMaterialInfos;
			float num = ((float)part.MaxHp - part.Hp) / (float)part.MaxHp;
			int i = 0;
			for (int count = fixMaterialInfos.Count; i < count; i++)
			{
				FixBuildMaterialInfo fixBuildMaterialInfo = fixMaterialInfos[i];
				int j = 0;
				for (int count2 = _repairList.Count; j < count2; j++)
				{
					RepairInfo repairInfo = _repairList[j];
					if (repairInfo.ItemId != fixBuildMaterialInfo.itemId)
					{
						continue;
					}
					int num2 = (int)Math.Ceiling(num * (float)fixBuildMaterialInfo.needNum);
					if (isAdd)
					{
						repairInfo.ItemNum += num2;
						break;
					}
					repairInfo.ItemNum -= num2;
					if (repairInfo.ItemNum <= 0)
					{
						_repairList.RemoveAt(j);
					}
					break;
				}
			}
		}

		private void OnBuildChangeRefreshUpgradeData(PartBehaviour part, bool isAdd)
		{
			OnlyAddList<int, UpgradeInfo> value;
			if (part.ToolBoxId != _curOpenInfo.instanceId || !_dicLevel2UpgradeList.TryGetValue(part.MyCfg.lv, out value))
			{
				return;
			}
			UpgradeInfo value2;
			if (!value.TryGetValue(part.MyCfg.id, out value2))
			{
				UpgradeInfo upgradeInfo = new UpgradeInfo();
				upgradeInfo.NeedItemNum = 0;
				upgradeInfo.Part = part;
				upgradeInfo.PartNum = 0;
				value2 = upgradeInfo;
				value[part.MyCfg.id] = value2;
			}
			int needNum = part.MyNextLvCfg.materialInfo.needNum;
			if (isAdd)
			{
				value2.NeedItemNum += needNum;
				return;
			}
			value2.NeedItemNum -= needNum;
			if (value2.NeedItemNum <= 0)
			{
				value.RemoveKey(part.MyCfg.id);
			}
		}

		private IEnumerator RefreshAllData()
		{
			yield return Utils.WaitForSeconds(0.1f);
			RefreshRepairData();
			yield return Utils.WaitForSeconds(0.1f);
			RefreshUpgradeData(1);
			yield return Utils.WaitForSeconds(0.1f);
			RefreshUpgradeData(2);
			yield return Utils.WaitForSeconds(0.1f);
			RefreshUpgradeData(3);
			UpdateAllRedDot();
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void OnAddItem(BagItem bagItem, int addNum)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, ManorChestItem> items = _curOpenInfo.items;
			foreach (KeyValuePair<int, ManorChestItem> item in items)
			{
				if (bagItem.itemId != item.Value.itemId)
				{
					continue;
				}
				int num = ItemCfg.Get(bagItem.itemId).maxPileNum - item.Value.number;
				if (num > 0)
				{
					int num2 = ((num >= addNum) ? addNum : num);
					dictionary.Add(item.Key, num2);
					addNum -= num2;
					if (addNum <= 0)
					{
						break;
					}
				}
			}
			if (addNum > 0)
			{
				if (_emptyIndex.Count <= 0)
				{
					AlertBox.Show(241);
					return;
				}
				dictionary.Add(_emptyIndex.Dequeue(), addNum);
			}
			Singleton<ManorChestMgr>.Ins.SendPutInManorChestItemMsg(bagItem.instanceId, dictionary);
		}

		private void UpdateRemainTime()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int i = 0;
			for (int count = _allItemList.Count; i < count; i++)
			{
				ManorChestItem manorChestItem = _allItemList[i];
				if (!dictionary.ContainsKey(manorChestItem.itemId))
				{
					dictionary[manorChestItem.itemId] = 0;
				}
				dictionary[manorChestItem.itemId] += manorChestItem.number;
			}
			Dictionary<int, int> expend = _curOpenInfo.expend;
			float num = ((expend.Count <= 0) ? 0f : float.MaxValue);
			foreach (KeyValuePair<int, int> item in expend)
			{
				int value;
				if (dictionary.TryGetValue(item.Key, out value))
				{
					float num2 = (float)value / (float)item.Value;
					num = ((!(num < num2)) ? num2 : num);
					continue;
				}
				num = 0f;
				break;
			}
			View.SetLabelText(txt_remain_timeText, GetHourTime(num, 0));
		}

		private void FiltrateBagItem()
		{
			_bagItems.Clear();
			List<BagItem> bagItems = Singleton<BagMgr>.Ins.BagItems;
			int i = 0;
			for (int count = bagItems.Count; i < count; i++)
			{
				BagItem bagItem = bagItems[i];
				if (_showMaterialItemIds.Contains(bagItem.itemId) && !bagItem.isBind)
				{
					_bagItems.Add(bagItem);
				}
			}
		}

		private void UpdateExpands()
		{
			Dictionary<int, int> expend = _curOpenInfo.expend;
			int num = 0;
			foreach (KeyValuePair<int, int> item in expend)
			{
				m_expends[num].SetActiveBetter(true);
				FillExpendCell(m_expendslist[num], item);
				num++;
			}
			int i = _curOpenInfo.expend.Count;
			for (int num2 = m_expends.Length; i < num2; i++)
			{
				m_expends[i].SetActiveBetter(false);
			}
		}

		private void FillExpendCell(GManorChestExpendCell cell, KeyValuePair<int, int> pair)
		{
			ItemCfg itemCfg = ItemCfg.Get(pair.Key);
			View.SetLabelText(cell.txt_nameText, Utils.GetString(100, itemCfg.name, string.Empty));
			View.SetLabelText(cell.txt_timeText, GetHourTime(pair.Value));
			Color color = ((GetItemInToolBoxByItemId(pair.Key) < pair.Value) ? Color.red : _normalColorExpends);
			cell.txt_nameText.color = color;
			cell.txt_timeText.color = color;
		}

		private int GetItemInToolBoxByItemId(int itemId)
		{
			int num = 0;
			foreach (KeyValuePair<int, ManorChestItem> item in _curOpenInfo.items)
			{
				ManorChestItem value = item.Value;
				if (value != null && value.itemId == itemId)
				{
					num += value.number;
				}
			}
			return num;
		}

		private void OnItemChange(int itemId, int num)
		{
			FiltrateBagItem();
			UpdateBagItem(true);
			SetBagCapacity();
			_nextTimeUpdateRepairDataTime = 0f;
			_nextTimeUpdateUpgradeDataTimes.Clear();
			if (m_repair.activeSelf)
			{
				RefreshRepairData();
			}
			if (m_upgrade.activeSelf)
			{
				RefreshUpgradeData(_curOpenLevel);
			}
			StartCoroutine(RefreshAllData());
		}

		private void SetBagCapacity()
		{
			View.SetLabelText(txt_bag_numText, Utils.GetString(9, Singleton<BagMgr>.Ins.BagItems.Count, Singleton<BagMgr>.Ins.BagCapacity));
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

		private void FillBagItemCell(GameObject go, int index)
		{
			_003CFillBagItemCell_003Ec__AnonStorey2 _003CFillBagItemCell_003Ec__AnonStorey = new _003CFillBagItemCell_003Ec__AnonStorey2();
			GManorChestBagCell component = go.GetComponent<GManorChestBagCell>();
			if (_bagItems.Count <= index)
			{
				component.m_something.SetActiveBetter(false);
				component.m_nothing.SetActiveBetter(true);
				return;
			}
			component.m_something.SetActiveBetter(true);
			component.m_nothing.SetActiveBetter(false);
			_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo = _bagItems[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId);
			component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.instanceId));
			component.m_durability.SetActiveBetter(false);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, _003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.number);
			_003CFillBagItemCell_003Ec__AnonStorey.isShowNoviceEffect = Singleton<GuideMgr>.Ins.IsShowToolboxEffect(_003CFillBagItemCell_003Ec__AnonStorey.bagItemInfo.itemId);
			component.m_novice_effect.SetActiveBetter(_003CFillBagItemCell_003Ec__AnonStorey.isShowNoviceEffect);
			ClickListener.Get(go, string.Empty).onClick = _003CFillBagItemCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void RefreshItemList()
		{
			_allItemList.Clear();
			Dictionary<int, ManorChestItem> items = _curOpenInfo.items;
			foreach (ManorChestItem value in items.Values)
			{
				_allItemList.Add(value);
			}
			_allItemList.Sort(SortItemList);
			_emptyIndex.Clear();
			int i = 0;
			for (int mANOR_CHEST_CAPACITY = cfg.Consts.MANOR_CHEST_CAPACITY; i < mANOR_CHEST_CAPACITY; i++)
			{
				if (!_curOpenInfo.items.ContainsKey(i))
				{
					_emptyIndex.Enqueue(i);
				}
			}
		}

		private int SortItemList(ManorChestItem item1, ManorChestItem item2)
		{
			if (item1.itemId == item2.itemId)
			{
				return item2.number - item1.number;
			}
			return item1.itemId - item2.itemId;
		}

		private void OnSPutInManorChestItem(SPutInManorChestItem msg)
		{
			if (msg.chestInstanceId != _curOpenInfo.instanceId)
			{
				return;
			}
			Dictionary<int, int> items = msg.items;
			foreach (KeyValuePair<int, int> item in items)
			{
				if (_curOpenInfo.items.ContainsKey(item.Key))
				{
					_curOpenInfo.items[item.Key].number += item.Value;
					continue;
				}
				_curOpenInfo.items.Add(item.Key, new ManorChestItem
				{
					itemId = msg.itemId,
					number = item.Value
				});
			}
			RefreshItemList();
			UpdateManorItems(true);
			UpdateRemainTime();
			UpdateExpands();
		}

		private void OnSGetManorChestItem(SGetManorChestItem msg)
		{
			ManorChestItem value;
			if (msg.chestInstanceId == _curOpenInfo.instanceId && _curOpenInfo.items.TryGetValue(msg.index, out value))
			{
				value.number -= msg.number;
				if (value.number <= 0)
				{
					_curOpenInfo.items.Remove(msg.index);
					_allItemList.Remove(value);
					_emptyIndex.Enqueue(msg.index);
				}
				RefreshItemList();
				UpdateManorItems(true);
				UpdateRemainTime();
				UpdateExpands();
			}
		}

		private void UpdateManorItems(bool isNoPos)
		{
			if (isNoPos)
			{
				_manorScrollPanel.ResetNoPosClear(cfg.Consts.MANOR_CHEST_CAPACITY, FillManorCell);
			}
			else
			{
				_manorScrollPanel.Reset(cfg.Consts.MANOR_CHEST_CAPACITY, FillManorCell);
			}
		}

		private void FillManorCell(GameObject go, int index)
		{
			_003CFillManorCell_003Ec__AnonStorey3 _003CFillManorCell_003Ec__AnonStorey = new _003CFillManorCell_003Ec__AnonStorey3();
			_003CFillManorCell_003Ec__AnonStorey._0024this = this;
			GManorChestItemCell component = go.GetComponent<GManorChestItemCell>();
			if (_allItemList.Count <= index)
			{
				component.m_icon.SetActiveBetter(false);
				component.txt_num.SetActiveBetter(false);
				return;
			}
			component.m_icon.SetActiveBetter(true);
			component.txt_num.SetActiveBetter(true);
			_003CFillManorCell_003Ec__AnonStorey.chestItemInfo = _allItemList[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillManorCell_003Ec__AnonStorey.chestItemInfo.itemId);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, _003CFillManorCell_003Ec__AnonStorey.chestItemInfo.number);
			ClickListener.Get(go, string.Empty).onClick = _003CFillManorCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private int GetIndexByManorChestItem(ManorChestItem item)
		{
			foreach (KeyValuePair<int, ManorChestItem> item2 in _curOpenInfo.items)
			{
				if (item == item2.Value)
				{
					return item2.Key;
				}
			}
			throw new KeyNotFoundException();
		}

		private IEnumerator WaitHide()
		{
			yield return Utils.WaitForSeconds(0.1f);
			Hide();
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			StopAllCoroutines();
			_allItemList.Clear();
			_nextTimeUpdateUpgradeDataTimes.Clear();
			_nextTimeUpdateRepairDataTime = 0f;
			SGetManorChestItem.handler = (SGetManorChestItem.Handler)Delegate.Remove(SGetManorChestItem.handler, new SGetManorChestItem.Handler(OnSGetManorChestItem));
			SPutInManorChestItem.handler = (SPutInManorChestItem.Handler)Delegate.Remove(SPutInManorChestItem.handler, new SPutInManorChestItem.Handler(OnSPutInManorChestItem));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			BagEvent.OnClickLingDiGuiBtn = (Action<BagItem, int>)Delegate.Remove(BagEvent.OnClickLingDiGuiBtn, new Action<BagItem, int>(OnAddItem));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Remove(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
			ManorChestEvent.OnBuildingToolBoxIdChangeEvent = (Action<long, long, long>)Delegate.Remove(ManorChestEvent.OnBuildingToolBoxIdChangeEvent, new Action<long, long, long>(OnSBuildingToolBoxIdChanged));
			EventHandlers.OnDestroyedPart -= OnBuildDestroy;
			SUpgradeAllBuilding.handler = (SUpgradeAllBuilding.Handler)Delegate.Remove(SUpgradeAllBuilding.handler, new SUpgradeAllBuilding.Handler(OnSUpgradeAllBuilding));
		}

		private string GetHourTime(int num)
		{
			return Utils.GetString(18, Utils.GetString(9, num, string.Empty));
		}

		private string GetHourTime(float num, int digits)
		{
			return Utils.GetString(18, (int)num);
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
			m_cell = View.AddComponentIfNotExist<GManorChestItemCell>(component.GameObjects[7].gameObject);
			txt_bag_num = component.GameObjects[8].gameObject;
			txt_bag_numText = txt_bag_num.GetComponent<Text>();
			txt_remain_time = component.GameObjects[9].gameObject;
			txt_remain_timeText = txt_remain_time.GetComponent<Text>();
			scp_ling_di_gui = component.GameObjects[10].gameObject;
			m_expends = component.GameObjects[11].gameObject.GetComponent<UIGameObjectList>().objects;
			m_expendsObj = component.GameObjects[11].gameObject;
			if (m_expendslist.Count <= 0)
			{
				for (int i = 0; i < m_expends.Length; i++)
				{
					m_expendslist.Add(View.AddComponentIfNotExist<GManorChestExpendCell>(m_expends[i].gameObject));
				}
			}
			btn_permit = component.GameObjects[12].gameObject;
			btn_repair = component.GameObjects[13].gameObject;
			m_repair_red_dot = component.GameObjects[14].gameObject;
			btn_upgrade = component.GameObjects[15].gameObject;
			m_upgrade_red_dot = component.GameObjects[16].gameObject;
			m_upgrade = component.GameObjects[17].gameObject;
			scp_upgrade = component.GameObjects[18].gameObject;
			txt_upgrade_total_num = component.GameObjects[19].gameObject;
			txt_upgrade_total_numText = txt_upgrade_total_num.GetComponent<Text>();
			m_to_stone = component.GameObjects[20].gameObject;
			btn_up_stone = component.GameObjects[21].gameObject;
			m_to_iron = component.GameObjects[22].gameObject;
			btn_up_iron = component.GameObjects[23].gameObject;
			m_to_steel = component.GameObjects[24].gameObject;
			btn_up_steel = component.GameObjects[25].gameObject;
			btn_to_stone = component.GameObjects[26].gameObject;
			txt_off = component.GameObjects[27].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_to_iron = component.GameObjects[28].gameObject;
			btn_to_steel = component.GameObjects[29].gameObject;
			btn_back_upgrade = component.GameObjects[30].gameObject;
			m_repair = component.GameObjects[31].gameObject;
			scp_repair = component.GameObjects[32].gameObject;
			txt_repair_total_num = component.GameObjects[33].gameObject;
			txt_repair_total_numText = txt_repair_total_num.GetComponent<Text>();
			btn_repair_confirm = component.GameObjects[34].gameObject;
			btn_back_repair = component.GameObjects[35].gameObject;
			m_red_dot_stone = component.GameObjects[36].gameObject;
			m_red_dot_iron = component.GameObjects[37].gameObject;
			m_red_dot_steel = component.GameObjects[38].gameObject;
			m_cell_0 = View.AddComponentIfNotExist<GManorChestUpgradeCell>(component.GameObjects[39].gameObject);
			txt_upgrade_stone_need = component.GameObjects[40].gameObject;
			txt_upgrade_stone_needText = txt_upgrade_stone_need.GetComponent<Text>();
			txt_upgrade_iron_need = component.GameObjects[41].gameObject;
			txt_upgrade_iron_needText = txt_upgrade_iron_need.GetComponent<Text>();
			txt_upgrade_steel_need = component.GameObjects[42].gameObject;
			txt_upgrade_steel_needText = txt_upgrade_steel_need.GetComponent<Text>();
			txt_on_0 = component.GameObjects[43].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[44].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_0 = component.GameObjects[45].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_2 = component.GameObjects[46].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_1 = component.GameObjects[47].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			m_cell_1 = View.AddComponentIfNotExist<GManorChestRepairCell>(component.GameObjects[48].gameObject);
			m_cell_2 = View.AddComponentIfNotExist<GManorChestBagCell>(component.GameObjects[49].gameObject);
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
			ViewMgr.Ins.ShowView<FriendPermitPanel>(_curOpenInfo.instanceId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			m_repair.SetActiveBetter(true);
			UpdateRepairPage();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			m_repair.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			m_upgrade.SetActiveBetter(true);
			RadioButton.ChooseBtn(btn_to_stone);
			UpdateUpgradePage(1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			UpdateUpgradePage(1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			UpdateUpgradePage(2);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			UpdateUpgradePage(3);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			m_upgrade.SetActiveBetter(false);
			_curOpenLevel = 0;
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__9(GameObject go)
		{
			if (_totalRepairNum == 0)
			{
				AlertBox.Show(389);
				return;
			}
			if (_repairNotEnoughItem.Count > 0)
			{
				ShowNotEnoughItemName(_repairNotEnoughItem, false);
				return;
			}
			Singleton<ManorChestMgr>.Ins.SendFixAllBuildingMsg(_curOpenInfo.instanceId);
			_nextTimeUpdateRepairDataTime = 0f;
			_repairList.Clear();
			_repairNotEnoughItem.Clear();
			_totalRepairNum = 0;
			UpdateRepairPageNotRefreshData();
			UpdateAllRedDot();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__A(GameObject go)
		{
			OnClickUpgradeConfirm(1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__B(GameObject go)
		{
			OnClickUpgradeConfirm(2);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__C(GameObject go)
		{
			OnClickUpgradeConfirm(3);
		}
	}
}
