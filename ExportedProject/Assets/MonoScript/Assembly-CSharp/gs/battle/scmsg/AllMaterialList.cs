using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class AllMaterialList : Marshal
	{
		public List<MaterialInfo> materialInfos = new List<MaterialInfo>();

		public Octets marshal(Octets oc)
		{
			oc.push(materialInfos.Count);
			foreach (MaterialInfo materialInfo in materialInfos)
			{
				oc.push(materialInfo);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				MaterialInfo materialInfo = new MaterialInfo();
				oc.pop(materialInfo);
				materialInfos.Add(materialInfo);
			}
			return oc;
		}
	}
}
