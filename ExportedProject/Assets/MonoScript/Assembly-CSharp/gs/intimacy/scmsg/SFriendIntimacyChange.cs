using Net;
using Share;

namespace gs.intimacy.scmsg
{
	public class SFriendIntimacyChange : Message
	{
		public delegate void Handler(SFriendIntimacyChange msg);

		public const int TYPE = 30411705;

		public static Handler handler;

		public long targetRoleId;

		public int intimacyLevel;

		public int intimacyValue;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 30411705;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(targetRoleId);
			oc.push(intimacyLevel);
			oc.push(intimacyValue);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			targetRoleId = oc.pop_long();
			intimacyLevel = oc.pop_int();
			intimacyValue = oc.pop_int();
			return oc;
		}
	}
}
