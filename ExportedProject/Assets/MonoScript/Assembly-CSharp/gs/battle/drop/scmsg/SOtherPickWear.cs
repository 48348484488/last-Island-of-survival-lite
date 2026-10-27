using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SOtherPickWear : Message
	{
		public delegate void Handler(SOtherPickWear msg);

		public const int TYPE = 12585920;

		public static Handler handler;

		public long otherId;

		public int wId;

		public int skinId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585920;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(wId);
			oc.push(skinId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			wId = oc.pop_int();
			skinId = oc.pop_int();
			return oc;
		}
	}
}
