using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncCameraLookPoint : Message
	{
		public delegate void Handler(SSyncCameraLookPoint msg);

		public const int TYPE = 11537417;

		public static Handler handler;

		public long roleId;

		public Vec3 point = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537417;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(point);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			oc.pop(point);
			return oc;
		}
	}
}
