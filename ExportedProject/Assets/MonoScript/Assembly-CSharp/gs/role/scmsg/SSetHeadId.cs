using Net;
using Share;

namespace gs.role.scmsg
{
	public class SSetHeadId : Message
	{
		public delegate void Handler(SSetHeadId msg);

		public const int TYPE = 4197321;

		public static Handler handler;

		public int headId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197321;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(headId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			headId = oc.pop_int();
			return oc;
		}
	}
}
