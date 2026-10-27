using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class NumberPanel : View
	{
		private GameObject m_rec_bg;

		private GameObject m_recording;

		private GameObject m_rec_bg_0;

		private GameObject btn_close_rec;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject inp_num;

		private GameObject btn_reduce;

		private GameObject btn_add;

		private GameObject btn_max;

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_rec_bg = component.GameObjects[0].gameObject;
			m_recording = component.GameObjects[1].gameObject;
			m_rec_bg_0 = component.GameObjects[2].gameObject;
			btn_close_rec = component.GameObjects[3].gameObject;
			m_icon = component.GameObjects[4].gameObject;
			txt_name = component.GameObjects[5].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			inp_num = component.GameObjects[6].gameObject;
			btn_reduce = component.GameObjects[7].gameObject;
			btn_add = component.GameObjects[8].gameObject;
			btn_max = component.GameObjects[9].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
