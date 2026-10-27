using System;
using UnityEngine;

public class vThirdPersonCamera : MonoBehaviour
{
	[NonSerialized]
	public Transform target;

	public float cameraDefaultSmooth = 8f;

	private LayerMask cullingLayer = 1108901889;

	[Tooltip("Change this value If the camera pass through the wall")]
	public float clipPlaneMargin;

	public bool showGizmos;

	public Vector2 mouseSensitivityFactor = new Vector2(2.5f, 2.5f);

	public Transform SelfTransform;

	[HideInInspector]
	public int indexList;

	[HideInInspector]
	public vThirdPersonCameraListData CameraStateList;

	private Camera _camera;

	public Camera ChildCamera;

	private Transform _targetLookAt;

	[NonSerialized]
	[HideInInspector]
	public vThirdPersonCameraState toState;

	private vThirdPersonCameraState _currentState;

	private vThirdPersonCameraState _tmpState;

	private float _cullingDistance;

	[NonSerialized]
	[HideInInspector]
	public bool dragging;

	private bool _statelocked;

	private Vector3 _beginAngles;

	private Vector3 _changedAngles = Vector3.zero;

	public Vector3 offsetAngles;

	private bool _lockedPoint;

	private Vector3 _lockedPosition;

	private Transform _lockedTransform;

	private float _changeStateProgress = -1f;

	private Vector2 m_ToRayVec2 = Vector2.zero;

	private bool distanceChanged;

	private Vector3 m_NearPlaneHalfSize;

	private float m_LastFov = -1f;

	private RaycastHit[] m_RaycastHit = new RaycastHit[5];

	private bool shaking;

	[NonSerialized]
	public float shakeDistance;

	private float shakeDistanceMax = 0.1f;

	private float shakeBackSpeed = 5f;

	private float shakeBeginSpeed = 10f;

	private float shakeSpeed;

	private float shakeSmooth = 10f;

	public float Fov
	{
		get
		{
			return _camera.fieldOfView;
		}
	}

	public Camera Camera
	{
		get
		{
			return _camera;
		}
	}

	public vThirdPersonCameraState CurrentState
	{
		get
		{
			return _currentState;
		}
	}

	private void Awake()
	{
		_camera = GetComponent<Camera>();
		SelfTransform = base.transform;
	}

	public void Init(Transform followTarget, string initialState = null)
	{
		target = followTarget;
		_statelocked = false;
		_tmpState = null;
		toState = null;
		dragging = false;
		_lockedPoint = false;
		_lockedTransform = null;
		if (CameraStateList == null)
		{
			Debug.LogError("CameraStateList not assigned!");
			return;
		}
		if (target == null)
		{
			Debug.LogError("Camera target not assigned!");
			return;
		}
		if (string.IsNullOrEmpty(initialState))
		{
			initialState = "Default";
		}
		_currentState = new vThirdPersonCameraState("NULL");
		if (ChangeState(initialState, false))
		{
			_targetLookAt = new GameObject("CameraLookAt").transform;
			_targetLookAt.position = target.TransformPoint(_currentState.lookAtPosition);
			_targetLookAt.hideFlags = HideFlags.HideInHierarchy;
			_targetLookAt.rotation = target.rotation;
			_beginAngles = _targetLookAt.eulerAngles;
			_changedAngles = Vector3.zero;
			offsetAngles = Vector3.zero;
		}
	}

	public void SetMouseSensitivity(float x, float y)
	{
		mouseSensitivityFactor.x = x;
		mouseSensitivityFactor.y = y;
	}

	public void SetMouseSensitivitX(float x)
	{
		mouseSensitivityFactor.x = x;
	}

	public void SetMouseSensitivitY(float y)
	{
		mouseSensitivityFactor.y = y;
	}

	public void SetCameraOffsetDirection(Vector3 direction)
	{
		Vector3 eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
		offsetAngles = NormalizeAngles(eulerAngles - _beginAngles - _changedAngles);
	}

	public void SetCameraTargetDirection(Vector3 direction)
	{
		_targetLookAt.rotation = Quaternion.LookRotation(direction);
		_beginAngles = NormalizeAngles(_targetLookAt.eulerAngles);
		_changedAngles = Vector3.zero;
		offsetAngles = Vector3.zero;
		StopLockPoint();
	}

	public void SetLockAtPoint(Vector3 point)
	{
		if (!_lockedPoint || !(_lockedTransform == null) || !(_lockedPosition == point))
		{
			_lockedPoint = true;
			_lockedPosition = point;
			_lockedTransform = null;
			_beginAngles = NormalizeDirectionAngles(_lockedPosition - _targetLookAt.position);
			_changedAngles = Vector3.zero;
		}
	}

