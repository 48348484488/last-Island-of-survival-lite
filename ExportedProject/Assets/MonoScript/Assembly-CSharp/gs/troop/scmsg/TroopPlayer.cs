using Share;
using gs.role.scmsg;

namespace gs.troop.scmsg
{
	public class TroopPlayer : Marshal
	{
		public RoleVersion role = new RoleVersion();

		public bool isOnline;

		public bool isInBattle;

		public bool isPower;

		public bool isReady;

		public bool modelSex;

		public int level;

		public int headId;

		public int duanWei;

		public int blood;

		public Octets marshal(Octets oc)
		{
			oc.push(role);
			oc.push(isOnline);
			oc.push(isInBattle);
			oc.push(isPower);
			oc.push(isReady);
			oc.push(modelSex);
			oc.push(level);
			oc.push(headId);
			oc.push(duanWei);
			oc.push(blood);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(role);
			isOnline = oc.pop_bool();
			isInBattle = oc.pop_bool();
			isPower = oc.pop_bool();
			isReady = oc.pop_bool();
			modelSex = oc.pop_bool();
			level = oc.pop_int();
			headId = oc.pop_int();
			duanWei = oc.pop_int();
			blood = oc.pop_int();
			return oc;
		}
	}
}
