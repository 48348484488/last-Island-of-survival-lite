using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class MaterialInfo : Marshal
	{
		public string prefabName = string.Empty;

		public List<MaterialPosInfo> materialPosInfos = new List<MaterialPosInfo>();

		public Octets marshal(Octets oc)
		{
			oc.push(prefabName);
			oc.push(materialPosInfos.Count);
			foreach (MaterialPosInfo materialPosInfo in materialPosInfos)
			{
				oc.push(materialPosInfo);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			prefabName = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				MaterialPosInfo materialPosInfo = new MaterialPosInfo();
				oc.pop(materialPosInfo);
				materialPosInfos.Add(materialPosInfo);
			}
			return oc;
		}
	}
}
