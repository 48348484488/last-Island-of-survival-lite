using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SC.UI;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.scmsg;
using gs.drop.scmsg;
using gs.ladder.scmsg;
using gs.role.scmsg;
using gs.shop.scmsg;

public class RoleMgr : Singleton<RoleMgr>
{
	public class MainPanelShowViewsObj
	{
		public Type ToShowView;

		public object ShowParam;

		public bool IsHideParent;
	}

	[CompilerGenerated]
	private sealed class _003CIsGoldEnough_003Ec__AnonStorey0
	{
		internal Action callBack;

		internal void _003C_003Em__0()
		{
			callBack();
		}
	}

	[CompilerGenerated]
	private sealed class _003CIsCouponEnough_003Ec__AnonStorey1
	{
		internal Action callBack;

		internal void _003C_003Em__0()
		{
			callBack();
		}
	}

	public BasicRoleInfo info = new BasicRoleInfo();

	public int Gold;

	public int Diamond;

	public int Coupons;

	public int VipExp;

	public int vipBestLevel;

	public int VipExpPool;

	public int VipDueTime;

	public int Country = 1;

	public int Province = 1;

	public int Zone;

	public bool RoleModelSex;

	public int Exp;

	private List<int> _equipProps = new List<int>();

	public HashSet<int> RoleSkins = new HashSet<int>();

	private HashSet<int> _roleBasicSkins = new HashSet<int>();

	private HashSet<int> _roleBasicEquips = new HashSet<int>();

	public bool IsGm;

	public int Score;

	public bool IsTop500;

	public int createRoleTime;

	private int _bloodMax;

	private int _blood;

	private int _hunger;

	private int _water;

	public Dictionary<int, int> roleSkills = new Dictionary<int, int>();

	public int Grade;

	private GameObject fps;

	private bool isLoadGps;

	private bool isLoginFinish;

	private CGetRoleMoreInformation _cGetRoleMoreInformation = new CGetRoleMoreInformation();

	private CChangeName _cChangeName = new CChangeName();

	private CSetFrameId _cSetFrameId = new CSetFrameId();

	private CSetHeadId _cSetHeadId = new CSetHeadId();

	private bool hadShowHunger;

	private CBuyGold _cBuyGold = new CBuyGold();

	private Dictionary<string, int> str2int;

	private int flagIndex = 1;

	private List<MainPanelShowViewsObj> _mainPanelShowViews = new List<MainPanelShowViewsObj>();

	private int _showedIndex;

	private bool _alreadyAddEvent;

	private bool _isQualifyingUpPanelAddHideEvent;

	private bool _isAchievementGetPanelAddHideEvent;

	private bool _isGainUsePanelAddHideEvent;

	public List<int> EquipProps
	{
		get
		{
			return _equipProps;
		}
	}

	public int HeadDefense
	{
		get
		{
			if (_equipProps.Count > 0)
			{
				return _equipProps[0];
			}
			return 0;
		}
	}

	public int BodyDefense
	{
		get
		{
			if (_equipProps.Count > 1)
			{
				return _equipProps[1];
			}
			return 0;
		}
	}

	public HashSet<int> RoleBasicSkins
	{
		get
		{
			if (_roleBasicSkins.Count <= 0)
			{
				foreach (int roleSkin in RoleSkins)
				{
					ItemCfg itemCfg = ItemCfg.Get(roleSkin);
					if (itemCfg != null && itemCfg.type == 9)
					{
						_roleBasicSkins.Add(roleSkin);
					}
				}
			}
			return _roleBasicSkins;
		}
	}

	public HashSet<int> RoleBasicEquips
	{
		get
		{
			if (_roleBasicEquips.Count <= 0)
			{
				foreach (int roleSkin in RoleSkins)
				{
					ItemCfg itemCfg = ItemCfg.Get(roleSkin);
					if (itemCfg != null && itemCfg.type == 119)
					{
						_roleBasicEquips.Add(roleSkin);
					}
				}
			}
			return _roleBasicEquips;
		}
	}

