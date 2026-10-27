using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CGetArmyGroupInfo : Message
	{
		public delegate void Handler(CGetArmyGroupInfo msg);

		public const int TYPE = 32508861;

		public static Handler handler;

		public long armyGroupId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508861;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(armyGroupId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			armyGroupId = oc.pop_long();
			return oc;
		}
	}
}
