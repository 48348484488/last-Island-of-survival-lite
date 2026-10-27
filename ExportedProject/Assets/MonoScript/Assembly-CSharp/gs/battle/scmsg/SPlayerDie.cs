using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SPlayerDie : Message
	{
		public delegate void Handler(SPlayerDie msg);

		public const int TYPE = 11537341;

		public static Handler handler;

		public long roleId;

		public byte bodyPart;

		public Vec3 forward = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537341;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(bodyPart);
			oc.push(forward);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			bodyPart = oc.pop_byte();
			oc.pop(forward);
			return oc;
		}
	}
}
