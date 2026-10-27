using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollPanel : MonoBehaviour
{
	public delegate void FillCell(GameObject cell, int index);

	public delegate void CleanCell(GameObject cell);

	private bool mHorizontal;

	private Rect mRect;

	private RectTransform mContentTransform;

	private GameObject mCell;

	private Vector2 mCellInitPos;

	private List<string> m_cellsName = new List<string> { "m_cell", "m_cell_0", "m_cell_1", "m_cell_2", "m_cell_3", "m_cell_4", "m_cell_5" };

	private FillCell mFillFunc;

	private CleanCell mCleanFunc;

	private readonly SortedDictionary<int, GameObject> mIndex2Cell = new SortedDictionary<int, GameObject>();

	private readonly LinkedList<GameObject> mCellObjCache = new LinkedList<GameObject>();

	public Vector2 CellSize = new Vector2(100f, 100f);

	private int mCellMax;

	public int rows = 1;

	public int cols = 1;

	private int mLineNum;

	private int mLastLineBegin = -1;

	private int mLastLineEnd = -1;

	public bool addTestCell;

	public int addTestCellNum = 5;

	public float AddContentHeight;

	private float _offsetY;

	private void Awake()
	{
		mHorizontal = GetComponent<ScrollRect>().horizontal;
		mRect = (base.transform as RectTransform).rect;
		mContentTransform = base.transform.Find("content") as RectTransform;
		if (mContentTransform == null)
		{
			Debug.LogError("[UIScrollPanel]must have a content gameobject");
			return;
		}
		Transform transform = null;
		int i = 0;
		for (int count = m_cellsName.Count; i < count; i++)
		{
			string n = m_cellsName[i];
			transform = mContentTransform.Find(n);
			if (transform != null)
			{
				break;
			}
		}
		if (transform == null)
		{
			Debug.LogError("[UIScrollPanel]must have a m_cell gameobject");
			return;
		}
		mCell = transform.gameObject;
		mCellInitPos = transform.localPosition;
		mCellObjCache.AddLast(mCell);
	}

	private void AdjustResolution()
	{
		float num = 1920f;
		float num2 = 1080f;
		if (Screen.width <= 1920)
		{
			float num3 = num / (float)Utils.ScreenWidth;
			_offsetY = (float)Utils.ScreenHeight * num3 - num2;
			RectTransform component = GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, component.sizeDelta.y + _offsetY);
			RectTransform component2 = GetComponent<ScrollRect>().content.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(component2.sizeDelta.x, component2.sizeDelta.y + _offsetY);
		}
	}

	public void Reset(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
	{
		Clear();
		SetCellNum(cellNum);
		mFillFunc = fillFunc;
		mCleanFunc = cleanFunc;
		mLastLineBegin = (mLastLineEnd = -1);
		ResetDragPostion();
		Update();
	}

	public void ResetNoPos(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
	{
		SetCellNum(cellNum);
		mFillFunc = fillFunc;
		mCleanFunc = cleanFunc;
		mLastLineBegin = (mLastLineEnd = -1);
		Update();
	}

	public void ResetNoPosClear(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
	{
		Clear();
		ResetNoPos(cellNum, fillFunc, cleanFunc);
	}

	public void ResetNoPosClearToEnd(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
	{
		Clear();
		ResetNoPos(cellNum, fillFunc, cleanFunc);
		int lineBegin;
		int lineEnd;
		CalVisibleRange(out lineBegin, out lineEnd);
		if (getCountPerRowOrCol() * lineEnd >= cellNum)
		{
			ResetEnd();
		}
	}

	public void ResetEnd()
	{
		mContentTransform.anchoredPosition = new Vector2(mContentTransform.anchoredPosition.x, mContentTransform.sizeDelta.y - mRect.height);
	}

	public void ResetEnd_OutExpo()
	{
		Vector2 endValue = new Vector2(mContentTransform.anchoredPosition.x, mContentTransform.sizeDelta.y - mRect.height);
		Tween t = mContentTransform.DoAnchoredPosition(endValue, 0.5f);
		t.SetEase(Ease.OutExpo);
	}

	public void ResetDragPostion()
	{
		Vector2 anchoredPosition = mContentTransform.anchoredPosition;
		if (mHorizontal)
		{
			anchoredPosition.x = 0f;
		}
		else
		{
			anchoredPosition.y = 0f;
		}
		mContentTransform.anchoredPosition = anchoredPosition;
	}

	public void UpdateAllCell(FillCell func)
	{
		foreach (KeyValuePair<int, GameObject> item in mIndex2Cell)
		{
			try
			{
				func(item.Value, item.Key);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public void UpdateCell(int index, FillCell func)
	{
		GameObject value = null;
		if (mIndex2Cell.TryGetValue(index, out value))
		{
			try
			{
				func(value, index);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public void Clear()
	{
		mCellMax = 0;
		mLineNum = 0;
		foreach (KeyValuePair<int, GameObject> item in mIndex2Cell)
		{
			GameObject value = item.Value;
			mCellObjCache.AddLast(value);
			if (mCleanFunc != null)
			{
				try
				{
					mCleanFunc(value);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			value.SetActiveBetter(false);
		}
		mIndex2Cell.Clear();
	}

	private void Rerange()
	{
		if (mCellMax <= 0)
		{
			return;
		}
		int lineBegin = 0;
		int lineEnd = 0;
		CalVisibleRange(out lineBegin, out lineEnd);
		if (mLastLineBegin == lineBegin && mLastLineEnd == lineEnd)
		{
			return;
		}
		int countPerRowOrCol = getCountPerRowOrCol();
		for (int i = mLastLineBegin; i <= mLastLineEnd; i++)
		{
			if (i >= lineBegin && i <= lineEnd)
			{
				continue;
			}
			for (int j = 0; j < countPerRowOrCol; j++)
			{
				int key = i * countPerRowOrCol + j;
				GameObject value;
				if (!mIndex2Cell.TryGetValue(key, out value))
				{
					continue;
				}
				mCellObjCache.AddLast(value);
				mIndex2Cell.Remove(key);
				if (mCleanFunc != null)
				{
					try
					{
						mCleanFunc(value);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
				value.SetActiveBetter(false);
			}
		}
		for (int k = lineBegin; k <= lineEnd; k++)
		{
			if (k >= mLastLineBegin && k <= mLastLineEnd)
			{
				continue;
			}
			for (int l = 0; l < countPerRowOrCol; l++)
			{
				int num = k * countPerRowOrCol + l;
				GameObject value2;
				mIndex2Cell.TryGetValue(num, out value2);
				if (num >= mCellMax)
				{
					if (!(value2 != null))
					{
						continue;
					}
					mCellObjCache.AddLast(value2);
					mIndex2Cell.Remove(num);
					if (mCleanFunc != null)
					{
						try
						{
							mCleanFunc(value2);
						}
						catch (Exception exception2)
						{
							Debug.LogException(exception2);
						}
					}
					value2.SetActiveBetter(false);
					continue;
				}
				if (value2 == null)
				{
					value2 = MakeCell();
					mIndex2Cell.Add(num, value2);
				}
				value2.transform.SetParent(mContentTransform, false);
				value2.SetActiveBetter(true);
				if (mFillFunc != null)
				{
					try
					{
						mFillFunc(value2, num);
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
					}
				}
			}
		}
		mLastLineBegin = lineBegin;
		mLastLineEnd = lineEnd;
		RangeCells();
	}

	public void MoveTo(int index)
	{
		Vector2 anchoredPosition = mContentTransform.anchoredPosition;
		if (mHorizontal)
		{
			anchoredPosition.x = CellSize.x * (float)index;
		}
		else
		{
			anchoredPosition.y = CellSize.y * (float)index;
		}
		mContentTransform.anchoredPosition = anchoredPosition;
	}

	public void MoveTo(int index, float offset)
	{
		Vector2 anchoredPosition = mContentTransform.anchoredPosition;
		if (mHorizontal)
		{
			anchoredPosition.x = CellSize.x * (float)index;
			return;
		}
		int num = index / cols - 1;
		if (num >= 0)
		{
			anchoredPosition.y = CellSize.y * (float)num + offset;
		}
	}

	public void MoveToIndexNotSingleLine(int index)
	{
		Vector2 zero = Vector2.zero;
		int num = getCountPerRowOrCol();
		if (num <= 0)
		{
			num = 1;
		}
		if (mHorizontal)
		{
			zero.x = CellSize.x * (float)(index / num);
		}
		else
		{
			zero.y = CellSize.y * (float)(index / num);
		}
		mContentTransform.anchoredPosition = zero;
	}

	private void SetCellNum(int num)
	{
		if (mCellMax != num)
		{
			mCellMax = num;
			if (mCellMax < 0)
			{
				mCellMax = 0;
			}
			if (mCellMax == 0)
			{
				Clear();
			}
			int countPerRowOrCol = getCountPerRowOrCol();
			mLineNum = (mCellMax - 1) / countPerRowOrCol + 1;
			if (mHorizontal)
			{
				mContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CellSize.x * (float)mLineNum + mCellInitPos.x);
			}
			else
			{
				mContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CellSize.y * (float)mLineNum - mCellInitPos.y + AddContentHeight);
			}
		}
	}

	private void RangeCells()
	{
		if (mHorizontal)
		{
			foreach (KeyValuePair<int, GameObject> item in mIndex2Cell)
			{
				int num = item.Key / rows;
				int num2 = item.Key % rows;
				GameObject value = item.Value;
				RectTransform rectTransform = value.transform as RectTransform;
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.x = mCellInitPos.x + (float)num * CellSize.x;
				anchoredPosition.y = mCellInitPos.y - (float)num2 * CellSize.y;
				rectTransform.anchoredPosition = anchoredPosition;
			}
			return;
		}
		foreach (KeyValuePair<int, GameObject> item2 in mIndex2Cell)
		{
			int num3 = item2.Key / cols;
			int num4 = item2.Key % cols;
			GameObject value2 = item2.Value;
			RectTransform rectTransform2 = value2.transform as RectTransform;
			Vector2 anchoredPosition2 = rectTransform2.anchoredPosition;
			anchoredPosition2.x = mCellInitPos.x + (float)num4 * CellSize.x;
			anchoredPosition2.y = mCellInitPos.y - (float)num3 * CellSize.y;
			rectTransform2.anchoredPosition = anchoredPosition2;
		}
	}

	private GameObject MakeCell()
	{
		if (mCellObjCache.Count > 0)
		{
			GameObject value = mCellObjCache.First.Value;
			mCellObjCache.RemoveFirst();
			return value;
		}
		return UnityEngine.Object.Instantiate(mCell);
	}

	private void Update()
	{
		Rerange();
	}

	private void CalVisibleRange(out int lineBegin, out int lineEnd)
	{
		Vector3 vector = mContentTransform.anchoredPosition;
		if (mHorizontal)
		{
			lineBegin = (int)(0f - mCellInitPos.x - vector.x - 1f) / (int)CellSize.x;
			lineEnd = (int)(0f - mCellInitPos.x - vector.x + mRect.width - 1f) / (int)CellSize.x;
		}
		else
		{
			lineBegin = (int)(mCellInitPos.y + vector.y - 1f) / (int)CellSize.y;
			lineEnd = (int)(mCellInitPos.y + vector.y + mRect.height - 1f) / (int)CellSize.y;
		}
		if (lineBegin < 0)
		{
			lineBegin = 0;
		}
		if (lineBegin >= mLineNum)
		{
			lineBegin = mLineNum - 1;
		}
		if (lineEnd >= mLineNum)
		{
			lineEnd = mLineNum - 1;
		}
		if (lineEnd < 0)
		{
			lineEnd = 0;
		}
	}

	private int getCountPerRowOrCol()
	{
		return (!mHorizontal) ? cols : rows;
	}
}
