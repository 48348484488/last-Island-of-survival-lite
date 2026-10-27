using System.Collections.Generic;
using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.cook.scmsg
{
	public class SCampFireOrFirePlaceInfo : Message
	{
		public delegate void Handler(SCampFireOrFirePlaceInfo msg);

		public const int TYPE = 20974521;

		public static Handler handler;

		public long instanceId;

		public Dictionary<int, UseItem> rawMaterials = new Dictionary<int, UseItem>();

		public UseItem fuels = new UseItem();

		public UseItem finisheds = new UseItem();

		public bool isStart;

		public int cfgId;

		public int cookId;

		public int cookCount;

		public int fire;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 20974521;
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
			oc.push(fuels);
			oc.push(finisheds);
			oc.push(isStart);
			oc.push(cfgId);
			oc.push(cookId);
			oc.push(cookCount);
			oc.push(fire);
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
			oc.pop(fuels);
			oc.pop(finisheds);
			isStart = oc.pop_bool();
			cfgId = oc.pop_int();
			cookId = oc.pop_int();
			cookCount = oc.pop_int();
			fire = oc.pop_int();
			return oc;
		}
	}
}
