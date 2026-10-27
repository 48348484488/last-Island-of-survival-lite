using System;
using System.Runtime.CompilerServices;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.map.board.scmsg;

namespace SC.UI
{
	public class WoodPanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal int index;

			internal WoodPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.ColorIndex = index;
				_0024this._woodenPlacard.SetColor(_0024this.ColorIndex);
			}
		}

		private GameObject m_alignment;

		private GameObject btn_upper;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_middle;

		private GameObject btn_lower;

		private GameObject btn_left;

		private GameObject btn_center;

		private GameObject btn_right;

		private GameObject m_size;

		private GameObject m_word_size;

		private GameObject txt_current_word_size;

		private Text txt_current_word_sizeText;

		private GameObject m_color;

		private GameObject[] m_word_colors;

		private GameObject m_word_colorsObj;

		private GameObject btn_cancel;

		private GameObject btn_ok;

		private GameObject m_model_root;

		private GameObject m_role_bg;

		private GameObject m_hero_tex;

		private GameObject m_model_role;

		private GameObject m_camera;

		private GameObject m_model;

		private GameObject inp_content;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		private GameObject txt_on_3;

		private Text txt_on_3Text;

		private GameObject txt_off_3;

		private Text txt_off_3Text;

		private GameObject txt_on_4;

		private Text txt_on_4Text;

		private GameObject txt_off_4;

		private Text txt_off_4Text;

		private GameObject txt_on_5;

		private Text txt_on_5Text;

		private GameObject txt_off_5;

		private Text txt_off_5Text;

		private GameObject txt_on_6;

		private Text txt_on_6Text;

		private GameObject txt_off_6;

		private Text txt_off_6Text;

		private GameObject txt_on_7;

		private Text txt_on_7Text;

		private GameObject txt_off_7;

		private Text txt_off_7Text;

		private GameObject txt_on_8;

		private Text txt_on_8Text;

		private GameObject txt_off_8;

		private Text txt_off_8Text;

		private GameObject txt_on_9;

		private Text txt_on_9Text;

		private GameObject txt_off_9;

		private Text txt_off_9Text;

		private GameObject txt_on_10;

		private Text txt_on_10Text;

		private GameObject txt_off_10;

		private Text txt_off_10Text;

		private GameObject txt_on_11;

		private Text txt_on_11Text;

		private GameObject txt_off_11;

		private Text txt_off_11Text;

		private GameObject txt_on_12;

		private Text txt_on_12Text;

		private GameObject txt_off_12;

		private Text txt_off_12Text;

		private GameObject txt_on_13;

		private Text txt_on_13Text;

		private GameObject txt_off_13;

		private Text txt_off_13Text;

		private GameObject txt_on_14;

		private Text txt_on_14Text;

		private GameObject txt_off_14;

		private Text txt_off_14Text;

		private GameObject txt_on_15;

		private Text txt_on_15Text;

		private GameObject txt_off_15;

		private Text txt_off_15Text;

		private GameObject txt_on_16;

		private Text txt_on_16Text;

		private GameObject txt_off_16;

		private Text txt_off_16Text;

		private WoodenPlacard _origainWoodenPlacard;

		private long _instanceId;

		private int _itemId;

		private Camera _camera;

		private RenderTexture _renderTexture;

		private WoodenPlacard _woodenPlacard;

		private WoodenPlacardMgr.HorizontalAlignment _horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Center;

		private WoodenPlacardMgr.VerticalAlignment _verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Middle;

		private int _colorIndex;

		private int _wordSize = 1;

		private Dropdown _wordSizeDropdown;

		private InputField _inputField;

		private BoardInfo _boardInfo = new BoardInfo();

		private GameObject _currentGunGameObject;

		private int ColorIndex
		{
			get
			{
				return _colorIndex;
			}
			set
			{
				_colorIndex = ((value >= m_word_colors.Length) ? m_word_colors.Length : value);
			}
		}

		private int WordSize
		{
			get
			{
				return _wordSize;
			}
			set
			{
				if (value > 10)
				{
					_wordSize = 10;
				}
				else if (value < 1)
				{
					_wordSize = 1;
				}
				else
				{
					_wordSize = value;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_alignment = component.GameObjects[0].gameObject;
			btn_upper = component.GameObjects[1].gameObject;
			txt_on = component.GameObjects[2].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[3].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_middle = component.GameObjects[4].gameObject;
			btn_lower = component.GameObjects[5].gameObject;
			btn_left = component.GameObjects[6].gameObject;
			btn_center = component.GameObjects[7].gameObject;
			btn_right = component.GameObjects[8].gameObject;
			m_size = component.GameObjects[9].gameObject;
			m_word_size = component.GameObjects[10].gameObject;
			txt_current_word_size = component.GameObjects[11].gameObject;
			txt_current_word_sizeText = txt_current_word_size.GetComponent<Text>();
			m_color = component.GameObjects[12].gameObject;
			m_word_colors = component.GameObjects[13].gameObject.GetComponent<UIGameObjectList>().objects;
			m_word_colorsObj = component.GameObjects[13].gameObject;
			btn_cancel = component.GameObjects[14].gameObject;
			btn_ok = component.GameObjects[15].gameObject;
			m_model_root = component.GameObjects[16].gameObject;
			m_role_bg = component.GameObjects[17].gameObject;
			m_hero_tex = component.GameObjects[18].gameObject;
			m_model_role = component.GameObjects[19].gameObject;
			m_camera = component.GameObjects[20].gameObject;
			m_model = component.GameObjects[21].gameObject;
			inp_content = component.GameObjects[22].gameObject;
			txt_on_0 = component.GameObjects[23].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[24].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[25].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_1 = component.GameObjects[26].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[27].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_2 = component.GameObjects[28].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			txt_on_3 = component.GameObjects[29].gameObject;
			txt_on_3Text = txt_on_3.GetComponent<Text>();
			txt_off_3 = component.GameObjects[30].gameObject;
			txt_off_3Text = txt_off_3.GetComponent<Text>();
			txt_on_4 = component.GameObjects[31].gameObject;
			txt_on_4Text = txt_on_4.GetComponent<Text>();
			txt_off_4 = component.GameObjects[32].gameObject;
			txt_off_4Text = txt_off_4.GetComponent<Text>();
			txt_on_5 = component.GameObjects[33].gameObject;
			txt_on_5Text = txt_on_5.GetComponent<Text>();
			txt_off_5 = component.GameObjects[34].gameObject;
			txt_off_5Text = txt_off_5.GetComponent<Text>();
			txt_on_6 = component.GameObjects[35].gameObject;
			txt_on_6Text = txt_on_6.GetComponent<Text>();
			txt_off_6 = component.GameObjects[36].gameObject;
			txt_off_6Text = txt_off_6.GetComponent<Text>();
			txt_on_7 = component.GameObjects[37].gameObject;
			txt_on_7Text = txt_on_7.GetComponent<Text>();
			txt_off_7 = component.GameObjects[38].gameObject;
			txt_off_7Text = txt_off_7.GetComponent<Text>();
			txt_on_8 = component.GameObjects[39].gameObject;
			txt_on_8Text = txt_on_8.GetComponent<Text>();
			txt_off_8 = component.GameObjects[40].gameObject;
			txt_off_8Text = txt_off_8.GetComponent<Text>();
			txt_on_9 = component.GameObjects[41].gameObject;
			txt_on_9Text = txt_on_9.GetComponent<Text>();
			txt_off_9 = component.GameObjects[42].gameObject;
			txt_off_9Text = txt_off_9.GetComponent<Text>();
			txt_on_10 = component.GameObjects[43].gameObject;
			txt_on_10Text = txt_on_10.GetComponent<Text>();
			txt_off_10 = component.GameObjects[44].gameObject;
			txt_off_10Text = txt_off_10.GetComponent<Text>();
			txt_on_11 = component.GameObjects[45].gameObject;
			txt_on_11Text = txt_on_11.GetComponent<Text>();
			txt_off_11 = component.GameObjects[46].gameObject;
			txt_off_11Text = txt_off_11.GetComponent<Text>();
			txt_on_12 = component.GameObjects[47].gameObject;
			txt_on_12Text = txt_on_12.GetComponent<Text>();
			txt_off_12 = component.GameObjects[48].gameObject;
			txt_off_12Text = txt_off_12.GetComponent<Text>();
			txt_on_13 = component.GameObjects[49].gameObject;
			txt_on_13Text = txt_on_13.GetComponent<Text>();
			txt_off_13 = component.GameObjects[50].gameObject;
			txt_off_13Text = txt_off_13.GetComponent<Text>();
			txt_on_14 = component.GameObjects[51].gameObject;
			txt_on_14Text = txt_on_14.GetComponent<Text>();
			txt_off_14 = component.GameObjects[52].gameObject;
			txt_off_14Text = txt_off_14.GetComponent<Text>();
			txt_on_15 = component.GameObjects[53].gameObject;
			txt_on_15Text = txt_on_15.GetComponent<Text>();
			txt_off_15 = component.GameObjects[54].gameObject;
			txt_off_15Text = txt_off_15.GetComponent<Text>();
			txt_on_16 = component.GameObjects[55].gameObject;
			txt_on_16Text = txt_on_16.GetComponent<Text>();
			txt_off_16 = component.GameObjects[56].gameObject;
			txt_off_16Text = txt_off_16.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		protected override void onInit()
		{
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			ClickListener.Get(btn_upper, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_lower, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_middle, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_left, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_center, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_right, string.Empty).onClick = _003ConInit_003Em__5;
			_wordSizeDropdown = m_word_size.GetComponent<Dropdown>();
			_wordSizeDropdown.onValueChanged.AddListener(OnSizeChanged);
			for (int i = 0; i < m_word_colors.Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.index = i;
				ClickListener.Get(m_word_colors[i], string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
			}
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener.Get(btn_ok, string.Empty).onClick = _003ConInit_003Em__7;
			_inputField = inp_content.GetComponent<InputField>();
			_inputField.onValueChanged.AddListener(OnContentChanged);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			Clear();
			_instanceId = (long)param;
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(_instanceId);
			_itemId = partByInsID.Id;
			_origainWoodenPlacard = partByInsID.GetComponent<WoodenPlacard>();
			_boardInfo.extraInfo = _origainWoodenPlacard.Code;
			_boardInfo.info = _origainWoodenPlacard.Content;
			InitSetting();
			SUpdateBoardInfo.handler = (SUpdateBoardInfo.Handler)Delegate.Combine(SUpdateBoardInfo.handler, new SUpdateBoardInfo.Handler(SUpdateBoardInfoHandle));
		}

		protected override void onHide(string childView = null)
		{
			SUpdateBoardInfo.handler = (SUpdateBoardInfo.Handler)Delegate.Remove(SUpdateBoardInfo.handler, new SUpdateBoardInfo.Handler(SUpdateBoardInfoHandle));
		}

		private void SUpdateBoardInfoHandle(SUpdateBoardInfo msg)
		{
			if (msg.boardId == _instanceId)
			{
				Hide();
			}
		}

		protected override void onDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		public void ShowMode(int itemId)
		{
			m_role_bg.SetActiveBetter(false);
			BuildPart buildPart = BuildPart.Get(_itemId);
			ResMgr.Ins.CreateFromAB(buildPart.model, null, _003CShowMode_003Em__8);
		}

		public void RefreshMode()
		{
			ShowMode(_itemId);
		}

		public void RefreshUi()
		{
			if ((bool)_woodenPlacard)
			{
				_woodenPlacard.Unmarshal(_boardInfo.extraInfo, _boardInfo.info);
			}
			RefreshAlignment();
			RefreshSize();
			RefreshColor();
		}

		public void RefreshAlignment()
		{
			btn_upper.GetComponent<RadioButton>().isChecked = _verticalAlignment == WoodenPlacardMgr.VerticalAlignment.Up;
			btn_lower.GetComponent<RadioButton>().isChecked = _verticalAlignment == WoodenPlacardMgr.VerticalAlignment.Down;
			btn_middle.GetComponent<RadioButton>().isChecked = _verticalAlignment == WoodenPlacardMgr.VerticalAlignment.Middle;
			btn_left.GetComponent<RadioButton>().isChecked = _horizontalAlignment == WoodenPlacardMgr.HorizontalAlignment.Left;
			btn_right.GetComponent<RadioButton>().isChecked = _horizontalAlignment == WoodenPlacardMgr.HorizontalAlignment.Right;
			btn_center.GetComponent<RadioButton>().isChecked = _horizontalAlignment == WoodenPlacardMgr.HorizontalAlignment.Center;
		}

		public void RefreshSize()
		{
			_wordSizeDropdown.value = WordSize - 1;
		}

		public void RefreshColor()
		{
			RadioButton.ChooseBtn(m_word_colors[ColorIndex]);
		}

		public void RefreshText()
		{
			_inputField.text = _boardInfo.info;
		}

		public void InitSetting()
		{
			if (!string.IsNullOrEmpty(_boardInfo.extraInfo))
			{
				Unmarshal(_boardInfo.extraInfo, _boardInfo.info);
			}
			RefreshMode();
		}

		public void Marshal()
		{
			_boardInfo.extraInfo = _instanceId + "_" + _itemId + "_" + ColorIndex + "_" + WordSize + "_" + (int)_horizontalAlignment + "_" + (int)_verticalAlignment + "_" + _woodenPlacard.PosX + "_" + _woodenPlacard.PosY;
		}

		public void Clear()
		{
			_boardInfo.info = string.Empty;
			_inputField.text = _boardInfo.info;
			ColorIndex = 0;
			WordSize = 1;
			_boardInfo.extraInfo = string.Empty;
			_horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Center;
			_verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Middle;
		}

		public void Unmarshal(string code, string content)
		{
			_boardInfo.extraInfo = code;
			string[] array = _boardInfo.extraInfo.Split('_');
			if (array.Length > 0)
			{
				_instanceId = long.Parse(array[0]);
				_itemId = int.Parse(array[1]);
				ColorIndex = int.Parse(array[2]);
				WordSize = int.Parse(array[3]);
				_horizontalAlignment = (WoodenPlacardMgr.HorizontalAlignment)int.Parse(array[4]);
				_verticalAlignment = (WoodenPlacardMgr.VerticalAlignment)int.Parse(array[5]);
				_boardInfo.info = content;
				_inputField.text = _boardInfo.info;
			}
		}

		private void OnSizeChanged(int index)
		{
			WordSize = index + 1;
			_woodenPlacard.SetWordSize(WordSize, OnInpuCallBack);
		}

		private void OnContentChanged(string content)
		{
			_boardInfo.info = content;
			if ((bool)_woodenPlacard)
			{
				_woodenPlacard.SetWordContentByUi(content, OnInpuCallBack);
			}
		}

		private void OnInpuCallBack(string content)
		{
			_inputField.text = content;
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			_verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Up;
			_woodenPlacard.SetVerticalAlignment(_verticalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			_verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Down;
			_woodenPlacard.SetVerticalAlignment(_verticalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			_verticalAlignment = WoodenPlacardMgr.VerticalAlignment.Middle;
			_woodenPlacard.SetVerticalAlignment(_verticalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			_horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Left;
			_woodenPlacard.SetHorizontalAlignment(_horizontalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			_horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Center;
			_woodenPlacard.SetHorizontalAlignment(_horizontalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			_horizontalAlignment = WoodenPlacardMgr.HorizontalAlignment.Right;
			_woodenPlacard.SetHorizontalAlignment(_horizontalAlignment);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			Marshal();
			if (string.IsNullOrEmpty(_boardInfo.info))
			{
				AlertBox.Show(261);
			}
			else
			{
				Singleton<WoodenPlacardMgr>.Ins.EditorBoardInfo(_instanceId, _boardInfo.extraInfo, _boardInfo.info);
			}
		}

		[CompilerGenerated]
		private void _003CShowMode_003Em__8(GameObject go)
		{
			if ((bool)_currentGunGameObject)
			{
				UnityEngine.Object.DestroyImmediate(_currentGunGameObject);
			}
			go.transform.SetParent(m_model.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			_currentGunGameObject = go;
			_woodenPlacard = go.AddComponent<WoodenPlacard>();
			_woodenPlacard.Unmarshal(_boardInfo.extraInfo, _boardInfo.info);
			RefreshUi();
		}
	}
}
