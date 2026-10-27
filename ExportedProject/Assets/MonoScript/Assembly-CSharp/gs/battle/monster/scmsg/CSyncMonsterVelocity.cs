using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class CSyncMonsterVelocity : Message
	{
		public delegate void Handler(CSyncMonsterVelocity msg);

		public const int TYPE = 28314566;

		public static Handler handler;

		public long instanceId;

		public ShortVec3 velocity = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314566;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(velocity);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			oc.pop(velocity);
			return oc;
		}
	}
}