	public void StopLockPoint()
	{
		if (_lockedPoint && !(_lockedTransform != null))
		{
			_lockedPoint = false;
			_beginAngles = NormalizeAngles(_beginAngles + _changedAngles);
			_changedAngles = Vector3.zero;
		}
	}

	public void SetLockAtTransform(Transform targetTransform, Vector3 pointOffset)
	{
		if (!_lockedPoint || !(_lockedTransform == targetTransform) || !(_lockedPosition == pointOffset))
		{
			_lockedPoint = true;
			_lockedPosition = pointOffset;
			_lockedTransform = targetTransform;
			_beginAngles = NormalizeDirectionAngles(_lockedTransform.TransformPoint(_lockedPosition) - _targetLookAt.position);
			_changedAngles = Vector3.zero;
		}
	}

	public void CancelLockAt()
	{
		_lockedPoint = false;
		_lockedTransform = null;
		_beginAngles = NormalizeAngles(_beginAngles + _changedAngles);
		_changedAngles = Vector3.zero;
	}

	private void LateUpdate()
	{
		if (_currentState == null || toState == null)
		{
			return;
		}
		if (_changeStateProgress >= 0f)
		{
			float num = _currentState.changeSmooth * Time.smoothDeltaTime;
			_changeStateProgress = Mathf.Lerp(_changeStateProgress, 1f, num);
			if (_changeStateProgress > 0.98f)
			{
				_currentState.CopyState(toState);
				_changeStateProgress = -1f;
				StopLockPoint();
			}
			else
			{
				_currentState.Slerp(toState, num);
			}
		}
		UpdateShake();
		switch (_currentState.cameraMode)
		{
		case TPCameraMode.FreeDirectional:
			CameraMovement();
			break;
		case TPCameraMode.FixedAngle:
			CameraMovement();
			break;
		}
	}

	public void SetMainTarget(Transform newTarget, string stateName = null)
	{
		target = newTarget;
		if (!string.IsNullOrEmpty(stateName))
		{
			ChangeState(target, stateName, cameraDefaultSmooth);
		}
	}

	public void SetLockedTarget(Transform newTarget, string stateName = null, bool needSmooth = true)
	{
		_tmpState = toState;
		if (!string.IsNullOrEmpty(stateName))
		{
			ChangeState(newTarget, stateName, (!needSmooth) ? 0f : cameraDefaultSmooth);
		}
		_statelocked = true;
	}

	public void Unlock()
	{
		_statelocked = false;
		if (_tmpState != null)
		{
			DoChangeState(_tmpState);
			_tmpState = null;
		}
	}

	public Ray ScreenPointToRay(Vector3 Point)
	{
		return _camera.ScreenPointToRay(Point);
	}

	public bool ChangeState(string stateName, bool smoothChange = true)
	{
		return ChangeState(target, stateName, (!smoothChange) ? 0f : cameraDefaultSmooth);
	}

	public bool ChangeState(string stateName, bool smoothChange, float changeSmooth)
	{
		return ChangeState(target, stateName, (!smoothChange) ? 0f : changeSmooth);
	}

	public bool ChangeState(Transform newTarget, string stateName, float changeSmooth = 0f)
	{
		if (toState != null && toState.target == newTarget && toState.Name == stateName)
		{
			return false;
		}
		vThirdPersonCameraState vThirdPersonCameraState2 = CameraStateList.FindState(stateName, out indexList);
		if (vThirdPersonCameraState2 == null)
		{
			Debug.LogError("Camera state " + stateName + " not exist when ChangeState");
			return false;
		}
		vThirdPersonCameraState2.target = newTarget;
		vThirdPersonCameraState2.changeSmooth = changeSmooth;
		if (_statelocked)
		{
			_tmpState = vThirdPersonCameraState2;
			return true;
		}
		DoChangeState(vThirdPersonCameraState2);
		return true;
	}

	private void DoChangeState(vThirdPersonCameraState newState)
	{
		toState = newState;
		_cullingDistance = toState.defaultDistance;
		_changeStateProgress = 0f;
		_camera.nearClipPlane = toState.NearClippingPlane;
		_camera.farClipPlane = toState.FarClippingPlane;
		if (toState.changeSmooth > 0f)
		{
			if (toState.target != null && _currentState.target != null)
			{
				_currentState.lookAtPosition = toState.target.InverseTransformPoint(_currentState.target.TransformPoint(_currentState.lookAtPosition));
			}
			_currentState.target = toState.target;
		}
		else
		{
			_currentState.CopyState(toState);
		}
	}

	public void StartDragCamera()
	{
		dragging = true;
		if (!_lockedPoint)
		{
			_beginAngles = NormalizeAngles(_targetLookAt.eulerAngles - offsetAngles);
			_changedAngles = Vector3.zero;
		}
	}

