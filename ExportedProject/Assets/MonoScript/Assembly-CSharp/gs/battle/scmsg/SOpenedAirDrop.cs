using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SOpenedAirDrop : Message
	{
		public delegate void Handler(SOpenedAirDrop msg);

		public const int TYPE = 11537461;

		public static Handler handler;

		public HashSet<int> cfgIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537461;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cfgIds.Count);
			foreach (int cfgId in cfgIds)
			{
				oc.push(cfgId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				cfgIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
