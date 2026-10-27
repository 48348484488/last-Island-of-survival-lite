using System.Collections.Generic;
using Net;
using Share;

namespace gs.mail.scmsg
{
	public class SGetAllMailReward : Message
	{
		public delegate void Handler(SGetAllMailReward msg);

		public const int TYPE = 5245886;

		public static Handler handler;

		public HashSet<int> isReads = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245886;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(isReads.Count);
			foreach (int isRead in isReads)
			{
				oc.push(isRead);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				isReads.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
