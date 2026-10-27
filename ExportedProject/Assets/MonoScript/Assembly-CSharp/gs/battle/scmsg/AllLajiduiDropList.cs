using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class AllLajiduiDropList : Marshal
	{
		public List<LajiduiDropInfo> lajiduis = new List<LajiduiDropInfo>();

		public Octets marshal(Octets oc)
		{
			oc.push(lajiduis.Count);
			foreach (LajiduiDropInfo lajidui in lajiduis)
			{
				oc.push(lajidui);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				LajiduiDropInfo lajiduiDropInfo = new LajiduiDropInfo();
				oc.pop(lajiduiDropInfo);
				lajiduis.Add(lajiduiDropInfo);
			}
			return oc;
		}
	}
}
