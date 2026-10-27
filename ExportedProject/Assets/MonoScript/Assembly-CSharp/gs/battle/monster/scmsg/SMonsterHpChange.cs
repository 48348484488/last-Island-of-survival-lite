using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class SMonsterHpChange : Message
	{
		public delegate void Handler(SMonsterHpChange msg);

		public const int TYPE = 28314568;

		public static Handler handler;

		public long instanceId;

		public long fromInsId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314568;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(fromInsId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			fromInsId = oc.pop_long();
			hp = oc.pop_int();
			return oc;
		}
	}
}
