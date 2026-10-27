using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CEditShortcutKey : Message
	{
		public delegate void Handler(CEditShortcutKey msg);

		public const int TYPE = 14683077;

		public static Handler handler;

		public const int ADD = 1;

		public const int DEL = 2;

		public int shortcutId;

		public int editType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683077;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shortcutId);
			oc.push(editType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			shortcutId = oc.pop_int();
			editType = oc.pop_int();
			return oc;
		}
	}
}
