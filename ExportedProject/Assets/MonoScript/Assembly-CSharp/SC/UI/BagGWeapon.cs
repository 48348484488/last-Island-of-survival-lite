using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Share;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BagGWeapon : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003CShowMode_003Ec__AnonStorey0
		{
			internal int itemId;

			internal BagGWeapon _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
				if ((bool)_0024this._currentGunGameObject)
				{
					UnityEngine.Object.DestroyImmediate(_0024this._currentGunGameObject);
				}
				go.transform.SetParent(_0024this.m_model.transform, true);
				go.transform.localScale = Vector3.one;
				go.transform.localPosition = Vector3.zero;
				go.transform.localEulerAngles = Vector3.zero;
				_0024this._currentGunGameObject = go;
				Transform transform = go.transform.Find("ef");
				if ((bool)transform)
				{
					transform.gameObject.SetActive(true);
				}
				if (_0024this._gunLength > 0f)
				{
					GunCfg gunCfg = GunCfg.Get(itemId);
					_0024this._gunLength = ((gunCfg.gunType != 1) ? 4f : 2.1f);
					SkinnedMeshRenderer componentInChildren = go.transform.GetComponentInChildren<SkinnedMeshRenderer>();
					float num = _0024this._gunLength / componentInChildren.bounds.size.x;
					go.transform.localScale = new Vector3(num, num, num);
					go.transform.localPosition -= go.transform.InverseTransformPoint(componentInChildren.bounds.center) * num;
				}
				else
				{
					go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetBulletData_003Ec__AnonStorey1
		{
			internal ItemCfg itemCfg;

			internal BagGWeapon _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.OpenCanAdd(24, 9999, _0024this.m_bullet.transform.localPosition);
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this.OpenCanAdd(24, 9999, _0024this.m_bullet.transform.localPosition);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this.OpenCanAdd(itemCfg.type, 9999, _0024this.m_bullet.transform.localPosition);
			}

			internal void _003C_003Em__3(GameObject go)
			{
				_0024this._bagPage.DragBagItem = _0024this._bulletBagItem;
				_0024this._bagPage.OnBeginDrag(_0024this._bagPage.DragBagItem, 3);
			}

			internal void _003C_003Em__4(GameObject go)
			{
				_0024this._bagPage.OnDragFromGunPartZone();
				_0024this._isClick = true;
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillPartItemData_003Ec__AnonStorey2
		{
			internal bool canAddPart;

			internal int itemType;

			internal BagGGunPartItem bagGGunPartItem;

			internal BagItem bagItem;

			internal BagGWeapon _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (canAddPart)
				{
					_0024this.OpenCanAdd(itemType, -1, bagGGunPartItem.gameObject.transform.localPosition);
				}
				else
				{
					AlertBox.Show(333);
				}
			}

			internal void _003C_003Em__1(GameObject go)
			{
				if (canAddPart)
				{
					_0024this.OpenCanAdd(itemType, bagItem.instanceId, bagGGunPartItem.gameObject.transform.localPosition);
				}
				else
				{
					AlertBox.Show(333);
				}
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this._bagPage.DragBagItem = bagItem;
				_0024this._bagPage.OnBeginDrag(_0024this._bagPage.DragBagItem, 3);
			}

			internal void _003C_003Em__3(GameObject go)
			{
				_0024this._bagPage.OnDragFromGunPartZone();
				_0024this._isClick = true;
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillCanAddBagItemData_003Ec__AnonStorey3
		{
			internal BagItem bagItem;

			internal BagGWeapon _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this._openItemType == 24)
				{
					if (_0024this._gun.bulletNumber > 0)
					{
						Singleton<BagMgr>.Ins.RemoveGunBullet(GunBagItem.instanceId, 9999, _0024this._gun.bulletNumber);
					}
				}
				else
				{
					Singleton<BagMgr>.Ins.UnLoadPart(_0024this._selectInstanceId);
				}
				_0024this.CloseCanAdd();
			}

			internal void _003C_003Em__1(GameObject g)
			{
				if (_0024this._openItemType == 24)
				{
					Singleton<BagMgr>.Ins.AutoLoadGunBullet(GunBagItem.instanceId, bagItem.itemId);
				}
				else if (bagItem.duration > 0)
				{
					Singleton<BagMgr>.Ins.PutGunPart(GunBagItem.instanceId, bagItem.instanceId);
				}
				else
				{
					AlertBox.Show(118);
				}
			}
		}

		private List<BagItem> _bagItems;

		private BagPage _bagPage;

		public static BagItem GunBagItem;

		private HashSet<BagItem> _gunParts;

		private GunParts _gun;

		public const int BulletInstanceId = 9999;

		private Camera _camera;

		private RenderTexture _renderTexture;

		private int _currentId;

		private Color _hasColor = new Color(0.8980392f, 0.9254902f, 4f / 15f);

		private Color _noColor = Color.white;

		private Vector3 _defaultAngle = Vector3.zero;

		private GunSkin _gunSkin;

		private Octets _gunPartOc = new Octets();

		private float _handGunLength = 3.5f;

		private float _gunLength = 4f;

		private readonly Vector3 _initEulerAngle = Vector3.zero;

		private GameObject _currentGunGameObject;

		private BagItem _bulletBagItem = new BagItem();

		public bool _isClick = true;

		public List<int> DrawOriginalIds = new List<int>();

		public List<int> DrawNecessaryIds = new List<int>();

		public List<int> DrawAssistIds = new List<int>();

		private List<BagItem> _partBagItems = new List<BagItem>();

		private UIScrollPanel _canAddScrollPanel;

		private int _openItemType;

		private int _selectInstanceId;

		public GameObject btn_back_desc;

		public GameObject btn_combination;

		public GameObject btn_destroy;

		public GameObject btn_discard;

		public GameObject btn_disrobe;

		public GameObject btn_equip;

		public GameObject btn_out_shortcut_bar;

		public GameObject btn_sell;

		public GameObject btn_split;

		public GameObject btn_to_shortcut_bar;

		public GameObject btn_tohand_equip;

		public GameObject btn_use;

		public GameObject m_bullet;

		public GameObject m_bullet_icon;

		public GameObject m_camera;

		public GameObject m_can_add;

		public GWeaponCanAddPart m_cell;

		public GameObject m_durability;

		public GameObject m_gun_partandbullet;

		public GameObject m_gun_partandbullet_root;

		public GameObject m_hero_tex;

		public GameObject m_model;

		public GameObject m_model_role;

		public GameObject m_model_root;

		public List<BagGGunPartItem> m_partslist = new List<BagGGunPartItem>();

		public GameObject[] m_parts;

		public GameObject m_partsObj;

		public GameObject m_role_bg;

		public GameObject m_weapon_zone;

		public GameObject scp_add_part;

		public GameObject txt_bullet_max_num;

		public Text txt_bullet_max_numText;

		public GameObject txt_bullet_name;

		public Text txt_bullet_nameText;

		public GameObject txt_bullet_num;

		public Text txt_bullet_numText;

		public GameObject txt_change_bullet_speed;

		public Text txt_change_bullet_speedText;

		public GameObject txt_damage;

		public Text txt_damageText;

		public GameObject txt_desc_desc;

		public Text txt_desc_descText;

		public GameObject txt_durability;

		public Text txt_durabilityText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_range;

		public Text txt_rangeText;

		public GameObject txt_shot_speed;

		public Text txt_shot_speedText;

		public GameObject txt_stability;

		public Text txt_stabilityText;

		public object context;

		public void Init(BagPage bagPage)
		{
			_gunSkin = new GunSkin(m_model);
			_defaultAngle = m_model.transform.eulerAngles;
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			_bagPage = bagPage;
			_canAddScrollPanel = scp_add_part.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_back_desc, string.Empty).onClick = _bagPage.OnClickBackDesc;
			ClickListener.Get(btn_sell, string.Empty).onClick = _bagPage.OnClickSell;
			ClickListener.Get(btn_discard, string.Empty).onClick = _bagPage.OnClickDiscard;
			ClickListener.Get(btn_split, string.Empty).onClick = _bagPage.OnClickSplit;
			ClickListener.Get(btn_combination, string.Empty).onClick = _bagPage.OnClickCombination;
			ClickListener.Get(btn_to_shortcut_bar, string.Empty).onClick = _bagPage.OnClickToShortBar;
			ClickListener.Get(btn_out_shortcut_bar, string.Empty).onClick = _bagPage.OnClickOutShortBar;
			ClickListener.Get(btn_use, string.Empty).onClick = _bagPage.OnClickUse;
			ClickListener.Get(btn_equip, string.Empty).onClick = _bagPage.OnClickEquip;
			UIEventListener.Get(m_weapon_zone, string.Empty).onEnter = _003CInit_003Em__0;
			ClickListener.Get(btn_tohand_equip, string.Empty).onClick = _bagPage.OnClickToHand;
			ClickListener.Get(btn_disrobe, string.Empty).onClick = _bagPage.OnClickOutHand;
			ClickListener.Get(btn_destroy, string.Empty).onClick = _bagPage.OnClickDestroy;
			UIEventListener.Get(m_hero_tex, string.Empty).onDrag = OnDragHero;
		}

		private void OnDragHero(GameObject go)
		{
			GameObject model = m_model;
			if (UIEventListener.pointEventData.delta.x < -1f)
			{
				model.transform.Rotate(Vector3.forward * -600f * Time.deltaTime, Space.World);
			}
			else if (UIEventListener.pointEventData.delta.x > 1f)
			{
				model.transform.Rotate(Vector3.forward * 600f * Time.deltaTime, Space.World);
			}
		}

		public void OnShow()
		{
			BagEvent.RefreshGunParts = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshGunParts, new Utils.VoidDelegate(RefreshGunPartsDelegent));
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUse));
			BagEvent.RefreshBullet = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshBullet, new Utils.VoidDelegate(RefreshBullet));
			BagEvent.RefreshGunAttrDelegate = (Utils.IntDelegate)Delegate.Combine(BagEvent.RefreshGunAttrDelegate, new Utils.IntDelegate(RefreshGunAttrDelegate));
			SLoadGunBullet.handler = (SLoadGunBullet.Handler)Delegate.Combine(SLoadGunBullet.handler, new SLoadGunBullet.Handler(SLoadGunBulletHandle));
			SRemoveGunBullet.handler = (SRemoveGunBullet.Handler)Delegate.Combine(SRemoveGunBullet.handler, new SRemoveGunBullet.Handler(SRemoveGunBulletHandle));
			BagEvent.DropGun = (Utils.IntDelegate)Delegate.Combine(BagEvent.DropGun, new Utils.IntDelegate(DropGunHandle));
		}

		private void DropGunHandle(int arg)
		{
			_bagPage.OnClickBackDesc(null);
		}

		private void SRemoveGunBulletHandle(SRemoveGunBullet msg)
		{
			if (msg.gunInstanceId == GunBagItem.instanceId && _gun != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(_gun.bulletId);
				AlertBox.Show(Utils.GetString(329, itemCfg.name));
			}
		}

		private void SLoadGunBulletHandle(SLoadGunBullet msg)
		{
			if (msg.gunInstanceId == GunBagItem.instanceId && _gun != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(_gun.bulletId);
				AlertBox.Show(Utils.GetString(328, itemCfg.name));
			}
		}

		public void ShowMode(int itemId)
		{
			_003CShowMode_003Ec__AnonStorey0 _003CShowMode_003Ec__AnonStorey = new _003CShowMode_003Ec__AnonStorey0();
			_003CShowMode_003Ec__AnonStorey.itemId = itemId;
			_003CShowMode_003Ec__AnonStorey._0024this = this;
			m_role_bg.SetActiveBetter(false);
			if (_currentId != _003CShowMode_003Ec__AnonStorey.itemId)
			{
				_currentId = _003CShowMode_003Ec__AnonStorey.itemId;
				ItemCfg itemCfg = ItemCfg.Get(_currentId);
				ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CShowMode_003Ec__AnonStorey._003C_003Em__0);
			}
		}

		private void RefreshBullet()
		{
			SetBulletData();
			CloseCanAdd();
		}

		private void RefreshQuickUse()
		{
		}

		public void OnHide()
		{
			BagEvent.RefreshGunParts = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshGunParts, new Utils.VoidDelegate(RefreshGunPartsDelegent));
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(RefreshQuickUse));
			BagEvent.RefreshBullet = (Utils.VoidDelegate)Delegate.Remove(BagEvent.RefreshBullet, new Utils.VoidDelegate(RefreshBullet));
			BagEvent.RefreshGunAttrDelegate = (Utils.IntDelegate)Delegate.Remove(BagEvent.RefreshGunAttrDelegate, new Utils.IntDelegate(RefreshGunAttrDelegate));
			SLoadGunBullet.handler = (SLoadGunBullet.Handler)Delegate.Remove(SLoadGunBullet.handler, new SLoadGunBullet.Handler(SLoadGunBulletHandle));
			SRemoveGunBullet.handler = (SRemoveGunBullet.Handler)Delegate.Remove(SRemoveGunBullet.handler, new SRemoveGunBullet.Handler(SRemoveGunBulletHandle));
			BagEvent.DropGun = (Utils.IntDelegate)Delegate.Remove(BagEvent.DropGun, new Utils.IntDelegate(DropGunHandle));
		}

		private void RefreshGunAttrDelegate(int arg)
		{
			SetAttr();
		}

		private void RefreshGunPartsDelegent()
		{
			if (GunBagItem != null)
			{
				SetData(GunBagItem);
				CloseCanAdd();
			}
		}

		public void OnDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		public void SetData(BagItem bagItem)
		{
			m_model.transform.eulerAngles = _defaultAngle;
			CloseCanAdd();
			GunBagItem = bagItem;
			_gun = Singleton<BagMgr>.Ins.GunDic[bagItem.instanceId];
			_gunParts = _gun.parts;
			_bagPage.m_weapon.gameObject.SetActiveBetter(true);
			_bagPage.m_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equip_desc.gameObject.SetActiveBetter(false);
			_bagPage.m_equipment.gameObject.SetActiveBetter(false);
			ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
			GunCfg gunCfg = GunCfg.Get(bagItem.itemId);
			_gunSkin.Clear();
			bool flag = GunBagItem.isBind;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (BagItem gunPart in _gunParts)
			{
				hashSet.Add(gunPart.itemId);
				if (!flag && gunPart.isBind)
				{
					flag = true;
				}
			}
			_gunSkin.LoadWeapon(bagItem.itemId, hashSet, (gunCfg.gunType != 1) ? 3.3f : 1.9f);
			View.SetLabelText(txt_nameText, BagPage.GetNameForBinding(Utils.GetString(9, itemCfg.name, gunCfg.gunTypeName), bagItem.isBind));
			if (itemCfg.durability > 0)
			{
				m_durability.SetActiveBetter(true);
				txt_durability.SetActiveBetter(true);
				View.SetSlider(m_durability, (float)bagItem.duration * 1f / (float)itemCfg.durability);
				View.SetLabelText(txt_durabilityText, Utils.GetString(9, bagItem.duration, itemCfg.durability));
			}
			else
			{
				m_durability.SetActiveBetter(false);
				txt_durability.SetActiveBetter(false);
			}
			View.SetLabelText(txt_desc_descText, itemCfg.desc);
			btn_sell.SetActiveBetter(itemCfg.sellType != 0);
			btn_split.SetActive(bagItem.number > 1);
			btn_combination.SetActiveBetter(false);
			bool flag2 = Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(GunBagItem.instanceId);
			btn_to_shortcut_bar.SetActiveBetter(!flag2);
			btn_out_shortcut_bar.SetActiveBetter(flag2);
			btn_use.SetActiveBetter(itemCfg.canUse);
			btn_equip.SetActiveBetter(Singleton<BagMgr>.Ins.IsSkinOrEquipContainType(itemCfg.type));
			btn_sell.SetActiveBetter(false);
			btn_disrobe.SetActiveBetter(Singleton<BagMgr>.Ins.HandInstanceId == GunBagItem.instanceId);
			btn_tohand_equip.SetActiveBetter(Singleton<BagMgr>.Ins.HandInstanceId != GunBagItem.instanceId && Singleton<BagMgr>.Ins.ToHandTypes.Contains(itemCfg.type));
			btn_discard.SetActiveBetter(!flag);
			btn_destroy.SetActiveBetter(flag);
			SetPartData();
			SetBulletData();
			SetAttr();
		}

		private void SetBulletData()
		{
			_003CSetBulletData_003Ec__AnonStorey1 _003CSetBulletData_003Ec__AnonStorey = new _003CSetBulletData_003Ec__AnonStorey1();
			_003CSetBulletData_003Ec__AnonStorey._0024this = this;
			_gun = Singleton<BagMgr>.Ins.GunDic[GunBagItem.instanceId];
			if (_gun == null)
			{
				return;
			}
			_gunParts = _gun.parts;
			if (GunBagItem == null)
			{
				return;
			}
			GunCfg gunCfg = GunCfg.Get(GunBagItem.itemId);
			if (_gun.bulletId <= 0)
			{
				View.SetLabelText(txt_bullet_nameText, Utils.GetString(250));
				View.SetLabelColor(txt_bullet_nameText, _noColor);
				m_bullet_icon.SetActiveBetter(false);
				View.SetLabelText(txt_bullet_numText, 0);
				ClickListener.Get(m_bullet, string.Empty).onClick = _003CSetBulletData_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(m_bullet_icon).onBeginDrag = null;
				DragListener.Get(m_bullet_icon).onDrag = null;
				DragListener.Get(m_bullet_icon).onEndDrag = null;
				return;
			}
			m_bullet_icon.SetActiveBetter(_gun.bulletNumber > 0);
			_003CSetBulletData_003Ec__AnonStorey.itemCfg = ItemCfg.Get(_gun.bulletId);
			View.SetItemSprite(m_bullet_icon, _003CSetBulletData_003Ec__AnonStorey.itemCfg.icon);
			int itemNum = Singleton<BagMgr>.Ins.GetItemNum(_gun.bulletId);
			View.SetLabelText(txt_bullet_nameText, _003CSetBulletData_003Ec__AnonStorey.itemCfg.name);
			View.SetLabelColor(txt_bullet_nameText, (_gun.bulletNumber <= 0) ? _noColor : _hasColor);
			View.SetLabelText(txt_bullet_numText, Utils.GetString(9, itemNum, _gun.bulletNumber));
			_bulletBagItem.instanceId = 9999;
			_bulletBagItem.itemId = _gun.bulletId;
			_bulletBagItem.number = _gun.bulletNumber;
			if (_bulletBagItem.number <= 0)
			{
				ClickListener.Get(m_bullet, string.Empty).onClick = _003CSetBulletData_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(m_bullet_icon).onBeginDrag = null;
				DragListener.Get(m_bullet_icon).onDrag = null;
				DragListener.Get(m_bullet_icon).onEndDrag = null;
			}
			else
			{
				ClickListener.Get(m_bullet, string.Empty).onClick = _003CSetBulletData_003Ec__AnonStorey._003C_003Em__2;
				DragListener.Get(m_bullet_icon).onBeginDrag = _003CSetBulletData_003Ec__AnonStorey._003C_003Em__3;
				DragListener.Get(m_bullet_icon).onDrag = _bagPage.OnDragMask;
				DragListener.Get(m_bullet_icon).onEndDrag = _003CSetBulletData_003Ec__AnonStorey._003C_003Em__4;
			}
		}

		private void SetAttr()
		{
			BagMgr.GunData gunDate = Singleton<BagMgr>.Ins.GetGunDate(GunBagItem.instanceId);
			if (gunDate != null)
			{
				GunCfg gunCfg = GunCfg.Get(gunDate.Id);
				SetAttr(txt_damageText, 47, gunCfg, gunDate);
				SetAttr(txt_rangeText, 48, gunCfg, gunDate);
				SetAttr(txt_shot_speedText, 50, gunCfg, gunDate);
				SetAttr(txt_bullet_max_numText, 34, gunCfg, gunDate);
				SetAttr(txt_stabilityText, 49, gunCfg, gunDate);
				SetAttr(txt_change_bullet_speedText, 51, gunCfg, gunDate);
			}
		}

		private void SetAttr(Text text, int factorIndex, GunCfg gunCfg, BagMgr.GunData gunData)
		{
			float num = gunData.Factors[factorIndex] - gunCfg.factors[factorIndex];
			if (num > 0f)
			{
				View.SetLabelText(text, Utils.GetString(334, gunCfg.factors[factorIndex], num));
			}
			else if (num < 0f)
			{
				View.SetLabelText(text, Utils.GetString(338, gunCfg.factors[factorIndex], num));
			}
			else
			{
				View.SetLabelText(text, gunCfg.factors[factorIndex]);
			}
		}

		private void SetPartData()
		{
			GunCfg gunCfg = GunCfg.Get(GunBagItem.itemId);
			for (int i = 0; i < m_parts.Length; i++)
			{
				int num = 0;
				switch (i)
				{
				case 3:
					num = gunCfg.muzzleParts.Count;
					break;
				case 4:
					num = gunCfg.propParts.Count;
					break;
				case 2:
					num = gunCfg.aimParts.Count;
					break;
				case 1:
					num = gunCfg.clipParts.Count;
					break;
				case 0:
					num = gunCfg.qiangbaParts.Count;
					break;
				}
				m_partslist[i].m_has.SetActiveBetter(num > 0);
				m_partslist[i].m_no_has.SetActiveBetter(num <= 0);
				FillPartItemData(m_partslist[i], i, num > 0);
			}
		}

		private void FillPartItemData(BagGGunPartItem bagGGunPartItem, int index, bool canAddPart)
		{
			_003CFillPartItemData_003Ec__AnonStorey2 _003CFillPartItemData_003Ec__AnonStorey = new _003CFillPartItemData_003Ec__AnonStorey2();
			_003CFillPartItemData_003Ec__AnonStorey.canAddPart = canAddPart;
			_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem = bagGGunPartItem;
			_003CFillPartItemData_003Ec__AnonStorey._0024this = this;
			_003CFillPartItemData_003Ec__AnonStorey.itemType = Singleton<BagMgr>.Ins.PartTypes[index];
			if (_003CFillPartItemData_003Ec__AnonStorey.itemType == 19)
			{
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject.SetActiveBetter(false);
				return;
			}
			_003CFillPartItemData_003Ec__AnonStorey.bagItem = Singleton<BagMgr>.Ins.GetPartBagItemIdByType(_gunParts, _003CFillPartItemData_003Ec__AnonStorey.itemType);
			if (_003CFillPartItemData_003Ec__AnonStorey.bagItem == null)
			{
				bool trueOrFalse = Singleton<BagMgr>.Ins.GunHasPartCanUse(GunBagItem.itemId, _gunParts, _003CFillPartItemData_003Ec__AnonStorey.itemType);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_gun_part_red.SetActiveBetter(trueOrFalse);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_icon.SetActiveBetter(false);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_duration.SetActiveBetter(false);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_suo.SetActiveBetter(false);
				ClickListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject, string.Empty).onClick = _003CFillPartItemData_003Ec__AnonStorey._003C_003Em__0;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onBeginDrag = null;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onDrag = null;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onEndDrag = null;
				switch (_003CFillPartItemData_003Ec__AnonStorey.itemType)
				{
				default:
					if (_003CFillPartItemData_003Ec__AnonStorey.itemType == 14)
					{
						View.SetLabelText(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, Utils.GetString(252));
					}
					break;
				case 19:
					View.SetLabelText(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, Utils.GetString(253));
					break;
				case 20:
					View.SetLabelText(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, Utils.GetString(282));
					break;
				case 18:
					View.SetLabelText(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, Utils.GetString(251));
					break;
				}
				View.SetLabelColor(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, _noColor);
			}
			else
			{
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_gun_part_red.SetActiveBetter(false);
				ItemCfg itemCfg = ItemCfg.Get(_003CFillPartItemData_003Ec__AnonStorey.bagItem.itemId);
				View.SetLabelText(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, itemCfg.name);
				View.SetLabelColor(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.txt_partnameText, _hasColor);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_icon.SetActiveBetter(true);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_duration.SetActiveBetter(true);
				_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_suo.SetActiveBetter(false);
				View.SetItemSprite(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_icon, itemCfg.icon);
				if (itemCfg.durability > 0)
				{
					_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_duration.SetActiveBetter(true);
					View.SetSlider(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_duration, (float)_003CFillPartItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
				}
				else
				{
					_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.m_duration.SetActiveBetter(false);
				}
				ClickListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject, string.Empty).onClick = _003CFillPartItemData_003Ec__AnonStorey._003C_003Em__1;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onBeginDrag = _003CFillPartItemData_003Ec__AnonStorey._003C_003Em__2;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onDrag = _bagPage.OnDragMask;
				DragListener.Get(_003CFillPartItemData_003Ec__AnonStorey.bagGGunPartItem.gameObject).onEndDrag = _003CFillPartItemData_003Ec__AnonStorey._003C_003Em__3;
			}
		}

		private void OpenCanAdd(int itemType, int instanceId, Vector3 localPos)
		{
			m_can_add.SetActiveBetter(true);
			_openItemType = itemType;
			_selectInstanceId = instanceId;
			m_can_add.transform.localPosition = new Vector3(m_can_add.transform.localPosition.x, localPos.y, m_can_add.transform.localPosition.z);
			RefreshCanAdd(itemType);
		}

		private void CloseCanAdd()
		{
			m_can_add.SetActiveBetter(false);
		}

		private void RefreshCanAdd(int itemType)
		{
			_partBagItems.Clear();
			BagItem bagItem = new BagItem();
			bagItem.instanceId = -1;
			_partBagItems.Add(bagItem);
			GunCfg gunCfg = GunCfg.Get(GunBagItem.itemId);
			switch (itemType)
			{
			case 18:
				_partBagItems.AddRange(GetCanAdd(gunCfg.muzzleParts));
				break;
			case 19:
				_partBagItems.AddRange(GetCanAdd(gunCfg.propParts));
				break;
			case 14:
				_partBagItems.AddRange(GetCanAdd(gunCfg.aimParts));
				break;
			case 20:
				_partBagItems.AddRange(GetCanAdd(gunCfg.clipParts));
				break;
			case 21:
				_partBagItems.AddRange(GetCanAdd(gunCfg.qiangbaParts));
				break;
			case 24:
				_partBagItems.AddRange(GetCanAddBullet(gunCfg.bulletIds));
				break;
			}
			SetCanAddData();
		}

		private void SetCanAddData()
		{
			_canAddScrollPanel.Clear();
			_canAddScrollPanel.Reset(_partBagItems.Count, FillCanAddBagItemData);
		}

		private void FillCanAddBagItemData(GameObject go, int index)
		{
			_003CFillCanAddBagItemData_003Ec__AnonStorey3 _003CFillCanAddBagItemData_003Ec__AnonStorey = new _003CFillCanAddBagItemData_003Ec__AnonStorey3();
			_003CFillCanAddBagItemData_003Ec__AnonStorey._0024this = this;
			GWeaponCanAddPart component = go.GetComponent<GWeaponCanAddPart>();
			_003CFillCanAddBagItemData_003Ec__AnonStorey.bagItem = _partBagItems[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillCanAddBagItemData_003Ec__AnonStorey.bagItem.itemId);
			if (_003CFillCanAddBagItemData_003Ec__AnonStorey.bagItem.instanceId < 0)
			{
				component.m_unload.SetActiveBetter(true);
				component.m_part.SetActiveBetter(false);
				ClickListener.Get(component.m_unload, string.Empty).onClick = _003CFillCanAddBagItemData_003Ec__AnonStorey._003C_003Em__0;
				return;
			}
			component.m_unload.SetActiveBetter(false);
			component.m_part.SetActiveBetter(true);
			component.m_select.SetActiveBetter(false);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_nameText, BagPage.GetNameForBinding(itemCfg.name, _003CFillCanAddBagItemData_003Ec__AnonStorey.bagItem.isBind));
			if (itemCfg.durability > 0)
			{
				component.m_duration.SetActiveBetter(true);
				View.SetSlider(component.m_duration, (float)_003CFillCanAddBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
			}
			else
			{
				component.m_duration.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillCanAddBagItemData_003Ec__AnonStorey._003C_003Em__1;
		}

		private List<BagItem> GetCanAddBullet(List<int> bulletItemIds)
		{
			List<BagItem> list = new List<BagItem>();
			for (int i = 0; i < bulletItemIds.Count; i++)
			{
				BagItem bagItem = new BagItem();
				bagItem.itemId = bulletItemIds[i];
				bagItem.instanceId = 9999;
				bagItem.number = Singleton<BagMgr>.Ins.GetItemNum(bagItem.itemId);
				if (bagItem.number > 0)
				{
					list.Add(bagItem);
				}
			}
			return list;
		}

		private List<BagItem> GetCanAdd(List<int> itemIds)
		{
			List<BagItem> list = new List<BagItem>();
			for (int i = 0; i < itemIds.Count; i++)
			{
				list.AddRange(Singleton<BagMgr>.Ins.GetAllItemByItemId(itemIds[i]));
			}
			return list;
		}

		private void Awake()
		{
			btn_back_desc = base.transform.Find("GameObject (2)/btn_back_desc").gameObject;
			btn_combination = base.transform.Find("GameObject (2)/anniu/btn_combination").gameObject;
			btn_destroy = base.transform.Find("GameObject (2)/anniu/btn_destroy").gameObject;
			btn_discard = base.transform.Find("GameObject (2)/anniu/btn_discard").gameObject;
			btn_disrobe = base.transform.Find("GameObject (2)/anniu/btn_disrobe").gameObject;
			btn_equip = base.transform.Find("GameObject (2)/anniu/btn_equip").gameObject;
			btn_out_shortcut_bar = base.transform.Find("GameObject (2)/anniu/btn_out_shortcut_bar").gameObject;
			btn_sell = base.transform.Find("GameObject (2)/anniu/btn_sell").gameObject;
			btn_split = base.transform.Find("GameObject (2)/anniu/btn_split").gameObject;
			btn_to_shortcut_bar = base.transform.Find("GameObject (2)/anniu/btn_to_shortcut_bar").gameObject;
			btn_tohand_equip = base.transform.Find("GameObject (2)/anniu/btn_tohand_equip").gameObject;
			btn_use = base.transform.Find("GameObject (2)/anniu/btn_use").gameObject;
			m_bullet = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts/m_bullet").gameObject;
			m_bullet_icon = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts/m_bullet/m_bullet_icon").gameObject;
			m_camera = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root/m_model_role/m_camera").gameObject;
			m_can_add = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/GameObject/m_can_add").gameObject;
			m_cell = View.AddComponentIfNotExist<GWeaponCanAddPart>(base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/GameObject/m_can_add/scp_add_part/content/m_cell").gameObject);
			m_durability = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/naijiu/GameObject/m_durability").gameObject;
			m_gun_partandbullet = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet").gameObject;
			m_gun_partandbullet_root = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root").gameObject;
			m_hero_tex = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root/m_hero_tex").gameObject;
			m_model = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root/m_model_role/m_model").gameObject;
			m_model_role = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root/m_model_role").gameObject;
			m_model_root = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root").gameObject;
			m_parts = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts").gameObject.GetComponent<UIGameObjectList>().objects;
			m_partsObj = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts").gameObject;
			if (m_partslist.Count <= 0)
			{
				for (int i = 0; i < m_parts.Length; i++)
				{
					m_partslist.Add(View.AddComponentIfNotExist<BagGGunPartItem>(m_parts[i].gameObject));
				}
			}
			m_role_bg = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/m_model_root/m_role_bg").gameObject;
			m_weapon_zone = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/m_weapon_zone").gameObject;
			scp_add_part = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/GameObject/m_can_add/scp_add_part").gameObject;
			txt_bullet_max_num = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject (3)/Image (3)/txt_bullet_max_num").gameObject;
			txt_bullet_max_numText = txt_bullet_max_num.GetComponent<Text>();
			txt_bullet_name = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts/m_bullet/Image/txt_bullet_name").gameObject;
			txt_bullet_nameText = txt_bullet_name.GetComponent<Text>();
			txt_bullet_num = base.transform.Find("GameObject (2)/Image/tasks/content/m_gun_partandbullet_root/m_gun_partandbullet/Image/m_parts/m_bullet/txt_bullet_num").gameObject;
			txt_bullet_numText = txt_bullet_num.GetComponent<Text>();
			txt_change_bullet_speed = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject (5)/Image (5)/txt_change_bullet_speed").gameObject;
			txt_change_bullet_speedText = txt_change_bullet_speed.GetComponent<Text>();
			txt_damage = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject/Image/txt_damage").gameObject;
			txt_damageText = txt_damage.GetComponent<Text>();
			txt_desc_desc = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/Image (6)/txt_desc_desc").gameObject;
			txt_desc_descText = txt_desc_desc.GetComponent<Text>();
			txt_durability = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/naijiu/txt_durability").gameObject;
			txt_durabilityText = txt_durability.GetComponent<Text>();
			txt_name = base.transform.Find("GameObject (2)/Image/tasks/content/RawImage/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_range = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject (1)/Image (1)/txt_range").gameObject;
			txt_rangeText = txt_range.GetComponent<Text>();
			txt_shot_speed = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject (2)/Image (2)/txt_shot_speed").gameObject;
			txt_shot_speedText = txt_shot_speed.GetComponent<Text>();
			txt_stability = base.transform.Find("GameObject (2)/Image/tasks/content/GameObject/GameObject/GameObject (4)/Image (4)/txt_stability").gameObject;
			txt_stabilityText = txt_stability.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003CInit_003Em__0(GameObject go)
		{
			_bagPage.OnEnter(3);
		}
	}
}
