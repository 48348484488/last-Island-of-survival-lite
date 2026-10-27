using System;
using UnityEngine;

public class BulletPreview : MonoBehaviour
{
	public Vector3 BeginPos = default(Vector3);

	public Vector3 EndPos = default(Vector3);

	public float Angle = 30f;

	public float Speed = 10f;

	private Vector2 velocity = default(Vector2);

	private float controlHeight;

	private void Start()
	{
		base.transform.position = BeginPos;
		Vector2 vector = new Vector2(EndPos.x - BeginPos.x, EndPos.z - BeginPos.z);
		if (Angle != 0f)
		{
			controlHeight = vector.magnitude * 0.5f * Mathf.Tan((float)Math.PI / 180f * Angle) + BeginPos.y;
		}
		else
		{
			controlHeight = 0f;
		}
		velocity = vector.normalized * Speed;
	}

	private void OnEnable()
	{
		Start();
	}

	private void Update()
	{
		Vector3 vector = Vector3.zero;
		Vector2 vector2 = new Vector2(base.transform.position.x, base.transform.position.z);
		vector2 += Time.deltaTime * velocity;
		if (isTargetArrived(BeginPos.x, BeginPos.z, EndPos.x, EndPos.z, vector2.x, vector2.y))
		{
			vector = EndPos;
		}
		else
		{
			float distancePercent = GetDistancePercent(BeginPos.x, BeginPos.z, EndPos.x, EndPos.z, vector2.x, vector2.y);
			vector.Set(newY: (Angle == 0f) ? ((EndPos.y - BeginPos.y) * distancePercent + BeginPos.y) : ((1f - distancePercent) * (1f - distancePercent) * BeginPos.y + 2f * distancePercent * (1f - distancePercent) * controlHeight + distancePercent * distancePercent * EndPos.y), newX: vector2.x, newZ: vector2.y);
		}
		if (vector != base.transform.position)
		{
			base.transform.rotation = Quaternion.FromToRotation(Vector3.forward, vector - base.transform.position);
		}
		base.transform.position = vector;
	}

	private static bool isTargetArrived(float sx, float sz, float tx, float tz, float cx, float cz)
	{
		if ((sx == tx && sz == tz) || (tx == cx && tz == cz))
		{
			return true;
		}
		float value = tx - sx;
		float value2 = tz - sz;
		if (Math.Abs(value) > Math.Abs(value2))
		{
			return getSign(cx - sx) * getSign(cx - tx) > 0;
		}
		return getSign(cz - sz) * getSign(cz - tz) > 0;
	}

	private static int getSign(float x)
	{
		return (x > 0f) ? 1 : ((x < 0f) ? (-1) : 0);
	}

	private static float GetDistancePercent(float sx, float sz, float tx, float tz, float cx, float cz)
	{
		if ((sx == tx && sz == tz) || (sx == cx && sz == cz))
		{
			return 0f;
		}
		float num = tx - sx;
		float num2 = tz - sz;
		if (Math.Abs(num) > Math.Abs(num2))
		{
			return (cx - sx) / num;
		}
		return (cz - sz) / num2;
	}
}
