using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CStopSound : Message
	{
		public delegate void Handler(CStopSound msg);

		public const int TYPE = 11537396;

		public static Handler handler;

		public int id;

		public int soundId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537396;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(soundId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			soundId = oc.pop_int();
			return oc;
		}
	}
}
