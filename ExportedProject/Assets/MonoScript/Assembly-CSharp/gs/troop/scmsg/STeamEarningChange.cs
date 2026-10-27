using Net;
using Share;

namespace gs.troop.scmsg
{
	public class STeamEarningChange : Message
	{
		public delegate void Handler(STeamEarningChange msg);

		public const int TYPE = 15731661;

		public static Handler handler;

		public int earningValue;

		public bool isCanGet;

		public int getCount;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731661;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(earningValue);
			oc.push(isCanGet);
			oc.push(getCount);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			earningValue = oc.pop_int();
			isCanGet = oc.pop_bool();
			getCount = oc.pop_int();
			return oc;
		}
	}
}
