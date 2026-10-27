using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncAnimatorSpeed : Message
	{
		public delegate void Handler(SSyncAnimatorSpeed msg);

		public const int TYPE = 11537413;

		public static Handler handler;

		public long roleId;

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
			return 11537413;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(speed);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			speed = oc.pop_float();
			return oc;
		}
	}
}
