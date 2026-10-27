using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COtherCloseWeaponHitPlayer : Message
	{
		public delegate void Handler(COtherCloseWeaponHitPlayer msg);

		public const int TYPE = 23071686;

		public static Handler handler;

		public long otherInsId;

		public long roleId;

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
			return 23071686;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherInsId);
			oc.push(roleId);
			oc.push(bodyPart);
			oc.push(forward);
			oc.push(hitPos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherInsId = oc.pop_long();
			roleId = oc.pop_long();
			bodyPart = oc.pop_byte();
			oc.pop(forward);
			oc.pop(hitPos);
			return oc;
		}
	}
}
