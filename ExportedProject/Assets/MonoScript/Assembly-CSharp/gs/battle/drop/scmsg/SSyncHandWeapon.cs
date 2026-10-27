using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SSyncHandWeapon : Message
	{
		public delegate void Handler(SSyncHandWeapon msg);

		public const int TYPE = 12585916;

		public static Handler handler;

		public long roleId;

		public int handWeapon;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585916;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(handWeapon);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			handWeapon = oc.pop_int();
			return oc;
		}
	}
}
