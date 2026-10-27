using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBuildingHpChange : Message
	{
		public delegate void Handler(SBuildingHpChange msg);

		public const int TYPE = 11537540;

		public static Handler handler;

		public long instanceId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537540;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			hp = oc.pop_int();
			return oc;
		}
	}
}
