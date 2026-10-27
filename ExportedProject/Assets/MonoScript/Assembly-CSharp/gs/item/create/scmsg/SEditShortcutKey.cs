using System.Collections.Generic;
using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class SEditShortcutKey : Message
	{
		public delegate void Handler(SEditShortcutKey msg);

		public const int TYPE = 14683078;

		public static Handler handler;

		public List<int> shortcutKeys = new List<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683078;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shortcutKeys.Count);
			foreach (int shortcutKey in shortcutKeys)
			{
				oc.push(shortcutKey);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				shortcutKeys.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
