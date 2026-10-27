using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncGunBattleAddPart : Message
	{
		public delegate void Handler(SSyncGunBattleAddPart msg);

		public const int TYPE = 11537495;

		public static Handler handler;

		public long roleId;

		public int instanceId;

		public int partId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537495;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(instanceId);
			oc.push(partId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			instanceId = oc.pop_int();
			partId = oc.pop_int();
			return oc;
		}
	}
}
