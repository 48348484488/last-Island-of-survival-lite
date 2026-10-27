using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SEnterBuildState : Message
	{
		public delegate void Handler(SEnterBuildState msg);

		public const int TYPE = 11537506;

		public static Handler handler;

		public long roleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537506;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			return oc;
		}
	}
}
