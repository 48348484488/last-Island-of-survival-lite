using Share;

namespace gs.battle.scmsg
{
	public class BuildingStatus : Marshal
	{
		public const int DEFAULT = 0;

		public const int DOOR_OPEN_1 = 1;

		public const int DOOR_OPEN_2 = 2;

		public const int UPGRADING = 3;

		public const int HAS_OUTPUT = 4;

		public const int DEVELOPING = 5;

		public const int HAS_PASSWORD = 6;

		public const int Opening = 6;

		public Octets marshal(Octets oc)
		{
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
