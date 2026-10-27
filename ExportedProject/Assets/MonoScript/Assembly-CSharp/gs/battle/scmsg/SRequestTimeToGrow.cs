using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SRequestTimeToGrow : Message
	{
		public delegate void Handler(SRequestTimeToGrow msg);

		public const int TYPE = 11537539;

		public static Handler handler;

		public long instanceId;

		public int time;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537539;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(time);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			time = oc.pop_int();
			return oc;
		}
	}
}
