using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncVelocity : Message
	{
		public delegate void Handler(SSyncVelocity msg);

		public const int TYPE = 11537354;

		public static Handler handler;

		public long roleId;

		public ShortVec3 velocity = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537354;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(velocity);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			oc.pop(velocity);
			return oc;
		}
	}
}
