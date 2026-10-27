using Net;
using Share;

namespace gs.role.scmsg
{
	public class SLevelExpChange : Message
	{
		public delegate void Handler(SLevelExpChange msg);

		public const int TYPE = 4197312;

		public static Handler handler;

		public int level;

		public int exp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197312;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(level);
			oc.push(exp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			level = oc.pop_int();
			exp = oc.pop_int();
			return oc;
		}
	}
}
