using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.drop.scmsg
{
	public class STrashCan : Message
	{
		public delegate void Handler(STrashCan msg);

		public const int TYPE = 12585927;

		public static Handler handler;

		public long objId;

		public int type;

		public Vec3 pos = new Vec3();

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
			return 12585927;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(objId);
			oc.push(type);
			oc.push(pos);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			objId = oc.pop_long();
			type = oc.pop_int();
			oc.pop(pos);
			hp = oc.pop_int();
			return oc;
		}
	}
}
