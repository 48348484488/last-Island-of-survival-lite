using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSafeZone : Message
	{
		public delegate void Handler(SSafeZone msg);

		public const int TYPE = 11537393;

		public static Handler handler;

		public Zone zone = new Zone();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537393;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(zone);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(zone);
			return oc;
		}
	}
}
