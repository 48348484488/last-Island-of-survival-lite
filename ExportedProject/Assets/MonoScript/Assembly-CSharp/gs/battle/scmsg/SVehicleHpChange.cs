using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SVehicleHpChange : Message
	{
		public delegate void Handler(SVehicleHpChange msg);

		public const int TYPE = 11537407;

		public static Handler handler;

		public int vehicleId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537407;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			hp = oc.pop_int();
			return oc;
		}
	}
}
