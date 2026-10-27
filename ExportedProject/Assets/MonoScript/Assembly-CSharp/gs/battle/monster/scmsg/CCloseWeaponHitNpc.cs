using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class CCloseWeaponHitNpc : Message
	{
		public delegate void Handler(CCloseWeaponHitNpc msg);

		public const int TYPE = 28314571;

		public static Handler handler;

		public long instanceId;

		public byte bodyPart;

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
			return 28314571;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(bodyPart);
			oc.push(forward);
			oc.push(hitPos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			bodyPart = oc.pop_byte();
			oc.pop(forward);
			oc.pop(hitPos);
			return oc;
		}
	}
}
