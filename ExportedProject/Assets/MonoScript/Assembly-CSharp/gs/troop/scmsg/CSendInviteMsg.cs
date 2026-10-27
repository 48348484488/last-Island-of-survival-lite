using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CSendInviteMsg : Message
	{
		public delegate void Handler(CSendInviteMsg msg);

		public const int TYPE = 15731649;

		public static Handler handler;

		public string text = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731649;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(text);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			text = oc.pop_string();
			return oc;
		}
	}
}
