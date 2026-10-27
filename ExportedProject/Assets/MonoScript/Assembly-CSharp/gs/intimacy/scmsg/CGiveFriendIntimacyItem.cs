using Net;
using Share;

namespace gs.intimacy.scmsg
{
	public class CGiveFriendIntimacyItem : Message
	{
		public delegate void Handler(CGiveFriendIntimacyItem msg);

		public const int TYPE = 30411704;

		public static Handler handler;

		public long targetRoleId;

		public int giveItemId;

		public int giveNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 30411704;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(targetRoleId);
			oc.push(giveItemId);
			oc.push(giveNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			targetRoleId = oc.pop_long();
			giveItemId = oc.pop_int();
			giveNum = oc.pop_int();
			return oc;
		}
	}
}
