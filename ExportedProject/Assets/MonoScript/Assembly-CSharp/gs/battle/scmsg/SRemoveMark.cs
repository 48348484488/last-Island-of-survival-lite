using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SRemoveMark : Message
	{
		public delegate void Handler(SRemoveMark msg);

		public const int TYPE = 11537365;

		public static Handler handler;

		public long roleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537365;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			return oc;
		}
	}
}
