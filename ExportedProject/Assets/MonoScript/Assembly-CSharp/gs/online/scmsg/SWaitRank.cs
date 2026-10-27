using Net;
using Share;

namespace gs.online.scmsg
{
	public class SWaitRank : Message
	{
		public delegate void Handler(SWaitRank msg);

		public const int TYPE = 2100166;

		public static Handler handler;

		public int rank;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100166;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(rank);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			rank = oc.pop_int();
			return oc;
		}
	}
}
