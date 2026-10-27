using Net;
using Share;

namespace gs.task.scmsg
{
	public class STaskLevelUp : Message
	{
		public delegate void Handler(STaskLevelUp msg);

		public const int TYPE = 10488765;

		public static Handler handler;

		public long oldId;

		public int progress;

		public int progressMax;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488765;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(oldId);
			oc.push(progress);
			oc.push(progressMax);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oldId = oc.pop_long();
			progress = oc.pop_int();
			progressMax = oc.pop_int();
			return oc;
		}
	}
}
