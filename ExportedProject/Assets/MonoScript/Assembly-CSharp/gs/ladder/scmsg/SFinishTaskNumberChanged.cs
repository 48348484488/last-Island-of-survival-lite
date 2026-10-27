using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class SFinishTaskNumberChanged : Message
	{
		public delegate void Handler(SFinishTaskNumberChanged msg);

		public const int TYPE = 19925954;

		public static Handler handler;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925954;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			number = oc.pop_int();
			return oc;
		}
	}
}
