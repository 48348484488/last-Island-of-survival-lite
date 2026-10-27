using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CFixBuilding : Message
	{
		public delegate void Handler(CFixBuilding msg);

		public const int TYPE = 11537541;

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
			return 11537541;
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
