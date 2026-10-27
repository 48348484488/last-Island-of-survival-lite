using System.Collections.Generic;
using Net;
using Share;

namespace gs.online.scmsg
{
	public class SAlert : Message
	{
		public delegate void Handler(SAlert msg);

		public const int TYPE = 2100162;

		public static Handler handler;

		public int strId;

		public List<string> args = new List<string>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100162;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(strId);
			oc.push(args.Count);
			foreach (string arg in args)
			{
				oc.push(arg);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			strId = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				args.Add(oc.pop_string());
			}
			return oc;
		}
	}
}
