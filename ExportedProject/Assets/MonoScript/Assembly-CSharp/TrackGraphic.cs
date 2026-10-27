using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGraphic : MonoBehaviour
{
	public float Interval = 0.05f;

	public int PointsCount = 100;

	public Vector3 velocity;

	private Vector3 pos = Vector3.zero;

	public LayerMask layer;

	public GameObject Yuanpan;

	private int RayCastSimplify = 2;

	private List<Vector3> Points = new List<Vector3>();

	private WaitForSeconds waittime = new WaitForSeconds(0.03f);

	private LineRenderer m_lineRender;

	private void Start()
	{
		Yuanpan.SetActive(false);
		StartCoroutine(UpdateDate());
		m_lineRender = GetComponent<LineRenderer>();
		m_lineRender.enabled = false;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private IEnumerator UpdateDate()
	{
		yield return new WaitForSeconds(0.6f);
		m_lineRender.enabled = true;
		while (true)
		{
			yield return waittime;
			Vector3 dir = Battle.Ins.MainCamera.transform.TransformDirection(new Vector3(0f, 0.5f, 1f)).normalized;
			base.transform.position = Battle.Ins.SelfPlayer.RightHandGuaDian.transform.position;
			base.transform.eulerAngles = Vector3.zero;
			velocity = dir * 15f;
			Points.Clear();
			pos = Vector3.zero;
			for (int i = 0; i < PointsCount; i++)
			{
				Points.Add(pos);
				m_lineRender.positionCount = Points.Count;
				m_lineRender.SetPosition(i, pos);
				if (i != 0 && i % RayCastSimplify == 0)
				{
					Vector3 vector = pos - Points[i - RayCastSimplify];
					RaycastHit hitInfo;
					if (Physics.Raycast(base.transform.position + Points[i - RayCastSimplify], vector.normalized, out hitInfo, vector.magnitude, layer.value))
					{
						Vector3 endPos = hitInfo.point;
						Yuanpan.SetActiveBetter(true);
						Yuanpan.transform.position = endPos;
						Yuanpan.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
						break;
					}
				}
				velocity += Vector3.down * 9.8f * Interval;
				pos += velocity * Interval;
			}
		}
	}
}
