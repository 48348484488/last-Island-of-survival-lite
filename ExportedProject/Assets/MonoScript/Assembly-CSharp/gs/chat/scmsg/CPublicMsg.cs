using Net;
using Share;

namespace gs.chat.scmsg
{
	public class CPublicMsg : Message
	{
		public delegate void Handler(CPublicMsg msg);

		public const int TYPE = 9440184;

		public static Handler handler;

		public byte msgType;

		public byte voiceTime;

		public string text = string.Empty;

		public Octets oct = new Octets();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440184;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(msgType);
			oc.push(voiceTime);
			oc.push(text);
			oc.push(oct);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			msgType = oc.pop_byte();
			voiceTime = oc.pop_byte();
			text = oc.pop_string();
			oct = oc.pop_octets();
			return oc;
		}
	}
}
