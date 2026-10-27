using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SFacilityBuildingUpdate : Message
	{
		public delegate void Handler(SFacilityBuildingUpdate msg);

		public const int TYPE = 11537554;

		public static Handler handler;

		public long id;

		public long operationRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537554;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(operationRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			operationRoleId = oc.pop_long();
			return oc;
		}
	}
}
