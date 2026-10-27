using Net;
using Share;

namespace gs.mail.scmsg
{
	public class CGetAllMailReward : Message
	{
		public delegate void Handler(CGetAllMailReward msg);

		public const int TYPE = 5245885;

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
			return 5245885;
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