	public int BloodMax
	{
		get
		{
			if (_bloodMax <= 0)
			{
				_bloodMax = ConstsBs.HpPlayer;
			}
			return _bloodMax;
		}
	}

	public int Blood
	{
		get
		{
			if (_blood < 0)
			{
				return 0;
			}
			return _blood;
		}
	}

	public int Hunger
	{
		get
		{
			if (_hunger < 0)
			{
				return 0;
			}
			return _hunger;
		}
	}

	public int Water
	{
		get
		{
			if (_water < 0)
			{
				return 0;
			}
			return _water;
		}
	}

	public bool IsSpecialAccount { get; private set; }

	public string RoleAnimStr
	{
		get
		{
			return (!info.sex) ? "MainPanel.womankongshou_zhan_idle01" : "MainPanel.kongshou_zhan_idle01";
		}
	}

	public static bool IsRoleIdEven
	{
		get
		{
			if (Singleton<RoleMgr>.Ins.info != null)
			{
				return (Singleton<RoleMgr>.Ins.info.roleId >> 12) % 2 == 0;
			}
			return false;
		}
	}

	public string SpecialChar
	{
		get
		{
			return " `~!@#$%^&*()+=|{}':;',[].<>/?~！@#￥%&*（）——+|{}【】‘；：”“’。，、？\"‥…▪•";
		}
	}

	public RoleMgr()
	{
		IsSpecialAccount = false;
	}

	public void Init()
	{
		SRoleInfo.handler = (SRoleInfo.Handler)Delegate.Combine(SRoleInfo.handler, new SRoleInfo.Handler(SRoleInfoHandle));
		SRemoveRoleSkin.handler = (SRemoveRoleSkin.Handler)Delegate.Combine(SRemoveRoleSkin.handler, new SRemoveRoleSkin.Handler(SRemoveRoleSkinHandle));
		SIsGmChange.handler = (SIsGmChange.Handler)Delegate.Combine(SIsGmChange.handler, new SIsGmChange.Handler(OnIsGm));
		SChangeRoleSex.handler = (SChangeRoleSex.Handler)Delegate.Combine(SChangeRoleSex.handler, new SChangeRoleSex.Handler(OnSChangeRoleSex));
		SSetFrameId.handler = (SSetFrameId.Handler)Delegate.Combine(SSetFrameId.handler, new SSetFrameId.Handler(OnSSetFrameId));
		SSetHeadId.handler = (SSetHeadId.Handler)Delegate.Combine(SSetHeadId.handler, new SSetHeadId.Handler(OnSSetHeadId));
		SLadderExpChange.handler = (SLadderExpChange.Handler)Delegate.Combine(SLadderExpChange.handler, new SLadderExpChange.Handler(SLadderExpChangeHandle));
		SCouponChange.handler = (SCouponChange.Handler)Delegate.Combine(SCouponChange.handler, new SCouponChange.Handler(SCouponChangeHandle));
		SGoldChange.handler = (SGoldChange.Handler)Delegate.Combine(SGoldChange.handler, new SGoldChange.Handler(SGoldChangeHandle));
		SHunger.handler = (SHunger.Handler)Delegate.Combine(SHunger.handler, new SHunger.Handler(SHungerHandle));
		SStrength.handler = (SStrength.Handler)Delegate.Combine(SStrength.handler, new SStrength.Handler(SStrengthHandle));
		SHpChange.handler = (SHpChange.Handler)Delegate.Combine(SHpChange.handler, new SHpChange.Handler(SHpChangeHandle));
		SGetRoleMoreInformation.handler = (SGetRoleMoreInformation.Handler)Delegate.Combine(SGetRoleMoreInformation.handler, new SGetRoleMoreInformation.Handler(SGetRoleMoreInformationHandle));
		SChangeName.handler = (SChangeName.Handler)Delegate.Combine(SChangeName.handler, new SChangeName.Handler(SChangeNameHandle));
		SPropsChange.handler = (SPropsChange.Handler)Delegate.Combine(SPropsChange.handler, new SPropsChange.Handler(SPropsChangeHandle));
		SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Combine(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(SBattleLoginFinishHandle));
		SSetHeadId.handler = (SSetHeadId.Handler)Delegate.Combine(SSetHeadId.handler, new SSetHeadId.Handler(SSetHeadIdHandle));
		SSetFrameId.handler = (SSetFrameId.Handler)Delegate.Combine(SSetFrameId.handler, new SSetFrameId.Handler(SSetFrameIdHandle));
		SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
		SBuyGold.handler = (SBuyGold.Handler)Delegate.Combine(SBuyGold.handler, new SBuyGold.Handler(SBuyDiamondHandle));
		SLadderInfo.handler = (SLadderInfo.Handler)Delegate.Combine(SLadderInfo.handler, new SLadderInfo.Handler(SLadderInfoHandle));
		fps = GameObject.Find("FPS");
		if ((bool)fps)
		{
			fps.GetComponent<Text>().enabled = false;
		}
	}

