using System.Collections.Generic;
using Share;

namespace gs.friends.scmsg
{
	public class OneChestPermit : Marshal
	{
		public long instanceId;

		public HashSet<long> roleIds = new HashSet<long>();

		public float posX;

		public float posY;

		public float posZ;

		public string name = string.Empty;

		public Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(roleIds.Count);
			foreach (long roleId in roleIds)
			{
				oc.push(roleId);
			}
			oc.push(posX);
			oc.push(posY);
			oc.push(posZ);
			oc.push(name);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleIds.Add(oc.pop_long());
			}
			posX = oc.pop_float();
			posY = oc.pop_float();
			posZ = oc.pop_float();
			name = oc.pop_string();
			return oc;
		}
	}
}
