using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using gs.battle.scmsg;
using gs.online.scmsg;

namespace SC.UI
{
	public class LoginLinePanel : View
	{
		private GameObject m_bg;

		private GameObject m_main_window;

		private GameObject btn_back;

		private GameObject txt_line_num;

		private Text txt_line_numText;

		private GameObject txt_line_time;

		private Text txt_line_timeText;

		protected override void onInit()
		{
			base.onInit();
			SWaitRank.handler = (SWaitRank.Handler)Delegate.Combine(SWaitRank.handler, new SWaitRank.Handler(onSWaitRank));
			SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Combine(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(onSBattleLoginFinish));
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onDestroy()
		{
			base.onDestroy();
			SWaitRank.handler = (SWaitRank.Handler)Delegate.Remove(SWaitRank.handler, new SWaitRank.Handler(onSWaitRank));
			SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Remove(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(onSBattleLoginFinish));
		}

		private void onSBattleLoginFinish(SBattleLoginFinish msg)
		{
			Hide();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			onSWaitRank(null);
		}

		protected override void onHide(string childView = null)
		{
			Singleton<PlatformMgr>.Ins.onCancelWait();
		}

		private void onSWaitRank(SWaitRank msg)
		{
			View.SetLabelText(txt_line_num, string.Empty + Singleton<LoginMgr>.Ins.nLoginServerWaitNum);
			View.SetLabelText(txt_line_time, string.Empty + Singleton<LoginMgr>.Ins.nLoginServerWaitNum * 6);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			m_main_window = component.GameObjects[1].gameObject;
			btn_back = component.GameObjects[2].gameObject;
			txt_line_num = component.GameObjects[3].gameObject;
			txt_line_numText = txt_line_num.GetComponent<Text>();
			txt_line_time = component.GameObjects[4].gameObject;
			txt_line_timeText = txt_line_time.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject o)
		{
			MessageBoxPanel.Show(371, _003ConInit_003Em__1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1()
		{
			CQuitQueue msg = new CQuitQueue
			{
				account = Singleton<AuthMgr>.Ins.UserId
			};
			Client2Gs.Ins.Send(msg);
			Hide();
			Client2Gs.Ins.Close();
		}
	}
}
