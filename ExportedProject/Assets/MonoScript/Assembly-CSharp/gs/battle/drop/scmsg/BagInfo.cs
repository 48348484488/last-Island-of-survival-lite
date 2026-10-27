using System.Collections.Generic;
using Share;

namespace gs.battle.drop.scmsg
{
	public class BagInfo : Marshal
	{
		public HashSet<int> wears = new HashSet<int>();

		public BagGun gun = new BagGun();

		public int handWeapon;

		public int instanceId;

		public Octets marshal(Octets oc)
		{
			oc.push(wears.Count);
			foreach (int wear in wears)
			{
				oc.push(wear);
			}
			oc.push(gun);
			oc.push(handWeapon);
			oc.push(instanceId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				wears.Add(oc.pop_int());
			}
			oc.pop(gun);
			handWeapon = oc.pop_int();
			instanceId = oc.pop_int();
			return oc;
		}
	}
}
