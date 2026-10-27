using System;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using gs.drop.scmsg;

public class DropMgr : Singleton<DropMgr>
{
	public class DropDesInfo
	{
		public string icon;

		public string name;

		public int type;

		public int itemId;

		public string frame;

		public int num;

		public int quality;

		public float useTime;

		public bool isBinding;
	}

	private const int MAX_PROBABILITY = 10000;

	public DropDetail GetDropDetail(int dropTableId)
	{
		DropDetail dropDetail = new DropDetail();
		DropItemCfg dropItemCfg = DropItemCfg.Get(dropTableId);
		if (dropItemCfg == null)
		{
			return null;
		}
		foreach (FixDrop fixDrop in dropItemCfg.fixDrops)
		{
			int value = fixDrop.value;
			int dropType = fixDrop.dropType;
			switch (dropType)
			{
			case 17:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 0:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 2:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 8:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 16:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 3:
				UpdateDropDetailItems(dropDetail, fixDrop.id, fixDrop.value);
				break;
			case 11:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 14:
				UpdateDropDetailProps(dropDetail, dropType, value);
				break;
			case 12:
				UpdateDropDetailFrag(dropDetail, fixDrop.id, value);
				break;
			case 18:
				UpdateDropDetailBindItems(dropDetail, fixDrop.id, value);
				break;
			}
		}
		if (dropItemCfg.relation)
		{
			foreach (DropDetailInfo detailId in dropItemCfg.detailIds)
			{
				DropItemDetailCfg dropItemDetailCfg = DropItemDetailCfg.Get(detailId.dropDetailId);
				foreach (DropInfo subItem in dropItemDetailCfg.subItems)
				{
					int value2 = subItem.value;
					int dropType2 = subItem.dropType;
					switch (dropType2)
					{
					case 17:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 0:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 2:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 8:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 16:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 3:
						UpdateDropDetailItems(dropDetail, value2, dropItemCfg.dropNum);
						break;
					case 11:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 14:
						UpdateDropDetailProps(dropDetail, dropType2, value2 * dropItemCfg.dropNum);
						break;
					case 12:
						UpdateDropDetailFrag(dropDetail, value2, value2 * dropItemCfg.dropNum);
						break;
					case 18:
						UpdateDropDetailBindItems(dropDetail, value2, value2 * dropItemCfg.dropNum);
						break;
					}
				}
			}
			return dropDetail;
		}
		return dropDetail;
	}

	public bool isEmpty(DropDetail dropDetail)
	{
		return dropDetail == null || (dropDetail.dropItem.Count == 0 && dropDetail.dropProp.Count == 0 && dropDetail.bindDropItem.Count == 0);
	}

	private void UpdateDropDetailProps(DropDetail dropDetail, int dropType, int value)
	{
		if (dropDetail.dropProp.ContainsKey(dropType))
		{
			dropDetail.dropProp[dropType] = dropDetail.dropProp[dropType] + value;
		}
		else
		{
			dropDetail.dropProp[dropType] = value;
		}
	}

	private void UpdateDropDetailItems(DropDetail dropDetail, int itemId, int value)
	{
		if (dropDetail.dropItem.ContainsKey(itemId))
		{
			dropDetail.dropItem[itemId] = dropDetail.dropItem[itemId] + value;
		}
		else
		{
			dropDetail.dropItem[itemId] = value;
		}
	}

	private void UpdateDropDetailBindItems(DropDetail dropDetail, int itemId, int value)
	{
		if (dropDetail.bindDropItem.ContainsKey(itemId))
		{
			dropDetail.bindDropItem[itemId] = dropDetail.bindDropItem[itemId] + value;
		}
		else
		{
			dropDetail.bindDropItem[itemId] = value;
		}
	}

	private void UpdateDropDetailFrag(DropDetail dropDetail, int fragId, int value)
	{
	}

	public List<DropDesInfo> GetDropDetailInfo(int dropTableId, int times = 1)
	{
		return GetDropDetailInfo(GetDropDetail(dropTableId), times);
	}

