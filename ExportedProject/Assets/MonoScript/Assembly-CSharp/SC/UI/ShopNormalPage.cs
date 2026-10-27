using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class ShopNormalPage : MonoBehaviour, IShopPage
	{
		[CompilerGenerated]
		private sealed class _003CFillTimeShopCell_003Ec__AnonStorey0
		{
			internal ShopCfg shopInfo;

			internal ShopNormalPage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this.SelectItem(shopInfo);
				_0024this.UpdateAllCellForSelect();
			}
		}

		private UIScrollPanel _normalScrollPanel;

		private InputField _inpNum;

		private int _maxCanBuy;

		private bool _isSelectDiscount;

		private int _selectFullPrice;

		private int _selectRealPrice;

		private int _selectId;

		private int _selectIndex;

		private GameObject _lastGo;

		private List<ShopCfg> _shopItemList;

		private Transform _modelParentTrans;

		private RenderTexture _renderTexture;

		private Camera _cam;

		private float _gunLength = 4f;

		public GameObject btn_add;

		public GameObject btn_buy;

		public GameObject btn_gift_bag;

		public GameObject btn_gun;

		public GameObject btn_item;

		public GameObject btn_max;

		public GameObject btn_recipe;

		public GameObject btn_reduce;

		public GameObject inp_num;

		public GameObject m_camera;

		public GNormalShopCell m_cell;

		public GameObject m_coupon_total;

		public GameObject m_gift_bag_new_tag;

		public GameObject m_gold_total;

		public GameObject m_gun_new_tag;

		public GameObject m_hero_tex;

		public GameObject m_item_image;

		public GameObject m_item_image_parent;

		public GameObject m_item_new_tag;

		public GameObject m_model_equip_root;

		public GameObject m_model_role;

		public GameObject m_model_root;

		public GameObject m_model_skin_root;

		public GameObject m_recipe_new_tag;

		public GameObject m_role_bg;

		public GameObject scp_normal_shop;

		public GameObject txt_desc;

		public Text txt_descText;

		public GameObject txt_full_total_coupon;

		public Text txt_full_total_couponText;

		public GameObject txt_full_total_gold;

		public Text txt_full_total_goldText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_off_0;

		public Text txt_off_0Text;

		public GameObject txt_off_1;

		public Text txt_off_1Text;

		public GameObject txt_off_2;

		public Text txt_off_2Text;

		public GameObject txt_on;

		public Text txt_onText;

		public GameObject txt_on_0;

		public Text txt_on_0Text;

		public GameObject txt_on_1;

		public Text txt_on_1Text;

		public GameObject txt_on_2;

		public Text txt_on_2Text;

		public GameObject txt_real_total_coupon;

		public Text txt_real_total_couponText;

		public GameObject txt_real_total_gold;

		public Text txt_real_total_goldText;

		public object context;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		private int SelectId
		{
			get
			{
				return _selectId;
			}
			set
			{
				UpdateSelectCanBuyNum(value);
				_selectId = value;
			}
		}

		private void UpdateSelectCanBuyNum(int id)
		{
			ShopCfg shopCfg = ShopCfg.Get(id);
			if (shopCfg != null)
			{
				ShopBuyCondition condition;
				Singleton<NormalShopMgr>.Ins.GetDailyNumLimit(shopCfg, out condition);
				int buyNumLimit = shopCfg.buyNumLimit;
				if (condition != null)
				{
					int arg = condition.arg1;
					_maxCanBuy = ((buyNumLimit >= arg) ? arg : buyNumLimit);
				}
				else
				{
					_maxCanBuy = buyNumLimit;
				}
			}
			else if (id > 0)
			{
				Debug.LogError("Id : " + id + " can't find ShopCfg.");
			}
			_isSelectDiscount = Singleton<NormalShopMgr>.Ins.GetOneCost(id, out _selectRealPrice, out _selectFullPrice);
		}

		public void OnInit()
		{
			m_cell.gameObject.SetActiveBetter(false);
			m_gun_new_tag.SetActiveBetter(false);
			m_item_new_tag.SetActiveBetter(false);
			m_recipe_new_tag.SetActiveBetter(false);
			m_gift_bag_new_tag.SetActiveBetter(false);
			_normalScrollPanel = scp_normal_shop.GetComponent<UIScrollPanel>();
			_inpNum = inp_num.GetComponent<InputField>();
			_inpNum.text = "1";
			_modelParentTrans = m_model_skin_root.transform;
			_cam = m_camera.GetComponent<Camera>();
			_inpNum.onValueChanged.AddListener(_003COnInit_003Em__0);
			ClickListener.Get(btn_add, string.Empty).onClick = _003COnInit_003Em__1;
			ClickListener.Get(btn_reduce, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener.Get(btn_max, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener.Get(btn_buy, string.Empty).onClick = _003COnInit_003Em__4;
			ClickListener.Get(btn_item, string.Empty).onClick = _003COnInit_003Em__5;
			ClickListener.Get(btn_gun, string.Empty).onClick = _003COnInit_003Em__6;
			ClickListener.Get(btn_recipe, string.Empty).onClick = _003COnInit_003Em__7;
			ClickListener.Get(btn_gift_bag, string.Empty).onClick = _003COnInit_003Em__8;
			_renderTexture = HeroTools.CreatShowHero(_cam, m_hero_tex);
		}

		private void SetTotalCost()
		{
			int num = Convert.ToInt32(_inpNum.text);
			ShopCfg shopCfg = ShopCfg.Get(_selectId);
			if (shopCfg != null)
			{
				m_gold_total.SetActiveBetter(shopCfg.moneyType == 1);
				m_coupon_total.SetActiveBetter(shopCfg.moneyType == 2);
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
			int num = (int)param;
			ShopCfg shopCfg;
			if (BagItemInfoPanel.IsJumpToGiftBag())
			{
				shopCfg = Singleton<NormalShopMgr>.Ins.GetGoodsList(5)[0];
				RadioButton.ChooseBtn(btn_gift_bag);
			}
			else if (num == 0)
			{
				shopCfg = Singleton<NormalShopMgr>.Ins.GetGoodsList(1)[0];
				RadioButton.ChooseBtn(btn_item);
			}
			else
			{
				shopCfg = ShopCfg.Get(num);
				if (shopCfg != null)
				{
					switch (shopCfg.itemSmallType)
					{
					case 1:
						RadioButton.ChooseBtn(btn_item);
						break;
					case 4:
						RadioButton.ChooseBtn(btn_gun);
						break;
					case 3:
						RadioButton.ChooseBtn(btn_recipe);
						break;
					default:
						RadioButton.ChooseBtn(btn_recipe);
						break;
					}
				}
				else
				{
					shopCfg = Singleton<NormalShopMgr>.Ins.GetGoodsList(1)[0];
				}
			}
			SelectItem(shopCfg);
			UpdateTimeShopPanel(false);
			if (num > 0 && _selectIndex > 0)
			{
				_normalScrollPanel.MoveToIndexNotSingleLine(_selectIndex);
				_selectIndex = 0;
			}
			ShopEvent.NormalShopShangXiaJiaAction = (Action)Delegate.Combine(ShopEvent.NormalShopShangXiaJiaAction, new Action(OnNormalShopRefreshAll));
		}

		private void OnNormalShopRefreshAll()
		{
			UpdateTimeShopPanel(true);
		}

		public void OnHide()
		{
			ShopEvent.NormalShopShangXiaJiaAction = (Action)Delegate.Remove(ShopEvent.NormalShopShangXiaJiaAction, new Action(OnNormalShopRefreshAll));
		}

		protected void OnDestroy()
		{
			_cam.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		private void UpdateTimeShopPanel(bool isNoPos)
		{
			_shopItemList = ((_selectId <= 0) ? null : Singleton<NormalShopMgr>.Ins.GetGoodsList(ShopCfg.Get(_selectId).itemSmallType));
			if (isNoPos)
			{
				_normalScrollPanel.ResetNoPosClear((_shopItemList != null) ? _shopItemList.Count : 0, FillTimeShopCell);
			}
			else
			{
				_normalScrollPanel.Reset((_shopItemList != null) ? _shopItemList.Count : 0, FillTimeShopCell);
			}
		}

		private void FillTimeShopCell(GameObject go, int index)
		{
			_003CFillTimeShopCell_003Ec__AnonStorey0 _003CFillTimeShopCell_003Ec__AnonStorey = new _003CFillTimeShopCell_003Ec__AnonStorey0();
			_003CFillTimeShopCell_003Ec__AnonStorey._0024this = this;
			GNormalShopCell component = go.GetComponent<GNormalShopCell>();
			_003CFillTimeShopCell_003Ec__AnonStorey.shopInfo = _shopItemList[index];
			ShopItem something = component.m_something;
			component.m_nothing.SetActiveBetter(false);
			bool flag = SelectId == _003CFillTimeShopCell_003Ec__AnonStorey.shopInfo.id;
			something.m_select.SetActiveBetter(flag);
			something.m_fen_ge_xian.SetActiveBetter(index % 3 == 0);
			if (flag)
			{
				_selectIndex = index;
			}
			Singleton<NormalShopMgr>.Ins.SetCellObjs(something, _003CFillTimeShopCell_003Ec__AnonStorey.shopInfo);
			ClickListener.Get(go, string.Empty).onClick = _003CFillTimeShopCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateAllCellForSelect()
		{
			_normalScrollPanel.UpdateAllCell(FillTimeShopCellSelect);
		}

		private void FillTimeShopCellSelect(GameObject go, int index)
		{
			GNormalShopCell component = go.GetComponent<GNormalShopCell>();
			ShopCfg shopCfg = _shopItemList[index];
			ShopItem something = component.m_something;
			something.m_select.SetActiveBetter(SelectId == shopCfg.id);
		}

		private void SelectItem(ShopCfg shopInfo)
		{
			SelectId = -1;
			if (shopInfo == null)
			{
				txt_name.SetActiveBetter(false);
				txt_desc.SetActiveBetter(false);
				return;
			}
			SelectId = shopInfo.id;
			_inpNum.text = "1";
			SetTotalCost();
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
			if (SelectId != itemId)
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
			btn_gift_bag = base.transform.Find("btns/btn_gift_bag").gameObject;
			btn_gun = base.transform.Find("btns/btn_gun").gameObject;
			btn_item = base.transform.Find("btns/btn_item").gameObject;
			btn_max = base.transform.Find("GameObject/Image (2)/btn_max").gameObject;
			btn_recipe = base.transform.Find("btns/btn_recipe").gameObject;
			btn_reduce = base.transform.Find("GameObject/Image (2)/btn_reduce").gameObject;
			inp_num = base.transform.Find("GameObject/Image (2)/inp_num").gameObject;
			m_camera = base.transform.Find("m_model_root/m_model_role/m_camera").gameObject;
			m_cell = View.AddComponentIfNotExist<GNormalShopCell>(base.transform.Find("zuo/scp_normal_shop/content/m_cell").gameObject);
			m_coupon_total = base.transform.Find("GameObject (1)/Image/m_coupon_total").gameObject;
			m_gift_bag_new_tag = base.transform.Find("btns/btn_gift_bag/m_gift_bag_new_tag").gameObject;
			m_gold_total = base.transform.Find("GameObject (1)/Image/m_gold_total").gameObject;
			m_gun_new_tag = base.transform.Find("btns/btn_gun/m_gun_new_tag").gameObject;
			m_hero_tex = base.transform.Find("m_model_root/m_hero_tex").gameObject;
			m_item_image = base.transform.Find("m_item_image_parent/m_item_image").gameObject;
			m_item_image_parent = base.transform.Find("m_item_image_parent").gameObject;
			m_item_new_tag = base.transform.Find("btns/btn_item/m_item_new_tag").gameObject;
			m_model_equip_root = base.transform.Find("m_model_root/m_model_role/m_model_equip_root").gameObject;
			m_model_role = base.transform.Find("m_model_root/m_model_role").gameObject;
			m_model_root = base.transform.Find("m_model_root").gameObject;
			m_model_skin_root = base.transform.Find("m_model_root/m_model_role/m_model_skin_root").gameObject;
			m_recipe_new_tag = base.transform.Find("btns/btn_recipe/m_recipe_new_tag").gameObject;
			m_role_bg = base.transform.Find("m_model_root/m_role_bg").gameObject;
			scp_normal_shop = base.transform.Find("zuo/scp_normal_shop").gameObject;
			txt_desc = base.transform.Find("GameObject (1)/txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_full_total_coupon = base.transform.Find("GameObject (1)/Image/m_coupon_total/txt_full_total_coupon").gameObject;
			txt_full_total_couponText = txt_full_total_coupon.GetComponent<Text>();
			txt_full_total_gold = base.transform.Find("GameObject (1)/Image/m_gold_total/txt_full_total_gold").gameObject;
			txt_full_total_goldText = txt_full_total_gold.GetComponent<Text>();
			txt_name = base.transform.Find("GameObject (1)/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_off = base.transform.Find("btns/btn_recipe/Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_off_0 = base.transform.Find("btns/btn_item/Off/txt_off").gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_off_1 = base.transform.Find("btns/btn_gun/Off/txt_off").gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_off_2 = base.transform.Find("btns/btn_gift_bag/Off/txt_off").gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			txt_on = base.transform.Find("btns/btn_item/On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_on_0 = base.transform.Find("btns/btn_recipe/On/txt_on").gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = base.transform.Find("btns/btn_gun/On/txt_on").gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_on_2 = base.transform.Find("btns/btn_gift_bag/On/txt_on").gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_real_total_coupon = base.transform.Find("GameObject (1)/Image/m_coupon_total/txt_real_total_coupon").gameObject;
			txt_real_total_couponText = txt_real_total_coupon.GetComponent<Text>();
			txt_real_total_gold = base.transform.Find("GameObject (1)/Image/m_gold_total/txt_real_total_gold").gameObject;
			txt_real_total_goldText = txt_real_total_gold.GetComponent<Text>();
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
			if (Singleton<NormalShopMgr>.Ins.CheckMoneyEnough(SelectId, Convert.ToInt32(txt_real_total_couponText.text)))
			{
				if (num > _maxCanBuy)
				{
					AlertBox.Show(270);
				}
				else
				{
					Singleton<NormalShopMgr>.Ins.SendBuyShopMsg(_selectId, num);
				}
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			ShopCfg shopInfo = Singleton<NormalShopMgr>.Ins.GetGoodsList(1)[0];
			SelectItem(shopInfo);
			UpdateTimeShopPanel(false);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__6(GameObject go)
		{
			ShopCfg shopInfo = Singleton<NormalShopMgr>.Ins.GetGoodsList(4)[0];
			SelectItem(shopInfo);
			UpdateTimeShopPanel(false);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__7(GameObject go)
		{
			ShopCfg shopInfo = Singleton<NormalShopMgr>.Ins.GetGoodsList(3)[0];
			SelectItem(shopInfo);
			UpdateTimeShopPanel(false);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__8(GameObject go)
		{
			ShopCfg shopInfo = Singleton<NormalShopMgr>.Ins.GetGoodsList(5)[0];
			SelectItem(shopInfo);
			UpdateTimeShopPanel(false);
		}
	}
}
