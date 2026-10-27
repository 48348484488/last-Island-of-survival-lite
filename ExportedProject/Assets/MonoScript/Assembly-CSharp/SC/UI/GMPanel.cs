using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using SC.LargeScene;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.gm.scmsg;
using gs.online.scmsg;

namespace SC.UI
{
	public class GMPanel : View
	{
		private readonly Dictionary<string, MethodInfo> _methodInfosdic = new Dictionary<string, MethodInfo>();

		private Type _type;

		private readonly List<GameObject> _addBtns = new List<GameObject>();

		private readonly List<Vector3> _addBtnsSartPos = new List<Vector3>();

		private bool _isshow = true;

		private Utils.VoidDelegate _durationDelegate;

		private float _duration;

		private bool isShowShowPhone;

		private StringBuilder info;

		private GameObject inp_gm;

		private GameObject btn_return;

		private GameObject btn_confirm;

		private GameObject m_phoneinfo_root;

		private GameObject txt_phoneinfo;

		private Text txt_phoneinfoText;

		private GameObject btn_drawall;

		private GameObject btn_material;

		private GameObject btn_allweapon;

		private GameObject btn_allequip;

		private GameObject btn_kill_self;

		private GameObject btn_all_part;

		private GameObject btn_all_bullet;

		private GameObject btn_hideallui;

		private GameObject btn_hideui;

		private GameObject btn_showallui;

		private GameObject btn_addOneHour;

		private long boxid
		{
			get
			{
				return long.Parse(PlayerPrefs.GetString("boxid"));
			}
			set
			{
				PlayerPrefs.SetString("boxid", value.ToString());
			}
		}

		protected override void onInit()
		{
			UIEventListener.Get(btn_confirm, string.Empty).onClick = OnSubmit;
			UIEventListener.Get(btn_kill_self, string.Empty).onClick = _003ConInit_003Em__0;
			UIEventListener.Get(btn_return, string.Empty).onClick = _003ConInit_003Em__1;
			InitMethod();
			for (int i = 0; i < _addBtns.Count; i++)
			{
				int index = i;
				_addBtns[index].SetActive(false);
			}
			UIEventListener.Get(btn_drawall, string.Empty).onClick = _003ConInit_003Em__2;
			UIEventListener.Get(btn_material, string.Empty).onClick = _003ConInit_003Em__3;
			UIEventListener.Get(btn_allweapon, string.Empty).onClick = _003ConInit_003Em__4;
			UIEventListener.Get(btn_allequip, string.Empty).onClick = _003ConInit_003Em__5;
			UIEventListener.Get(btn_all_bullet, string.Empty).onClick = _003ConInit_003Em__6;
			UIEventListener.Get(btn_all_part, string.Empty).onClick = _003ConInit_003Em__7;
			ClickListener.Get(btn_hideallui, string.Empty).onClick = _003ConInit_003Em__8;
			ClickListener.Get(btn_hideui, string.Empty).onClick = _003ConInit_003Em__9;
			ClickListener.Get(btn_showallui, string.Empty).onClick = _003ConInit_003Em__A;
			ClickListener.Get(btn_addOneHour, string.Empty).onClick = _003ConInit_003Em__B;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			isShowShowPhone = false;
			m_phoneinfo_root.SetActive(false);
		}

		private void ChangeShadow()
		{
			Singleton<RechargeMgr>.Ins.realSencondFromServer += 150;
			SingletonMono<DayNightSystem>.Ins.PlayCurAim();
		}

		private void ShowAlert(SAlert msg)
		{
			if (msg.strId > 0)
			{
				AlertBox.Show(msg.strId);
			}
		}

		protected override void onHide(string childView = null)
		{
			SAlert.handler = (SAlert.Handler)Delegate.Remove(SAlert.handler, new SAlert.Handler(ShowAlert));
		}

		private void OnSubmit(GameObject go)
		{
			string inputText = View.GetInputText(inp_gm);
			if (inputText.Length > 0 && !clientCmd(inputText))
			{
				sendCGMCommand(inputText);
			}
		}

