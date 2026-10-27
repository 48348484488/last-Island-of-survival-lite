using Share;

namespace auth.msg
{
	public class PlatformIds : Marshal
	{
		public const int TEST = 0;

		public const int HEROUSDK_ANDROID = 1;

		public const int HEROUSDK_IOS = 2;

		public const int TIMESDK_ANDROID = 3;

		public const int TIMESDK_IOS = 4;

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
