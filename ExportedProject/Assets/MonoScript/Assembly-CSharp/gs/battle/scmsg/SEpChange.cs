using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SEpChange : Message
	{
		public delegate void Handler(SEpChange msg);

		public const int TYPE = 11537361;

		public static Handler handler;

		public long roleId;

		public short ep;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537361;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(ep);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			ep = oc.pop_short();
			return oc;
		}
	}
}
