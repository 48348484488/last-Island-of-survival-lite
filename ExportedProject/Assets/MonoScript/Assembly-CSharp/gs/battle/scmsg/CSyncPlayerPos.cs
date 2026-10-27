using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncPlayerPos : Message
	{
		public delegate void Handler(CSyncPlayerPos msg);

		public const int TYPE = 11537351;

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
			return 11537351;
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
