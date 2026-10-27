using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SHpStatusChange : Message
	{
		public delegate void Handler(SHpStatusChange msg);

		public const int TYPE = 11537358;

		public static Handler handler;

		public long roleId;

		public bool isSecond;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537358;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(isSecond);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			isSecond = oc.pop_bool();
			return oc;
		}
	}
}
