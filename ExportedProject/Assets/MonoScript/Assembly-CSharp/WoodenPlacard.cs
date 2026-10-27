using System;
using UnityEngine;

public class WoodenPlacard : MonoBehaviour
{
	private MeshRenderer _textMeshRenderer;

	private BoxCollider _boxCollider;

	private Transform _boxTransform;

	private Transform _textTransform;

	private TextMesh _textMesh;

	private long _instanceId;

	private int _itemId;

	private Vector3 _originVector3 = new Vector3(0f, 0f, -0.06f);

	private int _maxWordCount = 5;

	private Color _color = Color.white;

	private int _colorIndex;

	private int _wordSize = 1;

	private string _code = string.Empty;

	private int _maxWordSize = 93;

	private int _minWordSize = 30;

	private int _spacingWordSize = 7;

	private WoodenPlacardMgr.HorizontalAlignment _horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Center;

	private WoodenPlacardMgr.VerticalAlignment _verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Middle;

	private int ColorIndex
	{
		get
		{
			return _colorIndex;
		}
		set
		{
			_colorIndex = ((value >= 12) ? 12 : value);
		}
	}

	private int WordSize
	{
		get
		{
			return _wordSize;
		}
		set
		{
			if (value > 10)
			{
				_wordSize = 10;
			}
			else if (value < 1)
			{
				_wordSize = 1;
			}
			else
			{
				_wordSize = value;
			}
		}
	}

	public string Code
	{
		get
		{
			return _code;
		}
	}

	public string Content
	{
		get
		{
			return _textMesh.text;
		}
	}

	public float PosX
	{
		get
		{
			return _textTransform.localPosition.x;
		}
	}

	public float PosY
	{
		get
		{
			return _textTransform.localPosition.y;
		}
	}

