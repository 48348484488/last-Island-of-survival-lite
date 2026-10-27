using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SToxicGasZone : Message
	{
		public delegate void Handler(SToxicGasZone msg);

		public const int TYPE = 11537392;

		public static Handler handler;

		public Zone zone = new Zone();

		public float speed;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537392;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(zone);
			oc.push(speed);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(zone);
			speed = oc.pop_float();
			return oc;
		}
	}
}
