using System;
using System.Collections.Generic;
using SC.LargeScene;

[Serializable]
public class SceneCell
{
	public float xPos;

	public float zPos;

	public List<string> smallSceneList;

	public List<SmallSceneInfo> smallSceneInfo { get; set; }

	public SceneCell()
	{
		smallSceneList = new List<string>();
		smallSceneInfo = new List<SmallSceneInfo>();
	}
}
