using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CGetTeamEarningReward : Message
	{
		public delegate void Handler(CGetTeamEarningReward msg);

		public const int TYPE = 15731660;

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
			return 15731660;
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
