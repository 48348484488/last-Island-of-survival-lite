using Share;

namespace gs.role.scmsg
{
	public class AllianceInfo : Marshal
	{
		public string allianceName = string.Empty;

		public int allianceJob;

		public Octets marshal(Octets oc)
		{
			oc.push(allianceName);
			oc.push(allianceJob);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			allianceName = oc.pop_string();
			allianceJob = oc.pop_int();
			return oc;
		}
	}
}
