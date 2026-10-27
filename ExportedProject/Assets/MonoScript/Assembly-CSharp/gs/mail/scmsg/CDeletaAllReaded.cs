using Net;
using Share;

namespace gs.mail.scmsg
{
	public class CDeletaAllReaded : Message
	{
		public delegate void Handler(CDeletaAllReaded msg);

		public const int TYPE = 5245887;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245887;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
