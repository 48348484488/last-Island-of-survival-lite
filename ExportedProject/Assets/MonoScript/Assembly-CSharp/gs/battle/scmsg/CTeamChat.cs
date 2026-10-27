using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CTeamChat : Message
	{
		public delegate void Handler(CTeamChat msg);

		public const int TYPE = 11537449;

		public static Handler handler;

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
			return 11537449;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(text);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			text = oc.pop_string();
			return oc;
		}
	}
}
