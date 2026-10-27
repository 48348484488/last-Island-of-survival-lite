using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CRebirth : Message
	{
		public delegate void Handler(CRebirth msg);

		public const int TYPE = 11537510;

		public static Handler handler;

		public const int Random = 1;

		public const int FromDiePos = 2;

		public const int RebirthPos = 3;

		public int type;

		public long rebirthPosId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537510;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(type);
			oc.push(rebirthPosId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			type = oc.pop_int();
			rebirthPosId = oc.pop_long();
			return oc;
		}
	}
}
