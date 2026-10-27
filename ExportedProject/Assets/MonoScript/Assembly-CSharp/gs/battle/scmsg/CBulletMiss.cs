using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CBulletMiss : Message
	{
		public delegate void Handler(CBulletMiss msg);

		public const int TYPE = 23071688;

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
			return 23071688;
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
