using Net;
using Share;

namespace gs.role.scmsg
{
	public class SSyncOnline : Message
	{
		public delegate void Handler(SSyncOnline msg);

		public const int TYPE = 4197311;

		public static Handler handler;

		public long roleId;

		public byte status;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197311;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(status);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			status = oc.pop_byte();
			return oc;
		}
	}
}
