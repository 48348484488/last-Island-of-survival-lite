using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class CSyncMonsterAnimator : Message
	{
		public delegate void Handler(CSyncMonsterAnimator msg);

		public const int TYPE = 28314560;

		public static Handler handler;

		public long instanceId;

		public byte layer;

		public int animationHash;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314560;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(layer);
			oc.push(animationHash);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			layer = oc.pop_byte();
			animationHash = oc.pop_int();
			return oc;
		}
	}
}
