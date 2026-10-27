using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using cfg;

public class Utils : MonoBehaviour
{
	public delegate void Event<T>(T arg);

	public delegate void VoidDelegate();

	public delegate void TypeDelegate(Type type);

	public delegate void SessionDelegate(Session session);

	public delegate void BoolDelegate(bool arg);

	public delegate bool ReturnBoolDelegate();

	public delegate void IntDelegate(int arg);

	public delegate void Int2Delegate(int arg1, int arg2);

	public delegate void Vector2Delegate(Vector2 vector2);

	public delegate void Vector3Delegate(Vector3 vector3);

	public delegate void FloatDelegate(float arg);

	public delegate void LongDelegate(long arg);

	public delegate void Long2Delegate(long arg1, long arg2);

	public delegate void IntLongDelegate(int arg1, long arg2);

	public delegate void StringDelegate(string arg);

	public delegate void String2Delegate(string arg1, string arg2);

	public delegate void ObjectDelegate(object arg);

	public delegate void ObjectArrayDelegate(params object[] args);

	public delegate void UnityObjectDelegate(UnityEngine.Object arg);

	public delegate void GameObjectDelegate(GameObject arg);

	public delegate void UnityObjectArrayDelegate(UnityEngine.Object[] arg);

	public delegate void DataDelegate<in T>(T arg);

