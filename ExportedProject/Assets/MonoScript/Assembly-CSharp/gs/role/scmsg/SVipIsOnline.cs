using Net;
using Share;

namespace gs.role.scmsg
{
	public class SVipIsOnline : Message
	{
		public delegate void Handler(SVipIsOnline msg);

		public const int TYPE = 4197323;

		public static Handler handler;

		public long roleId;

		public string name = string.Empty;

		public int headId;

		public int frameId;

		public int vipLevel;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197323;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(name);
			oc.push(headId);
			oc.push(frameId);
			oc.push(vipLevel);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			name = oc.pop_string();
			headId = oc.pop_int();
			frameId = oc.pop_int();
			vipLevel = oc.pop_int();
			return oc;
		}
	}
}
