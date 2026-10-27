using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SCanBuild : Message
	{
		public delegate void Handler(SCanBuild msg);

		public const int TYPE = 11537556;

		public static Handler handler;

		public bool flag;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537556;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(flag);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			flag = oc.pop_bool();
			return oc;
		}
	}
}
