using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SOwnDiePlayer : Message
	{
		public delegate void Handler(SOwnDiePlayer msg);

		public const int TYPE = 11537344;

		public static Handler handler;

		public byte msgType;

		public KillPlayerInfo die = new KillPlayerInfo();

		public byte hitType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537344;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(msgType);
			oc.push(die);
			oc.push(hitType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			msgType = oc.pop_byte();
			oc.pop(die);
			hitType = oc.pop_byte();
			return oc;
		}
	}
}
