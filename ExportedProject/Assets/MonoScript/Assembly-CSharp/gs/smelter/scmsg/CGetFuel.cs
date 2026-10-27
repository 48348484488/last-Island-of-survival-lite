using Net;
using Share;

namespace gs.smelter.scmsg
{
	public class CGetFuel : Message
	{
		public delegate void Handler(CGetFuel msg);

		public const int TYPE = 17828799;

		public static Handler handler;

		public long smelterId;

		public int getIndex;

		public bool isAll;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 17828799;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(smelterId);
			oc.push(getIndex);
			oc.push(isAll);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			smelterId = oc.pop_long();
			getIndex = oc.pop_int();
			isAll = oc.pop_bool();
			return oc;
		}
	}
}
