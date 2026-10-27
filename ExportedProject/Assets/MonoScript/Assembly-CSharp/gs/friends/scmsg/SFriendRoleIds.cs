using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SFriendRoleIds : Message
	{
		public delegate void Handler(SFriendRoleIds msg);

		public const int TYPE = 13634510;

		public static Handler handler;

		public HashSet<long> cares = new HashSet<long>();

		public HashSet<long> fans = new HashSet<long>();

		public HashSet<long> blacks = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634510;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cares.Count);
			foreach (long care in cares)
			{
				oc.push(care);
			}
			oc.push(fans.Count);
			foreach (long fan in fans)
			{
				oc.push(fan);
			}
			oc.push(blacks.Count);
			foreach (long black in blacks)
			{
				oc.push(black);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				cares.Add(oc.pop_long());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				fans.Add(oc.pop_long());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				blacks.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
