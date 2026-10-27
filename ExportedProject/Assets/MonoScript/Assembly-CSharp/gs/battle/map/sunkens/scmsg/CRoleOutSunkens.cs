using Net;
using Share;

namespace gs.battle.map.sunkens.scmsg
{
	public class CRoleOutSunkens : Message
	{
		public delegate void Handler(CRoleOutSunkens msg);

		public const int TYPE = 24120249;

		public static Handler handler;

		public long sunkensId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 24120249;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(sunkensId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			sunkensId = oc.pop_long();
			return oc;
		}
	}
}
