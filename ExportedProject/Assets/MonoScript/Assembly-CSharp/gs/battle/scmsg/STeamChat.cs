using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STeamChat : Message
	{
		public delegate void Handler(STeamChat msg);

		public const int TYPE = 11537450;

		public static Handler handler;

		public long roleId;

		public string text = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537450;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(text);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			text = oc.pop_string();
			return oc;
		}
	}
}
