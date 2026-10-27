using Share;
using gs.role.scmsg;

namespace gs.chat.scmsg
{
	public class MsgBean : Marshal
	{
		public RoleVersion role = new RoleVersion();

		public byte msgType;

		public byte voiceTime;

		public string text = string.Empty;

		public long extraId;

		public Octets oct = new Octets();

		public Octets marshal(Octets oc)
		{
			oc.push(role);
			oc.push(msgType);
			oc.push(voiceTime);
			oc.push(text);
			oc.push(extraId);
			oc.push(oct);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(role);
			msgType = oc.pop_byte();
			voiceTime = oc.pop_byte();
			text = oc.pop_string();
			extraId = oc.pop_long();
			oct = oc.pop_octets();
			return oc;
		}
	}
}
