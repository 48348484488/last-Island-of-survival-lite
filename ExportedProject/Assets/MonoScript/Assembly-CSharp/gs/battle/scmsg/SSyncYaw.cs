using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncYaw : Message
	{
		public delegate void Handler(SSyncYaw msg);

		public const int TYPE = 11537415;

		public static Handler handler;

		public long roleId;

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
			return 11537415;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(yaw);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			yaw = oc.pop_float();
			return oc;
		}
	}
}
