using Net;
using Share;

namespace gs.friends.scmsg
{
	public class CSearchByRoleId : Message
	{
		public delegate void Handler(CSearchByRoleId msg);

		public const int TYPE = 13634504;

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
			return 13634504;
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
