using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class SShowMonster : Message
	{
		public delegate void Handler(SShowMonster msg);

		public const int TYPE = 28314553;

		public static Handler handler;

		public MonsterInfo monsterInfo = new MonsterInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314553;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(monsterInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(monsterInfo);
			return oc;
		}
	}
}
