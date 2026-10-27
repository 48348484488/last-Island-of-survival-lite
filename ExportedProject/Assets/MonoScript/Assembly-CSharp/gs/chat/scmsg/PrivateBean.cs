using Share;
using gs.role.scmsg;

namespace gs.chat.scmsg
{
	public class PrivateBean : Marshal
	{
		public RoleVersion role = new RoleVersion();

		public byte voiceTime;

		public string text = string.Empty;

		public int sendTime;

		public Octets marshal(Octets oc)
		{
			oc.push(role);
			oc.push(voiceTime);
			oc.push(text);
			oc.push(sendTime);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(role);
			voiceTime = oc.pop_byte();
			text = oc.pop_string();
			sendTime = oc.pop_int();
			return oc;
		}
	}
}
