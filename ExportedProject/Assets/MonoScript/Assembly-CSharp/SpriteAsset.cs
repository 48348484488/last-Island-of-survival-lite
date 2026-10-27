using System.Collections.Generic;
using UnityEngine;

public class SpriteAsset : ScriptableObject
{
	public int ID;

	public bool _IsStatic;

	public Texture texSource;

	public List<SpriteInforGroup> listSpriteGroup;
}
