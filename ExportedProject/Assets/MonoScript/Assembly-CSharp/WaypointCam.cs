using UnityEngine;

public class WaypointCam : MonoBehaviour
{
	public Color WaypointsColor = new Color(1f, 0f, 0f, 1f);

	public bool draw = true;

	public static Transform[] waypoints;

	private void Awake()
	{
		waypoints = base.gameObject.GetComponentsInChildren<Transform>();
	}

	private void OnDrawGizmos()
	{
		if (draw)
		{
			waypoints = base.gameObject.GetComponentsInChildren<Transform>();
			Transform[] array = waypoints;
			foreach (Transform transform in array)
			{
				Gizmos.color = WaypointsColor;
				Gizmos.DrawSphere(transform.position, 1f);
				Gizmos.color = WaypointsColor;
				Gizmos.DrawWireSphere(transform.position, 6f);
			}
		}
	}
}
