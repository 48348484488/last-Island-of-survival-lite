using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CWatch : Message
	{
		public delegate void Handler(CWatch msg);

		public const int TYPE = 11537465;

		public static Handler handler;

		public long roleId;

		public long watchRoleId;

		public long roomId;

		public long secretKey;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537465;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(watchRoleId);
			oc.push(roomId);
			oc.push(secretKey);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			watchRoleId = oc.pop_long();
			roomId = oc.pop_long();
			secretKey = oc.pop_long();
			return oc;
		}
	}
}
