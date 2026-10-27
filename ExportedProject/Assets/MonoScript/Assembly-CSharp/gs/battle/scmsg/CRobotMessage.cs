using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CRobotMessage : Message
	{
		public delegate void Handler(CRobotMessage msg);

		public const int TYPE = 11537454;

		public static Handler handler;

		public long robotId;

		public Octets oct = new Octets();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537454;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(robotId);
			oc.push(oct);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			robotId = oc.pop_long();
			oct = oc.pop_octets();
			return oc;
		}
	}
}
