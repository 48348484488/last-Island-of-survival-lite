using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CBombDamage : Message
	{
		public delegate void Handler(CBombDamage msg);

		public const int TYPE = 23071674;

		public static Handler handler;

		public long insId;

		public List<long> damagedList = new List<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071674;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(damagedList.Count);
			foreach (long damaged in damagedList)
			{
				oc.push(damaged);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				damagedList.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
