using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncOrientation : Message
	{
		public delegate void Handler(SSyncOrientation msg);

		public const int TYPE = 11537346;

		public static Handler handler;

		public long roleId;

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
			return 11537346;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			oc.pop(orientation);
			return oc;
		}
	}
}
