using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CLeaveOutArmyGroup : Message
	{
		public delegate void Handler(CLeaveOutArmyGroup msg);

		public const int TYPE = 32508871;

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
			return 32508871;
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
