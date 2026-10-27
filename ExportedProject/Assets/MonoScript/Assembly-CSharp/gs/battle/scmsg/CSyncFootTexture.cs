using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncFootTexture : Message
	{
		public delegate void Handler(CSyncFootTexture msg);

		public const int TYPE = 11537388;

		public static Handler handler;

		public byte textureType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537388;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(textureType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			textureType = oc.pop_byte();
			return oc;
		}
	}
}
