using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SFlyBattlePlane : Message
	{
		public delegate void Handler(SFlyBattlePlane msg);

		public const int TYPE = 11537422;

		public static Handler handler;

		public Vec3 startPos = new Vec3();

		public Vec3 endPos = new Vec3();

		public float speed;

		public int curFlyTime;

		public int openDoorTime;

		public int curNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537422;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(startPos);
			oc.push(endPos);
			oc.push(speed);
			oc.push(curFlyTime);
			oc.push(openDoorTime);
			oc.push(curNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(startPos);
			oc.pop(endPos);
			speed = oc.pop_float();
			curFlyTime = oc.pop_int();
			openDoorTime = oc.pop_int();
			curNum = oc.pop_int();
			return oc;
		}
	}
}
