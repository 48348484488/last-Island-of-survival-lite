using System.Collections.Generic;
using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.facilitybuilding.scmsg
{
	public class CAddRawMaterial : Message
	{
		public delegate void Handler(CAddRawMaterial msg);

		public const int TYPE = 31460282;

		public static Handler handler;

		public long facilityBuildingId;

		public Dictionary<int, UseItem> rawMaterials = new Dictionary<int, UseItem>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 31460282;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(facilityBuildingId);
			oc.push(rawMaterials.Count);
			foreach (KeyValuePair<int, UseItem> rawMaterial in rawMaterials)
			{
				oc.push(rawMaterial.Key);
				oc.push(rawMaterial.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			facilityBuildingId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				rawMaterials.Add(key, useItem);
			}
			return oc;
		}
	}
}
