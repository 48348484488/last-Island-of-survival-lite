using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.map.circuitry.scmsg;
using gs.battle.scmsg;

namespace SC.UI
{
	public class DianluPanel : View
	{
		private class MyLinkedList<T>
		{
			private const int RefreshDicInterval = 4;

			private int _nextEmptyStartIndex;

			private int _nextEmptyEndIndex = 1;

			private readonly Dictionary<int, MyLinkedListNode<T>> _dicIndex2Node = new Dictionary<int, MyLinkedListNode<T>>(cfg.Consts.ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX - 1);

			private MyLinkedListNode<T> _first;

			private MyLinkedListNode<T> _last;

			public MyLinkedListNode<T> First
			{
				get
				{
					return _first;
				}
			}

			public MyLinkedListNode<T> Last
			{
				get
				{
					return _last;
				}
			}

			public int Count
			{
				get
				{
					return _nextEmptyEndIndex - _nextEmptyStartIndex - 1;
				}
			}

			public MyLinkedListNode<T> this[int index]
			{
				get
				{
					int num = index + _nextEmptyStartIndex + 1;
					if (num >= _nextEmptyEndIndex || num <= _nextEmptyStartIndex)
					{
						return null;
					}
					MyLinkedListNode<T> value;
					_dicIndex2Node.TryGetValue(num, out value);
					return value;
				}
			}

			public void Clear()
			{
				_dicIndex2Node.Clear();
				_first = null;
				_last = null;
				_nextEmptyStartIndex = 0;
				_nextEmptyEndIndex = 1;
			}

			internal void AddFirst(T valuePair)
			{
				MyLinkedListNode<T> myLinkedListNode = new MyLinkedListNode<T>();
				_dicIndex2Node[_nextEmptyStartIndex] = myLinkedListNode;
				myLinkedListNode.Value = valuePair;
				myLinkedListNode.Index = _nextEmptyStartIndex--;
				myLinkedListNode.Next = _first;
				if (_first != null)
				{
					_first.Previous = myLinkedListNode;
				}
				_first = myLinkedListNode;
				if (_last == null)
				{
					_last = myLinkedListNode;
				}
			}

			public void Remove(MyLinkedListNode<T> node)
			{
				if (node == null)
				{
					return;
				}
				_nextEmptyEndIndex = node.Index;
				if (node.Next != null)
				{
					node.Next.Previous = null;
					node.Next = null;
				}
				if (node.Previous != null)
				{
					_last = node.Previous;
					node.Previous.Next = null;
					node.Previous = null;
				}
				else
				{
					_first = (_last = null);
				}
				if (_dicIndex2Node.Count - Count > 4)
				{
					_dicIndex2Node.Clear();
					for (node = First; node != null; node = node.Next)
					{
						_dicIndex2Node[node.Index] = node;
					}
				}
			}

			public MyLinkedListNode<T> AddLast(T valuePair)
			{
				MyLinkedListNode<T> myLinkedListNode = new MyLinkedListNode<T>();
				_dicIndex2Node[_nextEmptyEndIndex] = myLinkedListNode;
				myLinkedListNode.Value = valuePair;
				myLinkedListNode.Index = _nextEmptyEndIndex++;
				myLinkedListNode.Previous = _last;
				if (_last != null)
				{
					_last.Next = myLinkedListNode;
				}
				_last = myLinkedListNode;
				if (_first == null)
				{
					_first = myLinkedListNode;
				}
				return myLinkedListNode;
			}

			public bool Contains(MyLinkedListNode<T> node)
			{
				return node != null && node.Index > _nextEmptyStartIndex && node.Index < _nextEmptyEndIndex && _dicIndex2Node.ContainsValue(node);
			}
		}

		private class MyLinkedListNode<T>
		{
			public int Index;

			public T Value;

			public MyLinkedListNode<T> Next;

			public MyLinkedListNode<T> Previous;
		}

		[CompilerGenerated]
		private sealed class _003CFillCanLinkCell_003Ec__AnonStorey1
		{
			internal long insId;

