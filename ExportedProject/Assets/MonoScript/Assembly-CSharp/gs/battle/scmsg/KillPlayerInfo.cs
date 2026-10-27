using Share;

namespace gs.battle.scmsg
{
	public class KillPlayerInfo : Marshal
	{
		public long roleId;

		public string name = string.Empty;

		public int zone;

		public int country;

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(name);
			oc.push(zone);
			oc.push(country);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			name = oc.pop_string();
			zone = oc.pop_int();
			country = oc.pop_int();
			return oc;
		}
	}
}
