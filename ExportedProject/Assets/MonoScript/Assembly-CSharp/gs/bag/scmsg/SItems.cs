using System.Collections.Generic;
using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SItems : Message
	{
		public delegate void Handler(SItems msg);

		public const int TYPE = 8391609;

		public static Handler handler;

		public int capacity;

		public List<BagItem> bagItems = new List<BagItem>();

		public List<BagItem> equipItems = new List<BagItem>();

		public List<BagItem> skinItems = new List<BagItem>();

		public Dictionary<int, BagItem> quickUseItems = new Dictionary<int, BagItem>();

		public bool showEquip;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391609;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(capacity);
			oc.push(bagItems.Count);
			foreach (BagItem bagItem in bagItems)
			{
				oc.push(bagItem);
			}
			oc.push(equipItems.Count);
			foreach (BagItem equipItem in equipItems)
			{
				oc.push(equipItem);
			}
			oc.push(skinItems.Count);
			foreach (BagItem skinItem in skinItems)
			{
				oc.push(skinItem);
			}
			oc.push(quickUseItems.Count);
			foreach (KeyValuePair<int, BagItem> quickUseItem in quickUseItems)
			{
				oc.push(quickUseItem.Key);
				oc.push(quickUseItem.Value);
			}
			oc.push(showEquip);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			capacity = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				BagItem bagItem = new BagItem();
				oc.pop(bagItem);
				bagItems.Add(bagItem);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				BagItem bagItem2 = new BagItem();
				oc.pop(bagItem2);
				equipItems.Add(bagItem2);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				BagItem bagItem3 = new BagItem();
				oc.pop(bagItem3);
				skinItems.Add(bagItem3);
			}
			int l = 0;
			for (int num4 = oc.pop_int(); l < num4; l++)
			{
				int key = oc.pop_int();
				BagItem bagItem4 = new BagItem();
				oc.pop(bagItem4);
				quickUseItems.Add(key, bagItem4);
			}
			showEquip = oc.pop_bool();
			return oc;
		}
	}
}
