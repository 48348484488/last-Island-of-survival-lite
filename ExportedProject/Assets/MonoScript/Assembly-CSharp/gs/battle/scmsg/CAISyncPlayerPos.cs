using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAISyncPlayerPos : Message
	{
		public delegate void Handler(CAISyncPlayerPos msg);

		public const int TYPE = 11537442;

		public static Handler handler;

		public long roleId;

		public Vec3 pos = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537442;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			oc.pop(pos);
			return oc;
		}
	}
}
