using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGrenade : Message
	{
		public delegate void Handler(SGrenade msg);

		public const int TYPE = 11537401;

		public static Handler handler;

		public long roleId;

		public int battleObjectId;

		public int id;

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
			return 11537401;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(battleObjectId);
			oc.push(id);
			oc.push(pos);
			oc.push(rotation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			battleObjectId = oc.pop_int();
			id = oc.pop_int();
			oc.pop(pos);
			oc.pop(rotation);
			return oc;
		}
	}
}
