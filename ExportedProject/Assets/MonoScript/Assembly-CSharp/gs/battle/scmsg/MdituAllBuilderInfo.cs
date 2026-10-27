using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class MdituAllBuilderInfo : Marshal
	{
		public byte terrainlightmapOffset;

		public List<string> prefabs = new List<string>();

		public List<MdituBuildingObjectInfo> buildings = new List<MdituBuildingObjectInfo>();

		public Octets marshal(Octets oc)
		{
			oc.push(terrainlightmapOffset);
			oc.push(prefabs.Count);
			foreach (string prefab in prefabs)
			{
				oc.push(prefab);
			}
			oc.push(buildings.Count);
			foreach (MdituBuildingObjectInfo building in buildings)
			{
				oc.push(building);
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
				MdituBuildingObjectInfo mdituBuildingObjectInfo = new MdituBuildingObjectInfo();
				oc.pop(mdituBuildingObjectInfo);
				buildings.Add(mdituBuildingObjectInfo);
			}
			return oc;
		}
	}
}
