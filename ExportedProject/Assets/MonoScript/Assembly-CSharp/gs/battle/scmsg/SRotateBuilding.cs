using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SRotateBuilding : Message
	{
		public delegate void Handler(SRotateBuilding msg);

		public const int TYPE = 11537478;

		public static Handler handler;

		public long buildingId;

		public float eulerY;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537478;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildingId);
			oc.push(eulerY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			buildingId = oc.pop_long();
			eulerY = oc.pop_float();
			return oc;
		}
	}
}
