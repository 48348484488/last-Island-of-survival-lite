using Share;

namespace gs.battle.scmsg
{
	public class MdituBuildingObjectInfo : Marshal
	{
		public GameObjectInfo info = new GameObjectInfo();

		public int buildingId;

		public Octets marshal(Octets oc)
		{
			oc.push(info);
			oc.push(buildingId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			buildingId = oc.pop_int();
			return oc;
		}
	}
}
