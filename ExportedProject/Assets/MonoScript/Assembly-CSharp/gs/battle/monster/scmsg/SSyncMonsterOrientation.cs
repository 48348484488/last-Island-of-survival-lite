using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class SSyncMonsterOrientation : Message
	{
		public delegate void Handler(SSyncMonsterOrientation msg);

		public const int TYPE = 28314565;

		public static Handler handler;

		public long instanceId;

		public ShortVec3 orientation = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314565;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			oc.pop(orientation);
			return oc;
		}
	}
}