	public List<DropDesInfo> GetDropDetailInfo(DropDetail dropDetail, int times = 1)
	{
		List<DropDesInfo> list = new List<DropDesInfo>();
		if (dropDetail == null)
		{
			return list;
		}
		if (dropDetail.dropItem.Count > 0)
		{
			foreach (KeyValuePair<int, int> item in dropDetail.dropItem)
			{
				ItemCfg itemCfg = ItemCfg.Get(item.Key);
				if (itemCfg == null)
				{
					throw new Exception("no frag " + item.Key + " fragId=" + item.Value);
				}
				DropDesInfo dropDesInfo = new DropDesInfo();
				if (itemCfg.type == 63)
				{
					if (itemCfg.extrasstring.Count > 1)
					{
						dropDesInfo.icon = itemCfg.extrasstring[1];
					}
				}
				else if (itemCfg.type == 73)
				{
					if (itemCfg.extras.Count > 0)
					{
						ItemCfg itemCfg2 = ItemCfg.Get((int)itemCfg.extras[0]);
						if (itemCfg2.extrasstring.Count > 1)
						{
							dropDesInfo.icon = itemCfg2.extrasstring[1];
						}
					}
				}
				else if (itemCfg.icon.StartsWith("texture"))
				{
					dropDesInfo.icon = itemCfg.icon;
				}
				else
				{
					dropDesInfo.icon = itemCfg.icon;
				}
				dropDesInfo.name = itemCfg.name;
				dropDesInfo.num = item.Value * times;
				dropDesInfo.useTime = itemCfg.useTime;
				switch (itemCfg.type)
				{
				case 16:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 42:
				case 43:
					dropDesInfo.type = 7;
					break;
				default:
					dropDesInfo.type = 3;
					break;
				}
				dropDesInfo.itemId = item.Key;
				dropDesInfo.quality = itemCfg.quality;
				list.Add(dropDesInfo);
			}
		}
		if (dropDetail.bindDropItem.Count > 0)
		{
			foreach (KeyValuePair<int, int> item2 in dropDetail.bindDropItem)
			{
				ItemCfg itemCfg3 = ItemCfg.Get(item2.Key);
				if (itemCfg3 == null)
				{
					throw new Exception("no frag " + item2.Key + " fragId=" + item2.Value);
				}
				DropDesInfo dropDesInfo2 = new DropDesInfo();
				if (itemCfg3.type == 63)
				{
					if (itemCfg3.extrasstring.Count > 1)
					{
						dropDesInfo2.icon = itemCfg3.extrasstring[1];
					}
				}
				else if (itemCfg3.type == 73)
				{
					if (itemCfg3.extras.Count > 0)
					{
						ItemCfg itemCfg4 = ItemCfg.Get((int)itemCfg3.extras[0]);
						if (itemCfg4.extrasstring.Count > 1)
						{
							dropDesInfo2.icon = itemCfg4.extrasstring[1];
						}
					}
				}
				else if (itemCfg3.icon.StartsWith("texture"))
				{
					dropDesInfo2.icon = itemCfg3.icon;
				}
				else
				{
					dropDesInfo2.icon = itemCfg3.icon;
				}
				dropDesInfo2.name = itemCfg3.name;
				dropDesInfo2.num = item2.Value * times;
				dropDesInfo2.useTime = itemCfg3.useTime;
				switch (itemCfg3.type)
				{
				case 16:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 42:
				case 43:
					dropDesInfo2.type = 7;
					break;
				default:
					dropDesInfo2.type = 3;
					break;
				}
				dropDesInfo2.itemId = item2.Key;
				dropDesInfo2.quality = itemCfg3.quality;
				dropDesInfo2.isBinding = true;
				list.Add(dropDesInfo2);
			}
		}
		if (dropDetail.dropProp.Count > 0)
		{
			foreach (KeyValuePair<int, int> item3 in dropDetail.dropProp)
			{
				DropDesInfo dropDesInfo3 = new DropDesInfo();
				switch (item3.Key)
				{
				case 0:
					dropDesInfo3.icon = "common/" + cfg.Consts.GOLD_ICON;
					dropDesInfo3.name = Utils.GetString(166);
					dropDesInfo3.type = 0;
					break;
				case 2:
					dropDesInfo3.icon = "common/" + cfg.Consts.COUPON_ICON;
					dropDesInfo3.name = Utils.GetString(100);
					dropDesInfo3.type = 0;
					break;
				case 8:
					dropDesInfo3.name = Utils.GetString(155);
					dropDesInfo3.type = 0;
					break;
				case 16:
					dropDesInfo3.icon = "common/" + cfg.Consts.LADDDER_LEVEL_IOCN;
					dropDesInfo3.name = Utils.GetString(303);
					dropDesInfo3.type = 16;
					break;
				case 11:
					dropDesInfo3.name = Utils.GetString(525);
					dropDesInfo3.type = 11;
					break;
				case 17:
					dropDesInfo3.icon = "common/" + cfg.Consts.LADDDER_LEVEL_IOCN;
					dropDesInfo3.name = Utils.GetString(201);
					dropDesInfo3.type = 17;
					break;
				}
				dropDesInfo3.num = item3.Value * times;
				list.Add(dropDesInfo3);
			}
		}
		list.Sort(sort);
		return list;
	}

	public string getItemDesc(int itemDropType, int itemId)
	{
		switch (itemDropType)
		{
		case 0:
			return string.Empty;
		case 7:
		{
			ShopCfg shopCfg = ShopCfg.Get(itemId);
			if (shopCfg != null)
			{
				return shopCfg.desc;
			}
			return string.Empty;
		}
		case 3:
		case 18:
		{
			ItemCfg itemCfg = ItemCfg.Get(itemId);
			if (itemCfg == null)
			{
				Debug.LogError("[dropMgr]getItemDesc:no item itemDropType=" + itemDropType + ",itemId=" + itemId);
			}
			return itemCfg.desc;
		}
		case 16:
			return string.Empty;
		case 11:
			return string.Empty;
		default:
			Debug.LogError("[dropMgr]getItemDesc:unknown itemDropType=" + itemDropType + ",itemId=" + itemId);
			return "404";
		}
	}

	private int sort(DropDesInfo info1, DropDesInfo info2)
	{
		if (info1.type != info2.type)
		{
			return info1.type - info2.type;
		}
		return info1.itemId - info2.itemId;
	}
}
