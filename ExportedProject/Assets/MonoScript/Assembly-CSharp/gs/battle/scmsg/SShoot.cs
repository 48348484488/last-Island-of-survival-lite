using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SShoot : Message
	{
		public delegate void Handler(SShoot msg);

		public const int TYPE = 23071678;

		public static Handler handler;

		public const int HasFireEffect = 1;

		public const int HasSound = 2;

		public long insId;

		public byte state;

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
			return 23071678;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(state);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			state = oc.pop_byte();
			oc.pop(pos);
			return oc;
		}
	}
}
