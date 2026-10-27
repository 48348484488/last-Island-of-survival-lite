using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class AllChushengdianList : Marshal
	{
		public List<ChushengdianPos> pos = new List<ChushengdianPos>();

		public Octets marshal(Octets oc)
		{
			oc.push(pos.Count);
			foreach (ChushengdianPos po in pos)
			{
				oc.push(po);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ChushengdianPos chushengdianPos = new ChushengdianPos();
				oc.pop(chushengdianPos);
				pos.Add(chushengdianPos);
			}
			return oc;
		}
	}
}
