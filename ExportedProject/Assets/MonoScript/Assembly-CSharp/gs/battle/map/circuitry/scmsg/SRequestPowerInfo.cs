using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SRequestPowerInfo : Message
	{
		public delegate void Handler(SRequestPowerInfo msg);

		public const int TYPE = 27265979;

		public static Handler handler;

		public long powerId;

		public Dictionary<byte, PowerFuleInfo> fuels = new Dictionary<byte, PowerFuleInfo>();

		public int remainTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265979;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(powerId);
			oc.push(fuels.Count);
			foreach (KeyValuePair<byte, PowerFuleInfo> fuel in fuels)
			{
				oc.push(fuel.Key);
				oc.push(fuel.Value);
			}
			oc.push(remainTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			powerId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				byte key = oc.pop_byte();
				PowerFuleInfo powerFuleInfo = new PowerFuleInfo();
				oc.pop(powerFuleInfo);
				fuels.Add(key, powerFuleInfo);
			}
			remainTime = oc.pop_int();
			return oc;
		}
	}
}
