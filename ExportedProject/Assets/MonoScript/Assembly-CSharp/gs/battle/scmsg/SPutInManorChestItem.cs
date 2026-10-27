using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SPutInManorChestItem : Message
	{
		public delegate void Handler(SPutInManorChestItem msg);

		public const int TYPE = 11537533;

		public static Handler handler;

		public long chestInstanceId;

		public int itemId;

		public Dictionary<int, int> items = new Dictionary<int, int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537533;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(chestInstanceId);
			oc.push(itemId);
			oc.push(items.Count);
			foreach (KeyValuePair<int, int> item in items)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			chestInstanceId = oc.pop_long();
			itemId = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				items.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}
