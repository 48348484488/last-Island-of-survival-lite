using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.shop.scmsg;

namespace SC.UI
{
	public class ShopTimePage : MonoBehaviour, IShopPage
	{
		[CompilerGenerated]
		private sealed class _003CFillTimeShopCell_003Ec__AnonStorey0
		{
			internal int index;

			internal ShopCfg shopInfo;

			internal ShopTimePage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this.SelectItem(index, shopInfo);
				_0024this.UpdateTimeShopPanel(true);
			}
		}

		private const int IntervalTime = 60;

		private float _passTime;

		private UIScrollPanel _timeScrollPanel;

		private InputField _inpNum;

		private int _maxCanBuy;

		private bool _isSelectDiscount;

		private int _selectFullPrice;

		private int _selectRealPrice;

		private int _selectIndex;

		private GameObject _lastGo;

		private List<RandomShopItemInfo> _randomItemInfoList;

		private Transform _modelParentTrans;

		private RenderTexture _renderTexture;

		private Camera _cam;

		private float _gunLength = 4f;

		public GameObject btn_add;

		public GameObject btn_buy;

		public GameObject btn_max;

		public GameObject btn_reduce;

		public GameObject btn_refresh;

		public GameObject inp_num;

		public GameObject m_camera;

		public GTimeShopCell m_cell;

		public GameObject m_count_down;

		public GameObject m_coupon_total;

		public GameObject m_gold_total;

		public GameObject m_hero_tex;

		public GameObject m_item_image;

		public GameObject m_item_image_parent;

		public GameObject m_model_equip_root;

		public GameObject m_model_role;

		public GameObject m_model_root;

		public GameObject m_model_skin_root;

		public GameObject m_refresh_money_icon;

		public GameObject m_role_bg;

		public GameObject scp_time_shop;

		public GameObject txt_count_down;

		public Text txt_count_downText;

		public GameObject txt_desc;

		public Text txt_descText;

		public GameObject txt_full_total_coupon;

		public Text txt_full_total_couponText;

		public GameObject txt_full_total_gold;

		public Text txt_full_total_goldText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_real_total_coupon;

		public Text txt_real_total_couponText;

		public GameObject txt_real_total_gold;

		public Text txt_real_total_goldText;

		public GameObject txt_refresh_cost;

		public Text txt_refresh_costText;

		public object context;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		private int SelectIndex
		{
			get
			{
				return _selectIndex;
			}
			set
			{
				UpdateSelectCanBuyNum(value);
				_selectIndex = value;
			}
		}

		private void UpdateSelectCanBuyNum(int index)
		{
			int indexLimitNum = Singleton<TimeShopMgr>.Ins.GetIndexLimitNum(index);
			ShopCfg shopCfgByIndex = Singleton<TimeShopMgr>.Ins.GetShopCfgByIndex(index);
			if (shopCfgByIndex != null)
			{
				int buyNumLimit = shopCfgByIndex.buyNumLimit;
				_maxCanBuy = ((buyNumLimit >= indexLimitNum) ? indexLimitNum : buyNumLimit);
			}
			else
			{
				_maxCanBuy = indexLimitNum;
			}
			_isSelectDiscount = Singleton<TimeShopMgr>.Ins.GetOneCost(index, out _selectRealPrice, out _selectFullPrice);
		}

