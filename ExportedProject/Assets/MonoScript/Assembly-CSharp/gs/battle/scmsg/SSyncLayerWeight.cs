using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncLayerWeight : Message
	{
		public delegate void Handler(SSyncLayerWeight msg);

		public const int TYPE = 11537350;

		public static Handler handler;

		public long roleId;

		public byte layer;

		public short value;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537350;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(layer);
			oc.push(value);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			layer = oc.pop_byte();
			value = oc.pop_short();
			return oc;
		}
	}
}
