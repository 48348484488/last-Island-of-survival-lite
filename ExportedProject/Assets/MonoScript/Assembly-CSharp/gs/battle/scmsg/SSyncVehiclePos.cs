using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncVehiclePos : Message
	{
		public delegate void Handler(SSyncVehiclePos msg);

		public const int TYPE = 11537369;

		public static Handler handler;

		public int id;

		public Vec3 pos = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537369;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			oc.pop(pos);
			return oc;
		}
	}
}
