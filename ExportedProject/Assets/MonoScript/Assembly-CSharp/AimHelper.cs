using System;
using System.Collections;
using UnityEngine;

public class AimHelper : MonoBehaviour
{
	private const float SwitchThreshold = 1600f;

	private const float LockMinDistance = 1225f;

	private const float AttractMaxDistance = 2025f;

	private vThirdPersonCamera _camera;

	private Transform _cameraTransform;

	private Transform _selfPlayerTransform;

	private int _bigLayer;

	private int _bigLayerMask;

	private int _sceneLayerMask;

	private AimMagnet _lockedTarget;

	private Collider _lockedCollider;

	private Transform _lockedTransform;

	private OtherPlayerController _lockedPlayer;

	private Vector3 _lockedOffset;

	private Ray _ray;

	public bool m_Disabled = true;

	private bool _attracting;

	private readonly WaitForSeconds _wait = new WaitForSeconds(0.2f);

	public bool Disabled
	{
		get
		{
			return m_Disabled;
		}
		set
		{
			m_Disabled = value;
			if (m_Disabled)
			{
				UnlockTarget();
			}
		}
	}

	public void Start()
	{
		_camera = GetComponent<vThirdPersonCamera>();
		_cameraTransform = _camera.transform;
		_selfPlayerTransform = Battle.Ins.SelfPlayer.PlayerTransform;
		_bigLayer = LayerMask.NameToLayer("AimHelperBig");
		_bigLayerMask = 1 << _bigLayer;
		_sceneLayerMask = LayerMask.GetMask("Default", "Door", "Car", "CarForBullet", "AimHelperSmall");
		StartCoroutine(CheckUnLock());
	}

	public void CancelIfLockedTarget(Transform target)
	{
		if (_lockedTransform != null && _lockedTransform == target)
		{
			UnlockTarget();
		}
	}

	public bool TryLockTarget()
	{
		return false;
	}

	private bool CheckTargetVisible(OtherPlayerController target)
	{
		if (target.IsPa || target.IsDownWaitSave || target.IsDie)
		{
			return false;
		}
		return !Physics.Linecast(_cameraTransform.position, target.RootBone.position, _sceneLayerMask) || !Physics.Linecast(_cameraTransform.position, target.HeadBub.position, _sceneLayerMask);
	}

	private void CheckAttractOrLock(Vector3 lockedPos)
	{
		float num = Vector3.SqrMagnitude(_selfPlayerTransform.position - _lockedTransform.position);
		if (num < 1225f)
		{
			if (!SettingMgr.AutoAim)
			{
				UnlockTarget();
			}
			else if (!_attracting)
			{
				_attracting = true;
				_lockedTarget.AimEffect.SetActiveBetter(false);
			}
		}
		else if (num > 2025f)
		{
			if (!SettingMgr.HelpAim)
			{
				UnlockTarget();
			}
			else if (_attracting)
			{
				_attracting = false;
				_lockedTarget.AimEffect.SetActiveBetter(true);
			}
		}
	}

	public void DoUpdate()
	{
		if (_lockedTransform == null)
		{
			return;
		}
		_ray.origin = _camera.GetTargetLookAtTransform().position;
		_ray.direction = _camera.GetBaseDirection();
		RaycastHit hitInfo;
		if (Physics.Raycast(_ray, out hitInfo, 600f, _bigLayerMask) && hitInfo.collider == _lockedCollider)
		{
			CheckAttractOrLock(hitInfo.point);
			if (_attracting)
			{
				Vector3 attractPoint = GetAttractPoint(hitInfo.point);
				if (!MathUtils.RoughlyEquals(attractPoint, hitInfo.point, 0.05f))
				{
					float val = Vector3.Distance(attractPoint, hitInfo.point);
					_camera.AttractCamera(attractPoint, 0.03f / Math.Max(val, 0.05f));
				}
			}
		}
		else
		{
			UnlockTarget();
		}
	}

	private Vector3 GetAttractPoint(Vector3 lockedPos)
	{
		Vector3 vector = _lockedCollider.ClosestPoint(_lockedPlayer.RootBone.position + _lockedPlayer.Velocity * 0.05f);
		Vector3 vector2 = _lockedCollider.ClosestPoint(_lockedPlayer.HeadBub.position + _lockedPlayer.Velocity * 0.05f);
		Vector3 onNormal = vector2 - vector;
		Vector3 result = Vector3.Project(lockedPos - vector, onNormal) + vector;
		result.x = MathUtils.ClampBetween(result.x, vector.x, vector2.x);
		result.y = MathUtils.ClampBetween(result.y, vector.y, vector2.y);
		result.z = MathUtils.ClampBetween(result.z, vector.z, vector2.z);
		return result;
	}

	private IEnumerator CheckUnLock()
	{
		while (true)
		{
			if (_lockedTransform != null && (!_lockedCollider.enabled || !CheckTargetVisible(_lockedPlayer)))
			{
				UnlockTarget();
			}
			yield return _wait;
		}
	}

	public void UnlockTarget()
	{
		if (!(_lockedTransform == null))
		{
			_lockedTarget.AimEffect.SetActive(false);
			_lockedTarget = null;
			_lockedCollider = null;
			_lockedTransform = null;
			_lockedPlayer = null;
			_attracting = false;
			_camera.CancelLockAt();
		}
	}
}
