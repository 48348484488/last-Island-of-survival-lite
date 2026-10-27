using System.Collections.Generic;
using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.cook.scmsg
{
	public class CDoStart : Message
	{
		public delegate void Handler(CDoStart msg);

		public const int TYPE = 20974523;

		public static Handler handler;

		public long instanceId;

		public Dictionary<int, UseItem> rawMaterials = new Dictionary<int, UseItem>();

		public int cookNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 20974523;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(rawMaterials.Count);
			foreach (KeyValuePair<int, UseItem> rawMaterial in rawMaterials)
			{
				oc.push(rawMaterial.Key);
				oc.push(rawMaterial.Value);
			}
			oc.push(cookNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				rawMaterials.Add(key, useItem);
			}
			cookNum = oc.pop_int();
			return oc;
		}
	}
}
