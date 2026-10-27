using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
	public static class ShortcutExtensions2
	{
		[CompilerGenerated]
		private sealed class _003CDOLocalluoMoveX_003Ec__AnonStorey0
		{
			internal Transform target;

			internal Vector3 _003C_003Em__0()
			{
				return target.localPosition;
			}

			internal void _003C_003Em__1(Vector3 x)
			{
				target.localPosition = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDOluoColor_003Ec__AnonStorey1
		{
			internal Material target;

			internal Color _003C_003Em__0()
			{
				return target.color;
			}

			internal void _003C_003Em__1(Color x)
			{
				target.color = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoAnchoredPosition_003Ec__AnonStorey2
		{
			internal RectTransform target;

			internal Vector2 _003C_003Em__0()
			{
				return target.anchoredPosition;
			}

			internal void _003C_003Em__1(Vector2 x)
			{
				target.anchoredPosition = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoSizeDelta_003Ec__AnonStorey3
		{
			internal RectTransform target;

			internal Vector2 _003C_003Em__0()
			{
				return target.sizeDelta;
			}

			internal void _003C_003Em__1(Vector2 x)
			{
				target.sizeDelta = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoFill_003Ec__AnonStorey4
		{
			internal Image target;

			internal float _003C_003Em__0()
			{
				return target.fillAmount;
			}

			internal void _003C_003Em__1(float x)
			{
				target.fillAmount = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoSlider_003Ec__AnonStorey5
		{
			internal Slider target;

			internal float _003C_003Em__0()
			{
				return target.value;
			}

			internal void _003C_003Em__1(float x)
			{
				target.value = x;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoWait_003Ec__AnonStorey6
		{
			internal GameObject target;

			internal float _003C_003Em__0()
			{
				return target.GameObjectTime(0f);
			}

			internal void _003C_003Em__1(float x)
			{
				target.GameObjectTime(0f);
			}
		}

		public static Tweener DOLocalluoMoveX(this Transform target, float endValue, float duration, bool snapping = false)
		{
			_003CDOLocalluoMoveX_003Ec__AnonStorey0 _003CDOLocalluoMoveX_003Ec__AnonStorey = new _003CDOLocalluoMoveX_003Ec__AnonStorey0();
			_003CDOLocalluoMoveX_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDOLocalluoMoveX_003Ec__AnonStorey._003C_003Em__0, _003CDOLocalluoMoveX_003Ec__AnonStorey._003C_003Em__1, new Vector3(endValue, 0f, 0f), duration).SetOptions(AxisConstraint.X, snapping).SetTarget(_003CDOLocalluoMoveX_003Ec__AnonStorey.target);
		}

		public static Tweener DOluoColor(this Material target, Color endValue, float duration)
		{
			_003CDOluoColor_003Ec__AnonStorey1 _003CDOluoColor_003Ec__AnonStorey = new _003CDOluoColor_003Ec__AnonStorey1();
			_003CDOluoColor_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDOluoColor_003Ec__AnonStorey._003C_003Em__0, _003CDOluoColor_003Ec__AnonStorey._003C_003Em__1, endValue, duration).SetTarget(_003CDOluoColor_003Ec__AnonStorey.target);
		}

		public static Tweener DoAnchoredPosition(this RectTransform target, Vector2 endValue, float duration)
		{
			_003CDoAnchoredPosition_003Ec__AnonStorey2 _003CDoAnchoredPosition_003Ec__AnonStorey = new _003CDoAnchoredPosition_003Ec__AnonStorey2();
			_003CDoAnchoredPosition_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDoAnchoredPosition_003Ec__AnonStorey._003C_003Em__0, _003CDoAnchoredPosition_003Ec__AnonStorey._003C_003Em__1, endValue, duration).SetTarget(_003CDoAnchoredPosition_003Ec__AnonStorey.target);
		}

		public static Tweener DoSizeDelta(this RectTransform target, Vector2 endValue, float duration)
		{
			_003CDoSizeDelta_003Ec__AnonStorey3 _003CDoSizeDelta_003Ec__AnonStorey = new _003CDoSizeDelta_003Ec__AnonStorey3();
			_003CDoSizeDelta_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDoSizeDelta_003Ec__AnonStorey._003C_003Em__0, _003CDoSizeDelta_003Ec__AnonStorey._003C_003Em__1, endValue, duration).SetTarget(_003CDoSizeDelta_003Ec__AnonStorey.target);
		}

		public static Tweener DoFill(this Image target, float endValue, float duration)
		{
			_003CDoFill_003Ec__AnonStorey4 _003CDoFill_003Ec__AnonStorey = new _003CDoFill_003Ec__AnonStorey4();
			_003CDoFill_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDoFill_003Ec__AnonStorey._003C_003Em__0, _003CDoFill_003Ec__AnonStorey._003C_003Em__1, endValue, duration).SetTarget(_003CDoFill_003Ec__AnonStorey.target);
		}

		public static Tweener DoSlider(this Slider target, float endValue, float duration)
		{
			_003CDoSlider_003Ec__AnonStorey5 _003CDoSlider_003Ec__AnonStorey = new _003CDoSlider_003Ec__AnonStorey5();
			_003CDoSlider_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDoSlider_003Ec__AnonStorey._003C_003Em__0, _003CDoSlider_003Ec__AnonStorey._003C_003Em__1, endValue, duration).SetTarget(_003CDoSlider_003Ec__AnonStorey.target);
		}

		public static Tweener DoWait(this GameObject target, float duration)
		{
			_003CDoWait_003Ec__AnonStorey6 _003CDoWait_003Ec__AnonStorey = new _003CDoWait_003Ec__AnonStorey6();
			_003CDoWait_003Ec__AnonStorey.target = target;
			return DOTween.To(_003CDoWait_003Ec__AnonStorey._003C_003Em__0, _003CDoWait_003Ec__AnonStorey._003C_003Em__1, 0f, duration).SetTarget(_003CDoWait_003Ec__AnonStorey.target);
		}

		public static float GameObjectTime(this GameObject target, float endValue)
		{
			return endValue;
		}
	}
}
