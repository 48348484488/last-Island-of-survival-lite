using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STreeGrow : Message
	{
		public delegate void Handler(STreeGrow msg);

		public const int TYPE = 11537489;

		public static Handler handler;

		public long treeInstanceId;

		public int treeId;

		public byte cfgId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537489;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(treeInstanceId);
			oc.push(treeId);
			oc.push(cfgId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			treeInstanceId = oc.pop_long();
			treeId = oc.pop_int();
			cfgId = oc.pop_byte();
			return oc;
		}
	}
}
