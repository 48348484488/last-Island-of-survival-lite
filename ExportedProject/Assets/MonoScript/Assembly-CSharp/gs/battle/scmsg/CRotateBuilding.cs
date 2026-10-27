using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CRotateBuilding : Message
	{
		public delegate void Handler(CRotateBuilding msg);

		public const int TYPE = 11537477;

		public static Handler handler;

		public long buildingId;

		public Dictionary<long, byte> outmap = new Dictionary<long, byte>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537477;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildingId);
			oc.push(outmap.Count);
			foreach (KeyValuePair<long, byte> item in outmap)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			buildingId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				outmap.Add(oc.pop_long(), oc.pop_byte());
			}
			return oc;
		}
	}
}
