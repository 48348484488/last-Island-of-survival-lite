using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncVehicleVelocity : Message
	{
		public delegate void Handler(SSyncVehicleVelocity msg);

		public const int TYPE = 11537373;

		public static Handler handler;

		public int id;

		public ShortVec3 velocity = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537373;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(velocity);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			oc.pop(velocity);
			return oc;
		}
	}
}
