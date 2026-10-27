using System;
using System.Collections.Generic;
using System.Linq;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.battle.drop.scmsg;

public class BagMgr : Singleton<BagMgr>
{
	public class GunData
	{
		public readonly int InsId;

		public readonly int Id;

		public readonly GunCfg GunCfg;

		public readonly float[] Factors;

		public readonly int SkinId;

		public int CurBulletNum;

		public readonly HashSet<int> PartsId = new HashSet<int>();

		public int Index;

		private GunParts m_gunParts;

		public int BulletMax
		{
			get
			{
				return (int)Factors[34];
			}
		}

		public GunData(long insId, int itemId, GunParts gunParts)
		{
			InsId = (int)insId;
			GunCfg = GunCfg.Get(itemId);
			Factors = new float[GunCfg.factors.Count];
			Id = itemId;
			m_gunParts = gunParts;
			CurBulletNum = gunParts.bulletNumber;
			UpdatePartsId();
			RefreshAllFactors();
		}

		public void UpdatePartsId()
		{
			PartsId.Clear();
			foreach (BagItem part in m_gunParts.parts)
			{
				PartsId.Add(part.itemId);
			}
		}

		public void RefreshAllFactors()
		{
			int i = 0;
			for (int num = Factors.Length; i < num; i++)
			{
				Factors[i] = GunCfg.factors[i];
			}
			foreach (BagItem part in m_gunParts.parts)
			{
				if (part.duration <= 0)
				{
					break;
				}
				ChangePartFactors(part.itemId, true);
			}
		}

		public void UpdateGunPart(GunParts gunParts)
		{
			m_gunParts = gunParts;
			UpdatePartsId();
		}

		public void ChangePartFactors(int partId, bool add)
		{
			PartCfg partCfg = PartCfg.Get(partId);
			if (partCfg == null)
			{
				Debug.LogError(string.Format("[gun]part {0} not exist in table cfg.PartCfg.", partId));
				return;
			}
			Utils.TriggerEvent(BagEvent.ChangeGunFactors, InsId);
			PartProp partProp = GetPartProp(GunCfg.id, partCfg);
			if (partProp == null)
			{
				return;
			}
			try
			{
				int i = 0;
				for (int count = partProp.ids.Count; i < count; i++)
				{
					Factors[partProp.ids[i]] += ((!add) ? (0f - partProp.values[i]) : partProp.values[i]);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private static PartProp GetPartProp(int gunid, PartCfg partCfg)
		{
			foreach (PartProp prop in partCfg.props)
			{
				if (prop.gunids.Contains(gunid))
				{
					return prop;
				}
			}
			return null;
		}
	}

	private List<BagItem> _bagItems = new List<BagItem>();

	private Dictionary<int, BagItem> _quickUseItems = new Dictionary<int, BagItem>();

	private int _quickIndex;

	private HashSet<int> _newBagItems = new HashSet<int>();

	private int _bagCapacity;

	public Dictionary<int, GunParts> _gunDic = new Dictionary<int, GunParts>();

	public Dictionary<long, int> _gunLastBullet = new Dictionary<long, int>();

	public int[] PartTypes = new int[5] { 21, 20, 14, 18, 19 };

	public int[] BuildTypes = new int[1] { 78 };

	public int[] ToHandTypes = new int[3] { 13, 82, 25 };

	private List<BagItem> _skinItems = new List<BagItem>();

	private List<BagItem> _equipItems = new List<BagItem>();

	private int _handInstanceId = -1;

	public bool IsShowEquip = true;

	private Dictionary<long, int> _bagItemZero = new Dictionary<long, int>();

	private Dictionary<long, int> _builds = new Dictionary<long, int>();

	private CBreakItem _cBreakItem = new CBreakItem();

	private CSetQuickUseItem _cSetQuickUseItem = new CSetQuickUseItem();

	private CDiscardItem _cDiscardItem = new CDiscardItem();

	private CUseItem _cUseItem = new CUseItem();

	private CSetShowEquip _cSetShowEquip = new CSetShowEquip();

	private CPutGunPart _cPutGunPart = new CPutGunPart();

	private CRemoveGunPart _cRemoveGunPart = new CRemoveGunPart();

	private CLoadGunBullet _cLoadGunBullet = new CLoadGunBullet();

	private CRemoveGunBullet _cRemoveGunBullet = new CRemoveGunBullet();

	private CPutSkin _cPutSkin = new CPutSkin();

	private CRemoveSkin _cRemoveSkin = new CRemoveSkin();

	private CPutEquip _cPutEquip = new CPutEquip();

	private CRemoveEquip _cRemoveEquip = new CRemoveEquip();

	private CPutTogether _cPutTogether = new CPutTogether();

	private CPutToHand _cPutToHand = new CPutToHand();

	private Dictionary<long, GunData> m_gunDataDic = new Dictionary<long, GunData>();

	public List<BagItem> BagItems
	{
		get
		{
			return _bagItems;
		}
	}

	public Dictionary<int, BagItem> QuickUseItems
	{
		get
		{
			return _quickUseItems;
		}
	}

	public int BagCapacity
	{
		get
		{
			return _bagCapacity;
		}
	}

	public Dictionary<int, GunParts> GunDic
	{
		get
		{
			return _gunDic;
		}
	}

	public List<BagItem> SkinItems
	{
		get
		{
			return _skinItems;
		}
	}

	public HashSet<int> SkinItemsIds
	{
		get
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (BagItem skinItem in _skinItems)
			{
				hashSet.Add(skinItem.itemId);
			}
			return hashSet;
		}
	}

	public List<BagItem> EquipItems
	{
		get
		{
			return _equipItems;
		}
	}

	public HashSet<int> EquipItemsIds
	{
		get
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (BagItem equipItem in _equipItems)
			{
				hashSet.Add(equipItem.itemId);
			}
			return hashSet;
		}
	}

	public int HandInstanceId
	{
		get
		{
			return _handInstanceId;
		}
	}

	public bool IsSkinOrEquipContainType(int itemType)
	{
		return itemType == 9 || itemType == 119;
	}

	public bool IsCanUse(int itemId)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		return itemCfg.canUse;
	}

