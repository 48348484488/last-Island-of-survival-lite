using System;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.battle.map.turret.scmsg;
using gs.battle.scmsg;
using gs.smelter.scmsg;

public class TurretMgr : Singleton<TurretMgr>
{
	private HashSet<long> _turretBuildPart = new HashSet<long>();

	private Dictionary<long, TurretGameObject> _turretGameObjectDic = new Dictionary<long, TurretGameObject>();

	private Dictionary<long, long> _turretBuildPartTargets = new Dictionary<long, long>();

	private Dictionary<long, SManagementTurret> _turretBuildPartManagements = new Dictionary<long, SManagementTurret>();

	private Dictionary<long, TurretItem> _turretHatredList = new Dictionary<long, TurretItem>();

	private Dictionary<long, int> _turretSwitchDic = new Dictionary<long, int>();

	private Dictionary<long, float> _turretAngle = new Dictionary<long, float>();

	private Dictionary<long, bool> _turretHasBullet = new Dictionary<long, bool>();

	private STurretBuildingInfo _sTurretBuildingInfo;

	private TurrentUpdate _turrentUpdate;

	private long _currentOpenTurret;

	private Coroutine _pingCoroutine;

	private CTurretBuildingInfo _cTurretBuildingInfo = new CTurretBuildingInfo();

	private CTurretTargetRole _cTurretTargetRole = new CTurretTargetRole();

	private CCancelTurretTargetRole _cCancelTurretTargetRole = new CCancelTurretTargetRole();

	private CAddPermissions _cAddPermissions = new CAddPermissions();

	private CRemovePermissions _cRemovePermissions = new CRemovePermissions();

	private CChangeBullets _cAddBullets = new CChangeBullets();

	private CRemoveBulletsToBag _cRemoveBulletsToBag = new CRemoveBulletsToBag();

	private CManagementTurret _cManagementTurret = new CManagementTurret();

	private CCancelManagementTurret _cCancelManagementTurret = new CCancelManagementTurret();

	private CEditorTurret _cEditorTurret = new CEditorTurret();

	private static readonly COtherShoot _cOtherShoot = new COtherShoot();

	private CReportManagementTurret _cReportManagementTurret = new CReportManagementTurret();

	private CChangeTurretCartridgeIndex _cChangeTurretCartridgeIndex = new CChangeTurretCartridgeIndex();

	private CChangeTurretStatus _cChangeTurretStatus = new CChangeTurretStatus();

	private CSynchronizedTurretPatrolAngle _cSynchronizedTurretPatrolAngle = new CSynchronizedTurretPatrolAngle();

	private CSynchronizedTurretBulletStatus _cSynchronizedTurretBulletStatus = new CSynchronizedTurretBulletStatus();

