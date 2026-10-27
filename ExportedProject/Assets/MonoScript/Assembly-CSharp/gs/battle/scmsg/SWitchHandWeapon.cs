using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SWitchHandWeapon : Message
	{
		public delegate void Handler(SWitchHandWeapon msg);

		public const int TYPE = 11537484;

		public static Handler handler;

		public long roleId;

		public int instanceId;

		public int weaponId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537484;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(instanceId);
			oc.push(weaponId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			instanceId = oc.pop_int();
			weaponId = oc.pop_int();
			return oc;
		}
	}
}
