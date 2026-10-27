using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncYaw : Message
	{
		public delegate void Handler(CSyncYaw msg);

		public const int TYPE = 11537414;

		public static Handler handler;

		public float yaw;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537414;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(yaw);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			yaw = oc.pop_float();
			return oc;
		}
	}
}
