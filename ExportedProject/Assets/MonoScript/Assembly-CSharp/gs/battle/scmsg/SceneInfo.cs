using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class SceneInfo : Marshal
	{
		public byte terrainlightmapOffset;

		public List<string> prefabs = new List<string>();

		public List<GrassInfo> grassList = new List<GrassInfo>();

		public List<TreeObjectInfo> treeList = new List<TreeObjectInfo>();

		public List<BuildingObjectInfo> buildings = new List<BuildingObjectInfo>();

		public List<GameObjectInfo> thingsNoLightMap = new List<GameObjectInfo>();

		public List<GameObjectWithLightmap> thingsWithLightMap = new List<GameObjectWithLightmap>();

		public List<StaticGroup> staticGroups = new List<StaticGroup>();

		public Octets marshal(Octets oc)
		{
			oc.push(terrainlightmapOffset);
			oc.push(prefabs.Count);
			foreach (string prefab in prefabs)
			{
				oc.push(prefab);
			}
			oc.push(grassList.Count);
			foreach (GrassInfo grass in grassList)
			{
				oc.push(grass);
			}
			oc.push(treeList.Count);
			foreach (TreeObjectInfo tree in treeList)
			{
				oc.push(tree);
			}
			oc.push(buildings.Count);
			foreach (BuildingObjectInfo building in buildings)
			{
				oc.push(building);
			}
			oc.push(thingsNoLightMap.Count);
			foreach (GameObjectInfo item in thingsNoLightMap)
			{
				oc.push(item);
			}
			oc.push(thingsWithLightMap.Count);
			foreach (GameObjectWithLightmap item2 in thingsWithLightMap)
			{
				oc.push(item2);
			}
			oc.push(staticGroups.Count);
			foreach (StaticGroup staticGroup in staticGroups)
			{
				oc.push(staticGroup);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			terrainlightmapOffset = oc.pop_byte();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				prefabs.Add(oc.pop_string());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				GrassInfo grassInfo = new GrassInfo();
				oc.pop(grassInfo);
				grassList.Add(grassInfo);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				TreeObjectInfo treeObjectInfo = new TreeObjectInfo();
				oc.pop(treeObjectInfo);
				treeList.Add(treeObjectInfo);
			}
			int l = 0;
			for (int num4 = oc.pop_int(); l < num4; l++)
			{
				BuildingObjectInfo buildingObjectInfo = new BuildingObjectInfo();
				oc.pop(buildingObjectInfo);
				buildings.Add(buildingObjectInfo);
			}
			int m = 0;
			for (int num5 = oc.pop_int(); m < num5; m++)
			{
				GameObjectInfo gameObjectInfo = new GameObjectInfo();
				oc.pop(gameObjectInfo);
				thingsNoLightMap.Add(gameObjectInfo);
			}
			int n = 0;
			for (int num6 = oc.pop_int(); n < num6; n++)
			{
				GameObjectWithLightmap gameObjectWithLightmap = new GameObjectWithLightmap();
				oc.pop(gameObjectWithLightmap);
				thingsWithLightMap.Add(gameObjectWithLightmap);
			}
			int num7 = 0;
			for (int num8 = oc.pop_int(); num7 < num8; num7++)
			{
				StaticGroup staticGroup = new StaticGroup();
				oc.pop(staticGroup);
				staticGroups.Add(staticGroup);
			}
			return oc;
		}
	}
}
