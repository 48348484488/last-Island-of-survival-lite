using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SKillPlayer : Message
	{
		public delegate void Handler(SKillPlayer msg);

		public const int TYPE = 11537343;

		public static Handler handler;

		public const int Die = 1;

		public const int Down = 2;

		public byte msgType;

		public KillPlayerInfo killer = new KillPlayerInfo();

		public KillPlayerInfo die = new KillPlayerInfo();

		public byte bodyPart;

		public byte hitType;

		public int itemId;

		public int killInRow;

		public int killerVipLevel;

		public int dierVipLevel;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537343;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(msgType);
			oc.push(killer);
			oc.push(die);
			oc.push(bodyPart);
			oc.push(hitType);
			oc.push(itemId);
			oc.push(killInRow);
			oc.push(killerVipLevel);
			oc.push(dierVipLevel);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			msgType = oc.pop_byte();
			oc.pop(killer);
			oc.pop(die);
			bodyPart = oc.pop_byte();
			hitType = oc.pop_byte();
			itemId = oc.pop_int();
			killInRow = oc.pop_int();
			killerVipLevel = oc.pop_int();
			dierVipLevel = oc.pop_int();
			return oc;
		}
	}
}