		private void sendCGMCommand(string cmd)
		{
			string cmd2 = cmd.Trim();
			CGMCommand cGMCommand = new CGMCommand();
			cGMCommand.cmd = cmd2;
			Client2Gs.Ins.Send(cGMCommand);
		}

		private void InitMethod()
		{
			_type = GetType();
			MethodInfo[] methods = _type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if (!_methodInfosdic.ContainsKey(methodInfo.Name))
				{
					_methodInfosdic.Add(methodInfo.Name, methodInfo);
				}
			}
		}

		private bool clientCmd(string cmd)
		{
			string[] array = cmd.Split(' ');
			if (array.Length == 0)
			{
				return true;
			}
			MethodInfo value;
			if (_methodInfosdic.TryGetValue(array[0], out value))
			{
				if (array.Length < 2)
				{
					value.Invoke(this, null);
				}
				else
				{
					ParameterInfo[] parameters = value.GetParameters();
					object[] array2 = new object[array.Length - 1];
					for (int i = 0; i < array2.Length; i++)
					{
						Type parameterType = parameters[i].ParameterType;
						array2[i] = Convert.ChangeType(array[i + 1], parameterType);
					}
					value.Invoke(this, array2);
				}
				return true;
			}
			return false;
		}

		public void ai(int level)
		{
		}

		public void openbox()
		{
			Debug.LogError("boxid:" + boxid);
			Singleton<BoxMgr>.Ins.OpenBox(boxid);
		}

		public void showallui()
		{
			BattlePanel view = ViewMgr.Ins.GetView<BattlePanel>();
			view.ShowAllUi();
		}

		public void hideallui()
		{
			BattlePanel view = ViewMgr.Ins.GetView<BattlePanel>();
			view.HideAllUi();
		}

		public void hideui()
		{
			BattlePanel view = ViewMgr.Ins.GetView<BattlePanel>();
			view.HideUi();
		}

		public void testalert()
		{
			AlertBox.Show("111");
			AlertBox.Show("222");
			AlertBox.Show("333");
			AlertBox.Show("444");
			AlertBox.Show("555");
		}

		public void clearcache()
		{
			PlayerPrefs.DeleteAll();
		}

		public void exchangepanel()
		{
			ViewMgr.Ins.ShowView<ExchangePanel>(null, false);
		}

		public void additemfromto(int itemId1, int itemId2, int num)
		{
			int num2 = ((itemId1 >= itemId2) ? itemId2 : itemId1);
			int num3 = ((itemId1 <= itemId2) ? itemId2 : itemId1);
			for (int i = num2; i < num3 + 1; i++)
			{
				if (ItemCfg.Get(i) != null)
				{
					sendCGMCommand("additem " + i + " " + num);
				}
			}
		}

		public void topos(float x, float y, float z)
		{
			LargeSceneManager.Ins.ToSceneCell(x, y, z);
		}

		public void showdie()
		{
			ViewMgr.Ins.ShowView<DeathPanel>(null, false);
		}

		public void showinfo()
		{
			Singleton<RoleMgr>.Ins.GetRoleMoreInformation(Singleton<RoleMgr>.Ins.info.roleId);
		}

