using System.Collections.Generic;
using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SOpenBox : Message
	{
		public delegate void Handler(SOpenBox msg);

		public const int TYPE = 8391641;

		public static Handler handler;

		public long boxId;

		public List<BagItem> bagItems = new List<BagItem>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391641;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boxId);
			oc.push(bagItems.Count);
			foreach (BagItem bagItem in bagItems)
			{
				oc.push(bagItem);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boxId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				BagItem bagItem = new BagItem();
				oc.pop(bagItem);
				bagItems.Add(bagItem);
			}
			return oc;
		}
	}
}
