using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CAddPermissions : Message
	{
		public delegate void Handler(CAddPermissions msg);

		public const int TYPE = 22023103;

		public static Handler handler;

		public long turretId;

		public List<long> targetIds = new List<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023103;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(targetIds.Count);
			foreach (long targetId in targetIds)
			{
				oc.push(targetId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				targetIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
