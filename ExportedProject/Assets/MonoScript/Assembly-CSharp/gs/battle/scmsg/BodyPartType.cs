using Share;

namespace gs.battle.scmsg
{
	public class BodyPartType : Marshal
	{
		public const int LeftArmHigh = 0;

		public const int RightArmHigh = 1;

		public const int LeftArmLow = 2;

		public const int RightArmLow = 3;

		public const int LeftLegHigh = 4;

		public const int RightLegHigh = 5;

		public const int LeftLegLow = 6;

		public const int RightLegLow = 7;

		public const int Body = 8;

		public const int Head = 9;

		public const int HeadTop = 10;

		public const int Root = 11;

		public const int LeftFoot = 12;

		public const int RightFoot = 13;

		public const int RightHand = 14;

		public const int LeftHand = 15;

		public const int Neck = 16;

		public const int HeadBub = 17;

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
