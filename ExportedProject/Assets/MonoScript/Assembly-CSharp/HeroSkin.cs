using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class HeroSkin
{
	[CompilerGenerated]
	private sealed class _003CLoadBaseHero_003Ec__AnonStorey0
	{
		internal Utils.VoidDelegate initAllSkin;

		internal Utils.VoidDelegate loadGunCallBack;

		internal string anim;

		internal HeroSkin _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (o != null)
			{
				o.transform.SetParent(_0024this._parentGo.transform, false);
				o.GetComponent<Rigidbody>().useGravity = false;
				OtherPlayerController component = o.GetComponent<OtherPlayerController>();
				if ((bool)component)
				{
					component.enabled = false;
				}
				o.transform.localScale = Vector3.one;
				o.transform.localEulerAngles = Vector3.zero;
				o.transform.localPosition = Vector3.zero;
				o.GetComponent<Rigidbody>().useGravity = false;
				_0024this._mCharacterSetSkin = o.GetComponent<CharacterSetSkin>();
				if (!_0024this._mCharacterSetSkin)
				{
					_0024this._mCharacterSetSkin = o.AddComponent<CharacterSetSkin>();
				}
				Utils.TriggerEvent(initAllSkin);
				Utils.TriggerEvent(loadGunCallBack);
				_0024this._roleAnimator = o.GetComponent<Animator>();
				_0024this._baseLayer = _0024this._roleAnimator.GetLayerIndex("Base Layer");
				_0024this._roleAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				_0024this._roleAnimator.CrossFade(anim, 0.1f, _0024this._baseLayer);
				o.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadFromAB_003Ec__AnonStorey1
	{
		internal ItemCfg modelInfo;

		internal int parentIndex;

		internal HeroSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			Rigidbody component = go.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.useGravity = false;
			}
			_0024this._skinModelLRU.Set(modelInfo.id, go);
			_0024this.SetSkinSuitable(modelInfo, parentIndex, go);
			if (!_0024this._nowSkins.ContainsKey(modelInfo.childType) || _0024this._nowSkins[modelInfo.childType] != modelInfo.id)
			{
				go.SetActiveBetter(false);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadWeapon_003Ec__AnonStorey2
	{
		internal GameObject root;

		internal HeroSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			_0024this.ClearGun();
			if (!root)
			{
				UnityEngine.Object.DestroyImmediate(go);
				return;
			}
			go.transform.SetParent(root.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			_0024this._weaponGameObject = go;
			Transform transform = go.transform.Find("ef");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(true);
			}
			_0024this.SetWeaponPartGameObject(_0024this._weaponGameObject, _0024this._parts);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadPartModel_003Ec__AnonStorey3
	{
		internal ItemCfg partCfg;

		internal GunInfo gunInfo;

		internal HeroSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if (partCfg == null)
			{
				UnityEngine.Object.Destroy(go);
				return;
			}
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			_0024this._partsLru.Set(partCfg.id, go);
			_0024this.EquipPart(partCfg, go, gunInfo);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadPartModel_003Ec__AnonStorey4
	{
		internal ItemCfg partCfg;

		internal HeroSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if (partCfg == null)
			{
				UnityEngine.Object.Destroy(go);
				return;
			}
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			_0024this._partsLru.Set(partCfg.id, go);
			if ((bool)_0024this._weaponGameObject)
			{
				GunInfo component = _0024this._weaponGameObject.GetComponent<GunInfo>();
				_0024this.EquipPart(partCfg, go, component);
			}
		}
	}

	private GameObject _parentGo;

	private CharacterSetSkin _mCharacterSetSkin;

	private Animator _roleAnimator;

	private int _baseLayer;

	private readonly Dictionary<int, int> _nowSkins = new Dictionary<int, int>();

	private readonly Dictionary<int, int> _originSkins = new Dictionary<int, int>();

	private readonly LRU<int, GameObject> _skinModelLRU = new LRU<int, GameObject>();

	private bool _isMan;

	private int _currentId;

	private HashSet<int> _parts = new HashSet<int>();

	private readonly LRU<int, GameObject> _partsLru = new LRU<int, GameObject>();

	private GameObject _weaponGameObject;

	public HashSet<int> CurSkins
	{
		get
		{
			return new HashSet<int>(_nowSkins.Values);
		}
	}

	public HeroSkin(GameObject parentGo, bool isMan)
	{
		_parentGo = parentGo;
		_skinModelLRU.onRemoveEntry = OnRemoveEntry;
		_isMan = isMan;
	}

	public void SetSex(bool isMan)
	{
		_isMan = isMan;
	}

	private bool OnRemoveEntry(int key, GameObject value)
	{
		if (value.activeSelf)
		{
			return false;
		}
		UnityEngine.Object.DestroyImmediate(value);
		return true;
	}

	public void Clear()
	{
		List<GameObject> values = _skinModelLRU.GetValues();
		int i = 0;
		for (int count = values.Count; i < count; i++)
		{
			UnityEngine.Object.DestroyImmediate(values[i], true);
		}
		if (_mCharacterSetSkin != null && (bool)_mCharacterSetSkin.gameObject)
		{
			UnityEngine.Object.DestroyImmediate(_mCharacterSetSkin.gameObject, true);
		}
		_skinModelLRU.Clear();
		_nowSkins.Clear();
		_originSkins.Clear();
		ClearGun();
		_parentGo = null;
		_roleAnimator = null;
	}

	public void LoadBasicHero(IEnumerable<int> skins, IEnumerable<int> originSkins, string anim, int gunId = -1, HashSet<int> parts = null)
	{
		if (originSkins == null)
		{
			throw new NullReferenceException("LocalChangeClothes : originSkins can't be null.");
		}
		ResetOriginSkins(originSkins);
		ResetPart2SkinDic(skins);
		SetCurrentId(gunId);
		SetPartsCurrentId(parts);
		LoadBaseHero(anim, InitAllSkin, LoadWeaponCallBack);
	}

	public void PlayRoleAni(string aniStr, string effect = "")
	{
		if (!(_mCharacterSetSkin == null))
		{
			Animator component = _mCharacterSetSkin.GetComponent<Animator>();
			_baseLayer = component.GetLayerIndex("Base Layer");
			component.Play(aniStr, _baseLayer, 0f);
			component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
	}

	private void ResetPart2SkinDic(IEnumerable<int> skins)
	{
		_nowSkins.Clear();
		foreach (int skin in skins)
		{
			ItemCfg itemCfg = ItemCfg.Get(skin);
			if (itemCfg != null)
			{
				_nowSkins[itemCfg.childType] = skin;
			}
		}
	}

	private void ResetOriginSkins(IEnumerable<int> skins)
	{
		_originSkins.Clear();
		foreach (int skin in skins)
		{
			ItemCfg itemCfg = ItemCfg.Get(skin);
			if (itemCfg != null)
			{
				_originSkins[itemCfg.childType] = skin;
			}
		}
	}

	private void LoadBaseHero(string anim, Utils.VoidDelegate initAllSkin, Utils.VoidDelegate loadGunCallBack)
	{
		_003CLoadBaseHero_003Ec__AnonStorey0 _003CLoadBaseHero_003Ec__AnonStorey = new _003CLoadBaseHero_003Ec__AnonStorey0();
		_003CLoadBaseHero_003Ec__AnonStorey.initAllSkin = initAllSkin;
		_003CLoadBaseHero_003Ec__AnonStorey.loadGunCallBack = loadGunCallBack;
		_003CLoadBaseHero_003Ec__AnonStorey.anim = anim;
		_003CLoadBaseHero_003Ec__AnonStorey._0024this = this;
		if ((bool)_mCharacterSetSkin)
		{
			Utils.TriggerEvent(_003CLoadBaseHero_003Ec__AnonStorey.initAllSkin);
			Utils.TriggerEvent(_003CLoadBaseHero_003Ec__AnonStorey.loadGunCallBack);
			_roleAnimator.CrossFade(_003CLoadBaseHero_003Ec__AnonStorey.anim, 0.1f, _baseLayer);
		}
		else if ((bool)Battle.Ins && Battle.Ins.PlayTest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("role")) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.SetParent(_parentGo.transform, false);
				gameObject.GetComponent<Rigidbody>().useGravity = false;
				OtherPlayerController component = gameObject.GetComponent<OtherPlayerController>();
				if ((bool)component)
				{
					component.enabled = false;
				}
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<Rigidbody>().useGravity = false;
				_mCharacterSetSkin = gameObject.GetComponent<CharacterSetSkin>();
				if (!_mCharacterSetSkin)
				{
					_mCharacterSetSkin = gameObject.AddComponent<CharacterSetSkin>();
				}
				Utils.TriggerEvent(_003CLoadBaseHero_003Ec__AnonStorey.initAllSkin);
				Utils.TriggerEvent(_003CLoadBaseHero_003Ec__AnonStorey.loadGunCallBack);
				_roleAnimator = gameObject.GetComponent<Animator>();
				_baseLayer = _roleAnimator.GetLayerIndex("Base Layer");
				_roleAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				_roleAnimator.CrossFade(_003CLoadBaseHero_003Ec__AnonStorey.anim, 0.1f, _baseLayer);
				gameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			}
		}
		else
		{
			ResMgr.Ins.CreateFromAB("role/role.ab", null, _003CLoadBaseHero_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private void InitAllSkin()
	{
		foreach (int value in _nowSkins.Values)
		{
			ItemCfg itemCfg = ItemCfg.Get(value);
			if (itemCfg != null && itemCfg.childType != 15)
			{
				if (itemCfg.childType == 23)
				{
					ItemCfg itemCfg2 = ItemCfg.Get(value + 1);
					SetNewSkin(itemCfg2, (!_isMan) ? itemCfg2.womanModelPath : itemCfg2.modelPath, itemCfg2.childType);
				}
				SetNewSkin(itemCfg, (!_isMan) ? itemCfg.womanModelPath : itemCfg.modelPath, itemCfg.childType);
			}
		}
	}

	private void SetNewSkin(ItemCfg modelInfo, string skinName, int parentIndex = -1)
	{
		GameObject value;
		if (_skinModelLRU.TryGetValue(modelInfo.id, out value))
		{
			SetSkinSuitable(modelInfo, parentIndex, value);
			value.SetActiveBetter(true);
		}
		else
		{
			LoadFromAB(modelInfo, skinName, parentIndex);
		}
	}

	private void SetSkinSuitable(ItemCfg modelInfo, int parentIndex, GameObject go)
	{
		if (string.IsNullOrEmpty(modelInfo.root))
		{
			_mCharacterSetSkin.SetSkin(go, modelInfo.childType);
		}
		else
		{
			_mCharacterSetSkin.SetSkinAttach(parentIndex, modelInfo.root, go);
		}
	}

	private void LoadFromAB(ItemCfg modelInfo, string skinName, int parentIndex)
	{
		_003CLoadFromAB_003Ec__AnonStorey1 _003CLoadFromAB_003Ec__AnonStorey = new _003CLoadFromAB_003Ec__AnonStorey1();
		_003CLoadFromAB_003Ec__AnonStorey.modelInfo = modelInfo;
		_003CLoadFromAB_003Ec__AnonStorey.parentIndex = parentIndex;
		_003CLoadFromAB_003Ec__AnonStorey._0024this = this;
		ResMgr.Ins.CreateFromAB(skinName, null, _003CLoadFromAB_003Ec__AnonStorey._003C_003Em__0);
	}

	public void ChangeSkin(int itemId)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg == null)
		{
			return;
		}
		if (_nowSkins.ContainsKey(itemCfg.childType))
		{
			RemoveRoleSkin(_nowSkins[itemCfg.childType]);
		}
		if (itemCfg.childType == 23)
		{
			if (_nowSkins.ContainsKey(31))
			{
				RemoveRoleSkin(_nowSkins[31]);
			}
			ItemCfg itemCfg2 = ItemCfg.Get(itemCfg.id + 1);
			SetNewSkin(itemCfg2, (!_isMan) ? itemCfg2.womanModelPath : itemCfg2.modelPath, itemCfg2.childType);
		}
		PutSkin(itemCfg);
	}

	private void PutSkin(ItemCfg itemCfg)
	{
		if (itemCfg != null)
		{
			_nowSkins[itemCfg.childType] = itemCfg.id;
			SetNewSkin(itemCfg, (!_isMan) ? itemCfg.womanModelPath : itemCfg.modelPath, itemCfg.childType);
		}
	}

	public void RemoveRoleSkin(int itemId)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg == null)
		{
			return;
		}
		_nowSkins.Remove(itemCfg.childType);
		GameObject value;
		if (_skinModelLRU.TryGetValue(itemCfg.id, out value))
		{
			value.SetActiveBetter(false);
		}
		if (itemCfg.childType == 23)
		{
			_nowSkins.Remove(31);
			GameObject value2;
			if (_skinModelLRU.TryGetValue(itemCfg.id + 1, out value2))
			{
				value2.SetActiveBetter(false);
			}
			_mCharacterSetSkin.RemoveCombineSkin(31);
		}
		_mCharacterSetSkin.RemoveCombineSkin(itemCfg.childType);
	}

	public void RemoveAllSkin()
	{
		if ((bool)_mCharacterSetSkin)
		{
			_mCharacterSetSkin.RemoveAllSkin();
		}
		foreach (KeyValuePair<int, int> nowSkin in _nowSkins)
		{
			GameObject value;
			if (_skinModelLRU.TryGetValue(nowSkin.Value, out value))
			{
				value.SetActiveBetter(false);
			}
		}
		_originSkins.Clear();
	}

	public void SetCurrentId(int itemId)
	{
		_currentId = itemId;
		_partsLru.onRemoveEntry = OnGunRemoveEntry;
	}

	private bool OnGunRemoveEntry(int key, GameObject value)
	{
		if (value.activeSelf)
		{
			return false;
		}
		UnityEngine.Object.DestroyImmediate(value);
		return true;
	}

	public void SetPartsCurrentId(HashSet<int> parts)
	{
		_parts = parts;
	}

	public void SetCurrentWeapon(int gunId, HashSet<int> parts = null)
	{
		if ((bool)_mCharacterSetSkin)
		{
			SetCurrentId(gunId);
			SetPartsCurrentId(parts);
			LoadWeaponCallBack();
		}
	}

	private void LoadWeaponCallBack()
	{
		if (_currentId < 0)
		{
			ClearGun();
			return;
		}
		GameObject childObjByName = Utils.GetChildObjByName("wuqi_guadian", _mCharacterSetSkin.gameObject);
		LoadWeapon(_currentId, childObjByName);
	}

	public void LoadWeapon(int itemId, GameObject root)
	{
		_003CLoadWeapon_003Ec__AnonStorey2 _003CLoadWeapon_003Ec__AnonStorey = new _003CLoadWeapon_003Ec__AnonStorey2();
		_003CLoadWeapon_003Ec__AnonStorey.root = root;
		_003CLoadWeapon_003Ec__AnonStorey._0024this = this;
		if ((bool)_003CLoadWeapon_003Ec__AnonStorey.root)
		{
			ItemCfg itemCfg = ItemCfg.Get(itemId);
			ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadWeapon_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public void SetWeaponPartGameObject(GameObject gunGo, ICollection<int> parts)
	{
		if (parts == null)
		{
			return;
		}
		GunInfo component = gunGo.GetComponent<GunInfo>();
		if (!(component != null))
		{
			return;
		}
		foreach (int part in parts)
		{
			if (part > 0)
			{
				LoadPartModel(component, part);
			}
		}
	}

	public void LoadPartModel(GunInfo gunInfo, int itemId)
	{
		_003CLoadPartModel_003Ec__AnonStorey3 _003CLoadPartModel_003Ec__AnonStorey = new _003CLoadPartModel_003Ec__AnonStorey3();
		_003CLoadPartModel_003Ec__AnonStorey.gunInfo = gunInfo;
		_003CLoadPartModel_003Ec__AnonStorey._0024this = this;
		_003CLoadPartModel_003Ec__AnonStorey.partCfg = ItemCfg.Get(itemId);
		if (!string.IsNullOrEmpty(_003CLoadPartModel_003Ec__AnonStorey.partCfg.modelPath))
		{
			GameObject value;
			if (_partsLru.TryGetValue(_003CLoadPartModel_003Ec__AnonStorey.partCfg.id, out value))
			{
				EquipPart(_003CLoadPartModel_003Ec__AnonStorey.partCfg, value, _003CLoadPartModel_003Ec__AnonStorey.gunInfo);
			}
			else
			{
				ResMgr.Ins.CreateFromAB(_003CLoadPartModel_003Ec__AnonStorey.partCfg.modelPath, null, _003CLoadPartModel_003Ec__AnonStorey._003C_003Em__0);
			}
		}
	}

	public void EquipPart(ItemCfg itemCfg, GameObject part, GunInfo gunInfo)
	{
		Transform anchor = GetAnchor(itemCfg.type, gunInfo);
		_partsLru.Set(itemCfg.id, part);
		part.transform.SetParent(anchor, false);
		part.SetActiveBetter(true);
	}

	public void LoadPartModel(int itemId)
	{
		_003CLoadPartModel_003Ec__AnonStorey4 _003CLoadPartModel_003Ec__AnonStorey = new _003CLoadPartModel_003Ec__AnonStorey4();
		_003CLoadPartModel_003Ec__AnonStorey._0024this = this;
		int oldPartId = GetOldPartId(itemId);
		if (oldPartId > 0)
		{
			UnLoadPartGameObject(oldPartId);
		}
		_003CLoadPartModel_003Ec__AnonStorey.partCfg = ItemCfg.Get(itemId);
		if (!string.IsNullOrEmpty(_003CLoadPartModel_003Ec__AnonStorey.partCfg.modelPath))
		{
			GameObject value;
			if (_partsLru.TryGetValue(_003CLoadPartModel_003Ec__AnonStorey.partCfg.id, out value) && (bool)_weaponGameObject)
			{
				GunInfo component = _weaponGameObject.GetComponent<GunInfo>();
				EquipPart(_003CLoadPartModel_003Ec__AnonStorey.partCfg, value, component);
			}
			else
			{
				ResMgr.Ins.CreateFromAB(_003CLoadPartModel_003Ec__AnonStorey.partCfg.modelPath, null, _003CLoadPartModel_003Ec__AnonStorey._003C_003Em__0);
			}
		}
	}

	public void UnLoadPartGameObject(int itemId)
	{
		_parts.Remove(itemId);
		GameObject value;
		if (_partsLru.TryGetValue(itemId, out value))
		{
			value.SetActiveBetter(false);
		}
	}

	public void UnLoadPartGameObject(GunInfo gunInfo, int itemType)
	{
		Transform anchor = GetAnchor(itemType, gunInfo);
		if (anchor.childCount > 0)
		{
			anchor.GetChild(0).gameObject.SetActiveBetter(false);
		}
	}

	public static Transform GetAnchor(int itemType, GunInfo gunInfo)
	{
		switch (itemType)
		{
		case 14:
			return gunInfo.Miaojiu;
		case 20:
			return gunInfo.Danjia;
		case 18:
			return gunInfo.Muzzle;
		case 19:
			return gunInfo.Chuizhiwoba;
		default:
			return null;
		}
	}

	public void ClearGun()
	{
		List<GameObject> values = _partsLru.GetValues();
		int i = 0;
		for (int count = values.Count; i < count; i++)
		{
			UnityEngine.Object.DestroyImmediate(values[i], true);
		}
		if ((bool)_weaponGameObject)
		{
			UnityEngine.Object.DestroyImmediate(_weaponGameObject, true);
		}
		_partsLru.Clear();
	}

	public int GetOldPartId(int itemId)
	{
		int result = -1;
		int type = ItemCfg.Get(itemId).type;
		foreach (int part in _parts)
		{
			if (ItemCfg.Get(part).type == type)
			{
				result = part;
				_parts.Remove(part);
				break;
			}
		}
		_parts.Add(itemId);
		return result;
	}
}