	private void SLadderInfoHandle(SLadderInfo msg)
	{
		Exp = msg.ladderInfo.exp;
	}

	private void SLadderExpChangeHandle(SLadderExpChange msg)
	{
		int level = info.level;
		bool flag = level != msg.level;
		info.level = (short)msg.level;
		Exp = msg.exp;
		if (flag)
		{
			Singleton<PlatformMgr>.Ins.OnLoginRole(false);
			Utils.TriggerEvent(RoleEvent.LevelChangeDelegate, level, msg.level);
		}
	}

	private void SBuyDiamondHandle(SBuyGold msg)
	{
		DropDetail dropDetail = new DropDetail();
		dropDetail.dropProp.Add(0, msg.cuponNum * cfg.Consts.CUPON_TO_DIAMOND);
		ViewMgr.Ins.ShowView<GainPanel>(dropDetail, false);
	}

	private void SPropsChangeHandle(SPropsChange msg)
	{
	}

	private void SRebirthHandle(SRebirth msg)
	{
		if (msg.playerInfo.roleId == info.roleId)
		{
			_bloodMax = msg.playerInfo.hpMax;
			_blood = msg.playerInfo.hp;
			Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
		}
	}

	private void SSetFrameIdHandle(SSetFrameId msg)
	{
		info.frameId = (short)msg.frameId;
		Utils.TriggerEvent(RoleEvent.RefreshFrameDelegate);
	}

	private void SSetHeadIdHandle(SSetHeadId msg)
	{
		info.headId = (short)msg.headId;
		Utils.TriggerEvent(RoleEvent.RefreshHeadDelegate);
	}

	private void SChangeNameHandle(SChangeName msg)
	{
		switch (msg.result)
		{
		case 1:
			AlertBox.Show(254);
			break;
		case 2:
			AlertBox.Show(255);
			break;
		case 3:
			AlertBox.Show(256);
			break;
		case 8:
			AlertBox.Show(273);
			break;
		case 0:
			info.name = msg.name;
			AlertBox.Show(274);
			break;
		}
		Utils.TriggerEvent(RoleEvent.IsSucessChangeNameDelegate, msg.result == 0);
	}

	private void SGetRoleMoreInformationHandle(SGetRoleMoreInformation msg)
	{
		if (ViewMgr.Ins.IsShow<ChatPanel>())
		{
			ViewMgr.Ins.HideView<ChatPanel>();
		}
		ViewMgr.Ins.ShowView<InforPanel>(msg);
	}

	public void GetRoleMoreInformation(long roleId)
	{
		_cGetRoleMoreInformation.targetRoleId = roleId;
		Client2Gs.Ins.Send(_cGetRoleMoreInformation);
	}

