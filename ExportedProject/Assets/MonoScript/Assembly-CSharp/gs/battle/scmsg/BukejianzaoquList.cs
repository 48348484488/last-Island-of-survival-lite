using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class BukejianzaoquList : Marshal
	{
		public List<BukejianzaoquPos> allpos = new List<BukejianzaoquPos>();

		public Octets marshal(Octets oc)
		{
			oc.push(allpos.Count);
			foreach (BukejianzaoquPos allpo in allpos)
			{
				oc.push(allpo);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				BukejianzaoquPos bukejianzaoquPos = new BukejianzaoquPos();
				oc.pop(bukejianzaoquPos);
				allpos.Add(bukejianzaoquPos);
			}
			return oc;
		}
	}
}
