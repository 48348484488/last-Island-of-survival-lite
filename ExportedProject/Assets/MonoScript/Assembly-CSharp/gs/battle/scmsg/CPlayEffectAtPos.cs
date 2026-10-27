using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CPlayEffectAtPos : Message
	{
		public delegate void Handler(CPlayEffectAtPos msg);

		public const int TYPE = 11537380;

		public static Handler handler;

		public int effectId;

		public Vec3 pos = new Vec3();

		public Vec3 forward = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537380;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(effectId);
			oc.push(pos);
			oc.push(forward);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			effectId = oc.pop_int();
			oc.pop(pos);
			oc.pop(forward);
			return oc;
		}
	}
}
