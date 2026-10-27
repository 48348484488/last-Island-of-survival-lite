using System;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using SC.UI;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;

public class TrusteeshipMgr : SingletonMono<TrusteeshipMgr>
{
	private Transform _selfPlayerTrans;

	public Transform SelfMainCameraTrans;

	public Transform SelfHeadTrans;

	private TrusteeshipAction _curAction;

	private TrusteeshipAction _nextAction;

	private vThirdPersonCamera _mainCamera;

	private PlayerController _selfController;

	private readonly Dictionary<TrusteeshipActionType, TrusteeshipAction> _dicType2Action = new Dictionary<TrusteeshipActionType, TrusteeshipAction>();

	private readonly Dictionary<long, bool> _dicCollectKey2Value = new Dictionary<long, bool>();

	private readonly Dictionary<int, bool> _dicPickKey2Value = new Dictionary<int, bool>();

	private readonly HashSet<StateID> _trusteeshipStates = new HashSet<StateID>();

	private bool _isInTrusteeship;

	public int TreeLayerMask;

	private bool _autoHeal;

	private bool _autoEat;

	private int _healValue;

	private int _eatValue;

	private bool _totalStop = true;

	public bool BagHaveCapacity;

	public long AimInsId;

	public int MedicineId = -1;

	public int FoodId = -1;

	private readonly CCanBuild _cCanBuild = new CCanBuild();

	private Coroutine _delayAction;

	public float IntervalTime = 0.1f;

	public bool UpdateEveryFrame;

	private float _passTime;

	public bool IsInTrusteeship
	{
		get
		{
			return _isInTrusteeship;
		}
		set
		{
			if (TrusteeshipEvent.TrusteeshipOnOffDelegate != null)
			{
				TrusteeshipEvent.TrusteeshipOnOffDelegate(value);
			}
			_isInTrusteeship = value;
		}
	}

	public Vector3 StartPos { get; private set; }

	public Vector3 SelfPos
	{
		get
		{
			return _selfPlayerTrans.position;
		}
	}

	protected override void Awake()
	{
		_trusteeshipStates.Add(StateID.Attack);
		_trusteeshipStates.Add(StateID.Swim);
		_trusteeshipStates.Add(StateID.CutPlant);
		_trusteeshipStates.Add(StateID.Stand);
		_trusteeshipStates.Add(StateID.Fall);
		_trusteeshipStates.Add(StateID.Nawuqi);
		_trusteeshipStates.Add(StateID.Huanzidan);
		_trusteeshipStates.Add(StateID.Zhuangtian);
		_trusteeshipStates.Add(StateID.Cross);
		_trusteeshipStates.Add(StateID.HoldNearWeaponState);
		_trusteeshipStates.Add(StateID.NullStateID);
		_trusteeshipStates.Add(StateID.UseItem);
		SCanBuild.handler = (SCanBuild.Handler)Delegate.Combine(SCanBuild.handler, new SCanBuild.Handler(OnSCanBuild));
	}

	protected void OnDestroy()
	{
		SCanBuild.handler = (SCanBuild.Handler)Delegate.Remove(SCanBuild.handler, new SCanBuild.Handler(OnSCanBuild));
	}

	private void OnSCanBuild(SCanBuild msg)
	{
		if ((bool)this)
		{
			if (msg.flag)
			{
				EnterTrusteeship();
				return;
			}
			AlertBox.Show(382);
			StopTrusteeship();
			IsInTrusteeship = false;
		}
	}

	public void OnCacheDataChange(float value)
	{
		RemoveAllCacheData();
		_healValue = SettingMgr.GetTrusteeshipHealValue();
		_eatValue = SettingMgr.GetTrusteeshipEatValue();
	}

	public void OnCacheDataChange(bool value)
	{
		RemoveAllCacheData();
		_autoHeal = SettingMgr.GetTrusteeshipHeal();
		_autoEat = SettingMgr.GetTrusteeshipEat();
		_healValue = SettingMgr.GetTrusteeshipHealValue();
		_eatValue = SettingMgr.GetTrusteeshipEatValue();
	}

