using Net;
using Share;

namespace gs.role.scmsg
{
	public class SRemoveRoleSkin : Message
	{
		public delegate void Handler(SRemoveRoleSkin msg);

		public const int TYPE = 4197322;

		public static Handler handler;

		public int itemId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197322;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			return oc;
		}
	}
}
