using Net;
using Share;

namespace gs.troop.scmsg
{
	public class STroopConfig : Message
	{
		public delegate void Handler(STroopConfig msg);

		public const int TYPE = 15731641;

		public static Handler handler;

		public bool isAutoAcceptApply;

		public bool isAutoAcceptInvite;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731641;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(isAutoAcceptApply);
			oc.push(isAutoAcceptInvite);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			isAutoAcceptApply = oc.pop_bool();
			isAutoAcceptInvite = oc.pop_bool();
			return oc;
		}
	}
}
