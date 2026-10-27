using System.Collections.Generic;
using Net;
using Share;

namespace gs.smelter.scmsg
{
	public class CAddFuel : Message
	{
		public delegate void Handler(CAddFuel msg);

		public const int TYPE = 17828795;

		public static Handler handler;

		public long smelterId;

		public Dictionary<int, UseItem> fuels = new Dictionary<int, UseItem>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 17828795;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(smelterId);
			oc.push(fuels.Count);
			foreach (KeyValuePair<int, UseItem> fuel in fuels)
			{
				oc.push(fuel.Key);
				oc.push(fuel.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			smelterId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				fuels.Add(key, useItem);
			}
			return oc;
		}
	}
}
