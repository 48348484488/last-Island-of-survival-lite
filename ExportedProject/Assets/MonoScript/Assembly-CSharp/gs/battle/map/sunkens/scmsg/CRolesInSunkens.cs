using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.map.sunkens.scmsg
{
	public class CRolesInSunkens : Message
	{
		public delegate void Handler(CRolesInSunkens msg);

		public const int TYPE = 24120248;

		public static Handler handler;

		public long sunkensId;

		public HashSet<long> inRoleIds = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 24120248;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(sunkensId);
			oc.push(inRoleIds.Count);
			foreach (long inRoleId in inRoleIds)
			{
				oc.push(inRoleId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			sunkensId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				inRoleIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
