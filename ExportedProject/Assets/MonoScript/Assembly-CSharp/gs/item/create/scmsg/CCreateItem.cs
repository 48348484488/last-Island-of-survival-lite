using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CCreateItem : Message
	{
		public delegate void Handler(CCreateItem msg);

		public const int TYPE = 14683067;

		public static Handler handler;

		public CreateInfo item = new CreateInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683067;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(item);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(item);
			return oc;
		}
	}
}
