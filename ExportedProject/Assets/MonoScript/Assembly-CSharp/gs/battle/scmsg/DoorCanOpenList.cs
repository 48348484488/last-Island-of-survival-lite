using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class DoorCanOpenList : Marshal
	{
		public long lockRoleId;

		public HashSet<long> roleIds = new HashSet<long>();

		public Octets marshal(Octets oc)
		{
			oc.push(lockRoleId);
			oc.push(roleIds.Count);
			foreach (long roleId in roleIds)
			{
				oc.push(roleId);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			lockRoleId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
