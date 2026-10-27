using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SEndShrinkToxicGas : Message
	{
		public delegate void Handler(SEndShrinkToxicGas msg);

		public const int TYPE = 11537391;

		public static Handler handler;

		public int nextTime;

		public int totalTime;

		public float shrinkBeginRadius;

		public float shrinkEndRadius;

		public float shrinkBeginX;

		public float shrinkBeginY;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537391;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(nextTime);
			oc.push(totalTime);
			oc.push(shrinkBeginRadius);
			oc.push(shrinkEndRadius);
			oc.push(shrinkBeginX);
			oc.push(shrinkBeginY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			nextTime = oc.pop_int();
			totalTime = oc.pop_int();
			shrinkBeginRadius = oc.pop_float();
			shrinkEndRadius = oc.pop_float();
			shrinkBeginX = oc.pop_float();
			shrinkBeginY = oc.pop_float();
			return oc;
		}
	}
}
