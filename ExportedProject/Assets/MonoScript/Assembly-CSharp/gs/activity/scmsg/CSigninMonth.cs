using Net;
using Share;

namespace gs.activity.scmsg
{
	public class CSigninMonth : Message
	{
		public delegate void Handler(CSigninMonth msg);

		public const int TYPE = 29363132;

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
			return 29363132;
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
