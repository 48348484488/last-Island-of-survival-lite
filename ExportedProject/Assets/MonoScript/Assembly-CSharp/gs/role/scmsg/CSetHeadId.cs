using Net;
using Share;

namespace gs.role.scmsg
{
	public class CSetHeadId : Message
	{
		public delegate void Handler(CSetHeadId msg);

		public const int TYPE = 4197320;

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
			return 4197320;
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
