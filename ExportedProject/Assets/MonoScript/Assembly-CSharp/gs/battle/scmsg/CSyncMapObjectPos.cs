using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncMapObjectPos : Message
	{
		public delegate void Handler(CSyncMapObjectPos msg);

		public const int TYPE = 11537402;

		public static Handler handler;

		public long insId;

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
			return 11537402;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(pos);
			oc.push(rotation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			oc.pop(pos);
			oc.pop(rotation);
			return oc;
		}
	}
}
