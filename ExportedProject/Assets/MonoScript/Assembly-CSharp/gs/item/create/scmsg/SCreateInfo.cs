using System.Collections.Generic;
using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class SCreateInfo : Message
	{
		public delegate void Handler(SCreateInfo msg);

		public const int TYPE = 14683065;

		public static Handler handler;

		public int errorcode;

		public HashSet<int> learnedDrawings = new HashSet<int>();

		public List<int> shortcutKeys = new List<int>();

		public List<CreateInfo> ceateInfos = new List<CreateInfo>();

		public int passTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683065;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(errorcode);
			oc.push(learnedDrawings.Count);
			foreach (int learnedDrawing in learnedDrawings)
			{
				oc.push(learnedDrawing);
			}
			oc.push(shortcutKeys.Count);
			foreach (int shortcutKey in shortcutKeys)
			{
				oc.push(shortcutKey);
			}
			oc.push(ceateInfos.Count);
			foreach (CreateInfo ceateInfo in ceateInfos)
			{
				oc.push(ceateInfo);
			}
			oc.push(passTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			errorcode = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				learnedDrawings.Add(oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				shortcutKeys.Add(oc.pop_int());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				CreateInfo createInfo = new CreateInfo();
				oc.pop(createInfo);
				ceateInfos.Add(createInfo);
			}
			passTime = oc.pop_int();
			return oc;
		}
	}
}
