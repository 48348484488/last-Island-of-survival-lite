using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SFlyPlaneOther : Message
	{
		public delegate void Handler(SFlyPlaneOther msg);

		public const int TYPE = 11537453;

		public static Handler handler;

		public Vec3 startPos = new Vec3();

		public Vec3 endPos = new Vec3();

		public float speed;

		public int curFlyTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537453;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(startPos);
			oc.push(endPos);
			oc.push(speed);
			oc.push(curFlyTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(startPos);
			oc.pop(endPos);
			speed = oc.pop_float();
			curFlyTime = oc.pop_int();
			return oc;
		}
	}
}
