using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CGetRecommendArmyGroupInfos : Message
	{
		public delegate void Handler(CGetRecommendArmyGroupInfos msg);

		public const int TYPE = 32508858;

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
			return 32508858;
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
