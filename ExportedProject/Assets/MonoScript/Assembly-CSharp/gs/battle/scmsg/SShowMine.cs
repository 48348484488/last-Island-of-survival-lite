using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SShowMine : Message
	{
		public delegate void Handler(SShowMine msg);

		public const int TYPE = 11537492;

		public static Handler handler;

		public long instanceId;

		public int mineTypeId;

		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537492;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(mineTypeId);
			oc.push(pos);
			oc.push(orientation);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			mineTypeId = oc.pop_int();
			oc.pop(pos);
			oc.pop(orientation);
			hp = oc.pop_int();
			return oc;
		}
	}
}