	private void RemoveAllCacheData()
	{
		_dicCollectKey2Value.Clear();
		_dicPickKey2Value.Clear();
	}

	public bool GetSettingCollect(SettingMgr.TrusteeshipCollectType type, int selfCfgId)
	{
		long key = ((long)type << 32) | selfCfgId;
		bool value;
		if (!_dicCollectKey2Value.TryGetValue(key, out value))
		{
			value = SettingMgr.GetTrusteeshipCollectSetById(type, selfCfgId);
			_dicCollectKey2Value[key] = value;
		}
		return value;
	}

	public bool GetSettingPick(int itemType)
	{
		bool value;
		if (!_dicPickKey2Value.TryGetValue(itemType, out value))
		{
			value = SettingMgr.GetTrusteeshipPick(itemType);
			_dicPickKey2Value[itemType] = value;
		}
		return value;
	}

	public void PutWeaponToHand()
	{
		bool flag = false;
		BagItem allItemByInstanceId;
		if (_selfController.CurrentWeaponInsId > 0)
		{
			allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_selfController.CurrentWeaponInsId);
			if (allItemByInstanceId != null && allItemByInstanceId.duration > 0)
			{
				ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
				if (itemCfg != null && itemCfg.type == 82)
				{
					flag = true;
				}
			}
			if (flag)
			{
				return;
			}
		}
		allItemByInstanceId = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.QuickUseItems.Values, 82);
		if (allItemByInstanceId == null)
		{
			allItemByInstanceId = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.BagItems, 82);
		}
		if (allItemByInstanceId != null && allItemByInstanceId.duration > 0)
		{
			if (_selfController.CurrentWeaponInsId != allItemByInstanceId.instanceId && (Singleton<BagMgr>.Ins.QuickUseIsContainsInstanceId(allItemByInstanceId.instanceId) || Singleton<BagMgr>.Ins.QuickUseItems.Count < cfg.Consts.MAX_QUICKUSEITEM))
			{
				_selfController.PlayChangeWeapen(allItemByInstanceId.instanceId);
			}
		}
		else if (_selfController.CurrentWeaponInsId > 0)
		{
			_selfController.PlayChangeWeapen(-1);
		}
	}

	private bool CanStartTrusteeship()
	{
		if (IsInTrusteeship)
		{
			return false;
		}
		int num = -1;
		if (_selfController.InBuildState)
		{
			num = 372;
		}
		else if (_selfController.IsDownWaitSave)
		{
			num = 373;
		}
		else if (_selfController.IsDriver)
		{
			num = 374;
		}
		else if (_selfController.AutoRun)
		{
			num = 376;
		}
		else if (_selfController.Climbing)
		{
			num = 377;
		}
		if (num > 0)
		{
			AlertBox.Show(num);
			return false;
		}
		return true;
	}

	public void StartTrusteeship()
	{
		if (_selfPlayerTrans == null)
		{
			_selfController = Battle.Ins.SelfPlayer;
			_selfPlayerTrans = _selfController.transform;
			_mainCamera = Battle.Ins.MainCamera;
			SelfMainCameraTrans = _mainCamera.transform;
			SelfHeadTrans = _selfController.HeadTop;
			TreeLayerMask = (1 << LayerMask.NameToLayer("Tree")) | (1 << LayerMask.NameToLayer("Outline"));
			_dicType2Action.Add(TrusteeshipActionType.FindTarget, new FindTargetAction());
			_dicType2Action.Add(TrusteeshipActionType.GoOver, new GoOverAction());
			_dicType2Action.Add(TrusteeshipActionType.CollectPlant, new CollectPlantAction());
			_dicType2Action.Add(TrusteeshipActionType.Mining, new MiningAction());
			_dicType2Action.Add(TrusteeshipActionType.CutDownATree, new CutDownATreeAction());
			_dicType2Action.Add(TrusteeshipActionType.WrongTypeStop, new WrongTypeStopAction());
			_dicType2Action.Add(TrusteeshipActionType.HitTrashcan, new HitTrashcanAction());
			_dicType2Action.Add(TrusteeshipActionType.PickItem, new PickItemAction());
			_dicType2Action.Add(TrusteeshipActionType.Heal, new HealAction());
			_dicType2Action.Add(TrusteeshipActionType.Eat, new EatAction());
		}
		if (TrusteeshipEvent.TrusteeshipOnOffDelegate != null)
		{
			TrusteeshipEvent.TrusteeshipOnOffDelegate(false);
		}
		if (CanStartTrusteeship())
		{
			if (CrossState.CanPlayStandCross)
			{
				_selfController.FSM.SwitchState(StateID.Stand);
			}
			else if (_selfController.FSM.CurrentState.ID != StateID.Swim && _selfController.FSM.CurrentState.ID != StateID.Stand)
			{
				AlertBox.Show(375);
				return;
			}
			Client2Gs.Ins.Send(_cCanBuild);
		}
	}

	public void EnterTrusteeship()
	{
		if (IsInTrusteeship)
		{
			return;
		}
		_autoHeal = SettingMgr.GetTrusteeshipHeal();
		_autoEat = SettingMgr.GetTrusteeshipEat();
		_healValue = SettingMgr.GetTrusteeshipHealValue();
		_eatValue = SettingMgr.GetTrusteeshipEatValue();
		_totalStop = false;
		IsInTrusteeship = true;
		StartPos = SelfPos;
		BagHaveCapacity = Singleton<BagMgr>.Ins.BagItems.Count < Singleton<BagMgr>.Ins.BagCapacity;
		_nextAction = _dicType2Action[TrusteeshipActionType.WrongTypeStop];
		if ((_autoHeal && Singleton<RoleMgr>.Ins.Blood < _healValue) || (_autoEat && Singleton<RoleMgr>.Ins.Hunger < _eatValue))
		{
			OnPropertyChange();
			if (_nextAction.SelfType != TrusteeshipActionType.WrongTypeStop)
			{
				_curAction = _nextAction;
			}
			else
			{
				_curAction = _dicType2Action[TrusteeshipActionType.FindTarget];
			}
		}
		else
		{
			_curAction = _dicType2Action[TrusteeshipActionType.FindTarget];
		}
		_curAction.OnEnter();
		BattleEvent.OnPlayerOperateStopTrusteeship += StopTrusteeship;
		RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnPropertyChange));
		BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
		BattleEvent.OnSelfSwitchState = (BattleEvent.OnSelfSwitchStateEvent)Delegate.Combine(BattleEvent.OnSelfSwitchState, new BattleEvent.OnSelfSwitchStateEvent(OnStateChange));
		BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Combine(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeWeapon));
		EventHandlers.OnAimedGo = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedGo, new Utils.LongDelegate(OnAimPlant));
	}

	private void OnAimPlant(long arg)
	{
		AimInsId = arg;
	}

	private void ChangeWeapon(long arg)
	{
		if (IsInTrusteeship && _curAction.SelfType != TrusteeshipActionType.Eat && _curAction.SelfType != TrusteeshipActionType.Heal && arg < 0)
		{
			PutWeaponToHand();
		}
	}

	private void OnStateChange(StateID from, StateID to)
	{
		if (IsInTrusteeship && !_trusteeshipStates.Contains(to))
		{
			AlertBox.Show(378);
			StopTrusteeship();
		}
	}

	private void OnItemChange(int itemId, int num)
	{
		if (IsInTrusteeship)
		{
			BagHaveCapacity = Singleton<BagMgr>.Ins.BagItems.Count < Singleton<BagMgr>.Ins.BagCapacity;
			if (!BagHaveCapacity)
			{
				AlertBox.Show(384);
				StopTrusteeship();
			}
		}
	}

	private void OnPropertyChange()
	{
		if (!IsInTrusteeship)
		{
			return;
		}
		if (_selfController.IsDownWaitSave)
		{
			MessageBoxPanel.Show(373);
			StopTrusteeship();
			return;
		}
		MedicineId = -1;
		FoodId = -1;
		if (_autoHeal && Singleton<RoleMgr>.Ins.Blood <= _healValue)
		{
			BagItem bagItemByItemType = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.QuickUseItems.Values, 32);
			if (bagItemByItemType == null)
			{
				bagItemByItemType = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.BagItems, 32);
			}
			if (bagItemByItemType != null)
			{
				MedicineId = bagItemByItemType.instanceId;
				SetNextAction(TrusteeshipActionType.Heal);
				return;
			}
			AlertBox.Show(369);
		}
		if (_autoEat && Singleton<RoleMgr>.Ins.Hunger <= _eatValue)
		{
			BagItem bagItemByItemType2 = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.QuickUseItems.Values, 124);
			if (bagItemByItemType2 == null)
			{
				bagItemByItemType2 = Singleton<BagMgr>.Ins.GetBagItemByItemType(Singleton<BagMgr>.Ins.BagItems, 124);
			}
			if (bagItemByItemType2 != null)
			{
				FoodId = bagItemByItemType2.instanceId;
				SetNextAction(TrusteeshipActionType.Eat);
			}
			else
			{
				AlertBox.Show(370);
			}
		}
	}

	public void StopTrusteeship()
	{
		IntervalTime = 0.1f;
		_totalStop = true;
		StopAllCoroutines();
		_delayAction = null;
		StopTrusteeshipAction();
	}

	private void StopTrusteeshipAction()
	{
		if (IsInTrusteeship)
		{
			IsInTrusteeship = false;
			_curAction.OnExit();
			_curAction = null;
			BattleEvent.OnPlayerOperateStopTrusteeship -= StopTrusteeship;
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnPropertyChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			BattleEvent.OnSelfSwitchState = (BattleEvent.OnSelfSwitchStateEvent)Delegate.Remove(BattleEvent.OnSelfSwitchState, new BattleEvent.OnSelfSwitchStateEvent(OnStateChange));
			BagEvent.ChangeHandInstanceIdDelegate = (Utils.LongDelegate)Delegate.Remove(BagEvent.ChangeHandInstanceIdDelegate, new Utils.LongDelegate(ChangeWeapon));
			EventHandlers.OnAimedGo = (Utils.LongDelegate)Delegate.Remove(EventHandlers.OnAimedGo, new Utils.LongDelegate(OnAimPlant));
		}
	}

	public void ChangeTrusteeshipAction(Transform targetTrans = null)
	{
		_curAction.OnExit();
		IntervalTime = 0.1f;
		_curAction = _nextAction;
		_curAction.OnEnter(targetTrans);
	}

	public void SetNextAction(TrusteeshipActionType type)
	{
		if (!_dicType2Action.TryGetValue(type, out _nextAction))
		{
			_nextAction = _dicType2Action[TrusteeshipActionType.FindTarget];
			Debug.LogError(string.Concat("[TrusteeshipMgr.cs]Not find action with type : ", type, "."));
		}
	}

	public void LookAtVector(Vector3 dir)
	{
		_mainCamera.SetCameraTargetDirection(dir);
	}

	public void ChangeActionWithDelay()
	{
		if (_delayAction == null)
		{
			_delayAction = StartCoroutine(ChangeActionDelay());
		}
	}

	private IEnumerator ChangeActionDelay()
	{
		yield return Utils.WaitForSeconds(0.1f);
		ChangeTrusteeshipAction();
		_delayAction = null;
	}

	private void Update()
	{
		if (TrusteeshipEvent.UpdateNeedChangeDelegate == null)
		{
			return;
		}
		if (UpdateEveryFrame)
		{
			if (TrusteeshipEvent.UpdateNeedChangeDelegate() && TrusteeshipEvent.UpdateFinishDelegate != null)
			{
				TrusteeshipEvent.UpdateFinishDelegate();
			}
			return;
		}
		_passTime += Time.deltaTime;
		if (_passTime > IntervalTime)
		{
			_passTime = 0f;
			if (TrusteeshipEvent.UpdateNeedChangeDelegate() && TrusteeshipEvent.UpdateFinishDelegate != null)
			{
				TrusteeshipEvent.UpdateFinishDelegate();
			}
		}
	}
}
