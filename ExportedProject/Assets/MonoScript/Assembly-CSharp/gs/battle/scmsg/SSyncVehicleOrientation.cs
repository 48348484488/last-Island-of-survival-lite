using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncVehicleOrientation : Message
	{
		public delegate void Handler(SSyncVehicleOrientation msg);

		public const int TYPE = 11537371;

		public static Handler handler;

		public int id;

		public ShortVec3 orientation = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537371;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			oc.pop(orientation);
			return oc;
		}
	}
}
