using System.Collections.Generic;
using Share;

namespace gs.drop.scmsg
{
	public class DropDetail : Marshal
	{
		public Dictionary<int, int> dropProp = new Dictionary<int, int>();

		public Dictionary<int, int> dropItem = new Dictionary<int, int>();

		public Dictionary<int, int> bindDropItem = new Dictionary<int, int>();

		public Octets marshal(Octets oc)
		{
			oc.push(dropProp.Count);
			foreach (KeyValuePair<int, int> item in dropProp)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(dropItem.Count);
			foreach (KeyValuePair<int, int> item2 in dropItem)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			oc.push(bindDropItem.Count);
			foreach (KeyValuePair<int, int> item3 in bindDropItem)
			{
				oc.push(item3.Key);
				oc.push(item3.Value);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				dropProp.Add(oc.pop_int(), oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				dropItem.Add(oc.pop_int(), oc.pop_int());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				bindDropItem.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}
