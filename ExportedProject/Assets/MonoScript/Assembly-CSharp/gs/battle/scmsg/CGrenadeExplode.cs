using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CGrenadeExplode : Message
	{
		public delegate void Handler(CGrenadeExplode msg);

		public const int TYPE = 11537404;

		public static Handler handler;

		public int battleObjectId;

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
			return 11537404;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(battleObjectId);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			battleObjectId = oc.pop_int();
			oc.pop(pos);
			return oc;
		}
	}
}
