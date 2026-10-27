using System.Collections.Generic;
using Net;
using Share;

namespace gs.mail.scmsg
{
	public class SMailInfo : Message
	{
		public delegate void Handler(SMailInfo msg);

		public const int TYPE = 5245880;

		public static Handler handler;

		public List<MailInfo> infos = new List<MailInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245880;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(infos.Count);
			foreach (MailInfo info in infos)
			{
				oc.push(info);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				MailInfo mailInfo = new MailInfo();
				oc.pop(mailInfo);
				infos.Add(mailInfo);
			}
			return oc;
		}
	}
}
