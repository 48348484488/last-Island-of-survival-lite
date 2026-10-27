using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class ViewMgr : MonoBehaviour
{
	public class ViewItemClass
	{
		public Type LoadingViewType;

		public object Param;

		public bool HideParent = true;
	}

	[CompilerGenerated]
	private sealed class _003CShowView_003Ec__AnonStorey3
	{
		internal bool hideParent;

		internal string parentType;

		internal string viewName;

		internal object param;

		internal ViewMgr _0024this;

		internal void _003C_003Em__0()
		{
			if (hideParent && parentType != null)
			{
				_0024this.DoHideView(parentType, viewName);
			}
			if (_0024this.mVisible.Contains(viewName))
			{
				_0024this.mStack.Add(viewName);
				_0024this.DoShowView(viewName, null, 0, param);
				_0024this.mLastViewName = viewName;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetLoadingView_003Ec__AnonStorey4<T>
	{
		internal Type viewType;

		internal ViewMgr _0024this;

		internal void _003C_003Em__0()
		{
			_0024this.mLoadingView = viewType;
		}
	}

	[CompilerGenerated]
	private sealed class _003CShowTopView_003Ec__AnonStorey5
	{
		internal string viewName;

		internal object param;

		internal ViewMgr _0024this;

		internal void _003C_003Em__0()
		{
			_0024this.DoShowView(viewName, null, 1, param);
		}
	}

	public static ViewMgr Ins;

	private const string VIEW_AB_PATH = "ui/";

	private const int VIEW_CACHE_CAPACITY = 6;

	private const int DefaultLayer = 0;

	private const int TopLayer = 1;

	private const int LayerNum = 2;

	public RectTransform CanvasTransfrom;

	public Vector2 CanvasSize = new Vector2(1920f, 1080f);

	private readonly List<Transform> layerTransforms = new List<Transform>();

	private readonly LRU<string, View> mViews = new LRU<string, View>(6);

	private readonly HashSet<string> mLoading = new HashSet<string>();

	private readonly HashSet<string> mVisible = new HashSet<string>();

	private readonly List<string> mStack = new List<string>();

	private string mLastViewName;

	private Type mLoadingView;

	private Utils.StringDelegate mOnShowEvent;

	private Utils.StringDelegate mOnHideEvent;

	private readonly Dictionary<string, Utils.VoidDelegate> mDicOnShowEvent = new Dictionary<string, Utils.VoidDelegate>();

	private readonly Dictionary<string, Utils.VoidDelegate> mDicOnHideEvent = new Dictionary<string, Utils.VoidDelegate>();

	public List<ViewItemClass> ViewItemList = new List<ViewItemClass>();

	public Camera UICamera { get; private set; }

	public Transform Canvas
	{
		get
		{
			return CanvasTransfrom;
		}
	}

	private void Awake()
	{
		Ins = this;
		CanvasTransfrom = base.transform as RectTransform;
		UnityEngine.Object.DontDestroyOnLoad(CanvasTransfrom);
		UICamera = UnityEngine.Object.FindObjectOfType<Camera>();
		mViews.onRemoveEntry = OnCacheOverflow;
	}

	private void Start()
	{
		for (int i = 0; i < 2; i++)
		{
			RectTransform rectTransform = new GameObject("Layer_" + i, typeof(RectTransform)).transform as RectTransform;
			rectTransform.SetParent(CanvasTransfrom, false);
			rectTransform.localPosition = Vector3.zero;
			rectTransform.localScale = Vector3.one;
			if (Utils.IsIphoneX())
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(2190f, 1100f);
			}
			else if (Utils.IsVivo())
			{
				Debug.LogError("IsVivoFullScreen");
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(2060f, 1080f);
			}
			else if (Utils.IsOppo())
			{
				Debug.LogError("IsOppoFullScreen");
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(2060f, 1080f);
			}
			else if (Utils.IsNeedChange())
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(2060f, 1080f);
			}
			else
			{
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.offsetMin = Vector2.zero;
				rectTransform.offsetMax = Vector2.zero;
			}
			layerTransforms.Add(rectTransform);
			Canvas canvas = rectTransform.gameObject.AddComponent<Canvas>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = i * 10000;
		}
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public static void Init(GameObject canvas)
	{
		canvas.AddComponent<ViewMgr>();
	}

	public Vector2 CanvasPosToScreenPos(Vector2 pos)
	{
		Vector2 canvasSize = Ins.CanvasSize;
		pos.x /= canvasSize.x;
		pos.y /= canvasSize.y;
		pos.x *= Utils.ScreenWidth;
		pos.y *= Utils.ScreenHeight;
		return pos;
	}

	public Vector2 ScreenPosToCanvasPos(Vector2 pos)
	{
		Vector2 canvasSize = Ins.CanvasSize;
		pos.x /= Utils.ScreenWidth;
		pos.y /= Utils.ScreenHeight;
		pos.x *= canvasSize.x;
		pos.y *= canvasSize.y;
		return pos;
	}

	public void addView(View view)
	{
		mViews.Set(view.name, view);
	}

	public T GetView<T>() where T : View
	{
		Type typeFromHandle = typeof(T);
		return (T)GetView(typeFromHandle.Name);
	}

	public View GetView(string name)
	{
		View value;
		mViews.TryGetValue(name, out value);
		return value;
	}

	public bool IsShow<T>()
	{
		Type typeFromHandle = typeof(T);
		return IsShow(typeFromHandle.Name);
	}

	public bool IsShow(string viewName)
	{
		return mVisible.Contains(viewName);
	}

	public Coroutine WaitHide<T>()
	{
		Type typeFromHandle = typeof(T);
		return StartCoroutine(DoWaitHide(typeFromHandle.Name));
	}

	public Coroutine WaitShow<T>()
	{
		Type typeFromHandle = typeof(T);
		return StartCoroutine(DoWaitShow(typeFromHandle.Name));
	}

	public void ShowView(Type viewType, object param = null, bool hideParent = true)
	{
		_003CShowView_003Ec__AnonStorey3 _003CShowView_003Ec__AnonStorey = new _003CShowView_003Ec__AnonStorey3();
		_003CShowView_003Ec__AnonStorey.hideParent = hideParent;
		_003CShowView_003Ec__AnonStorey.param = param;
		_003CShowView_003Ec__AnonStorey._0024this = this;
		_003CShowView_003Ec__AnonStorey.viewName = viewType.Name;
		if (!mVisible.Contains(_003CShowView_003Ec__AnonStorey.viewName))
		{
			mVisible.Add(_003CShowView_003Ec__AnonStorey.viewName);
			_003CShowView_003Ec__AnonStorey.parentType = mLastViewName;
			StartCoroutine(Load(viewType, _003CShowView_003Ec__AnonStorey._003C_003Em__0));
		}
	}

	public void JumpToView<T>(object param = null, bool hideParent = true)
	{
		HideAll();
		ShowView<T>(param, hideParent);
	}

	public void SetLoadingView<T>()
	{
		_003CSetLoadingView_003Ec__AnonStorey4<T> _003CSetLoadingView_003Ec__AnonStorey = new _003CSetLoadingView_003Ec__AnonStorey4<T>();
		_003CSetLoadingView_003Ec__AnonStorey._0024this = this;
		_003CSetLoadingView_003Ec__AnonStorey.viewType = typeof(T);
		StartCoroutine(Load(_003CSetLoadingView_003Ec__AnonStorey.viewType, _003CSetLoadingView_003Ec__AnonStorey._003C_003Em__0));
	}

	public void ShowView<T>(object param = null, bool hideParent = true)
	{
		Type typeFromHandle = typeof(T);
		ShowView(typeFromHandle, param, hideParent);
	}

	public void ShowTopView<T>(object param = null)
	{
		Type typeFromHandle = typeof(T);
		ShowTopView(typeFromHandle, param);
	}

	public void ShowTopView(Type viewType, object param = null)
	{
		_003CShowTopView_003Ec__AnonStorey5 _003CShowTopView_003Ec__AnonStorey = new _003CShowTopView_003Ec__AnonStorey5();
		_003CShowTopView_003Ec__AnonStorey.param = param;
		_003CShowTopView_003Ec__AnonStorey._0024this = this;
		_003CShowTopView_003Ec__AnonStorey.viewName = viewType.Name;
		if (!mVisible.Contains(_003CShowTopView_003Ec__AnonStorey.viewName))
		{
			mVisible.Add(_003CShowTopView_003Ec__AnonStorey.viewName);
			StartCoroutine(Load(viewType, _003CShowTopView_003Ec__AnonStorey._003C_003Em__0));
		}
	}

	public void HideView(Type viewType)
	{
		HideView(viewType.Name);
	}

	public void HideView(string name)
	{
		if (!mVisible.Contains(name))
		{
			return;
		}
		DoHideView(name, null);
		if (IsContainView(name))
		{
			RemoveView(name);
			ShowViewInList();
			mStack.RemoveAt(mStack.Count - 1);
		}
		else
		{
			if (mStack.Count <= 0 || !(mStack[mStack.Count - 1] == name))
			{
				return;
			}
			mStack.RemoveAt(mStack.Count - 1);
			if (mStack.Count > 0)
			{
				string text = mStack[mStack.Count - 1];
				if (!IsShow(text))
				{
					mVisible.Add(text);
					DoShowView(text, name, 0);
				}
				mLastViewName = text;
			}
		}
	}

	public void HideView<T>() where T : View
	{
		Type typeFromHandle = typeof(T);
		HideView(typeFromHandle.Name);
	}

	public void HideAll()
	{
		mStack.Clear();
		List<string> list = new List<string>(mVisible);
		foreach (string item in list)
		{
			HideView(item);
		}
		mVisible.Clear();
		mLastViewName = null;
	}

	public void Reset()
	{
		HideAll();
		List<View> values = mViews.GetValues();
		foreach (View item in values)
		{
			if ((bool)item)
			{
				Destroy(item.name);
			}
		}
		mViews.Clear();
		mLoading.Clear();
		mVisible.Clear();
		mStack.Clear();
		mLastViewName = null;
		GC.Collect();
	}

	public void Destroy<T>()
	{
		Type typeFromHandle = typeof(T);
		Destroy(typeFromHandle.Name);
	}

	public void Destroy(string viewName)
	{
		View value;
		mViews.TryGetValue(viewName, out value);
		if ((bool)value)
		{
			value._DoDestroy();
			mViews.Remove(viewName);
		}
		mVisible.Remove(viewName);
	}

	public void AddOnShowEvent(Utils.StringDelegate func)
	{
		mOnShowEvent = (Utils.StringDelegate)Delegate.Combine(mOnShowEvent, func);
	}

	public void RemoveOnShowEvent(Utils.StringDelegate func)
	{
		if (mOnShowEvent != null)
		{
			mOnShowEvent = (Utils.StringDelegate)Delegate.Remove(mOnShowEvent, func);
		}
	}

	public void AddOnHideEvent(Utils.StringDelegate func)
	{
		mOnHideEvent = (Utils.StringDelegate)Delegate.Combine(mOnHideEvent, func);
	}

	public void RemoveOnHideEvent(Utils.StringDelegate func)
	{
		if (mOnHideEvent != null)
		{
			mOnHideEvent = (Utils.StringDelegate)Delegate.Remove(mOnHideEvent, func);
		}
	}

	public void AddOnShowEvent<T>(Utils.VoidDelegate func) where T : View
	{
		Type typeFromHandle = typeof(T);
		if (mDicOnShowEvent.ContainsKey(typeFromHandle.Name))
		{
			Dictionary<string, Utils.VoidDelegate> dictionary;
			string key;
			(dictionary = mDicOnShowEvent)[key = typeFromHandle.Name] = (Utils.VoidDelegate)Delegate.Combine(dictionary[key], func);
		}
		else
		{
			mDicOnShowEvent[typeFromHandle.Name] = func;
		}
	}

	public void AddOnShowEvent(string viewName, Utils.VoidDelegate func)
	{
		if (mDicOnShowEvent.ContainsKey(viewName))
		{
			Dictionary<string, Utils.VoidDelegate> dictionary;
			string key;
			(dictionary = mDicOnShowEvent)[key = viewName] = (Utils.VoidDelegate)Delegate.Combine(dictionary[key], func);
		}
		else
		{
			mDicOnShowEvent[viewName] = func;
		}
	}

	public void AddOnHideEvent<T>(Utils.VoidDelegate func) where T : View
	{
		Type typeFromHandle = typeof(T);
		if (mDicOnHideEvent.ContainsKey(typeFromHandle.Name))
		{
			Dictionary<string, Utils.VoidDelegate> dictionary;
			string key;
			(dictionary = mDicOnHideEvent)[key = typeFromHandle.Name] = (Utils.VoidDelegate)Delegate.Combine(dictionary[key], func);
		}
		else
		{
			mDicOnHideEvent[typeFromHandle.Name] = func;
		}
	}

	public void RemoveOnShowEvent(string viewName, Utils.VoidDelegate func)
	{
		if (mDicOnShowEvent.ContainsKey(viewName))
		{
			Dictionary<string, Utils.VoidDelegate> dictionary;
			string key;
			(dictionary = mDicOnShowEvent)[key = viewName] = (Utils.VoidDelegate)Delegate.Remove(dictionary[key], func);
		}
	}

	public void RemoveOnHideEvent(string viewName, Utils.VoidDelegate func)
	{
		if (mDicOnHideEvent.ContainsKey(viewName))
		{
			Dictionary<string, Utils.VoidDelegate> dictionary;
			string key;
			(dictionary = mDicOnHideEvent)[key = viewName] = (Utils.VoidDelegate)Delegate.Remove(dictionary[key], func);
		}
	}

	public void SetLayerVisible(int index, bool visible)
	{
		layerTransforms[index].gameObject.SetActive(visible);
	}

	public bool IsScriptView(string view)
	{
		return false;
	}

	private IEnumerator Load(Type viewType, Utils.VoidDelegate onFinished)
	{
		_003CLoad_003Ec__Iterator0._003CLoad_003Ec__AnonStorey6 _003CLoad_003Ec__AnonStorey = new _003CLoad_003Ec__Iterator0._003CLoad_003Ec__AnonStorey6();
		_003CLoad_003Ec__AnonStorey._003C_003Ef__ref_00240 = this;
		string viewName = viewType.Name;
		View view2 = null;
		if (mViews.TryGetValue(viewName, out view2))
		{
			onFinished();
			yield break;
		}
		if (mLoadingView != null)
		{
			ShowTopView(mLoadingView);
		}
		while (mLoading.Contains(viewName))
		{
			yield return null;
			if (mViews.TryGetValue(viewName, out view2))
			{
				onFinished();
				yield break;
			}
		}
		string relativePath = "ui/" + viewName.ToLower() + ".ab";
		mLoading.Add(viewName);
		_003CLoad_003Ec__AnonStorey.root = null;
		yield return ResMgr.Ins.CreateFromAB(relativePath, viewName, _003CLoad_003Ec__AnonStorey._003C_003Em__0);
		mLoading.Remove(viewName);
		_003CLoad_003Ec__AnonStorey.root.name = viewName;
		view2 = (View)_003CLoad_003Ec__AnonStorey.root.AddComponent(viewType);
		mViews.Set(viewName, view2);
		_003CLoad_003Ec__AnonStorey.root.SetActive(false);
		view2._DoInit();
		onFinished();
		if (mLoadingView != null)
		{
			HideView(mLoadingView);
		}
	}

	private IEnumerator DoWaitShow(string viewName)
	{
		while (!IsShow(viewName))
		{
			yield return null;
		}
	}

	private void DoShowView(string viewName, string childView, int layer, object param = null)
	{
		if (!mVisible.Contains(viewName))
		{
			return;
		}
		View value = null;
		if (mViews.TryGetValue(viewName, out value) && !value.gameObject.activeSelf)
		{
			Transform transform = value.transform;
			Transform parent = layerTransforms[layer];
			transform.SetParent(parent, false);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			int num = -1;
			if (mStack.Contains(viewName))
			{
				num = mStack.LastIndexOf(viewName);
			}
			value._SetRenderSort((num >= 0) ? num : 0);
			transform.SetAsLastSibling();
			value.gameObject.SetActive(true);
			value.OpenSortRenderSort(true);
			value._DoShow(childView, param);
			NotifyOnShowEvent(viewName);
		}
	}

	private void DoHideView(string viewName, string childname)
	{
		mVisible.Remove(viewName);
		View value = null;
		if (mViews.TryGetValue(viewName, out value))
		{
			value.gameObject.SetActive(false);
			value._DoHide(childname);
			NotifyOnHideEvent(viewName);
		}
	}

	private IEnumerator DoWaitHide(string viewName)
	{
		while (IsShow(viewName))
		{
			yield return null;
		}
	}

	private void NotifyOnShowEvent(string viewName)
	{
		if (mDicOnShowEvent.ContainsKey(viewName) && mDicOnShowEvent[viewName] != null)
		{
			mDicOnShowEvent[viewName]();
		}
		if (mOnShowEvent != null)
		{
			mOnShowEvent(viewName);
		}
	}

	private void NotifyOnHideEvent(string viewName)
	{
		if (mDicOnHideEvent.ContainsKey(viewName) && mDicOnHideEvent[viewName] != null)
		{
			mDicOnHideEvent[viewName]();
		}
		if (mOnHideEvent != null)
		{
			mOnHideEvent(viewName);
		}
	}

	private bool OnCacheOverflow(string t, View v)
	{
		if (v == null)
		{
			return false;
		}
		if (v.IsShow)
		{
			return false;
		}
		if (v.GetType() == mLoadingView)
		{
			return false;
		}
		if (mStack.Contains(t))
		{
			return false;
		}
		mVisible.Remove(t);
		mLoading.Remove(t);
		v._DoDestroy();
		return true;
	}

	public void Show<T>(object Param, bool hideParent = true)
	{
	}

	public void AddView<T>(object Param, bool hideParent = true)
	{
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType == typeFromHandle)
			{
				ViewItemList.RemoveAt(i);
				break;
			}
		}
		ViewItemClass viewItemClass = new ViewItemClass();
		viewItemClass.LoadingViewType = typeFromHandle;
		viewItemClass.Param = Param;
		viewItemClass.HideParent = hideParent;
		ViewItemList.Add(viewItemClass);
		if (ViewItemList.Count < 2)
		{
			ShowViewInList();
		}
	}

	public void RemoveView<T>()
	{
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType == typeFromHandle)
			{
				ViewItemList.RemoveAt(i);
				break;
			}
		}
	}

	public void RemoveView(string viewName)
	{
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType.Name == viewName)
			{
				ViewItemList.RemoveAt(i);
				break;
			}
		}
	}

	public ViewItemClass GetFirstView<T>(object Param, bool hideParent = true)
	{
		if (ViewItemList.Count > 0)
		{
			return ViewItemList[0];
		}
		return null;
	}

	public ViewItemClass GetView<T>(object Param, bool hideParent = true)
	{
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType == typeFromHandle)
			{
				return ViewItemList[i];
			}
		}
		return null;
	}

	public bool IsContainView<T>()
	{
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType == typeFromHandle)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsContainView(string viewName)
	{
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType.Name == viewName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsContainView(Type viewType)
	{
		for (int i = 0; i < ViewItemList.Count; i++)
		{
			if (ViewItemList[i].LoadingViewType == viewType)
			{
				return true;
			}
		}
		return false;
	}

	public void ShowViewInList()
	{
		if (ViewItemList.Count > 0)
		{
			ViewItemClass viewItemClass = ViewItemList[0];
			ShowView(viewItemClass.LoadingViewType, viewItemClass.Param, viewItemClass.HideParent);
		}
	}
}
