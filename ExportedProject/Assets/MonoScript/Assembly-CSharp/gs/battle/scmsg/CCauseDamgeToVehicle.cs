using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CCauseDamgeToVehicle : Message
	{
		public delegate void Handler(CCauseDamgeToVehicle msg);

		public const int TYPE = 23071694;

		public static Handler handler;

		public int vehicleId;

		public int damage;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071694;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(damage);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			damage = oc.pop_int();
			return oc;
		}
	}
}