		private void OnShowPhoneInfo(GameObject go)
		{
			isShowShowPhone = !isShowShowPhone;
			if (isShowShowPhone)
			{
				info = new StringBuilder();
				m_phoneinfo_root.SetActive(true);
				info.AppendLine("设备与系统信息:");
				GetMessage("设备模型", SystemInfo.deviceModel);
				GetMessage("处理器名称", SystemInfo.processorType.ToString());
				GetMessage("处理核数", SystemInfo.processorCount.ToString());
				GetMessage("系统内存大小MB", SystemInfo.systemMemorySize.ToString());
				GetMessage("操作系统", SystemInfo.operatingSystem);
				GetMessage("显卡名称", SystemInfo.graphicsDeviceName);
				GetMessage("显卡类型", SystemInfo.graphicsDeviceType.ToString());
				GetMessage("显卡供应商", SystemInfo.graphicsDeviceVendor);
				GetMessage("显卡版本号", SystemInfo.graphicsDeviceVersion);
				GetMessage("显存大小MB", SystemInfo.graphicsMemorySize.ToString());
				GetMessage("显卡是否支持多线程渲染", SystemInfo.graphicsMultiThreaded.ToString());
				GetMessage("支持的渲染目标数量", SystemInfo.supportedRenderTargetCount.ToString());
				GetMessage("手机配置等级", Utils.PhoneLevel.ToString());
				GetMessage("Dpi", Screen.dpi.ToString());
				GetMessage("CurrentResolution", Screen.currentResolution.ToString());
				GetMessage("ScreenWidthAndHeight", Screen.width + "×" + Screen.height);
				View.SetLabelText(txt_phoneinfo, info);
			}
			else
			{
				m_phoneinfo_root.SetActive(false);
			}
		}

		public void opencamerafree()
		{
			GameObject gameObject = GameObject.Find("cameraTarget");
			if (!gameObject)
			{
				gameObject = new GameObject("cameraTarget");
			}
			CameraMoveFree cameraMoveFree = gameObject.GetComponent<CameraMoveFree>();
			if (!cameraMoveFree)
			{
				cameraMoveFree = gameObject.AddComponent<CameraMoveFree>();
			}
			cameraMoveFree.Open();
		}

		public void closecamerafree()
		{
			GameObject gameObject = GameObject.Find("cameraTarget");
			if ((bool)gameObject)
			{
				CameraMoveFree component = gameObject.GetComponent<CameraMoveFree>();
				if ((bool)component)
				{
					component.Close();
				}
			}
		}

		private void GetMessage(params string[] str)
		{
			if (str.Length == 2)
			{
				info.AppendLine(str[0] + ":" + str[1]);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			inp_gm = component.GameObjects[0].gameObject;
			btn_return = component.GameObjects[1].gameObject;
			btn_confirm = component.GameObjects[2].gameObject;
			m_phoneinfo_root = component.GameObjects[3].gameObject;
			txt_phoneinfo = component.GameObjects[4].gameObject;
			txt_phoneinfoText = txt_phoneinfo.GetComponent<Text>();
			btn_drawall = component.GameObjects[5].gameObject;
			btn_material = component.GameObjects[6].gameObject;
			btn_allweapon = component.GameObjects[7].gameObject;
			btn_allequip = component.GameObjects[8].gameObject;
			btn_kill_self = component.GameObjects[9].gameObject;
			btn_all_part = component.GameObjects[10].gameObject;
			btn_all_bullet = component.GameObjects[11].gameObject;
			btn_hideallui = component.GameObjects[12].gameObject;
			btn_hideui = component.GameObjects[13].gameObject;
			btn_showallui = component.GameObjects[14].gameObject;
			btn_addOneHour = component.GameObjects[15].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject o)
		{
			string text = "addhp -100000";
			if (text.Length > 0 && !clientCmd(text))
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject o)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			string text = "oneKeyLearnDrawings";
			if (text.Length > 0 && !clientCmd(text))
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			string text = "addAllByType " + 107 + " 200";
			if (text.Length > 0)
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			string text = "addAllByType " + 13 + " 1";
			if (text.Length > 0)
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			string text = "addAllByType " + 119 + " 1";
			if (text.Length > 0)
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			string text = "addAllByType " + 24 + " 999";
			if (text.Length > 0)
			{
				sendCGMCommand(text);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			int[] partTypes = Singleton<BagMgr>.Ins.PartTypes;
			foreach (int num in partTypes)
			{
				string text = "addAllByType " + num + " 1";
				if (text.Length > 0)
				{
					sendCGMCommand(text);
				}
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			hideallui();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__9(GameObject go)
		{
			hideui();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__A(GameObject go)
		{
			showallui();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__B(GameObject go)
		{
			ChangeShadow();
		}
	}
}
