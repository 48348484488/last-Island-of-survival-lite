using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SWatch : Message
	{
		public delegate void Handler(SWatch msg);

		public const int TYPE = 11537466;

		public static Handler handler;

		public const int SUCCESS = 0;

		public const int ERROR = 1;

		public const int BATTLE_IS_END = 2;

		public const int WRONG_SECRET_KEY = 3;

		public const int NOT_IN_WATCHLIST = 4;

		public const int PLAYER_NOT_READY = 5;

		public int code;

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
			return 11537466;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(code);
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			roleId = oc.pop_long();
			return oc;
		}
	}
}
