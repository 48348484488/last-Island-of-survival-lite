using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STreeDown : Message
	{
		public delegate void Handler(STreeDown msg);

		public const int TYPE = 11537488;

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
			return 11537488;
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
