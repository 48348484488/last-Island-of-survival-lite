using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CPlant : Message
	{
		public delegate void Handler(CPlant msg);

		public const int TYPE = 11537497;

		public static Handler handler;

		public int plantId;

		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537497;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(plantId);
			oc.push(pos);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			plantId = oc.pop_int();
			oc.pop(pos);
			oc.pop(orientation);
			return oc;
		}
	}
}
