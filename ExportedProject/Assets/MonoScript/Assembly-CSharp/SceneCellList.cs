using System;
using System.Collections.Generic;

[Serializable]
public class SceneCellList
{
	public static readonly string jsonFilePath = "gameconfig/scenecell.obj";

	public static readonly int AllSceneSize = 6000;

	public static readonly int perCellSize = 100;

	public static readonly int nCellCount = 60;

	public static readonly int nCellTotalCount = 3600;

	public static readonly int nScenePosOffset = 3000;

	public List<SceneCell> sceneList;

	public SceneCellList()
	{
		sceneList = new List<SceneCell>(nCellTotalCount + 1);
		for (int i = 0; i < nCellCount; i++)
		{
			for (int j = 0; j < nCellCount; j++)
			{
				SceneCell item = new SceneCell
				{
					xPos = i * perCellSize + perCellSize / 2 - nScenePosOffset,
					zPos = j * perCellSize + perCellSize / 2 - nScenePosOffset
				};
				sceneList.Add(item);
			}
		}
	}

	public void AddChuShendGaoCell()
	{
		SceneCell sceneCell = new SceneCell();
		sceneCell.smallSceneList.Add("chushengdao");
		sceneList.Add(sceneCell);
	}

	public SceneCell GetCellByPos(float x, float z)
	{
		x += (float)nScenePosOffset;
		z += (float)nScenePosOffset;
		int num = (int)x / perCellSize;
		int num2 = (int)z / perCellSize;
		int num3 = num * nCellCount + num2;
		if (num3 < 0 || num3 >= nCellTotalCount)
		{
			return sceneList[0];
		}
		return sceneList[num3];
	}
}
