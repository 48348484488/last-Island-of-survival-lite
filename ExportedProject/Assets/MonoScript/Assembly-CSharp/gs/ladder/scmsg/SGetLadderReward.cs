using Net;
using Share;
using gs.drop.scmsg;

namespace gs.ladder.scmsg
{
	public class SGetLadderReward : Message
	{
		public delegate void Handler(SGetLadderReward msg);

		public const int TYPE = 19925951;

		public static Handler handler;

		public int taskType;

		public int level;

		public DropDetail dropDetail = new DropDetail();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925951;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskType);
			oc.push(level);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskType = oc.pop_int();
			level = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
