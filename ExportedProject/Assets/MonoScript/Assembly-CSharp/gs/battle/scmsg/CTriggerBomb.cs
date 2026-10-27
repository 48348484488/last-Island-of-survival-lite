using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CTriggerBomb : Message
	{
		public delegate void Handler(CTriggerBomb msg);

		public const int TYPE = 23071672;

		public static Handler handler;

		public long insId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071672;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			return oc;
		}
	}
}
