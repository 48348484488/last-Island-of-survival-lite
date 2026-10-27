using System.Collections.Generic;
using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.drop.scmsg
{
	public class SItemBoxInfo : Message
	{
		public delegate void Handler(SItemBoxInfo msg);

		public const int TYPE = 12585922;

		public static Handler handler;

		public long objId;

		public Vec3 pos = new Vec3();

		public List<ItemInfo> items = new List<ItemInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585922;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(objId);
			oc.push(pos);
			oc.push(items.Count);
			foreach (ItemInfo item in items)
			{
				oc.push(item);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			objId = oc.pop_long();
			oc.pop(pos);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ItemInfo itemInfo = new ItemInfo();
				oc.pop(itemInfo);
				items.Add(itemInfo);
			}
			return oc;
		}
	}
}
