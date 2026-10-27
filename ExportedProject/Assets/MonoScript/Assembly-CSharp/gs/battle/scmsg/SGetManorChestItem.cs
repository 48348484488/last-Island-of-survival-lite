using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGetManorChestItem : Message
	{
		public delegate void Handler(SGetManorChestItem msg);

		public const int TYPE = 11537531;

		public static Handler handler;

		public long chestInstanceId;

		public int index;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537531;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(chestInstanceId);
			oc.push(index);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			chestInstanceId = oc.pop_long();
			index = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}
