using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class BagItemInfoPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CSetBtnCallBack_003Ec__AnonStorey1
		{
			internal ItemGetWayCfg getWayInfo;

			internal BagItemInfoPanel _0024this;
		}

		[CompilerGenerated]
		private sealed class _003CSetBtnCallBack_003Ec__AnonStorey0
		{
			internal OneItemGetWay way;

			internal _003CSetBtnCallBack_003Ec__AnonStorey1 _003C_003Ef__ref_00241;

			internal void _003C_003Em__0(GameObject go)
			{
				_lastClickBtnName = way.btnName;
				if (string.IsNullOrEmpty(way.desc))
				{
					_003C_003Ef__ref_00241._0024this.Hide();
					Type type = Type.GetType("SC.UI." + way.viewName);
					if (ViewMgr.Ins.IsShow(way.viewName))
					{
						Utils.TriggerEvent(ScProduceEvent.ItemInfoWayJumpDelegate, _003C_003Ef__ref_00241.getWayInfo.id);
					}
					else
					{
						ViewMgr.Ins.ShowView(type, _003C_003Ef__ref_00241.getWayInfo.id, false);
					}
				}
				else
				{
					_003C_003Ef__ref_00241._0024this.ShowWayDesc(way.desc);
				}
			}
		}

		private const string ProduceStr = "制造";

		private const string Recipe = "配方";

		private const string GiftBagStr = "商城礼包";

		private const string NameSpaceStr = "SC.UI.";

		private static string _lastClickBtnName = string.Empty;

		private GameObject btn_close;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_desc;

		private Text txt_descText;

		private GameObject txt_way_get;

		private Text txt_way_getText;

		private List<GBagItemInfoCommonBtnCell> m_btnslist = new List<GBagItemInfoCommonBtnCell>();

		private GameObject[] m_btns;

		private GameObject m_btnsObj;

		private GameObject m_way_desc;

		private GameObject txt_way_desc;

		private Text txt_way_descText;

		private GameObject m_exit_way_desc;

		protected override void onInit()
		{
			base.onInit();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(m_exit_way_desc, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			ItemCfg itemCfg = param as ItemCfg;
			if (itemCfg != null)
			{
				View.SetItemSprite(m_icon, itemCfg.icon);
				View.SetLabelText(txt_nameText, itemCfg.name);
				View.SetLabelText(txt_descText, itemCfg.desc);
				SetBtnCallBack(ItemGetWayCfg.Get(itemCfg.id));
			}
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
		}

		private void SetBtnCallBack(ItemGetWayCfg getWayInfo)
		{
			_003CSetBtnCallBack_003Ec__AnonStorey1 _003CSetBtnCallBack_003Ec__AnonStorey = new _003CSetBtnCallBack_003Ec__AnonStorey1();
			_003CSetBtnCallBack_003Ec__AnonStorey.getWayInfo = getWayInfo;
			_003CSetBtnCallBack_003Ec__AnonStorey._0024this = this;
			m_way_desc.SetActiveBetter(false);
			if (_003CSetBtnCallBack_003Ec__AnonStorey.getWayInfo == null)
			{
				m_btnsObj.SetActiveBetter(false);
				txt_way_get.SetActiveBetter(false);
				return;
			}
			txt_way_get.SetActiveBetter(true);
			m_btnsObj.SetActiveBetter(true);
			int i = 0;
			for (int num = m_btns.Length; i < num; i++)
			{
				if (i < _003CSetBtnCallBack_003Ec__AnonStorey.getWayInfo.getWayList.Count)
				{
					_003CSetBtnCallBack_003Ec__AnonStorey0 _003CSetBtnCallBack_003Ec__AnonStorey2 = new _003CSetBtnCallBack_003Ec__AnonStorey0();
					_003CSetBtnCallBack_003Ec__AnonStorey2._003C_003Ef__ref_00241 = _003CSetBtnCallBack_003Ec__AnonStorey;
					int num2 = i;
					_003CSetBtnCallBack_003Ec__AnonStorey2.way = _003CSetBtnCallBack_003Ec__AnonStorey.getWayInfo.getWayList[num2];
					bool flag = _003CSetBtnCallBack_003Ec__AnonStorey2.way.btnName.Equals("配方");
					bool flag2 = _003CSetBtnCallBack_003Ec__AnonStorey2.way.btnName.Equals("制造");
					if (flag || flag2)
					{
						int value;
						if (Singleton<ScProduceMgr>.Ins.DicProductionId2DrawId.TryGetValue(_003CSetBtnCallBack_003Ec__AnonStorey.getWayInfo.id, out value))
						{
							DrawingCfg drawingInfo = DrawingCfg.Get(value);
							bool flag3 = Singleton<ScProduceMgr>.Ins.IsLocked(drawingInfo, Singleton<RoleMgr>.Ins.info.level);
							if (flag3 && flag2)
							{
								m_btns[num2].SetActiveBetter(false);
								continue;
							}
							if (!flag3 && flag)
							{
								m_btns[num2].SetActiveBetter(false);
								continue;
							}
						}
						else
						{
							m_btns[num2].SetActiveBetter(false);
						}
					}
					GBagItemInfoCommonBtnCell gBagItemInfoCommonBtnCell = m_btnslist[num2];
					GameObject go = m_btns[num2];
					go.SetActiveBetter(true);
					View.SetLabelText(gBagItemInfoCommonBtnCell.txt_nameText, _003CSetBtnCallBack_003Ec__AnonStorey2.way.btnName);
					View.SetItemSprite(gBagItemInfoCommonBtnCell.m_icon, _003CSetBtnCallBack_003Ec__AnonStorey2.way.btnIcon);
					ClickListener.Get(go, string.Empty).onClick = _003CSetBtnCallBack_003Ec__AnonStorey2._003C_003Em__0;
				}
				else
				{
					m_btns[i].SetActiveBetter(false);
				}
			}
		}

		public static bool IsJumpToGiftBag()
		{
			return _lastClickBtnName.Equals("商城礼包");
		}

		private void ShowWayDesc(string wayDesc)
		{
			m_way_desc.SetActiveBetter(true);
			View.SetLabelText(txt_way_descText, wayDesc);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			m_icon = component.GameObjects[1].gameObject;
			txt_name = component.GameObjects[2].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_desc = component.GameObjects[3].gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_way_get = component.GameObjects[4].gameObject;
			txt_way_getText = txt_way_get.GetComponent<Text>();
			m_btns = component.GameObjects[5].gameObject.GetComponent<UIGameObjectList>().objects;
			m_btnsObj = component.GameObjects[5].gameObject;
			for (int i = 0; i < m_btns.Length; i++)
			{
				m_btnslist.Add(View.AddComponentIfNotExist<GBagItemInfoCommonBtnCell>(m_btns[i].gameObject));
			}
			m_way_desc = component.GameObjects[6].gameObject;
			txt_way_desc = component.GameObjects[7].gameObject;
			txt_way_descText = txt_way_desc.GetComponent<Text>();
			m_exit_way_desc = component.GameObjects[8].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			m_way_desc.SetActiveBetter(false);
		}
	}
}
