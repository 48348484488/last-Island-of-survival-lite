using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CCreateArmyGroup : Message
	{
		public delegate void Handler(CCreateArmyGroup msg);

		public const int TYPE = 32508856;

		public static Handler handler;

		public string armyGroupName = string.Empty;

		public string notice = string.Empty;

		public int icon;

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
			return 32508856;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(armyGroupName);
			oc.push(notice);
			oc.push(icon);
			oc.push(autoPassApply);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			armyGroupName = oc.pop_string();
			notice = oc.pop_string();
			icon = oc.pop_int();
			autoPassApply = oc.pop_bool();
			return oc;
		}
	}
}
