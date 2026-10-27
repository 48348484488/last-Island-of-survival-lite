using Net;
using Share;

namespace gs.troop.scmsg
{
	public class SErrorTroop : Message
	{
		public delegate void Handler(SErrorTroop msg);

		public const int TYPE = 15731655;

		public static Handler handler;

		public const int SUCCESS = 0;

		public const int TroopFull = 1;

		public const int TroopDissolve = 2;

		public const int YouNotLeader = 3;

		public const int InviteInCDing = 4;

		public const int HeNotInTroop = 5;

		public const int YetInTroop = 6;

		public const int TroopRefuseApply = 7;

		public const int HeNotOnline = 8;

		public const int YouNotTroop = 9;

		public const int HeInTroop = 10;

		public const int VoidInvite = 11;

		public const int TroopInMatch = 12;

		public const int OtherInMatch = 13;

		public const int BeBlocked = 14;

		public const int Error404 = 404;

		public int code;

		public long otherId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731655;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(code);
			oc.push(otherId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			otherId = oc.pop_long();
			return oc;
		}
	}
}
