using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CChangeCreateNum : Message
	{
		public delegate void Handler(CChangeCreateNum msg);

		public const int TYPE = 14683071;

		public static Handler handler;

		public int changeIndex;

		public int changeNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683071;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(changeIndex);
			oc.push(changeNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			changeIndex = oc.pop_int();
			changeNum = oc.pop_int();
			return oc;
		}
	}
}
