using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class SLadderInfo : Message
	{
		public delegate void Handler(SLadderInfo msg);

		public const int TYPE = 19925944;

		public static Handler handler;

		public LadderInfo ladderInfo = new LadderInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925944;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(ladderInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(ladderInfo);
			return oc;
		}
	}
}
