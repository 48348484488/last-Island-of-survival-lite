using System.Collections.Generic;
using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.battle.map.turret.scmsg
{
	public class CChangeBullets : Message
	{
		public delegate void Handler(CChangeBullets msg);

		public const int TYPE = 22023105;

		public static Handler handler;

		public long turretId;

		public Dictionary<int, UseItem> bullets = new Dictionary<int, UseItem>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023105;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(bullets.Count);
			foreach (KeyValuePair<int, UseItem> bullet in bullets)
			{
				oc.push(bullet.Key);
				oc.push(bullet.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				bullets.Add(key, useItem);
			}
			return oc;
		}
	}
}
