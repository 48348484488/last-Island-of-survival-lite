using System.Collections.Generic;
using UnityEngine;

public class UIContext : MonoBehaviour
{
	public object context;

	public Dictionary<string, object> pairs;

	public static void Attach(GameObject go, object context)
	{
		UIContext uIContext = go.GetComponent<UIContext>();
		if (uIContext == null)
		{
			uIContext = go.AddComponent<UIContext>();
		}
		uIContext.context = context;
	}

	public static void Attach(GameObject go, string name, object context)
	{
		UIContext uIContext = go.GetComponent<UIContext>();
		if (uIContext == null)
		{
			uIContext = go.AddComponent<UIContext>();
		}
		if (uIContext.pairs == null)
		{
			uIContext.pairs = new Dictionary<string, object>();
		}
		uIContext.pairs[name] = context;
	}

	public static void Dettach(GameObject go)
	{
		UIContext component = go.GetComponent<UIContext>();
		if (component != null)
		{
			object obj = component;
			Object.DestroyImmediate(component);
		}
	}

	public static object Get(GameObject go)
	{
		UIContext component = go.GetComponent<UIContext>();
		return (!(component == null)) ? component.context : null;
	}

	public static T Get<T>(GameObject go)
	{
		UIContext component = go.GetComponent<UIContext>();
		if (component != null && component.context != null)
		{
			return (T)component.context;
		}
		return default(T);
	}

	public static T Get<T>(GameObject go, string name)
	{
		UIContext component = go.GetComponent<UIContext>();
		if (component == null || component.pairs == null)
		{
			return default(T);
		}
		object value;
		if (component.pairs.TryGetValue(name, out value))
		{
			return (T)value;
		}
		return default(T);
	}
}
