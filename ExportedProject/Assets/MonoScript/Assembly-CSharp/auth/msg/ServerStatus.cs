using Share;

namespace auth.msg
{
	public class ServerStatus : Marshal
	{
		public const int FLUENT = 0;

		public const int BUSY = 1;

		public const int OVERLOAD = 2;

		public const int MAINTAIN = 3;

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
