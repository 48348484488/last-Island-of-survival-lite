using Net;
using Share;

namespace gs.friends.scmsg
{
	public class CChangeRename : Message
	{
		public delegate void Handler(CChangeRename msg);

		public const int TYPE = 13634502;

		public static Handler handler;

		public long roleId;

		public string rename = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634502;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(rename);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			rename = oc.pop_string();
			return oc;
		}
	}
}
