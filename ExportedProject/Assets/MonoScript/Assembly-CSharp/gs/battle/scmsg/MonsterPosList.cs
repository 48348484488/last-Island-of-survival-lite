using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class MonsterPosList : Marshal
	{
		public List<Vec3> poss = new List<Vec3>();

		public Octets marshal(Octets oc)
		{
			oc.push(poss.Count);
			foreach (Vec3 item in poss)
			{
				oc.push(item);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Vec3 vec = new Vec3();
				oc.pop(vec);
				poss.Add(vec);
			}
			return oc;
		}
	}
}
