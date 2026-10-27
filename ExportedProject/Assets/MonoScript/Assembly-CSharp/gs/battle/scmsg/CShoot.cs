using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CShoot : Message
	{
		public delegate void Handler(CShoot msg);

		public const int TYPE = 23071676;

		public static Handler handler;

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
			return 23071676;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			return oc;
		}
	}
}
