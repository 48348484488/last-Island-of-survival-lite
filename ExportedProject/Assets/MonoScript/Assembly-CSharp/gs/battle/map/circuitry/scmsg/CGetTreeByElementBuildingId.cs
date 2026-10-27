using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CGetTreeByElementBuildingId : Message
	{
		public delegate void Handler(CGetTreeByElementBuildingId msg);

		public const int TYPE = 27265976;

		public static Handler handler;

		public long elementBuildingId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265976;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(elementBuildingId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			elementBuildingId = oc.pop_long();
			return oc;
		}
	}
}
