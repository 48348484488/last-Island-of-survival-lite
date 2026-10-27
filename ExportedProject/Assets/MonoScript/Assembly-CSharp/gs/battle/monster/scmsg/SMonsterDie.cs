using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class SMonsterDie : Message
	{
		public delegate void Handler(SMonsterDie msg);

		public const int TYPE = 28314555;

		public static Handler handler;

		public long instanceId;

		public byte bodyPart;

		public Vec3 forward = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314555;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(bodyPart);
			oc.push(forward);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			bodyPart = oc.pop_byte();
			oc.pop(forward);
			return oc;
		}
	}
}
