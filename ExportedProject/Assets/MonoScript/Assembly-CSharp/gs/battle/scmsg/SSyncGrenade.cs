using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncGrenade : Message
	{
		public delegate void Handler(SSyncGrenade msg);

		public const int TYPE = 11537403;

		public static Handler handler;

		public int battleObjectId;

		public Vec3 pos = new Vec3();

		public Vec3 rotation = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537403;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(battleObjectId);
			oc.push(pos);
			oc.push(rotation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			battleObjectId = oc.pop_int();
			oc.pop(pos);
			oc.pop(rotation);
			return oc;
		}
	}
}
