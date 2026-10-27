using System;
using System.Collections.Generic;

[Serializable]
public class SpriteInforGroup
{
	public string tag = string.Empty;

	public List<SpriteInfor> listSpriteInfor = new List<SpriteInfor>();

	public float width = 1f;

	public float size = 24f;
}
