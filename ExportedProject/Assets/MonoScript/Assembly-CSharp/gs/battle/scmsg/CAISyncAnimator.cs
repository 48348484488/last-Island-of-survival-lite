using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAISyncAnimator : Message
	{
		public delegate void Handler(CAISyncAnimator msg);

		public const int TYPE = 11537440;

		public static Handler handler;

		public long roleId;

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
			return 11537440;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(layer);
			oc.push(animationHash);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			layer = oc.pop_byte();
			animationHash = oc.pop_int();
			return oc;
		}
	}
}
