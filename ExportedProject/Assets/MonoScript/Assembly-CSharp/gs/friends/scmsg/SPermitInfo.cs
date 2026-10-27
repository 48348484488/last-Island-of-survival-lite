using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SPermitInfo : Message
	{
		public delegate void Handler(SPermitInfo msg);

		public const int TYPE = 13634511;

		public static Handler handler;

		public List<OneChestPermit> ChestPermits = new List<OneChestPermit>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634511;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(ChestPermits.Count);
			foreach (OneChestPermit chestPermit in ChestPermits)
			{
				oc.push(chestPermit);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				OneChestPermit oneChestPermit = new OneChestPermit();
				oc.pop(oneChestPermit);
				ChestPermits.Add(oneChestPermit);
			}
			return oc;
		}
	}
}
