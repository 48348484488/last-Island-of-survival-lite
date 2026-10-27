using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncEngineSound : Message
	{
		public delegate void Handler(SSyncEngineSound msg);

		public const int TYPE = 11537395;

		public static Handler handler;

		public int id;

		public float volume;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537395;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(volume);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			volume = oc.pop_float();
			return oc;
		}
	}
}
