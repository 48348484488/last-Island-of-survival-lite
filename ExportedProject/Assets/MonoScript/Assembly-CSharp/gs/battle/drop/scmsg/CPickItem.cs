using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class CPickItem : Message
	{
		public delegate void Handler(CPickItem msg);

		public const int TYPE = 12585914;

		public static Handler handler;

		public long objId;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585914;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(objId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			objId = oc.pop_long();
			number = oc.pop_int();
			return oc;
		}
	}
}
