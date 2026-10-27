using Net;
using Share;

namespace gs.role.scmsg
{
	public class SGoldChange : Message
	{
		public delegate void Handler(SGoldChange msg);

		public const int TYPE = 4197309;

		public static Handler handler;

		public int gold;

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
			oc.push(gold);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gold = oc.pop_int();
			return oc;
		}
	}
}
