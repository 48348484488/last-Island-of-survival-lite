using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPage : MonoBehaviour
{
	private ScrollRect rect;

	private List<float> _pagesPercent = new List<float>();

	[HideInInspector]
	public int CurrentPageIndex = -1;

	[Header("滑动速度")]
	public float SmoothSpeed = 0.05f;

	public int Height = 300;

	public int Width = 300;

	[Range(1f, 9f)]
	[Header("拖动灵敏度")]
	public int Sensitivity = 4;

	[HideInInspector]
	public int PageCount;

	public Action PageChangeAction;

	[Header("自动滑动时间")]
	public float AutoTime = 5f;

	private float _autoTime = 5f;

	[Header("需要自动滑动")]
	public bool NeedAuto;

	private float targethorizontal;

	public bool IsDraging;

	public Action<int, int> OnPageChanged;

	public Action<int> OnPageChangedSameOne;

	private float startime;

	private float delay = 0.1f;

	private float h = 5f;

	private bool isRight;

	private float _starDragPosX;

	private void Awake()
	{
		rect = base.transform.GetComponent<ScrollRect>();
		DragListener dragListener = DragListener.Get(base.gameObject);
		dragListener.onBeginDrag = (DragListener.VoidDelegate)Delegate.Combine(dragListener.onBeginDrag, new DragListener.VoidDelegate(OnBeginDrag));
		DragListener dragListener2 = DragListener.Get(base.gameObject);
		dragListener2.onEndDrag = (DragListener.VoidDelegate)Delegate.Combine(dragListener2.onEndDrag, new DragListener.VoidDelegate(OnEndDrag));
	}

	private void OnDestroy()
	{
		OnPageChanged = null;
		OnPageChangedSameOne = null;
	}

	private void Start()
	{
		startime = Time.time;
		InitLayout();
		UpdatePagesPercent();
		rect.horizontalNormalizedPosition = 0f;
		_autoTime = AutoTime;
	}

	private void Update()
	{
		if (NeedAuto)
		{
			if (_autoTime < 0f)
			{
				if (isRight)
				{
					ChangePage(CurrentPageIndex + 1);
				}
				else
				{
					ChangePage(CurrentPageIndex - 1);
				}
				if (CurrentPageIndex == PageCount - 1)
				{
					isRight = false;
				}
				else if (CurrentPageIndex == 0)
				{
					isRight = true;
				}
				_autoTime = AutoTime;
			}
			_autoTime -= Time.deltaTime;
		}
		if (!(Time.time < startime + delay))
		{
			UpdatePagesPercent();
			if (!IsDraging && _pagesPercent.Count > 0)
			{
				rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * SmoothSpeed);
			}
		}
	}

	public void OnBeginDrag(GameObject go)
	{
		_starDragPosX = rect.horizontalNormalizedPosition;
		IsDraging = true;
	}

	public void OnEndDrag(GameObject go)
	{
		float horizontalNormalizedPosition = rect.horizontalNormalizedPosition;
		if (horizontalNormalizedPosition - _starDragPosX > 1f / (float)(PageCount * Sensitivity))
		{
			ChangePage(CurrentPageIndex + 1);
		}
		else if (horizontalNormalizedPosition - _starDragPosX < -1f / (float)(PageCount * Sensitivity))
		{
			ChangePage(CurrentPageIndex - 1);
		}
		else
		{
			if (OnPageChangedSameOne != null)
			{
				OnPageChangedSameOne(CurrentPageIndex);
			}
			ChangePage(CurrentPageIndex);
		}
		if (CurrentPageIndex == -1)
		{
			ChangePage(0);
		}
		IsDraging = false;
	}

	public void ChangePage(int pageNum)
	{
		if (pageNum >= 0 && pageNum <= _pagesPercent.Count - 1 && pageNum != CurrentPageIndex)
		{
			if (pageNum > _pagesPercent.Count - 1)
			{
				pageNum = _pagesPercent.Count - 1;
			}
			if (pageNum < 0)
			{
				pageNum = 0;
			}
			CurrentPageIndex = pageNum;
			if (OnPageChanged != null)
			{
				OnPageChanged(_pagesPercent.Count, CurrentPageIndex);
			}
			targethorizontal = _pagesPercent[CurrentPageIndex];
			if (PageChangeAction != null)
			{
				PageChangeAction();
			}
			IsDraging = false;
		}
	}

	public void ChangePageForRefreshInfo(int pageNum)
	{
		if (pageNum >= 0 && pageNum <= _pagesPercent.Count - 1)
		{
			if (pageNum > _pagesPercent.Count - 1)
			{
				pageNum = _pagesPercent.Count - 1;
			}
			if (pageNum < 0)
			{
				pageNum = 0;
			}
			CurrentPageIndex = pageNum;
			targethorizontal = _pagesPercent[CurrentPageIndex];
			IsDraging = false;
		}
	}

	private void InitLayout()
	{
		PageCount = rect.content.childCount;
		for (int i = 0; i < PageCount; i++)
		{
			GameObject gameObject = rect.content.GetChild(i).gameObject;
			if (gameObject.activeSelf)
			{
				LayoutElement component = gameObject.GetComponent<LayoutElement>();
				if (component == null)
				{
					component = gameObject.AddComponent<LayoutElement>();
					component.preferredWidth = Width;
					component.preferredHeight = Height;
				}
			}
		}
	}

	private void UpdatePagesPercent()
	{
		int childCount = rect.content.childCount;
		int num = 0;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = rect.content.GetChild(i).gameObject;
			if (gameObject.activeSelf)
			{
				num++;
			}
		}
		childCount = num;
		if (_pagesPercent.Count == childCount)
		{
			return;
		}
		if (childCount != 0)
		{
			_pagesPercent.Clear();
			for (int j = 0; j < childCount; j++)
			{
				float item = 0f;
				if (childCount != 1)
				{
					item = (float)j / (float)(childCount - 1);
				}
				_pagesPercent.Add(item);
			}
		}
		OnEndDrag(null);
	}
}