	public void Init()
	{
		SItems.handler = (SItems.Handler)Delegate.Combine(SItems.handler, new SItems.Handler(SItemsHandle));
		SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
		SItemDurationChanged.handler = (SItemDurationChanged.Handler)Delegate.Combine(SItemDurationChanged.handler, new SItemDurationChanged.Handler(SItemDurationChangedHandle));
		SItemNumberChange.handler = (SItemNumberChange.Handler)Delegate.Combine(SItemNumberChange.handler, new SItemNumberChange.Handler(SItemNumberChangeHandle));
		SBreakItem.handler = (SBreakItem.Handler)Delegate.Combine(SBreakItem.handler, new SBreakItem.Handler(SBreakItemHandle));
		SSetQuickUseItem.handler = (SSetQuickUseItem.Handler)Delegate.Combine(SSetQuickUseItem.handler, new SSetQuickUseItem.Handler(SSetQuickUseItemHandle));
		SDiscardItem.handler = (SDiscardItem.Handler)Delegate.Combine(SDiscardItem.handler, new SDiscardItem.Handler(SDiscardItemHandle));
		SUseItem.handler = (SUseItem.Handler)Delegate.Combine(SUseItem.handler, new SUseItem.Handler(SUseItemHandle));
		SPutGunPart.handler = (SPutGunPart.Handler)Delegate.Combine(SPutGunPart.handler, new SPutGunPart.Handler(SPutGunPartHandle));
		SRemoveGunPart.handler = (SRemoveGunPart.Handler)Delegate.Combine(SRemoveGunPart.handler, new SRemoveGunPart.Handler(SRemoveGunPartHandle));
		SLoadGunBullet.handler = (SLoadGunBullet.Handler)Delegate.Combine(SLoadGunBullet.handler, new SLoadGunBullet.Handler(SLoadGunBulletHandle));
		SRemoveGunBullet.handler = (SRemoveGunBullet.Handler)Delegate.Combine(SRemoveGunBullet.handler, new SRemoveGunBullet.Handler(SRemoveGunBulletHandle));
		SPutSkin.handler = (SPutSkin.Handler)Delegate.Combine(SPutSkin.handler, new SPutSkin.Handler(SPutSkinHandle));
		SRemoveSkin.handler = (SRemoveSkin.Handler)Delegate.Combine(SRemoveSkin.handler, new SRemoveSkin.Handler(SRemoveSkinHandle));
		SPutEquip.handler = (SPutEquip.Handler)Delegate.Combine(SPutEquip.handler, new SPutEquip.Handler(SPutEquipHandle));
		SRemoveEquip.handler = (SRemoveEquip.Handler)Delegate.Combine(SRemoveEquip.handler, new SRemoveEquip.Handler(SRemoveEquipHandle));
		SPutTogether.handler = (SPutTogether.Handler)Delegate.Combine(SPutTogether.handler, new SPutTogether.Handler(SPutTogetherHandle));
		SPutToHand.handler = (SPutToHand.Handler)Delegate.Combine(SPutToHand.handler, new SPutToHand.Handler(SPutToHandHandle));
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnClickUseBuild));
		SSetShowEquip.handler = (SSetShowEquip.Handler)Delegate.Combine(SSetShowEquip.handler, new SSetShowEquip.Handler(SSetShowEquipHandle));
		SBagSizeChange.handler = (SBagSizeChange.Handler)Delegate.Combine(SBagSizeChange.handler, new SBagSizeChange.Handler(SBagSizeChangeHandle));
		SItemInfo.handler = (SItemInfo.Handler)Delegate.Combine(SItemInfo.handler, new SItemInfo.Handler(SItemInfoHandle));
	}

	private void SItemInfoHandle(SItemInfo msg)
	{
	}

	private void SBagSizeChangeHandle(SBagSizeChange msg)
	{
		_bagCapacity = msg.size;
		Utils.TriggerEvent(BagEvent.RefreshBag);
	}

	private void SSetShowEquipHandle(SSetShowEquip msg)
	{
		IsShowEquip = msg.showEquip;
		Utils.TriggerEvent(BagEvent.IsShowEquipBoolDelegate, IsShowEquip);
	}

	private void SItemNumberChangeHandle(SItemNumberChange msg)
	{
		ItemChangeAlert(msg.itemId, msg.change);
	}

	public int GetBuildItemId(long instanceId)
	{
		if (_builds.ContainsKey(instanceId))
		{
			return _builds[instanceId];
		}
		return -1;
	}

	private void OnClickUseBuild(int itemId, long instanceId)
	{
		_builds[instanceId] = itemId;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		switch (itemCfg.childType)
		{
		case 99:
			Singleton<BoxMgr>.Ins.OpenBox(instanceId);
			break;
		case 100:
			ViewMgr.Ins.ShowView<DrawPanel>(instanceId);
			break;
		case 106:
			Singleton<TurretMgr>.Ins.TurretBuildingInfo(instanceId);
			break;
		case 105:
			Singleton<TurretMgr>.Ins.TurretBuildingInfo(instanceId);
			break;
		}
	}

	private void SPutToHandHandle(SPutToHand msg)
	{
		_handInstanceId = msg.instanceId;
		Utils.TriggerEvent(BagEvent.ChangeHandInstanceIdDelegate, msg.instanceId);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
		if (_handInstanceId > 0)
		{
			Debug.LogError("SPutToHandHandle:" + _handInstanceId);
			AlertBox.Show(43);
		}
	}

	private void SPutTogetherHandle(SPutTogether msg)
	{
		int form = 0;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.toInstanceId, out form);
		allItemByInstanceId.number += msg.number;
		if (msg.fromBag)
		{
			BagItem bagItemByInstanceId = GetBagItemByInstanceId(msg.fromInstanceId);
			bagItemByInstanceId.number -= msg.number;
			if (bagItemByInstanceId.number <= 0)
			{
				RemoveBagItemByInstance(bagItemByInstanceId.instanceId);
			}
		}
		else
		{
			BagItem quickUseItemByInstanceId = GetQuickUseItemByInstanceId(msg.fromInstanceId);
			if (quickUseItemByInstanceId != null)
			{
				quickUseItemByInstanceId.number -= msg.number;
				if (quickUseItemByInstanceId.number <= 0)
				{
					RemoveQuickUseItemByInstance(quickUseItemByInstanceId.instanceId);
				}
			}
		}
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
	}

	private void SRemoveEquipHandle(SRemoveEquip msg)
	{
		BagItem equipBagItemByInstanceId = GetEquipBagItemByInstanceId(msg.instanceId);
		_bagItems.Add(equipBagItemByInstanceId);
		_bagItems.Sort(Sort);
		RemoveEquipBagItemByInstanceId(msg.instanceId);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshEquips);
		Utils.TriggerEvent(BagEvent.RemoveEquipByItemId, equipBagItemByInstanceId.itemId);
		EquipCfg equipCfg = EquipCfg.Get(equipBagItemByInstanceId.itemId);
		if (IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.RemoveEquipByItemId2Battle, equipBagItemByInstanceId.itemId);
		}
		int itemId = -1;
		if (IsBasicEquipType(equipCfg.equipType, out itemId) && IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.PutEquipByItemId2Battle, itemId);
		}
	}

	private void SPutEquipHandle(SPutEquip msg)
	{
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId, out form);
		BagItem bagItem = PutNewBagItemAndOutOldEquip(_equipItems, allItemByInstanceId);
		switch (form)
		{
		case 1:
			RemoveBagItemByInstance(allItemByInstanceId.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshBag);
			break;
		case 2:
			RemoveQuickUseItemByInstance(allItemByInstanceId.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshQuickUse);
			break;
		}
		Utils.TriggerEvent(BagEvent.RefreshEquips);
		Utils.TriggerEvent(BagEvent.PutEquipByItemId, allItemByInstanceId.itemId);
		AlertBox.Show(43);
		if (bagItem != null)
		{
			if (IsShowEquip)
			{
				Utils.TriggerEvent(BagEvent.RemoveEquipByItemId2Battle, bagItem.itemId);
			}
		}
		else
		{
			EquipCfg equipCfg = EquipCfg.Get(allItemByInstanceId.itemId);
			int itemId = -1;
			if (IsBasicEquipType(equipCfg.equipType, out itemId) && IsShowEquip)
			{
				Utils.TriggerEvent(BagEvent.RemoveEquipByItemId2Battle, itemId);
			}
		}
		if (IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.PutEquipByItemId2Battle, allItemByInstanceId.itemId);
		}
	}

	private void SRemoveSkinHandle(SRemoveSkin msg)
	{
		BagItem skinBagItemByInstanceId = GetSkinBagItemByInstanceId(msg.instanceId);
		_bagItems.Add(skinBagItemByInstanceId);
		_bagItems.Sort(Sort);
		RemoveSkinBagItemByInstanceId(msg.instanceId);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshSkins);
		Utils.TriggerEvent(BagEvent.RemoveSkinByItemId, skinBagItemByInstanceId.itemId);
		if (!IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.RemoveSkinByItemId2Battle, skinBagItemByInstanceId.itemId);
		}
		SkinCfg skinCfg = SkinCfg.Get(skinBagItemByInstanceId.itemId);
		int itemId = -1;
		if (IsBasicSkinType(skinCfg.skinType, out itemId) && !IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.PutSkinByItemId2Battle, itemId);
		}
	}

	private void SPutSkinHandle(SPutSkin msg)
	{
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId, out form);
		BagItem bagItem = PutNewBagItemAndOutOldSkin(_skinItems, allItemByInstanceId);
		switch (form)
		{
		case 1:
			RemoveBagItemByInstance(allItemByInstanceId.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshBag);
			break;
		case 2:
			RemoveQuickUseItemByInstance(allItemByInstanceId.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshQuickUse);
			break;
		}
		Utils.TriggerEvent(BagEvent.RefreshSkins);
		Utils.TriggerEvent(BagEvent.PutSkinByItemId, allItemByInstanceId.itemId);
		AlertBox.Show(43);
		if (bagItem != null)
		{
			if (!IsShowEquip)
			{
				Utils.TriggerEvent(BagEvent.RemoveSkinByItemId2Battle, bagItem.itemId);
			}
		}
		else
		{
			SkinCfg skinCfg = SkinCfg.Get(allItemByInstanceId.itemId);
			int itemId = -1;
			if (IsBasicSkinType(skinCfg.skinType, out itemId) && !IsShowEquip)
			{
				Utils.TriggerEvent(BagEvent.RemoveSkinByItemId2Battle, itemId);
			}
		}
		if (!IsShowEquip)
		{
			Utils.TriggerEvent(BagEvent.PutSkinByItemId2Battle, allItemByInstanceId.itemId);
		}
	}

	public HashSet<int> GetNowSkinsData()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (BagItem skinItem in SkinItems)
		{
			hashSet.Add(skinItem.itemId);
		}
		foreach (int roleBasicSkin in Singleton<RoleMgr>.Ins.RoleBasicSkins)
		{
			SkinCfg skinCfg = SkinCfg.Get(roleBasicSkin);
			if (skinCfg != null)
			{
				BagItem bagItemByEquipType = GetBagItemByEquipType(skinCfg.skinType);
				if (bagItemByEquipType == null)
				{
					hashSet.Add(roleBasicSkin);
				}
			}
		}
		return hashSet;
	}

	public HashSet<int> GetNowEquipsData()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (BagItem equipItem in EquipItems)
		{
			hashSet.Add(equipItem.itemId);
		}
		foreach (int roleBasicEquip in Singleton<RoleMgr>.Ins.RoleBasicEquips)
		{
			EquipCfg equipCfg = EquipCfg.Get(roleBasicEquip);
			if (equipCfg != null)
			{
				BagItem bagItemByEquipType = GetBagItemByEquipType(equipCfg.equipType);
				if (bagItemByEquipType == null)
				{
					hashSet.Add(roleBasicEquip);
				}
			}
		}
		return hashSet;
	}

	private void SRemoveGunBulletHandle(SRemoveGunBullet msg)
	{
		if (_gunDic.ContainsKey(msg.gunInstanceId))
		{
			_gunDic[msg.gunInstanceId].bulletNumber -= msg.number;
		}
		if (m_gunDataDic.ContainsKey(msg.gunInstanceId))
		{
			m_gunDataDic[msg.gunInstanceId].CurBulletNum -= msg.number;
		}
		Utils.TriggerEvent(BagEvent.RefreshBullet);
		try
		{
			Utils.TriggerEvent(BagEvent.AddOrRemoveBullet, msg.gunInstanceId, m_gunDataDic[msg.gunInstanceId].CurBulletNum);
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void SLoadGunBulletHandle(SLoadGunBullet msg)
	{
		if (_gunDic.ContainsKey(msg.gunInstanceId))
		{
			if (_gunDic[msg.gunInstanceId].bulletId == msg.bulletItemId)
			{
				_gunDic[msg.gunInstanceId].bulletNumber += msg.number;
			}
			else
			{
				_gunDic[msg.gunInstanceId].bulletId = msg.bulletItemId;
				_gunDic[msg.gunInstanceId].bulletNumber = msg.number;
			}
			_gunLastBullet[msg.gunInstanceId] = msg.bulletItemId;
		}
		if (m_gunDataDic.ContainsKey(msg.gunInstanceId))
		{
			m_gunDataDic[msg.gunInstanceId].CurBulletNum = _gunDic[msg.gunInstanceId].bulletNumber;
		}
		try
		{
			Utils.TriggerEvent(BagEvent.AddOrRemoveBullet, msg.gunInstanceId, m_gunDataDic[msg.gunInstanceId].CurBulletNum);
		}
		catch (Exception)
		{
			throw;
		}
		Utils.TriggerEvent(BagEvent.RefreshBullet);
	}

	private void SRemoveGunPartHandle(SRemoveGunPart msg)
	{
		if (_gunDic.ContainsKey(msg.gunInstanceId))
		{
			int type = ItemCfg.Get(msg.itemId).type;
			foreach (BagItem part in _gunDic[msg.gunInstanceId].parts)
			{
				if (part.itemId == msg.itemId)
				{
					_gunDic[msg.gunInstanceId].parts.Remove(part);
					break;
				}
			}
			GunData gunData = m_gunDataDic[msg.gunInstanceId];
			if (gunData != null)
			{
				gunData.UpdateGunPart(_gunDic[msg.gunInstanceId]);
				gunData.ChangePartFactors(msg.itemId, false);
			}
		}
		Utils.TriggerEvent(BagEvent.RefreshGunParts);
		Utils.TriggerEvent(BagEvent.RemoveGunPart, msg.gunInstanceId, msg.itemId);
		RefreshAllFactorsByGunInstanceId(msg.gunInstanceId);
	}

	private void SPutGunPartHandle(SPutGunPart msg)
	{
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.partInstanceId, out form);
		if (_gunDic.ContainsKey(msg.gunInstanceId))
		{
			BagItem bagItem = PutPartInGunAndOutOld(_gunDic[msg.gunInstanceId].parts, allItemByInstanceId);
			if (bagItem != null)
			{
				switch (form)
				{
				case 1:
					_bagItems.Add(bagItem);
					_bagItems.Remove(allItemByInstanceId);
					_bagItems.Sort(Sort);
					break;
				case 2:
				{
					int quickUseIndexByInstanceId = GetQuickUseIndexByInstanceId(msg.partInstanceId);
					AddQuickUseItem(quickUseIndexByInstanceId, bagItem);
					break;
				}
				}
			}
			else
			{
				switch (form)
				{
				case 1:
					_bagItems.Remove(allItemByInstanceId);
					break;
				case 2:
				{
					int quickUseIndexByInstanceId2 = GetQuickUseIndexByInstanceId(msg.partInstanceId);
					_quickUseItems.Remove(quickUseIndexByInstanceId2);
					break;
				}
				}
			}
			GunData gunData = m_gunDataDic[msg.gunInstanceId];
			if (gunData != null)
			{
				gunData.UpdateGunPart(_gunDic[msg.gunInstanceId]);
				gunData.ChangePartFactors(allItemByInstanceId.itemId, true);
			}
		}
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
		Utils.TriggerEvent(BagEvent.RefreshGunParts);
		Utils.TriggerEvent(BagEvent.AddGunPart, msg.gunInstanceId, allItemByInstanceId.itemId);
		RefreshAllFactorsByGunInstanceId(msg.gunInstanceId);
		AlertBox.Show(43);
	}

	public void ShootBullet(int insId, int num)
	{
		if (GunDic.ContainsKey(insId) && GunDic[insId].bulletNumber > 0)
		{
			GunDic[insId].bulletNumber--;
			GunData gunDate = GetGunDate(insId);
			gunDate.CurBulletNum--;
			Utils.TriggerEvent(BagEvent.ShootBullet, insId, 1);
		}
	}

	private void SUseItemHandle(SUseItem msg)
	{
	}

	private void SDiscardItemHandle(SDiscardItem msg)
	{
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId, out form);
		switch (form)
		{
		case 1:
			RemoveBagItemByInstance(msg.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshBag);
			Utils.TriggerEvent(BagEvent.RefreshGunParts);
			Utils.TriggerEvent(BagEvent.RefreshQuickUse);
			break;
		case 2:
			RemoveQuickUseItemByInstance(msg.instanceId);
			Utils.TriggerEvent(BagEvent.RefreshQuickUse);
			break;
		}
		if (allItemByInstanceId != null)
		{
			ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
			if (itemCfg.type == 13)
			{
				m_gunDataDic.Remove(msg.instanceId);
				Utils.TriggerEvent(BagEvent.DropGun, msg.instanceId);
			}
			else
			{
				Utils.TriggerEvent(BagEvent.DiscardDelegate);
			}
		}
		AlertBox.Show(42);
	}

	private void SSetQuickUseItemHandle(SSetQuickUseItem msg)
	{
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId, out form);
		if (msg.index < 0)
		{
			_bagItems.Add(allItemByInstanceId);
			_bagItems.Sort(Sort);
			int quickUseIndexByInstanceId = GetQuickUseIndexByInstanceId(msg.instanceId);
			_quickUseItems.Remove(quickUseIndexByInstanceId);
			AlertBox.Show(331);
		}
		else
		{
			BagItem quickUseItemByIndex = GetQuickUseItemByIndex(msg.index);
			switch (form)
			{
			case 1:
				if (quickUseItemByIndex != null)
				{
					_bagItems.Add(quickUseItemByIndex);
					_bagItems.Sort(Sort);
				}
				_quickUseItems[msg.index] = allItemByInstanceId;
				RemoveBagItemByInstance(msg.instanceId);
				AlertBox.Show(330);
				break;
			case 2:
			{
				int quickUseIndexByInstanceId2 = GetQuickUseIndexByInstanceId(msg.instanceId);
				_quickUseItems[msg.index] = allItemByInstanceId;
				if (quickUseItemByIndex != null)
				{
					_quickUseItems[quickUseIndexByInstanceId2] = quickUseItemByIndex;
				}
				else
				{
					_quickUseItems.Remove(quickUseIndexByInstanceId2);
				}
				break;
			}
			}
		}
		Utils.TriggerEvent(BagEvent.SetQuickDelegate);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
	}

	private void SBreakItemHandle(SBreakItem msg)
	{
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId);
		allItemByInstanceId.number = msg.number;
		BagItem bagItem = new BagItem();
		bagItem.instanceId = msg.newItemInstanceId;
		bagItem.itemId = allItemByInstanceId.itemId;
		bagItem.number = msg.newNumber;
		_bagItems.Add(bagItem);
		_bagItems.Sort(Sort);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
		Utils.TriggerEvent(BagEvent.SplitItemSucess);
	}

	private void SItemDurationChangedHandle(SItemDurationChanged msg)
	{
		int form = -1;
		BagItem realyAllBagItemByInstanceId = GetRealyAllBagItemByInstanceId(msg.instanceId, out form);
		if (realyAllBagItemByInstanceId != null)
		{
			realyAllBagItemByInstanceId.duration = msg.duration;
			switch (form)
			{
			case 1:
				Utils.TriggerEvent(BagEvent.RefreshBag);
				break;
			case 2:
				Utils.TriggerEvent(BagEvent.RefreshQuickUse);
				break;
			case 3:
				Utils.TriggerEvent(BagEvent.RefreshSkins);
				break;
			case 4:
				Utils.TriggerEvent(BagEvent.RefreshEquips);
				break;
			}
		}
	}

	private void SItemChangedHandle(SItemChanged msg)
	{
		if (_bagItemZero.ContainsKey(msg.instanceId))
		{
			_bagItemZero.Remove(msg.instanceId);
		}
		int form = -1;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(msg.instanceId, out form);
		if (allItemByInstanceId == null)
		{
			allItemByInstanceId = new BagItem();
			allItemByInstanceId.instanceId = msg.instanceId;
			allItemByInstanceId.itemId = msg.itemId;
			allItemByInstanceId.duration = msg.duration;
			allItemByInstanceId.number = msg.num;
			allItemByInstanceId.extraInfo = msg.extraInfo;
			allItemByInstanceId.isBind = msg.isBind;
			_bagItems.Add(allItemByInstanceId);
			if (IsGun(msg.itemId))
			{
				AddGunDic(allItemByInstanceId);
				m_gunDataDic.Add(msg.instanceId, new GunData(msg.instanceId, GetAllItemByInstanceId(msg.instanceId).itemId, GetGunParts(allItemByInstanceId.extraInfo)));
			}
			Utils.TriggerEvent(BagEvent.ItemChange, allItemByInstanceId.itemId, allItemByInstanceId.number);
			AutoQuickUse(allItemByInstanceId.itemId, allItemByInstanceId.instanceId);
		}
		else
		{
			if (msg.num == 0)
			{
				_bagItemZero.Add(allItemByInstanceId.instanceId, allItemByInstanceId.itemId);
				RemoveItemInAllByInstanceId(allItemByInstanceId.instanceId);
				if (IsGun(msg.itemId))
				{
					RemoveGunDic(allItemByInstanceId);
					m_gunDataDic.Remove(msg.instanceId);
				}
			}
			else
			{
				allItemByInstanceId.number = msg.num;
			}
			allItemByInstanceId.isBind = msg.isBind;
			Utils.TriggerEvent(BagEvent.ItemChange, allItemByInstanceId.itemId, msg.num - allItemByInstanceId.number);
			allItemByInstanceId.duration = msg.duration;
		}
		_bagItems.Sort(Sort);
		Utils.TriggerEvent(BagEvent.RefreshBag);
		Utils.TriggerEvent(BagEvent.RefreshQuickUse);
		Utils.TriggerEvent(BagEvent.RefreshGunParts);
	}

	public bool AutoQuickUse(BagItem bagItem)
	{
		return AutoQuickUse(bagItem.itemId, bagItem.instanceId);
	}

	public bool AutoQuickUse(int itemId, int instanceId)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg == null)
		{
			return false;
		}
		if (!itemCfg.toQuickUse)
		{
			return false;
		}
		if (BagContainsInstanceId(instanceId))
		{
			BagItem bagItemByInstanceId = GetBagItemByInstanceId(instanceId);
			for (int i = 0; i < 7; i++)
			{
				if (QuickUseItems.ContainsKey(i))
				{
					BagItem quickUseItemByIndex = GetQuickUseItemByIndex(i);
					if (quickUseItemByIndex.itemId == itemCfg.id && itemCfg.isPileAble && itemCfg.maxPileNum >= 2)
					{
						int num = itemCfg.maxPileNum - quickUseItemByIndex.number;
						num = ((num <= bagItemByInstanceId.number) ? num : bagItemByInstanceId.number);
						if (num > 0)
						{
							PutTogether(quickUseItemByIndex.instanceId, instanceId, num, true);
							return true;
						}
					}
					continue;
				}
				SetQuickUseItem(instanceId, i);
				return true;
			}
		}
		return false;
	}

	private void ItemChangeAlert(int itemId, int num)
	{
		if (num > 0)
		{
			AlertBox.Show(Utils.GetString(ItemCfg.Get(itemId).name) + "  +" + num);
		}
		else if (num < 0)
		{
			AlertBox.Show(Utils.GetString(ItemCfg.Get(itemId).name) + "  " + num);
		}
	}

	private void SItemsHandle(SItems msg)
	{
		_bagItems = msg.bagItems;
		_bagItems.Sort(Sort);
		_quickUseItems = msg.quickUseItems;
		_bagCapacity = msg.capacity;
		_skinItems = msg.skinItems;
		_equipItems = msg.equipItems;
		IsShowEquip = msg.showEquip;
		InitGunDic();
	}

	public void BreakItem(int instanceId, int num)
	{
		if (_bagCapacity <= _bagItems.Count)
		{
			AlertBox.Show(8);
			return;
		}
		_cBreakItem.instanceId = instanceId;
		_cBreakItem.number = num;
		Client2Gs.Ins.Send(_cBreakItem);
	}

	public void SetQuickUseItem(int instanceId, int index)
	{
		if (IsCanQuickUse(instanceId))
		{
			_cSetQuickUseItem.instanceId = instanceId;
			_cSetQuickUseItem.index = index;
			Client2Gs.Ins.Send(_cSetQuickUseItem);
		}
	}

	public void DiscardItem(int instanceId)
	{
		_cDiscardItem.instanceId = instanceId;
		Client2Gs.Ins.Send(_cDiscardItem);
	}

	public void UseItem(int instanceId, int num)
	{
		_cUseItem.instanceId = instanceId;
		_cUseItem.number = num;
		Client2Gs.Ins.Send(_cUseItem);
	}

	public void SetShowEquip(bool isShowEquip)
	{
		_cSetShowEquip.showEquip = isShowEquip;
		Client2Gs.Ins.Send(_cSetShowEquip);
	}

	public void PutGunPart(int gunInstanceId, int partInstanceId)
	{
		_cPutGunPart.gunInstanceId = gunInstanceId;
		_cPutGunPart.partInstanceId = partInstanceId;
		Client2Gs.Ins.Send(_cPutGunPart);
	}

	public void RemoveGunPart(long gunInstanceId, int itemId, bool toBag)
	{
		_cRemoveGunPart.gunInstanceId = (int)gunInstanceId;
		_cRemoveGunPart.itemId = itemId;
		Client2Gs.Ins.Send(_cRemoveGunPart);
	}

	public void LoadGunBullet(int gunInstanceId, int bulletInstanceId, int number)
	{
		GunData gunDate = GetGunDate(gunInstanceId);
		Debug.LogError("maxbullet:" + gunDate.BulletMax);
		int num = gunDate.BulletMax - gunDate.CurBulletNum;
		int number2 = ((number <= num) ? number : num);
		_cLoadGunBullet.gunInstanceId = gunInstanceId;
		_cLoadGunBullet.bulletInstanceId = bulletInstanceId;
		_cLoadGunBullet.number = number2;
		Client2Gs.Ins.Send(_cLoadGunBullet);
	}

	public void OnClickUseItem(int instanceId, int num = 1)
	{
		BagItem allItemByInstanceId = GetAllItemByInstanceId(instanceId);
		ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
		if (!itemCfg.canUse)
		{
			return;
		}
		if (itemCfg.type == 79)
		{
			DrawingCfg drawingCfg = DrawingCfg.Get(allItemByInstanceId.itemId);
			if (drawingCfg.levelLimit > Singleton<RoleMgr>.Ins.info.level)
			{
				AlertBox.Show(87);
			}
			else if (Singleton<ScProduceMgr>.Ins.IsLearned(allItemByInstanceId.itemId))
			{
				AlertBox.Show(129);
			}
			else
			{
				Singleton<ScProduceMgr>.Ins.SendLearnDrawingMsg(allItemByInstanceId.itemId);
			}
		}
		else if (itemCfg.type == 32 || itemCfg.type == 124)
		{
			Utils.TriggerEvent(BattlePackEvent.UseItemByInstanceItemId, allItemByInstanceId.itemId, instanceId);
		}
		else if (itemCfg.type == 123)
		{
			ViewMgr.Ins.ShowView<BagUsePanel>(instanceId, false);
		}
		else
		{
			UseItem(instanceId, num);
		}
	}

	public void AutoLoadGunBullet(int gunInstanceId, int bulletItemId)
	{
		GunParts value;
		if (!GunDic.TryGetValue(gunInstanceId, out value))
		{
			return;
		}
		GunData gunDate = GetGunDate(gunInstanceId);
		int num = 0;
		num = ((value.bulletId != bulletItemId) ? gunDate.BulletMax : (gunDate.BulletMax - gunDate.CurBulletNum));
		GunCfg gunCfg = GunCfg.Get(gunDate.Id);
		List<BagItem> allItemByItemId = GetAllItemByItemId(bulletItemId);
		for (int i = 0; i < allItemByItemId.Count; i++)
		{
			BagItem bagItem = allItemByItemId[i];
			if (bagItem.number >= num)
			{
				GetAllItemByInstanceId(bagItem.instanceId);
				_cLoadGunBullet.gunInstanceId = gunInstanceId;
				_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
				_cLoadGunBullet.number = num;
				Client2Gs.Ins.Send(_cLoadGunBullet);
				break;
			}
			GetAllItemByInstanceId(bagItem.instanceId);
			num -= bagItem.number;
			_cLoadGunBullet.gunInstanceId = gunInstanceId;
			_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
			_cLoadGunBullet.number = bagItem.number;
			Client2Gs.Ins.Send(_cLoadGunBullet);
		}
	}

	public void AutoLoadGunBullet()
	{
		GunParts value;
		if (!GunDic.TryGetValue(_handInstanceId, out value))
		{
			return;
		}
		GunData gunDate = GetGunDate(_handInstanceId);
		int num = gunDate.BulletMax - gunDate.CurBulletNum;
		GunCfg gunCfg = GunCfg.Get(gunDate.Id);
		int value2;
		if (_gunLastBullet.TryGetValue(_handInstanceId, out value2))
		{
		}
		if (gunCfg.bulletIds.Contains(value2))
		{
			int itemNum = GetItemNum(value2);
			if (itemNum <= 0)
			{
				for (int i = 0; i < gunCfg.bulletIds.Count; i++)
				{
					int itemNum2 = GetItemNum(gunCfg.bulletIds[i]);
					if (itemNum2 > 0)
					{
						value2 = gunCfg.bulletIds[i];
						break;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < gunCfg.bulletIds.Count; j++)
			{
				int itemNum3 = GetItemNum(gunCfg.bulletIds[j]);
				if (itemNum3 > 0)
				{
					value2 = gunCfg.bulletIds[j];
					break;
				}
			}
		}
		List<BagItem> allItemByItemId = GetAllItemByItemId(value2);
		for (int k = 0; k < allItemByItemId.Count; k++)
		{
			BagItem bagItem = allItemByItemId[k];
			if (bagItem.number >= num)
			{
				_cLoadGunBullet.gunInstanceId = _handInstanceId;
				_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
				_cLoadGunBullet.number = num;
				Client2Gs.Ins.Send(_cLoadGunBullet);
				break;
			}
			num -= bagItem.number;
			_cLoadGunBullet.gunInstanceId = _handInstanceId;
			_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
			_cLoadGunBullet.number = bagItem.number;
			Client2Gs.Ins.Send(_cLoadGunBullet);
		}
	}

	public void LoadGunBullet(int bulletItemId, int number = 1)
	{
		GunParts value;
		if (!GunDic.TryGetValue(_handInstanceId, out value))
		{
			return;
		}
		GunData gunDate = GetGunDate(_handInstanceId);
		int num = gunDate.BulletMax - gunDate.CurBulletNum;
		if (number > num)
		{
			return;
		}
		List<BagItem> allItemByItemId = GetAllItemByItemId(bulletItemId);
		for (int i = 0; i < allItemByItemId.Count; i++)
		{
			BagItem bagItem = allItemByItemId[i];
			if (bagItem.number >= number)
			{
				_cLoadGunBullet.gunInstanceId = _handInstanceId;
				_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
				_cLoadGunBullet.number = number;
				Client2Gs.Ins.Send(_cLoadGunBullet);
				break;
			}
			number -= bagItem.number;
			_cLoadGunBullet.gunInstanceId = _handInstanceId;
			_cLoadGunBullet.bulletInstanceId = bagItem.instanceId;
			_cLoadGunBullet.number = number;
			Client2Gs.Ins.Send(_cLoadGunBullet);
		}
	}

	public int GetLastGunBulletIndex(long gunInstanceId)
	{
		GunData gunDate = GetGunDate(gunInstanceId);
		if (gunDate == null)
		{
			return -1;
		}
		int value;
		if (!_gunLastBullet.TryGetValue(gunInstanceId, out value))
		{
			return -1;
		}
		GunCfg gunCfg = GunCfg.Get(gunDate.Id);
		for (int i = 0; i < gunCfg.bulletIds.Count; i++)
		{
			if (gunCfg.bulletIds[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetCurrentGunBulletItemId(int gunInstanceId)
	{
		if (_gunDic.ContainsKey(gunInstanceId))
		{
			return _gunDic[gunInstanceId].bulletId;
		}
		return -1;
	}

	public int GetLastGunBulletItemId(long gunInstanceId)
	{
		GunData gunDate = GetGunDate(gunInstanceId);
		if (gunDate == null)
		{
			return -1;
		}
		int value;
		if (!_gunLastBullet.TryGetValue(gunInstanceId, out value))
		{
			return -1;
		}
		GunCfg gunCfg = GunCfg.Get(gunDate.Id);
		for (int i = 0; i < gunCfg.bulletIds.Count; i++)
		{
			if (gunCfg.bulletIds[i] == value)
			{
				return value;
			}
		}
		return -1;
	}

	public int ChangeBulletItemId(int gunInstanceId)
	{
		GunData gunDate = GetGunDate(gunInstanceId);
		if (gunDate == null)
		{
			return -1;
		}
		int value = -1;
		_gunLastBullet.TryGetValue(gunInstanceId, out value);
		if (value > 0)
		{
			int itemNum = GetItemNum(value);
			if (itemNum > 0)
			{
				return value;
			}
		}
		GunCfg gunCfg = GunCfg.Get(gunDate.Id);
		for (int i = 0; i < gunCfg.bulletIds.Count; i++)
		{
			int itemNum2 = GetItemNum(gunCfg.bulletIds[i]);
			if (itemNum2 > 0)
			{
				return gunCfg.bulletIds[i];
			}
		}
		return value;
	}

	public void RemoveGunBullet(int gunInstanceId, int bulletInstanceId, int number)
	{
		_cRemoveGunBullet.gunInstanceId = gunInstanceId;
		_cRemoveGunBullet.number = number;
		Client2Gs.Ins.Send(_cRemoveGunBullet);
	}

	public void RemoveGunBullet(BagItem gunBagItem)
	{
		if (_gunDic.ContainsKey(gunBagItem.instanceId))
		{
			GunParts gunParts = _gunDic[gunBagItem.instanceId];
			RemoveGunBullet(gunBagItem.instanceId, 9999, gunParts.bulletNumber);
		}
	}

	public void PutSkin(int instanceId)
	{
		BagItem bagItemByInstanceId = GetBagItemByInstanceId(instanceId);
		ItemCfg itemCfg = ItemCfg.Get(bagItemByInstanceId.itemId);
		if (!IsSexFix(itemCfg))
		{
			AlertBox.Show(286);
		}
		else if (IsSkinType(itemCfg.type))
		{
			_cPutSkin.instanceId = instanceId;
			Client2Gs.Ins.Send(_cPutSkin);
		}
	}

	public void RemoveSkin(int instanceId)
	{
		_cRemoveSkin.instanceId = instanceId;
		Client2Gs.Ins.Send(_cRemoveSkin);
	}

	public void PutEquip(int instanceId)
	{
		BagItem bagItemByInstanceId = GetBagItemByInstanceId(instanceId);
		if (bagItemByInstanceId.duration <= 0)
		{
			AlertBox.Show(118);
			return;
		}
		ItemCfg itemCfg = ItemCfg.Get(bagItemByInstanceId.itemId);
		if (!IsSexFix(itemCfg))
		{
			AlertBox.Show(286);
		}
		else if (IsEquipType(itemCfg.type))
		{
			_cPutEquip.instanceId = instanceId;
			Client2Gs.Ins.Send(_cPutEquip);
		}
	}

	public bool IsSexFix(ItemCfg itemCfg)
	{
		if (itemCfg.SexLimit == 2)
		{
			return true;
		}
		if (Singleton<RoleMgr>.Ins.info.sex && itemCfg.SexLimit == 1)
		{
			return true;
		}
		if (!Singleton<RoleMgr>.Ins.info.sex && itemCfg.SexLimit == 0)
		{
			return true;
		}
		return false;
	}

	public void RemoveEquip(int instanceId)
	{
		_cRemoveEquip.instanceId = instanceId;
		Client2Gs.Ins.Send(_cRemoveEquip);
	}

	public void PutTogether(int toInstanceId, int fromInstanceId, int num, bool fromBag)
	{
		_cPutTogether.toInstanceId = toInstanceId;
		_cPutTogether.fromInstanceId = fromInstanceId;
		_cPutTogether.number = num;
		_cPutTogether.fromBag = fromBag;
		Client2Gs.Ins.Send(_cPutTogether);
	}

	public void PutToHand(int instanceId)
	{
		if (instanceId > 0)
		{
			BagItem allItemByInstanceId = GetAllItemByInstanceId(instanceId);
			if (allItemByInstanceId != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
				if (itemCfg.type != 25 && allItemByInstanceId.duration <= 0)
				{
					AlertBox.Show(279);
					return;
				}
			}
		}
		_cPutToHand.instanceId = instanceId;
		Client2Gs.Ins.Send(_cPutToHand);
	}

	public GunParts GetGunParts(Octets octets)
	{
		GunParts gunParts = new GunParts();
		if (octets.Size > 0)
		{
			gunParts.unmarshal(octets.copy());
		}
		return gunParts;
	}

	public GunParts GetGunPartsByInstanceId(long instanceId)
	{
		BagItem bagItemByInstanceId = GetBagItemByInstanceId(instanceId);
		return GetGunParts(bagItemByInstanceId.extraInfo);
	}

	public BagItem GetBagItemByInstanceId(long instanceId)
	{
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].instanceId == instanceId)
			{
				return _bagItems[i];
			}
		}
		return null;
	}

	public BagItem GetBagItemQuickAngBagByInstanceId(long insId)
	{
		BagItem bagItemByInstanceId = GetBagItemByInstanceId(insId);
		if (bagItemByInstanceId != null)
		{
			return bagItemByInstanceId;
		}
		return GetQuickUseItemByInstanceId(insId);
	}

	public bool BagContainsInstanceId(long instanceId)
	{
		return GetBagItemByInstanceId(instanceId) != null;
	}

	public BagItem GetBagItemByInstanceId(long instanceId, out int bagIndex)
	{
		bagIndex = -1;
		for (int i = 0; i < _bagItems.Count; i++)
		{
			bagIndex = i;
			if (_bagItems[i].instanceId == instanceId)
			{
				return _bagItems[i];
			}
		}
		return null;
	}

	public BagItem GetAllItemByInstanceId(int instanceId)
	{
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].instanceId == instanceId)
			{
				return _bagItems[i];
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				return quickUseItem.Value;
			}
		}
		return null;
	}

	public BagItem GetRealyAllBagItemByInstanceId(long instanceId, out int form)
	{
		form = -1;
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].instanceId == instanceId)
			{
				form = 1;
				return _bagItems[i];
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				form = 2;
				return quickUseItem.Value;
			}
		}
		for (int j = 0; j < _skinItems.Count; j++)
		{
			if (_skinItems[j].instanceId == instanceId)
			{
				form = 3;
				return _skinItems[j];
			}
		}
		for (int k = 0; k < _equipItems.Count; k++)
		{
			if (_equipItems[k].instanceId == instanceId)
			{
				form = 4;
				return _equipItems[k];
			}
		}
		return null;
	}

	public BagItem GetAllItemByInstanceId(long instanceId, out int form)
	{
		form = -1;
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].instanceId == instanceId)
			{
				form = 1;
				return _bagItems[i];
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				form = 2;
				return quickUseItem.Value;
			}
		}
		return null;
	}

	public List<BagItem> GetAllItemByType(int type)
	{
		List<BagItem> list = new List<BagItem>();
		for (int i = 0; i < _bagItems.Count; i++)
		{
			ItemCfg itemCfg = ItemCfg.Get(_bagItems[i].itemId);
			if (itemCfg.type == type)
			{
				list.Add(_bagItems[i]);
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(quickUseItem.Value.itemId);
			if (itemCfg2.type == type)
			{
				list.Add(quickUseItem.Value);
			}
		}
		return list;
	}

	public List<BagItem> GetAllItemByItemId(int itemId, bool isAlsoGetBinding = true)
	{
		List<BagItem> list = new List<BagItem>();
		for (int i = 0; i < _bagItems.Count; i++)
		{
			ItemCfg itemCfg = ItemCfg.Get(_bagItems[i].itemId);
			if (itemCfg != null && MeetCondition(_bagItems[i], itemId, isAlsoGetBinding))
			{
				list.Add(_bagItems[i]);
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(quickUseItem.Value.itemId);
			if (itemCfg2 != null && MeetCondition(quickUseItem.Value, itemId, isAlsoGetBinding))
			{
				list.Add(quickUseItem.Value);
			}
		}
		return list;
	}

	public List<int> GetAllItemIdsByType(int type)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < _bagItems.Count; i++)
		{
			ItemCfg itemCfg = ItemCfg.Get(_bagItems[i].itemId);
			if (itemCfg == null)
			{
				Debug.LogError(_bagItems[i].itemId + "****************");
			}
			if (itemCfg.type == type)
			{
				list.Add(_bagItems[i].itemId);
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(quickUseItem.Value.itemId);
			if (itemCfg2.type == type)
			{
				list.Add(quickUseItem.Value.itemId);
			}
		}
		return list;
	}

	public int GetAllItemNumByType(int type)
	{
		int num = 0;
		for (int i = 0; i < _bagItems.Count; i++)
		{
			ItemCfg itemCfg = ItemCfg.Get(_bagItems[i].itemId);
			if (itemCfg.type == type)
			{
				num += _bagItems[i].number;
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(quickUseItem.Value.itemId);
			if (itemCfg2.type == type)
			{
				num += quickUseItem.Value.number;
			}
		}
		return num;
	}

	public void RemoveItemInAllByInstanceId(int instanceId)
	{
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].instanceId == instanceId)
			{
				_bagItems.RemoveAt(i);
				return;
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				_quickUseItems.Remove(quickUseItem.Key);
				break;
			}
		}
	}

	public void RemoveBagItemByInstance(int instanceId)
	{
		int bagIndex = -1;
		BagItem bagItemByInstanceId = GetBagItemByInstanceId(instanceId, out bagIndex);
		if (bagIndex > -1)
		{
			_bagItems.RemoveAt(bagIndex);
		}
	}

	public GunData GetCurGunDate()
	{
		if (m_gunDataDic.ContainsKey(HandInstanceId))
		{
			return m_gunDataDic[HandInstanceId];
		}
		Debug.LogError("not find HandInstanceId " + HandInstanceId);
		return null;
	}

	public void RemoveQuickUseItemByIndex(int index)
	{
		if (_quickUseItems.ContainsKey(index))
		{
			_quickUseItems.Remove(index);
		}
	}

	public void RemoveQuickUseItemByInstance(int instanceId)
	{
		int num = -1;
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				num = quickUseItem.Key;
				break;
			}
		}
		if (num >= 0)
		{
			_quickUseItems.Remove(num);
		}
	}

	public void AddQuickUseItem(int index, BagItem bagItem)
	{
		_quickUseItems[index] = bagItem;
	}

	public BagItem GetQuickUseItemByInstanceId(long instanceId)
	{
		int num = -1;
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				num = quickUseItem.Key;
				break;
			}
		}
		if (num > -1)
		{
			return _quickUseItems[num];
		}
		return null;
	}

	public BagItem GetQuickUseItemByIndex(int index)
	{
		if (_quickUseItems.ContainsKey(index))
		{
			return _quickUseItems[index];
		}
		return null;
	}

	public int GetQuickUseIndexByInstanceId(long instanceId)
	{
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (quickUseItem.Value.instanceId == instanceId)
			{
				return quickUseItem.Key;
			}
		}
		return -1;
	}

	public int GetQuickUseInstanceIdByIndex(int index)
	{
		if (_quickUseItems.ContainsKey(index))
		{
			return _quickUseItems[index].instanceId;
		}
		return -1;
	}

	public bool QuickUseIsContainsInstanceId(long instanceId)
	{
		return GetQuickUseIndexByInstanceId(instanceId) >= 0;
	}

	public void ClearNew()
	{
		_newBagItems.Clear();
	}

	public bool IsNew(int instanceId)
	{
		return _newBagItems.Contains(instanceId);
	}

	public int GetItemNum(int itemId, bool isAlsoGetBinding)
	{
		int num = 0;
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (MeetCondition(_bagItems[i], itemId, isAlsoGetBinding))
			{
				num += _bagItems[i].number;
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			if (MeetCondition(quickUseItem.Value, itemId, isAlsoGetBinding))
			{
				num += quickUseItem.Value.number;
			}
		}
		return num;
	}

	private bool MeetCondition(BagItem bagItemData, int needItemId, bool isAlsoGetBinding)
	{
		return bagItemData.itemId == needItemId && (!bagItemData.isBind || isAlsoGetBinding);
	}

	public int GetItemNum(int itemId)
	{
		return GetItemNum(itemId, true);
	}

	private void InitGunDic()
	{
		_gunDic.Clear();
		m_gunDataDic.Clear();
		for (int i = 0; i < _bagItems.Count; i++)
		{
			ItemCfg itemCfg = ItemCfg.Get(_bagItems[i].itemId);
			if (itemCfg != null && itemCfg.type == 13)
			{
				_gunDic.Add(_bagItems[i].instanceId, GetGunParts(_bagItems[i].extraInfo));
				_gunLastBullet[_bagItems[i].instanceId] = _gunDic[_bagItems[i].instanceId].bulletId;
			}
		}
		foreach (KeyValuePair<int, BagItem> quickUseItem in _quickUseItems)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(quickUseItem.Value.itemId);
			if (itemCfg2 == null)
			{
				Debug.LogError("quickUseItem.Value.itemId:" + quickUseItem.Value.itemId + ",is null");
				return;
			}
			if (itemCfg2.type == 13)
			{
				_gunDic.Add(quickUseItem.Value.instanceId, GetGunParts(quickUseItem.Value.extraInfo));
				_gunLastBullet[quickUseItem.Value.instanceId] = _gunDic[quickUseItem.Value.instanceId].bulletId;
			}
		}
		foreach (KeyValuePair<int, GunParts> item in _gunDic)
		{
			m_gunDataDic.Add(item.Key, new GunData(item.Key, GetAllItemByInstanceId(item.Key).itemId, item.Value));
		}
	}

	private bool IsGun(int itemId)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg == null)
		{
			return false;
		}
		return itemCfg.type == 13;
	}

	private void AddGunDic(BagItem bagItem)
	{
		_gunDic[bagItem.instanceId] = GetGunParts(bagItem.extraInfo);
	}

	private void RemoveGunDic(BagItem bagItem)
	{
		if (_gunDic.ContainsKey(bagItem.instanceId))
		{
			_gunDic.Remove(bagItem.instanceId);
		}
	}

	public static int PutIdOnlyOneType(HashSet<int> set, int itemId)
	{
		int result = -1;
		int type = ItemCfg.Get(itemId).type;
		foreach (int item in set)
		{
			if (ItemCfg.Get(item).type == type)
			{
				result = item;
				set.Remove(item);
				break;
			}
		}
		set.Add(itemId);
		return result;
	}

	public BagItem PutPartInGunAndOutOld(HashSet<BagItem> bagItems, BagItem part)
	{
		BagItem result = null;
		int type = ItemCfg.Get(part.itemId).type;
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				result = bagItem;
				bagItems.Remove(bagItem);
				break;
			}
		}
		bagItems.Add(part);
		return result;
	}

	public void RemovePart(HashSet<BagItem> bagItems, BagItem part)
	{
		BagItem bagItem = new BagItem();
		int type = ItemCfg.Get(part.itemId).type;
		foreach (BagItem bagItem2 in bagItems)
		{
			if (ItemCfg.Get(bagItem2.itemId).type == type)
			{
				bagItem = bagItem2;
				bagItems.Remove(bagItem2);
				break;
			}
		}
		bagItems.Add(part);
	}

	public int GetPartItemIdByInstanceId(int gunInstanceId, int partInstanceId)
	{
		foreach (BagItem part in _gunDic[gunInstanceId].parts)
		{
			if (part.instanceId == partInstanceId)
			{
				return part.itemId;
			}
		}
		return -1;
	}

	public int GetPartItemIdByInstanceIdAndPartType(int gunInstanceId, int partType)
	{
		foreach (BagItem part in _gunDic[gunInstanceId].parts)
		{
			ItemCfg itemCfg = ItemCfg.Get(part.itemId);
			if (itemCfg.type == partType)
			{
				return part.itemId;
			}
		}
		return -1;
	}

	public int GetPartItemIdByType(HashSet<BagItem> bagItems, int type)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				return bagItem.itemId;
			}
		}
		return -1;
	}

	public BagItem GetPartBagItemIdByType(HashSet<BagItem> bagItems, int type)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				return bagItem;
			}
		}
		return null;
	}

	public bool GunHasPartCanUse(int gunItemid, HashSet<BagItem> bagItems)
	{
		if (GunHasPartCanUse(gunItemid, bagItems, 21))
		{
			return true;
		}
		if (GunHasPartCanUse(gunItemid, bagItems, 20))
		{
			return true;
		}
		if (GunHasPartCanUse(gunItemid, bagItems, 14))
		{
			return true;
		}
		if (GunHasPartCanUse(gunItemid, bagItems, 18))
		{
			return true;
		}
		return false;
	}

	public bool GunHasPartCanUse(int gunItemid, HashSet<BagItem> bagItems, int type)
	{
		if (GetPartBagItemIdByType(bagItems, type) != null)
		{
			return false;
		}
		GunCfg gunCfg = GunCfg.Get(gunItemid);
		switch (type)
		{
		case 21:
			foreach (int qiangbaPart in gunCfg.qiangbaParts)
			{
				if (GetItemNum(qiangbaPart) > 0)
				{
					return true;
				}
			}
			break;
		case 20:
			foreach (int clipPart in gunCfg.clipParts)
			{
				if (GetItemNum(clipPart) > 0)
				{
					return true;
				}
			}
			break;
		case 14:
			foreach (int aimPart in gunCfg.aimParts)
			{
				if (GetItemNum(aimPart) > 0)
				{
					return true;
				}
			}
			break;
		case 18:
			foreach (int muzzlePart in gunCfg.muzzleParts)
			{
				if (GetItemNum(muzzlePart) > 0)
				{
					return true;
				}
			}
			break;
		}
		return false;
	}

	public int GetBulletNumBagItem(int gunInstanceId)
	{
		if (GunDic.ContainsKey(gunInstanceId))
		{
			return GunDic[gunInstanceId].bulletNumber;
		}
		return 0;
	}

	public BagItem PutNewBagItemAndOutOld(List<BagItem> bagItems, BagItem part)
	{
		BagItem result = null;
		int type = ItemCfg.Get(part.itemId).type;
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				result = bagItem;
				bagItems.Remove(bagItem);
				break;
			}
		}
		bagItems.Add(part);
		return result;
	}

	public BagItem PutNewBagItemAndOutOldEquip(List<BagItem> bagItems, BagItem part)
	{
		BagItem result = null;
		EquipCfg equipCfg = EquipCfg.Get(part.itemId);
		if (equipCfg != null)
		{
			int equipType = equipCfg.equipType;
			foreach (BagItem bagItem in bagItems)
			{
				EquipCfg equipCfg2 = EquipCfg.Get(bagItem.itemId);
				if (equipCfg2 == null || equipCfg2.equipType != equipType)
				{
					continue;
				}
				result = bagItem;
				bagItems.Remove(bagItem);
				break;
			}
			bagItems.Add(part);
		}
		return result;
	}

	public BagItem PutNewBagItemAndOutOldSkin(List<BagItem> bagItems, BagItem part)
	{
		BagItem result = null;
		SkinCfg skinCfg = SkinCfg.Get(part.itemId);
		if (skinCfg != null)
		{
			int skinType = skinCfg.skinType;
			foreach (BagItem bagItem in bagItems)
			{
				SkinCfg skinCfg2 = SkinCfg.Get(bagItem.itemId);
				if (skinCfg2 == null || skinCfg2.skinType != skinType)
				{
					continue;
				}
				result = bagItem;
				bagItems.Remove(bagItem);
				break;
			}
			bagItems.Add(part);
		}
		return result;
	}

	public BagItem GetBagItemFromList(List<BagItem> bagItems, int instanceId)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (bagItem.instanceId == instanceId)
			{
				return bagItem;
			}
		}
		return null;
	}

	public void RemoveBagItemFromList(List<BagItem> bagItems, int instanceId)
	{
		BagItem bagItem = null;
		foreach (BagItem bagItem2 in bagItems)
		{
			if (bagItem2.instanceId == instanceId)
			{
				bagItem = bagItem2;
				break;
			}
		}
		if (bagItem != null)
		{
			bagItems.Remove(bagItem);
		}
	}

	public BagItem GetBagItemByItemType(IEnumerable<BagItem> bagItems, int type)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				return bagItem;
			}
		}
		return null;
	}

	public BagItem GetBagItemByEquipType(int equipType)
	{
		foreach (BagItem equipItem in EquipItems)
		{
			EquipCfg equipCfg = EquipCfg.Get(equipItem.itemId);
			if (equipCfg == null || equipCfg.equipType != equipType)
			{
				continue;
			}
			return equipItem;
		}
		return null;
	}

	public BagItem GetBagItemBySkinType(int skinType)
	{
		foreach (BagItem skinItem in SkinItems)
		{
			SkinCfg skinCfg = SkinCfg.Get(skinItem.itemId);
			if (skinCfg == null || skinCfg.skinType != skinType)
			{
				continue;
			}
			return skinItem;
		}
		return null;
	}

	public BagItem GetSkinBagItemByItemType(int itemType)
	{
		return GetBagItemByItemType(_skinItems, itemType);
	}

	public BagItem GetEquipBagItemByItemType(int itemType)
	{
		return GetBagItemByItemType(_equipItems, itemType);
	}

	public BagItem GetSkinBagItemByInstanceId(int instanceId)
	{
		return GetBagItemFromList(_skinItems, instanceId);
	}

	public BagItem GetEquipBagItemByInstanceId(int instanceId)
	{
		return GetBagItemFromList(_equipItems, instanceId);
	}

	public void RemoveSkinBagItemByInstanceId(int instanceId)
	{
		RemoveBagItemFromList(_skinItems, instanceId);
	}

	public void RemoveEquipBagItemByInstanceId(int instanceId)
	{
		RemoveBagItemFromList(_equipItems, instanceId);
	}

	public bool IsContainPart(int instanceId)
	{
		foreach (KeyValuePair<int, GunParts> item in _gunDic)
		{
			foreach (BagItem part in item.Value.parts)
			{
				if (part.instanceId == instanceId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UnLoadPart(int instanceId)
	{
		foreach (KeyValuePair<int, GunParts> item in _gunDic)
		{
			foreach (BagItem part in item.Value.parts)
			{
				if (part.instanceId == instanceId)
				{
					if (IsHasCapacity(part.itemId))
					{
						RemoveGunPart(item.Key, part.itemId, true);
					}
					else
					{
						AlertBox.Show(26);
					}
					return;
				}
			}
		}
	}

	private int Sort(BagItem bagItem1, BagItem bagItem2)
	{
		ItemCfg itemCfg = ItemCfg.Get(bagItem1.itemId);
		ItemCfg itemCfg2 = ItemCfg.Get(bagItem2.itemId);
		if (bagItem1.itemId != bagItem2.itemId)
		{
			return bagItem1.itemId - bagItem2.itemId;
		}
		if (bagItem1.number != bagItem2.number)
		{
			return bagItem1.number - bagItem2.number;
		}
		return bagItem1.instanceId - bagItem2.instanceId;
	}

	private bool IsCanQuickUse(int instanceId)
	{
		BagItem allItemByInstanceId = GetAllItemByInstanceId(instanceId);
		if (allItemByInstanceId == null || allItemByInstanceId.itemId <= 0)
		{
			return false;
		}
		ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
		if (itemCfg == null)
		{
			return false;
		}
		return itemCfg.toQuickUse;
	}

	public bool IsBasicSkinType(int skinType, out int itemId)
	{
		foreach (int roleBasicSkin in Singleton<RoleMgr>.Ins.RoleBasicSkins)
		{
			SkinCfg skinCfg = SkinCfg.Get(roleBasicSkin);
			if (skinCfg == null || skinCfg.skinType != skinType)
			{
				continue;
			}
			itemId = roleBasicSkin;
			return true;
		}
		itemId = -1;
		return false;
	}

	public bool IsBasicEquipType(int equipType, out int itemId)
	{
		foreach (int roleBasicEquip in Singleton<RoleMgr>.Ins.RoleBasicEquips)
		{
			EquipCfg equipCfg = EquipCfg.Get(roleBasicEquip);
			if (equipCfg == null || equipCfg.equipType != equipType)
			{
				continue;
			}
			itemId = roleBasicEquip;
			return true;
		}
		itemId = -1;
		return false;
	}

	public bool IsGunContainPart(int instanceId, int itemId)
	{
		BagItem allItemByInstanceId = GetAllItemByInstanceId(instanceId);
		if (allItemByInstanceId == null)
		{
			return false;
		}
		GunCfg gunCfg = GunCfg.Get(allItemByInstanceId.itemId);
		if (gunCfg == null)
		{
			return false;
		}
		if (gunCfg.aimParts.Contains(itemId))
		{
			return true;
		}
		if (gunCfg.muzzleParts.Contains(itemId))
		{
			return true;
		}
		if (gunCfg.propParts.Contains(itemId))
		{
			return true;
		}
		if (gunCfg.aimParts.Contains(itemId))
		{
			return true;
		}
		if (gunCfg.clipParts.Contains(itemId))
		{
			return true;
		}
		if (gunCfg.qiangbaParts.Contains(itemId))
		{
			return true;
		}
		return false;
	}

	public bool IsGunContainBullet(int instanceId, int itemId)
	{
		BagItem allItemByInstanceId = GetAllItemByInstanceId(instanceId);
		if (allItemByInstanceId == null)
		{
			return false;
		}
		GunCfg gunCfg = GunCfg.Get(allItemByInstanceId.itemId);
		if (gunCfg == null)
		{
			return false;
		}
		return gunCfg.bulletIds[0] == itemId;
	}

	public int GetSkinIndex(int itemId)
	{
		SkinCfg skinCfg = SkinCfg.Get(itemId);
		if (skinCfg != null)
		{
			return skinCfg.cellIndex;
		}
		return -1;
	}

	public int GetEquipIndex(int itemId)
	{
		EquipCfg equipCfg = EquipCfg.Get(itemId);
		if (equipCfg != null)
		{
			return equipCfg.cellIndex;
		}
		return -1;
	}

	public List<BagItem> GetBagItemsByItemType(IEnumerable<BagItem> bagItems, int type)
	{
		List<BagItem> list = new List<BagItem>();
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				list.Add(bagItem);
			}
		}
		return list;
	}

	public List<int> GetBagItemIdsByItemType(IEnumerable<BagItem> bagItems, int type)
	{
		List<int> list = new List<int>();
		foreach (BagItem bagItem in bagItems)
		{
			if (ItemCfg.Get(bagItem.itemId).type == type)
			{
				list.Add(bagItem.itemId);
			}
		}
		return list;
	}

	public List<int> GetBuildIds()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < BuildTypes.Length; i++)
		{
			list.AddRange(GetAllItemIdsByType(BuildTypes[i]));
		}
		return list.Distinct().ToList();
	}

	public bool IsHasCapacity(int itemId)
	{
		return GetRemainCapacity(itemId, 1) > 0;
	}

	public bool IsHasCapacity(int itemId, int num)
	{
		return GetRemainCapacity(itemId, num) >= num;
	}

	public int GetRemainCapacity(int itemId, int num)
	{
		int num2 = 0;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].itemId == itemId && itemCfg.isPileAble && _bagItems[i].number < itemCfg.maxPileNum)
			{
				num2 += itemCfg.maxPileNum - _bagItems[i].number;
			}
		}
		if (_bagItems.Count < _bagCapacity)
		{
			num2 += (_bagCapacity - _bagItems.Count) * itemCfg.maxPileNum;
		}
		return (num <= num2) ? num : num2;
	}

	public int GetRemainCapacityForRonglu(int itemId, int num)
	{
		int num2 = 0;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		for (int i = 0; i < _bagItems.Count; i++)
		{
			if (_bagItems[i].itemId == itemId && itemCfg.isPileAble && _bagItems[i].number < itemCfg.maxPileNum)
			{
				num2 += itemCfg.maxPileNum - _bagItems[i].number;
			}
		}
		return (num <= num2) ? num : num2;
	}

	public bool IsRemainCapacity()
	{
		return _bagItems.Count < _bagCapacity;
	}

	public GunData GetGunDate(long insId)
	{
		if (m_gunDataDic.ContainsKey(insId))
		{
			return m_gunDataDic[insId];
		}
		Debug.LogError("not find GunDate " + insId);
		return null;
	}

	public void ReduceDurability()
	{
		int form;
		BagItem allItemByInstanceId = GetAllItemByInstanceId(_handInstanceId, out form);
		if (allItemByInstanceId == null)
		{
			return;
		}
		if (allItemByInstanceId.duration > 0)
		{
			allItemByInstanceId.duration--;
		}
		if (_gunDic.ContainsKey(_handInstanceId))
		{
			GunParts gunParts = _gunDic[_handInstanceId];
			GunData gunData = m_gunDataDic[_handInstanceId];
			foreach (BagItem part in gunParts.parts)
			{
				if (part.duration > 0)
				{
					part.duration--;
				}
				if (part.duration <= 0)
				{
					gunData.RefreshAllFactors();
				}
			}
		}
		if (allItemByInstanceId.duration <= 0)
		{
		}
		Utils.TriggerEvent(BagEvent.DurabilityChange, allItemByInstanceId.itemId, allItemByInstanceId.instanceId);
	}

	public bool IsCanRescue()
	{
		int itemNum = GetItemNum(8);
		if (itemNum > 0)
		{
			return true;
		}
		AlertBox.Show(284);
		return false;
	}

	public static bool IsEquipType(int itemType)
	{
		return itemType == 119;
	}

	public static bool IsSkinType(int itemType)
	{
		return itemType == 9;
	}

	public static bool IsEquip(int itemId)
	{
		return true;
	}

	public static bool IsSkin(int itemId)
	{
		return true;
	}

	public void RefreshAllFactorsByGunInstanceId(int instanceId)
	{
		if (m_gunDataDic.ContainsKey(instanceId))
		{
			GunData gunData = m_gunDataDic[instanceId];
			gunData.RefreshAllFactors();
			Utils.TriggerEvent(BagEvent.RefreshGunAttrDelegate, instanceId);
		}
	}
}
