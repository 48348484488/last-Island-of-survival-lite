using System.Collections.Generic;
using Share;

namespace gs.bag.scmsg
{
	public class GunParts : Marshal
	{
		public HashSet<BagItem> parts = new HashSet<BagItem>();

		public int bulletNumber;

		public int bulletId;

		public Octets marshal(Octets oc)
		{
			oc.push(parts.Count);
			foreach (BagItem part in parts)
			{
				oc.push(part);
			}
			oc.push(bulletNumber);
			oc.push(bulletId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				BagItem bagItem = new BagItem();
				oc.pop(bagItem);
				parts.Add(bagItem);
			}
			bulletNumber = oc.pop_int();
			bulletId = oc.pop_int();
			return oc;
		}
	}
}
