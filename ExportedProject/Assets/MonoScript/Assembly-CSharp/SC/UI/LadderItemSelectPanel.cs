using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class LadderItemSelectPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillBagItemData_003Ec__AnonStorey0
		{
			internal BagItem bagItem;

			internal LadderItemSelectPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._instanceId = bagItem.instanceId;
				_0024this.UpdateData();
			}
		}

		private int _itemId;

		private int _instanceId;

		private UIScrollPanel _bagScrollPanel;

		private List<BagItem> _bagitems = new List<BagItem>();

		public Dictionary<int, GunParts> _gunDic = new Dictionary<int, GunParts>();

		private GameObject btn_close;

		private GameObject scp_cell;

		private GLadderItemSelectPanelItem m_cell;

		private GameObject btn_handin;

		protected override void onInit()
		{
			_bagScrollPanel = scp_cell.GetComponent<UIScrollPanel>();
			_gunDic = Singleton<BagMgr>.Ins.GunDic;
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_handin, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_itemId = (int)param;
			if (_itemId > 0)
			{
				SetBagData();
			}
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		private void SetBagData()
		{
			_bagitems.Clear();
			_bagitems = Singleton<BagMgr>.Ins.GetAllItemByItemId(_itemId);
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_bagitems.Count, FillBagItemData);
		}

		private void FillBagItemData(GameObject go, int index)
		{
			_003CFillBagItemData_003Ec__AnonStorey0 _003CFillBagItemData_003Ec__AnonStorey = new _003CFillBagItemData_003Ec__AnonStorey0();
			_003CFillBagItemData_003Ec__AnonStorey._0024this = this;
			GLadderItemSelectPanelItem component = go.GetComponent<GLadderItemSelectPanelItem>();
			_003CFillBagItemData_003Ec__AnonStorey.bagItem = _bagitems[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBagItemData_003Ec__AnonStorey.bagItem.itemId);
			if (itemCfg == null)
			{
				return;
			}
			if (itemCfg.type == 13)
			{
				component.txt_bullet_num.SetActiveBetter(true);
				component.m_gun_part.SetActiveBetter(true);
				View.SetLabelText(component.txt_bullet_numText, _gunDic[_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId].bulletNumber);
				GunCfg gunCfg = GunCfg.Get(itemCfg.id);
				int num = 0;
				if (gunCfg.muzzleParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.aimParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.clipParts.Count > 0)
				{
					num++;
				}
				if (gunCfg.qiangbaParts.Count > 0)
				{
					num++;
				}
				for (int i = 0; i < component.m_gun_part_num_bgs.Length; i++)
				{
					component.m_gun_part_num_bgs[i].SetActiveBetter(i < num);
				}
				for (int j = 0; j < component.m_gun_part_nums.Length; j++)
				{
					component.m_gun_part_nums[j].SetActiveBetter(j < _gunDic[_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId].parts.Count);
				}
			}
			else
			{
				component.m_gun_part.SetActiveBetter(false);
				component.txt_bullet_num.SetActiveBetter(false);
			}
			View.SetItemSprite(component.m_shortcut_bar_icon, itemCfg.icon);
			component.m_select.SetActiveBetter(_003CFillBagItemData_003Ec__AnonStorey.bagItem.instanceId == _instanceId);
			if (itemCfg.durability > 0)
			{
				component.m_durability.SetActiveBetter(true);
				View.SetSlider(component.m_durability, (float)_003CFillBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)itemCfg.durability);
			}
			else
			{
				component.m_durability.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillBagItemData_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateData()
		{
			_bagScrollPanel.UpdateAllCell(UpdateBagItemData);
		}

		private void UpdateBagItemData(GameObject go, int index)
		{
			GLadderItemSelectPanelItem component = go.GetComponent<GLadderItemSelectPanelItem>();
			BagItem bagItem = _bagitems[index];
			component.m_select.SetActiveBetter(bagItem.instanceId == _instanceId);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			scp_cell = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GLadderItemSelectPanelItem>(component.GameObjects[2].gameObject);
			btn_handin = component.GameObjects[3].gameObject;
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
			if (_instanceId > 0)
			{
				Singleton<TaskMgr>.Ins.UploadItem(_instanceId, _itemId, 1);
				Hide();
			}
		}
	}
}
