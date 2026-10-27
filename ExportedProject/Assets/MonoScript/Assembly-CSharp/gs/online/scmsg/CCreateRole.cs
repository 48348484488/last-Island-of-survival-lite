using System.Collections.Generic;
using Net;
using Share;

namespace gs.online.scmsg
{
	public class CCreateRole : Message
	{
		public delegate void Handler(CCreateRole msg);

		public const int TYPE = 2100154;

		public static Handler handler;

		public string name = string.Empty;

		public string channelId = string.Empty;

		public string appChannelId = string.Empty;

		public bool modelSex;

		public HashSet<int> roleSkins = new HashSet<int>();

		public string phoneID = string.Empty;

		public string systemType = string.Empty;

		public string version = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100154;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(name);
			oc.push(channelId);
			oc.push(appChannelId);
			oc.push(modelSex);
			oc.push(roleSkins.Count);
			foreach (int roleSkin in roleSkins)
			{
				oc.push(roleSkin);
			}
			oc.push(phoneID);
			oc.push(systemType);
			oc.push(version);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			name = oc.pop_string();
			channelId = oc.pop_string();
			appChannelId = oc.pop_string();
			modelSex = oc.pop_bool();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleSkins.Add(oc.pop_int());
			}
			phoneID = oc.pop_string();
			systemType = oc.pop_string();
			version = oc.pop_string();
			return oc;
		}
	}
}
