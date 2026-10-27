using Net;
using Share;

namespace gs.task.scmsg
{
	public class CIsReadTask : Message
	{
		public delegate void Handler(CIsReadTask msg);

		public const int TYPE = 10488767;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488767;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
