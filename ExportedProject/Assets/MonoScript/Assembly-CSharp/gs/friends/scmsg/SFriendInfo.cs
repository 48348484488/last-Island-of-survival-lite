using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SFriendInfo : Message
	{
		public delegate void Handler(SFriendInfo msg);

		public const int TYPE = 13634488;

		public static Handler handler;

		public List<FansRoleInfo> fans = new List<FansRoleInfo>();

		public List<BlackRoleInfo> blacks = new List<BlackRoleInfo>();

		public List<CareRoleInfo> cares = new List<CareRoleInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634488;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(fans.Count);
			foreach (FansRoleInfo fan in fans)
			{
				oc.push(fan);
			}
			oc.push(blacks.Count);
			foreach (BlackRoleInfo black in blacks)
			{
				oc.push(black);
			}
			oc.push(cares.Count);
			foreach (CareRoleInfo care in cares)
			{
				oc.push(care);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				FansRoleInfo fansRoleInfo = new FansRoleInfo();
				oc.pop(fansRoleInfo);
				fans.Add(fansRoleInfo);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				BlackRoleInfo blackRoleInfo = new BlackRoleInfo();
				oc.pop(blackRoleInfo);
				blacks.Add(blackRoleInfo);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				CareRoleInfo careRoleInfo = new CareRoleInfo();
				oc.pop(careRoleInfo);
				cares.Add(careRoleInfo);
			}
			return oc;
		}
	}
}
