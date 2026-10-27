using Net;
using Share;

namespace gs.facilitybuilding.scmsg
{
	public class CGetFuel : Message
	{
		public delegate void Handler(CGetFuel msg);

		public const int TYPE = 31460287;

		public static Handler handler;

		public long facilityBuildingId;

		public int getIndex;

		public bool isAll;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 31460287;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(facilityBuildingId);
			oc.push(getIndex);
			oc.push(isAll);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			facilityBuildingId = oc.pop_long();
			getIndex = oc.pop_int();
			isAll = oc.pop_bool();
			return oc;
		}
	}
}
