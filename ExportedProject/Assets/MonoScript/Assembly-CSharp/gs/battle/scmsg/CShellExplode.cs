using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CShellExplode : Message
	{
		public delegate void Handler(CShellExplode msg);

		public const int TYPE = 23071675;

		public static Handler handler;

		public int itemId;

		public Vec3 pos = new Vec3();

		public List<long> damagedList = new List<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071675;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			oc.push(pos);
			oc.push(damagedList.Count);
			foreach (long damaged in damagedList)
			{
				oc.push(damaged);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			oc.pop(pos);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				damagedList.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
