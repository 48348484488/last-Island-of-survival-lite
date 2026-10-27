using Net;
using Share;

namespace gs.research.scmsg
{
	public class CStartResearch : Message
	{
		public delegate void Handler(CStartResearch msg);

		public const int TYPE = 18877370;

		public static Handler handler;

		public long researchId;

		public int addInstanceId;

		public bool isAddAssist;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 18877370;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(researchId);
			oc.push(addInstanceId);
			oc.push(isAddAssist);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			researchId = oc.pop_long();
			addInstanceId = oc.pop_int();
			isAddAssist = oc.pop_bool();
			return oc;
		}
	}
}
