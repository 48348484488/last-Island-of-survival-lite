using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COpenParachute : Message
	{
		public delegate void Handler(COpenParachute msg);

		public const int TYPE = 11537429;

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
			return 11537429;
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
