using Net;
using Share;

namespace gs.role.scmsg
{
	public class CChangeName : Message
	{
		public delegate void Handler(CChangeName msg);

		public const int TYPE = 4197314;

		public static Handler handler;

		public string name = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197314;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(name);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			name = oc.pop_string();
			return oc;
		}
	}
}
