using System;
using System.Collections.Generic;

namespace SC.LargeScene
{
	[Serializable]
	public class SmallSceneList
	{
		public static string sceneInfoPath = "gameconfig/allscenelist.obj";

		public List<SmallSceneInfo> sceneList;

		public SmallSceneList()
		{
			sceneList = new List<SmallSceneInfo>();
		}

		public SmallSceneList(List<SmallSceneInfo> info)
		{
			sceneList = info;
		}

		public void AddScene(SmallSceneInfo scene)
		{
			sceneList.Add(scene);
		}

		public SmallSceneInfo GetSceneByName(string name)
		{
			foreach (SmallSceneInfo scene in sceneList)
			{
				if (scene.scenename.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return scene;
				}
			}
			return null;
		}
	}
}
