using Net;
using Share;

namespace gs.troop.scmsg
{
	public class SOperateCMD : Message
	{
		public delegate void Handler(SOperateCMD msg);

		public const int TYPE = 15731657;

		public static Handler handler;

		public const int Online = 1;

		public const int Offline = 2;

		public const int Level = 3;

		public const int BeKick = 4;

		public const int UpLeader = 5;

		public const int ObtainPower = 8;

		public const int LosePower = 9;

		public const int AllowAddStranger = 10;

		public const int RefuseAddStranger = 11;

		public int troopId;

		public long otherId;

		public int operateType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731657;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(troopId);
			oc.push(otherId);
			oc.push(operateType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			troopId = oc.pop_int();
			otherId = oc.pop_long();
			operateType = oc.pop_int();
			return oc;
		}
	}
}
