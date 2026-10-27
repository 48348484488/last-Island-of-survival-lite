using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SHitPlayer : Message
	{
		public delegate void Handler(SHitPlayer msg);

		public const int TYPE = 23071687;

		public static Handler handler;

		public byte bodyPart;

		public long roleId;

		public ShortVec3 forward = new ShortVec3();

		public ShortVec3 hitPos = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071687;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(bodyPart);
			oc.push(roleId);
			oc.push(forward);
			oc.push(hitPos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			bodyPart = oc.pop_byte();
			roleId = oc.pop_long();
			oc.pop(forward);
			oc.pop(hitPos);
			return oc;
		}
	}
}
