using Net;
using Share;

namespace gs.facilitybuilding.scmsg
{
	public class CStop : Message
	{
		public delegate void Handler(CStop msg);

		public const int TYPE = 31460285;

		public static Handler handler;

		public long facilityBuildingId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 31460285;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(facilityBuildingId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			facilityBuildingId = oc.pop_long();
			return oc;
		}
	}
}
