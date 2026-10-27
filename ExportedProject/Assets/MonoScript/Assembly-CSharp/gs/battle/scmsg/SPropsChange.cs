using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SPropsChange : Message
	{
		public delegate void Handler(SPropsChange msg);

		public const int TYPE = 11537360;

		public static Handler handler;

		public List<int> props = new List<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537360;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(props.Count);
			foreach (int prop in props)
			{
				oc.push(prop);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				props.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
