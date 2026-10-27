using Net;
using Share;

namespace gs.mail.scmsg
{
	public class SAddmail : Message
	{
		public delegate void Handler(SAddmail msg);

		public const int TYPE = 5245884;

		public static Handler handler;

		public MailInfo info = new MailInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245884;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(info);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			return oc;
		}
	}
}
