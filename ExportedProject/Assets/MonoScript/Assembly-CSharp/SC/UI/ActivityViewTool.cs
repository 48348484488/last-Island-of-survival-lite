using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

namespace SC.UI
{
	public class ActivityViewTool
	{
		[CompilerGenerated]
		private sealed class _003CSetActivitySevenDayItems_003Ec__AnonStorey0
		{
			internal DropMgr.DropDesInfo dropInfo;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(dropInfo.itemId));
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetDailyGiftCell_003Ec__AnonStorey2
		{
			internal bool isBuyed;

			internal DaylyGiftCfg cfgInfo;

			internal List<DropMgr.DropDesInfo> itemInfos;

			internal int cfgId;

			internal void _003C_003Em__0(GameObject go)
			{
				if (!isBuyed && Singleton<RoleMgr>.Ins.IsCouponEnough(cfgInfo.price, Singleton<RechargeMgr>.Ins.ShowRecharge))
				{
					if (!BagCapacityEnough(itemInfos))
					{
						AlertBox.Show(26);
					}
					else
					{
						Singleton<ActivityMgr>.Ins.SendBuyDailyGiftMsg(cfgId);
					}
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetDailyGiftCell_003Ec__AnonStorey1
		{
			internal DropMgr.DropDesInfo dropInfo;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(dropInfo.itemId));
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetFirstChargeCell_003Ec__AnonStorey4
		{
			internal bool isAlreadyGot;

			internal List<DropMgr.DropDesInfo> itemInfos;

			internal void _003C_003Em__0(GameObject go)
			{
				if (!isAlreadyGot)
				{
					if (!BagCapacityEnough(itemInfos))
					{
						AlertBox.Show(26);
					}
					else
					{
						Singleton<RechargeMgr>.Ins.ShowRecharge(true);
					}
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetFirstChargeCell_003Ec__AnonStorey3
		{
			internal DropMgr.DropDesInfo dropInfo;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(dropInfo.itemId));
			}
		}

		public static void SetActivitySevenDayItems(GameObject itemParent, List<DropMgr.DropDesInfo> itemInfos)
		{
			Transform transform = itemParent.transform;
			int childCount = transform.childCount;
			int count = itemInfos.Count;
			int num = ((childCount <= count) ? count : childCount);
			GameObject gameObject = transform.GetChild(0).gameObject;
			for (int i = 0; i < num; i++)
			{
				_003CSetActivitySevenDayItems_003Ec__AnonStorey0 _003CSetActivitySevenDayItems_003Ec__AnonStorey = new _003CSetActivitySevenDayItems_003Ec__AnonStorey0();
				if (count <= i)
				{
					transform.GetChild(i).gameObject.SetActiveBetter(false);
					continue;
				}
				GameObject gameObject3;
				if (childCount <= i)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject);
					Transform transform2 = gameObject2.transform;
					transform2.SetParent(transform);
					transform2.localPosition = Vector3.zero;
					transform2.localScale = Vector3.one;
					transform2.localEulerAngles = Vector3.zero;
					gameObject3 = gameObject2;
				}
				else
				{
					gameObject3 = transform.GetChild(i).gameObject;
				}
				gameObject3.SetActiveBetter(true);
				GActivitySevenDayItemCell component = gameObject3.GetComponent<GActivitySevenDayItemCell>();
				_003CSetActivitySevenDayItems_003Ec__AnonStorey.dropInfo = itemInfos[i];
				component.m_bind.SetActiveBetter(_003CSetActivitySevenDayItems_003Ec__AnonStorey.dropInfo.isBinding);
				View.SetItemSprite(component.m_icon, _003CSetActivitySevenDayItems_003Ec__AnonStorey.dropInfo.icon);
				View.SetLabelText(component.txt_numText, Utils.GetString(281, _003CSetActivitySevenDayItems_003Ec__AnonStorey.dropInfo.num));
				ClickListener.Get(component.gameObject, string.Empty).onClick = _003CSetActivitySevenDayItems_003Ec__AnonStorey._003C_003Em__0;
			}
		}

		public static void SetDailyGiftCell(GActivityDailyGiftCell cell, DaylyGiftCfg cfgInfo)
		{
			_003CSetDailyGiftCell_003Ec__AnonStorey2 _003CSetDailyGiftCell_003Ec__AnonStorey = new _003CSetDailyGiftCell_003Ec__AnonStorey2();
			_003CSetDailyGiftCell_003Ec__AnonStorey.cfgInfo = cfgInfo;
			_003CSetDailyGiftCell_003Ec__AnonStorey.cfgId = _003CSetDailyGiftCell_003Ec__AnonStorey.cfgInfo.id;
			View.SetLabelText(cell.txt_priceText, _003CSetDailyGiftCell_003Ec__AnonStorey.cfgInfo.price);
			_003CSetDailyGiftCell_003Ec__AnonStorey.isBuyed = Singleton<ActivityMgr>.Ins.IsAlreadyGotDailyGift(_003CSetDailyGiftCell_003Ec__AnonStorey.cfgId);
			cell.btn_buy.SetActiveBetter(!_003CSetDailyGiftCell_003Ec__AnonStorey.isBuyed);
			cell.btn_already_got.SetActiveBetter(_003CSetDailyGiftCell_003Ec__AnonStorey.isBuyed);
			_003CSetDailyGiftCell_003Ec__AnonStorey.itemInfos = Singleton<DropMgr>.Ins.GetDropDetailInfo(_003CSetDailyGiftCell_003Ec__AnonStorey.cfgInfo.dropId);
			int num = cell.m_items.Length;
			int count = _003CSetDailyGiftCell_003Ec__AnonStorey.itemInfos.Count;
			for (int i = 0; i < num; i++)
			{
				_003CSetDailyGiftCell_003Ec__AnonStorey1 _003CSetDailyGiftCell_003Ec__AnonStorey2 = new _003CSetDailyGiftCell_003Ec__AnonStorey1();
				if (count <= i)
				{
					cell.m_items[i].SetActiveBetter(false);
					continue;
				}
				GActivityDailyGiftItemCell gActivityDailyGiftItemCell = cell.m_itemslist[i];
				_003CSetDailyGiftCell_003Ec__AnonStorey2.dropInfo = _003CSetDailyGiftCell_003Ec__AnonStorey.itemInfos[i];
				gActivityDailyGiftItemCell.m_bind.SetActiveBetter(_003CSetDailyGiftCell_003Ec__AnonStorey2.dropInfo.isBinding);
				View.SetItemSprite(gActivityDailyGiftItemCell.m_icon, _003CSetDailyGiftCell_003Ec__AnonStorey2.dropInfo.icon);
				View.SetLabelText(gActivityDailyGiftItemCell.txt_numText, Utils.GetString(281, _003CSetDailyGiftCell_003Ec__AnonStorey2.dropInfo.num));
				ClickListener.Get(cell.m_items[i], string.Empty).onClick = _003CSetDailyGiftCell_003Ec__AnonStorey2._003C_003Em__0;
			}
			ClickListener.Get(cell.btn_buy, string.Empty).onClick = _003CSetDailyGiftCell_003Ec__AnonStorey._003C_003Em__0;
		}

		public static void SetFirstChargeCell(GActivityFirstChargeCell cell, List<DropMgr.DropDesInfo> itemInfos)
		{
			_003CSetFirstChargeCell_003Ec__AnonStorey4 _003CSetFirstChargeCell_003Ec__AnonStorey = new _003CSetFirstChargeCell_003Ec__AnonStorey4();
			_003CSetFirstChargeCell_003Ec__AnonStorey.itemInfos = itemInfos;
			cell.m_price.SetActiveBetter(false);
			_003CSetFirstChargeCell_003Ec__AnonStorey.isAlreadyGot = Singleton<ActivityMgr>.Ins.IsAlreadyGotFirstCharge();
			cell.btn_buy.SetActiveBetter(!_003CSetFirstChargeCell_003Ec__AnonStorey.isAlreadyGot);
			cell.btn_already_got.SetActiveBetter(_003CSetFirstChargeCell_003Ec__AnonStorey.isAlreadyGot);
			int num = cell.m_items.Length;
			int count = _003CSetFirstChargeCell_003Ec__AnonStorey.itemInfos.Count;
			for (int i = 0; i < num; i++)
			{
				_003CSetFirstChargeCell_003Ec__AnonStorey3 _003CSetFirstChargeCell_003Ec__AnonStorey2 = new _003CSetFirstChargeCell_003Ec__AnonStorey3();
				if (count <= i)
				{
					cell.m_items[i].SetActiveBetter(false);
					continue;
				}
				GActivityFirstChargeItemCell gActivityFirstChargeItemCell = cell.m_itemslist[i];
				_003CSetFirstChargeCell_003Ec__AnonStorey2.dropInfo = _003CSetFirstChargeCell_003Ec__AnonStorey.itemInfos[i];
				gActivityFirstChargeItemCell.m_bind.SetActiveBetter(_003CSetFirstChargeCell_003Ec__AnonStorey2.dropInfo.isBinding);
				View.SetItemSprite(gActivityFirstChargeItemCell.m_icon, _003CSetFirstChargeCell_003Ec__AnonStorey2.dropInfo.icon);
				View.SetLabelText(gActivityFirstChargeItemCell.txt_numText, Utils.GetString(281, _003CSetFirstChargeCell_003Ec__AnonStorey2.dropInfo.num));
				ClickListener.Get(cell.m_items[i], string.Empty).onClick = _003CSetFirstChargeCell_003Ec__AnonStorey2._003C_003Em__0;
			}
			ClickListener.Get(cell.btn_buy, string.Empty).onClick = _003CSetFirstChargeCell_003Ec__AnonStorey._003C_003Em__0;
		}

		public static bool BagCapacityEnough(List<DropMgr.DropDesInfo> infoList)
		{
			int num = 0;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int i = 0;
			for (int count = infoList.Count; i < count; i++)
			{
				DropMgr.DropDesInfo dropDesInfo = infoList[i];
				int value;
				if (!dictionary.TryGetValue(dropDesInfo.itemId, out value))
				{
					dictionary[dropDesInfo.itemId] = dropDesInfo.num;
					continue;
				}
				int maxPileNum = ItemCfg.Get(dropDesInfo.itemId).maxPileNum;
				value += dropDesInfo.num;
				if (value >= maxPileNum)
				{
					num++;
					value -= maxPileNum;
				}
				dictionary[dropDesInfo.itemId] = value;
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
	}
}
