using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SHideTree : Message
	{
		public delegate void Handler(SHideTree msg);

		public const int TYPE = 11537487;

		public static Handler handler;

		public int treeId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537487;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(treeId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			treeId = oc.pop_int();
			return oc;
		}
	}
}
