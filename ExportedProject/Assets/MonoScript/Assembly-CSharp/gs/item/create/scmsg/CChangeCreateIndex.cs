using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CChangeCreateIndex : Message
	{
		public delegate void Handler(CChangeCreateIndex msg);

		public const int TYPE = 14683075;

		public static Handler handler;

		public int changeIndex;

		public int targetIndex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683075;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(changeIndex);
			oc.push(targetIndex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			changeIndex = oc.pop_int();
			targetIndex = oc.pop_int();
			return oc;
		}
	}
}
