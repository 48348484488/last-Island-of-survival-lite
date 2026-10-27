using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncOrientation : Message
	{
		public delegate void Handler(CSyncOrientation msg);

		public const int TYPE = 11537345;

		public static Handler handler;

		public ShortVec3 orientation = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537345;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(orientation);
			return oc;
		}
	}
}
