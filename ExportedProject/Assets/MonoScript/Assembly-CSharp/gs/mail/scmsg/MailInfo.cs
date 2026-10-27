using Share;
using gs.drop.scmsg;

namespace gs.mail.scmsg
{
	public class MailInfo : Marshal
	{
		public int id;

		public string senderName = string.Empty;

		public string title = string.Empty;

		public string content = string.Empty;

		public bool isRead;

		public bool canGetReward;

		public DropDetail dropDetail = new DropDetail();

		public int timeCreate;

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(senderName);
			oc.push(title);
			oc.push(content);
			oc.push(isRead);
			oc.push(canGetReward);
			oc.push(dropDetail);
			oc.push(timeCreate);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			senderName = oc.pop_string();
			title = oc.pop_string();
			content = oc.pop_string();
			isRead = oc.pop_bool();
			canGetReward = oc.pop_bool();
			oc.pop(dropDetail);
			timeCreate = oc.pop_int();
			return oc;
		}
	}
}
