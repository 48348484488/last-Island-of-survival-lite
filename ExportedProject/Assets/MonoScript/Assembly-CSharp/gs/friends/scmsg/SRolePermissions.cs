using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SRolePermissions : Message
	{
		public delegate void Handler(SRolePermissions msg);

		public const int TYPE = 13634512;

		public static Handler handler;

		public HashSet<long> permissions = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634512;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(permissions.Count);
			foreach (long permission in permissions)
			{
				oc.push(permission);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				permissions.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
