using System.Collections.Generic;
using UnityEngine;

public class CharacterSetSkin : MonoBehaviour
{
	private Dictionary<int, GameObject> _skinCombineDic = new Dictionary<int, GameObject>();

	private Dictionary<int, GameObject> _skinAttachDic = new Dictionary<int, GameObject>();

	private int m_boneArrayCurIndex;

	public GameObject Bones;

	private Dictionary<string, Transform> _boneTransforms = new Dictionary<string, Transform>();

	private bool _isShowAll = true;

	private Transform[] _bonesTransforms;

	private string[] _bonesTransformNames;

	private Transform _skinMeshRoot;

	public GameObject GetSkinGameObject(int basicType)
	{
		GameObject value;
		if (_skinCombineDic.TryGetValue(basicType, out value))
		{
			return value;
		}
		return null;
	}

	public void SetSkinAttach(int partIndex, string rootName, GameObject part)
	{
		if (!part)
		{
			Debug.LogError("part is null:rootName:" + rootName + "partIndex:" + partIndex);
			return;
		}
		if (string.IsNullOrEmpty(rootName))
		{
			Debug.LogError("rootName is null:");
			return;
		}
		Transform transform = GetTransform(Bones.transform, rootName);
		if (!transform)
		{
			Debug.LogError("root is null:");
			return;
		}
		part.transform.parent = transform;
		part.transform.localPosition = Vector3.zero;
		part.transform.localEulerAngles = Vector3.zero;
		part.transform.localScale = Vector3.one;
		part.SetActive(true);
		if (!_skinAttachDic.ContainsKey(partIndex))
		{
			_skinAttachDic.Add(partIndex, part);
		}
		else
		{
			_skinAttachDic[partIndex].SetActive(false);
			_skinAttachDic[partIndex] = part;
		}
		part.SetActive(_isShowAll);
	}

	public void RemoveAllSkin()
	{
		_skinCombineDic.Clear();
		_skinAttachDic.Clear();
	}

	public void DestroyAllSkin()
	{
		foreach (KeyValuePair<int, GameObject> item in _skinAttachDic)
		{
			Object.Destroy(item.Value);
		}
		foreach (KeyValuePair<int, GameObject> item2 in _skinCombineDic)
		{
			Object.Destroy(item2.Value);
		}
		_skinCombineDic.Clear();
		_skinAttachDic.Clear();
		_isShowAll = true;
	}

	public GameObject RemoveCombineSkin(int part)
	{
		GameObject gameObject = null;
		if (_skinAttachDic.ContainsKey(part))
		{
			gameObject = _skinAttachDic[part];
			gameObject.SetActive(false);
			_skinAttachDic.Remove(part);
			ShowSkins();
		}
		else if (_skinCombineDic.ContainsKey(part))
		{
			gameObject = _skinCombineDic[part];
			gameObject.SetActive(false);
			_skinCombineDic.Remove(part);
			ShowSkins();
		}
		return gameObject;
	}

