using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class STrashCanDisappear : Message
	{
		public delegate void Handler(STrashCanDisappear msg);

		public const int TYPE = 12585928;

		public static Handler handler;

		public long objId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585928;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(objId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			objId = oc.pop_long();
			return oc;
		}
	}
}
