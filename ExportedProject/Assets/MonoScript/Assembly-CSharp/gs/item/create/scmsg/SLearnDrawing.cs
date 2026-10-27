using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class SLearnDrawing : Message
	{
		public delegate void Handler(SLearnDrawing msg);

		public const int TYPE = 14683080;

		public static Handler handler;

		public int learnedDrawId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683080;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(learnedDrawId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			learnedDrawId = oc.pop_int();
			return oc;
		}
	}
}
