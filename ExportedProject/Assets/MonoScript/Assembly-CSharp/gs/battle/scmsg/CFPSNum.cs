using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CFPSNum : Message
	{
		public delegate void Handler(CFPSNum msg);

		public const int TYPE = 11537448;

		public static Handler handler;

		public byte fpsNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537448;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(fpsNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			fpsNum = oc.pop_byte();
			return oc;
		}
	}
}
