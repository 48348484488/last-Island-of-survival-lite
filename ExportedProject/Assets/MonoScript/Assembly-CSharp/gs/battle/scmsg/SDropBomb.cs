using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SDropBomb : Message
	{
		public delegate void Handler(SDropBomb msg);

		public const int TYPE = 11537421;

		public static Handler handler;

		public Vec2 pos = new Vec2();

		public float bombRadius;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537421;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(bombRadius);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			bombRadius = oc.pop_float();
			return oc;
		}
	}
}
