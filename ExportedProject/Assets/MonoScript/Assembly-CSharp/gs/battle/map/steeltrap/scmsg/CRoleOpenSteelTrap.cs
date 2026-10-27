using Net;
using Share;

namespace gs.battle.map.steeltrap.scmsg
{
	public class CRoleOpenSteelTrap : Message
	{
		public delegate void Handler(CRoleOpenSteelTrap msg);

		public const int TYPE = 25168825;

		public static Handler handler;

		public long steelTrapId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 25168825;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(steelTrapId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			steelTrapId = oc.pop_long();
			return oc;
		}
	}
}
