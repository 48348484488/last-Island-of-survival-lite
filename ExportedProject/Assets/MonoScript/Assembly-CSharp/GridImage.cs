using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class GridImage : Graphic
{
	[SerializeField]
	private Sprite m_Sprite;

	private Sprite m_OverrideSprite;

	private RectTransform _rectTransform;

	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			m_Sprite = value;
			SetAllDirty();
		}
	}

	public Sprite overrideSprite
	{
		get
		{
			return (!(m_OverrideSprite == null)) ? m_OverrideSprite : sprite;
		}
		set
		{
			SetAllDirty();
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (overrideSprite == null)
			{
				if (material != null && material.mainTexture != null)
				{
					return material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
			return overrideSprite.texture;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_rectTransform = GetComponent<RectTransform>();
	}

	public void SetGrid(int i, int j)
	{
		base.transform.localPosition = new Vector2(i, j) * _rectTransform.rect.width;
	}

	public void SetGrid(Vector2 RowCol)
	{
		base.transform.localPosition = RowCol * _rectTransform.rect.width;
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (overrideSprite == null)
		{
			base.OnPopulateMesh(toFill);
		}
		else
		{
			GenerateSimpleSprite(toFill, false);
		}
	}

	private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
	{
		Vector4 drawingDimensions = GetDrawingDimensions(lPreserveAspect);
		Vector4 vector = ((!(overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite));
		Color color = this.color;
		vh.Clear();
		vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x, vector.y));
		vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x, vector.w));
		vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z, vector.w));
		vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z, vector.y));
		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(2, 3, 0);
	}

	private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
	{
		Vector4 vector = ((!(overrideSprite == null)) ? DataUtility.GetPadding(overrideSprite) : Vector4.zero);
		Vector2 vector2 = ((!(overrideSprite == null)) ? new Vector2(overrideSprite.rect.width, overrideSprite.rect.height) : Vector2.zero);
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		int num = Mathf.RoundToInt(vector2.x);
		int num2 = Mathf.RoundToInt(vector2.y);
		Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
		if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
		{
			float num3 = vector2.x / vector2.y;
			float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
			if (num3 > num4)
			{
				float height = pixelAdjustedRect.height;
				pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
				pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
			}
			else
			{
				float width = pixelAdjustedRect.width;
				pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
				pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
			}
		}
		return new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
	}
}
