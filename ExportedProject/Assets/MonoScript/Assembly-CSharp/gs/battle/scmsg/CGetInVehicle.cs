using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CGetInVehicle : Message
	{
		public delegate void Handler(CGetInVehicle msg);

		public const int TYPE = 11537374;

		public static Handler handler;

		public long roleId;

		public int id;

		public int seat;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537374;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(id);
			oc.push(seat);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			id = oc.pop_int();
			seat = oc.pop_int();
			return oc;
		}
	}
}
