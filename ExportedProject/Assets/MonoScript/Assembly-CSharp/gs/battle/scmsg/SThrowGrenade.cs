using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SThrowGrenade : Message
	{
		public delegate void Handler(SThrowGrenade msg);

		public const int TYPE = 11537401;

		public static Handler handler;

		public long playerInsId;

		public long grenadeInsId;

		public int grenadeTypeId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537401;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(playerInsId);
			oc.push(grenadeInsId);
			oc.push(grenadeTypeId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			playerInsId = oc.pop_long();
			grenadeInsId = oc.pop_long();
			grenadeTypeId = oc.pop_int();
			return oc;
		}
	}
}
