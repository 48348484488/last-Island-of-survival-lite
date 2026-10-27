using System.Collections.Generic;
using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class SGetFinshCreate : Message
	{
		public delegate void Handler(SGetFinshCreate msg);

		public const int TYPE = 14683074;

		public static Handler handler;

		public List<CreateInfo> ceateInfos = new List<CreateInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683074;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(ceateInfos.Count);
			foreach (CreateInfo ceateInfo in ceateInfos)
			{
				oc.push(ceateInfo);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				CreateInfo createInfo = new CreateInfo();
				oc.pop(createInfo);
				ceateInfos.Add(createInfo);
			}
			return oc;
		}
	}
}
