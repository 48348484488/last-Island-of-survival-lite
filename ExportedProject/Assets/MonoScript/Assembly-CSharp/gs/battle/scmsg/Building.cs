using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class Building : Marshal
	{
		public int id;

		public int bType;

		public Vec3 pos = new Vec3();

		public Dictionary<int, Vec3> dropPoints = new Dictionary<int, Vec3>();

		public Dictionary<int, Vec3> roofPoints = new Dictionary<int, Vec3>();

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(bType);
			oc.push(pos);
			oc.push(dropPoints.Count);
			foreach (KeyValuePair<int, Vec3> dropPoint in dropPoints)
			{
				oc.push(dropPoint.Key);
				oc.push(dropPoint.Value);
			}
			oc.push(roofPoints.Count);
			foreach (KeyValuePair<int, Vec3> roofPoint in roofPoints)
			{
				oc.push(roofPoint.Key);
				oc.push(roofPoint.Value);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			bType = oc.pop_int();
			oc.pop(pos);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				Vec3 vec = new Vec3();
				oc.pop(vec);
				dropPoints.Add(key, vec);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				int key2 = oc.pop_int();
				Vec3 vec2 = new Vec3();
				oc.pop(vec2);
				roofPoints.Add(key2, vec2);
			}
			return oc;
		}
	}
}
