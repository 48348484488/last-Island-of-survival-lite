using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COtherCloseWeaponHitNoLiveObj : Message
	{
		public delegate void Handler(COtherCloseWeaponHitNoLiveObj msg);

		public const int TYPE = 23071682;

		public static Handler handler;

		public long otherInsId;

		public long instanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071682;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherInsId);
			oc.push(instanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherInsId = oc.pop_long();
			instanceId = oc.pop_long();
			return oc;
		}
	}
}
