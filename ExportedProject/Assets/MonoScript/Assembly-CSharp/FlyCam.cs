using UnityEngine;

public class FlyCam : MonoBehaviour
{
	private int currentWaypoint;

	public float rotateSpeed = 1f;

	public float moveSpeed = 10f;

	public float magnitudeMax = 10f;

	private void Update()
	{
		if (WaypointCam.waypoints.Length <= 0)
		{
			return;
		}
		Vector3 vector = base.transform.InverseTransformPoint(new Vector3(WaypointCam.waypoints[currentWaypoint].position.x, WaypointCam.waypoints[currentWaypoint].position.y, WaypointCam.waypoints[currentWaypoint].position.z));
		Vector3 vector2 = new Vector3(WaypointCam.waypoints[currentWaypoint].position.x, WaypointCam.waypoints[currentWaypoint].position.y, WaypointCam.waypoints[currentWaypoint].position.z);
		Quaternion b = Quaternion.LookRotation(vector2 - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotateSpeed);
		Vector3 vector3 = base.transform.TransformDirection(Vector3.forward);
		base.transform.position += vector3 * moveSpeed * Time.deltaTime;
		if (vector.magnitude < magnitudeMax)
		{
			currentWaypoint++;
			if (currentWaypoint >= WaypointCam.waypoints.Length)
			{
				currentWaypoint = 0;
			}
		}
	}
}
