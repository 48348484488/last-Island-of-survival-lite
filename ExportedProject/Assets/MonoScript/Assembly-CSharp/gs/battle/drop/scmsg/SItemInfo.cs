using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.drop.scmsg
{
	public class SItemInfo : Message
	{
		public delegate void Handler(SItemInfo msg);

		public const int TYPE = 12585912;

		public static Handler handler;

		public ItemInfo itemInfo = new ItemInfo();

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
			return 12585912;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemInfo);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(itemInfo);
			oc.pop(pos);
			return oc;
		}
	}
}
