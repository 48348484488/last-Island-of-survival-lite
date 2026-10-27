using Net;
using Share;

namespace gs.online.scmsg
{
	public class CGsDynamicMessage : Message
	{
		public delegate void Handler(CGsDynamicMessage msg);

		public const int TYPE = 2100163;

		public static Handler handler;

		public string messageName = string.Empty;

		public Octets data = new Octets();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100163;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(messageName);
			oc.push(data);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			messageName = oc.pop_string();
			data = oc.pop_octets();
			return oc;
		}
	}
}
