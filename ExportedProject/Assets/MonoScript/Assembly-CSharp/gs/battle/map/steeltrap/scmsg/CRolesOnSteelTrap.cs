using Net;
using Share;

namespace gs.battle.map.steeltrap.scmsg
{
	public class CRolesOnSteelTrap : Message
	{
		public delegate void Handler(CRolesOnSteelTrap msg);

		public const int TYPE = 25168824;

		public static Handler handler;

		public long steelTrapId;

		public long onRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 25168824;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(steelTrapId);
			oc.push(onRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			steelTrapId = oc.pop_long();
			onRoleId = oc.pop_long();
			return oc;
		}
	}
}
