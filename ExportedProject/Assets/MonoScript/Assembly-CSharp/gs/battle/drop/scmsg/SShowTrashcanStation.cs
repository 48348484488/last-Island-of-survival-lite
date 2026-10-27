using System.Collections.Generic;
using Net;
using Share;
using gs.battle.scmsg;

namespace gs.battle.drop.scmsg
{
	public class SShowTrashcanStation : Message
	{
		public delegate void Handler(SShowTrashcanStation msg);

		public const int TYPE = 12585929;

		public static Handler handler;

		public long instanceId;

		public Vec3 pos = new Vec3();

		public List<TrashcanInfo> trashcanInfos = new List<TrashcanInfo>();

		public int typeId;

		public Vec3 orientation = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585929;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(pos);
			oc.push(trashcanInfos.Count);
			foreach (TrashcanInfo trashcanInfo in trashcanInfos)
			{
				oc.push(trashcanInfo);
			}
			oc.push(typeId);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			oc.pop(pos);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TrashcanInfo trashcanInfo = new TrashcanInfo();
				oc.pop(trashcanInfo);
				trashcanInfos.Add(trashcanInfo);
			}
			typeId = oc.pop_int();
			oc.pop(orientation);
			return oc;
		}
	}
}
