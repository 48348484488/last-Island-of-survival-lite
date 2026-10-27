using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGrenadeExplode : Message
	{
		public delegate void Handler(SGrenadeExplode msg);

		public const int TYPE = 11537404;

		public static Handler handler;

		public long insId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537404;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			return oc;
		}
	}
}
