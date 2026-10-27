using System.Collections.Generic;
using Net;
using Share;
using gs.battle.drop.scmsg;

namespace gs.battle.scmsg
{
	public class SSelfDie : Message
	{
		public delegate void Handler(SSelfDie msg);

		public const int TYPE = 11537527;

		public static Handler handler;

		public HashSet<ItemInfo> dropItems = new HashSet<ItemInfo>();

		public Vec3 pos = new Vec3();

		public int boxLeftTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537527;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(dropItems.Count);
			foreach (ItemInfo dropItem in dropItems)
			{
				oc.push(dropItem);
			}
			oc.push(pos);
			oc.push(boxLeftTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ItemInfo itemInfo = new ItemInfo();
				oc.pop(itemInfo);
				dropItems.Add(itemInfo);
			}
			oc.pop(pos);
			boxLeftTime = oc.pop_int();
			return oc;
		}
	}
}
