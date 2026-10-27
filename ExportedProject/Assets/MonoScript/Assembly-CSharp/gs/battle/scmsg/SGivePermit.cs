using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGivePermit : Message
	{
		public delegate void Handler(SGivePermit msg);

		public const int TYPE = 11537535;

		public static Handler handler;

		public long instanceId;

		public long roleId;

		public bool addOrRemove;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537535;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(roleId);
			oc.push(addOrRemove);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			roleId = oc.pop_long();
			addOrRemove = oc.pop_bool();
			return oc;
		}
	}
}
