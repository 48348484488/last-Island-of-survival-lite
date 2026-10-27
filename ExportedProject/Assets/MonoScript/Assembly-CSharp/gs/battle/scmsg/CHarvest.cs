using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CHarvest : Message
	{
		public delegate void Handler(CHarvest msg);

		public const int TYPE = 11537501;

		public static Handler handler;

		public long instanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537501;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			return oc;
		}
	}
}
