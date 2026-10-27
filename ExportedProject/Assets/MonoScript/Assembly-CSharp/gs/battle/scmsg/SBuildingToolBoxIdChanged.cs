using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBuildingToolBoxIdChanged : Message
	{
		public delegate void Handler(SBuildingToolBoxIdChanged msg);

		public const int TYPE = 11537525;

		public static Handler handler;

		public long buildingId;

		public long toolBoxId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537525;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildingId);
			oc.push(toolBoxId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			buildingId = oc.pop_long();
			toolBoxId = oc.pop_long();
			return oc;
		}
	}
}
