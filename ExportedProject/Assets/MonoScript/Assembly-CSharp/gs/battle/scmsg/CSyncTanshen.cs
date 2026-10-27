using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncTanshen : Message
	{
		public delegate void Handler(CSyncTanshen msg);

		public const int TYPE = 11537418;

		public static Handler handler;

		public bool isTanshen;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537418;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(isTanshen);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			isTanshen = oc.pop_bool();
			return oc;
		}
	}
}
