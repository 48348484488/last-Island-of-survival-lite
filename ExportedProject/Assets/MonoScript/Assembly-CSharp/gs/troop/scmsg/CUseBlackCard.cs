using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CUseBlackCard : Message
	{
		public delegate void Handler(CUseBlackCard msg);

		public const int TYPE = 15731658;

		public static Handler handler;

		public bool use;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731658;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(use);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			use = oc.pop_bool();
			return oc;
		}
	}
}
