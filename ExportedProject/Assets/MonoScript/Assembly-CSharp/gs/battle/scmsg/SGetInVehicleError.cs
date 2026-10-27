using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGetInVehicleError : Message
	{
		public delegate void Handler(SGetInVehicleError msg);

		public const int TYPE = 11537434;

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
			return 11537434;
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
