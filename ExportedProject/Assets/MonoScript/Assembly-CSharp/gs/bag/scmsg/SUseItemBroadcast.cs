using System.Collections.Generic;
using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SUseItemBroadcast : Message
	{
		public delegate void Handler(SUseItemBroadcast msg);

		public const int TYPE = 8391639;

		public static Handler handler;

		public long roleId;

		public string name = string.Empty;

		public HashSet<int> itemIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391639;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(name);
			oc.push(itemIds.Count);
			foreach (int itemId in itemIds)
			{
				oc.push(itemId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			name = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				itemIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
