using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ViewTools
{
	[CompilerGenerated]
	private sealed class _003CSetSprite_003Ec__AnonStorey0
	{
		internal Image image;

		internal void _003C_003Em__0(Object o)
		{
			image.sprite = o as Sprite;
			image.enabled = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSetItemSprite_003Ec__AnonStorey1
	{
		internal ImageName imgName;

		internal Image image;

		internal void _003C_003Em__0(Object o)
		{
			if (o.name == imgName.imageName)
			{
				image.sprite = o as Sprite;
			}
			image.enabled = true;
		}
	}

	public static void SetSprite(GameObject go, string name, string SpriteABpath)
	{
		_003CSetSprite_003Ec__AnonStorey0 _003CSetSprite_003Ec__AnonStorey = new _003CSetSprite_003Ec__AnonStorey0();
		_003CSetSprite_003Ec__AnonStorey.image = go.GetComponent<Image>();
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(SpriteABpath))
		{
			_003CSetSprite_003Ec__AnonStorey.image.sprite = null;
			ResMgr.Ins.CleanRef(go);
		}
		else
		{
			_003CSetSprite_003Ec__AnonStorey.image.enabled = false;
			ResMgr.Ins.LoadAssetFromAB<Sprite>(SpriteABpath, name, go, _003CSetSprite_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public static void SetItemSprite(GameObject go, string name)
	{
		_003CSetItemSprite_003Ec__AnonStorey1 _003CSetItemSprite_003Ec__AnonStorey = new _003CSetItemSprite_003Ec__AnonStorey1();
		_003CSetItemSprite_003Ec__AnonStorey.image = go.GetComponent<Image>();
		_003CSetItemSprite_003Ec__AnonStorey.imgName = go.GetComponent<ImageName>();
		if (_003CSetItemSprite_003Ec__AnonStorey.imgName == null)
		{
			_003CSetItemSprite_003Ec__AnonStorey.imgName = go.AddComponent<ImageName>();
		}
		_003CSetItemSprite_003Ec__AnonStorey.imgName.imageName = name;
		if (!(_003CSetItemSprite_003Ec__AnonStorey.image.sprite != null) || !(_003CSetItemSprite_003Ec__AnonStorey.image.sprite.name == name))
		{
			if (string.IsNullOrEmpty(name))
			{
				ResMgr.Ins.CleanRef(go);
				_003CSetItemSprite_003Ec__AnonStorey.image.sprite = null;
			}
			else
			{
				_003CSetItemSprite_003Ec__AnonStorey.image.enabled = false;
				ResMgr.Ins.LoadAssetFromAB<Sprite>("icon/item.ab", name, go, _003CSetItemSprite_003Ec__AnonStorey._003C_003Em__0);
			}
		}
	}
}
