using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class SControllMonster : Message
	{
		public delegate void Handler(SControllMonster msg);

		public const int TYPE = 28314557;

		public static Handler handler;

		public long instanceId;

		public long roleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314557;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			roleId = oc.pop_long();
			return oc;
		}
	}
}
