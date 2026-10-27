using System.Collections.Generic;
using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class SArmyGroupInfos : Message
	{
		public delegate void Handler(SArmyGroupInfos msg);

		public const int TYPE = 32508860;

		public static Handler handler;

		public List<ArmyGroupInfo> armyGroupInfos = new List<ArmyGroupInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508860;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(armyGroupInfos.Count);
			foreach (ArmyGroupInfo armyGroupInfo in armyGroupInfos)
			{
				oc.push(armyGroupInfo);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ArmyGroupInfo armyGroupInfo = new ArmyGroupInfo();
				oc.pop(armyGroupInfo);
				armyGroupInfos.Add(armyGroupInfo);
			}
			return oc;
		}
	}
}
