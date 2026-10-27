using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncAnimator : Message
	{
		public delegate void Handler(CSyncAnimator msg);

		public const int TYPE = 11537347;

		public static Handler handler;

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
			return 11537347;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(layer);
			oc.push(animationHash);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			layer = oc.pop_byte();
			animationHash = oc.pop_int();
			return oc;
		}
	}
}
