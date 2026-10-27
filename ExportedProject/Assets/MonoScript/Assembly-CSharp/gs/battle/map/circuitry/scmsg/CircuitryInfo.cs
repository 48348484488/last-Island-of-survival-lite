using System.Collections.Generic;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CircuitryInfo : Marshal
	{
		public long instanceId;

		public int cfgId;

		public string name = string.Empty;

		public List<long> childIds = new List<long>();

		public Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(cfgId);
			oc.push(name);
			oc.push(childIds.Count);
			foreach (long childId in childIds)
			{
				oc.push(childId);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			cfgId = oc.pop_int();
			name = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				childIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
