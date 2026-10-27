using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBattleLoginFinish : Message
	{
		public delegate void Handler(SBattleLoginFinish msg);

		public const int TYPE = 11537337;

		public static Handler handler;

		public PlayerInfo playerInfo = new PlayerInfo();

		public int timeToRebirth;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537337;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(playerInfo);
			oc.push(timeToRebirth);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(playerInfo);
			timeToRebirth = oc.pop_int();
			return oc;
		}
	}
}
