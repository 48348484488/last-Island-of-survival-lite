using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SShowTree : Message
	{
		public delegate void Handler(SShowTree msg);

		public const int TYPE = 11537486;

		public static Handler handler;

		public long treeInstanceId;

		public int treeId;

		public byte cfgId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537486;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(treeInstanceId);
			oc.push(treeId);
			oc.push(cfgId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			treeInstanceId = oc.pop_long();
			treeId = oc.pop_int();
			cfgId = oc.pop_byte();
			hp = oc.pop_int();
			return oc;
		}
	}
}
