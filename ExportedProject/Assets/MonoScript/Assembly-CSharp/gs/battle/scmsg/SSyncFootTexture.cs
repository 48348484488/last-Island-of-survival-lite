using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncFootTexture : Message
	{
		public delegate void Handler(SSyncFootTexture msg);

		public const int TYPE = 11537389;

		public static Handler handler;

		public long roleId;

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
			return 11537389;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(textureType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			textureType = oc.pop_byte();
			return oc;
		}
	}
}
