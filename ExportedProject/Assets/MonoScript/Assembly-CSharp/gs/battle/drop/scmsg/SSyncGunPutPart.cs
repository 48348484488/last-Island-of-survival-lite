using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SSyncGunPutPart : Message
	{
		public delegate void Handler(SSyncGunPutPart msg);

		public const int TYPE = 12585917;

		public static Handler handler;

		public long roleId;

		public int index;

		public int partId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585917;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(index);
			oc.push(partId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			index = oc.pop_int();
			partId = oc.pop_int();
			return oc;
		}
	}
}
