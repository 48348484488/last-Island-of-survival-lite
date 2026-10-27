using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class UseTimePanel : View
	{
		public class UseTimeItem
		{
			public float Usetime;

			public string Desc;
		}

		public static UseTimeItem useTimeItem = new UseTimeItem();

		private int ItemId;

		private Image usetimeImage;

		private Tweener FillTweener;

		private float usetime;

		private GameObject txt_use_item_name;

		private Text txt_use_item_nameText;

		private GameObject m_use_time;

		private GameObject txt_use_time;

		private Text txt_use_timeText;

		protected override void onInit()
		{
			usetimeImage = m_use_time.GetComponent<Image>();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			usetimeImage.fillAmount = 1f;
			usetime = useTimeItem.Usetime;
			FillTweener = usetimeImage.DOFillAmount(0f, usetime).OnComplete(base.Hide);
			View.SetLabelText(txt_use_item_nameText, useTimeItem.Desc);
		}

		protected override void onHide(string childView = null)
		{
			FillTweener.Kill();
		}

		private void Update()
		{
			View.SetLabelText(txt_use_timeText, usetime.ToString("n1"));
			usetime -= Time.deltaTime;
		}

		protected override void onDestroy()
		{
		}

		public static void ShowUseTimePanel(float time, string desc)
		{
			useTimeItem.Usetime = time;
			useTimeItem.Desc = desc;
			ViewMgr.Ins.ShowTopView<UseTimePanel>();
		}

		public static void HideUseTimePanel()
		{
			ViewMgr.Ins.HideView(typeof(UseTimePanel));
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			txt_use_item_name = component.GameObjects[0].gameObject;
			txt_use_item_nameText = txt_use_item_name.GetComponent<Text>();
			m_use_time = component.GameObjects[1].gameObject;
			txt_use_time = component.GameObjects[2].gameObject;
			txt_use_timeText = txt_use_time.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}
	}
}
