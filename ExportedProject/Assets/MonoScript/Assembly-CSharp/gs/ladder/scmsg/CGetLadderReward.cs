using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class CGetLadderReward : Message
	{
		public delegate void Handler(CGetLadderReward msg);

		public const int TYPE = 19925950;

		public static Handler handler;

		public int taskType;

		public int level;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925950;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskType);
			oc.push(level);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskType = oc.pop_int();
			level = oc.pop_int();
			return oc;
		}
	}
}
