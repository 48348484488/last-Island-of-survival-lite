using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public abstract class View : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CSetSpriteCommon_003Ec__AnonStorey2
	{
		internal Image image;

		internal ImageName imgName;

		internal bool isSetNativeSize;

		internal void _003C_003Em__0(UnityEngine.Object o)
		{
			if (image == null)
			{
				return;
			}
			if (o.name == imgName.imageName)
			{
				image.sprite = o as Sprite;
				if (isSetNativeSize)
				{
					image.SetNativeSize();
				}
			}
			image.enabled = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetSpriteABCommon_003Ec__AnonStorey3
	{
		internal Image image;

		internal string abPath;

		internal ImageName imgName;

		internal bool isSetNativeSize;

		internal void _003C_003Em__0(UnityEngine.Object o)
		{
			if (image == null)
			{
				return;
			}
			if (abPath == imgName.imageName)
			{
				imgName.CurImageName = abPath;
				image.sprite = o as Sprite;
				if (isSetNativeSize)
				{
					image.SetNativeSize();
				}
			}
			image.enabled = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetTexture_003Ec__AnonStorey4
	{
		internal RawImage rawImage;

		internal bool isSetNativeSize;

		internal void _003C_003Em__0(UnityEngine.Object o)
		{
			if (rawImage == null)
			{
				UnityEngine.Object.DestroyImmediate(o, true);
				return;
			}
			if (isSetNativeSize)
			{
				rawImage.SetNativeSize();
			}
			rawImage.texture = o as Texture;
			rawImage.enabled = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetTextureAsync_003Ec__AnonStorey5
	{
		internal RawImage rawImage;

		internal void _003C_003Em__0(UnityEngine.Object o)
		{
			rawImage.texture = o as Texture2D;
		}
	}

	[CompilerGenerated]
	private sealed class _003CDoHideAnim_003Ec__AnonStorey6
	{
		internal GameObject obj;

		internal GameObject canClickBtn;

		internal void _003C_003Em__0()
		{
			obj.SetActive(false);
			obj.transform.localPosition = new Vector3(0f, 0f, 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			if (canClickBtn != null)
			{
				canClickBtn.SetActive(true);
			}
		}
	}

	private static Dictionary<string, Sprite> headDic = new Dictionary<string, Sprite>();

	public static Dictionary<GameObject, long> headGos = new Dictionary<GameObject, long>();

	private static Sprite HeadDefaultSpt;

	public bool IsShow
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	protected virtual void onInit()
	{
	}

	protected virtual void onShow(object param = null, string childView = null)
	{
	}

	protected virtual void onHide(string childView = null)
	{
	}

	protected virtual void onDestroy()
	{
	}

	public void _DoInit()
	{
		try
		{
			onInit();
		}
		catch (Exception message)
		{
			Debug.LogError("[view]init view " + base.gameObject.name + " error");
			Debug.LogError(message);
		}
	}

	public void _DoShow(string childView, object param = null)
	{
		try
		{
			onShow(param, childView);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			Debug.LogError("[view]show view " + base.gameObject.name + " error.");
		}
	}

	public virtual void _SetRenderSort(int RendingSort)
	{
		Canvas component = GetComponent<Canvas>();
		if (component == null)
		{
			Debug.LogError(component.gameObject.name + "canvas is null");
			return;
		}
		Canvas component2 = base.transform.parent.GetComponent<Canvas>();
		component.pixelPerfect = false;
		component.overrideSorting = true;
		component.sortingOrder = RendingSort * 10 + component2.sortingOrder;
	}

	public virtual void OpenSortRenderSort(bool value)
	{
		Canvas component = GetComponent<Canvas>();
		if (component == null)
		{
			Debug.LogError(component.gameObject.name + "canvas is null");
		}
		else
		{
			component.overrideSorting = true;
		}
	}

	public void _DoHide(string childName)
	{
		try
		{
			onHide(childName);
		}
		catch (Exception message)
		{
			Debug.LogError("[view]hide view " + base.gameObject.name + " error.");
			Debug.LogError(message);
		}
	}

	public void _DoDestroy()
	{
		if (IsShow)
		{
			Hide();
		}
		try
		{
			onDestroy();
		}
		catch (Exception message)
		{
			Debug.LogError("[view]destroy view " + base.gameObject.name + " error.");
			Debug.LogError(message);
		}
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public void Hide()
	{
		ViewMgr.Ins.HideView(GetType().Name);
	}

	public void Destroy()
	{
		ViewMgr.Ins.Destroy(GetType().Name);
	}

	public static GameObject GetRelativePathGameObject(GameObject go, GameObject parent, GameObject child)
	{
		if (go == parent)
		{
			return child;
		}
		return go.transform.Find(Utils.GetRelativePath(parent, child)).gameObject;
	}

	public static T AddComponentIfNotExist<T>(GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<T>();
		}
		return val;
	}

	public static void BindOnClick(GameObject go, UIEventListener.VoidDelegate func)
	{
		UIEventListener.Get(go, string.Empty).onClick = func;
	}

	public static string GetLabelText(GameObject go)
	{
		Text component = go.GetComponent<Text>();
		if (!component)
		{
			return string.Empty;
		}
		return (!(component == null)) ? component.text : null;
	}

	public static string GetButtonText(GameObject go)
	{
		Text componentInChildren = go.GetComponentInChildren<Text>();
		if (!componentInChildren)
		{
			return string.Empty;
		}
		return (!(componentInChildren == null)) ? componentInChildren.text : null;
	}

	public static void SetButtonText(GameObject go, string text)
	{
		Text componentInChildren = go.GetComponentInChildren<Text>();
		if ((bool)componentInChildren)
		{
			componentInChildren.text = text;
		}
	}

	public static int GetDropDown(GameObject go)
	{
		Dropdown component = go.GetComponent<Dropdown>();
		return (!(component == null)) ? component.value : 0;
	}

	public static void SetDropDown(GameObject go, int value)
	{
		Dropdown component = go.GetComponent<Dropdown>();
		if ((bool)component)
		{
			component.value = value;
		}
	}

	public static void DropDownAddItem(GameObject go, string value)
	{
		Dropdown component = go.GetComponent<Dropdown>();
		if ((bool)component)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			optionData.text = value;
			component.options.Add(optionData);
		}
	}

	public static void SetLabelText(GameObject go, object text, bool isTranslate = true)
	{
		if (go == null)
		{
			return;
		}
		Text component = go.GetComponent<Text>();
		if (!component)
		{
			Debug.LogError(go.name + "not Have Text");
			return;
		}
		string text2 = ((!isTranslate) ? text.ToString() : Singleton<MultiLanguageMgr>.Ins.GetLanguage(text.ToString()));
		if (!(component.text == text2))
		{
			component.text = text2;
		}
	}

	public static void SetLabelText(Text label, object text, bool isTranslate = true)
	{
		if (!(label == null) && text != null)
		{
			string text2 = ((!isTranslate) ? text.ToString() : Singleton<MultiLanguageMgr>.Ins.GetLanguage(text.ToString()));
			if (!(label.text == text2))
			{
				label.text = text2;
			}
		}
	}

	public static void SetLabelText(GameObject go, object text, Color color)
	{
		Text component = go.GetComponent<Text>();
		if ((bool)component)
		{
			component.text = Singleton<MultiLanguageMgr>.Ins.GetLanguage(text.ToString());
			component.color = color;
		}
	}

	public static void SetLabelColor(GameObject go, Color color)
	{
		Text component = go.GetComponent<Text>();
		if ((bool)component)
		{
			component.color = color;
		}
	}

	public static void SetLabelColor(Text _text, Color color)
	{
		if ((bool)_text)
		{
			_text.color = color;
		}
	}

	public static void SetSlider(GameObject go, float progress)
	{
		Slider component = go.GetComponent<Slider>();
		if ((bool)component)
		{
			component.value = progress;
		}
	}

	public static void SetSlider(Slider slider, float progress)
	{
		if ((bool)slider)
		{
			slider.value = progress;
		}
	}

	public static void SetSlider(GameObject go, float cur, float max)
	{
		Slider component = go.GetComponent<Slider>();
		if ((bool)component)
		{
			if (max == 0f)
			{
				component.value = 1f;
			}
			else
			{
				component.value = cur / max;
			}
		}
	}

	public static void SetCheckbox(GameObject go, bool bChecked)
	{
		Toggle component = go.GetComponent<Toggle>();
		if ((bool)component)
		{
			component.isOn = bChecked;
		}
	}

	public static bool IsCheckboxChecked(GameObject go)
	{
		Toggle component = go.GetComponent<Toggle>();
		if (!component)
		{
			return false;
		}
		return component.isOn;
	}

	public static void SetInput(GameObject go, string value)
	{
		InputField component = go.GetComponent<InputField>();
		if ((bool)component)
		{
			component.text = value;
		}
	}

	public static string GetInputText(GameObject go)
	{
		InputField component = go.GetComponent<InputField>();
		if ((bool)component)
		{
			return component.text;
		}
		return string.Empty;
	}

	public static void SetInputText(GameObject go, string text)
	{
		InputField component = go.GetComponent<InputField>();
		if ((bool)component)
		{
			component.text = text;
		}
	}

	public static void SetProgressBar(GameObject go, float progress)
	{
		Image component = go.GetComponent<Image>();
		component.fillAmount = progress;
	}

	public static void SetImageColor(GameObject go, Color color)
	{
		Image component = go.GetComponent<Image>();
		component.color = color;
	}

	public static void SetImageColor(Image image, Color color)
	{
		if (image != null)
		{
			image.color = color;
		}
	}

	public static void SetSpriteCommon(string abPath, string name, GameObject go, bool isSetNativeSize = false)
	{
		_003CSetSpriteCommon_003Ec__AnonStorey2 _003CSetSpriteCommon_003Ec__AnonStorey = new _003CSetSpriteCommon_003Ec__AnonStorey2();
		_003CSetSpriteCommon_003Ec__AnonStorey.isSetNativeSize = isSetNativeSize;
		abPath = abPath.ToLower();
		_003CSetSpriteCommon_003Ec__AnonStorey.image = go.GetComponent<Image>();
		if (_003CSetSpriteCommon_003Ec__AnonStorey.image == null)
		{
			return;
		}
		_003CSetSpriteCommon_003Ec__AnonStorey.imgName = _003CSetSpriteCommon_003Ec__AnonStorey.image.gameObject.GetComponent<ImageName>();
		if (_003CSetSpriteCommon_003Ec__AnonStorey.imgName == null)
		{
			_003CSetSpriteCommon_003Ec__AnonStorey.imgName = _003CSetSpriteCommon_003Ec__AnonStorey.image.gameObject.AddComponent<ImageName>();
		}
		if (_003CSetSpriteCommon_003Ec__AnonStorey.image.sprite != null && _003CSetSpriteCommon_003Ec__AnonStorey.imgName.imageName == name)
		{
			if (!_003CSetSpriteCommon_003Ec__AnonStorey.image.enabled)
			{
				_003CSetSpriteCommon_003Ec__AnonStorey.image.enabled = true;
			}
			return;
		}
		_003CSetSpriteCommon_003Ec__AnonStorey.imgName.imageName = name;
		if (string.IsNullOrEmpty(name))
		{
			ResMgr.Ins.CleanRef(_003CSetSpriteCommon_003Ec__AnonStorey.image.gameObject);
			_003CSetSpriteCommon_003Ec__AnonStorey.image.sprite = null;
		}
		else
		{
			_003CSetSpriteCommon_003Ec__AnonStorey.image.enabled = false;
			ResMgr.Ins.LoadAssetFromAB<Sprite>(abPath, name, _003CSetSpriteCommon_003Ec__AnonStorey.image.gameObject, _003CSetSpriteCommon_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void SetSpriteABCommon(string abPath, GameObject go, bool isSetNativeSize = false)
	{
		_003CSetSpriteABCommon_003Ec__AnonStorey3 _003CSetSpriteABCommon_003Ec__AnonStorey = new _003CSetSpriteABCommon_003Ec__AnonStorey3();
		_003CSetSpriteABCommon_003Ec__AnonStorey.abPath = abPath;
		_003CSetSpriteABCommon_003Ec__AnonStorey.isSetNativeSize = isSetNativeSize;
		_003CSetSpriteABCommon_003Ec__AnonStorey.abPath = _003CSetSpriteABCommon_003Ec__AnonStorey.abPath.ToLower();
		_003CSetSpriteABCommon_003Ec__AnonStorey.image = go.GetComponent<Image>();
		if (_003CSetSpriteABCommon_003Ec__AnonStorey.image == null)
		{
			return;
		}
		_003CSetSpriteABCommon_003Ec__AnonStorey.imgName = _003CSetSpriteABCommon_003Ec__AnonStorey.image.gameObject.GetComponent<ImageName>();
		if (_003CSetSpriteABCommon_003Ec__AnonStorey.imgName == null)
		{
			_003CSetSpriteABCommon_003Ec__AnonStorey.imgName = _003CSetSpriteABCommon_003Ec__AnonStorey.image.gameObject.AddComponent<ImageName>();
		}
		if (_003CSetSpriteABCommon_003Ec__AnonStorey.image.sprite != null && string.Equals(_003CSetSpriteABCommon_003Ec__AnonStorey.imgName.CurImageName, _003CSetSpriteABCommon_003Ec__AnonStorey.abPath))
		{
			if (!_003CSetSpriteABCommon_003Ec__AnonStorey.image.enabled)
			{
				_003CSetSpriteABCommon_003Ec__AnonStorey.image.enabled = true;
			}
			return;
		}
		_003CSetSpriteABCommon_003Ec__AnonStorey.imgName.imageName = _003CSetSpriteABCommon_003Ec__AnonStorey.abPath;
		_003CSetSpriteABCommon_003Ec__AnonStorey.imgName.CurImageName = string.Empty;
		if (string.IsNullOrEmpty(_003CSetSpriteABCommon_003Ec__AnonStorey.abPath))
		{
			_003CSetSpriteABCommon_003Ec__AnonStorey.image.sprite = null;
			ResMgr.Ins.CleanRef(_003CSetSpriteABCommon_003Ec__AnonStorey.image.gameObject);
		}
		else
		{
			_003CSetSpriteABCommon_003Ec__AnonStorey.image.enabled = false;
			ResMgr.Ins.LoadAssetFromAB<Sprite>(_003CSetSpriteABCommon_003Ec__AnonStorey.abPath, null, _003CSetSpriteABCommon_003Ec__AnonStorey.image.gameObject, _003CSetSpriteABCommon_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void SetQualitySprite(GameObject go, string name)
	{
		SetSpriteABCommon("icon/pinzhi/" + name, go);
	}

	public static void SetShopTagSprite(GameObject go, string name)
	{
		SetSpriteCommon("icon/shop.ab", name, go);
	}

	public static void SetBigHeadSprite(GameObject go, string abPath)
	{
		SetSpriteABCommon("bigherohead/" + abPath + ".ab", go);
	}

	public static void SetFragSprite(GameObject go, string abPath)
	{
		SetSpriteCommon("icon/chip.ab", abPath, go);
	}

	public static void SetBuildPartSprite(GameObject go, string abPath)
	{
		SetSpriteCommon("icon/buildpart.ab", abPath, go);
	}

	public static void Attach(GameObject go, long id)
	{
		if (headGos.ContainsKey(go))
		{
			headGos[go] = id;
		}
		else
		{
			headGos.Add(go, id);
		}
	}

	public static long GetId(GameObject go)
	{
		return (!headGos.ContainsKey(go)) ? 0 : headGos[go];
	}

	public static void SetProductSprite(GameObject go, string name, bool isSetNativeSize = false)
	{
		SetSpriteCommon("icon/product.ab", name, go, isSetNativeSize);
	}

	public static void SetCommonSprite(GameObject go, string name)
	{
		SetSpriteCommon("icon/common1.ab", name, go);
	}

	public static void SetPhotoHead(GameObject go, string name)
	{
	}

	public static void SetSpriteAlpha(GameObject go, float value)
	{
		Image component = go.GetComponent<Image>();
		if ((bool)component)
		{
			component.color = new Color(component.color.r, component.color.g, component.color.b, value);
		}
	}

	public static void SetSpriteAsync(GameObject go, string abPath, bool isSetNativeSize = false)
	{
		SetSpriteABCommon(abPath, go, isSetNativeSize);
	}

	public static float GetUIWidth(GameObject go)
	{
		RectTransform component = go.GetComponent<RectTransform>();
		if (!component)
		{
			return 0f;
		}
		return component.rect.width;
	}

	public static float GetTextWidth(Transform go)
	{
		Text component = go.GetComponent<Text>();
		if (!component)
		{
			return 0f;
		}
		return component.preferredWidth;
	}

	public static void SetBattleGunSprite(GameObject go, string iconName)
	{
		SetSpriteABCommon("icon/" + iconName, go, true);
	}

	public static void SetItemSprite(GameObject go, string name, bool isSetNativeSize = false)
	{
		SetSpriteABCommon("icon/" + name, go, isSetNativeSize);
	}

	public static void SetQrCode(GameObject go)
	{
		string value = string.Empty;
		bool active = false;
		try
		{
			value = AndroidSDKInterface.Instance.GetPackageName();
		}
		catch (Exception)
		{
		}
		foreach (QrCodeCfg value2 in QrCodeCfg.GetAll().Values)
		{
			if (value2.packageName.Equals(value))
			{
				active = true;
				SetTexture(go, "texture/" + value2.icon);
				break;
			}
		}
		go.SetActive(active);
	}

	public static void SetSprite(GameObject go, Sprite sprite)
	{
		Image component = go.GetComponent<Image>();
		if ((bool)component)
		{
			component.enabled = true;
			component.sprite = sprite;
		}
	}

	public static void SetTexture(GameObject go, string abPath, bool isSetNativeSize = false)
	{
		_003CSetTexture_003Ec__AnonStorey4 _003CSetTexture_003Ec__AnonStorey = new _003CSetTexture_003Ec__AnonStorey4();
		_003CSetTexture_003Ec__AnonStorey.isSetNativeSize = isSetNativeSize;
		_003CSetTexture_003Ec__AnonStorey.rawImage = go.GetComponent<RawImage>();
		if (!(_003CSetTexture_003Ec__AnonStorey.rawImage == null))
		{
			if (string.IsNullOrEmpty(abPath))
			{
				_003CSetTexture_003Ec__AnonStorey.rawImage.texture = null;
				ResMgr.Ins.CleanRef(go);
			}
			else
			{
				_003CSetTexture_003Ec__AnonStorey.rawImage.enabled = false;
				ResMgr.Ins.LoadAssetFromAB<Texture>(abPath + ".ab", null, go, _003CSetTexture_003Ec__AnonStorey._003C_003Em__0);
			}
		}
	}

	public static void SetTexture(GameObject go, Texture tex)
	{
		RawImage component = go.GetComponent<RawImage>();
		if (!(component == null))
		{
			component.texture = tex;
		}
	}

	public static void SetTextureAsync(GameObject go, string abPath, string name)
	{
		_003CSetTextureAsync_003Ec__AnonStorey5 _003CSetTextureAsync_003Ec__AnonStorey = new _003CSetTextureAsync_003Ec__AnonStorey5();
		_003CSetTextureAsync_003Ec__AnonStorey.rawImage = go.GetComponent<RawImage>();
		if (!(_003CSetTextureAsync_003Ec__AnonStorey.rawImage == null))
		{
			if (string.IsNullOrEmpty(name))
			{
				_003CSetTextureAsync_003Ec__AnonStorey.rawImage.texture = null;
				ResMgr.Ins.CleanRef(go);
			}
			else
			{
				ResMgr.Ins.LoadAssetFromAB<Texture2D>(abPath, name, go, _003CSetTextureAsync_003Ec__AnonStorey._003C_003Em__0);
			}
		}
	}

	public static void DoHideAnim(GameObject obj, GameObject canClickBtn = null)
	{
		_003CDoHideAnim_003Ec__AnonStorey6 _003CDoHideAnim_003Ec__AnonStorey = new _003CDoHideAnim_003Ec__AnonStorey6();
		_003CDoHideAnim_003Ec__AnonStorey.obj = obj;
		_003CDoHideAnim_003Ec__AnonStorey.canClickBtn = canClickBtn;
		if (_003CDoHideAnim_003Ec__AnonStorey.canClickBtn != null)
		{
			_003CDoHideAnim_003Ec__AnonStorey.canClickBtn.SetActive(false);
		}
		_003CDoHideAnim_003Ec__AnonStorey.obj.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.5f);
		_003CDoHideAnim_003Ec__AnonStorey.obj.transform.DOLocalJump(new Vector3(-900f, 0f), 100f, 4, 2f).OnComplete(_003CDoHideAnim_003Ec__AnonStorey._003C_003Em__0);
	}

	public static void DoEnterAnim(GameObject obj)
	{
		obj.transform.localPosition = new Vector3(900f, 0f, 0f);
		obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		obj.SetActive(true);
		obj.transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
		obj.transform.DOLocalJump(new Vector3(0f, 0f), 100f, 4, 1.5f);
	}

	public static void DoEnterSaleAnim(GameObject go)
	{
		go.SetActive(true);
		go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		go.transform.DOScale(new Vector3(1f, 1f, 1f), 0.4f);
	}

	public static void DoDownToUpAnim(GameObject go)
	{
		go.transform.localPosition = new Vector3(0f, 1320f, 0f);
		go.transform.DOLocalMoveY(0f, 0.4f);
	}

	public static void DoUpToDownAnim(GameObject go)
	{
		go.transform.localPosition = new Vector3(0f, -1320f, 0f);
		go.transform.DOLocalMoveY(0f, 0.4f);
	}

	public static void DoLeftToRightAnim(GameObject go)
	{
		go.transform.localPosition = new Vector3(-900f, 0f, 0f);
		go.transform.DOLocalMoveX(0f, 0.4f);
	}

	public static void DoRightToLeftAnim(GameObject go)
	{
		go.transform.localPosition = new Vector3(900f, 0f, 0f);
		go.SetActive(true);
		go.transform.DOLocalMoveX(0f, 0.4f);
	}

	public static void DoFanzhuanAnim(GameObject go)
	{
		go.transform.DOFlip();
	}

	public IEnumerator DoMatchingAnim(GameObject go)
	{
		List<string> fonts = new List<string>
		{
			string.Empty,
			".",
			"..",
			"..."
		};
		int i = 0;
		while (true)
		{
			SetLabelText(go, fonts[i]);
			i++;
			yield return new WaitForSeconds(0.5f);
			if (i > 3)
			{
				i = 0;
			}
		}
	}

	public IEnumerator DoLoadingAnim(GameObject go)
	{
		List<string> fonts = new List<string> { "Loading", "Loading.", "Loading..", "Loading..." };
		int i = 0;
		while (true)
		{
			SetLabelText(go, fonts[i]);
			i++;
			yield return new WaitForSeconds(0.5f);
			if (i > 3)
			{
				i = 0;
			}
		}
	}

	protected virtual void Awake()
	{
		Singleton<MultiLanguageMgr>.Ins.SetMultiLanguage(base.gameObject);
	}
}
