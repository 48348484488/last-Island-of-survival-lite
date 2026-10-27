using Net;
using Share;

namespace gs.role.scmsg
{
	public class SIsGmChange : Message
	{
		public delegate void Handler(SIsGmChange msg);

		public const int TYPE = 4197305;

		public static Handler handler;

		public bool isGM;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197305;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(isGM);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			isGM = oc.pop_bool();
			return oc;
		}
	}
}
