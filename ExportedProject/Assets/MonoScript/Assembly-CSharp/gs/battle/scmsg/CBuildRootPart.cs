using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CBuildRootPart : Message
	{
		public delegate void Handler(CBuildRootPart msg);

		public const int TYPE = 11537472;

		public static Handler handler;

		public int typeId;

		public Vec3 pos = new Vec3();

		public float eulerY;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537472;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(typeId);
			oc.push(pos);
			oc.push(eulerY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			typeId = oc.pop_int();
			oc.pop(pos);
			eulerY = oc.pop_float();
			return oc;
		}
	}
}
