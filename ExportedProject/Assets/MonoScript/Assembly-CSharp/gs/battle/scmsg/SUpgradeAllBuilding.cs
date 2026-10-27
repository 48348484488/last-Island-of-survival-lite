using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SUpgradeAllBuilding : Message
	{
		public delegate void Handler(SUpgradeAllBuilding msg);

		public const int TYPE = 11537558;

		public static Handler handler;

		public long toolBoxId;

		public int level;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537558;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(toolBoxId);
			oc.push(level);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			toolBoxId = oc.pop_long();
			level = oc.pop_int();
			return oc;
		}
	}
}
