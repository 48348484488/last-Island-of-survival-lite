using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CEditorAutoPassApply : Message
	{
		public delegate void Handler(CEditorAutoPassApply msg);

		public const int TYPE = 32508864;

		public static Handler handler;

		public bool autoPassApply;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508864;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(autoPassApply);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			autoPassApply = oc.pop_bool();
			return oc;
		}
	}
}
