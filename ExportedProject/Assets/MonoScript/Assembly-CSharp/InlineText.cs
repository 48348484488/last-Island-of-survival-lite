using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class InlineText : Text, IPointerClickHandler, IEventSystemHandler
{
	[Serializable]
	public class HrefClickEvent : UnityEvent<string, int>
	{
	}

	private class HrefInfo
	{
		public int id;

		public int startIndex;

		public int endIndex;

		public string name;

		public readonly List<Rect> boxes = new List<Rect>();
	}

	private static readonly Regex _InputTagRegex = new Regex("\\[(\\-{0,1}\\d{0,})#(.+?)\\]", RegexOptions.Singleline);

	private InlineManager _InlineManager;

	private string _OutputText = string.Empty;

	private Dictionary<int, SpriteTagInfo> _SpriteInfo = new Dictionary<int, SpriteTagInfo>();

	private Dictionary<int, List<SpriteTagInfo>> _DrawSpriteInfo = new Dictionary<int, List<SpriteTagInfo>>();

	public HrefClickEvent OnHrefClick = new HrefClickEvent();

	private readonly List<HrefInfo> _ListHrefInfos = new List<HrefInfo>();

	private readonly UIVertex[] m_TempVerts = new UIVertex[4];

	public override float preferredWidth
	{
		get
		{
			TextGenerationSettings generationSettings = GetGenerationSettings(Vector2.zero);
			return base.cachedTextGeneratorForLayout.GetPreferredWidth(_OutputText, generationSettings) / base.pixelsPerUnit;
		}
	}

	public override float preferredHeight
	{
		get
		{
			TextGenerationSettings generationSettings = GetGenerationSettings(new Vector2(base.rectTransform.rect.size.x, 0f));
			return base.cachedTextGeneratorForLayout.GetPreferredHeight(_OutputText, generationSettings) / base.pixelsPerUnit;
		}
	}

	protected override void Start()
	{
		ActiveText();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UpdateDrawnSprite();
	}

	public void ActiveText()
	{
		base.supportRichText = true;
		base.alignByGeometry = false;
		if (!_InlineManager)
		{
			_InlineManager = GetComponentInParent<InlineManager>();
		}
		SetVerticesDirty();
	}

	public override void SetVerticesDirty()
	{
		base.SetVerticesDirty();
		if (!_InlineManager)
		{
			_OutputText = m_Text;
		}
		else
		{
			_OutputText = GetOutputText();
		}
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (base.font == null)
		{
			return;
		}
		m_DisableFontTextureRebuiltCallback = true;
		Vector2 size = base.rectTransform.rect.size;
		TextGenerationSettings generationSettings = GetGenerationSettings(size);
		try
		{
			base.cachedTextGenerator.Populate(_OutputText, generationSettings);
			IList<UIVertex> verts = base.cachedTextGenerator.verts;
			float num = 1f / base.pixelsPerUnit;
			int num2 = verts.Count - 4;
			Vector2 vector = new Vector2(verts[0].position.x, verts[0].position.y) * num;
			vector = PixelAdjustPoint(vector) - vector;
			toFill.Clear();
			ClearQuadUVs(verts);
			List<Vector3> list = new List<Vector3>();
			if (vector != Vector2.zero)
			{
				for (int i = 0; i < num2; i++)
				{
					int num3 = i & 3;
					m_TempVerts[num3] = verts[i];
					m_TempVerts[num3].position *= num;
					m_TempVerts[num3].position.x += vector.x;
					m_TempVerts[num3].position.y += vector.y;
					if (num3 == 3)
					{
						toFill.AddUIVertexQuad(m_TempVerts);
					}
					list.Add(m_TempVerts[num3].position);
				}
			}
			else
			{
				for (int j = 0; j < num2; j++)
				{
					int num4 = j & 3;
					m_TempVerts[num4] = verts[j];
					m_TempVerts[num4].position *= num;
					if (num4 == 3)
					{
						toFill.AddUIVertexQuad(m_TempVerts);
					}
					list.Add(m_TempVerts[num4].position);
				}
			}
			CalcQuadInfo(list);
			CalcBoundsInfo(list, toFill, generationSettings);
			m_DisableFontTextureRebuiltCallback = false;
		}
		catch (Exception)
		{
		}
	}

	private void ClearQuadUVs(IList<UIVertex> verts)
	{
		foreach (KeyValuePair<int, SpriteTagInfo> item in _SpriteInfo)
		{
			if (item.Key + 4 <= verts.Count)
			{
				for (int i = item.Key; i < item.Key + 4; i++)
				{
					UIVertex value = verts[i];
					value.uv0 = Vector2.zero;
					verts[i] = value;
				}
			}
		}
	}

	private void CalcQuadInfo(List<Vector3> _listVertsPos)
	{
		float num = 1f / base.pixelsPerUnit;
		foreach (KeyValuePair<int, SpriteTagInfo> item in _SpriteInfo)
		{
			if (item.Key + 4 <= _listVertsPos.Count)
			{
				for (int i = item.Key; i < item.Key + 4; i++)
				{
					item.Value._Pos[i - item.Key] = _listVertsPos[i] + new Vector3(0f, 11f, 0f) * num;
				}
			}
		}
		UpdateDrawnSprite();
	}

	private void UpdateDrawnSprite()
	{
		_DrawSpriteInfo = new Dictionary<int, List<SpriteTagInfo>>();
		foreach (KeyValuePair<int, SpriteTagInfo> item in _SpriteInfo)
		{
			int iD = item.Value._ID;
			List<SpriteTagInfo> list = null;
			if (_DrawSpriteInfo.ContainsKey(iD))
			{
				list = _DrawSpriteInfo[iD];
			}
			else
			{
				list = new List<SpriteTagInfo>();
				_DrawSpriteInfo.Add(iD, list);
			}
			list.Add(item.Value);
		}
		foreach (KeyValuePair<int, List<SpriteTagInfo>> item2 in _DrawSpriteInfo)
		{
			_InlineManager.UpdateTextInfo(item2.Key, this, item2.Value);
		}
	}

	private void CalcBoundsInfo(List<Vector3> _listVertsPos, VertexHelper toFill, TextGenerationSettings settings)
	{
		foreach (HrefInfo listHrefInfo in _ListHrefInfos)
		{
			listHrefInfo.boxes.Clear();
			if (listHrefInfo.startIndex >= _listVertsPos.Count)
			{
				continue;
			}
			Vector3 center = _listVertsPos[listHrefInfo.startIndex];
			Bounds bounds = new Bounds(center, Vector3.zero);
			int i = listHrefInfo.startIndex;
			for (int endIndex = listHrefInfo.endIndex; i < endIndex && i < _listVertsPos.Count; i++)
			{
				center = _listVertsPos[i];
				if (center.x < bounds.min.x)
				{
					listHrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
					bounds = new Bounds(center, Vector3.zero);
				}
				else
				{
					bounds.Encapsulate(center);
				}
			}
			listHrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
		}
		TextGenerator textGenerator = new TextGenerator();
		textGenerator.Populate("_", settings);
		IList<UIVertex> verts = textGenerator.verts;
		foreach (HrefInfo listHrefInfo2 in _ListHrefInfos)
		{
			for (int j = 0; j < listHrefInfo2.boxes.Count; j++)
			{
				Vector3[] array = new Vector3[4];
				array[0] = listHrefInfo2.boxes[j].position + new Vector2(0f, (float)base.fontSize * 0.2f);
				array[1] = array[0] + new Vector3(listHrefInfo2.boxes[j].width, 0f);
				array[2] = listHrefInfo2.boxes[j].position + new Vector2(listHrefInfo2.boxes[j].width, 0f);
				array[3] = listHrefInfo2.boxes[j].position;
				for (int k = 0; k < 4; k++)
				{
					m_TempVerts[k] = verts[k];
					m_TempVerts[k].color = Color.blue;
					m_TempVerts[k].position = array[k];
					if (k == 3)
					{
						toFill.AddUIVertexQuad(m_TempVerts);
					}
				}
			}
		}
	}

	private string GetOutputText()
	{
		_SpriteInfo = new Dictionary<int, SpriteTagInfo>();
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		IEnumerator enumerator = _InputTagRegex.Matches(text).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Match match = (Match)enumerator.Current;
				int num2 = 0;
				if (!string.IsNullOrEmpty(match.Groups[1].Value) && !match.Groups[1].Value.Equals("-"))
				{
					num2 = int.Parse(match.Groups[1].Value);
				}
				string value = match.Groups[2].Value;
				if (num2 < 0)
				{
					stringBuilder.Append(text.Substring(num, match.Index - num));
					stringBuilder.Append("<color=blue>");
					int startIndex = stringBuilder.Length * 4;
					stringBuilder.Append("[" + match.Groups[2].Value + "]");
					int endIndex = stringBuilder.Length * 4 - 2;
					stringBuilder.Append("</color>");
					HrefInfo hrefInfo = new HrefInfo();
					hrefInfo.id = Mathf.Abs(num2);
					hrefInfo.startIndex = startIndex;
					hrefInfo.endIndex = endIndex;
					hrefInfo.name = match.Groups[2].Value;
					HrefInfo item = hrefInfo;
					_ListHrefInfos.Add(item);
				}
				else
				{
					if (!_InlineManager._IndexSpriteInfo.ContainsKey(num2) || !_InlineManager._IndexSpriteInfo[num2].ContainsKey(value))
					{
						continue;
					}
					SpriteInforGroup spriteInforGroup = _InlineManager._IndexSpriteInfo[num2][value];
					stringBuilder.Append(text.Substring(num, match.Index - num));
					int key = stringBuilder.Length * 4;
					stringBuilder.Append("<quad size=" + spriteInforGroup.size * 1.8f + " width=" + spriteInforGroup.width + " />");
					SpriteTagInfo spriteTagInfo = new SpriteTagInfo();
					spriteTagInfo._ID = num2;
					spriteTagInfo._Tag = value;
					spriteTagInfo._Size = new Vector2(spriteInforGroup.size * spriteInforGroup.width, spriteInforGroup.size);
					spriteTagInfo._Pos = new Vector3[4];
					spriteTagInfo._UV = spriteInforGroup.listSpriteInfor[0].uv;
					SpriteTagInfo value2 = spriteTagInfo;
					if (!_SpriteInfo.ContainsKey(key))
					{
						_SpriteInfo.Add(key, value2);
					}
				}
				num = match.Index + match.Length;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		stringBuilder.Append(text.Substring(num, text.Length - num));
		return stringBuilder.ToString();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
		foreach (HrefInfo listHrefInfo in _ListHrefInfos)
		{
			List<Rect> boxes = listHrefInfo.boxes;
			for (int i = 0; i < boxes.Count; i++)
			{
				if (boxes[i].Contains(localPoint))
				{
					OnHrefClick.Invoke(listHrefInfo.name, listHrefInfo.id);
					return;
				}
			}
		}
	}
}
