using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CGetOutVehicle : Message
	{
		public delegate void Handler(CGetOutVehicle msg);

		public const int TYPE = 11537376;

		public static Handler handler;

		public long roleId;

		public int id;

		public Vec3 rolePos = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537376;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(id);
			oc.push(rolePos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			id = oc.pop_int();
			oc.pop(rolePos);
			return oc;
		}
	}
}
