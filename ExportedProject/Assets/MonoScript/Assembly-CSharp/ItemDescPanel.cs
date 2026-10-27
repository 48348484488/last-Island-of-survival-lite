using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class ItemDescPanel : View
{
	public class ItemDescPanelParam
	{
		public string name;

		public int num;

		public string desc;
	}

	private float Width;

	private float Height;

	private const int OFF_XY = 20;

	private static Canvas _canvas;

	private Vector3 _offectScreen = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);

	public static ItemDescPanelParam Param = new ItemDescPanelParam();

	private GameObject btn_back;

	private GameObject m_desc;

	private GameObject txt_des;

	private Text txt_desText;

	private GameObject txt_name;

	private Text txt_nameText;

	private GameObject txt_num;

	private Text txt_numText;

	private static Canvas Canvas
	{
		get
		{
			return _canvas ?? (_canvas = GameObject.Find("UIRootCanvas").GetComponent<Canvas>());
		}
	}

	protected override void onInit()
	{
		UIEventListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
		UIEventListener.Get(btn_back, string.Empty).onUp = _003ConInit_003Em__1;
		Height = (m_desc.transform as RectTransform).rect.height;
		Width = (m_desc.transform as RectTransform).rect.width;
	}

	public override void _SetRenderSort(int RendingSort)
	{
		base._SetRenderSort(10);
	}

	protected override void onShow(object param = null, string childView = null)
	{
		if (!(Canvas == null))
		{
			Vector2 localPoint;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, Input.mousePosition, Canvas.worldCamera, out localPoint))
			{
				m_desc.GetComponent<RectTransform>().anchoredPosition = localPoint;
			}
			View.SetLabelText(txt_desText, Param.desc);
			View.SetLabelText(txt_nameText, Param.name);
			View.SetLabelText(txt_numText, Utils.GetString(17, Param.num));
			m_desc.transform.localPosition = localPoint;
			Vector3 localPosition = m_desc.transform.localPosition;
			float y = ((!(localPosition.y + Height < ViewMgr.Ins.CanvasSize.y / 2f)) ? (localPosition.y - Height / 2f - 20f) : (localPosition.y + Height / 2f + 20f));
			float x = ((!(localPosition.x + Width < ViewMgr.Ins.CanvasSize.x / 2f)) ? (localPosition.x - Width / 2f - 20f) : (localPosition.x + Width / 2f + 20f));
			m_desc.transform.localPosition = new Vector3(x, y, 1f);
		}
	}

	public static void ShowItemDescPanel(int itemId, int num = 1)
	{
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg != null)
		{
			ShowDescPanel(itemCfg.name, num, itemCfg.desc);
		}
	}

	public static void ShowWordDescPanel(int wordId, int num = 1)
	{
	}

	public static void ShowHeroDescPanel(int heroId, int num = 1)
	{
	}

	public static void ShowDescPanel(int itemId, int num = 1, int itemType = 3)
	{
		switch (itemType)
		{
		case 3:
			ShowItemDescPanel(itemId, num);
			break;
		case 12:
			ShowHeroDescPanel(itemId, num);
			break;
		case 15:
			ShowWordDescPanel(itemId, num);
			break;
		default:
			ShowItemDescPanel(itemId, num);
			break;
		}
	}

	public static void ShowDescPanel(string name, int num, string desc)
	{
		Param.name = name;
		Param.num = num;
		Param.desc = desc;
		ViewMgr.Ins.ShowTopView<ItemDescPanel>(Param);
	}

	public static void HideItemDescPanel()
	{
		ViewMgr.Ins.HideView<ItemDescPanel>();
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
		btn_back = component.GameObjects[0].gameObject;
		m_desc = component.GameObjects[1].gameObject;
		txt_des = component.GameObjects[2].gameObject;
		txt_desText = txt_des.GetComponent<Text>();
		txt_name = component.GameObjects[3].gameObject;
		txt_nameText = txt_name.GetComponent<Text>();
		txt_num = component.GameObjects[4].gameObject;
		txt_numText = txt_num.GetComponent<Text>();
		ViewMgr.Ins.addView(this);
	}

	[CompilerGenerated]
	private void _003ConInit_003Em__0(GameObject go)
	{
		Hide();
	}

	[CompilerGenerated]
	private void _003ConInit_003Em__1(GameObject go)
	{
		Hide();
	}
}
