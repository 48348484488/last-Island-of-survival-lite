using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SGetTreeByElementBuildingId : Message
	{
		public delegate void Handler(SGetTreeByElementBuildingId msg);

		public const int TYPE = 27265977;

		public static Handler handler;

		public List<CircuitryInfo> circuitryInfos = new List<CircuitryInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265977;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(circuitryInfos.Count);
			foreach (CircuitryInfo circuitryInfo in circuitryInfos)
			{
				oc.push(circuitryInfo);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				CircuitryInfo circuitryInfo = new CircuitryInfo();
				oc.pop(circuitryInfo);
				circuitryInfos.Add(circuitryInfo);
			}
			return oc;
		}
	}
}
