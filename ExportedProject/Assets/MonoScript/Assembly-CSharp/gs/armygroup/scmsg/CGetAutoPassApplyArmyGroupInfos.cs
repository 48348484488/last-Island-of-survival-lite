using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CGetAutoPassApplyArmyGroupInfos : Message
	{
		public delegate void Handler(CGetAutoPassApplyArmyGroupInfos msg);

		public const int TYPE = 32508859;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508859;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
