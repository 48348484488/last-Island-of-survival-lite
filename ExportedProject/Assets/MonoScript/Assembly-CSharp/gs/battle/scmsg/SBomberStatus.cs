using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBomberStatus : Message
	{
		public delegate void Handler(SBomberStatus msg);

		public const int TYPE = 11537420;

		public static Handler handler;

		public const int Appear = 1;

		public const int Bomb = 3;

		public const int End = 4;

		public int cfgId;

		public int curStatus;

		public Vec2 centerPoint = new Vec2();

		public float airRadius;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537420;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cfgId);
			oc.push(curStatus);
			oc.push(centerPoint);
			oc.push(airRadius);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cfgId = oc.pop_int();
			curStatus = oc.pop_int();
			oc.pop(centerPoint);
			airRadius = oc.pop_float();
			return oc;
		}
	}
}
