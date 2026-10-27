using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class SceneAllBuildings : Marshal
	{
		public List<Building> buildings = new List<Building>();

		public Octets marshal(Octets oc)
		{
			oc.push(buildings.Count);
			foreach (Building building in buildings)
			{
				oc.push(building);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Building building = new Building();
				oc.pop(building);
				buildings.Add(building);
			}
			return oc;
		}
	}
}
