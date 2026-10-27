using System.Collections.Generic;
using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class SWorkbenchInfo : Message
	{
		public delegate void Handler(SWorkbenchInfo msg);

		public const int TYPE = 16780217;

		public static Handler handler;

		public long workbenchId;

		public int workbenchLevel;

		public int finishTime;

		public Dictionary<int, Development> developments = new Dictionary<int, Development>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780217;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			oc.push(workbenchLevel);
			oc.push(finishTime);
			oc.push(developments.Count);
			foreach (KeyValuePair<int, Development> development in developments)
			{
				oc.push(development.Key);
				oc.push(development.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			workbenchLevel = oc.pop_int();
			finishTime = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				Development development = new Development();
				oc.pop(development);
				developments.Add(key, development);
			}
			return oc;
		}
	}
}
