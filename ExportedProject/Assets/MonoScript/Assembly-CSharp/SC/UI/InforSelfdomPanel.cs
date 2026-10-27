using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class InforSelfdomPanel : View
	{
		private enum InforSelfdomPanelType
		{
			Head = 0,
			Frame = 1
		}

		[CompilerGenerated]
		private sealed class _003CFillHeadItemData_003Ec__AnonStorey0
		{
			internal RoleHeadCfg roleHeadCfg;

			internal InforSelfdomPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._selectHead = roleHeadCfg.id;
				_0024this.SetHead();
				_0024this.UpdateHeadData();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillFrameItemData_003Ec__AnonStorey1
		{
			internal RoleHeadFrameCfg roleHeadFrameCfg;

			internal InforSelfdomPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._selectFrame = roleHeadFrameCfg.id;
				_0024this.SetHeadFrame();
				_0024this.UpdateFrameData();
			}
		}

		private int _selectHead;

		private int _selectFrame;

		private InforSelfdomPanelType _currentType;

		private List<RoleHeadCfg> _roleHeadCfg;

		private List<RoleHeadFrameCfg> _roleHeadFrameCfg;

		private UIScrollPanel _uiScrollPanel;

		private GameObject btn_close;

		private GameObject btn_head;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_new;

		private GameObject btn_head_frame;

		private GameObject scp_items;

		private GInforSelfPanelItem m_cell;

		private GameObject btn_confirm;

		private GameObject txt_time;

		private Text txt_timeText;

		private GameObject m_head_icon;

		private GameObject m_head_frame;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject m_new_0;

		protected override void onInit()
		{
			m_cell.gameObject.SetActiveBetter(false);
			m_new.SetActiveBetter(false);
			m_new_0.SetActiveBetter(false);
			_roleHeadCfg = RoleHeadCfg.GetAllList();
			_roleHeadFrameCfg = RoleHeadFrameCfg.GetAllList();
			_uiScrollPanel = scp_items.GetComponent<UIScrollPanel>();
			txt_time.SetActiveBetter(false);
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_head, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_head_frame, string.Empty).onClick = _003ConInit_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_selectHead = Singleton<RoleMgr>.Ins.info.headId;
			_selectFrame = Singleton<RoleMgr>.Ins.info.frameId;
			SetHead();
			SetHeadFrame();
			RadioButton.ChooseBtn(btn_head);
			SetHeadData();
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		private void SetHeadData()
		{
			_uiScrollPanel.Clear();
			int cellNum = ((_roleHeadCfg.Count <= 15) ? 15 : _roleHeadCfg.Count);
			_uiScrollPanel.Reset(cellNum, FillHeadItemData);
		}

		private void FillHeadItemData(GameObject go, int index)
		{
			_003CFillHeadItemData_003Ec__AnonStorey0 _003CFillHeadItemData_003Ec__AnonStorey = new _003CFillHeadItemData_003Ec__AnonStorey0();
			_003CFillHeadItemData_003Ec__AnonStorey._0024this = this;
			GInforSelfPanelItem component = go.GetComponent<GInforSelfPanelItem>();
			if (index >= _roleHeadCfg.Count)
			{
				component.m_has_bg.SetActiveBetter(false);
				component.m_nohas_bg.SetActiveBetter(true);
				component.m_lock.SetActiveBetter(false);
				component.m_unlcok_item.SetActiveBetter(false);
				return;
			}
			_003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg = _roleHeadCfg[index];
			component.m_new.SetActiveBetter(false);
			View.SetItemSprite(component.m_lock_icon, _003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg.icon);
			View.SetItemSprite(component.m_unlock_icon, _003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg.icon);
			View.SetLabelText(component.txt_lock_nameText, _003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg.name);
			View.SetLabelText(component.txt_unlock_nameText, _003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg.name);
			bool flag = IsLockHead(_003CFillHeadItemData_003Ec__AnonStorey.roleHeadCfg.id);
			component.m_has_bg.SetActiveBetter(!flag);
			component.m_nohas_bg.SetActiveBetter(flag);
			component.m_lock.SetActiveBetter(flag);
			component.m_unlcok_item.SetActiveBetter(!flag);
			ClickListener.Get(component.m_unlock_icon, string.Empty).onClick = _003CFillHeadItemData_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateHeadData()
		{
			_uiScrollPanel.UpdateAllCell(UpdateHeadItemData);
		}

		private void UpdateHeadItemData(GameObject go, int index)
		{
			GInforSelfPanelItem component = go.GetComponent<GInforSelfPanelItem>();
			if (index >= _roleHeadCfg.Count)
			{
				component.m_has_bg.SetActiveBetter(false);
				component.m_nohas_bg.SetActiveBetter(true);
				component.m_lock.SetActiveBetter(false);
				component.m_unlcok_item.SetActiveBetter(false);
			}
			else
			{
				RoleHeadCfg roleHeadCfg = _roleHeadCfg[index];
				component.m_new.SetActiveBetter(false);
			}
		}

		private void SetFrameData()
		{
			_uiScrollPanel.Clear();
			_uiScrollPanel.Reset(_roleHeadFrameCfg.Count, FillFrameItemData);
		}

		private void FillFrameItemData(GameObject go, int index)
		{
			_003CFillFrameItemData_003Ec__AnonStorey1 _003CFillFrameItemData_003Ec__AnonStorey = new _003CFillFrameItemData_003Ec__AnonStorey1();
			_003CFillFrameItemData_003Ec__AnonStorey._0024this = this;
			GInforSelfPanelItem component = go.GetComponent<GInforSelfPanelItem>();
			_003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg = _roleHeadFrameCfg[index];
			component.m_new.SetActiveBetter(false);
			View.SetItemSprite(component.m_lock_icon, _003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg.frame);
			View.SetItemSprite(component.m_unlock_icon, _003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg.frame);
			View.SetLabelText(component.txt_lock_nameText, _003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg.name);
			View.SetLabelText(component.txt_unlock_nameText, _003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg.name);
			bool flag = IsLockFrame(_003CFillFrameItemData_003Ec__AnonStorey.roleHeadFrameCfg.id);
			component.m_lock.SetActiveBetter(flag);
			component.m_unlcok_item.SetActiveBetter(!flag);
			ClickListener.Get(component.m_unlock_icon, string.Empty).onClick = _003CFillFrameItemData_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateFrameData()
		{
			_uiScrollPanel.UpdateAllCell(UpdateFrameItemData);
		}

		private void UpdateFrameItemData(GameObject go, int index)
		{
			GInforSelfPanelItem component = go.GetComponent<GInforSelfPanelItem>();
			RoleHeadFrameCfg roleHeadFrameCfg = _roleHeadFrameCfg[index];
			component.m_new.SetActiveBetter(false);
		}

		private void OnClickConfirm(GameObject go)
		{
			Singleton<RoleMgr>.Ins.SetFrameId(_selectFrame);
			Singleton<RoleMgr>.Ins.SetHeadId(_selectHead);
		}

		private void SetHead()
		{
			RoleHeadCfg roleHeadCfg = RoleHeadCfg.Get(_selectHead);
			if (roleHeadCfg != null)
			{
				View.SetSpriteAsync(m_head_icon, "icon/" + roleHeadCfg.icon);
				View.SetLabelText(txt_nameText, roleHeadCfg.name);
			}
		}

		private void SetHeadFrame()
		{
			RoleHeadFrameCfg roleHeadFrameCfg = RoleHeadFrameCfg.Get(_selectFrame);
			if (roleHeadFrameCfg != null)
			{
				View.SetSpriteAsync(m_head_frame, "icon/" + roleHeadFrameCfg.frame);
			}
		}

		private bool IsLockHead(int id)
		{
			return false;
		}

		private bool IsLockFrame(int id)
		{
			return false;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			btn_head = component.GameObjects[1].gameObject;
			txt_on = component.GameObjects[2].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[3].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_new = component.GameObjects[4].gameObject;
			btn_head_frame = component.GameObjects[5].gameObject;
			scp_items = component.GameObjects[6].gameObject;
			m_cell = View.AddComponentIfNotExist<GInforSelfPanelItem>(component.GameObjects[7].gameObject);
			btn_confirm = component.GameObjects[8].gameObject;
			txt_time = component.GameObjects[9].gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
			m_head_icon = component.GameObjects[10].gameObject;
			m_head_frame = component.GameObjects[11].gameObject;
			txt_name = component.GameObjects[12].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_on_0 = component.GameObjects[13].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[14].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			m_new_0 = component.GameObjects[15].gameObject;
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
			_currentType = InforSelfdomPanelType.Head;
			SetHeadData();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			_currentType = InforSelfdomPanelType.Frame;
			SetFrameData();
		}
	}
}
