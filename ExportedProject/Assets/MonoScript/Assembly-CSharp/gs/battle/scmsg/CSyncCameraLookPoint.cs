using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncCameraLookPoint : Message
	{
		public delegate void Handler(CSyncCameraLookPoint msg);

		public const int TYPE = 11537416;

		public static Handler handler;

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
			return 11537416;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(point);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(point);
			return oc;
		}
	}
}
