using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COtherShoot : Message
	{
		public delegate void Handler(COtherShoot msg);

		public const int TYPE = 23071677;

		public static Handler handler;

		public long shooterInsId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071677;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shooterInsId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			shooterInsId = oc.pop_long();
			return oc;
		}
	}
}
