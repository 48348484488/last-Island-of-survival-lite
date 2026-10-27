using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CEditorNotice : Message
	{
		public delegate void Handler(CEditorNotice msg);

		public const int TYPE = 32508862;

		public static Handler handler;

		public string notice = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508862;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(notice);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			notice = oc.pop_string();
			return oc;
		}
	}
}
