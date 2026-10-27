using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.online.scmsg;

namespace SC.UI
{
	public class EstablishPanel : View
	{
		private enum Sex
		{
			None = 0,
			Male = 1,
			Female = 2
		}

		private const string DefaultUserName = "";

		private List<EstablishRole> _establishRolelist;

		private EstablishRole _currentEstablishRole;

		private int _currentEstablishRoleId;

		private int _baseLayer;

		private List<MultiLanguageNameCfg> _multiLanguageNames;

		private RenderTexture _renderTexture;

		private Vector3 _defaultAngle = new Vector3(0f, 180f, 0f);

		private Sex _currentSex;

		private const int MaleId = 1;

		private const int FeMaleId = 2;

		private readonly string REGEX_STRING = " `~!@#$%^&*()+=|{}':;',[].<>/?~！@#￥%&*（）——+|{}【】‘；：”“’。，、？\"‥…▪•";

		private static int nameLength;

		private static int nameindex;

		private RenderTexture _heroRender;

		private Camera _camera;

		private List<int> _nowSkinsMale = new List<int>();

		private HeroSkin _heroSkinMale;

		private List<int> _nowSkinsFemale = new List<int>();

		private HeroSkin _heroSkinFemale;

		private GameObject m_hero_tex;

		private GameObject inp_inputName;

		private GameObject btn_name;

		private GameObject btn_creat;

		private GameObject m_role_model;

		private GameObject m_camera;

		private GameObject btn_female;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_male;

		private GameObject m_role_model_male_root;

		private GameObject m_role_model_female_root;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			ClickListener.Get(btn_creat, string.Empty).onClick = OnClickOk;
			btn_creat.SetActive(false);
			btn_creat.SetActive(true);
			UIEventListener.Get(btn_name, string.Empty).onClick = OnClickRandomName;
			UIEventListener.Get(m_hero_tex, string.Empty).onDrag = OnDragHero;
			UIEventListener.Get(btn_male, string.Empty).onClick = _003ConInit_003Em__0;
			UIEventListener.Get(btn_female, string.Empty).onClick = _003ConInit_003Em__1;
			View.SetInputText(inp_inputName, string.Empty);
			_establishRolelist = EstablishRole.GetAllList();
			_multiLanguageNames = MultiLanguageNameCfg.GetAllList();
		}

		private void OnClickReturn(GameObject go)
		{
			Singleton<LoginMgr>.Ins.CloseGs();
			ViewMgr.Ins.ShowView<LoginPanel>();
		}

		public void OnDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		private void OnClickOk(GameObject go)
		{
			TweenTime.Begin(go, 1.5f, _003COnClickOk_003Em__2);
		}

		private void OnDragHero(GameObject go)
		{
			if (UIEventListener.pointEventData.delta.x < -1f)
			{
				GameObject gameObject = ((_currentSex != Sex.Female) ? m_role_model_male_root : m_role_model_female_root);
				gameObject.transform.Rotate(Vector3.up * 600f * Time.deltaTime, Space.World);
			}
			else if (UIEventListener.pointEventData.delta.x > 1f)
			{
				GameObject gameObject2 = ((_currentSex != Sex.Female) ? m_role_model_male_root : m_role_model_female_root);
				gameObject2.transform.Rotate(Vector3.up * -600f * Time.deltaTime, Space.World);
			}
		}

		private bool CheckName(string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
			{
				AlertBox.Show(Utils.GetString(132));
				return false;
			}
			if (ContainSpecialCharacter(playerName))
			{
				AlertBox.Show(Utils.GetString(131));
				return false;
			}
			return true;
		}

		private bool ContainSpecialCharacter(string str)
		{
			for (int num = str.Length - 1; num >= 0; num--)
			{
				for (int num2 = REGEX_STRING.Length - 1; num2 >= 0; num2--)
				{
					if (str[num] == REGEX_STRING[num2])
					{
						return true;
					}
				}
			}
			return false;
		}

