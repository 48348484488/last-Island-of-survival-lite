using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncEngineSound : Message
	{
		public delegate void Handler(CSyncEngineSound msg);

		public const int TYPE = 11537394;

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
			return 11537394;
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
