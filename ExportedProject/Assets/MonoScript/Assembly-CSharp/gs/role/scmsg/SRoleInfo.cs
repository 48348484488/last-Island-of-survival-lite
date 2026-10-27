using System.Collections.Generic;
using Net;
using Share;

namespace gs.role.scmsg
{
	public class SRoleInfo : Message
	{
		public delegate void Handler(SRoleInfo msg);

		public const int TYPE = 4197304;

		public static Handler handler;

		public BasicRoleInfo basicInfo = new BasicRoleInfo();

		public int coupon;

		public int gold;

		public bool modelSex;

		public HashSet<int> skins = new HashSet<int>();

		public int createTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197304;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(basicInfo);
			oc.push(coupon);
			oc.push(gold);
			oc.push(modelSex);
			oc.push(skins.Count);
			foreach (int skin in skins)
			{
				oc.push(skin);
			}
			oc.push(createTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(basicInfo);
			coupon = oc.pop_int();
			gold = oc.pop_int();
			modelSex = oc.pop_bool();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				skins.Add(oc.pop_int());
			}
			createTime = oc.pop_int();
			return oc;
		}
	}
}