			internal DianluPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this._curSelectInfo == null)
				{
					AlertBox.Show(297);
					Debug.LogError("[DianluPanel.cs]No select btn_use show.");
					return;
				}
				long key;
				long key2;
				if (_0024this._isSelectHeadBlank)
				{
					key = insId;
					key2 = _0024this._curSelectInfo.Value.Key;
				}
				else
				{
					key = _0024this._curSelectInfo.Value.Key;
					key2 = insId;
				}
				if (_0024this.CanConnect(key, key2, _0024this._isSelectHeadBlank))
				{
					Singleton<ElectricityMgr>.Ins.SendConnectMsg(key, key2);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillFuelCell_003Ec__AnonStorey2
		{
			internal byte index;

			internal DianluPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this._selectBagMaterialIndex = index;
				_0024this.ShowMaterialCanAdd();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBagMaterialCell_003Ec__AnonStorey3
		{
			internal KeyValuePair<int, int> pair;

			internal DianluPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				PowerFuleInfo value;
				int itemId;
				int maxCanAddNum;
				if (_0024this._dicIndex2FuelInfo.TryGetValue(_0024this._selectBagMaterialIndex, out value))
				{
					if (value.fuelId != pair.Key)
					{
						AlertBox.Show(312);
						return;
					}
					itemId = value.fuelId;
					int num = cfg.Consts.MAX_PILE_NUM_ELECTRIC_FUEL - value.fuelNum;
					maxCanAddNum = ((num >= pair.Value) ? pair.Value : num);
				}
				else
				{
					itemId = pair.Key;
					int mAX_PILE_NUM_ELECTRIC_FUEL = cfg.Consts.MAX_PILE_NUM_ELECTRIC_FUEL;
					maxCanAddNum = ((mAX_PILE_NUM_ELECTRIC_FUEL >= pair.Value) ? pair.Value : mAX_PILE_NUM_ELECTRIC_FUEL);
				}
				ElectricAddItemPanel.ShowArg param = new ElectricAddItemPanel.ShowArg
				{
					InsId = _0024this._curSelectInfo.Value.Key,
					ItemId = itemId,
					MaxCanAddNum = maxCanAddNum,
					Index = _0024this._selectBagMaterialIndex
				};
				ViewMgr.Ins.ShowTopView<ElectricAddItemPanel>(param);
				_0024this.m_bag_materials.SetActiveBetter(false);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillElectricElementCell_003Ec__AnonStorey4
		{
			internal int index;

			internal MyLinkedListNode<KeyValuePair<long, int>> curNode;

			internal CircuitryInfo dataInfo;

			internal int indexInCurCell;

			internal DianluPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (index == 0)
				{
					_0024this._isSelectHeadBlank = true;
					if (_0024this.IsLinkPageShow())
					{
						_0024this.RebuildCanConnectDic();
						_0024this.UpdateLinkPanel(false);
					}
					else
					{
						_0024this.ShowLinkPage();
					}
					return;
				}
				_0024this._isSelectHeadBlank = false;
				MyLinkedListNode<KeyValuePair<long, int>> myLinkedListNode = _0024this._selectInsId2IndexList[index - 1];
				long key = myLinkedListNode.Value.Key;
				if (_0024this._curSelectInfo.Value.Key == key)
				{
					if (_0024this.IsLinkPageShow())
					{
						_0024this.UpdateLinkPanel(false);
					}
					else
					{
						_0024this.ShowLinkPage();
					}
					return;
				}
				_0024this._curSelectInfo = myLinkedListNode;
				if (_0024this.IsLinkPageShow())
				{
					_0024this.RebuildCanConnectDic();
					_0024this.RefreshLinkPage(false, true);
				}
				else
				{
					_0024this.ShowLinkPage();
				}
				_0024this.UpdateAllTree(true, true);
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this._isSelectHeadBlank = false;
				long num = ((curNode != null && curNode.Previous != null) ? curNode.Previous.Value.Key : 0);
				MyLinkedListNode<KeyValuePair<long, int>> myLinkedListNode = _0024this._selectInsId2IndexList.First;
				if (curNode == null)
				{
					KeyValuePair<long, int> valuePair = new KeyValuePair<long, int>(dataInfo.instanceId, indexInCurCell);
					_0024this._curSelectInfo = _0024this._selectInsId2IndexList.AddLast(valuePair);
				}
				else if (curNode.Value.Value != indexInCurCell)
				{
					while (myLinkedListNode != null)
					{
						if (myLinkedListNode.Value.Key == num)
						{
							_0024this._selectInsId2IndexList.Remove(myLinkedListNode.Next);
							break;
						}
						myLinkedListNode = myLinkedListNode.Next;
					}
					KeyValuePair<long, int> valuePair2 = new KeyValuePair<long, int>(dataInfo.instanceId, indexInCurCell);
					_0024this._curSelectInfo = _0024this._selectInsId2IndexList.AddLast(valuePair2);
				}
				else
				{
					_0024this._curSelectInfo = curNode;
				}
				if (_0024this.IsLinkPageShow())
				{
					_0024this.ShowSelectInfo();
				}
				else
				{
					_0024this.FillSelectInfo();
				}
				_0024this.UpdateAllTree(true, true);
			}
		}

		private const int AllTypeValue = -1;

		private const int MaxChildCountView = 5;

		private const int OneIntervalDistance = 160;

		private const int CompensationXBlack = 8;

		private const int CompensationXYellow = 6;

		private const int MiddleIndex = 2;

		private readonly Vector2 PaperUpOffset = new Vector2(0f, 74f);

		private readonly Vector2 PaperDownOffset = new Vector2(0f, -20f);

		private readonly MyLinkedList<KeyValuePair<long, int>> _selectInsId2IndexList = new MyLinkedList<KeyValuePair<long, int>>();

		private readonly Dictionary<int, List<long>> _dicType2CanLink = new Dictionary<int, List<long>>();

		private List<long> _curCanLinkList;

		private UIScrollPanel _linkScrollPanel;

		private UIScrollPanel _canLinkScrollPanel;

		private UIScrollPanel _bagMaterialScrollPanel;

		private RectTransform _descTrans;

		private RectTransform _delayBtnTrans;

		private InputField _inpDelayTime;

		private MyLinkedListNode<KeyValuePair<long, int>> _curSelectInfo;

		private int _totalWatt;

		private readonly WaitForSeconds _wait01 = new WaitForSeconds(0.1f);

		private int _curFunctionType;

		private Dictionary<byte, PowerFuleInfo> _dicIndex2FuelInfo;

		private byte _selectBagMaterialIndex;

		private readonly List<KeyValuePair<int, int>> _bagMaterialItemIds = new List<KeyValuePair<int, int>>();

		private float _finishTime;

		private bool _isSelectHeadBlank;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_link;

		private GameObject btn_kaiguan;

		private GameObject btn_chuanganqi;

		private GameObject btn_sheshi;

		private GameObject btn_dianyuan;

		private GElectricElementCell m_cell;

		private GameObject m_info;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject btn_rename;

		private GameObject btn_unload;

		private List<GElectricMaterialCell> m_materialslist = new List<GElectricMaterialCell>();

		private GameObject[] m_materials;

		private GameObject m_materialsObj;

		private GameObject txt_material;

		private Text txt_materialText;

		private GameObject scp_link_tree;

		private GameObject scp_element_can_link;

		private GameObject m_bag_materials;

		private GameObject scp_bag_material;

		private GameObject txt_desc;

		private Text txt_descText;

		private GameObject m_delay;

		private GameObject inp_num;

		private GameObject btn_reduce;

		private GameObject btn_add;

		private GameObject btn_confirm_delay;

		private GameObject m_frame;

		private GameObject m_icon;

		private GameObject btn_quanbu;

		private GameObject m_desc;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		private GameObject txt_on_3;

		private Text txt_on_3Text;

		private GameObject txt_off_3;

		private Text txt_off_3Text;

		private GElectricBagMaterialsCell m_cell_0;

		private GameObject txt_material_0;

		private Text txt_material_0Text;

		private GameObject m_delay_btn;

		private GElectricLinkCell m_cell_1;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			m_cell_0.gameObject.SetActiveBetter(false);
			m_info.SetActiveBetter(false);
			m_link.SetActiveBetter(false);
			btn_confirm_delay.SetActiveBetter(false);
			_linkScrollPanel = scp_link_tree.GetComponent<UIScrollPanel>();
			_canLinkScrollPanel = scp_element_can_link.GetComponent<UIScrollPanel>();
			_bagMaterialScrollPanel = scp_bag_material.GetComponent<UIScrollPanel>();
			_descTrans = m_desc.transform as RectTransform;
			_delayBtnTrans = m_delay_btn.transform as RectTransform;
			_inpDelayTime = inp_num.GetComponent<InputField>();
			_inpDelayTime.onValueChanged.AddListener(_003ConInit_003Em__0);
			ClickListener.Get(btn_confirm_delay, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_unload, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_rename, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_quanbu, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_kaiguan, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_chuanganqi, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener.Get(btn_sheshi, string.Empty).onClick = _003ConInit_003Em__7;
			ClickListener.Get(btn_dianyuan, string.Empty).onClick = _003ConInit_003Em__8;
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__9;
			ClickListener.Get(btn_add, string.Empty).onClick = _003ConInit_003Em__A;
			ClickListener.Get(btn_reduce, string.Empty).onClick = _003ConInit_003Em__B;
		}

		private void ShowConfirmBtn()
		{
			btn_confirm_delay.SetActiveBetter(true);
		}

		private void OnChangeSuccess()
		{
			btn_confirm_delay.SetActiveBetter(false);
			AlertBox.Show(274);
		}

		private void ClickRadioBtn(int functionType)
		{
			_curFunctionType = functionType;
			RefreshLinkPage(true);
		}

		private IEnumerator WaitHide()
		{
			yield return _wait01;
			Hide();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			if (param == null)
			{
				StartCoroutine(WaitHide());
				return;
			}
			_isSelectHeadBlank = false;
			CircuitryInfo info;
			if (!Singleton<ElectricityMgr>.Ins.GetInfoByInsId((long)param, out info))
			{
				StartCoroutine(WaitHide());
				return;
			}
			ElectricityEvent.OnSwitchDelayTimeRefreshDelegate = (Action<long, byte>)Delegate.Combine(ElectricityEvent.OnSwitchDelayTimeRefreshDelegate, new Action<long, byte>(UpdateDelayTime));
			ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate = (Action)Delegate.Combine(ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate, new Action(OnChangeSuccess));
			View.SetLabelText(txt_descText, string.Empty);
			BuildAllTree(info);
			CalTotalWatt();
			_curSelectInfo = _selectInsId2IndexList.Last;
			UpdateAllTree(false);
			ShowSelectInfo();
			OnMoneyChange();
			ElectricityEvent.ConnectDelegate = (Action<long, long>)Delegate.Combine(ElectricityEvent.ConnectDelegate, new Action<long, long>(OnConnect));
			ElectricityEvent.DisconnectDelegate = (Action<long, long>)Delegate.Combine(ElectricityEvent.DisconnectDelegate, new Action<long, long>(OnDisConnect));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SGetTreeByElementBuildingId.handler = (SGetTreeByElementBuildingId.Handler)Delegate.Combine(SGetTreeByElementBuildingId.handler, new SGetTreeByElementBuildingId.Handler(OnTreeRefresh));
			ElectricityEvent.UpdateFuelInfoDelegate = (Action<long, int, Dictionary<byte, PowerFuleInfo>>)Delegate.Combine(ElectricityEvent.UpdateFuelInfoDelegate, new Action<long, int, Dictionary<byte, PowerFuleInfo>>(OnUpdateFuelInfo));
			SAddFuel.handler = (SAddFuel.Handler)Delegate.Combine(SAddFuel.handler, new SAddFuel.Handler(OnSAddFuel));
			SGetFuel.handler = (SGetFuel.Handler)Delegate.Combine(SGetFuel.handler, new SGetFuel.Handler(OnSGetFuel));
			ElectricityEvent.OnSwitchStateChangeDelegate = (Action)Delegate.Combine(ElectricityEvent.OnSwitchStateChangeDelegate, new Action(OnSwitchStateChange));
			SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Combine(SChangeBuildingName.handler, new SChangeBuildingName.Handler(OnSChangeBuildingName));
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			_selectInsId2IndexList.Clear();
			Singleton<ElectricityMgr>.Ins.OnPanelHide();
			_dicType2CanLink.Clear();
			_curCanLinkList = null;
			ElectricityEvent.ConnectDelegate = (Action<long, long>)Delegate.Remove(ElectricityEvent.ConnectDelegate, new Action<long, long>(OnConnect));
			ElectricityEvent.DisconnectDelegate = (Action<long, long>)Delegate.Remove(ElectricityEvent.DisconnectDelegate, new Action<long, long>(OnDisConnect));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SGetTreeByElementBuildingId.handler = (SGetTreeByElementBuildingId.Handler)Delegate.Remove(SGetTreeByElementBuildingId.handler, new SGetTreeByElementBuildingId.Handler(OnTreeRefresh));
			ElectricityEvent.UpdateFuelInfoDelegate = (Action<long, int, Dictionary<byte, PowerFuleInfo>>)Delegate.Remove(ElectricityEvent.UpdateFuelInfoDelegate, new Action<long, int, Dictionary<byte, PowerFuleInfo>>(OnUpdateFuelInfo));
			SAddFuel.handler = (SAddFuel.Handler)Delegate.Remove(SAddFuel.handler, new SAddFuel.Handler(OnSAddFuel));
			SGetFuel.handler = (SGetFuel.Handler)Delegate.Remove(SGetFuel.handler, new SGetFuel.Handler(OnSGetFuel));
			ElectricityEvent.OnSwitchStateChangeDelegate = (Action)Delegate.Remove(ElectricityEvent.OnSwitchStateChangeDelegate, new Action(OnSwitchStateChange));
			SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Remove(SChangeBuildingName.handler, new SChangeBuildingName.Handler(OnSChangeBuildingName));
			ElectricityEvent.OnSwitchDelayTimeRefreshDelegate = (Action<long, byte>)Delegate.Remove(ElectricityEvent.OnSwitchDelayTimeRefreshDelegate, new Action<long, byte>(UpdateDelayTime));
			ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate = (Action)Delegate.Remove(ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate, new Action(OnChangeSuccess));
		}

		private void UpdateDelayTime(long insId, byte delay)
		{
			if (_curSelectInfo != null && _curSelectInfo.Value.Key == insId)
			{
				_inpDelayTime.text = delay.ToString();
				btn_confirm_delay.SetActiveBetter(false);
			}
		}

		private void OnSChangeBuildingName(SChangeBuildingName msg)
		{
			if (_curSelectInfo.Value.Key == msg.insId)
			{
				View.SetLabelText(txt_nameText, msg.name);
			}
		}

		private void OnSwitchStateChange()
		{
			UpdateAllTree(true);
			if (!IsLinkPageShow())
			{
				FillSelectInfo();
			}
		}

		private void OnSGetFuel(SGetFuel msg)
		{
			if (_curSelectInfo.Value.Key == msg.powerId && _dicIndex2FuelInfo != null)
			{
				CircuitryInfo info;
				Singleton<ElectricityMgr>.Ins.GetInfoByInsId(msg.powerId, out info);
				int num = 0;
				PowerFuleInfo value;
				if (_dicIndex2FuelInfo.TryGetValue(msg.gridIndex, out value))
				{
					int value2;
					PowerCfg.Get(info.cfgId).fuels.TryGetValue(value.fuelId, out value2);
					num = value2 * value.fuelNum;
				}
				_dicIndex2FuelInfo.Remove(msg.gridIndex);
				m_materialsObj.SetActiveBetter(true);
				UpdateFuelPanel();
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (_totalWatt > 0)
				{
					_finishTime = GetDifferenceMoreThanZero(_finishTime, realtimeSinceStartup) - (float)num / (float)_totalWatt;
				}
				else
				{
					_finishTime = realtimeSinceStartup;
				}
				SetDesc(CircuitryCfg.Get(info.cfgId));
			}
		}

		private void OnSAddFuel(SAddFuel msg)
		{
			if (_curSelectInfo.Value.Key == msg.powerId && _dicIndex2FuelInfo != null)
			{
				PowerFuleInfo value;
				if (_dicIndex2FuelInfo.TryGetValue(msg.gridIndex, out value))
				{
					value.fuelNum = msg.fuelNum;
				}
				else
				{
					value = new PowerFuleInfo();
					value.fuelId = msg.fuelId;
					value.fuelNum = msg.fuelNum;
					_dicIndex2FuelInfo[msg.gridIndex] = value;
				}
				m_materialsObj.SetActiveBetter(true);
				UpdateFuelPanel();
				CircuitryInfo info;
				Singleton<ElectricityMgr>.Ins.GetInfoByInsId(msg.powerId, out info);
				int value2;
				PowerCfg.Get(info.cfgId).fuels.TryGetValue(msg.fuelId, out value2);
				int num = value2 * msg.fuelNum;
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (_totalWatt > 0)
				{
					_finishTime = GetDifferenceMoreThanZero(_finishTime, realtimeSinceStartup) + (float)num / (float)_totalWatt;
				}
				else
				{
					_finishTime = realtimeSinceStartup;
				}
				SetDesc(CircuitryCfg.Get(info.cfgId));
			}
		}

		private float GetDifferenceMoreThanZero(float minuend, float subtrahend)
		{
			if (minuend < subtrahend)
			{
				return 0f;
			}
			return minuend - subtrahend;
		}

		private void OnUpdateFuelInfo(long insId, int remainTime, Dictionary<byte, PowerFuleInfo> fuels)
		{
			if (_curSelectInfo.Value.Key != insId)
			{
				return;
			}
			CircuitryInfo info;
			Singleton<ElectricityMgr>.Ins.GetInfoByInsId(insId, out info);
			BuildPart buildPart = BuildPart.Get(info.cfgId);
			if (Singleton<ElectricityMgr>.Ins.IsDianChi(buildPart.functionType))
			{
				if (remainTime < 0)
				{
					_finishTime = Time.realtimeSinceStartup;
				}
				else
				{
					_finishTime = Time.realtimeSinceStartup + (float)remainTime;
				}
				SetDesc(CircuitryCfg.Get(info.cfgId));
			}
			if (_curSelectInfo.Value.Key == insId)
			{
				_dicIndex2FuelInfo = fuels;
				m_materialsObj.SetActiveBetter(true);
				UpdateFuelPanel();
			}
		}

		private void BuildAllTree(CircuitryInfo node)
		{
			_totalWatt = 0;
			_selectInsId2IndexList.Clear();
			while (true)
			{
				HashSet<long> parentIds;
				Singleton<ElectricityMgr>.Ins.GetParentIds(node.instanceId, out parentIds);
				long value;
				CircuitryInfo info;
				if (!GetOneValueFromHashSet(parentIds, out value) || !Singleton<ElectricityMgr>.Ins.GetInfoByInsId(value, out info))
				{
					break;
				}
				List<long> childIds = info.childIds;
				int i = 0;
				for (int count = childIds.Count; i < count; i++)
				{
					if (childIds[i] == node.instanceId)
					{
						_selectInsId2IndexList.AddFirst(new KeyValuePair<long, int>(node.instanceId, i));
						break;
					}
				}
				node = info;
			}
			_selectInsId2IndexList.AddFirst(new KeyValuePair<long, int>(node.instanceId, 2));
		}

		private bool GetOneValueFromHashSet(HashSet<long> set, out long value)
		{
			value = 0L;
			if (set == null || set.Count == 0)
			{
				return false;
			}
			using (HashSet<long>.Enumerator enumerator = set.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					long num = (value = enumerator.Current);
					return true;
				}
			}
			return false;
		}

		private void CalTotalWatt()
		{
			MyLinkedListNode<KeyValuePair<long, int>> first = _selectInsId2IndexList.First;
			if (first != null)
			{
				long key = first.Value.Key;
				if (key <= 0)
				{
					key = first.Next.Value.Key;
				}
				_totalWatt = Singleton<ElectricityMgr>.Ins.GetTotalWattByHeadInsId(key, false);
			}
		}

		private void OnTreeRefresh(SGetTreeByElementBuildingId msg)
		{
			UpdateAllTree(true, true);
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void OnDisConnect(long parentId, long childId)
		{
			if (_curSelectInfo == null || _curSelectInfo.Value.Key != childId)
			{
				return;
			}
			MyLinkedListNode<KeyValuePair<long, int>> myLinkedListNode = _selectInsId2IndexList.First;
			bool flag = false;
			while (myLinkedListNode != null)
			{
				if (myLinkedListNode.Value.Key == childId)
				{
					_curSelectInfo = myLinkedListNode.Previous;
					_selectInsId2IndexList.Remove(myLinkedListNode);
					flag = true;
					break;
				}
				myLinkedListNode = myLinkedListNode.Next;
			}
			if (flag)
			{
				UpdateAllTree(false);
			}
			CalTotalWatt();
			RebuildCanConnectDic();
			if (IsLinkPageShow())
			{
				RefreshLinkPage(true, true);
			}
			else
			{
				ShowSelectInfo();
			}
		}

		private void OnConnect(long parentId, long childId)
		{
			if (_curSelectInfo != null && (_curSelectInfo.Value.Key == parentId || _curSelectInfo.Value.Key == childId))
			{
				CalTotalWatt();
				FillSelectInfo();
				if (_isSelectHeadBlank)
				{
					CircuitryInfo info;
					Singleton<ElectricityMgr>.Ins.GetInfoByInsId(childId, out info);
					BuildAllTree(info);
					_isSelectHeadBlank = false;
				}
				UpdateAllTree(false);
				if (IsLinkPageShow())
				{
					RebuildCanConnectDic();
					RefreshLinkPage(true, true);
				}
			}
		}

		private void ShowLinkPage()
		{
			m_link.SetActiveBetter(true);
			m_info.SetActiveBetter(false);
			_curFunctionType = -1;
			RadioButton.ChooseBtn(btn_quanbu);
			RebuildCanConnectDic();
			RefreshLinkPage(false);
		}

		private void RefreshLinkPage(bool isNoPos, bool isClear = false)
		{
			_dicType2CanLink.TryGetValue(_curFunctionType, out _curCanLinkList);
			UpdateLinkPanel(isNoPos, isClear);
		}

		private void RebuildCanConnectDic()
		{
			_dicType2CanLink.Clear();
			if (_isSelectHeadBlank)
			{
				HashSet<long> lastInsIds = Singleton<ElectricityMgr>.Ins.LastInsIds;
				{
					foreach (long item in lastInsIds)
					{
						if (item == _curSelectInfo.Value.Key)
						{
							continue;
						}
						CircuitryInfo info;
						Singleton<ElectricityMgr>.Ins.GetInfoByInsId(item, out info);
						CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
						BuildPart buildPart = BuildPart.Get(info.cfgId);
						if (circuitryCfg.childCount > 0 && circuitryCfg.childCount > info.childIds.Count)
						{
							int suitableFunctionType = GetSuitableFunctionType(buildPart.functionType);
							List<long> value;
							if (!_dicType2CanLink.TryGetValue(suitableFunctionType, out value))
							{
								value = new List<long>();
								_dicType2CanLink[suitableFunctionType] = value;
							}
							value.Add(item);
							if (!_dicType2CanLink.TryGetValue(-1, out value))
							{
								value = new List<long>();
								_dicType2CanLink[-1] = value;
							}
							value.Add(item);
						}
					}
					return;
				}
			}
			CircuitryInfo info2;
			if (!Singleton<ElectricityMgr>.Ins.GetInfoByInsId(0L, out info2))
			{
				return;
			}
			List<long> childIds = info2.childIds;
			foreach (long item2 in childIds)
			{
				if (item2 == _curSelectInfo.Value.Key)
				{
					continue;
				}
				CircuitryInfo info3;
				Singleton<ElectricityMgr>.Ins.GetInfoByInsId(item2, out info3);
				BuildPart buildPart2 = BuildPart.Get(info3.cfgId);
				CircuitryCfg circuitryCfg2 = CircuitryCfg.Get(info3.cfgId);
				HashSet<long> parentIds;
				if (!Singleton<ElectricityMgr>.Ins.GetParentIds(item2, out parentIds) || parentIds.Count < circuitryCfg2.parentCount)
				{
					int suitableFunctionType2 = GetSuitableFunctionType(buildPart2.functionType);
					List<long> value2;
					if (!_dicType2CanLink.TryGetValue(suitableFunctionType2, out value2))
					{
						value2 = new List<long>();
						_dicType2CanLink[suitableFunctionType2] = value2;
					}
					value2.Add(item2);
					if (!_dicType2CanLink.TryGetValue(-1, out value2))
					{
						value2 = new List<long>();
						_dicType2CanLink[-1] = value2;
					}
					value2.Add(item2);
				}
			}
		}

		private int GetSuitableFunctionType(int functionType)
		{
			if (functionType == 17)
			{
				return 15;
			}
			return functionType;
		}

		private void UpdateLinkPanel(bool isNoPos, bool isClear = false)
		{
			if (isClear)
			{
				_canLinkScrollPanel.Clear();
			}
			if (isNoPos)
			{
				_canLinkScrollPanel.ResetNoPosClear((_curCanLinkList != null) ? _curCanLinkList.Count : 0, FillCanLinkCell);
			}
			else
			{
				_canLinkScrollPanel.Reset((_curCanLinkList != null) ? _curCanLinkList.Count : 0, FillCanLinkCell);
			}
		}

		private void FillCanLinkCell(GameObject go, int index)
		{
			_003CFillCanLinkCell_003Ec__AnonStorey1 _003CFillCanLinkCell_003Ec__AnonStorey = new _003CFillCanLinkCell_003Ec__AnonStorey1();
			_003CFillCanLinkCell_003Ec__AnonStorey._0024this = this;
			GElectricElementCell component = go.GetComponent<GElectricElementCell>();
			_003CFillCanLinkCell_003Ec__AnonStorey.insId = _curCanLinkList[index];
			CircuitryInfo info;
			Singleton<ElectricityMgr>.Ins.GetInfoByInsId(_003CFillCanLinkCell_003Ec__AnonStorey.insId, out info);
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			ItemCfg itemCfg = ItemCfg.Get(circuitryCfg.id);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_nameText, info.name);
			View.SetLabelText(component.txt_descText, itemCfg.desc);
			View.SetLabelText(component.txt_gonglvText, Utils.GetString(313, circuitryCfg.power));
			HashSet<long> parentIds;
			bool flag = (_isSelectHeadBlank ? (!Singleton<ElectricityMgr>.Ins.CanLinkChildForChildCount(_003CFillCanLinkCell_003Ec__AnonStorey.insId)) : (Singleton<ElectricityMgr>.Ins.GetParentIds(_003CFillCanLinkCell_003Ec__AnonStorey.insId, out parentIds) && (parentIds.Count >= circuitryCfg.parentCount || parentIds.Contains(_curSelectInfo.Value.Key))));
			component.btn_use.SetActiveBetter(!flag);
			component.txt_used.SetActiveBetter(flag);
			ClickListener.Get(component.gameObject, string.Empty).onClick = _003CFillCanLinkCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private bool CanConnect(long parentId, long childId, bool isSelectHeadBlank)
		{
			if (parentId == childId)
			{
				Debug.LogError("[DianluPanel.cs]Connect to self.");
				return false;
			}
			if (!Singleton<ElectricityMgr>.Ins.CanLinkChildForChildCount(parentId))
			{
				AlertBox.Show(298);
				return false;
			}
			long headInsIdOfLast;
			if (!NotOverTreeStratum(parentId, childId, out headInsIdOfLast))
			{
				AlertBox.Show(309);
				return false;
			}
			return true;
		}

		private bool NotOverTreeStratum(long lastInsId, long firstInsId, out long headInsIdOfLast)
		{
			int num = 0;
			List<long>[] array = new List<long>[2]
			{
				new List<long>(),
				new List<long>()
			};
			array[0].Add(firstInsId);
			while (true)
			{
				List<long> list = array[num % 2];
				int count = list.Count;
				if (count <= 0)
				{
					break;
				}
				num++;
				List<long> list2 = array[num % 2];
				list2.Clear();
				for (int i = 0; i < count; i++)
				{
					long insId = list[i];
					CircuitryInfo info;
					if (Singleton<ElectricityMgr>.Ins.GetInfoByInsId(insId, out info))
					{
						list2.AddRange(info.childIds);
					}
				}
			}
			int stratum;
			GetFirstInsIdAndStratumByLastInsId(lastInsId, out headInsIdOfLast, out stratum);
			num += stratum;
			return num <= cfg.Consts.ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX;
		}

		public void GetFirstInsIdAndStratumByLastInsId(long lastInsId, out long firstInsId, out int stratum)
		{
			stratum = 0;
			firstInsId = lastInsId;
			while (true)
			{
				stratum++;
				HashSet<long> parentIds;
				if (!Singleton<ElectricityMgr>.Ins.GetParentIds(lastInsId, out parentIds) || !GetOneValueFromHashSet(parentIds, out lastInsId) || lastInsId <= 0)
				{
					break;
				}
				firstInsId = lastInsId;
			}
		}

		private void ShowSelectInfo()
		{
			m_info.SetActiveBetter(true);
			m_link.SetActiveBetter(false);
			FillSelectInfo();
		}

		private void FillSelectInfo()
		{
			m_bag_materials.SetActiveBetter(false);
			btn_unload.SetActiveBetter(_curSelectInfo != null && _curSelectInfo != _selectInsId2IndexList.First);
			if (_curSelectInfo == null)
			{
				return;
			}
			CircuitryInfo info;
			Singleton<ElectricityMgr>.Ins.GetInfoByInsId(_curSelectInfo.Value.Key, out info);
			BuildPart buildPart = BuildPart.Get(info.cfgId);
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			View.SetLabelText(txt_nameText, info.name);
			View.SetItemSprite(m_icon, buildPart.icon);
			SetDesc(circuitryCfg);
			if (Singleton<ElectricityMgr>.Ins.IsDianChi(buildPart.functionType))
			{
				m_materialsObj.SetActiveBetter(true);
				Singleton<ElectricityMgr>.Ins.SendRequestPowerInfoMsg(info.instanceId);
				m_delay.SetActiveBetter(false);
				_descTrans.anchoredPosition = PaperUpOffset;
			}
			else if (Singleton<ElectricityMgr>.Ins.IsDelaySwitch(circuitryCfg))
			{
				m_materialsObj.SetActiveBetter(false);
				m_delay.SetActiveBetter(true);
				byte delayTime;
				if (!Singleton<ElectricityMgr>.Ins.GetDelaySwitchTime(info.instanceId, out delayTime))
				{
					delayTime = (byte)cfg.Consts.ELECTRIC_DELAY_SWITCH_DEFAULT_TIME;
					Singleton<ElectricityMgr>.Ins.SendRequestSwitchInfoMsg(_curSelectInfo.Value.Key);
				}
				_inpDelayTime.text = delayTime.ToString();
				_descTrans.anchoredPosition = PaperUpOffset;
				btn_confirm_delay.SetActiveBetter(false);
			}
			else
			{
				m_materialsObj.SetActiveBetter(false);
				m_delay.SetActiveBetter(false);
				_descTrans.anchoredPosition = PaperDownOffset;
			}
		}

		private void SetDesc(CircuitryCfg cirCfgInfo)
		{
			View.SetLabelText(txt_descText, Utils.GetString(cirCfgInfo.desc, Utils.GetString(313, _totalWatt), Utils.GetString(313, cirCfgInfo.power), Utils.GetSurplusTimeString((int)(_finishTime - Time.realtimeSinceStartup))));
		}

		private void UpdateFuelPanel()
		{
			if (_dicIndex2FuelInfo == null)
			{
				m_materialsObj.SetActiveBetter(false);
				return;
			}
			int i = 0;
			for (int num = m_materials.Length; i < num; i++)
			{
				byte b = (byte)i;
				PowerFuleInfo value;
				_dicIndex2FuelInfo.TryGetValue(b, out value);
				FillFuelCell(m_materialslist[i], value, b);
			}
		}

		private void FillFuelCell(GElectricMaterialCell cell, PowerFuleInfo info, byte index)
		{
			_003CFillFuelCell_003Ec__AnonStorey2 _003CFillFuelCell_003Ec__AnonStorey = new _003CFillFuelCell_003Ec__AnonStorey2();
			_003CFillFuelCell_003Ec__AnonStorey.index = index;
			_003CFillFuelCell_003Ec__AnonStorey._0024this = this;
			if (info == null)
			{
				cell.m_icon.SetActiveBetter(false);
				cell.txt_num.SetActiveBetter(false);
			}
			else
			{
				cell.m_icon.SetActiveBetter(true);
				cell.txt_num.SetActiveBetter(true);
				ItemCfg itemCfg = ItemCfg.Get(info.fuelId);
				View.SetItemSprite(cell.m_icon, itemCfg.icon);
				View.SetLabelText(cell.txt_numText, info.fuelNum);
			}
			ClickListener.Get(cell.gameObject, string.Empty).onClick = _003CFillFuelCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void ShowMaterialCanAdd()
		{
			m_bag_materials.SetActiveBetter(true);
			_bagMaterialItemIds.Clear();
			CircuitryInfo info;
			Singleton<ElectricityMgr>.Ins.GetInfoByInsId(_curSelectInfo.Value.Key, out info);
			PowerCfg powerCfg = PowerCfg.Get(info.cfgId);
			Dictionary<int, int> fuels = powerCfg.fuels;
			foreach (int key in fuels.Keys)
			{
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(key);
				if (itemNum > 0)
				{
					KeyValuePair<int, int> item = new KeyValuePair<int, int>(key, itemNum);
					_bagMaterialItemIds.Add(item);
				}
			}
			_bagMaterialScrollPanel.Reset(_bagMaterialItemIds.Count + 1, FillBagMaterialCell);
		}

		private void FillBagMaterialCell(GameObject go, int index)
		{
			GElectricBagMaterialsCell component = go.GetComponent<GElectricBagMaterialsCell>();
			if (index == 0)
			{
				component.m_unload.SetActiveBetter(true);
				component.m_part.SetActiveBetter(false);
				ClickListener.Get(component.m_unload, string.Empty).onClick = _003CFillBagMaterialCell_003Em__C;
				return;
			}
			_003CFillBagMaterialCell_003Ec__AnonStorey3 _003CFillBagMaterialCell_003Ec__AnonStorey = new _003CFillBagMaterialCell_003Ec__AnonStorey3();
			_003CFillBagMaterialCell_003Ec__AnonStorey._0024this = this;
			component.m_unload.SetActiveBetter(false);
			component.m_part.SetActiveBetter(true);
			_003CFillBagMaterialCell_003Ec__AnonStorey.pair = _bagMaterialItemIds[index - 1];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBagMaterialCell_003Ec__AnonStorey.pair.Key);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_nameText, itemCfg.name);
			ClickListener.Get(component.m_part, string.Empty).onClick = _003CFillBagMaterialCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateAllTree(bool isNoPos, bool isClear = false)
		{
			int num = CalCompensationValue();
			if (isClear)
			{
				_linkScrollPanel.Clear();
			}
			if (isNoPos)
			{
				_linkScrollPanel.ResetNoPos(_selectInsId2IndexList.Count + num, FillLinkCell);
			}
			else
			{
				_linkScrollPanel.Reset(_selectInsId2IndexList.Count + num, FillLinkCell);
			}
		}

		private int CalCompensationValue()
		{
			CircuitryInfo info;
			if (!Singleton<ElectricityMgr>.Ins.GetInfoByInsId(_selectInsId2IndexList.Last.Value.Key, out info))
			{
				return 0;
			}
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			return (_selectInsId2IndexList.Count < cfg.Consts.ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX && circuitryCfg != null && circuitryCfg.childCount > 0) ? 1 : 0;
		}

		private void FillLinkCell(GameObject go, int index)
		{
			GElectricLinkCell component = go.GetComponent<GElectricLinkCell>();
			MyLinkedListNode<KeyValuePair<long, int>> myLinkedListNode = _selectInsId2IndexList[index];
			component.m_elementslist[2].m_connect_shuxian.SetActiveBetter(index != 0);
			KeyValuePair<long, int> keyValuePair;
			if (index == 0)
			{
				keyValuePair = new KeyValuePair<long, int>(-1L, 2);
			}
			else
			{
				MyLinkedListNode<KeyValuePair<long, int>> myLinkedListNode2 = ((myLinkedListNode != null && myLinkedListNode.Previous != null) ? myLinkedListNode.Previous : _selectInsId2IndexList.Last);
				keyValuePair = myLinkedListNode2.Value;
			}
			int childCount = 0;
			CircuitryInfo info;
			List<long> list;
			if (Singleton<ElectricityMgr>.Ins.GetInfoByInsId(keyValuePair.Key, out info))
			{
				CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
				childCount = circuitryCfg.childCount;
				list = info.childIds;
			}
			else
			{
				list = new List<long>();
				list.Add(myLinkedListNode.Value.Key);
			}
			GameObject[] elements = component.m_elements;
			List<GElectricLinkElementCell> elementslist = component.m_elementslist;
			int num = elementslist.Count - 1;
			int startIndex;
			int endIndex;
			CalChildStartEnd(num, childCount, keyValuePair.Value, out startIndex, out endIndex);
			bool flag = false;
			int i = 0;
			for (int count = elementslist.Count; i < count; i++)
			{
				if (i < startIndex || i > endIndex)
				{
					elements[i].SetActiveBetter(false);
					continue;
				}
				elements[i].SetActiveBetter(true);
				int num2 = i - startIndex;
				if (list.Count > num2)
				{
					long num3 = list[num2];
					CircuitryInfo info2;
					Singleton<ElectricityMgr>.Ins.GetInfoByInsId(num3, out info2);
					GElectricLinkElementCell gElectricLinkElementCell = elementslist[i];
					FillElectricElementCell(index, i, gElectricLinkElementCell, info2, _curSelectInfo.Index - _selectInsId2IndexList.First.Index == index && _curSelectInfo.Value.Key == num3 && i == _curSelectInfo.Value.Value, myLinkedListNode);
					if (info2 == null)
					{
						flag = true;
						component.m_child_no_h_line.SetActiveBetter(false);
						component.m_child_yellow_h_line.SetActiveBetter(false);
						component.m_child_no_v_line.SetActiveBetter(false);
						component.m_child_yellow_v_line.SetActiveBetter(true);
						SetChildVLine(component.m_child_yellow_v_line, gElectricLinkElementCell.gameObject);
						SetChildLineCommon(component.m_child_yellow_h_line, 2, 2);
					}
					else
					{
						if (_selectInsId2IndexList.Count <= index || myLinkedListNode.Value.Value != i)
						{
							continue;
						}
						CircuitryCfg circuitryCfg2 = CircuitryCfg.Get(info2.cfgId);
						flag = circuitryCfg2.childCount > 0 && index + 1 < cfg.Consts.ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX;
						if (!flag)
						{
							continue;
						}
						int childCount2 = circuitryCfg2.childCount;
						List<long> childIds = info2.childIds;
						int count2 = childIds.Count;
						bool flag2 = count2 == childCount2;
						bool flag3 = count2 > 0;
						int startIndex2;
						int endIndex2;
						CalChildStartEnd(num, childCount2, i, out startIndex2, out endIndex2);
						if (flag3)
						{
							component.m_child_no_v_line.SetActiveBetter(false);
							component.m_child_yellow_v_line.SetActiveBetter(true);
							SetChildVLine(component.m_child_yellow_v_line, gElectricLinkElementCell.gameObject);
							component.m_child_yellow_h_line.SetActiveBetter(true);
							int num4 = startIndex2 + count2 - 1;
							num4 = ((num4 <= myLinkedListNode.Value.Value) ? myLinkedListNode.Value.Value : num4);
							SetChildLineCommon(component.m_child_yellow_h_line, startIndex2, num4);
							if (Singleton<ElectricityMgr>.Ins.IsChildElectrify(info2.instanceId))
							{
								int start;
								int end;
								CalChildTongDianStartEnd(myLinkedListNode, num, info2, i, out start, out end);
								if (start <= num && end >= 0)
								{
									component.m_child_tonglediandehengxian.SetActiveBetter(true);
									component.m_child_tonglediandeshuxian.SetActiveBetter(true);
									SetChildVLine(component.m_child_tonglediandeshuxian, gElectricLinkElementCell.gameObject);
									int lineEndIndex = ((end <= myLinkedListNode.Value.Value) ? myLinkedListNode.Value.Value : end);
									SetChildLineCommon(component.m_child_tonglediandehengxian, start, lineEndIndex);
								}
								else
								{
									component.m_child_tonglediandehengxian.SetActiveBetter(false);
									component.m_child_tonglediandeshuxian.SetActiveBetter(false);
								}
							}
							else
							{
								component.m_child_tonglediandehengxian.SetActiveBetter(false);
								component.m_child_tonglediandeshuxian.SetActiveBetter(false);
							}
						}
						else
						{
							component.m_child_no_v_line.SetActiveBetter(true);
							component.m_child_yellow_v_line.SetActiveBetter(false);
							SetChildVLine(component.m_child_no_v_line, gElectricLinkElementCell.gameObject);
							component.m_child_yellow_h_line.SetActiveBetter(false);
							component.m_child_tonglediandehengxian.SetActiveBetter(false);
							component.m_child_tonglediandeshuxian.SetActiveBetter(false);
						}
						if (flag2)
						{
							component.m_child_no_h_line.SetActiveBetter(false);
							continue;
						}
						component.m_child_no_h_line.SetActiveBetter(true);
						SetChildLineCommon(component.m_child_no_h_line, startIndex2, endIndex2);
					}
				}
				else
				{
					FillElectricElementCell(index, i, elementslist[i], null, false);
				}
			}
			component.m_child_line.SetActiveBetter(flag);
		}

		private void CalChildTongDianStartEnd(MyLinkedListNode<KeyValuePair<long, int>> node, int maxEnd, CircuitryInfo curStratumSelectDataInfo, int curStratumSelectIndex, out int start, out int end)
		{
			start = maxEnd + 1;
			end = -1;
			if (curStratumSelectDataInfo == null || curStratumSelectDataInfo.childIds.Count <= 0)
			{
				return;
			}
			List<long> childIds = curStratumSelectDataInfo.childIds;
			int i = 0;
			for (int count = childIds.Count; i < count; i++)
			{
				if (ConnectToBatteryThisLine(node))
				{
					if (start > i)
					{
						start = i;
					}
					if (end < i)
					{
						end = i;
					}
				}
			}
			GetSuitableStartEnd(maxEnd, ref start, ref end);
		}

		private void CalChildStartEnd(int maxEnd, int childCount, int parentIndex, out int startIndex, out int endIndex)
		{
			if (childCount == 0)
			{
				startIndex = (endIndex = 2);
				return;
			}
			startIndex = parentIndex - childCount / 2;
			endIndex = parentIndex + (childCount - 1) / 2;
			GetSuitableStartEnd(maxEnd, ref startIndex, ref endIndex);
		}

		private void GetSuitableStartEnd(int maxEnd, ref int startIndex, ref int endIndex)
		{
			if (endIndex - startIndex > maxEnd)
			{
				Debug.LogError("[DianluPanel.cs]Wrong index.");
				return;
			}
			if (startIndex < 0)
			{
				endIndex -= startIndex;
				startIndex = 0;
			}
			if (endIndex > maxEnd)
			{
				startIndex -= endIndex - maxEnd;
				endIndex = maxEnd;
			}
		}

		private void FillElectricElementCell(int index, int indexInCurCell, GElectricLinkElementCell cell, CircuitryInfo dataInfo, bool isSelect, MyLinkedListNode<KeyValuePair<long, int>> curNode = null)
		{
			_003CFillElectricElementCell_003Ec__AnonStorey4 _003CFillElectricElementCell_003Ec__AnonStorey = new _003CFillElectricElementCell_003Ec__AnonStorey4();
			_003CFillElectricElementCell_003Ec__AnonStorey.index = index;
			_003CFillElectricElementCell_003Ec__AnonStorey.curNode = curNode;
			_003CFillElectricElementCell_003Ec__AnonStorey.dataInfo = dataInfo;
			_003CFillElectricElementCell_003Ec__AnonStorey.indexInCurCell = indexInCurCell;
			_003CFillElectricElementCell_003Ec__AnonStorey._0024this = this;
			if (_003CFillElectricElementCell_003Ec__AnonStorey.dataInfo == null)
			{
				cell.m_connect.SetActiveBetter(false);
				cell.m_not_connect.SetActiveBetter(true);
				ClickListener.Get(cell.btn_add, string.Empty).onClick = _003CFillElectricElementCell_003Ec__AnonStorey._003C_003Em__0;
				return;
			}
			cell.m_connect.SetActiveBetter(true);
			cell.m_not_connect.SetActiveBetter(false);
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(_003CFillElectricElementCell_003Ec__AnonStorey.dataInfo.cfgId);
			View.SetItemSprite(cell.m_icon, ItemCfg.Get(circuitryCfg.id).icon);
			cell.m_select.SetActiveBetter(isSelect);
			cell.m_jiajianhao.SetActiveBetter(circuitryCfg.childCount > 0);
			cell.m_jiahao.SetActiveBetter(!isSelect);
			cell.m_jianhao.SetActiveBetter(isSelect);
			cell.m_tongdian_shuxian.SetActiveBetter(_003CFillElectricElementCell_003Ec__AnonStorey.index != 0 && ConnectToBatteryThisLine(_003CFillElectricElementCell_003Ec__AnonStorey.curNode));
			ClickListener.Get(cell.m_connect, string.Empty).onClick = _003CFillElectricElementCell_003Ec__AnonStorey._003C_003Em__1;
		}

		private bool ConnectToBatteryThisLine(MyLinkedListNode<KeyValuePair<long, int>> node)
		{
			if (node == null)
			{
				return _selectInsId2IndexList.Last != null && Singleton<ElectricityMgr>.Ins.IsChildElectrify(_selectInsId2IndexList.Last.Value.Key);
			}
			if (Singleton<ElectricityMgr>.Ins.IsDianChi(node.Value.Key))
			{
				return true;
			}
			node = node.Previous;
			if (node != null)
			{
				return Singleton<ElectricityMgr>.Ins.IsChildElectrify(node.Value.Key);
			}
			return false;
		}

		private void SetChildVLine(GameObject vLine, GameObject parentGo)
		{
			RectTransform rectTransform = vLine.transform as RectTransform;
			float x = (parentGo.transform as RectTransform).anchoredPosition.x;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.x = x;
			rectTransform.anchoredPosition = anchoredPosition;
		}

		private void SetChildLineCommon(GameObject line, int lineStartIndex, int lineEndIndex)
		{
			RectTransform rectTransform = line.transform as RectTransform;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.x = 6 + (lineEndIndex - lineStartIndex) * 160;
			rectTransform.sizeDelta = sizeDelta;
			sizeDelta = rectTransform.anchoredPosition;
			sizeDelta.x = (lineStartIndex + lineEndIndex + 1 - 5) * 80;
			rectTransform.anchoredPosition = sizeDelta;
			line.SetActiveBetter(true);
		}

		private void SetChildHLineBlackComplex(GameObject bLine, int parentIndex, int connectedChildCount, int cfgChildStartIndex, int cfgChildEndIndex)
		{
			int num = cfgChildStartIndex + connectedChildCount - 1;
			int num2 = ((num <= parentIndex) ? parentIndex : num);
			if (num2 != cfgChildEndIndex)
			{
				SetChildLineCommon(bLine, num2, cfgChildEndIndex);
			}
		}

		private bool IsLinkPageShow()
		{
			return m_link.activeSelf;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_blood = component.GameObjects[1].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[2].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[3].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[5].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_link = component.GameObjects[6].gameObject;
			btn_kaiguan = component.GameObjects[7].gameObject;
			btn_chuanganqi = component.GameObjects[8].gameObject;
			btn_sheshi = component.GameObjects[9].gameObject;
			btn_dianyuan = component.GameObjects[10].gameObject;
			m_cell = View.AddComponentIfNotExist<GElectricElementCell>(component.GameObjects[11].gameObject);
			m_info = component.GameObjects[12].gameObject;
			txt_name = component.GameObjects[13].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			btn_rename = component.GameObjects[14].gameObject;
			btn_unload = component.GameObjects[15].gameObject;
			m_materials = component.GameObjects[16].gameObject.GetComponent<UIGameObjectList>().objects;
			m_materialsObj = component.GameObjects[16].gameObject;
			if (m_materialslist.Count <= 0)
			{
				for (int i = 0; i < m_materials.Length; i++)
				{
					m_materialslist.Add(View.AddComponentIfNotExist<GElectricMaterialCell>(m_materials[i].gameObject));
				}
			}
			txt_material = component.GameObjects[17].gameObject;
			txt_materialText = txt_material.GetComponent<Text>();
			scp_link_tree = component.GameObjects[18].gameObject;
			scp_element_can_link = component.GameObjects[19].gameObject;
			m_bag_materials = component.GameObjects[20].gameObject;
			scp_bag_material = component.GameObjects[21].gameObject;
			txt_desc = component.GameObjects[22].gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			m_delay = component.GameObjects[23].gameObject;
			inp_num = component.GameObjects[24].gameObject;
			btn_reduce = component.GameObjects[25].gameObject;
			btn_add = component.GameObjects[26].gameObject;
			btn_confirm_delay = component.GameObjects[27].gameObject;
			m_frame = component.GameObjects[28].gameObject;
			m_icon = component.GameObjects[29].gameObject;
			btn_quanbu = component.GameObjects[30].gameObject;
			m_desc = component.GameObjects[31].gameObject;
			txt_on_0 = component.GameObjects[32].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[33].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[34].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_1 = component.GameObjects[35].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[36].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_2 = component.GameObjects[37].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			txt_on_3 = component.GameObjects[38].gameObject;
			txt_on_3Text = txt_on_3.GetComponent<Text>();
			txt_off_3 = component.GameObjects[39].gameObject;
			txt_off_3Text = txt_off_3.GetComponent<Text>();
			m_cell_0 = View.AddComponentIfNotExist<GElectricBagMaterialsCell>(component.GameObjects[40].gameObject);
			txt_material_0 = component.GameObjects[41].gameObject;
			txt_material_0Text = txt_material_0.GetComponent<Text>();
			m_delay_btn = component.GameObjects[42].gameObject;
			m_cell_1 = View.AddComponentIfNotExist<GElectricLinkCell>(component.GameObjects[43].gameObject);
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(string str)
		{
			if (_curSelectInfo == null)
			{
				Debug.LogError("[DianluPanel.cs]No select,but can change delay time.");
				return;
			}
			byte b = Convert.ToByte(str);
			if (b > cfg.Consts.ELECTRIC_DELAY_SWITCH_TIME_MAX)
			{
				b = (byte)cfg.Consts.ELECTRIC_DELAY_SWITCH_TIME_MAX;
				_inpDelayTime.text = b.ToString();
			}
			else if (b < cfg.Consts.ELECTRIC_DELAY_SWITCH_TIME_MIN)
			{
				b = (byte)cfg.Consts.ELECTRIC_DELAY_SWITCH_TIME_MIN;
				_inpDelayTime.text = b.ToString();
			}
			ShowConfirmBtn();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			byte b = Convert.ToByte(_inpDelayTime.text);
			byte delayTime;
			if (!Singleton<ElectricityMgr>.Ins.GetDelaySwitchTime(_curSelectInfo.Value.Key, out delayTime) || delayTime != b)
			{
				Singleton<ElectricityMgr>.Ins.SendSetSwitchDelayMsg(_curSelectInfo.Value.Key, b);
			}
			else
			{
				OnChangeSuccess();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (_curSelectInfo == null)
			{
				Debug.LogError("[DianluPanel.cs]No Select.");
				return;
			}
			KeyValuePair<long, int> value = _curSelectInfo.Value;
			if (_curSelectInfo.Previous == null || !Singleton<ElectricityMgr>.Ins.ContainsInsIdDataInfo(_curSelectInfo.Previous.Value.Key))
			{
				AlertBox.Show(302);
			}
			else
			{
				Singleton<ElectricityMgr>.Ins.SendDisconnectMsg(_curSelectInfo.Previous.Value.Key, value.Key);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			FacilityRenamePanel.Show(Singleton<StructureMenuMgr>.Ins.SendChangeBuildingNameMsg, txt_nameText.text, _curSelectInfo.Value.Key);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			ClickRadioBtn(-1);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			ClickRadioBtn(14);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			ClickRadioBtn(15);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			ClickRadioBtn(16);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			ClickRadioBtn(13);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__9(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__A(GameObject go)
		{
			int num = Convert.ToInt32(_inpDelayTime.text);
			_inpDelayTime.text = (num + 1).ToString();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__B(GameObject go)
		{
			int num = Convert.ToInt32(_inpDelayTime.text);
			_inpDelayTime.text = (num - 1).ToString();
		}

		[CompilerGenerated]
		private void _003CFillBagMaterialCell_003Em__C(GameObject o)
		{
			PowerFuleInfo value;
			if (_dicIndex2FuelInfo.TryGetValue(_selectBagMaterialIndex, out value))
			{
				if (value.fuelNum > Singleton<BagMgr>.Ins.GetRemainCapacity(value.fuelId, value.fuelNum))
				{
					AlertBox.Show(26);
					return;
				}
				Singleton<ElectricityMgr>.Ins.SendGetFuelMsg(_curSelectInfo.Value.Key, _selectBagMaterialIndex);
			}
			m_bag_materials.SetActiveBetter(false);
		}
	}
}