	public void ChangeName(string roleName)
	{
		_cChangeName.name = roleName;
		Client2Gs.Ins.Send(_cChangeName);
	}

	public void SetFrameId(int frameId)
	{
		_cSetFrameId.frameId = frameId;
		Client2Gs.Ins.Send(_cSetFrameId);
	}

	public void SetHeadId(int headId)
	{
		_cSetHeadId.headId = headId;
		Client2Gs.Ins.Send(_cSetHeadId);
	}

	private void SBattleLoginFinishHandle(SBattleLoginFinish msg)
	{
		_bloodMax = msg.playerInfo.hpMax;
		_blood = msg.playerInfo.hp;
		if (_blood < 0)
		{
			AlertBox.Show(365);
		}
	}

	private void SHpChangeHandle(SHpChange msg)
	{
		if (msg.roleId == info.roleId)
		{
			if (_blood > msg.hp && _hunger == 0 && !hadShowHunger)
			{
				AlertBox.Show(383);
				hadShowHunger = true;
			}
			_bloodMax = msg.hpMax;
			if (_blood > 0 && msg.hp <= 0)
			{
				AlertBox.Show(365);
			}
			_blood = msg.hp;
			Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
		}
	}

	private void SStrengthHandle(SStrength msg)
	{
		_water = msg.strength;
		Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
	}

	private void SHungerHandle(SHunger msg)
	{
		_hunger = msg.hunger;
		if (_hunger > 0)
		{
			hadShowHunger = false;
		}
		Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
	}

	private void SCouponChangeHandle(SCouponChange msg)
	{
		Coupons = msg.coupon;
		Utils.TriggerEvent(RoleEvent.TokenMoneyChangeDelegate);
	}

	private void SGoldChangeHandle(SGoldChange msg)
	{
		Gold = msg.gold;
		Utils.TriggerEvent(RoleEvent.TokenMoneyChangeDelegate);
	}

	private void OnSSetHeadId(SSetHeadId msg)
	{
		info.headId = (short)msg.headId;
		if (info.headId < 0)
		{
			info.headId = 1;
		}
		AlertBox.Show(274);
	}

	private void OnSSetFrameId(SSetFrameId msg)
	{
		info.frameId = msg.frameId;
		AlertBox.Show(274);
	}

	private void OnSChangeRoleSex(SChangeRoleSex msg)
	{
		info.sex = msg.sex;
	}

	private void OnIsGm(SIsGmChange msg)
	{
		IsGm = msg.isGM;
		fps.GetComponent<Text>().enabled = IsGm;
	}

	private void SRemoveRoleSkinHandle(SRemoveRoleSkin msg)
	{
		RoleSkins.Remove(msg.itemId);
	}

	public void BuyDiamondHandle(int num)
	{
		if (Coupons - num >= 0)
		{
			_cBuyGold.cuponNum = num;
			Client2Gs.Ins.Send(_cBuyGold);
		}
	}

	public void SetRoleSkins(HashSet<int> skins)
	{
		RoleSkins.Clear();
		foreach (int skin in skins)
		{
			RoleSkins.Add(skin);
		}
	}

	private void SRoleInfoHandle(SRoleInfo msg)
	{
		info = msg.basicInfo;
		if (info.roleId == -1)
		{
			Debug.LogError("info.roleId is -1");
		}
		Gold = msg.gold;
		Coupons = msg.coupon;
		if (Zone == 0)
		{
			Zone = 1;
			RoleModelSex = msg.modelSex;
			RoleSkins = msg.skins;
			createRoleTime = msg.createTime;
		}
	}

	public string getIDKey(long roleId)
	{
		if (roleId <= 0)
		{
			return "ERROR";
		}
		return (roleId ^ 0x55555555).ToString();
	}

	public static bool IsInt(string value)
	{
		return Regex.IsMatch(value, "^\\d*$");
	}

