using UnityEngine;

public class Bullet3rdControl : MonoBehaviour
{
	private static readonly Color DebugColor = Color.green;

	private static readonly bool IsDebug = true;

	private RaycastHit hitInfo;

	private Vector3 m_PreviousPosition;

	public float Speed;

	public Vector3 dir;

	private void Update()
	{
		if (IsDebug)
		{
			Debug.DrawLine(base.transform.position, m_PreviousPosition, DebugColor, 3f);
		}
		base.transform.Translate(dir * Speed * Time.deltaTime, Space.World);
		if (Physics.Linecast(m_PreviousPosition, base.transform.position, out hitInfo, 1847363585))
		{
			Battle.Ins.Bullet3rdPool.Recycle(this);
		}
		m_PreviousPosition = base.transform.position;
	}

	public void SetPos(Vector3 p)
	{
		m_PreviousPosition = p;
		base.transform.position = p;
	}
}
