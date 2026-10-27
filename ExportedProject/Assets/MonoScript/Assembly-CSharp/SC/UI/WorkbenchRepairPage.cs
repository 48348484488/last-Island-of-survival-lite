using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.workbench.scmsg;

namespace SC.UI
{
	public class WorkbenchRepairPage : MonoBehaviour, IWorkbenchPage
	{
		[CompilerGenerated]
		private sealed class _003CFillRepairMaterial_003Ec__AnonStorey1
		{
			internal ItemCfg itemInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CJiaTeXiao_003Ec__AnonStorey2
		{
			internal AlertBox alertBox;

			internal WorkbenchRepairPage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (alertBox != null)
				{
					GameObject txtAlertBox = alertBox.GetTxtAlertBox();
					if (txtAlertBox != null)
					{
						RectTransform rectTransform = o.transform as RectTransform;
						rectTransform.SetParent(txtAlertBox.transform);
						rectTransform.anchoredPosition = Vector3.zero;
						rectTransform.localScale = Vector3.one;
						rectTransform.eulerAngles = Vector3.zero;
						o.SetActive(false);
						o.SetActive(true);
						_0024this.StartCoroutine(_0024this.WaitTeXiao(o));
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(o, true);
					}
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(o, true);
				}
			}
		}

		public GameObject btn_repair;

		public List<GWorkbenchRepairMaterialCell> m_materiallist = new List<GWorkbenchRepairMaterialCell>();

		public GameObject[] m_material;

		public GameObject m_materialObj;

		public GameObject m_price;

		public GWorkbenchRepairCell m_repair;

		public GameObject txt_price;

		public Text txt_priceText;

		public object context;

		private BagItem _repairItem;

		private WorkbenchRepairCfg _repairInfo;

		private readonly Dictionary<int, BagItem> _dicIndex2BagItemInfo = new Dictionary<int, BagItem>();

		private readonly Dictionary<int, int> _dicItemId2Index = new Dictionary<int, int>();

		private readonly LinkedList<int> _emptyIndex = new LinkedList<int>();

		private float _alpha = 0.3f;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		private void Awake()
		{
			btn_repair = base.transform.Find("GameObject (2)/btn_repair").gameObject;
			m_material = base.transform.Find("m_material").gameObject.GetComponent<UIGameObjectList>().objects;
			m_materialObj = base.transform.Find("m_material").gameObject;
			if (m_materiallist.Count <= 0)
			{
				for (int i = 0; i < m_material.Length; i++)
				{
					m_materiallist.Add(View.AddComponentIfNotExist<GWorkbenchRepairMaterialCell>(m_material[i].gameObject));
				}
			}
			m_price = base.transform.Find("GameObject (2)/GameObject (1)/m_price").gameObject;
			m_repair = View.AddComponentIfNotExist<GWorkbenchRepairCell>(base.transform.Find("m_material/m_repair").gameObject);
			txt_price = base.transform.Find("GameObject (2)/GameObject (1)/txt_price").gameObject;
			txt_priceText = txt_price.GetComponent<Text>();
		}

		public void OnInit()
		{
			ClickListener.Get(btn_repair, string.Empty).onClick = _003COnInit_003Em__0;
		}

		public void OnShow(int level)
		{
			RemoveRepairItem();
			SRepair.handler = (SRepair.Handler)Delegate.Combine(SRepair.handler, new SRepair.Handler(OnSRepair));
		}

		public void OnHide()
		{
			SRepair.handler = (SRepair.Handler)Delegate.Remove(SRepair.handler, new SRepair.Handler(OnSRepair));
		}

		public void OnClickBagItem(BagItem bagItemInfo, int num)
		{
			WorkbenchRepairCfg workbenchRepairCfg = WorkbenchRepairCfg.Get(bagItemInfo.itemId);
			int value;
			if (workbenchRepairCfg != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(bagItemInfo.itemId);
				if (itemCfg.durability == bagItemInfo.duration)
				{
					AlertBox.Show(181);
				}
				else if (_repairItem != null)
				{
					bool flag = _repairItem.itemId != workbenchRepairCfg.itemId;
					RemoveRepairItem();
					RemoveAllRepairMaterial();
					if (flag)
					{
						AddRepairItem(bagItemInfo);
						if (WorkbenchEvent.AutoAddRepairMaterialDelegate != null)
						{
							WorkbenchEvent.AutoAddRepairMaterialDelegate(_repairInfo);
						}
					}
				}
				else
				{
					AddRepairItem(bagItemInfo);
					if (_dicItemId2Index.Count > 0)
					{
						RemoveAllRepairMaterial();
					}
					if (WorkbenchEvent.AutoAddRepairMaterialDelegate != null)
					{
						WorkbenchEvent.AutoAddRepairMaterialDelegate(_repairInfo);
					}
				}
			}
			else if (_repairItem == null)
			{
				AlertBox.Show(172);
			}
			else if (_dicItemId2Index.TryGetValue(bagItemInfo.itemId, out value))
			{
				AlertBox.Show(171, ItemCfg.Get(bagItemInfo.itemId).name);
			}
			else if (!MaterialContains(bagItemInfo.itemId))
			{
				AlertBox.Show(173);
			}
			else
			{
				AddRepairMaterial(bagItemInfo);
			}
		}

