using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class LajiduiDropInfo : Marshal
	{
		public int id;

		public List<Vec3> tongs = new List<Vec3>();

		public List<Vec3> xiangzi = new List<Vec3>();

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(tongs.Count);
			foreach (Vec3 tong in tongs)
			{
				oc.push(tong);
			}
			oc.push(xiangzi.Count);
			foreach (Vec3 item in xiangzi)
			{
				oc.push(item);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Vec3 vec = new Vec3();
				oc.pop(vec);
				tongs.Add(vec);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				Vec3 vec2 = new Vec3();
				oc.pop(vec2);
				xiangzi.Add(vec2);
			}
			return oc;
		}
	}
}
