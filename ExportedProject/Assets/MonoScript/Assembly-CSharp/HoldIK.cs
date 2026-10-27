using UnityEngine;

public class HoldIK : MonoBehaviour
{
	public Transform rootBone;

	public Transform middleBone;

	public Transform middleBone2;

	public Transform endBone;

	[SerializeField]
	private float m_rootBoneLength;

	[SerializeField]
	private float m_middleBoneLength;

	private float m_weight;

	private Vector3 m_hintPosition;

	public float IKWeight
	{
		get
		{
			return m_weight;
		}
	}

	public void Init(Transform rootBone, Transform middleBone, Transform middleBone2, Transform endBone)
	{
		this.rootBone = rootBone;
		this.middleBone = middleBone;
		this.middleBone2 = middleBone2;
		this.endBone = endBone;
		m_rootBoneLength = (middleBone.position - rootBone.position).magnitude;
		m_middleBoneLength = (endBone.position - middleBone.position).magnitude;
	}

	public virtual void SetIKWeight(float weight)
	{
		m_weight = weight;
	}

	public virtual void SetIKPosition(Vector3 ikPosition)
	{
		if (!(m_weight <= 0f))
		{
			Vector3 middleBoneDirection;
			if (m_hintPosition != Vector3.zero)
			{
				middleBoneDirection = m_hintPosition - rootBone.position;
			}
			else
			{
				Vector3 lhs = endBone.position - rootBone.position;
				middleBoneDirection = Vector3.Cross(lhs, Vector3.Cross(lhs, endBone.position - middleBone.position));
			}
			Vector3 hintPosition = GetHintPosition(rootBone.position, ikPosition, middleBoneDirection);
			Quaternion b = Quaternion.FromToRotation(middleBone.position - rootBone.position, hintPosition - rootBone.position) * rootBone.rotation;
			if (!float.IsNaN(b.x) && !float.IsNaN(b.y) && !float.IsNaN(b.z))
			{
				rootBone.rotation = Quaternion.Slerp(rootBone.rotation, b, m_weight);
				Quaternion b2 = Quaternion.FromToRotation(endBone.position - middleBone.position, ikPosition - hintPosition) * middleBone.rotation;
				middleBone.rotation = Quaternion.Slerp(middleBone.rotation, b2, m_weight);
				middleBone2.rotation = middleBone.rotation;
			}
			m_hintPosition = Vector3.zero;
		}
	}

	public virtual void SetIKRotation(Quaternion rotation)
	{
		if (!(m_weight <= 0f))
		{
			endBone.rotation = Quaternion.Slerp(endBone.rotation, rotation, m_weight);
		}
	}

	public virtual void SetIKHintPosition(Vector3 hintPosition)
	{
		m_hintPosition = hintPosition;
	}

	private Vector3 GetHintPosition(Vector3 rootPos, Vector3 endPos, Vector3 middleBoneDirection)
	{
		Vector3 vector = endPos - rootPos;
		float num = vector.magnitude;
		float num2 = (m_rootBoneLength + m_middleBoneLength) * 0.999f;
		if (num > num2)
		{
			endPos = rootPos + vector.normalized * num2;
			vector = endPos - rootPos;
			num = num2;
		}
		float num3 = Mathf.Abs(m_rootBoneLength - m_middleBoneLength) * 1.001f;
		if (num < num3)
		{
			endPos = rootPos + vector.normalized * num3;
			vector = endPos - rootPos;
			num = num3;
		}
		float num4 = (num * num + m_rootBoneLength * m_rootBoneLength - m_middleBoneLength * m_middleBoneLength) * 0.5f / num;
		float num5 = Mathf.Sqrt(m_rootBoneLength * m_rootBoneLength - num4 * num4);
		Vector3 vector2 = Vector3.Cross(vector, Vector3.Cross(middleBoneDirection, vector));
		return rootPos + num4 * vector.normalized + num5 * vector2.normalized;
	}
}
