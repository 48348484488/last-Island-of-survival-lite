using Net;
using Share;

namespace gs.task.scmsg
{
	public class SRemoveTask : Message
	{
		public delegate void Handler(SRemoveTask msg);

		public const int TYPE = 10488762;

		public static Handler handler;

		public long taskId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488762;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskId = oc.pop_long();
			return oc;
		}
	}
}
