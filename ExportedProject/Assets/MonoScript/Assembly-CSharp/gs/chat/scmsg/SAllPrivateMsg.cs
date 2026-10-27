using System.Collections.Generic;
using Net;
using Share;

namespace gs.chat.scmsg
{
	public class SAllPrivateMsg : Message
	{
		public delegate void Handler(SAllPrivateMsg msg);

		public const int TYPE = 9440190;

		public static Handler handler;

		public List<PrivateBean> beans = new List<PrivateBean>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440190;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(beans.Count);
			foreach (PrivateBean bean in beans)
			{
				oc.push(bean);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				PrivateBean privateBean = new PrivateBean();
				oc.pop(privateBean);
				beans.Add(privateBean);
			}
			return oc;
		}
	}
}
