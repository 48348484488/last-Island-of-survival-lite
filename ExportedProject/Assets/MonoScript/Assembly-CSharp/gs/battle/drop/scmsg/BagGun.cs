using System.Collections.Generic;
using Share;

namespace gs.battle.drop.scmsg
{
	public class BagGun : Marshal
	{
		public int gunId;

		public int bulletId;

		public int bulletNumber;

		public HashSet<int> accessory = new HashSet<int>();

		public int skinId;

		public Octets marshal(Octets oc)
		{
			oc.push(gunId);
			oc.push(bulletId);
			oc.push(bulletNumber);
			oc.push(accessory.Count);
			foreach (int item in accessory)
			{
				oc.push(item);
			}
			oc.push(skinId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			gunId = oc.pop_int();
			bulletId = oc.pop_int();
			bulletNumber = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				accessory.Add(oc.pop_int());
			}
			skinId = oc.pop_int();
			return oc;
		}
	}
}
