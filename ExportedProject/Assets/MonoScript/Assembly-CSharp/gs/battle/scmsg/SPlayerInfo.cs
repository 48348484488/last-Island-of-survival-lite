using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SPlayerInfo : Message
	{
		public delegate void Handler(SPlayerInfo msg);

		public const int TYPE = 11537339;

		public static Handler handler;

		public PlayerInfo playerInfo = new PlayerInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537339;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(playerInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(playerInfo);
			return oc;
		}
	}
}
