using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CLearnDrawing : Message
	{
		public delegate void Handler(CLearnDrawing msg);

		public const int TYPE = 14683079;

		public static Handler handler;

		public int drawingId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683079;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(drawingId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			drawingId = oc.pop_int();
			return oc;
		}
	}
}
