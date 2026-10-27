using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CPutGunPart : Message
	{
		public delegate void Handler(CPutGunPart msg);

		public const int TYPE = 8391616;

		public static Handler handler;

		public int gunInstanceId;

		public int partInstanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391616;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			oc.push(partInstanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			partInstanceId = oc.pop_int();
			return oc;
		}
	}
}
