using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class DuquanPoints : Marshal
	{
		public List<Vec2> dupoints = new List<Vec2>();

		public Octets marshal(Octets oc)
		{
			oc.push(dupoints.Count);
			foreach (Vec2 dupoint in dupoints)
			{
				oc.push(dupoint);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Vec2 vec = new Vec2();
				oc.pop(vec);
				dupoints.Add(vec);
			}
			return oc;
		}
	}
}
