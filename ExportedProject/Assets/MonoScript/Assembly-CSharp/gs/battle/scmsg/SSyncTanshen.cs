using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncTanshen : Message
	{
		public delegate void Handler(SSyncTanshen msg);

		public const int TYPE = 11537419;

		public static Handler handler;

		public long roleId;

		public int id;

		public bool isTanshen;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537419;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(id);
			oc.push(isTanshen);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			id = oc.pop_int();
			isTanshen = oc.pop_bool();
			return oc;
		}
	}
}