	public long getRoleIdByKey(string IDKey)
	{
		if (!IsInt(IDKey))
		{
			return 0L;
		}
		return Convert.ToInt64(IDKey) ^ 0x55555555;
	}

	public bool IsGoldEnough(int costGold, Action callBack)
	{
		_003CIsGoldEnough_003Ec__AnonStorey0 _003CIsGoldEnough_003Ec__AnonStorey = new _003CIsGoldEnough_003Ec__AnonStorey0();
		_003CIsGoldEnough_003Ec__AnonStorey.callBack = callBack;
		if (Gold < costGold)
		{
			MessageBoxPanel.Show(155, _003CIsGoldEnough_003Ec__AnonStorey._003C_003Em__0);
			return false;
		}
		return true;
	}

	public bool IsDiamondEnough(int costDiamond)
	{
		if (Diamond < costDiamond)
		{
			AlertBox.Show(156);
			return false;
		}
		return true;
	}

	public bool IsMoneyEnough(int cost, int moneyType, Action goldCallBack, Action couponCallBack)
	{
		switch (moneyType)
		{
		case 2:
			return IsCouponEnough(cost, couponCallBack);
		case 1:
			return IsGoldEnough(cost, goldCallBack);
		default:
			return false;
		}
	}

	public void HungerChange(int hungerValue)
	{
		_hunger += hungerValue;
		if (_hunger < 0)
		{
			_hunger = 0;
		}
		else if (_hunger > ConstsBs.MAX_HUNGER)
		{
			_hunger = ConstsBs.MAX_HUNGER;
		}
		Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
	}

	public bool IsCouponEnough(int costCoupon, Action callBack)
	{
		_003CIsCouponEnough_003Ec__AnonStorey1 _003CIsCouponEnough_003Ec__AnonStorey = new _003CIsCouponEnough_003Ec__AnonStorey1();
		_003CIsCouponEnough_003Ec__AnonStorey.callBack = callBack;
		if (Coupons < costCoupon)
		{
			MessageBoxPanel.Show(157, _003CIsCouponEnough_003Ec__AnonStorey._003C_003Em__0);
			return false;
		}
		return true;
	}

	public void RemoveOnMainPanelShowView(Type viewName)
	{
		int i = 0;
		for (int count = _mainPanelShowViews.Count; i < count; i++)
		{
			if (_mainPanelShowViews[i].ToShowView == viewName)
			{
				_mainPanelShowViews.RemoveAt(i);
				break;
			}
		}
	}

	private void ShowNext()
	{
		if (_mainPanelShowViews.Count > _showedIndex)
		{
			ViewMgr.Ins.ShowTopView(_mainPanelShowViews[_showedIndex].ToShowView, _mainPanelShowViews[_showedIndex].ShowParam);
			_showedIndex++;
		}
		else
		{
			ClearShowViewListCache();
		}
	}

	public void ClearShowViewListCache()
	{
		try
		{
			_isQualifyingUpPanelAddHideEvent = false;
			_isAchievementGetPanelAddHideEvent = false;
			ViewMgr.Ins.RemoveOnHideEvent("AchievementGetPanel", ShowNext);
			ViewMgr.Ins.RemoveOnHideEvent("QualifyingUpPanel", ShowNext);
			ViewMgr.Ins.RemoveOnHideEvent("GainUsePanel", ShowNext);
			ViewMgr.Ins.Destroy("AchievementGetPanel");
			ViewMgr.Ins.Destroy("QualifyingUpPanel");
			ViewMgr.Ins.Destroy("GainUsePanel");
			_mainPanelShowViews.Clear();
			_showedIndex = 0;
			ViewMgr.Ins.RemoveOnShowEvent("MainPanel", ShowNext);
			_alreadyAddEvent = false;
		}
		catch (Exception)
		{
		}
	}

	public bool IsContainSpecialChar(string content)
	{
		return SpecialChar.Contains(content);
	}
}
