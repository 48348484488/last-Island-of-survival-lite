using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class CellMinMaxHeightList : Marshal
	{
		public List<CellMinMaxHeightForServer> allcells = new List<CellMinMaxHeightForServer>();

		public Octets marshal(Octets oc)
		{
			oc.push(allcells.Count);
			foreach (CellMinMaxHeightForServer allcell in allcells)
			{
				oc.push(allcell);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				CellMinMaxHeightForServer cellMinMaxHeightForServer = new CellMinMaxHeightForServer();
				oc.pop(cellMinMaxHeightForServer);
				allcells.Add(cellMinMaxHeightForServer);
			}
			return oc;
		}
	}
}
