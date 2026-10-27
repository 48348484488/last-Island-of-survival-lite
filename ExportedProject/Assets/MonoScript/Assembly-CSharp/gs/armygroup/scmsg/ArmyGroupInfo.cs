using Share;

namespace gs.armygroup.scmsg
{
	public class ArmyGroupInfo : Marshal
	{
		public int armyGroupIcon;

		public string armyGroupName = string.Empty;

		public int armyGroupLevel;

		public long armyGroupId;

		public string notice = string.Empty;

		public int memberCount;

		public string leaderName = string.Empty;

		public long prosperous;

		public Octets marshal(Octets oc)
		{
			oc.push(armyGroupIcon);
			oc.push(armyGroupName);
			oc.push(armyGroupLevel);
			oc.push(armyGroupId);
			oc.push(notice);
			oc.push(memberCount);
			oc.push(leaderName);
			oc.push(prosperous);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			armyGroupIcon = oc.pop_int();
			armyGroupName = oc.pop_string();
			armyGroupLevel = oc.pop_int();
			armyGroupId = oc.pop_long();
			notice = oc.pop_string();
			memberCount = oc.pop_int();
			leaderName = oc.pop_string();
			prosperous = oc.pop_long();
			return oc;
		}
	}
}
