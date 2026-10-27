using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class CSyncMonsterPos : Message
	{
		public delegate void Handler(CSyncMonsterPos msg);

		public const int TYPE = 28314562;

		public static Handler handler;

		public long instanceId;

		public Vec3 pos = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314562;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(pos);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			oc.pop(pos);
			return oc;
		}
	}
}