	public void StopDragCamera()
	{
		dragging = false;
	}

	public void DragCamera(float x, float y)
	{
		dragging = true;
		if (_currentState.dragByAngle)
		{
			RotateCameraByAngle(x, y);
		}
		else
		{
			RotateCameraByPixel(x, y);
		}
	}

	public void RotateCameraByAngle(float x, float y)
	{
		if (_currentState.cameraMode != TPCameraMode.FixedAngle)
		{
			Vector3 zero = Vector3.zero;
			zero.y = x * _currentState.xMouseSensitivity * mouseSensitivityFactor.x;
			zero.x = (0f - y) * _currentState.yMouseSensitivity * mouseSensitivityFactor.y;
			_changedAngles = NormalizeAngles(_changedAngles + zero);
		}
	}

	public void RotateCameraByPixel(float x, float y)
	{
		if (_currentState.cameraMode != TPCameraMode.FixedAngle && (!(Math.Abs(x) < 0.001f) || !(Math.Abs(y) < 0.001f)))
		{
			x *= _currentState.xMouseSensitivity * mouseSensitivityFactor.x;
			y *= _currentState.yMouseSensitivity * mouseSensitivityFactor.y;
			m_ToRayVec2.Set((float)_camera.pixelWidth * 0.5f + x, (float)_camera.pixelHeight * 0.5f + y);
			if (!(m_ToRayVec2.x < 0f) && !(m_ToRayVec2.x >= (float)Screen.width) && !(m_ToRayVec2.y < 0f) && !(m_ToRayVec2.y >= (float)Screen.height))
			{
				_changedAngles = NormalizeAngles(Quaternion.LookRotation(_camera.ScreenPointToRay(m_ToRayVec2).direction).eulerAngles - _targetLookAt.eulerAngles + _changedAngles);
			}
		}
	}

	public void AttractCamera(Vector3 attractPosition, float force)
	{
		_changedAngles = NormalizeAngles(NormalizeAngles(Quaternion.LookRotation(attractPosition - _targetLookAt.position).eulerAngles - _targetLookAt.eulerAngles) * Mathf.Min(force, 1f) + _changedAngles);
	}

	public Vector3 GetBaseDirection()
	{
		Vector3 euler = ClampTargetAngles(_beginAngles + _changedAngles);
		return Quaternion.Euler(euler) * Vector3.forward;
	}

	public Transform GetTargetLookAtTransform()
	{
		return _targetLookAt;
	}

	private Vector3 NormalizeDirectionAngles(Vector3 direction)
	{
		Vector3 eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
		return NormalizeAngles(eulerAngles);
	}

	private Vector3 ClampTargetAngles(Vector3 angles)
	{
		angles.x = vExtensions.ClampAngle2(angles.x, _currentState.yMinLimit, _currentState.yMaxLimit);
		angles.y = vExtensions.ClampAngle2(angles.y, _currentState.xMinLimit, _currentState.xMaxLimit);
		angles.z = 0f;
		return angles;
	}

	private Vector3 NormalizeAngles(Vector3 angles)
	{
		angles.x = vExtensions.NormalizeAngle(angles.x);
		angles.y = vExtensions.NormalizeAngle(angles.y);
		angles.z = 0f;
		return angles;
	}

