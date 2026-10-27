using System.Collections.Generic;
using Net;
using Share;

namespace gs.smelter.scmsg
{
	public class SSmelterInfo : Message
	{
		public delegate void Handler(SSmelterInfo msg);

		public const int TYPE = 17828793;

		public static Handler handler;

		public long smelterId;

		public Dictionary<int, UseItem> rawMaterials = new Dictionary<int, UseItem>();

		public Dictionary<int, UseItem> fuels = new Dictionary<int, UseItem>();

		public List<UseItem> finisheds = new List<UseItem>();

		public bool isStart;

		public int cfgId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 17828793;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(smelterId);
			oc.push(rawMaterials.Count);
			foreach (KeyValuePair<int, UseItem> rawMaterial in rawMaterials)
			{
				oc.push(rawMaterial.Key);
				oc.push(rawMaterial.Value);
			}
			oc.push(fuels.Count);
			foreach (KeyValuePair<int, UseItem> fuel in fuels)
			{
				oc.push(fuel.Key);
				oc.push(fuel.Value);
			}
			oc.push(finisheds.Count);
			foreach (UseItem finished in finisheds)
			{
				oc.push(finished);
			}
			oc.push(isStart);
			oc.push(cfgId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			smelterId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				rawMaterials.Add(key, useItem);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				int key2 = oc.pop_int();
				UseItem useItem2 = new UseItem();
				oc.pop(useItem2);
				fuels.Add(key2, useItem2);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				UseItem useItem3 = new UseItem();
				oc.pop(useItem3);
				finisheds.Add(useItem3);
			}
			isStart = oc.pop_bool();
			cfgId = oc.pop_int();
			return oc;
		}
	}
}
