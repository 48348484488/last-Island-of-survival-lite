using Net;
using Share;

namespace gs.troop.scmsg
{
	public class COperateCMD : Message
	{
		public delegate void Handler(COperateCMD msg);

		public const int TYPE = 15731656;

		public static Handler handler;

		public long otherId;

		public int operateType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731656;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(operateType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			operateType = oc.pop_int();
			return oc;
		}
	}
}
