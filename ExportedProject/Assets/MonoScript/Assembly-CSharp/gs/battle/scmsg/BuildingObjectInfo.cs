using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class BuildingObjectInfo : Marshal
	{
		public GameObjectInfo info = new GameObjectInfo();

		public List<LightmapInfo> lightmapInfos = new List<LightmapInfo>();

		public int buildingId;

		public Octets marshal(Octets oc)
		{
			oc.push(info);
			oc.push(lightmapInfos.Count);
			foreach (LightmapInfo lightmapInfo in lightmapInfos)
			{
				oc.push(lightmapInfo);
			}
			oc.push(buildingId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				LightmapInfo lightmapInfo = new LightmapInfo();
				oc.pop(lightmapInfo);
				lightmapInfos.Add(lightmapInfo);
			}
			buildingId = oc.pop_int();
			return oc;
		}
	}
}
