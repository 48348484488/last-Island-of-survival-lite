using UnityEngine;

public class Vehicle3rdSyncer : MonoBehaviour
{
	private Transform mTransform;

	private Vector3 m_Velocity = Vector3.zero;

	private Vector3 m_TargetPos = Vector3.zero;

	private float m_Speed;

	private Quaternion m_TargetRotation = Quaternion.identity;

	private bool m_Rotating;

	private bool m_Moving;

	private void Awake()
	{
		mTransform = base.transform;
		m_Moving = false;
		m_Rotating = false;
		m_Speed = 0f;
		m_TargetPos = mTransform.position;
	}

	private void Update()
	{
		UpdateMove();
	}

	public void SetPos(float x, float y, float z)
	{
		m_TargetPos.Set(x, y, z);
		m_Moving = true;
	}

	public void SetVelocity(float x, float y, float z)
	{
		m_Velocity.Set(x, y, z);
	}

	public void SetOrientation(float x, float y, float z)
	{
		m_TargetRotation = Quaternion.Euler(x, y, z);
		m_Rotating = true;
	}

	public float GetSpeed()
	{
		return m_Speed * 3.6f;
	}

	private void UpdateMove()
	{
		if (m_Moving)
		{
			Vector3 position = mTransform.position;
			Vector3 vector = Vector3.SmoothDamp(position, m_TargetPos, ref m_Velocity, 0.3f);
			m_Speed = m_Velocity.magnitude;
			mTransform.position = vector;
			m_Moving = m_TargetPos != vector;
			if (!m_Moving)
			{
				m_Velocity = Vector3.zero;
				m_Speed = 0f;
			}
		}
		if (m_Rotating)
		{
			Quaternion quaternion = Quaternion.Lerp(mTransform.rotation, m_TargetRotation, 5f * Time.deltaTime);
			mTransform.rotation = quaternion;
			m_Rotating = quaternion != m_TargetRotation;
		}
	}
}
