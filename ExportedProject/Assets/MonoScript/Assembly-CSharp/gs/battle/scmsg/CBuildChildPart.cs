using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CBuildChildPart : Message
	{
		public delegate void Handler(CBuildChildPart msg);

		public const int TYPE = 11537473;

		public static Handler handler;

		public int typeId;

		public long parentInsId;

		public Dictionary<long, byte> inmap = new Dictionary<long, byte>();

		public Dictionary<long, byte> outmap = new Dictionary<long, byte>();

		public short rotateY;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537473;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(typeId);
			oc.push(parentInsId);
			oc.push(inmap.Count);
			foreach (KeyValuePair<long, byte> item in inmap)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(outmap.Count);
			foreach (KeyValuePair<long, byte> item2 in outmap)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			oc.push(rotateY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			typeId = oc.pop_int();
			parentInsId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				inmap.Add(oc.pop_long(), oc.pop_byte());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				outmap.Add(oc.pop_long(), oc.pop_byte());
			}
			rotateY = oc.pop_short();
			return oc;
		}
	}
}
