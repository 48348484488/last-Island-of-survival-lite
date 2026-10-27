using System.Collections.Generic;
using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SDropedItemsDuringOffline : Message
	{
		public delegate void Handler(SDropedItemsDuringOffline msg);

		public const int TYPE = 8391656;

		public static Handler handler;

		public Dictionary<int, int> dropedItems = new Dictionary<int, int>();

		public float diePosX;

		public float diePosY;

		public float diePosZ;

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
			return 8391656;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(dropedItems.Count);
			foreach (KeyValuePair<int, int> dropedItem in dropedItems)
			{
				oc.push(dropedItem.Key);
				oc.push(dropedItem.Value);
			}
			oc.push(diePosX);
			oc.push(diePosY);
			oc.push(diePosZ);
			oc.push(boxLeftTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				dropedItems.Add(oc.pop_int(), oc.pop_int());
			}
			diePosX = oc.pop_float();
			diePosY = oc.pop_float();
			diePosZ = oc.pop_float();
			boxLeftTime = oc.pop_int();
			return oc;
		}
	}
}