	public void SetSkin(GameObject partGameObject, int part)
	{
		partGameObject.transform.parent = _skinMeshRoot;
		if (!_skinCombineDic.ContainsKey(part))
		{
			_skinCombineDic.Add(part, partGameObject);
		}
		else
		{
			_skinCombineDic[part].SetActive(false);
			_skinCombineDic[part] = partGameObject;
		}
		partGameObject.transform.localPosition = Vector3.zero;
		SkinnedMeshRenderer[] componentsInChildren = partGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer == null)
			{
				Debug.LogError("SkinnedMeshRenderer is null" + partGameObject.gameObject);
			}
			else
			{
				SetBones(skinnedMeshRenderer);
			}
		}
		ShowSkins();
	}

	public void HideAll()
	{
		foreach (KeyValuePair<int, GameObject> item in _skinAttachDic)
		{
			if ((bool)item.Value)
			{
				item.Value.SetActive(false);
			}
		}
		if ((bool)_skinMeshRoot)
		{
			_skinMeshRoot.gameObject.SetActive(false);
		}
		_isShowAll = false;
	}

	public void HideAllNoHideParent()
	{
		foreach (KeyValuePair<int, GameObject> item in _skinAttachDic)
		{
			item.Value.gameObject.GetComponentInChildren<MeshRenderer>(true).enabled = false;
		}
		foreach (KeyValuePair<int, GameObject> item2 in _skinCombineDic)
		{
			item2.Value.gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true).enabled = false;
		}
	}

	public void ShowAllNoHideParent()
	{
		foreach (KeyValuePair<int, GameObject> item in _skinAttachDic)
		{
			item.Value.gameObject.GetComponentInChildren<MeshRenderer>(true).enabled = true;
		}
		foreach (KeyValuePair<int, GameObject> item2 in _skinCombineDic)
		{
			item2.Value.gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true).enabled = true;
		}
	}

	public void HideSkin(int skin)
	{
		if (_skinAttachDic.ContainsKey(skin))
		{
			_skinAttachDic[skin].GetComponentInChildren<MeshRenderer>(true).enabled = false;
		}
		else if (_skinCombineDic.ContainsKey(skin))
		{
			_skinCombineDic[skin].GetComponentInChildren<SkinnedMeshRenderer>(true).enabled = false;
			ShowSkins();
		}
	}

	public void ShowAll()
	{
		foreach (KeyValuePair<int, GameObject> item in _skinAttachDic)
		{
			item.Value.gameObject.SetActiveBetter(true);
		}
		_skinMeshRoot.gameObject.SetActiveBetter(true);
		_isShowAll = true;
	}

	public void ShowSkin(int skin)
	{
		if (_skinAttachDic.ContainsKey(skin))
		{
			_skinAttachDic[skin].GetComponentInChildren<MeshRenderer>().enabled = true;
		}
		else if (_skinCombineDic.ContainsKey(skin))
		{
			_skinCombineDic[skin].GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
			ShowSkins();
		}
	}

	public bool ContainSkin(int skin)
	{
		if (_skinAttachDic.ContainsKey(skin))
		{
			return true;
		}
		if (_skinCombineDic.ContainsKey(skin))
		{
			return true;
		}
		return false;
	}

	public void OpenUpdateWhenOffscreen(int skin)
	{
		if (_skinCombineDic.ContainsKey(skin))
		{
			SkinnedMeshRenderer componentInChildren = _skinCombineDic[skin].GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.updateWhenOffscreen = true;
			}
		}
	}

	public void CloseUpdateWhenOffscreen(int skin)
	{
		if (_skinCombineDic.ContainsKey(skin))
		{
			SkinnedMeshRenderer componentInChildren = _skinCombineDic[skin].GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.updateWhenOffscreen = false;
			}
		}
	}

	public void SetBones(Transform boneRoot, Transform[] trans = null)
	{
		if (!boneRoot)
		{
			Debug.LogError("boneRoot is Null:");
			return;
		}
		boneRoot.SetParent(base.transform, false);
		Transform[] array = trans ?? boneRoot.GetComponentsInChildren<Transform>(true);
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!dictionary.ContainsKey(array[i].name))
			{
				dictionary.Add(array[i].name, array[i]);
			}
			else
			{
				Debug.LogError("transforms[i].name:" + array[i].name);
			}
		}
		foreach (KeyValuePair<int, GameObject> item in _skinCombineDic)
		{
			if ((bool)item.Value)
			{
				SkinnedMeshRenderer componentInChildren = item.Value.GetComponentInChildren<SkinnedMeshRenderer>(true);
				SetBone(componentInChildren, boneRoot, dictionary);
			}
		}
		foreach (KeyValuePair<int, GameObject> item2 in _skinAttachDic)
		{
			if ((bool)item2.Value)
			{
				Transform transform = GetTransform(Bones.transform, item2.Value.transform.parent.name);
				Transform transform2 = GetTransform(Bones.transform, item2.Value.transform.name);
				Transform transform3 = GetTransform(boneRoot, item2.Value.transform.parent.name);
				if (!transform)
				{
					Debug.LogError("old Bone AttachRoot is Null:" + item2.Value.transform.parent.name);
				}
				if (!transform3)
				{
					Debug.LogError("old Bone AttachRootnew is Null:" + item2.Value.transform.parent.name);
				}
				if ((bool)transform && (bool)transform2 && (bool)transform3)
				{
					transform2.SetParent(transform3, false);
				}
			}
		}
		Bones.SetActiveBetter(false);
		Bones = boneRoot.gameObject;
		Bones.SetActiveBetter(true);
		_boneTransforms = dictionary;
		_bonesTransforms = array;
	}

	public void SetBones(SkinnedMeshRenderer skinMeshRenderer)
	{
		if (!skinMeshRenderer)
		{
			Debug.LogError("skinMeshRenderer is Null:");
			return;
		}
		if (!Bones)
		{
			Bones = Utils.GetChildObjByName("Bip001", base.gameObject);
			if (!Bones)
			{
				Debug.LogError("Bones is Null:");
				return;
			}
		}
		if (_bonesTransforms == null)
		{
			_boneTransforms.Clear();
			_bonesTransforms = Bones.GetComponentsInChildren<Transform>();
			_bonesTransformNames = new string[_bonesTransforms.Length];
			for (int i = 0; i < _bonesTransforms.Length; i++)
			{
				if (!_boneTransforms.ContainsKey(_bonesTransforms[i].name))
				{
					_boneTransforms.Add(_bonesTransforms[i].name, _bonesTransforms[i]);
				}
				_bonesTransformNames[i] = _bonesTransforms[i].name;
			}
		}
		m_boneArrayCurIndex = 0;
		Transform[] bones = skinMeshRenderer.bones;
		int num = bones.Length;
		Transform[] array = new Transform[num];
		if (bones == null)
		{
			Debug.LogError("skinMeshRenderer.bones is null:" + skinMeshRenderer.name);
			return;
		}
		for (int j = 0; j < num; j++)
		{
			Transform value;
			if (!(bones[j] == null) && _boneTransforms.TryGetValue(bones[j].name, out value) && (bool)value && (bool)bones[j])
			{
				array[j] = _boneTransforms[bones[j].name];
			}
		}
		skinMeshRenderer.bones = array;
		skinMeshRenderer.rootBone = Bones.transform;
		skinMeshRenderer.allowOcclusionWhenDynamic = true;
		string text = skinMeshRenderer.name;
		if (text.Contains("head") || text.Contains("hair") || text.Contains("helmet") || text.Contains("hat"))
		{
			skinMeshRenderer.updateWhenOffscreen = true;
		}
	}

	private void ShowSkins()
	{
		SkinnedMeshRenderer component = _skinMeshRoot.GetComponent<SkinnedMeshRenderer>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		foreach (KeyValuePair<int, GameObject> item in _skinCombineDic)
		{
			if ((bool)item.Value)
			{
				item.Value.SetActiveBetter(true);
			}
		}
	}

	private int Get2Pow(int into)
	{
		int num = 1;
		for (int i = 0; i < 10; i++)
		{
			num *= 2;
			if (num > into)
			{
				break;
			}
		}
		return num;
	}

	private Transform GetTransform(Transform root, string name)
	{
		Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == name)
			{
				return transform;
			}
		}
		return null;
	}

	public void SetBone(SkinnedMeshRenderer skinMeshRenderer, Transform boneRoot, Dictionary<string, Transform> boneTransforms)
	{
		Transform[] bones = skinMeshRenderer.bones;
		int num = bones.Length;
		Transform[] array = new Transform[num];
		for (int i = 0; i < num; i++)
		{
			Transform value;
			if (boneTransforms.TryGetValue(bones[i].name, out value) && (bool)value && (bool)bones[i])
			{
				array[i] = boneTransforms[bones[i].name];
			}
		}
		skinMeshRenderer.bones = array;
		skinMeshRenderer.rootBone = boneRoot;
	}

	public void SetSkins(Dictionary<int, GameObject> baseDictionary)
	{
	}

	public void RemoveAttachSkin(int part, int part2 = 1)
	{
	}

	private void Awake()
	{
		_skinMeshRoot = new GameObject("SkinMeshRoot").transform;
		_skinMeshRoot.SetParent(base.transform);
		_skinMeshRoot.localScale = Vector3.one;
		_skinMeshRoot.localPosition = Vector3.zero;
	}
}
