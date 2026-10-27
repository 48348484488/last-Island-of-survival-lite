using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CRequestNewbeeMonster : Message
	{
		public delegate void Handler(CRequestNewbeeMonster msg);

		public const int TYPE = 11537553;

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
			return 11537553;
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
