using Net;
using Share;

namespace gs.battle.map.board.scmsg
{
	public class CEditorBoardInfo : Message
	{
		public delegate void Handler(CEditorBoardInfo msg);

		public const int TYPE = 26217400;

		public static Handler handler;

		public long boardId;

		public BoardInfo boardInfo = new BoardInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 26217400;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boardId);
			oc.push(boardInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boardId = oc.pop_long();
			oc.pop(boardInfo);
			return oc;
		}
	}
}
