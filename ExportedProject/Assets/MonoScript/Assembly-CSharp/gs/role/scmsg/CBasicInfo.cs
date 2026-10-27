using Net;
using Share;

namespace gs.role.scmsg
{
	public class CBasicInfo : Message
	{
		public delegate void Handler(CBasicInfo msg);

		public const int TYPE = 4197306;

		public static Handler handler;

		public long otherId;

		public int version;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197306;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(version);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			version = oc.pop_int();
			return oc;
		}
	}
}
