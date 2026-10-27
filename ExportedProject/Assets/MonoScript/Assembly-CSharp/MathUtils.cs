using System;
using UnityEngine;

public sealed class MathUtils
{
	private const float RadToDegreeFactor = 180f / (float)Math.PI;

	public static float ClampBetween(float t, float t1, float t2)
	{
		return (!(t1 < t2)) ? Mathf.Clamp(t, t2, t1) : Mathf.Clamp(t, t1, t2);
	}

	public static float Short2Float(short v)
	{
		return (float)v * 0.1f;
	}

	public static Vector3 Short2Float(Vector3 v)
	{
		return new Vector3(v.x * 0.1f, v.y * 0.1f, v.z * 0.1f);
	}

	public static short Float2Short(float v)
	{
		return (short)(v * 10f);
	}

	public static bool isTargetArrived(Vector2 from, Vector2 target, Vector2 current)
	{
		return isTargetArrived(from.x, from.y, target.x, target.y, current.x, current.y);
	}

	public static bool isTargetArrived(float sx, float sz, float tx, float tz, float cx, float cz)
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

	public static int getSign(float x)
	{
		return (x > 0f) ? 1 : ((x < 0f) ? (-1) : 0);
	}

	public static bool IsClockwise(float angle1, float angle2)
	{
		return (!(angle2 >= angle1)) ? (angle1 - angle2 >= 180f) : (angle2 - angle1 <= 180f);
	}

	public static float GetAngleBetween(float angle1, float angle2)
	{
		float num = Mathf.Abs(angle1 - angle2);
		return (!(num > 180f)) ? num : (360f - num);
	}

	public static bool IsSameOrientation(float angle1, float angle2)
	{
		return Mathf.Abs(angle1 - angle2) < 90f;
	}

	public static float GetOrientation(float x, float y)
	{
		float num = Mathf.Atan2(y, x) * (180f / (float)Math.PI);
		return (!(num < 0f)) ? num : (360f + num);
	}

	public static float GetOrientation(Vector2 input)
	{
		return GetOrientation(input.y, input.x);
	}

	public static float GetDistancePercent(float sx, float sz, float tx, float tz, float cx, float cz)
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

	public static Vector2 rotate(Vector2 vec, float angle)
	{
		float num = (float)Math.PI / 180f * angle;
		float num2 = Mathf.Sin(0f - num);
		float num3 = Mathf.Cos(0f - num);
		return new Vector2(vec.x * num3 - vec.y * num2, vec.x * num2 + vec.y * num3);
	}

	public static int Clip(int value, int min, int max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	public static bool IsParallel(Vector2 v1, Vector2 v2)
	{
		float value = v1.y * v2.x - v1.x * v2.y;
		return Math.Abs(value) < 0.0001f;
	}

	public static float DistanceSqrMagnitude(Vector2 p1, Vector2 p2)
	{
		return (p2 - p1).sqrMagnitude;
	}

	public static short NormalizeAngle(float angle)
	{
		angle = ((angle >= 45f && angle <= 135f) ? 90f : (((angle >= 0f && angle <= 45f) || (angle >= 315f && angle <= 360f)) ? 0f : ((!(angle >= 135f) || !(angle <= 225f)) ? 270f : 180f)));
		return (short)angle;
	}

	public static bool RoughlyEquals(Vector2 vec1, Vector2 vec2, float errorValue)
	{
		return Mathf.Abs(vec1.x - vec2.x) < errorValue && Mathf.Abs(vec1.y - vec2.y) < errorValue;
	}

	public static bool RoughlyEquals(Vector3 vec1, Vector3 vec2, float errorValue)
	{
		return Mathf.Abs(vec1.x - vec2.x) < errorValue && Mathf.Abs(vec1.y - vec2.y) < errorValue && Mathf.Abs(vec1.z - vec2.z) < errorValue;
	}

	public static float Angle_360(Vector3 from, Vector3 to)
	{
		if (Vector3.Cross(from, to).y > 0f)
		{
			return Vector3.Angle(from, to);
		}
		return 360f - Vector3.Angle(from, to);
	}
}
