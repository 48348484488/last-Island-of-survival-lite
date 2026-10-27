using System.Collections.Generic;
using Net;
using Share;

namespace gs.mail.scmsg
{
	public class SDeletaAllReaded : Message
	{
		public delegate void Handler(SDeletaAllReaded msg);

		public const int TYPE = 5245888;

		public static Handler handler;

		public HashSet<int> ids = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245888;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(ids.Count);
			foreach (int id in ids)
			{
				oc.push(id);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ids.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
