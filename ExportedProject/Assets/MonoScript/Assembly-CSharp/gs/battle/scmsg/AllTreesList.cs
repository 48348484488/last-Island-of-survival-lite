using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class AllTreesList : Marshal
	{
		public List<TreePosInfo> trees = new List<TreePosInfo>();

		public Octets marshal(Octets oc)
		{
			oc.push(trees.Count);
			foreach (TreePosInfo tree in trees)
			{
				oc.push(tree);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TreePosInfo treePosInfo = new TreePosInfo();
				oc.pop(treePosInfo);
				trees.Add(treePosInfo);
			}
			return oc;
		}
	}
}