		private int LengthString(string name)
		{
			int num = 0;
			for (int num2 = name.Length - 1; num2 >= 0; num2--)
			{
				num = ((name[num2] < '\0' || name[num2] >= '\u007f') ? (num + 2) : (num + 1));
			}
			return num;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			btn_creat.SetActive(false);
			btn_creat.SetActive(true);
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinish));
			m_role_model_male_root.transform.localEulerAngles = _defaultAngle;
			m_role_model_female_root.transform.localEulerAngles = _defaultAngle;
			InitModes();
			InitName();
			string roleName = ReRandomName();
			roleName = FixName(roleName);
			View.SetInputText(inp_inputName, roleName);
			RadioButton.ChooseBtn(btn_male);
			ChangeSex(Sex.Male);
		}

		private void InitName()
		{
			nameindex = Singleton<MultiLanguageMgr>.Ins.GetNameIndex(out nameLength);
		}

		private void OnSLoginFinish(SLoginFinished s)
		{
		}

		protected override void onHide(string childView = null)
		{
			_heroSkinMale.Clear();
			_heroSkinFemale.Clear();
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Remove(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinish));
			ViewMgr.Ins.Destroy<EstablishPanel>();
		}

		protected override void onDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_heroRender, true);
		}

		private void OnClickRandomName(GameObject go)
		{
			SingletonMono<AudioManager>.Ins.PlayButtonClick();
			string roleName = ReRandomName();
			roleName = FixName(roleName);
			View.SetInputText(inp_inputName, roleName);
		}

		public string FixName(string roleName)
		{
			if (roleName.Length > 6)
			{
				roleName = roleName.Substring(0, roleName.Length - 1);
				return FixName(roleName);
			}
			return roleName;
		}

		public static string ReRandomName()
		{
			if (Singleton<PlatformMgr>.Ins.IsOverSea())
			{
				int key = Utils.Random(0, nameLength);
				return MultiLanguageNameCfg.Get(key).languageNames[nameindex];
			}
			bool flag = Utils.Random(1, 3) > 1;
			string text = string.Empty;
			if (flag)
			{
				Name1Cfg name1Cfg = Name1Cfg.Get(Utils.Random(1, Name1Cfg.GetAll().Count));
				if (name1Cfg != null)
				{
					text += name1Cfg.name;
				}
				Name2Cfg name2Cfg = Name2Cfg.Get(Utils.Random(1, Name2Cfg.GetAll().Count));
				if (name2Cfg != null)
				{
					text += name2Cfg.name;
				}
			}
			Name3Cfg name3Cfg = Name3Cfg.Get(Utils.Random(1, Name3Cfg.GetAll().Count));
			if (name3Cfg != null)
			{
				text += name3Cfg.name;
			}
			if (!flag)
			{
				Name4Cfg name4Cfg = Name4Cfg.Get(Utils.Random(1, Name4Cfg.GetAll().Count));
				if (name4Cfg != null)
				{
					text += name4Cfg.name;
				}
				Name5Cfg name5Cfg = Name5Cfg.Get(Utils.Random(1, Name5Cfg.GetAll().Count));
				if (name5Cfg != null)
				{
					text += name5Cfg.name;
				}
			}
			return text;
		}

		private void InitModes()
		{
			if (_establishRolelist != null && _establishRolelist.Count != 0)
			{
				InitSkinsMale();
				InitSkinsFemale();
				ShowModeMale();
				ShowModeFemale();
			}
		}

		public void ShowHero()
		{
			_heroRender = new RenderTexture(716, 716, 1);
			_heroRender.antiAliasing = 4;
			_heroRender.depth = 24;
			_heroRender.anisoLevel = 16;
			m_hero_tex.GetComponent<RawImage>().texture = _heroRender;
			_camera = m_camera.GetComponent<Camera>();
			_camera.targetTexture = _heroRender;
		}

		public void InitSkinsMale()
		{
			_nowSkinsMale.Clear();
			EstablishRole combatRole = EstablishRole.Get(1);
			InitSkins(_nowSkinsMale, combatRole);
		}

		public void ShowModeMale()
		{
			_heroSkinMale = new HeroSkin(m_role_model_male_root, true);
			_heroSkinMale.LoadBasicHero(_nowSkinsMale, _nowSkinsMale, "MainPanel.kongshou_zhan_idle01");
		}

		public void InitSkinsFemale()
		{
			_nowSkinsFemale.Clear();
			EstablishRole combatRole = EstablishRole.Get(2);
			InitSkins(_nowSkinsFemale, combatRole);
		}

		public void ShowModeFemale()
		{
			_heroSkinFemale = new HeroSkin(m_role_model_female_root, Singleton<RoleMgr>.Ins.info.sex);
			_heroSkinFemale.LoadBasicHero(_nowSkinsFemale, _nowSkinsFemale, "MainPanel.womankongshou_zhan_idle01");
		}

		private void InitSkins(List<int> nowSkins, EstablishRole combatRole)
		{
			nowSkins.Clear();
			if (combatRole.head.Count > 0)
			{
				nowSkins.Add(combatRole.head[0]);
			}
			if (combatRole.Clothes.Count > 0)
			{
				nowSkins.Add(combatRole.Clothes[0]);
			}
			if (combatRole.Pants.Count > 0)
			{
				nowSkins.Add(combatRole.Pants[0]);
			}
			if (combatRole.Shoes.Count > 0)
			{
				nowSkins.Add(combatRole.Shoes[0]);
			}
			if (combatRole.Glove.Count > 0)
			{
				nowSkins.Add(combatRole.Glove[0]);
			}
			if (combatRole.face.Count > 0)
			{
				nowSkins.Add(combatRole.face[0]);
			}
		}

		private void ChangeSex(Sex sex)
		{
			if (_currentSex != sex)
			{
				_currentSex = sex;
				m_role_model_male_root.SetActive(_currentSex == Sex.Male);
				m_role_model_female_root.SetActive(_currentSex == Sex.Female);
				if (_currentSex == Sex.Male && _heroSkinMale != null)
				{
					m_role_model_male_root.transform.localEulerAngles = _defaultAngle;
					_heroSkinMale.PlayRoleAni("MainPanel.kongshou_zhan_idle01", string.Empty);
				}
				if (_currentSex == Sex.Female && _heroSkinFemale != null)
				{
					m_role_model_female_root.transform.localEulerAngles = _defaultAngle;
					_heroSkinFemale.PlayRoleAni("MainPanel.womankongshou_zhan_idle01", string.Empty);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_hero_tex = component.GameObjects[0].gameObject;
			inp_inputName = component.GameObjects[1].gameObject;
			btn_name = component.GameObjects[2].gameObject;
			btn_creat = component.GameObjects[3].gameObject;
			m_role_model = component.GameObjects[4].gameObject;
			m_camera = component.GameObjects[5].gameObject;
			btn_female = component.GameObjects[6].gameObject;
			txt_on = component.GameObjects[7].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[8].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_male = component.GameObjects[9].gameObject;
			m_role_model_male_root = component.GameObjects[10].gameObject;
			m_role_model_female_root = component.GameObjects[11].gameObject;
			txt_on_0 = component.GameObjects[12].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[13].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			ChangeSex(Sex.Male);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			ChangeSex(Sex.Female);
		}

		[CompilerGenerated]
		private void _003COnClickOk_003Em__2()
		{
			if (!CheckName(View.GetInputText(inp_inputName)))
			{
				return;
			}
			CCreateRole cCreateRole = new CCreateRole();
			string inputText = View.GetInputText(inp_inputName);
			if (inputText.Length > 6)
			{
				AlertBox.Show(256);
				return;
			}
			if (inputText.Length < 2)
			{
				AlertBox.Show(255);
				return;
			}
			cCreateRole.name = inputText;
			if (_currentSex == Sex.Male)
			{
				EstablishRole establishRole = EstablishRole.Get(1);
				_nowSkinsMale.AddRange(establishRole.basicEquip);
				cCreateRole.roleSkins = new HashSet<int>(_nowSkinsMale);
				cCreateRole.modelSex = true;
			}
			else
			{
				EstablishRole establishRole2 = EstablishRole.Get(2);
				_nowSkinsFemale.AddRange(establishRole2.basicEquip);
				cCreateRole.roleSkins = new HashSet<int>(_nowSkinsFemale);
				cCreateRole.modelSex = false;
			}
			cCreateRole.version = LoginMgr.ReadyVeision();
			cCreateRole.phoneID = SystemInfo.deviceUniqueIdentifier;
			cCreateRole.systemType = SystemInfo.operatingSystem;
			Client2Gs.Ins.Send(cCreateRole);
		}
	}
}
