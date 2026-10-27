using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using cfg;

public class DoorInfo : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003COpenRealityDoorAnim_003Ec__AnonStorey0
	{
		internal BoxCollider boxCo;

		internal DoorInfo _0024this;

		internal void _003C_003Em__0()
		{
			if ((bool)boxCo)
			{
				boxCo.isTrigger = true;
			}
			_0024this.CanOperation = false;
		}

		internal void _003C_003Em__1()
		{
			if ((bool)boxCo)
			{
				boxCo.isTrigger = false;
			}
			_0024this.CanOperation = true;
			if (_0024this._bAutoClose)
			{
				Singleton<DoorMgr>.Ins.CloseDoor(_0024this);
				_0024this._bAutoClose = false;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CCloseRealityDoorAnim_003Ec__AnonStorey1
	{
		internal BoxCollider boxCo;

		internal DoorInfo _0024this;

		internal void _003C_003Em__0()
		{
			if ((bool)boxCo)
			{
				boxCo.isTrigger = true;
			}
			_0024this.CanOperation = false;
		}

		internal void _003C_003Em__1()
		{
			if ((bool)boxCo)
			{
				boxCo.isTrigger = false;
			}
			_0024this.CanOperation = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CAddLock_003Ec__AnonStorey2
	{
		internal Transform lockParent;

		internal DoorInfo _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if (((bool)lockParent && lockParent.childCount > 0) || !Singleton<DoorMgr>.Ins.IsHasLock(_0024this._state))
			{
				UnityEngine.Object.DestroyImmediate(go, true);
				return;
			}
			Transform transform = go.transform;
			transform.SetParent(lockParent);
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;
		}
	}

	[HideInInspector]
	public long InstanceId;

	[HideInInspector]
	public int Level = 1;

	private int _state;

	private bool _bAutoClose;

	[HideInInspector]
	public bool CanOperation = true;

	private readonly Dictionary<int, Transform[]> _dicLevel2DoorTrans = new Dictionary<int, Transform[]>();

	[SerializeField]
	private Transform[] m_LockTrans;

	[SerializeField]
	private Transform[] m_doorsLevelOne;

	[SerializeField]
	private Transform[] m_doorsLevelTwo;

	[SerializeField]
	private Transform[] m_doorsLevelThree;

	[SerializeField]
	private Transform[] m_doorsLevelFoue;

	public int State
	{
		get
		{
			return _state;
		}
	}

	private void InitDoorDataReal(long instanceId, int level, int status)
	{
		InstanceId = instanceId;
		Level = level;
		if (Singleton<DoorMgr>.Ins.IsClose(State) && !Singleton<DoorMgr>.Ins.IsClose(status))
		{
			OpenDoor(status, false);
		}
		else if (!Singleton<DoorMgr>.Ins.IsClose(State) && Singleton<DoorMgr>.Ins.IsClose(status))
		{
			CloseDoor(status, false);
		}
		if (Singleton<DoorMgr>.Ins.IsHasLock(status))
		{
			AddLock(cfg.Consts.PASSWORD_LOCK_ITEM_ID, status);
		}
		_state = status;
		CanOperation = true;
		int i = 0;
		for (int num = m_doorsLevelOne.Length; i < num; i++)
		{
			m_doorsLevelOne[i].name = InstanceId.ToString();
			m_doorsLevelTwo[i].name = InstanceId.ToString();
			m_doorsLevelThree[i].name = InstanceId.ToString();
			m_doorsLevelFoue[i].name = InstanceId.ToString();
		}
	}

	protected void Awake()
	{
		InstanceId = -1L;
		_dicLevel2DoorTrans[1] = m_doorsLevelOne;
		_dicLevel2DoorTrans[2] = m_doorsLevelTwo;
		_dicLevel2DoorTrans[3] = m_doorsLevelThree;
		_dicLevel2DoorTrans[4] = m_doorsLevelFoue;
	}

	protected void OnDestroy()
	{
		Singleton<DoorMgr>.Ins.DestroyDoorObj(this);
	}

	public void OpenDoor(int state, bool isPlayAnim)
	{
		if (!CanOperation)
		{
			return;
		}
		Transform[] doorsByLevel = GetDoorsByLevel();
		_state = state;
		Action<Transform, int> action = ((!isPlayAnim) ? new Action<Transform, int>(OpenRealityDoorNoAnim) : new Action<Transform, int>(OpenRealityDoorAnim));
		if (doorsByLevel == null)
		{
			return;
		}
		int i = 0;
		for (int num = doorsByLevel.Length; i < num; i++)
		{
			if (num > 1)
			{
				action(doorsByLevel[i], (Singleton<DoorMgr>.Ins.IsOpen_1(state) == (i % 2 == 0)) ? 2 : 0);
			}
			else
			{
				action(doorsByLevel[i], (!Singleton<DoorMgr>.Ins.IsOpen_1(state)) ? 2 : 0);
			}
		}
	}

	private Transform[] GetDoorsByLevel()
	{
		Transform[] value;
		_dicLevel2DoorTrans.TryGetValue(Level, out value);
		return value;
	}

	public void CloseDoor(int state, bool isPlayAnim)
	{
		if (!CanOperation)
		{
			return;
		}
		Transform[] doorsByLevel = GetDoorsByLevel();
		_state = state;
		Action<Transform, int> action = ((!isPlayAnim) ? new Action<Transform, int>(CloseRealityDoorNoAnim) : new Action<Transform, int>(CloseRealityDoorAnim));
		if (doorsByLevel == null)
		{
			return;
		}
		int i = 0;
		for (int num = doorsByLevel.Length; i < num; i++)
		{
			if (num == 1)
			{
				action(doorsByLevel[i], _state);
			}
			else
			{
				action(doorsByLevel[i], (Singleton<DoorMgr>.Ins.IsOpen_1(state) != (i % 2 == 0)) ? 2 : 0);
			}
		}
	}

	private void OpenRealityDoorAnim(Transform trans, int state)
	{
		_003COpenRealityDoorAnim_003Ec__AnonStorey0 _003COpenRealityDoorAnim_003Ec__AnonStorey = new _003COpenRealityDoorAnim_003Ec__AnonStorey0();
		_003COpenRealityDoorAnim_003Ec__AnonStorey._0024this = this;
		Vector3 localEulerAngles = trans.localEulerAngles;
		Vector3 endValue = default(Vector3);
		endValue.x = localEulerAngles.x;
		endValue.y = localEulerAngles.y;
		endValue.z = localEulerAngles.z + (float)((!Singleton<DoorMgr>.Ins.IsOpen_1(state)) ? (-90) : 90);
		SoundCfg soundCfg = SoundCfg.Get(307);
		SingletonMono<AudioManager>.Ins.Play(soundCfg.path, trans.position, false, soundCfg.volume, true, 0f, soundCfg.radius);
		Tweener t = trans.DOLocalRotate(endValue, ConstsBs.TIME_DOOR_ACTION);
		_003COpenRealityDoorAnim_003Ec__AnonStorey.boxCo = trans.GetComponent<BoxCollider>();
		t.OnStart(_003COpenRealityDoorAnim_003Ec__AnonStorey._003C_003Em__0);
		t.OnComplete(_003COpenRealityDoorAnim_003Ec__AnonStorey._003C_003Em__1);
	}

	private void OpenRealityDoorNoAnim(Transform trans, int state)
	{
		Vector3 localEulerAngles = trans.localEulerAngles;
		Vector3 localEulerAngles2 = default(Vector3);
		localEulerAngles2.x = localEulerAngles.x;
		localEulerAngles2.y = localEulerAngles.y;
		localEulerAngles2.z = localEulerAngles.z + (float)((!Singleton<DoorMgr>.Ins.IsOpen_1(state)) ? (-90) : 90);
		trans.localEulerAngles = localEulerAngles2;
	}

	private void CloseRealityDoorAnim(Transform trans, int state)
	{
		_003CCloseRealityDoorAnim_003Ec__AnonStorey1 _003CCloseRealityDoorAnim_003Ec__AnonStorey = new _003CCloseRealityDoorAnim_003Ec__AnonStorey1();
		_003CCloseRealityDoorAnim_003Ec__AnonStorey._0024this = this;
		Vector3 localEulerAngles = trans.localEulerAngles;
		Vector3 endValue = default(Vector3);
		endValue.x = localEulerAngles.x;
		endValue.y = localEulerAngles.y;
		endValue.z = localEulerAngles.z + (float)((!Singleton<DoorMgr>.Ins.IsOpen_1(state)) ? (-90) : 90);
		SoundCfg soundCfg = SoundCfg.Get(308);
		SingletonMono<AudioManager>.Ins.Play(soundCfg.path, trans.position, false, soundCfg.volume, true, 0f, soundCfg.radius);
		Tweener t = trans.DOLocalRotate(endValue, ConstsBs.TIME_DOOR_ACTION);
		_003CCloseRealityDoorAnim_003Ec__AnonStorey.boxCo = trans.GetComponent<BoxCollider>();
		t.OnStart(_003CCloseRealityDoorAnim_003Ec__AnonStorey._003C_003Em__0);
		t.OnComplete(_003CCloseRealityDoorAnim_003Ec__AnonStorey._003C_003Em__1);
	}

	private void CloseRealityDoorNoAnim(Transform trans, int state)
	{
		Vector3 localEulerAngles = trans.localEulerAngles;
		Vector3 localEulerAngles2 = default(Vector3);
		localEulerAngles2.x = localEulerAngles.x;
		localEulerAngles2.y = localEulerAngles.y;
		localEulerAngles2.z = localEulerAngles.z + (float)((!Singleton<DoorMgr>.Ins.IsOpen_1(state)) ? (-90) : 90);
		trans.localEulerAngles = localEulerAngles2;
	}

	public void AddLock(int itemId, int state)
	{
		_state = state;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg != null)
		{
			_003CAddLock_003Ec__AnonStorey2 _003CAddLock_003Ec__AnonStorey = new _003CAddLock_003Ec__AnonStorey2();
			_003CAddLock_003Ec__AnonStorey._0024this = this;
			_003CAddLock_003Ec__AnonStorey.lockParent = GetLockParent();
			if (_003CAddLock_003Ec__AnonStorey.lockParent == null)
			{
				Debug.LogError("[DoorInfo.cs:AddLock]No Lock Parent Find.");
			}
			else if (_003CAddLock_003Ec__AnonStorey.lockParent.childCount <= 0)
			{
				ResMgr.Ins.CreateFromAB((!base.name.StartsWith("gao")) ? itemCfg.modelPath : itemCfg.womanModelPath, string.Empty, _003CAddLock_003Ec__AnonStorey._003C_003Em__0);
			}
			else
			{
				_003CAddLock_003Ec__AnonStorey.lockParent.GetChild(0).gameObject.SetActiveBetter(true);
			}
		}
	}

	private Transform GetLockParent()
	{
		int num = Level - 1;
		if (num < m_LockTrans.Length)
		{
			return m_LockTrans[num];
		}
		return null;
	}

	public void RemoveLock(int state)
	{
		Transform lockParent = GetLockParent();
		if (lockParent == null)
		{
			Debug.LogError("[DoorInfo.cs:RemoveLock]No Lock Parent Find.");
		}
		else if (lockParent.childCount > 0)
		{
			_state = state;
			lockParent.GetChild(0).gameObject.SetActiveBetter(false);
		}
	}

	public void InitDoorData(long instanceId, int level, int status)
	{
		InitDoorDataReal(instanceId, level, status);
	}
}