		private bool MaterialContains(int itemId)
		{
			if (_repairInfo == null)
			{
				return false;
			}
			bool result = false;
			List<DrawingNeedMaterial> material = _repairInfo.material;
			int i = 0;
			for (int count = material.Count; i < count; i++)
			{
				if (material[i].itemId == itemId)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void AddRepairItem(BagItem bagItemInfo)
		{
			_repairItem = bagItemInfo;
			_repairInfo = WorkbenchRepairCfg.Get(bagItemInfo.itemId);
			FillRepairItem();
			SetCost();
			if (WorkbenchEvent.SetInUseItemInstanceIdDelegate != null)
			{
				WorkbenchEvent.SetInUseItemInstanceIdDelegate(bagItemInfo.itemId, bagItemInfo.instanceId);
			}
		}

		private void SetCost()
		{
			if (_repairInfo == null)
			{
				m_price.SetActiveBetter(false);
				txt_price.SetActiveBetter(false);
			}
			else
			{
				View.SetItemSprite(m_price, "common" + GetPriceIcon(_repairInfo.costType));
				View.SetLabelText(txt_priceText, _repairInfo.costValue);
			}
		}

		private void FillRepairItem()
		{
			if (_repairItem == null)
			{
				m_repair.m_item.SetActiveBetter(false);
				ClickListener.Get(m_repair.gameObject, string.Empty).onClick = null;
				return;
			}
			m_repair.m_item.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(_repairItem.itemId);
			View.SetItemSprite(m_repair.m_icon, itemCfg.icon);
			View.SetLabelText(m_repair.txt_name, itemCfg.name);
			Slider component = m_repair.m_durability.GetComponent<Slider>();
			component.maxValue = itemCfg.durability;
			component.value = _repairItem.duration;
			View.SetLabelText(m_repair.txt_duration_valueText, _repairItem.duration);
			ClickListener.Get(m_repair.gameObject, string.Empty).onClick = _003CFillRepairItem_003Em__1;
		}

		public void AddRepairMaterial(BagItem bagItemInfo, bool refreshBag = true)
		{
			if (_dicItemId2Index.Count == m_material.Length)
			{
				AlertBox.Show(175);
				return;
			}
			int value = _emptyIndex.First.Value;
			FillRepairMaterial(bagItemInfo, value);
			_dicIndex2BagItemInfo.Add(value, bagItemInfo);
			_dicItemId2Index.Add(bagItemInfo.itemId, value);
			_emptyIndex.RemoveFirst();
			if (refreshBag && WorkbenchEvent.SetInUseItemInstanceIdDelegate != null)
			{
				WorkbenchEvent.SetInUseItemInstanceIdDelegate(bagItemInfo.itemId, bagItemInfo.instanceId);
			}
		}

		private void FillRepairMaterial(BagItem bagItemInfo, int index)
		{
			_003CFillRepairMaterial_003Ec__AnonStorey1 _003CFillRepairMaterial_003Ec__AnonStorey = new _003CFillRepairMaterial_003Ec__AnonStorey1();
			GWorkbenchRepairMaterialCell gWorkbenchRepairMaterialCell = m_materiallist[index];
			if (bagItemInfo == null)
			{
				gWorkbenchRepairMaterialCell.m_item.SetActiveBetter(false);
				PressListener.Get(m_material[index]).onPress = null;
				ClickListener.Get(m_material[index], string.Empty).onClick = null;
				return;
			}
			gWorkbenchRepairMaterialCell.m_item.SetActiveBetter(true);
			_003CFillRepairMaterial_003Ec__AnonStorey.itemInfo = ItemCfg.Get(bagItemInfo.itemId);
			View.SetItemSprite(gWorkbenchRepairMaterialCell.m_icon, _003CFillRepairMaterial_003Ec__AnonStorey.itemInfo.icon);
			View.SetLabelText(gWorkbenchRepairMaterialCell.txt_nameText, _003CFillRepairMaterial_003Ec__AnonStorey.itemInfo.name);
			Image component = gWorkbenchRepairMaterialCell.m_icon.GetComponent<Image>();
			Color color = component.color;
			color.a = ((bagItemInfo.number != 0) ? 1f : _alpha);
			component.color = color;
			if (_repairInfo != null)
			{
				int num = 0;
				int i = 0;
				for (int count = _repairInfo.material.Count; i < count; i++)
				{
					DrawingNeedMaterial drawingNeedMaterial = _repairInfo.material[i];
					if (drawingNeedMaterial.itemId == _003CFillRepairMaterial_003Ec__AnonStorey.itemInfo.id)
					{
						num = drawingNeedMaterial.num;
						break;
					}
				}
				gWorkbenchRepairMaterialCell.btn_add.SetActiveBetter(num > bagItemInfo.number);
				View.SetLabelText(gWorkbenchRepairMaterialCell.txt_numText, Utils.GetString(9, bagItemInfo.number, num));
			}
			else
			{
				gWorkbenchRepairMaterialCell.btn_add.SetActiveBetter(false);
				View.SetLabelText(gWorkbenchRepairMaterialCell.txt_numText, bagItemInfo.number);
			}
			ClickListener.Get(m_material[index], string.Empty).onClick = _003CFillRepairMaterial_003Ec__AnonStorey._003C_003Em__0;
		}

		public void RemoveRepairItem()
		{
			_repairItem = null;
			_repairInfo = null;
			FillRepairItem();
			SetCost();
			RemoveAllRepairMaterial();
		}

		public void RemoveRepairMaterial(int index)
		{
			FillRepairMaterial(null, index);
			BagItem value;
			if (WorkbenchEvent.RemoveInUseItemIdDelegate != null && _dicIndex2BagItemInfo.TryGetValue(index, out value))
			{
				WorkbenchEvent.RemoveInUseItemIdDelegate(value.itemId);
			}
			_dicIndex2BagItemInfo.Remove(index);
			LinkedListNode<int> linkedListNode = _emptyIndex.First;
			bool flag = false;
			while (linkedListNode != null)
			{
				if (linkedListNode.Value > index)
				{
					_emptyIndex.AddBefore(linkedListNode, index);
					flag = true;
					break;
				}
				linkedListNode = linkedListNode.Next;
			}
			if (!flag)
			{
				_emptyIndex.AddLast(index);
			}
		}

		private void RemoveAllRepairMaterial()
		{
			_dicItemId2Index.Clear();
			_dicIndex2BagItemInfo.Clear();
			_emptyIndex.Clear();
			int i = 0;
			for (int num = m_material.Length; i < num; i++)
			{
				FillRepairMaterial(null, i);
				_emptyIndex.AddLast(i);
			}
			if (WorkbenchEvent.RemoveAllInUseItemDelegate != null)
			{
				WorkbenchEvent.RemoveAllInUseItemDelegate();
			}
		}

		private void OnSRepair(SRepair msg)
		{
			ViewMgr.Ins.AddOnShowEvent<AlertBox>(JiaTeXiao);
			AlertBox.Show(180);
			RemoveRepairItem();
		}

		private void JiaTeXiao()
		{
			_003CJiaTeXiao_003Ec__AnonStorey2 _003CJiaTeXiao_003Ec__AnonStorey = new _003CJiaTeXiao_003Ec__AnonStorey2();
			_003CJiaTeXiao_003Ec__AnonStorey._0024this = this;
			ViewMgr.Ins.RemoveOnShowEvent("AlertBox", JiaTeXiao);
			_003CJiaTeXiao_003Ec__AnonStorey.alertBox = ViewMgr.Ins.GetView<AlertBox>();
			ResMgr.Ins.CreateFromAB("effect/ui_xiulichenggong_01.ab", null, _003CJiaTeXiao_003Ec__AnonStorey._003C_003Em__0);
		}

		private IEnumerator WaitTeXiao(GameObject go)
		{
			yield return Utils.WaitForSeconds(3f);
			UnityEngine.Object.DestroyImmediate(go);
		}

		private string GetPriceIcon(int moneyType)
		{
			switch (moneyType)
			{
			case 1:
				return cfg.Consts.GOLD_ICON;
			case 2:
				return cfg.Consts.COUPON_ICON;
			default:
				return string.Empty;
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			if (_repairInfo == null)
			{
				AlertBox.Show(174);
				return;
			}
			List<DrawingNeedMaterial> material = _repairInfo.material;
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			int i = 0;
			for (int count = material.Count; i < count; i++)
			{
				int itemId = material[i].itemId;
				int value;
				BagItem value2;
				if (!_dicItemId2Index.TryGetValue(itemId, out value) || !_dicIndex2BagItemInfo.TryGetValue(value, out value2) || value2.itemId != itemId)
				{
					hashSet.Add(itemId);
				}
				else if (value2.number < material[i].num)
				{
					hashSet2.Add(itemId);
				}
			}
			if (hashSet.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (int item in hashSet)
				{
					stringBuilder.Append(Utils.GetString(ItemCfg.Get(item).name));
					stringBuilder.Append(',');
				}
				AlertBox.Show(Utils.GetString(135) + stringBuilder.ToString().TrimEnd(','));
			}
			else if (hashSet2.Count > 0)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (int item2 in hashSet2)
				{
					stringBuilder2.Append(Utils.GetString(ItemCfg.Get(item2).name));
					stringBuilder2.Append(',');
				}
				AlertBox.Show(10, stringBuilder2.ToString().TrimEnd(','));
			}
			else
			{
				Singleton<WorkbenchPanelMgr>.Ins.SendRepairItemMsg(_repairItem.instanceId);
			}
		}

		[CompilerGenerated]
		private void _003CFillRepairItem_003Em__1(GameObject o)
		{
			RemoveRepairItem();
		}
	}
}
