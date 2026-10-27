using System.Collections.Generic;
using Share;

namespace client
{
	public class ProduceLocalData : Marshal
	{
		public long RoleId;

		public HashSet<int> NewUnlock = new HashSet<int>();

		public Octets marshal(Octets oc)
		{
			oc.push(RoleId);
			oc.push(NewUnlock.Count);
			foreach (int item in NewUnlock)
			{
				oc.push(item);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			RoleId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				NewUnlock.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
