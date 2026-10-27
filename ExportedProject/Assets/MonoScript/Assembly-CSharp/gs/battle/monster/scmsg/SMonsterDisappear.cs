using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class SMonsterDisappear : Message
	{
		public delegate void Handler(SMonsterDisappear msg);

		public const int TYPE = 28314554;

		public static Handler handler;

		public long instanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314554;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			return oc;
		}
	}
}