	private void CameraMovement()
	{
		Transform transform = _currentState.target;
		if (transform == null)
		{
			return;
		}
		Vector3 vector = transform.TransformPoint(_currentState.lookAtPosition);
		Vector3 vector2 = vector - _targetLookAt.position;
		_targetLookAt.position = vector;
		if (_lockedPoint)
		{
			if (!_lockedTransform)
			{
				_beginAngles = NormalizeAngles(Quaternion.LookRotation(_lockedPosition - _targetLookAt.position).eulerAngles);
			}
			else
			{
				_beginAngles = NormalizeAngles(Quaternion.LookRotation(_lockedTransform.TransformPoint(_lockedPosition) - _targetLookAt.position).eulerAngles);
			}
		}
		else if (!dragging && (toState.followDirectionX || toState.followDirectionY))
		{
			Vector3 eulerAngles = transform.eulerAngles;
			if (toState.followDirectionX)
			{
				_beginAngles.x = eulerAngles.x;
			}
			if (toState.followDirectionY)
			{
				_beginAngles.y = eulerAngles.y;
			}
			_beginAngles = NormalizeAngles(Quaternion.Lerp(_targetLookAt.rotation, Quaternion.Euler(_beginAngles), toState.followSmooth * Time.deltaTime).eulerAngles);
			_changedAngles = Vector3.zero;
		}
		Vector3 euler = ClampTargetAngles(_beginAngles + _changedAngles + offsetAngles);
		_targetLookAt.rotation = Quaternion.Euler(euler);
		Vector3 forward = _targetLookAt.forward;
		float num = _currentState.defaultDistance - shakeDistance * 51f / _currentState.fov;
		float num2 = num;
		if (CullingRayCast2(vector + forward * 0.2f, -forward, num2 + 0.2f, cullingLayer))
		{
			num2 = Mathf.Clamp(_cullingDistance, 0f, num2);
			distanceChanged = true;
		}
		if (distanceChanged)
		{
			float num3 = Vector3.Distance(SelfTransform.position + vector2, vector);
			if (num2 - num3 > 0.01f)
			{
				num2 = Mathf.MoveTowards(num3, num2, Time.deltaTime * 2f);
			}
			else if (num3 > num || num - num3 < 0.01f)
			{
				distanceChanged = false;
			}
		}
		SelfTransform.position = vector - forward * num2;
		SelfTransform.rotation = _targetLookAt.rotation;
		_camera.fieldOfView = _currentState.fov;
		if ((bool)ChildCamera)
		{
			ChildCamera.fieldOfView = _currentState.fov;
		}
		if ((bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer)
		{
			Battle.Ins.SelfPlayer.FollowCamearRotation();
		}
	}

	private void UpdateNearClipPlaneHalfSize()
	{
		if (!(Math.Abs(m_LastFov - _camera.fieldOfView) < 0.001f))
		{
			m_LastFov = _camera.fieldOfView;
			float f = m_LastFov * 0.5f * ((float)Math.PI / 180f);
			float aspect = _camera.aspect;
			float nearClipPlane = _camera.nearClipPlane;
			m_NearPlaneHalfSize.y = nearClipPlane * Mathf.Tan(f) + 0.01f;
			m_NearPlaneHalfSize.x = m_NearPlaneHalfSize.y * aspect + 0.01f;
		}
	}

	private bool CullingRayCast2(Vector3 from, Vector3 direction, float distance, LayerMask testLayer)
	{
		UpdateNearClipPlaneHalfSize();
		_cullingDistance = distance;
		int num = Physics.BoxCastNonAlloc(from, m_NearPlaneHalfSize, direction, m_RaycastHit, Quaternion.LookRotation(direction), distance, testLayer, QueryTriggerInteraction.Ignore);
		if (num > 0)
		{
			_cullingDistance = m_RaycastHit[0].distance - 0.2f;
			return true;
		}
		return false;
	}

	private bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask testLayer, Color color)
	{
		bool result = false;
		if (showGizmos)
		{
			Debug.DrawRay(from, _to.LowerLeft - from, color);
			Debug.DrawLine(_to.LowerLeft, _to.LowerRight, color);
			Debug.DrawLine(_to.UpperLeft, _to.UpperRight, color);
			Debug.DrawLine(_to.UpperLeft, _to.LowerLeft, color);
			Debug.DrawLine(_to.UpperRight, _to.LowerRight, color);
			Debug.DrawRay(from, _to.LowerRight - from, color);
			Debug.DrawRay(from, _to.UpperLeft - from, color);
			Debug.DrawRay(from, _to.UpperRight - from, color);
		}
		if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, testLayer, QueryTriggerInteraction.Ignore))
		{
			result = true;
			_cullingDistance = hitInfo.distance;
		}
		if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, testLayer, QueryTriggerInteraction.Ignore))
		{
			result = true;
			if (_cullingDistance > hitInfo.distance)
			{
				_cullingDistance = hitInfo.distance;
			}
		}
		if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, testLayer, QueryTriggerInteraction.Ignore))
		{
			result = true;
			if (_cullingDistance > hitInfo.distance)
			{
				_cullingDistance = hitInfo.distance;
			}
		}
		if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, testLayer, QueryTriggerInteraction.Ignore))
		{
			result = true;
			if (_cullingDistance > hitInfo.distance)
			{
				_cullingDistance = hitInfo.distance;
			}
		}
		return result;
	}

	public void Shake()
	{
		shaking = true;
		shakeSpeed = shakeBeginSpeed;
	}

	private void UpdateShake()
	{
		if (shaking)
		{
			if (shakeDistance > shakeDistanceMax)
			{
				shakeDistance = shakeDistanceMax;
			}
			shakeDistance += shakeSpeed * Time.deltaTime;
			shakeSpeed = Mathf.Lerp(shakeSpeed, 0f, shakeSmooth * Time.deltaTime);
			shakeDistance -= shakeBackSpeed * Time.deltaTime;
			if (shakeDistance <= 0f)
			{
				shakeDistance = 0f;
				shaking = false;
			}
		}
	}
}
