using Net;
using Share;

namespace gs.role.scmsg
{
	public class SDiamondChange : Message
	{
		public delegate void Handler(SDiamondChange msg);

		public const int TYPE = 4197309;

		public static Handler handler;

		public int diamond;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197309;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(diamond);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			diamond = oc.pop_int();
			return oc;
		}
	}
}