	private void Awake()
	{
		_boxTransform = GetTransform(base.transform, "component");
		_textTransform = GetTransform(base.transform, "txt_content");
		_textMeshRenderer = _textTransform.GetComponent<MeshRenderer>();
		_boxCollider = _boxTransform.GetComponent<BoxCollider>();
		_textMesh = _textTransform.GetComponent<TextMesh>();
		MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Combine(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(RefreshFixedLableDelegateHandle));
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

	public void RefreshColor()
	{
		_color = Singleton<WoodenPlacardMgr>.Ins.Colors[ColorIndex];
		_textMesh.color = _color;
	}

	public void RefreshWordSize()
	{
		_textMesh.fontSize = WordSize * _spacingWordSize + _minWordSize;
	}

	public void RefreshTextPosition(float x, float y)
	{
		_textTransform.localPosition = new Vector3(x, y, _textTransform.localPosition.z);
	}

	public void RefreshHorizontalAlignment()
	{
		float num = _boxCollider.size.x / 2f;
		float num2 = Mathf.Abs(Mathf.Cos(base.transform.eulerAngles.y));
		float num3 = _textMeshRenderer.bounds.size.x / num2 / 2f;
		float x = 0f;
		switch (_horizontalAlignment)
		{
		case WoodenPlacardMgr.HorizontalAlignment.Left:
			x = 0f - (num3 - num);
			break;
		case WoodenPlacardMgr.HorizontalAlignment.Right:
			x = num3 - num;
			break;
		}
		_textTransform.localPosition = new Vector3(x, _textTransform.localPosition.y, _textTransform.localPosition.z);
	}

	public void RefreshVerticalAlignment()
	{
		float y = _boxCollider.size.y;
		float num = y / 2f;
		float y2 = _textMeshRenderer.bounds.size.y;
		float num2 = y2 / 2f;
		float y3 = 0f;
		switch (_verticalAlignment)
		{
		case WoodenPlacardMgr.VerticalAlignment.Up:
			y3 = 0f - num2 + num;
			break;
		case WoodenPlacardMgr.VerticalAlignment.Down:
			y3 = num2 - num;
			break;
		}
		_textTransform.localPosition = new Vector3(_textTransform.localPosition.x, y3, _textTransform.localPosition.z);
	}

	public void CheckText(string text)
	{
		_textMesh.text = text;
		RefreshVerticalAlignment();
		RefreshHorizontalAlignment();
		CheckText();
	}

	public void CheckText()
	{
		if (_textMeshRenderer.bounds.size.x > _boxCollider.size.x)
		{
			string text = _textMesh.text.Remove(_textMesh.text.Length - 1);
			_textMesh.text = text;
		}
	}

	public void UpToServer()
	{
	}

	public void Marshal()
	{
		_code = string.Concat(_instanceId, "_", _itemId, "_", ColorIndex, "_", WordSize, "_", _horizontalAlignment, "_", _verticalAlignment);
	}

	public void Unmarshal(string code, string content)
	{
		_code = code;
		if (string.IsNullOrEmpty(_code))
		{
			Clear();
			return;
		}
		string[] array = _code.Split('_');
		if (array.Length <= 0)
		{
			Clear();
			return;
		}
		if (array.Length > 0)
		{
			_instanceId = long.Parse(array[0]);
		}
		if (array.Length > 1)
		{
			_itemId = int.Parse(array[1]);
		}
		if (array.Length > 2)
		{
			ColorIndex = int.Parse(array[2]);
		}
		if (array.Length > 3)
		{
			WordSize = int.Parse(array[3]);
		}
		if (array.Length > 4)
		{
			_horizontalAlignment = (WoodenPlacardMgr.HorizontalAlignment)int.Parse(array[4]);
		}
		if (array.Length > 5)
		{
			_verticalAlignment = (WoodenPlacardMgr.VerticalAlignment)int.Parse(array[5]);
		}
		_textMesh.text = content;
		RefreshContent();
		if (array.Length > 5)
		{
			float x = float.Parse(array[6]);
			float y = float.Parse(array[7]);
			_textTransform.localPosition = new Vector3(x, y, _textTransform.localPosition.z);
		}
	}

	public void RefreshUnmarshal()
	{
		if (string.IsNullOrEmpty(_code))
		{
			Clear();
			return;
		}
		string[] array = _code.Split('_');
		if (array.Length <= 0)
		{
			Clear();
		}
	}

	public void RefreshContent()
	{
		RefreshWordSize();
		RefreshColor();
		RefreshHorizontalAlignment();
		RefreshVerticalAlignment();
	}

	public void Clear()
	{
		_textMesh.text = Utils.GetString(264);
		ColorIndex = 0;
		WordSize = 1;
		_code = string.Empty;
		_horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Center;
		_verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Middle;
		RefreshContent();
	}

	public void SetColor(int color)
	{
		ColorIndex = color;
		RefreshColor();
	}

	public void SetWordSize(int size, Utils.StringDelegate callback = null)
	{
		WordSize = size;
		RefreshWordSize();
		CheckText();
		Utils.TriggerEvent(callback, _textMesh.text);
	}

	public void SetWordContent(string content)
	{
		CheckText(content);
	}

	public void SetWordContentByUi(string content, Utils.StringDelegate callback)
	{
		CheckText(content);
		Utils.TriggerEvent(callback, _textMesh.text);
	}

	public void SetHorizontalAlignment(WoodenPlacardMgr.HorizontalAlignment horizontalAlignment, Utils.StringDelegate callback = null)
	{
		_horizontalAlignment = horizontalAlignment;
		RefreshHorizontalAlignment();
	}

	public void SetVerticalAlignment(WoodenPlacardMgr.VerticalAlignment verticalAlignment, Utils.StringDelegate callback = null)
	{
		_verticalAlignment = verticalAlignment;
		RefreshVerticalAlignment();
	}

	private void Destroy()
	{
		MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Remove(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(RefreshFixedLableDelegateHandle));
	}

	private void RefreshFixedLableDelegateHandle()
	{
		RefreshUnmarshal();
	}
}
