using Net;
using Share;

namespace gs.chat.scmsg
{
	public class CPrivateMsg : Message
	{
		public delegate void Handler(CPrivateMsg msg);

		public const int TYPE = 9440188;

		public static Handler handler;

		public long otherId;

		public byte voiceTime;

		public string text = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440188;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(voiceTime);
			oc.push(text);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			voiceTime = oc.pop_byte();
			text = oc.pop_string();
			return oc;
		}
	}
}
