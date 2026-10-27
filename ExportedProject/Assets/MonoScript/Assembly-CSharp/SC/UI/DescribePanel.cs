using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class DescribePanel : View
	{
		private GameObject m_recording;

		private GameObject m_rec_bg;

		private GameObject btn_close_rec;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_desc;

		private Text txt_descText;

		private GameObject btn_go;

		private GameObject btn_confirm;

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_recording = component.GameObjects[0].gameObject;
			m_rec_bg = component.GameObjects[1].gameObject;
			btn_close_rec = component.GameObjects[2].gameObject;
			m_icon = component.GameObjects[3].gameObject;
			txt_name = component.GameObjects[4].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_desc = component.GameObjects[5].gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			btn_go = component.GameObjects[6].gameObject;
			btn_confirm = component.GameObjects[7].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
