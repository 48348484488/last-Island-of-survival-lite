using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAISyncOrientation : Message
	{
		public delegate void Handler(CAISyncOrientation msg);

		public const int TYPE = 11537439;

		public static Handler handler;

		public long roleId;

		public Vec3 orientation = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537439;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			oc.pop(orientation);
			return oc;
		}
	}
}
