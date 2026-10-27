using System.Collections.Generic;
using Net;
using Share;

namespace gs.role.scmsg
{
	public class SGetRoleMoreInformation : Message
	{
		public delegate void Handler(SGetRoleMoreInformation msg);

		public const int TYPE = 4197325;

		public static Handler handler;

		public int headId;

		public int frameId;

		public string roleName = string.Empty;

		public long roleId;

		public AllianceInfo allianceInfo = new AllianceInfo();

		public int totalTime;

		public int level;

		public HashSet<int> roleShow = new HashSet<int>();

		public HashSet<int> learnedDrawings = new HashSet<int>();

		public bool sex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197325;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(headId);
			oc.push(frameId);
			oc.push(roleName);
			oc.push(roleId);
			oc.push(allianceInfo);
			oc.push(totalTime);
			oc.push(level);
			oc.push(roleShow.Count);
			foreach (int item in roleShow)
			{
				oc.push(item);
			}
			oc.push(learnedDrawings.Count);
			foreach (int learnedDrawing in learnedDrawings)
			{
				oc.push(learnedDrawing);
			}
			oc.push(sex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			headId = oc.pop_int();
			frameId = oc.pop_int();
			roleName = oc.pop_string();
			roleId = oc.pop_long();
			oc.pop(allianceInfo);
			totalTime = oc.pop_int();
			level = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleShow.Add(oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				learnedDrawings.Add(oc.pop_int());
			}
			sex = oc.pop_bool();
			return oc;
		}
	}
}