		public void OnInit()
		{
			m_cell.gameObject.SetActiveBetter(false);
			_timeScrollPanel = scp_time_shop.GetComponent<UIScrollPanel>();
			_inpNum = inp_num.GetComponent<InputField>();
			_inpNum.text = "1";
			_modelParentTrans = m_model_skin_root.transform;
			_cam = m_camera.GetComponent<Camera>();
			_inpNum.onValueChanged.AddListener(_003COnInit_003Em__0);
			ClickListener.Get(btn_add, string.Empty).onClick = _003COnInit_003Em__1;
			ClickListener.Get(btn_reduce, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener.Get(btn_max, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener.Get(btn_buy, string.Empty).onClick = _003COnInit_003Em__4;
			ClickListener.Get(btn_refresh, string.Empty).onClick = _003COnInit_003Em__5;
			_renderTexture = HeroTools.CreatShowHero(_cam, m_hero_tex);
		}

		private void SetTotalCost()
		{
			int num = Convert.ToInt32(_inpNum.text);
			ShopCfg shopCfgByIndex = Singleton<TimeShopMgr>.Ins.GetShopCfgByIndex(_selectIndex);
			if (shopCfgByIndex != null)
			{
				m_gold_total.SetActiveBetter(shopCfgByIndex.moneyType == 1);
				m_coupon_total.SetActiveBetter(shopCfgByIndex.moneyType == 2);
			}
			if (_isSelectDiscount)
			{
				txt_full_total_coupon.SetActiveBetter(true);
				txt_full_total_gold.SetActiveBetter(true);
				View.SetLabelText(txt_full_total_couponText, _selectFullPrice * num);
				View.SetLabelText(txt_full_total_goldText, _selectFullPrice * num);
			}
			else
			{
				txt_full_total_coupon.SetActiveBetter(false);
				txt_full_total_gold.SetActiveBetter(false);
			}
			View.SetLabelText(txt_real_total_couponText, _selectRealPrice * num);
			View.SetLabelText(txt_real_total_goldText, _selectRealPrice * num);
		}

		public void OnShow(object param)
		{
			int index;
			ShopCfg shopCfg;
			Singleton<TimeShopMgr>.Ins.GetFirstCanBuy(out index, out shopCfg);
			SelectItem(index, shopCfg);
			UpdateTimeShopPanel(false);
			SetRefreshInfo();
			ShopEvent.TimeShopRefreshAllAction = (Action)Delegate.Combine(ShopEvent.TimeShopRefreshAllAction, new Action(OnTimeShopRefreshAll));
			ShopEvent.TimeShopRefreshIndexAction = (Action<int>)Delegate.Combine(ShopEvent.TimeShopRefreshIndexAction, new Action<int>(OnTimeShopRefreshIndex));
		}

		private void OnTimeShopRefreshIndex(int index)
		{
			UpdateSelectCanBuyNum(SelectIndex);
			SelectItem(SelectIndex, Singleton<TimeShopMgr>.Ins.GetShopCfgByIndex(SelectIndex));
			_timeScrollPanel.UpdateCell(index, FillTimeShopCell);
		}

		private void OnTimeShopRefreshAll()
		{
			int index;
			ShopCfg shopCfg;
			Singleton<TimeShopMgr>.Ins.GetFirstCanBuy(out index, out shopCfg);
			SelectIndex = index;
			SelectItem(SelectIndex, shopCfg);
			UpdateTimeShopPanel(false);
			SetRefreshInfo();
		}

		private void SetRefreshInfo()
		{
			RandomShopRefreshPriceInfo refreshPriceInfo = GetRefreshPriceInfo();
			Singleton<NormalShopMgr>.Ins.SetMoneyIcon(m_refresh_money_icon, refreshPriceInfo.moneyType);
			View.SetLabelText(txt_refresh_costText, Utils.GetString(269, refreshPriceInfo.price));
			View.SetLabelText(txt_count_downText, Singleton<TimeShopMgr>.Ins.GetNextTimeRefresh());
		}

		private RandomShopRefreshPriceInfo GetRefreshPriceInfo()
		{
			RandomShopRefreshPriceCfg randomShopRefreshPriceCfg = RandomShopRefreshPriceCfg.Get(1);
			int num = Singleton<TimeShopMgr>.Ins.GetRefreshIndex();
			if (num < 0)
			{
				num = 0;
			}
			int num2 = randomShopRefreshPriceCfg.refreshPriceInfos.Count - 1;
			if (num > num2)
			{
				num = num2;
			}
			return randomShopRefreshPriceCfg.refreshPriceInfos[num];
		}

		public void OnHide()
		{
			_passTime = 0f;
			ShopEvent.TimeShopRefreshAllAction = (Action)Delegate.Remove(ShopEvent.TimeShopRefreshAllAction, new Action(OnTimeShopRefreshAll));
			ShopEvent.TimeShopRefreshIndexAction = (Action<int>)Delegate.Remove(ShopEvent.TimeShopRefreshIndexAction, new Action<int>(OnTimeShopRefreshIndex));
		}

		protected void OnDestroy()
		{
			_cam.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		protected void Update()
		{
			_passTime += Time.deltaTime;
			if (_passTime > 60f)
			{
				_passTime = 0f;
				View.SetLabelText(txt_count_downText, Singleton<TimeShopMgr>.Ins.GetNextTimeRefresh());
			}
		}

		private void UpdateTimeShopPanel(bool isNoPos)
		{
			_randomItemInfoList = Singleton<TimeShopMgr>.Ins.GetItemList();
			if (isNoPos)
			{
				_timeScrollPanel.ResetNoPosClear((_randomItemInfoList != null) ? _randomItemInfoList.Count : 0, FillTimeShopCell);
			}
			else
			{
				_timeScrollPanel.Reset((_randomItemInfoList != null) ? _randomItemInfoList.Count : 0, FillTimeShopCell);
			}
		}

		private void FillTimeShopCell(GameObject go, int index)
		{
			_003CFillTimeShopCell_003Ec__AnonStorey0 _003CFillTimeShopCell_003Ec__AnonStorey = new _003CFillTimeShopCell_003Ec__AnonStorey0();
			_003CFillTimeShopCell_003Ec__AnonStorey.index = index;
			_003CFillTimeShopCell_003Ec__AnonStorey._0024this = this;
			GTimeShopCell component = go.GetComponent<GTimeShopCell>();
			RandomShopItemInfo randomShopItemInfo = _randomItemInfoList[_003CFillTimeShopCell_003Ec__AnonStorey.index];
			_003CFillTimeShopCell_003Ec__AnonStorey.shopInfo = ShopCfg.Get(randomShopItemInfo.shopId);
			ShopItem something = component.m_something;
			component.m_nothing.SetActiveBetter(false);
			something.m_select.SetActiveBetter(SelectIndex == _003CFillTimeShopCell_003Ec__AnonStorey.index);
			something.m_fen_ge_xian.SetActiveBetter(_003CFillTimeShopCell_003Ec__AnonStorey.index % 3 == 0);
			Singleton<TimeShopMgr>.Ins.SetCellObjs(something, _003CFillTimeShopCell_003Ec__AnonStorey.shopInfo, _003CFillTimeShopCell_003Ec__AnonStorey.index);
			ClickListener.Get(go, string.Empty).onClick = _003CFillTimeShopCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void SelectItem(int index, ShopCfg shopInfo)
		{
			SelectIndex = index;
			_inpNum.text = "1";
			SetTotalCost();
			if (shopInfo == null)
			{
				txt_name.SetActiveBetter(false);
				txt_desc.SetActiveBetter(false);
				return;
			}
			txt_name.SetActiveBetter(true);
			txt_desc.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(shopInfo.id);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_descText, shopInfo.desc);
			ItemCfg itemCfg2 = ItemCfg.Get(shopInfo.id);
			if (itemCfg2 != null)
			{
				if (string.IsNullOrEmpty(itemCfg2.modelPath) || !shopInfo.isShowModel)
				{
					m_model_root.SetActiveBetter(false);
					m_item_image_parent.SetActiveBetter(true);
					View.SetItemSprite(m_item_image, shopInfo.icon);
					return;
				}
				if ((bool)_lastGo)
				{
					_lastGo.SetActiveBetter(false);
					_lastGo = null;
				}
				Singleton<ShopLoadModelMgr>.Ins.GetModel(itemCfg2, LoadModelCallBack);
			}
			else
			{
				m_model_root.SetActiveBetter(false);
				m_item_image_parent.SetActiveBetter(false);
			}
		}

		private void LoadModelCallBack(int itemId, GameObject go)
		{
			if (_randomItemInfoList == null || _randomItemInfoList.Count <= SelectIndex)
			{
				go.SetActiveBetter(false);
				return;
			}
			if (_randomItemInfoList[SelectIndex].shopId != itemId)
			{
				go.SetActiveBetter(false);
				return;
			}
			go.SetActiveBetter(true);
			_lastGo = go;
			m_model_root.SetActiveBetter(true);
			m_item_image_parent.SetActiveBetter(false);
			Transform transform = go.transform;
			transform.SetParent(_modelParentTrans);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.eulerAngles = Vector3.zero;
			if (GunCfg.Get(itemId) != null)
			{
				Transform transform2 = go.transform.Find("ef");
				if ((bool)transform2)
				{
					transform2.gameObject.SetActive(true);
				}
				if (_gunLength > 0f)
				{
					GunCfg gunCfg = GunCfg.Get(itemId);
					_gunLength = ((gunCfg.gunType != 1) ? 4f : 3.5f);
					SkinnedMeshRenderer componentInChildren = go.transform.GetComponentInChildren<SkinnedMeshRenderer>();
					float num = _gunLength / componentInChildren.bounds.size.x;
					go.transform.localScale = new Vector3(num, num, num);
					go.transform.localPosition -= go.transform.InverseTransformPoint(componentInChildren.bounds.center) * num;
				}
				else
				{
					go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
				}
			}
		}

		private void Awake()
		{
			btn_add = base.transform.Find("GameObject/Image (2)/btn_add").gameObject;
			btn_buy = base.transform.Find("GameObject/btn_buy").gameObject;
			btn_max = base.transform.Find("GameObject/Image (2)/btn_max").gameObject;
			btn_reduce = base.transform.Find("GameObject/Image (2)/btn_reduce").gameObject;
			btn_refresh = base.transform.Find("GameObject/GameObject/btn_refresh").gameObject;
			inp_num = base.transform.Find("GameObject/Image (2)/inp_num").gameObject;
			m_camera = base.transform.Find("m_model_root/m_model_role/m_camera").gameObject;
			m_cell = View.AddComponentIfNotExist<GTimeShopCell>(base.transform.Find("zuo/scp_time_shop/content/m_cell").gameObject);
			m_count_down = base.transform.Find("GameObject/GameObject/m_count_down").gameObject;
			m_coupon_total = base.transform.Find("GameObject/Image/m_coupon_total").gameObject;
			m_gold_total = base.transform.Find("GameObject/Image/m_gold_total").gameObject;
			m_hero_tex = base.transform.Find("m_model_root/m_hero_tex").gameObject;
			m_item_image = base.transform.Find("m_item_image_parent/m_item_image").gameObject;
			m_item_image_parent = base.transform.Find("m_item_image_parent").gameObject;
			m_model_equip_root = base.transform.Find("m_model_root/m_model_role/m_model_equip_root").gameObject;
			m_model_role = base.transform.Find("m_model_root/m_model_role").gameObject;
			m_model_root = base.transform.Find("m_model_root").gameObject;
			m_model_skin_root = base.transform.Find("m_model_root/m_model_role/m_model_skin_root").gameObject;
			m_refresh_money_icon = base.transform.Find("GameObject/GameObject/btn_refresh/m_refresh_money_icon").gameObject;
			m_role_bg = base.transform.Find("m_model_root/m_role_bg").gameObject;
			scp_time_shop = base.transform.Find("zuo/scp_time_shop").gameObject;
			txt_count_down = base.transform.Find("GameObject/GameObject/m_count_down/txt_count_down").gameObject;
			txt_count_downText = txt_count_down.GetComponent<Text>();
			txt_desc = base.transform.Find("GameObject (1)/txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_full_total_coupon = base.transform.Find("GameObject/Image/m_coupon_total/txt_full_total_coupon").gameObject;
			txt_full_total_couponText = txt_full_total_coupon.GetComponent<Text>();
			txt_full_total_gold = base.transform.Find("GameObject/Image/m_gold_total/txt_full_total_gold").gameObject;
			txt_full_total_goldText = txt_full_total_gold.GetComponent<Text>();
			txt_name = base.transform.Find("GameObject (1)/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_real_total_coupon = base.transform.Find("GameObject/Image/m_coupon_total/txt_real_total_coupon").gameObject;
			txt_real_total_couponText = txt_real_total_coupon.GetComponent<Text>();
			txt_real_total_gold = base.transform.Find("GameObject/Image/m_gold_total/txt_real_total_gold").gameObject;
			txt_real_total_goldText = txt_real_total_gold.GetComponent<Text>();
			txt_refresh_cost = base.transform.Find("GameObject/GameObject/btn_refresh/txt_refresh_cost").gameObject;
			txt_refresh_costText = txt_refresh_cost.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(string str)
		{
			int num = Convert.ToInt32(str);
			if (num > _maxCanBuy)
			{
				num = _maxCanBuy;
			}
			if (num < 1)
			{
				num = 1;
			}
			_inpNum.text = num.ToString();
			SetTotalCost();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__1(GameObject go)
		{
			int num = Convert.ToInt32(_inpNum.text);
			num++;
			if (num > _maxCanBuy)
			{
				AlertBox.Show(270);
			}
			_inpNum.text = num.ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			int num = Convert.ToInt32(_inpNum.text);
			_inpNum.text = (num - 1).ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__3(GameObject go)
		{
			_inpNum.text = _maxCanBuy.ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__4(GameObject go)
		{
			int num = Convert.ToInt32(_inpNum.text);
			if (num > _maxCanBuy)
			{
				AlertBox.Show(270);
			}
			else if (Singleton<TimeShopMgr>.Ins.CheckMoneyEnough(SelectIndex, Convert.ToInt32(txt_real_total_couponText.text)))
			{
				Singleton<TimeShopMgr>.Ins.SendBuyRandomShopMsg(_selectIndex, num);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			RandomShopRefreshPriceInfo refreshPriceInfo = GetRefreshPriceInfo();
			if (Singleton<RoleMgr>.Ins.IsMoneyEnough(refreshPriceInfo.price, refreshPriceInfo.moneyType, Singleton<NormalShopMgr>.Ins.GoldNotEnoughCallBack, Singleton<NormalShopMgr>.Ins.CouponNotEnoughCallBack))
			{
				Singleton<TimeShopMgr>.Ins.SendRefreshRandomShopMsg();
			}
		}
	}
}
