using Net;
using Share;

namespace gs.online.scmsg
{
	public class SLoginFinished : Message
	{
		public delegate void Handler(SLoginFinished msg);

		public const int TYPE = 2100160;

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
			return 2100160;
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
