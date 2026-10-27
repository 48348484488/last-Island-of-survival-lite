using UnityEngine;
using UnityEngine.UI;

public class SpriteGraphic : MaskableGraphic
{
	public SpriteAsset m_spriteAsset;

	public override Texture mainTexture
	{
		get
		{
			if (m_spriteAsset == null || m_spriteAsset.texSource == null)
			{
				return Graphic.s_WhiteTexture;
			}
			return m_spriteAsset.texSource;
		}
	}

	protected override void OnEnable()
	{
	}

	protected override void OnRectTransformDimensionsChange()
	{
	}

	public new void UpdateMaterial()
	{
		base.UpdateMaterial();
	}
}