	public class CustomTransform
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 localScale;
	}

	public enum PhoneLevelEnum
	{
		None = 0,
		LowEndPhone = 1,
		MiddleEndPhone = 2,
		HighEndPhone = 3
	}

	private static Utils ins;

	public static readonly System.Random mRandom = new System.Random();

	private const int SECONDS_OF_DAY = 86400;

	private const int SECONDS_OF_HOUR = 3600;

	private const int SECONDS_OF_MINUTE = 60;

	public static int ScreenWidth = 1920;

	public static int ScreenHeight = 1080;

	public static int OriginalScreenWidth = 1920;

	public static int OriginalScreenHeight = 1080;

	public static int HalfScreenWidth = ScreenWidth / 2;

	public static int HalfScreenHeight = ScreenHeight / 2;

	public static Vector2 ScreenCenter;

	private static string mPersistentDataPath = string.Empty;

	public static readonly DateTime date_1970 = new DateTime(1970, 1, 1);

	public static int DefaultLayer;

	public static int GroundLayer;

	public static PhoneLevelEnum PhoneLevel;

	private static string[] m_VivoDeviceName = new string[21]
	{
		"vivo X21", "vivo X21A", "vivo X21A", "vivo X21UD", "vivo X21UD A", "vivo Y85", "vivo Y85A", "vivo X21", "vivo X21A", "vivo X21UD",
		"vivo X21UD A", "vivo Y85", "vivo Y85A", "vivo Y83", "vivo Y83A", "vivo X21i", "vivo X21i A", "vivo Z1", "vivo Y89", "V1809A",
		"V1809T"
	};

	private static string[] m_OppoDeviceName = new string[2] { "oppo r15", "oppo paat00" };

	private static string[] m_NeedChangeDeviceName = new string[2] { "lenovo z5", "lenvovo l78011" };

	private static readonly Dictionary<float, WaitForSeconds> _dicWaits = new Dictionary<float, WaitForSeconds>();

	public static bool IsFullDisplayCreen
	{
		get
		{
			return Screen.currentResolution.height == 1080 && Screen.currentResolution.width >= 2100;
		}
	}

	public static float GetRadio()
	{
		return (float)OriginalScreenWidth * 1f / (float)OriginalScreenHeight;
	}

	public static void Init(int screenWidth, int screenHeight)
	{
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;
		HalfScreenWidth = ScreenWidth / 2;
		HalfScreenHeight = ScreenHeight / 2;
		DefaultLayer = LayerMask.NameToLayer("Default");
		GroundLayer = LayerMask.NameToLayer("Ground");
		if (string.IsNullOrEmpty(mPersistentDataPath))
		{
			mPersistentDataPath = Application.persistentDataPath;
			GameObject gameObject = new GameObject("utils");
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.AddComponent<Utils>();
			PhoneLevel = IsLowEndPhonefun();
		}
		ScreenCenter = new Vector2((float)ScreenWidth * 0.5f, (float)ScreenHeight * 0.5f);
	}

	public static void SetScreenWidthHeight(int screenWidth, int screenHeight)
	{
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;
		HalfScreenWidth = ScreenWidth / 2;
		HalfScreenHeight = ScreenHeight / 2;
		ScreenCenter = new Vector2((float)ScreenWidth * 0.5f, (float)ScreenHeight * 0.5f);
	}

	public static void SetPesistentPathFromJava()
	{
		try
		{
			string text = AndroidSDKInterface.Instance.GetStreamAssetsPath();
			Debug.LogError("java persistent path:" + text);
			if (text.Length > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			if (text != mPersistentDataPath)
			{
				mPersistentDataPath = text;
				Debug.LogError(mPersistentDataPath + " != " + text);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public static void SetCanvasMatchWidthOrHeight(GameObject go)
	{
		float radio = GetRadio();
		float num = (float)Screen.width * 1f / (float)Screen.height;
		CanvasScaler component = go.GetComponent<CanvasScaler>();
		component.matchWidthOrHeight = ((num > radio) ? 1 : 0);
	}

	public static bool TriggerEvent(VoidDelegate e)
	{
		if (e == null)
		{
			return false;
		}
		e();
		return true;
	}

	public static bool TriggerEvent(IntDelegate e, int arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(StringDelegate e, string arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(String2Delegate e, string arg1, string arg2)
	{
		if (e == null)
		{
			return false;
		}
		e(arg1, arg2);
		return true;
	}

	public static bool TriggerEvent(BoolDelegate e, bool arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(FloatDelegate e, float arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(LongDelegate e, long arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(Vector2Delegate e, Vector2 arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(Vector3Delegate e, Vector3 arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(Int2Delegate e, int arg1, int arg2)
	{
		if (e == null)
		{
			return false;
		}
		e(arg1, arg2);
		return true;
	}

	public static bool TriggerEvent(IntLongDelegate e, int arg1, long arg2)
	{
		if (e == null)
		{
			return false;
		}
		e(arg1, arg2);
		return true;
	}

	public static bool TriggerEvent(Long2Delegate e, long arg1, long arg2)
	{
		if (e == null)
		{
			return false;
		}
		e(arg1, arg2);
		return true;
	}

	public static bool TriggerEvent(GameObjectDelegate e, GameObject arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static Coroutine WaitTrue(ReturnBoolDelegate func)
	{
		return StartConroutine(DoWaitTrue(func));
	}

	private static IEnumerator DoWaitTrue(ReturnBoolDelegate func)
	{
		while (!func())
		{
			yield return null;
		}
	}

	public static bool IsParentTransform(Transform child, Transform parent)
	{
		if (child == parent)
		{
			return true;
		}
		while (child.parent != null)
		{
			if (child.parent == parent)
			{
				return true;
			}
			child = child.parent;
		}
		return false;
	}

	public static bool IsVisible(Bounds bounds, Camera camera)
	{
		Vector3 extents = bounds.extents;
		Vector3 center = bounds.center;
		if (IsVisible(bounds.min, camera))
		{
			return true;
		}
		if (IsVisible(bounds.max, camera))
		{
			return true;
		}
		Vector3 vPoint = center + new Vector3(0f - extents.x, extents.y, 0f - extents.z);
		Vector3 vPoint2 = center + new Vector3(0f - extents.x, extents.y, extents.z);
		Vector3 vPoint3 = center + new Vector3(0f - extents.x, 0f - extents.y, extents.z);
		Vector3 vPoint4 = center + new Vector3(extents.x, extents.y, 0f - extents.z);
		Vector3 vPoint5 = center + new Vector3(extents.x, 0f - extents.y, 0f - extents.z);
		Vector3 vPoint6 = center + new Vector3(extents.x, 0f - extents.y, extents.z);
		if (IsVisible(vPoint, camera))
		{
			return true;
		}
		if (IsVisible(vPoint2, camera))
		{
			return true;
		}
		if (IsVisible(vPoint3, camera))
		{
			return true;
		}
		if (IsVisible(vPoint4, camera))
		{
			return true;
		}
		if (IsVisible(vPoint5, camera))
		{
			return true;
		}
		if (IsVisible(vPoint6, camera))
		{
			return true;
		}
		return false;
	}

	public static bool IsVisible(Vector3 vPoint, Camera camera)
	{
		Vector3 vector = camera.WorldToScreenPoint(vPoint);
		float num = Screen.width;
		float num2 = Screen.height;
		if (vector.x < 0f || vector.x > num)
		{
			return false;
		}
		if (vector.y < 0f || vector.y > num2)
		{
			return false;
		}
		if (vector.z < camera.nearClipPlane || vector.z > camera.farClipPlane)
		{
			return false;
		}
		return true;
	}

	public static void Clamp<T>(ref T val, T min, T max) where T : IComparable
	{
		if (val.CompareTo(min) < 0)
		{
			val = min;
		}
		if (val.CompareTo(max) > 0)
		{
			val = max;
		}
	}

	public static T Min<T>(T a, T b) where T : IComparable
	{
		return (a.CompareTo(b) >= 0) ? b : a;
	}

	public static T Max<T>(T a, T b) where T : IComparable
	{
		return (a.CompareTo(b) >= 0) ? a : b;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static float DistanceFromPointToLine(Vector3 vPoint, Vector3 vLinePoint, Vector3 vLineDir, out Vector3 vVertical)
	{
		vLineDir.Normalize();
		Vector3 lhs = vPoint - vLinePoint;
		float num = Vector3.Dot(lhs, vLineDir);
		vVertical = vLinePoint + vLineDir * num;
		return (vPoint - vVertical).magnitude;
	}

	public static float DistanceFromPointToLine(Vector2 vPoint, Vector2 vLinePoint, Vector2 vLineDir, out Vector2 vVertical)
	{
		vLineDir.Normalize();
		Vector2 lhs = vPoint - vLinePoint;
		float num = Vector2.Dot(lhs, vLineDir);
		vVertical = vLinePoint + vLineDir * num;
		return (vPoint - vVertical).magnitude;
	}

	public static bool LineIntersect(Vector2 vPoint1, Vector2 vDir1, Vector2 vPoint2, Vector2 vDir2, out Vector2 vIntersect)
	{
		vIntersect = Vector2.zero;
		vDir1.Normalize();
		vDir2.Normalize();
		if (vDir1 == vDir2)
		{
			return false;
		}
		Vector2 vVertical;
		float num = DistanceFromPointToLine(vPoint2, vPoint1, vDir1, out vVertical);
		Vector2 vector = vVertical - vPoint2;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num2 = Quaternion.Angle(Quaternion.LookRotation(vDir2), Quaternion.LookRotation(vector));
		if (num2 > 90f)
		{
			vDir2 = -vDir2;
		}
		float num3 = magnitude / Mathf.Cos(num2);
		vIntersect = vPoint2 + vDir2 * num3;
		return true;
	}

	public static string GetString(int id, params object[] args)
	{
		Strings strings = Strings.Get(id);
		if (strings == null)
		{
			return string.Empty;
		}
		try
		{
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = Singleton<MultiLanguageMgr>.Ins.GetLanguage(args[i].ToString());
			}
			return string.Format(Singleton<MultiLanguageMgr>.Ins.GetLanguage(strings.content), args);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return string.Empty;
		}
	}

	public static string GetString(string text, params object[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			args[i] = Singleton<MultiLanguageMgr>.Ins.GetLanguage(args[i].ToString());
		}
		return string.Format(Singleton<MultiLanguageMgr>.Ins.GetLanguage(text), args);
	}

	public static void DestroyChildren(GameObject go)
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.parent = null;
				list.Add(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (GameObject item in list)
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	public static T AddComponentIfNotExist<T>(GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<T>();
		}
		return val;
	}

	public static void AddModalCollider(GameObject go)
	{
		BoxCollider component = go.GetComponent<BoxCollider>();
		if (component == null)
		{
			component = go.AddComponent<BoxCollider>();
			component.size = new Vector3(10000f, 10000f, 0f);
			component.center = new Vector3(0f, 0f, 5f);
		}
	}

	public static string GetRelativePath(GameObject parent, GameObject child)
	{
		string text = child.name;
		Transform parent2 = child.transform.parent;
		while (parent2 != null && parent2 != parent.transform)
		{
			text = parent2.name + "/" + text;
			parent2 = parent2.parent;
		}
		return text;
	}

	public static Coroutine StartConroutine(IEnumerator routine)
	{
		return ins.StartCoroutine(routine);
	}

	public static void StopConroutine(Coroutine coroutine)
	{
		if (coroutine != null)
		{
			ins.StopCoroutine(coroutine);
		}
	}

	public static void StopConroutine(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			ins.StopCoroutine(name);
		}
	}

	private void Awake()
	{
		ins = this;
	}

	public static Quaternion LookRotation(Vector3 forward, Quaternion defaultRotation)
	{
		if (forward.sqrMagnitude > 0.001f)
		{
			return Quaternion.LookRotation(forward);
		}
		return defaultRotation;
	}

	public static Vector3 GetScreenPosition(Vector3 vWorldPos, Camera worldCamera)
	{
		Vector3 position = worldCamera.WorldToScreenPoint(vWorldPos);
		return Camera.main.ScreenToWorldPoint(position);
	}

	public static float GetEffectDuration(GameObject effect, bool ignoreloop = true)
	{
		float num = 0f;
		ParticleSystem component = effect.GetComponent<ParticleSystem>();
		if ((bool)component)
		{
			if (component.loop && !ignoreloop)
			{
				return float.PositiveInfinity;
			}
			float num2 = component.startLifetime + component.startDelay;
			if (num2 > num)
			{
				num = num2;
			}
			if (num < component.duration)
			{
				num = component.duration;
			}
		}
		Component[] componentsInChildren = effect.GetComponentsInChildren(typeof(ParticleSystem), true);
		Component[] array = componentsInChildren;
		foreach (Component component2 in array)
		{
			ParticleSystem particleSystem = component2 as ParticleSystem;
			if ((bool)particleSystem)
			{
				if (particleSystem.loop && !ignoreloop)
				{
					return float.PositiveInfinity;
				}
				float num3 = particleSystem.startLifetime + particleSystem.startDelay;
				if (num3 > num)
				{
					num = num3;
				}
				if (num < particleSystem.duration)
				{
					num = particleSystem.duration;
				}
			}
		}
		return num;
	}

	public static string GetTimeString(int iTimeInSecond)
	{
		if (iTimeInSecond <= 0)
		{
			return "00:00:00";
		}
		int num = iTimeInSecond / 3600;
		int num2 = (iTimeInSecond - num * 3600) / 60;
		int num3 = iTimeInSecond % 60;
		return ((num >= 10) ? string.Empty : "0") + num + ":" + ((num2 >= 10) ? string.Empty : "0") + num2 + ":" + ((num3 >= 10) ? string.Empty : "0") + num3;
	}

	public static string GetOffTimeString(int iTimeInSecond)
	{
		if (iTimeInSecond <= 0)
		{
			return GetString(128);
		}
		int num = iTimeInSecond / 3600;
		if (num >= 24)
		{
			int num2 = num / 24;
			if (num2 > 0)
			{
				return GetString(125, num2);
			}
		}
		int num3 = (iTimeInSecond - num * 3600) / 60;
		if (num > 0 && num3 > 0)
		{
			return GetString(126, num) + GetString(127, num3);
		}
		if (num > 0 && num3 == 0)
		{
			return GetString(126, num);
		}
		if (num == 0 && num3 > 0)
		{
			return GetString(127, num3);
		}
		return string.Empty;
	}

	public static string GetUseTimeString(int nSecond)
	{
		if (nSecond == 0)
		{
			return GetString(89);
		}
		int num = nSecond / 3600;
		if (num >= 24)
		{
			num /= 24;
			return GetString(125, num);
		}
		return GetString(126, num);
	}

	public static string GetSurplusTimeString(int iTimeInSecond)
	{
		int num = iTimeInSecond / 3600;
		if (num > 24)
		{
			int num2 = num / 24;
			if (num2 > 0)
			{
				return GetString(167, num2);
			}
		}
		int num3 = (iTimeInSecond - num * 3600) / 60;
		if (num > 0 && num3 > 0)
		{
			return GetString(18, num) + GetString(19, num3);
		}
		if (num > 0 && num3 == 0)
		{
			return GetString(18, num);
		}
		if (num == 0 && num3 > 0)
		{
			return GetString(19, num3);
		}
		return GetString(19, 0);
	}

	public static string GetLiveTimeString(int iTimeInSecond)
	{
		int num = iTimeInSecond / 60;
		int num2 = iTimeInSecond % 60;
		return GetString(139, num) + GetString(140, num2);
	}

	public static string GetNoHourTimeString(int iTimeInSecond)
	{
		if (iTimeInSecond <= 0)
		{
			return "00:00";
		}
		int num = iTimeInSecond / 60;
		int num2 = iTimeInSecond % 60;
		return ((num >= 10) ? string.Empty : "0") + num + ":" + ((num2 >= 10) ? string.Empty : "0") + num2;
	}

	public static string GetCountDownTime(int iTimeInSecond)
	{
		int num = iTimeInSecond / 3600;
		int num2 = (iTimeInSecond - num * 3600) / 60;
		if (num > 0 && num2 > 0)
		{
			return GetString(18, num) + GetString(19, num2);
		}
		if (num > 0 && num2 == 0)
		{
			return GetString(18, num);
		}
		int num3 = (iTimeInSecond - num * 3600) % 60;
		if (num2 > 0 && num3 > 0)
		{
			return GetString(19, num2) + GetString(20, num3);
		}
		if (num2 > 0 && num3 == 0)
		{
			return GetString(19, num2);
		}
		if (num3 > 0)
		{
			return GetString(20, num3);
		}
		return GetString(20, 0);
	}

	public static IEnumerator CastGameObject(GameObject go, float initScale, float initAlpha, float duration)
	{
		Vector3 oriScale = go.transform.localScale;
		float curScale = initScale;
		float scaleSpeed = (initScale - 1f) / duration;
		float lastTime = Time.time;
		while (duration > 0f)
		{
			float curTime = Time.time;
			float deltaTime = curTime - lastTime;
			lastTime = curTime;
			duration -= deltaTime;
			curScale -= deltaTime * scaleSpeed;
			if (scaleSpeed > 0f && curScale < 1f)
			{
				curScale = 1f;
			}
			if (scaleSpeed < 0f && curScale > 1f)
			{
				curScale = 1f;
			}
			go.transform.localScale = oriScale * curScale;
			yield return true;
		}
		go.transform.localScale = oriScale;
	}

	public static IEnumerator Vibration(GameObject go, float duration, float radiusX, float radiusY, float radiusZ)
	{
		Vector3 initPos = go.transform.localPosition;
		float time = Time.realtimeSinceStartup;
		while (duration > 0f)
		{
			float deltaTime = Time.realtimeSinceStartup - time;
			time = Time.realtimeSinceStartup;
			duration -= deltaTime;
			go.transform.localPosition = initPos + new Vector3(UnityEngine.Random.Range(0f - radiusX, radiusX), UnityEngine.Random.Range(0f - radiusY, radiusY), UnityEngine.Random.Range(0f - radiusZ, radiusZ));
			yield return new WaitForSeconds(0.01f);
		}
		go.transform.localPosition = initPos;
	}

	public static IEnumerator ScalePingpong(GameObject go, int times, float once_duration, float minScale, float maxScale, float attenuation)
	{
		Vector3 initScale = go.transform.localScale;
		if (minScale == maxScale)
		{
			yield break;
		}
		if (maxScale < minScale)
		{
			Swap(ref minScale, ref maxScale);
		}
		if (minScale < 0f)
		{
			minScale = 0f;
		}
		if (maxScale < 1f)
		{
			maxScale = 1f;
		}
		if (minScale > 1f)
		{
			minScale = 1f;
		}
		float speed = (maxScale - minScale) * 2f / once_duration;
		float curScale = 1f;
		while (times > 0)
		{
			while (curScale > minScale)
			{
				curScale -= Time.deltaTime * speed;
				Clamp(ref curScale, minScale, maxScale);
				go.transform.localScale = initScale * curScale;
				yield return true;
			}
			while (curScale < maxScale)
			{
				curScale += Time.deltaTime * speed;
				Clamp(ref curScale, minScale, maxScale);
				go.transform.localScale = initScale * curScale;
				yield return true;
			}
			if (minScale < 1f)
			{
				minScale = 1f - (1f - minScale) * attenuation;
			}
			if (maxScale > 1f)
			{
				maxScale *= attenuation;
			}
			speed = (maxScale - minScale) * 2f / once_duration;
			times--;
		}
		go.transform.localScale = initScale;
	}

	public static IEnumerator RotatePingpong(GameObject go, int times, float once_duration, float angle, float attenuation, Vector3 forward)
	{
		if (angle == 0f)
		{
			yield break;
		}
		Quaternion initRotation = go.transform.rotation;
		if (Math.Abs(angle) > 360f)
		{
			angle -= (float)((int)(angle / 360f) * 360);
		}
		float minAngle = 0f - angle;
		float maxAngle = angle;
		if (minAngle > maxAngle)
		{
			Swap(ref minAngle, ref maxAngle);
		}
		float speed = angle * 2f * 2f / once_duration;
		float curAngle = 0f;
		while (times > 0)
		{
			while (curAngle > minAngle)
			{
				curAngle -= Time.deltaTime * speed;
				if (curAngle < minAngle)
				{
					curAngle = minAngle;
				}
				go.transform.rotation = initRotation * (Quaternion.Euler(0f, 0f, curAngle) * Quaternion.LookRotation(forward));
				yield return true;
			}
			while (curAngle < maxAngle)
			{
				curAngle += Time.deltaTime * speed;
				if (curAngle > maxAngle)
				{
					curAngle = maxAngle;
				}
				go.transform.rotation = initRotation * (Quaternion.Euler(0f, 0f, curAngle) * Quaternion.LookRotation(forward));
				yield return true;
			}
			if (minAngle < 0f)
			{
				minAngle *= attenuation;
			}
			if (maxAngle > 0f)
			{
				maxAngle *= attenuation;
			}
			speed = (maxAngle - minAngle) * 2f / once_duration;
			times--;
		}
		go.transform.rotation = initRotation;
	}

	public static int Random(int min, int max)
	{
		return mRandom.Next(min, max);
	}

	public static float Random(float min, float max)
	{
		return (float)(mRandom.NextDouble() * (double)(max - min)) + min;
	}

	public static IEnumerator Flicker(GameObject[] gameObjects, float duration)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(duration);
		foreach (GameObject gameObject2 in gameObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	public static string ConvertChatJavaTime(int javaTime)
	{
		long num = (long)javaTime * 1000L;
		long ticks = date_1970.Ticks;
		long ticks2 = ticks + num * 10000;
		DateTime dateTime = new DateTime(ticks2).ToLocalTime();
		DateTime now = DateTime.Now;
		if (dateTime.Year == now.Year && dateTime.Month == now.Month && dateTime.Day == now.Day)
		{
			return string.Format("{0:HH:mm}", dateTime);
		}
		return string.Format("{0:MM/dd HH:mm}", dateTime);
	}

	public static string CountDownTime(int javaTime)
	{
		string text = (javaTime / 3600).ToString();
		javaTime %= 3600;
		string text2 = (javaTime / 60).ToString();
		javaTime %= 60;
		string text3 = javaTime.ToString();
		string arg = ((text.Length <= 1) ? ("0" + text) : text);
		string arg2 = ((text2.Length <= 1) ? ("0" + text2) : text2);
		string arg3 = ((text3.Length <= 1) ? ("0" + text3) : text3);
		return string.Format("{0}:{1}:{2}", arg, arg2, arg3);
	}

	public static string ConvertJavaTime(int javaTime)
	{
		long num = (long)javaTime * 1000L;
		long ticks = date_1970.Ticks;
		long ticks2 = ticks + num * 10000;
		DateTime dateTime = new DateTime(ticks2).ToLocalTime();
		return string.Format("{0:yyyy/MM/dd HH:mm}", dateTime);
	}

	public static string ConvertJavaTimeDay(int javaTime)
	{
		long num = (long)javaTime * 1000L;
		long ticks = date_1970.Ticks;
		long ticks2 = ticks + num * 10000;
		DateTime dateTime = new DateTime(ticks2).ToLocalTime();
		return string.Format("{0:yyyy/MM/dd}", dateTime);
	}

	public static string ConvertJavaTime(int javaTime, string format)
	{
		long num = (long)javaTime * 1000L;
		long ticks = date_1970.Ticks;
		long ticks2 = ticks + num * 10000;
		DateTime dateTime = new DateTime(ticks2).ToLocalTime();
		return string.Format("{0:" + format + "}", dateTime);
	}

	public static long GetTicksFromJavaTime(long javaTimeInMilliseconds)
	{
		long ticks = date_1970.Ticks;
		return ticks + javaTimeInMilliseconds * 10000;
	}

	public static void ResetEffect(GameObject go)
	{
		ParticleSystem component = go.GetComponent<ParticleSystem>();
		if ((bool)component)
		{
			component.time = 0f;
			component.Clear(true);
			component.Stop(true);
		}
		Component[] componentsInChildren = go.GetComponentsInChildren(typeof(ParticleSystem), true);
		Component[] array = componentsInChildren;
		foreach (Component component2 in array)
		{
			ParticleSystem particleSystem = component2 as ParticleSystem;
			particleSystem.Clear(true);
			particleSystem.time = 0f;
			particleSystem.Stop(true);
		}
		go.SetActive(false);
	}

	public static IEnumerator FlickEffect(GameObject[] gameObjects, float duration)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(duration);
		foreach (GameObject gameObject2 in gameObjects)
		{
			ResetEffect(gameObject2);
			gameObject2.SetActive(false);
		}
	}

	public static IEnumerator FlickGameObject(GameObject[] gameObjects, float duration)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(duration);
		foreach (GameObject gameObject2 in gameObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	public static void Flip(GameObject go, bool keepXPositive, bool keepYPositive, bool keepZPositive)
	{
		Vector3 localScale = go.transform.localScale;
		if (keepXPositive == localScale.x < 0f)
		{
			localScale.x = 0f - localScale.x;
		}
		if (keepYPositive == localScale.y < 0f)
		{
			localScale.y = 0f - localScale.y;
		}
		if (keepZPositive == localScale.z < 0f)
		{
			localScale.z = 0f - localScale.z;
		}
		go.transform.localScale = localScale;
	}

	public static void SetVolume(float volume)
	{
		AudioListener.volume = volume;
	}

	public static void EnableComponent<T>(GameObject go, bool enabled) where T : Behaviour
	{
		if ((bool)go)
		{
			T component = go.GetComponent<T>();
			if ((bool)(UnityEngine.Object)component)
			{
				component.enabled = enabled;
			}
		}
	}

	public static IEnumerator HidePopupWindow(GameObject go)
	{
		Vector3 oriScale = go.transform.localScale;
		yield return new WaitForSeconds(0.13f);
		yield return new WaitForSeconds(0.13f);
		go.transform.localScale = oriScale;
		go.SetActive(false);
	}

	public static IEnumerator ShowPopupWindow(GameObject go)
	{
		Vector3 oriScale = go.transform.localScale;
		go.transform.localScale = oriScale * 0.5f;
		go.SetActive(true);
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.182f);
		yield return new WaitForSeconds(0.13f);
		go.transform.localScale = oriScale;
	}

	public static IEnumerator NumberChange(int current, int target, int times, GameObject label, float duration)
	{
		int step = (target - current) / times;
		for (int i = 0; i < times + 1; i++)
		{
			current += step;
			if (step < 0 && current < target)
			{
				current = target;
			}
			if (step > 0 && current > target)
			{
				current = target;
			}
			float dura = duration - Time.deltaTime;
			if (dura < 0f)
			{
				dura = 0f;
			}
			yield return new WaitForSeconds(dura);
		}
		if (current == target)
		{
		}
	}

	public static void NormalizeInWorld(GameObject go)
	{
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
	}

	public static void NormalizeInLocal(GameObject go)
	{
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
	}

	public static void PauseEffect(GameObject effect, bool pause)
	{
		ParticleSystem[] componentsInChildren = effect.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			if (pause)
			{
				particleSystem.Pause(true);
			}
			else
			{
				particleSystem.Play(true);
			}
		}
	}

	public static string GetIconImagePath(string name)
	{
		return "res/platform/icon/" + name + ".ab";
	}

	public static string GetStreamingAssetPathForWWW(string relPath)
	{
		if (relPath.Length > 0 && relPath[0] == '/')
		{
			relPath = relPath.Substring(1);
		}
		string result = string.Empty;
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			result = "file:///" + Application.dataPath + "/StreamingAssets/" + relPath;
			break;
		case RuntimePlatform.Android:
			result = Application.streamingAssetsPath + "/" + relPath;
			break;
		case RuntimePlatform.OSXEditor:
			result = "file://" + Application.dataPath + "/StreamingAssets/" + relPath;
			break;
		case RuntimePlatform.IPhonePlayer:
			result = "file://" + Application.streamingAssetsPath + "/" + relPath;
			break;
		}
		return result;
	}

	public static string GetPersistentDataPathForWWW(string relPath)
	{
		string text = GetPersistentDataPath(relPath);
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			text = "file:///" + text;
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			text = "file://" + text;
			break;
		}
		return text;
	}

	public static string GetPersistentDataPath(string relPath)
	{
		if (Application.isMobilePlatform)
		{
			string text = null;
			if (relPath != null)
			{
				text = relPath.Substring(relPath.LastIndexOf("/") + 1);
			}
			return mPersistentDataPath + "/" + text;
		}
		return GetStreamingAssetPath(relPath);
	}

	public static string GetDownLoadPath(string relPath)
	{
		return "res" + mPersistentDataPath + "/" + relPath;
	}

	public static string GetPersistentPath(string relPath)
	{
		if (relPath.Length > 0 && relPath[0] == '/')
		{
			relPath = relPath.Substring(1);
		}
		return mPersistentDataPath + "/" + relPath;
	}

	public static string GetPersistentPathForWWW(string relPath)
	{
		string text = GetPersistentPath(relPath);
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			text = "file:///" + text;
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			text = "file://" + text;
			break;
		}
		return text;
	}

	public static string GetStreamingAssetPath(string relPath = null)
	{
		if (relPath.Length > 0 && relPath[0] == '/')
		{
			relPath = relPath.Substring(1);
		}
		string result = string.Empty;
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			result = Application.dataPath + "/StreamingAssets/" + relPath;
			break;
		case RuntimePlatform.Android:
			result = Application.streamingAssetsPath + "/" + relPath;
			break;
		case RuntimePlatform.OSXEditor:
			result = Application.dataPath + "/StreamingAssets/" + relPath;
			break;
		case RuntimePlatform.OSXPlayer:
			result = Application.dataPath + "/Resources/Data/StreamingAssets/" + relPath;
			break;
		case RuntimePlatform.IPhonePlayer:
			result = Application.streamingAssetsPath + "/" + relPath;
			break;
		}
		return result;
	}

	public static string GetFilePath(string relativePath)
	{
		string persistentPath = GetPersistentPath("res/" + relativePath);
		if (File.Exists(persistentPath))
		{
			return persistentPath;
		}
		if (Singleton<PlatformMgr>.Ins.IsHgAndroid)
		{
			return GetObbAssetPath(relativePath);
		}
		return GetStreamingAssetPath(relativePath);
	}

	public static string GetObbAssetPath(string relPath = null)
	{
		string result = string.Empty;
		if (Singleton<PlatformMgr>.Ins.IsHgAndroid)
		{
			result = (AndroidSDKInterface.Instance.IsFullApk() ? GetStreamingAssetPath(relPath) : AndroidSDKInterface.Instance.GetObbAssetsPath(relPath));
		}
		return result;
	}

	public static string GetFilePathForWWW(string relativePath)
	{
		string relPath = "res/" + relativePath;
		string persistentPath = GetPersistentPath(relPath);
		if (File.Exists(persistentPath))
		{
			return GetPersistentPathForWWW(relPath);
		}
		return GetStreamingAssetPathForWWW(relativePath);
	}

	public static string GetClockTime(int milliseconds)
	{
		int num = milliseconds / 1000;
		int num2 = num / 3600;
		int num3 = (num - num2 * 3600) / 60;
		int num4 = num % 60;
		return ((num2 >= 10) ? string.Empty : "0") + num2 + ":" + ((num3 >= 10) ? string.Empty : "0") + num3 + ":" + ((num4 >= 10) ? string.Empty : "0") + num4;
	}

	public static string GetClockTimeNoSecond(int milliseconds)
	{
		int num = milliseconds / 1000;
		int num2 = num / 3600;
		int num3 = (num - num2 * 3600) / 60;
		return ((num2 >= 10) ? string.Empty : "0") + num2 + ":" + ((num3 >= 10) ? string.Empty : "0") + num3;
	}

	public static void ToggleOn(GameObject[] goes, int index)
	{
		for (int i = 0; i < goes.Length; i++)
		{
			goes[i].SetActive(i == index);
		}
	}

	public static void ToggleOff(GameObject[] goes, int index)
	{
		for (int i = 0; i < goes.Length; i++)
		{
			goes[i].SetActive(i != index);
		}
	}

	public static void ActiveN(GameObject[] goes, int n)
	{
		for (int i = 0; i < goes.Length; i++)
		{
			goes[i].SetActive(i < n);
		}
	}

	public static void JumpAim(Transform obj, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
	{
		obj.DOLocalJump(endValue, jumpPower, numJumps, duration, snapping);
	}

	public static bool IsLowEndProduct()
	{
		if (SystemInfo.systemMemorySize < 4096)
		{
			return true;
		}
		return false;
	}

	public static void PrinteSystemInfo()
	{
		Debug.LogError("persistentDataPath:" + Application.persistentDataPath);
		Debug.LogError("streampath:" + Application.streamingAssetsPath);
		Debug.LogError("OperatingSystem:" + SystemInfo.operatingSystem);
		Debug.LogError("SystemMemorySize:" + SystemInfo.systemMemorySize);
		Debug.LogError("ProcessorCount:" + SystemInfo.processorCount);
		Debug.LogError("ProcessorType:" + SystemInfo.processorType);
		Debug.LogError("supportsInstancing:" + SystemInfo.supportsInstancing);
		Debug.LogError("graphicsShaderLevel:" + SystemInfo.graphicsShaderLevel);
		Debug.LogError("Open Gl::" + SystemInfo.graphicsDeviceVersion);
	}

	public static string GetAndroidId()
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
			return androidJavaClass2.CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return string.Empty;
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null)
		{
			return (T)null;
		}
		T val = (T)null;
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			Transform parent = go.transform.parent;
			while (parent != null && (UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = parent.gameObject.GetComponent<T>();
				parent = parent.parent;
			}
		}
		return val;
	}

	public static string FormatDistance(float dis)
	{
		int num = (int)(dis * 1000f);
		if (num < 100)
		{
			return "<100m";
		}
		if (num < 1000)
		{
			return num + "m";
		}
		return string.Format("{0:N2}km", dis);
	}

	private static PhoneLevelEnum IsLowEndPhonefun()
	{
		int num = Mathf.Min(Screen.width, Screen.height);
		if (num < 1080)
		{
			return PhoneLevelEnum.LowEndPhone;
		}
		if (SystemInfo.systemMemorySize < 2048)
		{
			return PhoneLevelEnum.MiddleEndPhone;
		}
		return PhoneLevelEnum.HighEndPhone;
	}

	public static GameObject GetChildObjByName(string name, GameObject go)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == name)
			{
				return componentsInChildren[i].gameObject;
			}
		}
		return null;
	}

	public static void HideAllChild(Transform t)
	{
		Transform[] componentsInChildren = t.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(false);
		}
	}

	public static bool IsIphoneX()
	{
		string deviceModel = SystemInfo.deviceModel;
		if (deviceModel == "iPhone10,3" || deviceModel == "iPhone10,6")
		{
			return true;
		}
		return false;
	}

	public static bool IsVivo()
	{
		string deviceModel = SystemInfo.deviceModel;
		string[] vivoDeviceName = m_VivoDeviceName;
		foreach (string value in vivoDeviceName)
		{
			if (deviceModel.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsOppo()
	{
		string text = SystemInfo.deviceModel.ToLower();
		string[] oppoDeviceName = m_OppoDeviceName;
		foreach (string value in oppoDeviceName)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsNeedChange()
	{
		string text = SystemInfo.deviceModel.ToLower();
		string[] oppoDeviceName = m_OppoDeviceName;
		foreach (string value in oppoDeviceName)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	public static Rect GetGameobjectScreenRect(GameObject go)
	{
		Vector3 vector = ViewMgr.Ins.UICamera.WorldToScreenPoint(go.transform.position);
		RectTransform rectTransform = go.transform as RectTransform;
		Rect result = new Rect(rectTransform.rect);
		result.x = vector.x - result.width * rectTransform.pivot.x;
		result.y = vector.y - result.height * rectTransform.pivot.y;
		return result;
	}

	public static bool CanSee(Vector3 from, Vector3 to)
	{
		if (Physics.Raycast(from + Vector3.up * 0.1f, to - from, Vector3.Distance(from, to) - 0.5f, (1 << GroundLayer) | (1 << DefaultLayer), QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		return true;
	}

	public bool CanSeeObj(GameObject from, GameObject to, float fromExtraY = 0f, float toExtraY = 0f)
	{
		RaycastHit hitInfo;
		if (Physics.Linecast(from.transform.position + Vector3.up * fromExtraY, to.transform.position + Vector3.up * toExtraY, out hitInfo, -1, QueryTriggerInteraction.Ignore) && hitInfo.collider.gameObject != from && hitInfo.collider.gameObject != to)
		{
			return false;
		}
		return true;
	}

	public static WaitForSeconds WaitForSeconds(float waitTime)
	{
		WaitForSeconds value;
		if (!_dicWaits.TryGetValue(waitTime, out value))
		{
			value = new WaitForSeconds(waitTime);
			_dicWaits[waitTime] = value;
		}
		return value;
	}

	public static IEnumerator YieldAniFinish(Animator ani, string aniName, UnityAction action, int layer = 1, float spaceTime = 0.1f)
	{
		AnimatorStateInfo stateinfo;
		do
		{
			yield return WaitForSeconds(spaceTime);
			if (!ani)
			{
				yield break;
			}
			stateinfo = ani.GetCurrentAnimatorStateInfo(layer);
		}
		while (!stateinfo.IsName(aniName) || !(stateinfo.normalizedTime > 1f));
		action();
	}
}