	public void Init()
	{
		SBuildPart.handler = (SBuildPart.Handler)Delegate.Combine(SBuildPart.handler, new SBuildPart.Handler(SBuildPartHandle));
		SCanManagementTurret.handler = (SCanManagementTurret.Handler)Delegate.Combine(SCanManagementTurret.handler, new SCanManagementTurret.Handler(SCanManagementTurretHandle));
		SManagementTurret.handler = (SManagementTurret.Handler)Delegate.Combine(SManagementTurret.handler, new SManagementTurret.Handler(SManagementTurretHandle));
		SCancelManagementTurret.handler = (SCancelManagementTurret.Handler)Delegate.Combine(SCancelManagementTurret.handler, new SCancelManagementTurret.Handler(SCancelManagementTurretHandle));
		STurretTargetRole.handler = (STurretTargetRole.Handler)Delegate.Combine(STurretTargetRole.handler, new STurretTargetRole.Handler(STurretTargetRoleHandle));
		SCancelTurretTargetRole.handler = (SCancelTurretTargetRole.Handler)Delegate.Combine(SCancelTurretTargetRole.handler, new SCancelTurretTargetRole.Handler(SCancelTurretTargetRoleHandle));
		EventHandlers.OnBuildPartUpdate = (EventHandlers.BuildPartUpdate)Delegate.Combine(EventHandlers.OnBuildPartUpdate, new EventHandlers.BuildPartUpdate(OnBuildUpgrade));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(SBuildingStatusChangeHandle));
		TurrentEvent.DestoryTurrentDelegate = (Utils.LongDelegate)Delegate.Combine(TurrentEvent.DestoryTurrentDelegate, new Utils.LongDelegate(DestoryTurrentDelegate));
		STurretBuildingInfo.handler = (STurretBuildingInfo.Handler)Delegate.Combine(STurretBuildingInfo.handler, new STurretBuildingInfo.Handler(STurretBuildingInfoHandle));
		TurrentEvent.CloseTurrentDelegate = (Utils.LongDelegate)Delegate.Combine(TurrentEvent.CloseTurrentDelegate, new Utils.LongDelegate(CloseTurrentDelegate));
		_turrentUpdate = TurrentUpdate.Init();
		_turrentUpdate.AddUpdate(Update);
		Singleton<StructureMenuMgr>.Ins.AddPermitFunc(106, SetOpenBtns);
		Singleton<StructureMenuMgr>.Ins.AddPermitFunc(105, SetOpenBtns);
		SEnterBuilding.handler = (SEnterBuilding.Handler)Delegate.Combine(SEnterBuilding.handler, new SEnterBuilding.Handler(SEnterBuildingHandle));
		SCreateRoleTroopChange.handler = (SCreateRoleTroopChange.Handler)Delegate.Combine(SCreateRoleTroopChange.handler, new SCreateRoleTroopChange.Handler(SCreateRoleTroopChangeHandle));
		SSynchronizedTurretPatrolAngle.handler = (SSynchronizedTurretPatrolAngle.Handler)Delegate.Combine(SSynchronizedTurretPatrolAngle.handler, new SSynchronizedTurretPatrolAngle.Handler(SSynchronizedTurretPatrolAngleHandle));
		SSynchronizedTurretBulletStatus.handler = (SSynchronizedTurretBulletStatus.Handler)Delegate.Combine(SSynchronizedTurretBulletStatus.handler, new SSynchronizedTurretBulletStatus.Handler(SSynchronizedTurretBulletStatusHanlde));
		STurrentBeAttacked.handler = (STurrentBeAttacked.Handler)Delegate.Combine(STurrentBeAttacked.handler, new STurrentBeAttacked.Handler(STurrentBeAttackedHandle));
	}

	private void STurrentBeAttackedHandle(STurrentBeAttacked msg)
	{
		TurretGameObject value;
		if (_turretBuildPartManagements.ContainsKey(msg.turretId) && _turretGameObjectDic.TryGetValue(msg.turretId, out value))
		{
			value.AttackTurret(msg.roleId);
		}
	}

	private void SSynchronizedTurretBulletStatusHanlde(SSynchronizedTurretBulletStatus msg)
	{
		_turretHasBullet[msg.turretId] = msg.isHave;
	}

	private void SSynchronizedTurretPatrolAngleHandle(SSynchronizedTurretPatrolAngle msg)
	{
		_turretAngle[msg.turretId] = msg.angle;
	}

	private void SBuildingStatusChangeHandle(SBuildingStatusChange msg)
	{
		if (_turretSwitchDic.ContainsKey(msg.id))
		{
			_turretSwitchDic[msg.id] = msg.status;
			bool flag = IsTurrentOpen(msg.id);
			if (_turretGameObjectDic.ContainsKey(msg.id))
			{
				_turretGameObjectDic[msg.id].SetSwitch(flag);
			}
			if (flag && _sTurretBuildingInfo.turretId == msg.id)
			{
				ManagementTurretLocal(_sTurretBuildingInfo.turretId);
				CancelTurretTargetRole(_sTurretBuildingInfo.turretId);
			}
		}
	}

	private void SCreateRoleTroopChangeHandle(SCreateRoleTroopChange msg)
	{
		if (_turretBuildPartManagements.ContainsKey(msg.turretId))
		{
			_turretBuildPartManagements[msg.turretId].allRoleIdsInTroop = msg.allRoleIdsInTroop;
		}
	}

	private void SEnterBuildingHandle(SEnterBuilding msg)
	{
		if (msg.buildPartInfo.typeId > 0)
		{
			ItemCfg itemCfg = ItemCfg.Get(msg.buildPartInfo.typeId);
			if (itemCfg != null && (itemCfg.childType == 105 || itemCfg.childType == 106) && msg.buildPartInfo.roleId == Singleton<RoleMgr>.Ins.info.roleId && !_turretBuildPart.Contains(msg.buildPartInfo.instanceId))
			{
				_turretBuildPart.Add(msg.buildPartInfo.instanceId);
			}
		}
	}

	private bool SetOpenBtns(long instanceId)
	{
		return _turretBuildPart.Contains(instanceId);
	}

	public void Update()
	{
	}

	private void CloseTurrentDelegate(long turretId)
	{
		if (_currentOpenTurret == turretId)
		{
			_currentOpenTurret = 0L;
		}
	}

	public void ManagementTurretLocal(long turretId)
	{
		if (_sTurretBuildingInfo.turretId == turretId)
		{
			SManagementTurret sManagementTurret = new SManagementTurret();
			sManagementTurret.turretId = turretId;
			sManagementTurret.isAttackCompanions = _sTurretBuildingInfo.isAttackCompanions;
			sManagementTurret.isAttackAllies = _sTurretBuildingInfo.isAttackAllies;
			sManagementTurret.permissionsList = _sTurretBuildingInfo.permissionsList;
			sManagementTurret.bullets = _sTurretBuildingInfo.bullets;
			sManagementTurret.allRoleIdsInTroop = _sTurretBuildingInfo.allRoleIdsInTroop;
			SManagementTurretHandle(sManagementTurret);
		}
	}

	public void UpdateManagementTurretBullet(long turretId)
	{
		if (_turretBuildPartManagements.ContainsKey(turretId) && turretId == _sTurretBuildingInfo.turretId)
		{
			_turretBuildPartManagements[turretId].bullets = _sTurretBuildingInfo.bullets;
			CheckManagerTurretHasBullet(turretId, _sTurretBuildingInfo.bullets);
		}
	}

	private void DestoryTurrentDelegate(long turrentInstanceId)
	{
		CancelManagementTurret(turrentInstanceId);
	}

	private void OnBuildFinish(long insid, BuildPart buildpartcfg, int status, Octets extrainfooc)
	{
		ItemCfg itemCfg = ItemCfg.Get(buildpartcfg.id);
		if (itemCfg == null || (itemCfg.childType != 106 && itemCfg.childType != 105))
		{
			return;
		}
		_turretSwitchDic[insid] = status;
		GameObject partGoByInsID = SingletonMono<BuildManager>.Ins.GetPartGoByInsID(insid);
		TurretGameObject turretGameObject = partGoByInsID.GetComponent<TurretGameObject>();
		if (!turretGameObject)
		{
			turretGameObject = partGoByInsID.AddComponent<TurretGameObject>();
		}
		turretGameObject.SetData(insid, buildpartcfg.id);
		_turretGameObjectDic[insid] = turretGameObject;
		if (extrainfooc.Size > 0)
		{
			long num = extrainfooc.pop_long();
			if (num > 0)
			{
				_turretBuildPartTargets[insid] = num;
				_turretGameObjectDic[insid].SetTarget(num);
			}
			float value = extrainfooc.pop_float();
			if (num > 0)
			{
				_turretAngle[insid] = value;
			}
			bool value2 = extrainfooc.pop_bool();
			_turretHasBullet[insid] = value2;
		}
		turretGameObject.SetSwitch(IsTurrentOpen(insid));
	}

	private void OnBuildUpgrade(long insid, BuildPart buildpartcfg)
	{
	}

	private void SCancelTurretTargetRoleHandle(SCancelTurretTargetRole msg)
	{
		if (_turretBuildPartTargets.ContainsKey(msg.turretId))
		{
			_turretBuildPartTargets.Remove(msg.turretId);
		}
		if (_turretGameObjectDic.ContainsKey(msg.turretId))
		{
			_turretGameObjectDic[msg.turretId].CancelTarget();
		}
	}

	private void STurretTargetRoleHandle(STurretTargetRole msg)
	{
		_turretBuildPartTargets[msg.turretId] = msg.targetId;
		if (_turretGameObjectDic.ContainsKey(msg.turretId))
		{
			_turretGameObjectDic[msg.turretId].SetTarget(msg.targetId);
		}
	}

	private void SCancelManagementTurretHandle(SCancelManagementTurret msg)
	{
		if (_turretBuildPartManagements.ContainsKey(msg.turretId))
		{
			_turretBuildPartManagements.Remove(msg.turretId);
		}
		if (_turretHatredList.ContainsKey(msg.turretId))
		{
			_turretHatredList.Remove(msg.turretId);
		}
	}

	private void SManagementTurretHandle(SManagementTurret msg)
	{
		_turretBuildPartManagements[msg.turretId] = msg;
		ReportManagementTurret(msg.turretId);
		CheckManagerTurretHasBullet(msg.turretId, msg.bullets);
		if (_pingCoroutine == null)
		{
			_pingCoroutine = Utils.StartConroutine(PingGs());
		}
		if (!_turretHatredList.ContainsKey(msg.turretId))
		{
			_turretHatredList.Add(msg.turretId, new TurretItem());
		}
	}

	private void SCanManagementTurretHandle(SCanManagementTurret msg)
	{
		ManagementTurret(msg.turretId);
	}

	private void SBuildPartHandle(SBuildPart msg)
	{
		if (msg.buildPartInfo.typeId > 0)
		{
			ItemCfg itemCfg = ItemCfg.Get(msg.buildPartInfo.typeId);
			if (itemCfg != null && (itemCfg.childType == 105 || itemCfg.childType == 106) && msg.buildPartInfo.roleId == Singleton<RoleMgr>.Ins.info.roleId && !_turretBuildPart.Contains(msg.buildPartInfo.instanceId))
			{
				_turretBuildPart.Add(msg.buildPartInfo.instanceId);
			}
		}
	}

	private void STurretBuildingInfoHandle(STurretBuildingInfo msg)
	{
		_sTurretBuildingInfo = msg;
		if (!ViewMgr.Ins.IsShow<TurretPanel>())
		{
			ViewMgr.Ins.ShowView<TurretPanel>(_sTurretBuildingInfo);
			_currentOpenTurret = _sTurretBuildingInfo.turretId;
			if (IsTurrentOpen(_sTurretBuildingInfo.turretId))
			{
				ManagementTurretLocal(_sTurretBuildingInfo.turretId);
				CancelTurretTargetRole(_sTurretBuildingInfo.turretId);
			}
		}
		UpdateManagementTurretBullet(_sTurretBuildingInfo.turretId);
	}

	public void TurretBuildingInfo(long instanceId)
	{
		_cTurretBuildingInfo.turretId = instanceId;
		Client2Gs.Ins.Send(_cTurretBuildingInfo);
		CancelTurretTargetRole(instanceId);
	}

	public void TurretTargetRole(long instanceId, long targetId)
	{
		if (_turretBuildPartManagements.ContainsKey(instanceId) && ManagementHasBullet(instanceId) && _currentOpenTurret != instanceId)
		{
			_cTurretTargetRole.turretId = instanceId;
			_cTurretTargetRole.targetId = targetId;
			Client2Gs.Ins.Send(_cTurretTargetRole);
		}
	}

	public void CancelTurretTargetRole(long turretId)
	{
		if (_turretBuildPartManagements.ContainsKey(turretId))
		{
			_cCancelTurretTargetRole.turretId = turretId;
			Client2Gs.Ins.Send(_cCancelTurretTargetRole);
		}
	}

	public void AddPermissions(long instanceId, List<long> targetIds)
	{
		_cAddPermissions.turretId = instanceId;
		_cAddPermissions.targetIds = targetIds;
		Client2Gs.Ins.Send(_cAddPermissions);
	}

	public void RemovePermissions(long instanceId, long targetId)
	{
		_cRemovePermissions.turretId = instanceId;
		Client2Gs.Ins.Send(_cRemovePermissions);
	}

	public void AddBullets(long instanceId, Dictionary<int, UseItem> bullets)
	{
		_cAddBullets.turretId = instanceId;
		_cAddBullets.bullets = bullets;
		Client2Gs.Ins.Send(_cAddBullets);
	}

	public void RemoveBulletsToBag(long instanceId, Dictionary<int, UseItem> bullets)
	{
		_cRemoveBulletsToBag.turretId = instanceId;
		_cRemoveBulletsToBag.bullets = bullets;
		Client2Gs.Ins.Send(_cRemoveBulletsToBag);
	}

	public void ManagementTurret(long turretId)
	{
		_cManagementTurret.turretId = turretId;
		Client2Gs.Ins.Send(_cManagementTurret);
	}

	public void CancelManagementTurret(long turretId)
	{
		_cCancelManagementTurret.turretId = turretId;
		Client2Gs.Ins.Send(_cCancelManagementTurret);
	}

	public void EditorTurret(long turretInstanceId, bool isAttackAllies, bool isAttackTeam)
	{
		_cEditorTurret.turretId = turretInstanceId;
		_cEditorTurret.isAttackAllies = isAttackAllies;
		_cEditorTurret.isAttackCompanions = isAttackTeam;
		Client2Gs.Ins.Send(_cEditorTurret);
	}

	public void Shoot(long turretInstanceId)
	{
		_cOtherShoot.shooterInsId = turretInstanceId;
		Client2Gs.Ins.Send(_cOtherShoot);
	}

	public void ReportManagementTurret(long turretInstanceId)
	{
		_cReportManagementTurret.turretId = turretInstanceId;
		Client2Gs.Ins.Send(_cReportManagementTurret);
	}

	public void ReportManagementTurret(long turretInstanceId, int changeIndex, int targetIndex)
	{
		_cChangeTurretCartridgeIndex.turretId = turretInstanceId;
		_cChangeTurretCartridgeIndex.changeIndex = changeIndex;
		_cChangeTurretCartridgeIndex.targetIndex = targetIndex;
		Client2Gs.Ins.Send(_cChangeTurretCartridgeIndex);
	}

	public void ChangeTurretStatus(long turretInstanceId, bool isOpen)
	{
		byte change = (byte)((!isOpen) ? 1 : 0);
		ChangeTurretStatus(turretInstanceId, change);
	}

	public void ChangeTurretStatus(long turretInstanceId, byte change)
	{
		_cChangeTurretStatus.turretId = turretInstanceId;
		_cChangeTurretStatus.change = change;
		Client2Gs.Ins.Send(_cChangeTurretStatus);
	}

	public void SynchronizedTurretPatrolAngle(long instanceId, float angle)
	{
		_cSynchronizedTurretPatrolAngle.turretId = instanceId;
		_cSynchronizedTurretPatrolAngle.angle = angle;
		Client2Gs.Ins.Send(_cSynchronizedTurretPatrolAngle);
	}

	public void SynchronizedTurretBulletStatus(long instanceId, bool isHave)
	{
		_cSynchronizedTurretBulletStatus.turretId = instanceId;
		_cSynchronizedTurretBulletStatus.isHave = isHave;
		Client2Gs.Ins.Send(_cSynchronizedTurretBulletStatus);
	}

	public void CheckManagerTurretHasBullet(long turretInstanceId, Dictionary<int, UseItem> bullets)
	{
		foreach (KeyValuePair<int, UseItem> bullet in bullets)
		{
			if (bullet.Value.num > 0)
			{
				SynchronizedTurretBulletStatus(turretInstanceId, true);
				return;
			}
		}
		SynchronizedTurretBulletStatus(turretInstanceId, false);
	}

	public bool IsHasBullet(long turretInstanceId)
	{
		if (_turretHasBullet.ContainsKey(turretInstanceId))
		{
			return _turretHasBullet[turretInstanceId];
		}
		return false;
	}

	public bool IsFire(long turrentInstanceId, long roleId)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(turrentInstanceId);
		if (!partByInsID)
		{
			return false;
		}
		if (partByInsID.OwnerRoleId == roleId)
		{
			return false;
		}
		if (!_turretBuildPartManagements.ContainsKey(turrentInstanceId))
		{
			return false;
		}
		SManagementTurret sManagementTurret = _turretBuildPartManagements[turrentInstanceId];
		if (sManagementTurret.isAttackCompanions && sManagementTurret.allRoleIdsInTroop.Contains(roleId))
		{
			return true;
		}
		return !sManagementTurret.permissionsList.Contains(roleId);
	}

	public bool IsOwner(long turrentInstanceId, long roleId)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(turrentInstanceId);
		return partByInsID.OwnerRoleId == roleId;
	}

	public void UseBullet(long turrentInstanceId)
	{
		if (!_turretBuildPartManagements.ContainsKey(turrentInstanceId))
		{
			return;
		}
		SManagementTurret sManagementTurret = _turretBuildPartManagements[turrentInstanceId];
		bool flag = false;
		foreach (KeyValuePair<int, UseItem> bullet in sManagementTurret.bullets)
		{
			UseItem value = bullet.Value;
			if (value.num > 1)
			{
				value.num--;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			CancelTurretTargetRole(turrentInstanceId);
			SynchronizedTurretBulletStatus(turrentInstanceId, false);
		}
	}

	public int GetTurrentState(long turretId)
	{
		if (_turretSwitchDic.ContainsKey(turretId) && _turretSwitchDic[turretId] == 0)
		{
			return 0;
		}
		return 1;
	}

	public bool IsTurrentOpen(long turretId)
	{
		return GetTurrentState(turretId) == 0;
	}

	public void UseBullet(long turrentInstanceId, int bulletInstanceId, int bulletNum)
	{
		if (!_turretBuildPartManagements.ContainsKey(turrentInstanceId))
		{
			return;
		}
		SManagementTurret sManagementTurret = _turretBuildPartManagements[turrentInstanceId];
		if (sManagementTurret.bullets.ContainsKey(bulletInstanceId))
		{
			UseItem useItem = sManagementTurret.bullets[bulletInstanceId];
			if (useItem.num >= 0)
			{
				useItem.num -= bulletNum;
			}
		}
	}

	public bool ManagementHasBullet(long turrentInstanceId)
	{
		if (!_turretBuildPartManagements.ContainsKey(turrentInstanceId))
		{
			return false;
		}
		SManagementTurret sManagementTurret = _turretBuildPartManagements[turrentInstanceId];
		foreach (KeyValuePair<int, UseItem> bullet in sManagementTurret.bullets)
		{
			UseItem value = bullet.Value;
			if (value.num > 0)
			{
				return true;
			}
		}
		return false;
	}

	public int GetTurretGunId(int turrentItemId)
	{
		TurretCfg turretCfg = TurretCfg.Get(turrentItemId);
		if (turretCfg == null)
		{
			return -1;
		}
		return turretCfg.gunId;
	}

	public int GetTurretGunId(TurretCfg turretCfg)
	{
		if (turretCfg == null)
		{
			return -1;
		}
		return turretCfg.gunId;
	}

	public bool IsTurretManager(long instanceId)
	{
		return _turretBuildPartManagements.ContainsKey(instanceId);
	}

	public float GetAngle(long instanceId)
	{
		if (_turretAngle.ContainsKey(instanceId))
		{
			return _turretAngle[instanceId];
		}
		return 0f;
	}

	private IEnumerator PingGs()
	{
		while (true)
		{
			foreach (KeyValuePair<long, SManagementTurret> turretBuildPartManagement in _turretBuildPartManagements)
			{
				ReportManagementTurret(turretBuildPartManagement.Key);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public TurretItem GetTurretItem(long turretInstanceId)
	{
		if (!_turretHatredList.ContainsKey(turretInstanceId))
		{
			return null;
		}
		return _turretHatredList[turretInstanceId];
	}

	public long GetHatredRoleId(long turretInstanceId)
	{
		TurretItem turretItem = GetTurretItem(turretInstanceId);
		if (turretItem == null)
		{
			return -1L;
		}
		if (turretItem.AttackHatredRoleId > 0)
		{
			return turretItem.AttackHatredRoleId;
		}
		List<long> rangeHatredList = turretItem.RangeHatredList;
		if (rangeHatredList.Count > 0)
		{
			return rangeHatredList[0];
		}
		return -1L;
	}

	public bool IsContainRoleId(long turretInstanceId, long roldId)
	{
		TurretItem turretItem = GetTurretItem(turretInstanceId);
		if (turretItem == null)
		{
			return false;
		}
		if (turretItem.AttackHatredRoleId == roldId)
		{
			return true;
		}
		return turretItem.RangeHatredList.Contains(roldId);
	}

	public void RefreshHatredList(long turretInstanceId, List<long> roleIds)
	{
		TurretItem turretItem = GetTurretItem(turretInstanceId);
		if (turretItem == null)
		{
			return;
		}
		List<long> rangeHatredList = turretItem.RangeHatredList;
		if (rangeHatredList == null)
		{
			return;
		}
		for (int i = 0; i < rangeHatredList.Count; i++)
		{
			if (!roleIds.Contains(rangeHatredList[i]))
			{
				rangeHatredList.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < roleIds.Count; j++)
		{
			if (!rangeHatredList.Contains(roleIds[j]))
			{
				rangeHatredList.Add(roleIds[j]);
			}
		}
	}

	public long GetTurretAttackRole(long turretInstanceId)
	{
		if (!_turretHatredList.ContainsKey(turretInstanceId))
		{
			return -1L;
		}
		return _turretHatredList[turretInstanceId].AttackHatredRoleId;
	}

	public void SetTurretAttackRole(long turretInstanceId, long roleId)
	{
		if (_turretHatredList.ContainsKey(turretInstanceId))
		{
			_turretHatredList[turretInstanceId].AttackHatredRoleId = roleId;
		}
	}

	public void ClearTurretAttackRole(long turretInstanceId)
	{
		if (_turretHatredList.ContainsKey(turretInstanceId))
		{
			_turretHatredList[turretInstanceId].AttackHatredRoleId = -1L;
			_turretHatredList[turretInstanceId].RangeHatredList.Clear();
		}
	}
}
