using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncLayerWeight : Message
	{
		public delegate void Handler(CSyncLayerWeight msg);

		public const int TYPE = 11537349;

		public static Handler handler;

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
			return 11537349;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(layer);
			oc.push(value);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			layer = oc.pop_byte();
			value = oc.pop_short();
			return oc;
		}
	}
}
