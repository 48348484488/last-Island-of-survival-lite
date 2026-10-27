using System;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrackIK : MonoBehaviour
{
	public const float minAngleX = -90f;

	public const float maxAngleX = 90f;

	public const float minAngleY = -90f;

	public const float maxAngleY = 90f;

	public const float smooth = 12f;

	public const float distanceToLook = 100f;

	public Transform root;

	public Transform head;

	public List<Transform> spines;

	public bool useLimitAngle;

	public Vector2 offsetSpine = Vector2.zero;

	private float yRotation;

	private float xRotation;

	private float _currentHeadWeight;

	private float _currentbodyWeight;

	private float yAngle;

	private float xAngle;

	private float _yAngle;

	private float _xAngle;

	public void Init(Transform root, Transform head, List<Transform> spines)
	{
		this.root = root;
		this.head = head;
		this.spines = spines;
	}

	public void LookAt(Vector3 point, float headWeight, float bodyWeight)
	{
		Vector3 direction = point - head.position;
		Vector2 targetAngle = GetTargetAngle(direction);
		if (!useLimitAngle && !TargetIsOnRange(direction))
		{
			SmoothValues();
		}
		else
		{
			SmoothValues(headWeight, bodyWeight, targetAngle.x, targetAngle.y);
		}
		Quaternion quaternion = Quaternion.AngleAxis(yRotation, root.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(xRotation, root.right);
		Quaternion quaternion3 = quaternion * quaternion2;
		Vector3 vector = quaternion3 * root.forward;
		point = head.position + vector * 100f;
		SetLookAtPosition(point, _currentHeadWeight, _currentbodyWeight);
	}

	public void ResetValues()
	{
		_currentHeadWeight = 0f;
		_currentbodyWeight = 0f;
		yRotation = 0f;
		xRotation = 0f;
	}

	private void SetLookAtPosition(Vector3 point, float headWeight, float spineWeight)
	{
		Vector3 vector = Quaternion.LookRotation(point - spines[spines.Count - 1].position).eulerAngles - root.eulerAngles;
		float b = NormalizeAngle(vector.y);
		float b2 = NormalizeAngle(vector.x);
		xAngle = Mathf.Clamp(Mathf.Lerp(xAngle, b2, 12f * Time.fixedDeltaTime), -90f, 90f);
		yAngle = Mathf.Clamp(Mathf.Lerp(yAngle, b, 12f * Time.fixedDeltaTime), -90f, 90f);
		xAngle = NormalizeAngle(xAngle + Quaternion.Euler(offsetSpine).eulerAngles.x);
		yAngle = NormalizeAngle(yAngle + Quaternion.Euler(offsetSpine).eulerAngles.y);
		foreach (Transform spine in spines)
		{
			Quaternion quaternion = Quaternion.AngleAxis(xAngle * spineWeight / (float)spines.Count, spine.InverseTransformDirection(root.right));
			Quaternion quaternion2 = Quaternion.AngleAxis(yAngle * spineWeight / (float)spines.Count, spine.InverseTransformDirection(root.up));
			spine.rotation *= quaternion * quaternion2;
		}
		_yAngle = Mathf.Lerp(_yAngle, (yAngle - yAngle * spineWeight) * headWeight, 12f * Time.fixedDeltaTime);
		_xAngle = Mathf.Lerp(_xAngle, (xAngle - xAngle * spineWeight) * headWeight, 12f * Time.fixedDeltaTime);
		Quaternion quaternion3 = Quaternion.AngleAxis(_xAngle, head.InverseTransformDirection(root.right));
		Quaternion quaternion4 = Quaternion.AngleAxis(_yAngle, head.InverseTransformDirection(root.up));
		head.rotation *= quaternion3 * quaternion4;
	}

	private Vector2 GetTargetAngle(Vector3 direction)
	{
		Vector3 euler = Quaternion.LookRotation(direction, root.up).eulerAngles - root.eulerAngles;
		Quaternion quaternion = Quaternion.Euler(euler);
		float x = (float)Math.Round(NormalizeAngle(quaternion.eulerAngles.x), 2);
		float y = (float)Math.Round(NormalizeAngle(quaternion.eulerAngles.y), 2);
		return new Vector2(x, y);
	}

	private bool TargetIsOnRange(Vector3 direction)
	{
		Vector2 targetAngle = GetTargetAngle(direction);
		return targetAngle.x >= -90f && targetAngle.x <= 90f && targetAngle.y >= -90f && targetAngle.y <= 90f;
	}

	private float NormalizeAngle(float angle)
	{
		if (angle < -180f)
		{
			return angle + 360f;
		}
		if (angle > 180f)
		{
			return angle - 360f;
		}
		return angle;
	}

	private void SmoothValues(float _headWeight = 0f, float _bodyWeight = 0f, float _x = 0f, float _y = 0f)
	{
		_currentHeadWeight = Mathf.Lerp(_currentHeadWeight, _headWeight, 12f * Time.deltaTime);
		_currentbodyWeight = Mathf.Lerp(_currentbodyWeight, _bodyWeight, 12f * Time.deltaTime);
		yRotation = Mathf.Lerp(yRotation, _y, 12f * Time.deltaTime);
		xRotation = Mathf.Lerp(xRotation, _x, 12f * Time.deltaTime);
		yRotation = Mathf.Clamp(yRotation, -90f, 90f);
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
	}
}
