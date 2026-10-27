using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COtherBulletMiss : Message
	{
		public delegate void Handler(COtherBulletMiss msg);

		public const int TYPE = 23071689;

		public static Handler handler;

		public long otherInsId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071689;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherInsId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherInsId = oc.pop_long();
			return oc;
		}
	}
}
