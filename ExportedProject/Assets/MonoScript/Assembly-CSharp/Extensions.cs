using UnityEngine;

public static class Extensions
{
	private static Vector3 tmpVector3 = Vector3.zero;

	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = new Vector3(newX, t.position.y, t.position.z);
	}

	public static void SetPositionY(this Transform t, float newY)
	{
		t.position = new Vector3(t.position.x, newY, t.position.z);
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = new Vector3(t.position.x, t.position.y, newZ);
	}

	public static void SetLocalPositionX(this Transform t, float newX)
	{
		t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
	}

	public static void SetLocalPositionY(this Transform t, float newY)
	{
		t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
	}

	public static void SetLocalPositionZ(this Transform t, float newZ)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
	}

	public static void SetPosition(this Transform t, float x, float y, float z)
	{
		tmpVector3.Set(x, y, z);
		t.position = tmpVector3;
	}

	public static void SetLocalPosition(this Transform t, float x, float y, float z)
	{
		tmpVector3.Set(x, y, z);
		t.localPosition = tmpVector3;
	}

	public static void SetLocalEulerAngles(this Transform t, float x, float y, float z)
	{
		tmpVector3.Set(x, y, z);
		t.localEulerAngles = tmpVector3;
	}

	public static void SetEulerAnglesY(this Transform t, float y)
	{
		tmpVector3.Set(t.eulerAngles.x, y, t.eulerAngles.z);
		t.eulerAngles = tmpVector3;
	}

	public static void SetActiveBetter(this GameObject go, bool trueOrFalse)
	{
		if ((bool)go && trueOrFalse != go.activeSelf)
		{
			go.SetActive(trueOrFalse);
		}
	}

	public static Vector2 FixedTouchDelta(this Touch aTouch)
	{
		float num = Time.deltaTime / aTouch.deltaTime;
		if (num == 0f || float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		return aTouch.deltaPosition * num;
	}

	public static void ForceCrossFade(this Animator animator, string name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
	{
		animator.Update(0f);
		if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
		{
			animator.CrossFade(name, transitionDuration, layer, normalizedTime);
			return;
		}
		animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
		animator.Update(0f);
		animator.CrossFade(name, transitionDuration, layer, normalizedTime);
	}

	public static void ForceCrossFade(this Animator animator, int name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
	{
		animator.Update(0f);
		if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
		{
			animator.CrossFade(name, transitionDuration, layer, normalizedTime);
			return;
		}
		animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
		animator.Update(0f);
		animator.CrossFade(name, transitionDuration, layer, normalizedTime);
	}
}
