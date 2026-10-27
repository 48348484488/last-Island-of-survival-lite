using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using cfg;

namespace SC.UI
{
	public class LadderPopupPanel : View
	{
		private GameObject m_bg;

		private GameObject m_root;

		[CompilerGenerated]
		private static TweenCallback _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			base.onInit();
			ClickListener.Get(m_bg, string.Empty).onClick = (ClickListener.Get(m_root, string.Empty).onClick = _003ConInit_003Em__0);
		}

		private void PlayAnim()
		{
			Transform target = m_root.transform;
			if ((bool)Singleton<GuideMgr>.Ins.UIRoot)
			{
				Transform transform = Singleton<GuideMgr>.Ins.UIRoot.Find(cfg.Consts.LADDER_TASK_BTN_NAME);
				if ((bool)transform)
				{
					target.DOMove(transform.position, 0.5f);
					Tweener t = target.DOScale(Vector3.one * 0.15f, 0.5f);
					if (_003C_003Ef__am_0024cache0 == null)
					{
						_003C_003Ef__am_0024cache0 = _003CPlayAnim_003Em__1;
					}
					t.OnComplete(_003C_003Ef__am_0024cache0);
				}
				else
				{
					ViewMgr.Ins.Destroy<LadderPopupPanel>();
				}
			}
			else
			{
				ViewMgr.Ins.Destroy<LadderPopupPanel>();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			m_root = component.GameObjects[1].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			PlayAnim();
		}

		[CompilerGenerated]
		private static void _003CPlayAnim_003Em__1()
		{
			ViewMgr.Ins.Destroy<LadderPopupPanel>();
		}
	}
}
