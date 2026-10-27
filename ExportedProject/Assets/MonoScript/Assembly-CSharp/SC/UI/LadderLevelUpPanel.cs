using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class LadderLevelUpPanel : View
	{
		private GameObject txt_level;

		private Text txt_levelText;

		private GameObject m_levelup_01;

		private GameObject m_levelup_oversea;

		private GameObject txt_level_oversea;

		private Text txt_level_overseaText;

		protected override void onInit()
		{
			ClickListener.Get(base.gameObject, string.Empty).onClick = _003ConInit_003Em__0;
			m_levelup_oversea.SetActiveBetter(false);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			int level = Singleton<LadderMgr>.Ins.Info.level;
			View.SetLabelText(txt_level, Utils.GetString(149, level));
			base.gameObject.SetActiveBetter(false);
			base.gameObject.SetActiveBetter(true);
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			txt_level = component.GameObjects[0].gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
			m_levelup_01 = component.GameObjects[1].gameObject;
			m_levelup_oversea = component.GameObjects[2].gameObject;
			txt_level_oversea = component.GameObjects[3].gameObject;
			txt_level_overseaText = txt_level_oversea.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}
