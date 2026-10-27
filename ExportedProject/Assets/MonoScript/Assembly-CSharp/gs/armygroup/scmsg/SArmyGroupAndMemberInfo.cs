using System.Collections.Generic;
using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class SArmyGroupAndMemberInfo : Message
	{
		public delegate void Handler(SArmyGroupAndMemberInfo msg);

		public const int TYPE = 32508857;

		public static Handler handler;

		public ArmyGroupInfo armyGroupInfo = new ArmyGroupInfo();

		public HashSet<MemberInfo> memberInfos = new HashSet<MemberInfo>();

		public HashSet<ApplyJoinMemberInfo> applyJoins = new HashSet<ApplyJoinMemberInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508857;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(armyGroupInfo);
			oc.push(memberInfos.Count);
			foreach (MemberInfo memberInfo in memberInfos)
			{
				oc.push(memberInfo);
			}
			oc.push(applyJoins.Count);
			foreach (ApplyJoinMemberInfo applyJoin in applyJoins)
			{
				oc.push(applyJoin);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(armyGroupInfo);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				MemberInfo memberInfo = new MemberInfo();
				oc.pop(memberInfo);
				memberInfos.Add(memberInfo);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				ApplyJoinMemberInfo applyJoinMemberInfo = new ApplyJoinMemberInfo();
				oc.pop(applyJoinMemberInfo);
				applyJoins.Add(applyJoinMemberInfo);
			}
			return oc;
		}
	}
}
