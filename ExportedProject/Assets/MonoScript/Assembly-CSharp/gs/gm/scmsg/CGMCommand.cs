using Net;
using Share;

namespace gs.gm.scmsg
{
	public class CGMCommand : Message
	{
		public delegate void Handler(CGMCommand msg);

		public const int TYPE = 3148728;

		public static Handler handler;

		public string cmd = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 3148728;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cmd);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cmd = oc.pop_string();
			return oc;
		}
	}
}
