using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class DropdownBtns : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CAwake_003Ec__AnonStorey0
	{
		internal int index;

		internal DropdownBtns _0024this;

		internal void _003C_003Em__0(int o)
		{
			_0024this.OpenCurrentIndex(index);
		}
	}

	public List<DropdownBtnItem> Level1Btns = new List<DropdownBtnItem>();

	public int _currentShowIndex = -1;

	public static int _currentRadioBtnIndex;

	protected float _level1BtnsHeight;

	private RectTransform _thisRectTransform;

	private Vector2 _preferrdSizeDelta;

	private List<Vector2> _bigBtnsInitPos = new List<Vector2>();

	private List<Vector3> _bigBtnsPos = new List<Vector3>();

	private Vector2 _initPos = Vector2.zero;

	private void Awake()
	{
		_level1BtnsHeight = ((Level1Btns[0].transform as RectTransform).anchoredPosition.y - (Level1Btns[1].transform as RectTransform).anchoredPosition.y) * (float)Level1Btns.Count + 30f;
		_thisRectTransform = GetComponent<RectTransform>();
		_preferrdSizeDelta.x = _thisRectTransform.rect.width;
		for (int i = 0; i < Level1Btns.Count; i++)
		{
			_003CAwake_003Ec__AnonStorey0 _003CAwake_003Ec__AnonStorey = new _003CAwake_003Ec__AnonStorey0();
			_003CAwake_003Ec__AnonStorey._0024this = this;
			_003CAwake_003Ec__AnonStorey.index = i;
			Level1Btns[i].OnClickBtn = _003CAwake_003Ec__AnonStorey._003C_003Em__0;
			_bigBtnsInitPos.Add(Level1Btns[i].btn.GetComponent<RectTransform>().anchoredPosition);
			Level1Btns[i].Index = _003CAwake_003Ec__AnonStorey.index;
		}
		OpenCurrentIndex(0);
	}

	public void OpenCurrentIndex(int index)
	{
		if (index > -1 && index == _currentShowIndex)
		{
			index = -1;
		}
		else
		{
			_currentRadioBtnIndex = index;
		}
		float num = 0f;
		for (int i = 0; i < Level1Btns.Count; i++)
		{
			if (index > -1 && index != _currentShowIndex && i > index)
			{
				num = Level1Btns[index].MaskHeight;
			}
			(Level1Btns[i].transform as RectTransform).DOAnchorPosY(_bigBtnsInitPos[i].y - num, 0.29f);
			if (Level1Btns[i].MaskHeight > 0f)
			{
				Level1Btns[i].Open.SetActiveBetter(i == index);
				Level1Btns[i].Close.SetActiveBetter(i != index);
			}
			else
			{
				Level1Btns[i].Open.SetActiveBetter(false);
				Level1Btns[i].Close.SetActiveBetter(false);
			}
		}
		CloseCurrentMaskIndex(_currentShowIndex);
		OpenCurrentMaskIndex(index);
		_currentShowIndex = index;
		if (index >= 0)
		{
			_preferrdSizeDelta.y = _level1BtnsHeight + Level1Btns[index].MaskHeight;
			_thisRectTransform.sizeDelta = _preferrdSizeDelta;
		}
	}

	private void OpenCurrentMaskIndex(int index)
	{
		if (index >= 0)
		{
			RectTransform rectTransform = Level1Btns[index].Mask.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.DoSizeDelta(new Vector2(rectTransform.sizeDelta.x, Level1Btns[index].MaskHeight), 0.29f);
			}
		}
	}

	private void CloseCurrentMaskIndex(int index)
	{
		if (index >= 0)
		{
			_preferrdSizeDelta.y = _level1BtnsHeight;
			_thisRectTransform.sizeDelta = _preferrdSizeDelta;
			RectTransform rectTransform = Level1Btns[index].Mask.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.DoSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0f), 0.29f);
			}
		}
	}

	public void Init()
	{
		_currentRadioBtnIndex = 0;
		_currentShowIndex = 0;
		float num = 0f;
		for (int i = 0; i < Level1Btns.Count; i++)
		{
			if (i == _currentRadioBtnIndex)
			{
				RectTransform rectTransform = Level1Btns[i].Mask.transform as RectTransform;
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Level1Btns[i].MaskHeight);
			}
			else
			{
				RectTransform rectTransform2 = Level1Btns[i].Mask.transform as RectTransform;
				rectTransform2.sizeDelta = new Vector2(rectTransform2.sizeDelta.x, 0f);
			}
			_initPos.Set(0f, _bigBtnsInitPos[i].y - num);
			(Level1Btns[i].transform as RectTransform).anchoredPosition = _initPos;
			if (i == _currentRadioBtnIndex)
			{
				num = Level1Btns[i].MaskHeight;
			}
			Level1Btns[i].Open.SetActiveBetter(i == _currentRadioBtnIndex);
			Level1Btns[i].Close.SetActiveBetter(i != _currentRadioBtnIndex);
		}
		_thisRectTransform.anchoredPosition = Vector2.zero;
	}
}
