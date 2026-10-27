using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class UIExtension
	{
		public static bool IsCursorOverUserInterface()
		{
			if (EventSystem.current != null)
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{
					return true;
				}
				for (int i = 0; i < Input.touchCount; i++)
				{
					if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
					{
						return true;
					}
				}
			}
			return GUIUtility.hotControl != 0;
		}
	}
}
