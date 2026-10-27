using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class GunSkin
{
	[CompilerGenerated]
	private sealed class _003CLoadWeapon_003Ec__AnonStorey0
	{
		internal float gunLength;

		internal ICollection<int> partsGameObject;

		internal GunSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if ((bool)_0024this._basicGameObject)
			{
				Object.DestroyImmediate(_0024this._basicGameObject);
			}
			go.transform.SetParent(_0024this._root.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			go.transform.localEulerAngles = Vector3.zero;
			_0024this._basicGameObject = go;
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
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
				go.transform.localPosition -= go.transform.InverseTransformPoint(componentInChildren.bounds.center) * num;
			}
			else
			{
				go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			}
			_0024this.SetWeaponPartGameObject(_0024this._basicGameObject, partsGameObject);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadPartModel_003Ec__AnonStorey1
	{
		internal ItemCfg partCfg;

		internal GunInfo gunInfo;

		internal GunSkin _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if (partCfg == null)
			{
				Object.Destroy(go);
				return;
			}
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			_0024this._partsLru.Set(partCfg.id, go);
			_0024this.EquipPart(partCfg, go, gunInfo);
		}
	}

	private GameObject _root;

	private GameObject _basicGameObject;

	private readonly LRU<int, GameObject> _partsLru = new LRU<int, GameObject>();

	private Dictionary<int, int> _nowParts = new Dictionary<int, int>();

	public GunSkin(GameObject root)
	{
		_root = root;
		_partsLru.onRemoveEntry = OnRemoveEntry;
	}

	private bool OnRemoveEntry(int key, GameObject value)
	{
		if (value.activeSelf)
		{
			return false;
		}
		Object.DestroyImmediate(value);
		return true;
	}

	public void LoadWeapon(int gunId, ICollection<int> partsGameObject, float gunLength = 0f)
	{
		_003CLoadWeapon_003Ec__AnonStorey0 _003CLoadWeapon_003Ec__AnonStorey = new _003CLoadWeapon_003Ec__AnonStorey0();
		_003CLoadWeapon_003Ec__AnonStorey.gunLength = gunLength;
		_003CLoadWeapon_003Ec__AnonStorey.partsGameObject = partsGameObject;
		_003CLoadWeapon_003Ec__AnonStorey._0024this = this;
		ItemCfg itemCfg = ItemCfg.Get(gunId);
		ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadWeapon_003Ec__AnonStorey._003C_003Em__0);
	}

	public void SetWeaponPartGameObject(GameObject gunGo, ICollection<int> parts)
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
				LoadPartModel(component, part);
			}
		}
	}

	public void UnLoadPartGameObject(int itemId)
	{
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

	public void LoadPartModel(GunInfo gunInfo, int itemId)
	{
		_003CLoadPartModel_003Ec__AnonStorey1 _003CLoadPartModel_003Ec__AnonStorey = new _003CLoadPartModel_003Ec__AnonStorey1();
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
		_partsLru.Set(itemCfg.id, part);
		Transform anchor = GetAnchor(itemCfg.type, gunInfo);
		part.transform.SetParent(anchor, false);
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

	public void Clear()
	{
		List<GameObject> values = _partsLru.GetValues();
		int i = 0;
		for (int count = values.Count; i < count; i++)
		{
			Object.DestroyImmediate(values[i], true);
		}
		if ((bool)_basicGameObject)
		{
			Object.DestroyImmediate(_basicGameObject, true);
		}
		_partsLru.Clear();
		_nowParts.Clear();
	}
}
