using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CDissolveArmyGroup : Message
	{
		public delegate void Handler(CDissolveArmyGroup msg);

		public const int TYPE = 32508869;

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
			return 32508869;
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
