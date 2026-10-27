using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class BattleLoadingPanel : View
	{
		private Slider m_Slider;

		private bool IsClearBattle;

		private bool IsNeedConnectGs;

		private bool IsPlayAgain;

		private bool IsReconnectBattle;

		private bool IsRebirth;

		private WaitForSeconds _waitLow = new WaitForSeconds(0.1f);

		private WaitForSeconds _waitHigh = new WaitForSeconds(0.06f);

		private GameObject m_bg;

		private GameObject txt_loding_name;

		private Text txt_loding_nameText;

		private GameObject m_loding_slider;

		protected override void onInit()
		{
		}

		protected override void onShow(object param = null, string childView = null)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			IsClearBattle = false;
			IsNeedConnectGs = false;
			IsPlayAgain = false;
			IsReconnectBattle = false;
			IsRebirth = false;
			if (param != null)
			{
				BattleLodingInfo battleLodingInfo = param as BattleLodingInfo;
				if (battleLodingInfo.IsClearBattle)
				{
					IsClearBattle = true;
				}
				if (battleLodingInfo.IsNeedConnectGs)
				{
					IsNeedConnectGs = true;
				}
				if (battleLodingInfo.IsPlayAgain)
				{
					IsPlayAgain = true;
				}
				if (battleLodingInfo.IsReconnectBattle)
				{
					IsReconnectBattle = true;
				}
			}
			if (!IsRebirth)
			{
				ViewMgr.Ins.UICamera.GetComponent<AudioListener>().enabled = false;
				SingletonMono<AudioManager>.Ins.StopMusicBgFadeOut();
			}
			m_Slider = m_loding_slider.GetComponent<Slider>();
			m_Slider.value = 0f;
			View.SetLabelText(txt_loding_nameText, 0 + "%");
			StartCoroutine(Tick());
		}

		private IEnumerator Tick()
		{
			float i = 0f;
			while (true)
			{
				i += 0.01f;
				OnLoadSceneProcess(i);
				if (i == 0.2f)
				{
				}
				if (i >= 1f)
				{
					break;
				}
				if (Utils.IsLowEndProduct())
				{
					yield return _waitLow;
				}
				else
				{
					yield return _waitHigh;
				}
			}
		}

		private void OnLoadSceneProcess(float arg)
		{
			m_Slider.value = arg;
			double num = Math.Round(arg, 4) * 100.0;
			if (num > 100.0)
			{
				num = 100.0;
			}
			View.SetLabelText(txt_loding_nameText, num + "%");
		}

		protected override void onHide(string childView = null)
		{
			View.SetLabelText(txt_loding_nameText, 100 + "%");
			SceneEvent.LoadSceneProcess = (Utils.FloatDelegate)Delegate.Remove(SceneEvent.LoadSceneProcess, new Utils.FloatDelegate(OnLoadSceneProcess));
			if (!IsClearBattle)
			{
				Battle.Ins.UICamrea.GetComponent<AudioListener>().enabled = false;
			}
			if (Battle.Ins != null && Battle.Ins.SelfPlayer != null)
			{
				if (Battle.Ins.SelfPlayer != null)
				{
					Battle.Ins.SelfPlayer.SetGroundPos(Battle.Ins.SelfPlayer.Pos);
				}
				Battle.Ins.SelfPlayer.EnablePhysic();
			}
		}

		protected override void onDestroy()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			txt_loding_name = component.GameObjects[1].gameObject;
			txt_loding_nameText = txt_loding_name.GetComponent<Text>();
			m_loding_slider = component.GameObjects[2].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
