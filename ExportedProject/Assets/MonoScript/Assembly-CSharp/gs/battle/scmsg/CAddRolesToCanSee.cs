using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAddRolesToCanSee : Message
	{
		public delegate void Handler(CAddRolesToCanSee msg);

		public const int TYPE = 11537444;

		public static Handler handler;

		public HashSet<long> roleIds = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537444;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleIds.Count);
			foreach (long roleId in roleIds)
			{
				oc.push(roleId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
