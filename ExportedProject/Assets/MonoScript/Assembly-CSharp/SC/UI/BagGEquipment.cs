using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BagGEquipment : MonoBehaviour
	{
		public enum EquipType
		{
			None = 0,
			Equip = 1,
			Skin = 2
		}

		[CompilerGenerated]
		private sealed class _003CFillSkinItemDataByBagItem_003Ec__AnonStorey0
		{
			internal BagItem bagItem;

			internal BagGEquipment _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this._bagPage.SetItemDescData(bagItem, 4);
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this._bagPage.DragBagItem = bagItem;
				_0024this._bagPage.OnBeginDrag(_0024this._bagPage.DragBagItem, 4);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this._bagPage.OnDragFromEquipmentZone();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillEquipItemDataByBagItem_003Ec__AnonStorey1
		{
			internal ItemCfg itemCfg;

			internal BagItem bagItem;

			internal BagGEquipment _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (itemCfg.type == 119)
				{
					_0024this._bagPage.SetEquipDescItemDescData(bagItem, 4);
				}
				else
				{
					_0024this._bagPage.SetItemDescData(bagItem, 4);
				}
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this._bagPage.DragBagItem = bagItem;
				_0024this._bagPage.OnBeginDrag(_0024this._bagPage.DragBagItem, 4);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this._bagPage.OnDragFromEquipmentZone();
			}
		}

		private List<BagItem> _bagItems;

		private Dictionary<int, BagItem> _quickUseItems;

		private BagPage _bagPage;

		private RenderTexture _renderTexture;

		private RenderTexture heroRender;

		private Camera _camera;

		public EquipType CurrentEquipType;

		private Vector3 _defaultAngle = Vector3.zero;

		private HeroSkin _heroSkin;

		private HeroSkin _heroSkinEquip;

		private List<int> _nowItemIds = new List<int>();

		private List<int> _nowEquipItemIds = new List<int>();

		public GameObject btn_equipment;

		public GameObject btn_equipment_r;

		public GameObject m_camera;

		public GameObject m_equipment_zone;

		public List<GEquipmentsRItem> m_equipmentslist = new List<GEquipmentsRItem>();

		public GameObject[] m_equipments;

		public GameObject m_equipmentsObj;

		public List<GEquipmentsRItem> m_equipments_rlist = new List<GEquipmentsRItem>();

		public GameObject[] m_equipments_r;

		public GameObject m_equipments_rObj;

		public GameObject m_hero_tex;

		public GameObject m_model_equip_root;

		public GameObject m_model_role;

		public GameObject m_model_root;

		public GameObject m_model_skin_root;

		public GameObject m_role_bg;

		public GameObject m_show_equip;

		public GameObject m_show_skin;

		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_off_0;

		public Text txt_off_0Text;

		public GameObject txt_on;

		public Text txt_onText;

		public GameObject txt_on_0;

		public Text txt_on_0Text;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		public void Init(BagPage bagPage)
		{
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			_bagPage = bagPage;
			ClickListener.Get(btn_equipment, string.Empty).onClick = OnClickEuipment;
			ClickListener.Get(btn_equipment_r, string.Empty).onClick = OnClickEuipmentR;
			UIEventListener.Get(m_hero_tex, string.Empty).onDrag = OnDragHero;
			UIEventListener.Get(m_equipment_zone, string.Empty).onEnter = _003CInit_003Em__0;
			ClickListener clickListener = ClickListener.Get(m_show_equip, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CInit_003Em__1;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener clickListener2 = ClickListener.Get(m_show_skin, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003CInit_003Em__2;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
			m_role_bg.SetActiveBetter(false);
		}

		public void OnShow()
		{
			BagEvent.RefreshSkins = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshSkins, new Utils.VoidDelegate(RefreshSkinsDelegent));
			BagEvent.RefreshEquips = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshEquips, new Utils.VoidDelegate(RefreshEquipsDelegent));
			BagEvent.PutSkinByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutSkinByItemId, new Utils.IntDelegate(PutSkinByItemId));
			BagEvent.PutEquipByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutEquipByItemId, new Utils.IntDelegate(PutEquipByItemId));
			BagEvent.RemoveSkinByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveSkinByItemId, new Utils.IntDelegate(RemoveSkinByItemId));
			BagEvent.RemoveEquipByItemId = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveEquipByItemId, new Utils.IntDelegate(RemoveEquipByItemId));
			BagEvent.IsShowEquipBoolDelegate = (Utils.BoolDelegate)Delegate.Combine(BagEvent.IsShowEquipBoolDelegate, new Utils.BoolDelegate(IsShowEquipBoolDelegate));
			BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Combine(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeHandInstanceIdDelegate));
			BagEvent.RemoveGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.RemoveGunPart, new Utils.Int2Delegate(RemoveGunPartHandle));
			BagEvent.AddGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.AddGunPart, new Utils.Int2Delegate(AddGunPartHandle));
			InitSkinsData();
			ShowHeroSkinMode();
			InitEquipsData();
			ShowHeroSkinEquipMode();
			SetData(null);
			RefreshShowEquip();
		}

		private void AddGunPartHandle(int instanceId, int partId)
		{
			if (Singleton<BagMgr>.Ins.HandInstanceId == instanceId)
			{
				if (_heroSkin != null)
				{
					_heroSkin.LoadPartModel(partId);
				}
				if (_heroSkinEquip != null)
				{
					_heroSkinEquip.LoadPartModel(partId);
				}
			}
		}

		private void RemoveGunPartHandle(int instanceId, int partId)
		{
			if (Singleton<BagMgr>.Ins.HandInstanceId == instanceId)
			{
				if (_heroSkin != null)
				{
					_heroSkin.UnLoadPartGameObject(partId);
				}
				if (_heroSkinEquip != null)
				{
					_heroSkinEquip.UnLoadPartGameObject(partId);
				}
			}
		}

		private void ChangeHandInstanceIdDelegate(long arg)
		{
			int currentHandWeaponId = GetCurrentHandWeaponId();
			HashSet<int> currentHandWeaponParts = GetCurrentHandWeaponParts();
			if (_heroSkin != null)
			{
				_heroSkin.SetCurrentWeapon(currentHandWeaponId, currentHandWeaponParts);
			}
			if (_heroSkinEquip != null)
			{
				_heroSkinEquip.SetCurrentWeapon(currentHandWeaponId, currentHandWeaponParts);
			}
			if (_heroSkin != null)
			{
				_heroSkin.PlayRoleAni(GetAnimStr(), string.Empty);
			}
			if (_heroSkinEquip != null)
			{
				_heroSkinEquip.PlayRoleAni(GetAnimStr(), string.Empty);
			}
		}

		private void IsShowEquipBoolDelegate(bool isShowEquip)
		{
			RefreshShowEquip();
		}

		private void RefreshShowEquip()
		{
			m_show_equip.GetComponent<Toggle>().isOn = Singleton<BagMgr>.Ins.IsShowEquip;
			m_show_skin.GetComponent<Toggle>().isOn = !Singleton<BagMgr>.Ins.IsShowEquip;
		}

		private void OnEnable()
		{
			if (_heroSkin != null)
			{
				_heroSkin.PlayRoleAni(GetAnimStr(), string.Empty);
			}
			if (_heroSkinEquip != null)
			{
				_heroSkinEquip.PlayRoleAni(GetAnimStr(), string.Empty);
			}
		}

		private string GetAnimStr()
		{
			int handInstanceId = Singleton<BagMgr>.Ins.HandInstanceId;
			if (handInstanceId < 0)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(handInstanceId);
			if (allItemByInstanceId == null)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
			if (itemCfg == null)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			if (itemCfg.type != 13)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			GunCfg gunCfg = GunCfg.Get(allItemByInstanceId.itemId);
			if (gunCfg == null)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			if (gunCfg.gunType == 5 || gunCfg.gunType == 3 || gunCfg.gunType == 6)
			{
				return "MainPanel.jujiqiang_zhan_stand02";
			}
			if (gunCfg.gunType == 1)
			{
				return Singleton<RoleMgr>.Ins.RoleAnimStr;
			}
			if (gunCfg.gunType == 4)
			{
				return "MainPanel.sandanqiang_zhan_stand02";
			}
			return "MainPanel.chongfengqiang_zhan_stand02";
		}

		private void RemoveEquipByItemId(int itemId)
		{
			EquipCfg equipCfg = EquipCfg.Get(itemId);
			if (equipCfg != null)
			{
				int itemId2 = -1;
				if (Singleton<BagMgr>.Ins.IsBasicEquipType(equipCfg.equipType, out itemId2))
				{
					_heroSkinEquip.ChangeSkin(itemId2);
				}
				else
				{
					_heroSkinEquip.RemoveRoleSkin(itemId);
				}
			}
		}

		private void PutEquipByItemId(int itemId)
		{
			_heroSkinEquip.ChangeSkin(itemId);
		}

		private void RefreshEquipsDelegent()
		{
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(true);
			SetEquipData();
			RadioButton.ChooseBtn(btn_equipment);
			ChangeEquipmentPage(EquipType.Equip);
		}

		private void RemoveSkinByItemId(int itemId)
		{
			SkinCfg skinCfg = SkinCfg.Get(itemId);
			int itemId2 = -1;
			if (Singleton<BagMgr>.Ins.IsBasicSkinType(skinCfg.skinType, out itemId2))
			{
				_heroSkin.ChangeSkin(itemId2);
			}
			else
			{
				_heroSkin.RemoveRoleSkin(itemId);
			}
		}

		private void PutSkinByItemId(int itemId)
		{
			_heroSkin.ChangeSkin(itemId);
		}

		private void RefreshSkinsDelegent()
		{
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(true);
			SetSkinData();
			RadioButton.ChooseBtn(btn_equipment_r);
			ChangeEquipmentPage(EquipType.Skin);
		}

		public void OnHide()
		{
			BagEvent.RefreshSkins = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshSkins, new Utils.VoidDelegate(RefreshSkinsDelegent));
			BagEvent.RefreshEquips = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshEquips, new Utils.VoidDelegate(RefreshEquipsDelegent));
			BagEvent.PutSkinByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.PutSkinByItemId, new Utils.IntDelegate(PutSkinByItemId));
			BagEvent.PutEquipByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.PutEquipByItemId, new Utils.IntDelegate(PutEquipByItemId));
			BagEvent.RemoveSkinByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.RemoveSkinByItemId, new Utils.IntDelegate(RemoveSkinByItemId));
			BagEvent.RemoveEquipByItemId = (Utils.IntDelegate)Delegate.Remove(BagEvent.RemoveEquipByItemId, new Utils.IntDelegate(RemoveEquipByItemId));
			BagEvent.IsShowEquipBoolDelegate = (Utils.BoolDelegate)Delegate.Remove(BagEvent.IsShowEquipBoolDelegate, new Utils.BoolDelegate(IsShowEquipBoolDelegate));
			BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Remove(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeHandInstanceIdDelegate));
			_heroSkin.Clear();
			_heroSkinEquip.Clear();
		}

		public void SetData(BagItem bagItem)
		{
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(true);
			RadioButton.ChooseBtn(btn_equipment);
			SetEquipment();
			ChangeEquipmentPage(EquipType.Equip);
		}

		public void SetData(EquipType equipType)
		{
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_weapon.gameObject.SetActiveBetter(false);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(true);
			GameObject btn = ((equipType != EquipType.Equip) ? btn_equipment_r : btn_equipment);
			RadioButton.ChooseBtn(btn);
			ChangeEquipmentPage(equipType);
			SetEquipment();
		}

		public void SetEquipment()
		{
			SetEquipData();
			SetSkinData();
		}

		private void SetSkinData()
		{
			for (int i = 0; i < m_equipments_r.Length; i++)
			{
				CleatSkinItemData(m_equipments_rlist[i], i);
			}
			foreach (BagItem skinItem in Singleton<BagMgr>.Ins.SkinItems)
			{
				FillSkinItemDataByBagItem(skinItem);
			}
		}

		private void CleatSkinItemData(GEquipmentsRItem bagGGunPartItem, int index)
		{
			UIEventListener.Get(bagGGunPartItem.gameObject, string.Empty).onEnter = _003CCleatSkinItemData_003Em__3;
			bagGGunPartItem.m_durability.SetActiveBetter(false);
			bagGGunPartItem.m_equipment_icon.SetActiveBetter(false);
			bagGGunPartItem.m_no_equipment.SetActiveBetter(true);
			ClickListener.Get(bagGGunPartItem.gameObject, string.Empty).onClick = null;
			DragListener.Get(bagGGunPartItem.gameObject).onBeginDrag = null;
			DragListener.Get(bagGGunPartItem.gameObject).onDrag = null;
			DragListener.Get(bagGGunPartItem.gameObject).onEndDrag = null;
		}

		private void FillSkinItemDataByBagItem(BagItem bagItem)
		{
			_003CFillSkinItemDataByBagItem_003Ec__AnonStorey0 _003CFillSkinItemDataByBagItem_003Ec__AnonStorey = new _003CFillSkinItemDataByBagItem_003Ec__AnonStorey0();
			_003CFillSkinItemDataByBagItem_003Ec__AnonStorey.bagItem = bagItem;
			_003CFillSkinItemDataByBagItem_003Ec__AnonStorey._0024this = this;
			SkinCfg skinCfg = SkinCfg.Get(_003CFillSkinItemDataByBagItem_003Ec__AnonStorey.bagItem.itemId);
			if (skinCfg != null)
			{
				GEquipmentsRItem gEquipmentsRItem = m_equipments_rlist[skinCfg.cellIndex];
				ItemCfg itemCfg = ItemCfg.Get(_003CFillSkinItemDataByBagItem_003Ec__AnonStorey.bagItem.itemId);
				gEquipmentsRItem.m_durability.SetActiveBetter(true);
				gEquipmentsRItem.m_equipment_icon.SetActiveBetter(true);
				gEquipmentsRItem.m_no_equipment.SetActiveBetter(false);
				View.SetItemSprite(gEquipmentsRItem.m_equipment_icon, itemCfg.icon);
				if (itemCfg.durability > 0)
				{
					gEquipmentsRItem.m_durability.SetActiveBetter(true);
					View.SetSlider(gEquipmentsRItem.m_durability, (float)_003CFillSkinItemDataByBagItem_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
				}
				else
				{
					gEquipmentsRItem.m_durability.SetActiveBetter(false);
				}
				ClickListener.Get(gEquipmentsRItem.gameObject, string.Empty).onClick = _003CFillSkinItemDataByBagItem_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(gEquipmentsRItem.gameObject).onBeginDrag = _003CFillSkinItemDataByBagItem_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(gEquipmentsRItem.gameObject).onDrag = _bagPage.OnDragMask;
				DragListener.Get(gEquipmentsRItem.gameObject).onEndDrag = _003CFillSkinItemDataByBagItem_003Ec__AnonStorey._003C_003Em__2;
			}
		}

		private void SetEquipData()
		{
			for (int i = 0; i < m_equipments.Length; i++)
			{
				ClearEquipItemData(m_equipmentslist[i], i);
			}
			foreach (BagItem equipItem in Singleton<BagMgr>.Ins.EquipItems)
			{
				FillEquipItemDataByBagItem(equipItem);
			}
		}

		private void ClearEquipItemData(GEquipmentsRItem bagGGunPartItem, int index)
		{
			UIEventListener.Get(bagGGunPartItem.gameObject, string.Empty).onEnter = _003CClearEquipItemData_003Em__4;
			bagGGunPartItem.m_durability.SetActiveBetter(false);
			bagGGunPartItem.m_equipment_icon.SetActiveBetter(false);
			bagGGunPartItem.m_no_equipment.SetActiveBetter(true);
			ClickListener.Get(bagGGunPartItem.gameObject, string.Empty).onClick = null;
			DragListener.Get(bagGGunPartItem.gameObject).onBeginDrag = null;
			DragListener.Get(bagGGunPartItem.gameObject).onDrag = null;
			DragListener.Get(bagGGunPartItem.gameObject).onEndDrag = null;
		}

		private void FillEquipItemDataByBagItem(BagItem bagItem)
		{
			_003CFillEquipItemDataByBagItem_003Ec__AnonStorey1 _003CFillEquipItemDataByBagItem_003Ec__AnonStorey = new _003CFillEquipItemDataByBagItem_003Ec__AnonStorey1();
			_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.bagItem = bagItem;
			_003CFillEquipItemDataByBagItem_003Ec__AnonStorey._0024this = this;
			EquipCfg equipCfg = EquipCfg.Get(_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.bagItem.itemId);
			if (equipCfg != null)
			{
				GEquipmentsRItem gEquipmentsRItem = m_equipmentslist[equipCfg.cellIndex];
				_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.itemCfg = ItemCfg.Get(_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.bagItem.itemId);
				gEquipmentsRItem.m_durability.SetActiveBetter(true);
				gEquipmentsRItem.m_equipment_icon.SetActiveBetter(true);
				gEquipmentsRItem.m_no_equipment.SetActiveBetter(false);
				View.SetItemSprite(gEquipmentsRItem.m_equipment_icon, _003CFillEquipItemDataByBagItem_003Ec__AnonStorey.itemCfg.icon);
				if (_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.itemCfg.durability > 0)
				{
					gEquipmentsRItem.m_durability.SetActiveBetter(true);
					View.SetSlider(gEquipmentsRItem.m_durability, (float)_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.bagItem.duration * 1f / (float)_003CFillEquipItemDataByBagItem_003Ec__AnonStorey.itemCfg.durability);
				}
				else
				{
					gEquipmentsRItem.m_durability.SetActiveBetter(false);
				}
				ClickListener.Get(gEquipmentsRItem.gameObject, string.Empty).onClick = _003CFillEquipItemDataByBagItem_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(gEquipmentsRItem.gameObject).onBeginDrag = _003CFillEquipItemDataByBagItem_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(gEquipmentsRItem.gameObject).onDrag = _bagPage.OnDragMask;
				DragListener.Get(gEquipmentsRItem.gameObject).onEndDrag = _003CFillEquipItemDataByBagItem_003Ec__AnonStorey._003C_003Em__2;
			}
		}

		public void OnClickEuipment(GameObject og)
		{
			ChangeEquipmentPage(EquipType.Equip);
		}

		public void OnClickEuipmentR(GameObject og)
		{
			ChangeEquipmentPage(EquipType.Skin);
		}

		private void ChangeEquipmentPage(EquipType type)
		{
			if (CurrentEquipType != type)
			{
				CurrentEquipType = type;
				m_equipmentsObj.SetActiveBetter(CurrentEquipType == EquipType.Equip);
				m_equipments_rObj.SetActiveBetter(CurrentEquipType == EquipType.Skin);
				m_model_equip_root.SetActiveBetter(CurrentEquipType == EquipType.Equip);
				m_model_skin_root.SetActiveBetter(CurrentEquipType == EquipType.Skin);
				if (CurrentEquipType == EquipType.Skin && _heroSkin != null)
				{
					m_model_skin_root.transform.localEulerAngles = _defaultAngle;
					_heroSkin.PlayRoleAni(GetAnimStr(), string.Empty);
				}
				if (CurrentEquipType == EquipType.Equip && _heroSkinEquip != null)
				{
					m_model_equip_root.transform.localEulerAngles = _defaultAngle;
					_heroSkinEquip.PlayRoleAni(GetAnimStr(), string.Empty);
				}
			}
		}

		public int GetCurrentHandWeaponId()
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(Singleton<BagMgr>.Ins.HandInstanceId);
			if (allItemByInstanceId == null)
			{
				return -1;
			}
			ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
			if (itemCfg.type == 13 || itemCfg.type == 17 || itemCfg.type == 25 || itemCfg.type == 82)
			{
				return allItemByInstanceId.itemId;
			}
			return -1;
		}

		public HashSet<int> GetCurrentHandWeaponParts()
		{
			if (Singleton<BagMgr>.Ins.HandInstanceId > 0 && Singleton<BagMgr>.Ins.GunDic.ContainsKey(Singleton<BagMgr>.Ins.HandInstanceId))
			{
				GunParts gunParts = Singleton<BagMgr>.Ins.GunDic[Singleton<BagMgr>.Ins.HandInstanceId];
				HashSet<int> hashSet = new HashSet<int>();
				{
					foreach (BagItem part in gunParts.parts)
					{
						hashSet.Add(part.itemId);
					}
					return hashSet;
				}
			}
			return null;
		}

		public void ShowHeroSkinMode()
		{
			_heroSkin = new HeroSkin(m_model_skin_root, Singleton<RoleMgr>.Ins.info.sex);
			int currentHandWeaponId = GetCurrentHandWeaponId();
			HashSet<int> currentHandWeaponParts = GetCurrentHandWeaponParts();
			_heroSkin.LoadBasicHero(_nowItemIds, Singleton<RoleMgr>.Ins.RoleBasicSkins, GetAnimStr(), currentHandWeaponId, currentHandWeaponParts);
		}

		public void ShowHeroSkinEquipMode()
		{
			_heroSkinEquip = new HeroSkin(m_model_equip_root, Singleton<RoleMgr>.Ins.info.sex);
			int currentHandWeaponId = GetCurrentHandWeaponId();
			HashSet<int> currentHandWeaponParts = GetCurrentHandWeaponParts();
			_heroSkinEquip.LoadBasicHero(_nowEquipItemIds, Singleton<RoleMgr>.Ins.RoleBasicEquips, GetAnimStr(), currentHandWeaponId, currentHandWeaponParts);
		}

		public void InitSkinsData()
		{
			_nowItemIds.Clear();
			foreach (BagItem skinItem in Singleton<BagMgr>.Ins.SkinItems)
			{
				_nowItemIds.Add(skinItem.itemId);
			}
			foreach (int roleSkin in Singleton<RoleMgr>.Ins.RoleSkins)
			{
				SkinCfg skinCfg = SkinCfg.Get(roleSkin);
				if (skinCfg != null)
				{
					BagItem bagItemByEquipType = Singleton<BagMgr>.Ins.GetBagItemByEquipType(skinCfg.skinType);
					if (bagItemByEquipType == null)
					{
						_nowItemIds.Add(roleSkin);
					}
				}
			}
		}

		public void InitEquipsData()
		{
			_nowEquipItemIds.Clear();
			foreach (BagItem equipItem in Singleton<BagMgr>.Ins.EquipItems)
			{
				_nowEquipItemIds.Add(equipItem.itemId);
			}
			foreach (int roleBasicEquip in Singleton<RoleMgr>.Ins.RoleBasicEquips)
			{
				EquipCfg equipCfg = EquipCfg.Get(roleBasicEquip);
				if (equipCfg != null)
				{
					BagItem bagItemByEquipType = Singleton<BagMgr>.Ins.GetBagItemByEquipType(equipCfg.equipType);
					if (bagItemByEquipType == null)
					{
						_nowEquipItemIds.Add(roleBasicEquip);
					}
				}
			}
		}

		private void OnDragHero(GameObject go)
		{
			GameObject gameObject = ((CurrentEquipType != EquipType.Equip) ? m_model_skin_root : m_model_equip_root);
			if (UIEventListener.pointEventData.delta.x < -1f)
			{
				gameObject.transform.Rotate(Vector3.up * 600f * Time.deltaTime, Space.World);
			}
			else if (UIEventListener.pointEventData.delta.x > 1f)
			{
				gameObject.transform.Rotate(Vector3.up * -600f * Time.deltaTime, Space.World);
			}
		}

		public void DisrobeSkin(GameObject go)
		{
		}

		private void Awake()
		{
			btn_equipment = base.transform.Find("GameObject/btn_equipment").gameObject;
			btn_equipment_r = base.transform.Find("GameObject/btn_equipment_r").gameObject;
			m_camera = base.transform.Find("m_model_root/m_model_role/m_camera").gameObject;
			m_equipment_zone = base.transform.Find("m_equipment_zone").gameObject;
			m_equipments = base.transform.Find("m_equipments").gameObject.GetComponent<UIGameObjectList>().objects;
			m_equipmentsObj = base.transform.Find("m_equipments").gameObject;
			if (m_equipmentslist.Count <= 0)
			{
				for (int i = 0; i < m_equipments.Length; i++)
				{
					m_equipmentslist.Add(View.AddComponentIfNotExist<GEquipmentsRItem>(m_equipments[i].gameObject));
				}
			}
			m_equipments_r = base.transform.Find("m_equipments_r").gameObject.GetComponent<UIGameObjectList>().objects;
			m_equipments_rObj = base.transform.Find("m_equipments_r").gameObject;
			if (m_equipments_rlist.Count <= 0)
			{
				for (int j = 0; j < m_equipments_r.Length; j++)
				{
					m_equipments_rlist.Add(View.AddComponentIfNotExist<GEquipmentsRItem>(m_equipments_r[j].gameObject));
				}
			}
			m_hero_tex = base.transform.Find("m_model_root/m_hero_tex").gameObject;
			m_model_equip_root = base.transform.Find("m_model_root/m_model_role/m_model_equip_root").gameObject;
			m_model_role = base.transform.Find("m_model_root/m_model_role").gameObject;
			m_model_root = base.transform.Find("m_model_root").gameObject;
			m_model_skin_root = base.transform.Find("m_model_root/m_model_role/m_model_skin_root").gameObject;
			m_role_bg = base.transform.Find("m_model_root/m_role_bg").gameObject;
			m_show_equip = base.transform.Find("GameObject (1)/m_show_equip").gameObject;
			m_show_skin = base.transform.Find("GameObject (1)/m_show_skin").gameObject;
			txt_off = base.transform.Find("GameObject/btn_equipment/Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_off_0 = base.transform.Find("GameObject/btn_equipment_r/Off/txt_off").gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on = base.transform.Find("GameObject/btn_equipment/On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_on_0 = base.transform.Find("GameObject/btn_equipment_r/On/txt_on").gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003CInit_003Em__0(GameObject go)
		{
			_bagPage.OnEnter(4);
		}

		[CompilerGenerated]
		private static void _003CInit_003Em__1(GameObject go)
		{
			if (!Singleton<BagMgr>.Ins.IsShowEquip)
			{
				Singleton<BagMgr>.Ins.SetShowEquip(true);
			}
		}

		[CompilerGenerated]
		private static void _003CInit_003Em__2(GameObject go)
		{
			if (Singleton<BagMgr>.Ins.IsShowEquip)
			{
				Singleton<BagMgr>.Ins.SetShowEquip(false);
			}
		}

		[CompilerGenerated]
		private void _003CCleatSkinItemData_003Em__3(GameObject go)
		{
			_bagPage.OnEnter(4);
		}

		[CompilerGenerated]
		private void _003CClearEquipItemData_003Em__4(GameObject go)
		{
			_bagPage.OnEnter(4);
		}
	}
}
