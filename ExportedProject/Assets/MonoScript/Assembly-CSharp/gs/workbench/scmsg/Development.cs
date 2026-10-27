using Share;

namespace gs.workbench.scmsg
{
	public class Development : Marshal
	{
		public int developmentId;

		public bool isUsedAssist;

		public int finishTime;

		public bool itemIsGet;

		public Octets marshal(Octets oc)
		{
			oc.push(developmentId);
			oc.push(isUsedAssist);
			oc.push(finishTime);
			oc.push(itemIsGet);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			developmentId = oc.pop_int();
			isUsedAssist = oc.pop_bool();
			finishTime = oc.pop_int();
			itemIsGet = oc.pop_bool();
			return oc;
		}
	}
}
