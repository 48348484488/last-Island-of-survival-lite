using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SNewBeeStatusChange : Message
	{
		public delegate void Handler(SNewBeeStatusChange msg);

		public const int TYPE = 11537557;

		public static Handler handler;

		public long roleId;

		public bool flag;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537557;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(flag);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			flag = oc.pop_bool();
			return oc;
		}
	}
}
