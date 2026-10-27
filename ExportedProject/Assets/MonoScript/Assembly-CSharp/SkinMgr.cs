using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class SkinMgr : MonoBehaviour
{
	public class BasicGameObjectClass
	{
		public GameObject basicGameObject;
	}

	[CompilerGenerated]
	private sealed class _003CLoadHero_003Ec__AnonStorey0
	{
		internal BasicGameObjectClass basicGameObject;

		internal GameObject root;

		internal string aniStr;

		internal HashSet<int> model;

		internal Dictionary<int, GameObject> modelDictionary;

		internal Utils.GameObjectDelegate gameObjectCallback;

		internal Utils.VoidDelegate callback;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!(o != null))
			{
				return;
			}
			if ((bool)basicGameObject.basicGameObject)
			{
				Object.DestroyImmediate(o);
				o = basicGameObject.basicGameObject;
			}
			else
			{
				if (root == null)
				{
					Object.DestroyImmediate(o);
					return;
				}
				o.transform.SetParent(root.transform, false);
				o.transform.localScale = Vector3.one;
				o.transform.localEulerAngles = Vector3.zero;
				o.transform.localPosition = Vector3.zero;
				basicGameObject.basicGameObject = o;
				Rigidbody component = o.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					component.useGravity = false;
					component.detectCollisions = false;
				}
			}
			CharacterSetSkin component2 = o.GetComponent<CharacterSetSkin>();
			component2.Bones = o.transform.Find("Bip001").gameObject;
			PlayRoleAni(o, aniStr, string.Empty);
			if (model != null)
			{
				SetSkin(component2, modelDictionary, model);
			}
			Utils.TriggerEvent(gameObjectCallback, basicGameObject.basicGameObject);
			Utils.TriggerEvent(callback);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadHero_003Ec__AnonStorey1
	{
		internal GameObject basicGameObject;

		internal GameObject root;

		internal bool roleModelSex;

		internal HashSet<int> model;

		internal Dictionary<int, GameObject> modelDictionary;

		internal Utils.GameObjectDelegate gameObjectCallback;

		internal Utils.VoidDelegate callback;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!(o != null))
			{
				return;
			}
			if ((bool)basicGameObject)
			{
				Object.DestroyImmediate(o);
				o = basicGameObject;
			}
			else
			{
				if (root == null)
				{
					Object.DestroyImmediate(o);
					return;
				}
				o.transform.SetParent(root.transform, false);
				o.transform.localScale = Vector3.one;
				o.transform.localEulerAngles = Vector3.zero;
				o.transform.localPosition = Vector3.zero;
				basicGameObject = o;
				Rigidbody component = o.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					component.useGravity = false;
					component.detectCollisions = false;
				}
			}
			CharacterSetSkin component2 = o.GetComponent<CharacterSetSkin>();
			component2.Bones = o.transform.Find("Bip001").gameObject;
			PlayRoleAni(o, roleModelSex);
			if (model != null)
			{
				SetSkin(component2, modelDictionary, model);
			}
			Utils.TriggerEvent(gameObjectCallback, basicGameObject);
			Utils.TriggerEvent(callback);
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetNewSkin_003Ec__AnonStorey2
	{
		internal CharacterSetSkin characterSetSkin;

		internal Dictionary<int, GameObject> modelDictionary;

		internal ItemCfg model;

		internal void _003C_003Em__0(GameObject go)
		{
			if (characterSetSkin == null)
			{
				Object.DestroyImmediate(go);
				return;
			}
			if (modelDictionary.ContainsKey(model.id))
			{
				Object.DestroyImmediate(go);
				go = modelDictionary[model.id];
			}
			else
			{
				modelDictionary.Add(model.id, go);
			}
			SetSkin(model, go, characterSetSkin);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadHero_003Ec__AnonStorey3
	{
		internal GameObject root;

		internal BasicGameObjectClass basicGameObject;

		internal ICollection<int> model;

		internal string animator;

		internal string effectStr;

		internal Utils.VoidDelegate callback;

		internal void _003C_003Em__0(GameObject o)
		{
			if (o != null)
			{
				o.transform.SetParent(root.transform, false);
				o.transform.localScale = Vector3.one;
				o.transform.localEulerAngles = Vector3.zero;
				o.transform.localPosition = Vector3.zero;
				Rigidbody component = o.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					component.useGravity = false;
					component.detectCollisions = false;
				}
				if ((bool)basicGameObject.basicGameObject)
				{
					Object.DestroyImmediate(basicGameObject.basicGameObject);
				}
				basicGameObject.basicGameObject = o;
				CharacterSetSkin component2 = o.GetComponent<CharacterSetSkin>();
				component2.Bones = o.transform.Find("Bip001").gameObject;
				SetSkin(component2, model);
				if (!string.IsNullOrEmpty(animator))
				{
					PlayRoleAni(basicGameObject.basicGameObject, animator, effectStr);
				}
				Utils.TriggerEvent(callback);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetNewSkin_003Ec__AnonStorey4
	{
		internal CharacterSetSkin characterSetSkin;

		internal ItemCfg model;

		internal void _003C_003Em__0(GameObject go)
		{
			if (characterSetSkin == null)
			{
				Object.DestroyImmediate(go);
				return;
			}
			SetSkin(model, go, characterSetSkin);
			Transform transform = go.transform.Find("ef");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(true);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadWeapon_003Ec__AnonStorey5
	{
		internal BasicGameObjectClass basicGameObject;

		internal GameObject root;

		internal ICollection<int> parts;

		internal float gunLength;

		internal bool isCenter;

		internal void _003C_003Em__0(GameObject go)
		{
			if ((bool)basicGameObject.basicGameObject)
			{
				Object.DestroyImmediate(basicGameObject.basicGameObject);
			}
			if (!root)
			{
				Object.DestroyImmediate(go);
				return;
			}
			go.transform.SetParent(root.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			basicGameObject.basicGameObject = go;
			SetWeaponGameObject(basicGameObject.basicGameObject, parts);
			Transform transform = go.transform.Find("ef");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(true);
			}
			if (gunLength > 0f)
			{
				SkinnedMeshRenderer componentInChildren = go.transform.GetComponentInChildren<SkinnedMeshRenderer>();
				float num = gunLength / componentInChildren.bounds.size.x;
				go.transform.localScale = new Vector3(num, num, num);
				if (isCenter)
				{
					go.transform.localPosition -= go.transform.InverseTransformPoint(componentInChildren.bounds.center) * num;
				}
			}
			else if (isCenter)
			{
				go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadWeapon_003Ec__AnonStorey6
	{
		internal BasicGameObjectClass basicGameObject;

		internal GameObject root;

		internal List<int> partsInGunGameObject;

		internal List<int> partsPrepareInGunGameObject;

		internal void _003C_003Em__0(GameObject go)
		{
			if ((bool)basicGameObject.basicGameObject)
			{
				Object.DestroyImmediate(basicGameObject.basicGameObject);
			}
			go.transform.SetParent(root.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			go.transform.localEulerAngles = Vector3.zero;
			basicGameObject.basicGameObject = go;
			SetWeaponGameObject(basicGameObject.basicGameObject, partsInGunGameObject, partsPrepareInGunGameObject);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_LoadPartModel_003Ec__AnonStorey7
	{
		internal ItemCfg partCfg;

		internal GunInfo gunInfo;

		internal void _003C_003Em__0(GameObject o)
		{
			if (partCfg == null)
			{
				Object.Destroy(o);
				return;
			}
			Transform anchor = GetAnchor(partCfg.type, gunInfo);
			o.transform.SetParent(anchor, false);
		}
	}

	private static int BaseLayer;

	public static void LoadHero(BasicGameObjectClass basicGameObject, string aniStr, Dictionary<int, GameObject> modelDictionary, GameObject root, HashSet<int> model = null, Utils.VoidDelegate callback = null, Utils.GameObjectDelegate gameObjectCallback = null)
	{
		_003CLoadHero_003Ec__AnonStorey0 _003CLoadHero_003Ec__AnonStorey = new _003CLoadHero_003Ec__AnonStorey0();
		_003CLoadHero_003Ec__AnonStorey.basicGameObject = basicGameObject;
		_003CLoadHero_003Ec__AnonStorey.root = root;
		_003CLoadHero_003Ec__AnonStorey.aniStr = aniStr;
		_003CLoadHero_003Ec__AnonStorey.model = model;
		_003CLoadHero_003Ec__AnonStorey.modelDictionary = modelDictionary;
		_003CLoadHero_003Ec__AnonStorey.gameObjectCallback = gameObjectCallback;
		_003CLoadHero_003Ec__AnonStorey.callback = callback;
		foreach (KeyValuePair<int, GameObject> item in _003CLoadHero_003Ec__AnonStorey.modelDictionary)
		{
			item.Value.SetActive(false);
		}
		if ((bool)_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject)
		{
			_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject.SetActive(true);
			BaseLayer = _003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject.GetComponent<Animator>().GetLayerIndex("Base Layer");
			PlayRoleAni(_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject, _003CLoadHero_003Ec__AnonStorey.aniStr, string.Empty);
			if (_003CLoadHero_003Ec__AnonStorey.model != null)
			{
				SetSkin(_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject.GetComponent<CharacterSetSkin>(), _003CLoadHero_003Ec__AnonStorey.modelDictionary, _003CLoadHero_003Ec__AnonStorey.model);
			}
			Utils.TriggerEvent(_003CLoadHero_003Ec__AnonStorey.gameObjectCallback, _003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject);
			Utils.TriggerEvent(_003CLoadHero_003Ec__AnonStorey.callback);
		}
		else
		{
			ResMgr.Ins.CreateFromAB("role/role.ab", null, _003CLoadHero_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void LoadHero(GameObject basicGameObject, bool roleModelSex, Dictionary<int, GameObject> modelDictionary, GameObject root, HashSet<int> model = null, Utils.VoidDelegate callback = null, Utils.GameObjectDelegate gameObjectCallback = null)
	{
		_003CLoadHero_003Ec__AnonStorey1 _003CLoadHero_003Ec__AnonStorey = new _003CLoadHero_003Ec__AnonStorey1();
		_003CLoadHero_003Ec__AnonStorey.basicGameObject = basicGameObject;
		_003CLoadHero_003Ec__AnonStorey.root = root;
		_003CLoadHero_003Ec__AnonStorey.roleModelSex = roleModelSex;
		_003CLoadHero_003Ec__AnonStorey.model = model;
		_003CLoadHero_003Ec__AnonStorey.modelDictionary = modelDictionary;
		_003CLoadHero_003Ec__AnonStorey.gameObjectCallback = gameObjectCallback;
		_003CLoadHero_003Ec__AnonStorey.callback = callback;
		foreach (KeyValuePair<int, GameObject> item in _003CLoadHero_003Ec__AnonStorey.modelDictionary)
		{
			item.Value.SetActive(false);
		}
		if ((bool)_003CLoadHero_003Ec__AnonStorey.basicGameObject)
		{
			_003CLoadHero_003Ec__AnonStorey.basicGameObject.SetActive(true);
			BaseLayer = _003CLoadHero_003Ec__AnonStorey.basicGameObject.GetComponent<Animator>().GetLayerIndex("Base Layer");
			PlayRoleAni(_003CLoadHero_003Ec__AnonStorey.basicGameObject, _003CLoadHero_003Ec__AnonStorey.roleModelSex);
			if (_003CLoadHero_003Ec__AnonStorey.model != null)
			{
				SetSkin(_003CLoadHero_003Ec__AnonStorey.basicGameObject.GetComponent<CharacterSetSkin>(), _003CLoadHero_003Ec__AnonStorey.modelDictionary, _003CLoadHero_003Ec__AnonStorey.model);
			}
			Utils.TriggerEvent(_003CLoadHero_003Ec__AnonStorey.gameObjectCallback, _003CLoadHero_003Ec__AnonStorey.basicGameObject);
			Utils.TriggerEvent(_003CLoadHero_003Ec__AnonStorey.callback);
		}
		else
		{
			ResMgr.Ins.CreateFromAB("role/role.ab", null, _003CLoadHero_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private static void SetRole(GameObject basicGameObject, int roleModelId, Dictionary<int, GameObject> modelDictionary, HashSet<int> model, GameObject root, Utils.VoidDelegate callback = null, Utils.GameObjectDelegate gameObjectCallback = null)
	{
		basicGameObject.SetActive(true);
		BaseLayer = basicGameObject.GetComponent<Animator>().GetLayerIndex("Base Layer");
		string stateName = ((roleModelId != 1) ? "MainPanel.womankongshou_zhan_idle01" : "MainPanel.kongshou_zhan_idle01");
		basicGameObject.GetComponent<Animator>().CrossFade(stateName, 0.1f, BaseLayer);
		SetSkin(basicGameObject.GetComponent<CharacterSetSkin>(), modelDictionary, model);
		Utils.TriggerEvent(callback);
		Utils.TriggerEvent(gameObjectCallback, basicGameObject);
	}

	private static void PlayRoleAni(GameObject roleGameObject, bool roleModelSex)
	{
		string stateName = ((!roleModelSex) ? "MainPanel.womankongshou_zhan_idle01" : "MainPanel.kongshou_zhan_idle01");
		Animator component = roleGameObject.GetComponent<Animator>();
		component.CrossFade(stateName, 0.1f, BaseLayer);
		component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}

	public static void SetSkin(CharacterSetSkin characterSetSkin, Dictionary<int, GameObject> modelDictionary, HashSet<int> model)
	{
		characterSetSkin.RemoveAllSkin();
		foreach (int item in model)
		{
			ItemCfg itemCfg = ItemCfg.Get(item);
			if (itemCfg != null && itemCfg.type != 15)
			{
				SetNewSkin(itemCfg, modelDictionary, characterSetSkin);
			}
		}
	}

	private static void SetSkin(ItemCfg model, GameObject skinItem, CharacterSetSkin characterSetSkin)
	{
		skinItem.SetActive(true);
		characterSetSkin.RemoveCombineSkin(model.type);
		if (string.IsNullOrEmpty(model.root))
		{
			characterSetSkin.SetSkin(skinItem, model.type);
		}
		else
		{
			characterSetSkin.SetSkinAttach(model.type, model.root, skinItem);
		}
	}

	private static void SetNewSkin(ItemCfg model, Dictionary<int, GameObject> modelDictionary, CharacterSetSkin characterSetSkin)
	{
		_003CSetNewSkin_003Ec__AnonStorey2 _003CSetNewSkin_003Ec__AnonStorey = new _003CSetNewSkin_003Ec__AnonStorey2();
		_003CSetNewSkin_003Ec__AnonStorey.characterSetSkin = characterSetSkin;
		_003CSetNewSkin_003Ec__AnonStorey.modelDictionary = modelDictionary;
		_003CSetNewSkin_003Ec__AnonStorey.model = model;
		GameObject value;
		if (_003CSetNewSkin_003Ec__AnonStorey.modelDictionary.TryGetValue(_003CSetNewSkin_003Ec__AnonStorey.model.id, out value))
		{
			SetSkin(_003CSetNewSkin_003Ec__AnonStorey.model, value, _003CSetNewSkin_003Ec__AnonStorey.characterSetSkin);
		}
		else
		{
			ResMgr.Ins.CreateFromAB(_003CSetNewSkin_003Ec__AnonStorey.model.modelPath, null, _003CSetNewSkin_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void PlayRoleAni(GameObject roleGameObject, string aniStr, string effect = "")
	{
		int layerIndex = roleGameObject.GetComponent<Animator>().GetLayerIndex("Base Layer");
		Animator component = roleGameObject.GetComponent<Animator>();
		component.enabled = false;
		component.Rebind();
		component.Update(0f);
		component.enabled = true;
		component.Play(aniStr, layerIndex);
		component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		Transform transform = roleGameObject.transform.Find("effectroot");
		if (!transform)
		{
			GameObject gameObject = new GameObject("effectroot");
			gameObject.transform.SetParent(roleGameObject.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localEulerAngles = Vector3.zero;
			transform = gameObject.transform;
		}
		if (transform.childCount > 0)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Object.DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}
		if (!string.IsNullOrEmpty(effect))
		{
			SingletonMono<EffectMgr>.Ins.PlayEffectAtPos(effect, transform, Vector3.zero);
		}
	}

	public static void LoadHero(BasicGameObjectClass basicGameObject, ICollection<int> model, GameObject root, bool roleModelSex, string effectStr, string animator = "", Utils.VoidDelegate callback = null)
	{
		_003CLoadHero_003Ec__AnonStorey3 _003CLoadHero_003Ec__AnonStorey = new _003CLoadHero_003Ec__AnonStorey3();
		_003CLoadHero_003Ec__AnonStorey.root = root;
		_003CLoadHero_003Ec__AnonStorey.basicGameObject = basicGameObject;
		_003CLoadHero_003Ec__AnonStorey.model = model;
		_003CLoadHero_003Ec__AnonStorey.animator = animator;
		_003CLoadHero_003Ec__AnonStorey.effectStr = effectStr;
		_003CLoadHero_003Ec__AnonStorey.callback = callback;
		if ((bool)_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject)
		{
			CharacterSetSkin component = _003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject.GetComponent<CharacterSetSkin>();
			component.DestroyAllSkin();
			SetSkin(component, _003CLoadHero_003Ec__AnonStorey.model);
			if (!string.IsNullOrEmpty(_003CLoadHero_003Ec__AnonStorey.animator))
			{
				PlayRoleAni(_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject, _003CLoadHero_003Ec__AnonStorey.animator, _003CLoadHero_003Ec__AnonStorey.effectStr);
			}
			_003CLoadHero_003Ec__AnonStorey.basicGameObject.basicGameObject.transform.localEulerAngles = Vector3.zero;
			Utils.TriggerEvent(_003CLoadHero_003Ec__AnonStorey.callback);
		}
		else
		{
			ResMgr.Ins.CreateFromAB("role/role.ab", null, _003CLoadHero_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void SetSkin(CharacterSetSkin characterSetSkin, ICollection<int> model)
	{
		foreach (int item in model)
		{
			ItemCfg itemCfg = ItemCfg.Get(item);
			if (itemCfg != null && itemCfg.type != 15)
			{
				SetNewSkin(itemCfg, characterSetSkin);
			}
		}
	}

	private static void SetNewSkin(ItemCfg model, CharacterSetSkin characterSetSkin)
	{
		_003CSetNewSkin_003Ec__AnonStorey4 _003CSetNewSkin_003Ec__AnonStorey = new _003CSetNewSkin_003Ec__AnonStorey4();
		_003CSetNewSkin_003Ec__AnonStorey.characterSetSkin = characterSetSkin;
		_003CSetNewSkin_003Ec__AnonStorey.model = model;
		ResMgr.Ins.CreateFromAB(_003CSetNewSkin_003Ec__AnonStorey.model.modelPath, null, _003CSetNewSkin_003Ec__AnonStorey._003C_003Em__0);
	}

	public static void LoadWeapon(BasicGameObjectClass basicGameObject, int gunId, GameObject root, ICollection<int> parts, bool isCenter = false, float gunLength = 0f)
	{
		_003CLoadWeapon_003Ec__AnonStorey5 _003CLoadWeapon_003Ec__AnonStorey = new _003CLoadWeapon_003Ec__AnonStorey5();
		_003CLoadWeapon_003Ec__AnonStorey.basicGameObject = basicGameObject;
		_003CLoadWeapon_003Ec__AnonStorey.root = root;
		_003CLoadWeapon_003Ec__AnonStorey.parts = parts;
		_003CLoadWeapon_003Ec__AnonStorey.gunLength = gunLength;
		_003CLoadWeapon_003Ec__AnonStorey.isCenter = isCenter;
		if ((bool)_003CLoadWeapon_003Ec__AnonStorey.root)
		{
			ItemCfg itemCfg = ItemCfg.Get(gunId);
			ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadWeapon_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void LoadWeapon(BasicGameObjectClass basicGameObject, int gunId, GameObject root, List<int> partsInGunGameObject, List<int> partsPrepareInGunGameObject)
	{
		_003CLoadWeapon_003Ec__AnonStorey6 _003CLoadWeapon_003Ec__AnonStorey = new _003CLoadWeapon_003Ec__AnonStorey6();
		_003CLoadWeapon_003Ec__AnonStorey.basicGameObject = basicGameObject;
		_003CLoadWeapon_003Ec__AnonStorey.root = root;
		_003CLoadWeapon_003Ec__AnonStorey.partsInGunGameObject = partsInGunGameObject;
		_003CLoadWeapon_003Ec__AnonStorey.partsPrepareInGunGameObject = partsPrepareInGunGameObject;
		ItemCfg itemCfg = ItemCfg.Get(gunId);
		ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadWeapon_003Ec__AnonStorey._003C_003Em__0);
	}

	public static void SetWeaponGameObject(GameObject gunGo, List<int> partsInGunGameObject, List<int> partsPrepareInGunGameObject)
	{
		GunInfo component = gunGo.GetComponent<GunInfo>();
		if (!(component != null))
		{
			return;
		}
		for (int i = 0; i < partsPrepareInGunGameObject.Count; i++)
		{
			if (partsPrepareInGunGameObject[i] < 0 && partsInGunGameObject[i] > 0)
			{
				UnLoadPartGameObject(component, GetItemTypeByIndex(i));
			}
			if (partsPrepareInGunGameObject[i] > 0 && partsPrepareInGunGameObject[i] != partsInGunGameObject[i])
			{
				if (partsInGunGameObject[i] > 0)
				{
					UnLoadPartGameObject(component, GetItemTypeByIndex(i));
				}
				_LoadPartModel(component, partsPrepareInGunGameObject[i]);
				partsInGunGameObject[i] = partsPrepareInGunGameObject[i];
			}
		}
	}

	public static int GetItemTypeByIndex(int index)
	{
		switch (index)
		{
		case 0:
			return 18;
		case 1:
			return 19;
		case 2:
			return 20;
		case 3:
			return 14;
		case 4:
			return 19;
		default:
			return -1;
		}
	}

	public static void SetWeaponGameObject(GameObject gunGo, ICollection<int> parts)
	{
		GunInfo component = gunGo.GetComponent<GunInfo>();
		if (!(component != null))
		{
			return;
		}
		foreach (int part in parts)
		{
			if (part > 0)
			{
				_LoadPartModel(component, part);
			}
		}
	}

	public static void UnLoadPartGameObject(GunInfo gunInfo, int itemType)
	{
		Transform anchor = GetAnchor(itemType, gunInfo);
		if (anchor.childCount > 0)
		{
			Object.DestroyImmediate(anchor.GetChild(0).gameObject);
		}
	}

	private static void _LoadPartModel(GunInfo gunInfo, int itemId)
	{
		_003C_LoadPartModel_003Ec__AnonStorey7 _003C_LoadPartModel_003Ec__AnonStorey = new _003C_LoadPartModel_003Ec__AnonStorey7();
		_003C_LoadPartModel_003Ec__AnonStorey.gunInfo = gunInfo;
		_003C_LoadPartModel_003Ec__AnonStorey.partCfg = ItemCfg.Get(itemId);
		if (!string.IsNullOrEmpty(_003C_LoadPartModel_003Ec__AnonStorey.partCfg.modelPath))
		{
			ResMgr.Ins.CreateFromAB(_003C_LoadPartModel_003Ec__AnonStorey.partCfg.modelPath, null, _003C_LoadPartModel_003Ec__AnonStorey._003C_003Em__0);
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

	public static void OnDragHero(BasicGameObjectClass basicGameObject)
	{
		if ((bool)basicGameObject.basicGameObject)
		{
			if (UIEventListener.pointEventData.delta.x < -1f)
			{
				basicGameObject.basicGameObject.transform.Rotate(Vector3.up * 600f * Time.deltaTime, Space.World);
			}
			else if (UIEventListener.pointEventData.delta.x > 1f)
			{
				basicGameObject.basicGameObject.transform.Rotate(Vector3.up * -600f * Time.deltaTime, Space.World);
			}
		}
	}

	public static void OnDragWeaponHero(BasicGameObjectClass basicGameObject)
	{
		if ((bool)basicGameObject.basicGameObject)
		{
			if (UIEventListener.pointEventData.delta.x < -1f)
			{
				basicGameObject.basicGameObject.transform.parent.Rotate(Vector3.up * 600f * Time.deltaTime, Space.World);
			}
			else if (UIEventListener.pointEventData.delta.x > 1f)
			{
				basicGameObject.basicGameObject.transform.parent.Rotate(Vector3.up * -600f * Time.deltaTime, Space.World);
			}
		}
	}
}
