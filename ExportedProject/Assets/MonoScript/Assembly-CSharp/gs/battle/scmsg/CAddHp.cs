using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAddHp : Message
	{
		public delegate void Handler(CAddHp msg);

		public const int TYPE = 11537470;

		public static Handler handler;

		public short hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537470;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			hp = oc.pop_short();
			return oc;
		}
	}
}
