using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncBigPlaneNumber : Message
	{
		public delegate void Handler(SSyncBigPlaneNumber msg);

		public const int TYPE = 11537427;

		public static Handler handler;

		public int curNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537427;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(curNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			curNum = oc.pop_int();
			return oc;
		}
	}
}
