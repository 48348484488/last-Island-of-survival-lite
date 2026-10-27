using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CCloseWeaponHitBuilding : Message
	{
		public delegate void Handler(CCloseWeaponHitBuilding msg);

		public const int TYPE = 23071680;

		public static Handler handler;

		public long buildingInsId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071680;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildingInsId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			buildingInsId = oc.pop_long();
			return oc;
		}
	}
}
