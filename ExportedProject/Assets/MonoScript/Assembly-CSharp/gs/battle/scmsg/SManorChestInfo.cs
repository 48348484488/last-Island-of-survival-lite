using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SManorChestInfo : Message
	{
		public delegate void Handler(SManorChestInfo msg);

		public const int TYPE = 11537529;

		public static Handler handler;

		public long instanceId;

		public Dictionary<int, ManorChestItem> items = new Dictionary<int, ManorChestItem>();

		public Dictionary<int, int> expend = new Dictionary<int, int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537529;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(items.Count);
			foreach (KeyValuePair<int, ManorChestItem> item in items)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(expend.Count);
			foreach (KeyValuePair<int, int> item2 in expend)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				ManorChestItem manorChestItem = new ManorChestItem();
				oc.pop(manorChestItem);
				items.Add(key, manorChestItem);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				expend.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}
