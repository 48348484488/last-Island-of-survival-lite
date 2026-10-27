using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class ItemHead : MonoBehaviour
{
	public class ItemDescInfo
	{
		public DropMgr.DropDesInfo info;

		public Vector3 v3;

		public ItemDescInfo(DropMgr.DropDesInfo info, Vector3 v3)
		{
			this.info = info;
			this.v3 = v3;
		}
	}

	[CompilerGenerated]
	private sealed class _003CFill_003Ec__AnonStorey1
	{
		internal DropMgr.DropDesInfo info;

		internal ItemHead _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			ItemDescPanel.ShowItemDescPanel(info.itemId, info.num);
		}

		internal void _003C_003Em__1(GameObject go)
		{
			ItemDescPanel.ShowItemDescPanel(_0024this.itemInfo.itemId, _0024this.itemInfo.num);
		}
	}

	[CompilerGenerated]
	private sealed class _003CFillWordActivity_003Ec__AnonStorey2
	{
		internal DropMgr.DropDesInfo info;

		internal void _003C_003Em__0(GameObject go)
		{
			ItemDescPanel.ShowDescPanel(info.itemId, info.num, info.type);
		}
	}

	[CompilerGenerated]
	private sealed class _003CFill_003Ec__AnonStorey3<T>
	{
		internal DropMgr.DropDesInfo info;

		internal Utils.DataDelegate<T> dataDelegate;

		internal T arg;

		internal ItemHead _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			ItemDescPanel.ShowItemDescPanel(info.itemId, info.num);
		}

		internal void _003C_003Em__1(GameObject go)
		{
			_0024this.waitToShow = Utils.StartConroutine(_0024this.ForWait());
		}

		internal void _003C_003Em__2(GameObject go)
		{
			Utils.StopConroutine(_0024this.waitToShow);
			if (_0024this.isTriggerEvent && dataDelegate != null)
			{
				dataDelegate(arg);
			}
		}

		internal void _003C_003Em__3(GameObject go)
		{
			_0024this.isTriggerEvent = false;
			if (_0024this.isShowDesc)
			{
				_0024this.isShowDesc = false;
			}
			else
			{
				Utils.StopConroutine(_0024this.waitToShow);
			}
		}
	}

	private DropMgr.DropDesInfo itemInfo;

	private bool isShowDesc;

	private bool isTriggerEvent = true;

	private Coroutine waitToShow;

	public GameObject frame;

	public GameObject icon;

	public GameObject itemName;

	public GameObject num;

	public GameObject quality;

	public GameObject pressShowDesc;

	public GameObject useTime;

	public GameObject useTimeRoot;

	[CompilerGenerated]
	private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache2;

	public void Fill(DropMgr.DropDesInfo info, int numFormat = -1)
	{
		_003CFill_003Ec__AnonStorey1 _003CFill_003Ec__AnonStorey = new _003CFill_003Ec__AnonStorey1();
		_003CFill_003Ec__AnonStorey.info = info;
		_003CFill_003Ec__AnonStorey._0024this = this;
		itemInfo = _003CFill_003Ec__AnonStorey.info;
		if (frame != null && !string.IsNullOrEmpty(_003CFill_003Ec__AnonStorey.info.frame))
		{
			View.SetItemSprite(frame, _003CFill_003Ec__AnonStorey.info.frame);
		}
		if (icon != null)
		{
			View.SetItemSprite(icon, _003CFill_003Ec__AnonStorey.info.icon);
			DownUpListener.Get(icon).onDown = _003CFill_003Ec__AnonStorey._003C_003Em__0;
			DownUpListener downUpListener = DownUpListener.Get(icon);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CFill_003Em__0;
			}
			downUpListener.onUp = _003C_003Ef__am_0024cache0;
		}
		if (itemName != null)
		{
			int type = _003CFill_003Ec__AnonStorey.info.type;
			if (type == 7)
			{
				QualityCfg qualityCfg = QualityCfg.Get(_003CFill_003Ec__AnonStorey.info.quality);
				if (qualityCfg != null)
				{
					View.SetLabelText(itemName, string.Format(qualityCfg.color, Utils.GetString(_003CFill_003Ec__AnonStorey.info.name)));
				}
				else
				{
					View.SetLabelText(itemName, _003CFill_003Ec__AnonStorey.info.name);
				}
			}
			else
			{
				View.SetLabelText(itemName, _003CFill_003Ec__AnonStorey.info.name);
			}
		}
		if (num != null)
		{
			View.SetLabelText(num, (numFormat <= 0) ? _003CFill_003Ec__AnonStorey.info.num.ToString() : Utils.GetString(numFormat, _003CFill_003Ec__AnonStorey.info.num));
			num.SetActiveBetter(_003CFill_003Ec__AnonStorey.info.num > 0);
		}
		if (quality != null)
		{
			QualityCfg qualityCfg2 = QualityCfg.Get(_003CFill_003Ec__AnonStorey.info.quality);
			if (qualityCfg2 != null)
			{
				quality.SetActive(true);
				View.SetQualitySprite(quality, qualityCfg2.icon);
			}
			else
			{
				quality.SetActive(false);
			}
		}
		if (pressShowDesc != null)
		{
			DownUpListener.Get(pressShowDesc).onDown = _003CFill_003Ec__AnonStorey._003C_003Em__1;
			DownUpListener downUpListener2 = DownUpListener.Get(pressShowDesc);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003CFill_003Em__1;
			}
			downUpListener2.onUp = _003C_003Ef__am_0024cache1;
		}
		if (useTime != null)
		{
			if (_003CFill_003Ec__AnonStorey.info.useTime <= 0f)
			{
				useTimeRoot.SetActiveBetter(false);
				return;
			}
			useTimeRoot.SetActiveBetter(true);
			string useTimeString = Utils.GetUseTimeString((int)_003CFill_003Ec__AnonStorey.info.useTime);
			View.SetLabelText(useTime, useTimeString);
		}
	}

	public void FillWordActivity(DropMgr.DropDesInfo info, bool isEqualSign)
	{
		_003CFillWordActivity_003Ec__AnonStorey2 _003CFillWordActivity_003Ec__AnonStorey = new _003CFillWordActivity_003Ec__AnonStorey2();
		_003CFillWordActivity_003Ec__AnonStorey.info = info;
		if ((bool)frame)
		{
			frame.SetActiveBetter(!isEqualSign);
		}
		if ((bool)num)
		{
			num.SetActiveBetter(!isEqualSign);
			if (!string.IsNullOrEmpty(_003CFillWordActivity_003Ec__AnonStorey.info.frame))
			{
				num.SetActiveBetter(true);
				View.SetLabelText(num, _003CFillWordActivity_003Ec__AnonStorey.info.frame);
			}
			else
			{
				num.SetActiveBetter(false);
			}
		}
		if ((bool)icon)
		{
			View.SetItemSprite(icon, _003CFillWordActivity_003Ec__AnonStorey.info.icon);
			DownUpListener.Get(icon).onDown = _003CFillWordActivity_003Ec__AnonStorey._003C_003Em__0;
			DownUpListener downUpListener = DownUpListener.Get(icon);
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003CFillWordActivity_003Em__2;
			}
			downUpListener.onUp = _003C_003Ef__am_0024cache2;
		}
		if (quality != null)
		{
			QualityCfg qualityCfg = QualityCfg.Get(_003CFillWordActivity_003Ec__AnonStorey.info.quality);
			if (qualityCfg != null)
			{
				quality.SetActive(true);
				View.SetQualitySprite(quality, qualityCfg.icon);
			}
			else
			{
				quality.SetActive(false);
			}
		}
		if ((bool)useTime && (bool)useTimeRoot)
		{
			if (_003CFillWordActivity_003Ec__AnonStorey.info.useTime <= 0f)
			{
				useTimeRoot.SetActiveBetter(false);
				return;
			}
			useTimeRoot.SetActiveBetter(true);
			string useTimeString = Utils.GetUseTimeString((int)_003CFillWordActivity_003Ec__AnonStorey.info.useTime);
			View.SetLabelText(useTime, useTimeString);
		}
	}

	public void Fill<T>(DropMgr.DropDesInfo info, int numFormat = -1, Utils.DataDelegate<T> dataDelegate = null, T arg = default(T))
	{
		_003CFill_003Ec__AnonStorey3<T> _003CFill_003Ec__AnonStorey = new _003CFill_003Ec__AnonStorey3<T>();
		_003CFill_003Ec__AnonStorey.info = info;
		_003CFill_003Ec__AnonStorey.dataDelegate = dataDelegate;
		_003CFill_003Ec__AnonStorey.arg = arg;
		_003CFill_003Ec__AnonStorey._0024this = this;
		itemInfo = _003CFill_003Ec__AnonStorey.info;
		if (frame != null && _003CFill_003Ec__AnonStorey.info.frame != null)
		{
			View.SetItemSprite(frame, _003CFill_003Ec__AnonStorey.info.frame);
		}
		if (icon != null)
		{
			View.SetItemSprite(icon, _003CFill_003Ec__AnonStorey.info.icon, true);
			DownUpListener.Get(icon).onDown = _003CFill_003Ec__AnonStorey._003C_003Em__0;
			DownUpListener.Get(icon).onUp = _003CFill_00601_003Em__3<T>;
		}
		if (itemName != null)
		{
			View.SetLabelText(itemName, _003CFill_003Ec__AnonStorey.info.name);
		}
		if (num != null)
		{
			View.SetLabelText(num, (numFormat <= 0) ? _003CFill_003Ec__AnonStorey.info.num.ToString() : Utils.GetString(numFormat, _003CFill_003Ec__AnonStorey.info.num));
			num.SetActiveBetter(_003CFill_003Ec__AnonStorey.info.num > 0);
		}
		if (quality != null)
		{
			QualityCfg qualityCfg = QualityCfg.Get(_003CFill_003Ec__AnonStorey.info.quality);
			if (qualityCfg != null)
			{
				quality.SetActive(true);
				View.SetQualitySprite(quality, qualityCfg.icon);
			}
			else
			{
				quality.SetActive(false);
			}
		}
		if (pressShowDesc != null)
		{
			UIEventListener.Get(pressShowDesc, string.Empty).onDown = _003CFill_003Ec__AnonStorey._003C_003Em__1;
			UIEventListener.Get(pressShowDesc, string.Empty).onUp = _003CFill_003Ec__AnonStorey._003C_003Em__2;
			UIEventListener.Get(pressShowDesc, string.Empty).onExit = _003CFill_003Ec__AnonStorey._003C_003Em__3;
		}
		if (useTime != null)
		{
			if (_003CFill_003Ec__AnonStorey.info.useTime <= 0f)
			{
				useTimeRoot.SetActiveBetter(false);
				return;
			}
			useTimeRoot.SetActiveBetter(true);
			string useTimeString = Utils.GetUseTimeString((int)_003CFill_003Ec__AnonStorey.info.useTime);
			View.SetLabelText(useTime, useTimeString);
		}
	}

	private IEnumerator ForWait()
	{
		isTriggerEvent = true;
		isShowDesc = false;
		yield return Utils.WaitForSeconds(cfg.Consts.TIME_PRESS_SHOW_DESC);
		isShowDesc = true;
		isTriggerEvent = false;
		Vector3 v3 = new Vector3(pressShowDesc.transform.position.x, pressShowDesc.transform.position.y, 1f);
		ItemDescPanel.ShowItemDescPanel(itemInfo.itemId, itemInfo.num);
		Utils.StopConroutine(waitToShow);
	}

	public static void FillList(GameObject parentGo, List<DropMgr.DropDesInfo> list, bool isCenter = true, int width = 200, int numFormat = -1, float sale = 1f)
	{
		GameObject original = parentGo.transform.GetChild(0).gameObject;
		int num = ((parentGo.transform.childCount <= list.Count) ? list.Count : parentGo.transform.childCount);
		float[] array;
		if (isCenter)
		{
			array = GameConst.culcPos(list.Count, width);
		}
		else
		{
			array = new float[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = width * i;
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (j >= list.Count)
			{
				parentGo.transform.GetChild(j).gameObject.SetActive(false);
				continue;
			}
			if (j >= parentGo.transform.childCount)
			{
				GameObject gameObject = Object.Instantiate(original);
				gameObject.transform.parent = parentGo.transform;
				gameObject.transform.localScale = Vector3.one;
			}
			GameObject gameObject2 = parentGo.transform.GetChild(j).gameObject;
			gameObject2.transform.localPosition = new Vector3(array[j], 0f, 0f);
			DropMgr.DropDesInfo info = list[j];
			ItemHead component = gameObject2.GetComponent<ItemHead>();
			component.Fill(info, numFormat);
			gameObject2.transform.localScale = new Vector3(sale, sale, sale);
			gameObject2.SetActive(true);
		}
	}

	public static void ShowItems(List<ItemHead> list, GameObject item, List<DropMgr.DropDesInfo> items, float width = 100f)
	{
		if (items.Count <= 0)
		{
			return;
		}
		int num = items.Count - list.Count;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Object.Instantiate(item.gameObject);
				gameObject.transform.parent = item.transform.parent;
				gameObject.transform.localScale = Vector3.one;
				list.Add(gameObject.GetComponent<ItemHead>());
			}
		}
		if (num < 0)
		{
			for (int num2 = list.Count - 1; num2 > items.Count; num2--)
			{
				list[num2].gameObject.SetActive(false);
			}
		}
		int num3 = 0;
		foreach (DropMgr.DropDesInfo item2 in items)
		{
			list[num3].gameObject.SetActive(true);
			list[num3].transform.localPosition = new Vector3(width * (float)num3, 0f, 0f);
			if (item2 == null)
			{
				num3++;
				continue;
			}
			list[num3].Fill(item2);
			num3++;
		}
		item.gameObject.SetActive(false);
	}

	[CompilerGenerated]
	private static void _003CFill_003Em__0(GameObject go)
	{
		ItemDescPanel.HideItemDescPanel();
	}

	[CompilerGenerated]
	private static void _003CFill_003Em__1(GameObject go)
	{
		ItemDescPanel.HideItemDescPanel();
	}

	[CompilerGenerated]
	private static void _003CFillWordActivity_003Em__2(GameObject go)
	{
		ItemDescPanel.HideItemDescPanel();
	}

	[CompilerGenerated]
	private static void _003CFill_00601_003Em__3<T>(GameObject go)
	{
		ItemDescPanel.HideItemDescPanel();
	}
}
