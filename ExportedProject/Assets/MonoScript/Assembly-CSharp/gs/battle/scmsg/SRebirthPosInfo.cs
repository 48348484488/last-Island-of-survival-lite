using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SRebirthPosInfo : Message
	{
		public delegate void Handler(SRebirthPosInfo msg);

		public const int TYPE = 11537512;

		public static Handler handler;

		public Dictionary<long, RebirthPos> info = new Dictionary<long, RebirthPos>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537512;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(info.Count);
			foreach (KeyValuePair<long, RebirthPos> item in info)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				long key = oc.pop_long();
				RebirthPos rebirthPos = new RebirthPos();
				oc.pop(rebirthPos);
				info.Add(key, rebirthPos);
			}
			return oc;
		}
	}
}
