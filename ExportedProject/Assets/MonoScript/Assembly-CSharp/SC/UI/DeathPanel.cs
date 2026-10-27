using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

namespace SC.UI
{
	public class DeathPanel : View
	{
		private enum RebirthPageType
		{
			Desc = 0,
			Rebieth = 1
		}

		[CompilerGenerated]
		private sealed class _003CRebirthItemData_003Ec__AnonStorey4
		{
			internal GDeathPanelRebirthItem gDeathPanelRebirthItem;

			internal DeathPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._currentSelectBed = -1L;
				_0024this.UpdateBirthData();
				_0024this.UpdateRebirthBuild();
			}
		}

		[CompilerGenerated]
		private sealed class _003CRebirthItemData_003Ec__AnonStorey5
		{
			internal long instanceId;

			internal _003CRebirthItemData_003Ec__AnonStorey4 _003C_003Ef__ref_00244;

			internal void _003C_003Em__0()
			{
				_003C_003Ef__ref_00244._0024this.UpdateBirthData();
				_003C_003Ef__ref_00244._0024this.UpdateRebirthBuild();
				_003C_003Ef__ref_00244.gDeathPanelRebirthItem.ckb_select.SetActiveBetter(true);
				_003C_003Ef__ref_00244.gDeathPanelRebirthItem.m_time.SetActiveBetter(false);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_003C_003Ef__ref_00244._0024this._currentSelectBed = instanceId;
				_003C_003Ef__ref_00244._0024this.UpdateBirthData();
				_003C_003Ef__ref_00244._0024this.UpdateRebirthBuild();
			}
		}

		private Vector2 _dragBeginPos;

		private RawImage _rawImage;

		private float _imageWidth;

		private float _imageHeight;

		private readonly float MAP_SIZE = 6000f;

		private float _hRate;

		private float _vRate;

		private readonly int MIN_ZOOM_LEVEL = 1;

		private readonly int MAX_ZOOM_LEVEL = 8;

		private int _zoomLevel;

		private Vec3 _myRotation = new Vec3();

		private Sprite[] _sprites = new Sprite[4];

		private List<long> _roleIds = new List<long>();

		private readonly int MAKE_MARK_SOUND_ID = 353;

		private float _duCircleRadio;

		private ScrollRect _mapScroll;

		private Canvas _canvas;

		private Slider _mapSlide;

		private Image _image;

		private readonly List<Image> _lineColumnImages = new List<Image>();

		private readonly List<Image> _lineRowImages = new List<Image>();

		private int _lineCount = 6;

		private float _intervalWidth = 108f;

		private Transform _showFlyLineTransform;

		public Dictionary<int, BattlePanel.AirDropDistance> AirDropDistanceDic = new Dictionary<int, BattlePanel.AirDropDistance>();

		private readonly List<GDeathBirthPosItem> _rebirthPosGameObjects = new List<GDeathBirthPosItem>();

		private RebirthPageType _currentRebirthPageType;

		private UIScrollPanel _rebirethScrollPanel;

		private List<long> _rebirthBuilds = new List<long>();

		private long _currentSelectBed = -1L;

		private UIScrollPanel _lossItemsScrollPanel;

		private float _rebirthRemainTime;

		private Vector3 offset;

		private Rect _zoomrect = Rect.zero;

		private Vector3 _worldPos = Vector3.zero;

		private bool _canDouble;

		private bool doubleing;

		private Vector2 _birthPos = Vector2.zero;

		private Vector2 vector2 = default(Vector2);

		private Vector2 _localPosOnMapPos = default(Vector2);

		private Vector2 lastTouchPos1 = Vector2.zero;

		private Vector2 lastTouchPos2 = Vector2.zero;

		private bool IsOpenSlider;

		private float minWid = 1080f;

		private float maxWid = 8640f;

		private float _widthfix = 1080f;

		private float maxMilitaryBaseW = 636f;

		private float maxMilitaryBaseH = 378f;

		private float _scaleMilitaryBaseW;

		private float _scaleMilitaryBaseH;

		private RectTransform rect;

		private float scale = 1f;

		private Vector2 _offset = Vector2.zero;

		private float _updateTime;

		private int level = 1;

		private int levelint;

		private Color _outLineColor = new Color(0.95686275f, 37f / 51f, 0.38039216f);

		private Color _inLineColor = new Color(1f, 1f, 1f, 0.3f);

		private GameObject m_map_scroll;

		private GameObject m_Map;

		private GameObject m_map_line_root;

		private GameObject m_line;

		private GameObject m_dead_pos;

		private GameObject m_birth_pos_root;

		private GDeathBirthPosItem m_birth_pos;

		private GameObject btn_close;

		private GameObject m_map_slide_root;

		private GameObject m_map_slide;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_rebirth;

		private GameObject m_rebirth_page;

		private GameObject scp_rebirth;

		private GDeathPanelRebirthItem m_cell;

		private GameObject m_die_desc_page;

		private GameObject txt_dead_desc;

		private Text txt_dead_descText;

		private GDeathPanelKilledPlayer m_killed_player;

		private GameObject scp_loss;

		private GameObject btn_rebirth_rander;

		private GDeathPanelLossItem m_cell_1;

		private GameObject btn_die_desc_tab;

		private GameObject btn_rebirth_tab;

		private GameObject txt_auto_rebirth_time;

		private Text txt_auto_rebirth_timeText;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			m_cell.gameObject.SetActiveBetter(false);
			m_cell_1.gameObject.SetActiveBetter(false);
			NoKillPlayer();
			_rebirethScrollPanel = scp_rebirth.GetComponent<UIScrollPanel>();
			_lossItemsScrollPanel = scp_loss.GetComponent<UIScrollPanel>();
			_rebirthPosGameObjects.Add(m_birth_pos);
			_image = m_line.GetComponent<Image>();
			_image.rectTransform.sizeDelta = new Vector2(4f, maxWid);
			m_line.SetActiveBetter(false);
			_mapSlide = m_map_slide.GetComponent<Slider>();
			_mapSlide.onValueChanged.AddListener(OnMapSlide);
			View.SetTexture(m_Map, "texture/map_map");
			_canvas = GameObject.Find("UIRootCanvas").GetComponent<Canvas>();
			_mapScroll = m_map_scroll.GetComponent<ScrollRect>();
			rect = m_Map.GetComponent<RectTransform>();
			_rawImage = m_Map.GetComponent<RawImage>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			BindMapClickListener();
			_widthfix = minWid;
			_intervalWidth = minWid / (float)_lineCount;
			DoubleClickListener.Get(_rawImage.gameObject).SoundName = SoundCfg.Get(MAKE_MARK_SOUND_ID).path;
			ClickListener.Get(btn_up, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_down, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_die_desc_tab, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_rebirth_tab, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_rebirth, string.Empty).onClick = OnClickRebirth;
			ClickListener.Get(btn_rebirth_rander, string.Empty).onClick = OnClickRebirthRandom;
			ClickListener.Get(m_killed_player.btn_care, string.Empty).onClick = OnClickCare;
			ClickListener clickListener = ClickListener.Get(m_killed_player.btn_report, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__5;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			btn_close.SetActiveBetter(false);
			_rebirthRemainTime = Singleton<RebirthMgr>.Ins.RebirthRemainTime - (Time.realtimeSinceStartup - Singleton<RebirthMgr>.Ins.RebirthRemainTimeReatime);
		}

		private void OnChangePage(RebirthPageType rebirthPageType)
		{
			_currentRebirthPageType = rebirthPageType;
			m_die_desc_page.SetActiveBetter(rebirthPageType == RebirthPageType.Desc);
			m_rebirth_page.SetActiveBetter(rebirthPageType == RebirthPageType.Rebieth);
		}

		private void OnMapSlide(float arg)
		{
			_widthfix = minWid + arg * (maxWid - minWid);
			UpdateMapRaw();
			Vector2 vector = m_dead_pos.transform.localPosition;
			Vector2 localPosOnMap = GetLocalPosOnMap(Singleton<MapMgr>.Ins.SelfDiePos.x, Singleton<MapMgr>.Ins.SelfDiePos.z);
			Vector2 vector2 = localPosOnMap - vector;
			rect.anchoredPosition -= vector2;
			UpdateMap();
			_offset = Vector2.zero;
			if (rect.anchoredPosition.x > 0f)
			{
				float num = minWid / 2f + rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
				if (0f < num)
				{
					_offset.x = 0f - num;
				}
			}
			else
			{
				float num2 = minWid / 2f - rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
				if (0f < num2)
				{
					_offset.x = num2;
				}
			}
			if (rect.anchoredPosition.y > 0f)
			{
				float num3 = minWid / 2f + rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
				if (0f < num3)
				{
					_offset.y = 0f - num3;
				}
			}
			else
			{
				float num4 = minWid / 2f - rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
				if (0f < num4)
				{
					_offset.y = num4;
				}
			}
			rect.anchoredPosition += _offset;
			SetSelfPos();
		}

		private void SetSlider()
		{
			_mapSlide.onValueChanged.RemoveListener(OnMapSlide);
			float value = (_widthfix - minWid) / (maxWid - minWid);
			_mapSlide.value = value;
			_mapSlide.onValueChanged.AddListener(OnMapSlide);
		}

		private void OnZoomLevelChange(bool zoomIn, Vector2 point)
		{
			int value = ((!zoomIn) ? (_zoomLevel - 1) : (_zoomLevel + 1));
			value = Mathf.Clamp(value, MIN_ZOOM_LEVEL, MAX_ZOOM_LEVEL);
			if (value != _zoomLevel)
			{
				_zoomLevel = value;
				StartCoroutine(Zoom(point, zoomIn));
			}
		}

		private void BindMapClickListener()
		{
			DoubleClickListener.Get(_rawImage.gameObject).onDoubleClick = OnDoubleClickMap;
		}

		private void OnDoubleClickMap(GameObject go, Vector2 data)
		{
			if (!((double)Math.Abs(_widthfix - maxWid) < 0.0001))
			{
				_canDouble = true;
				if (!doubleing)
				{
					StartCoroutine(Zooms(data));
				}
			}
		}

		private void ShowMap()
		{
			Rect uvRect = default(Rect);
			uvRect.center = Vector2.zero;
			uvRect.size = new Vector2(1f / (float)_zoomLevel, 1f / (float)_zoomLevel);
			_rawImage.uvRect = uvRect;
		}

		private IEnumerator Zoom(Vector2 clickPoint, bool zoomIn = true)
		{
			Vector2 localPos = _rawImage.transform.InverseTransformPoint(ViewMgr.Ins.UICamera.ScreenToWorldPoint(clickPoint));
			float currentSize = _rawImage.uvRect.size.x;
			float finalSize = 1f / (float)_zoomLevel;
			int oldLvevel = ((!zoomIn) ? (_zoomLevel + 1) : (_zoomLevel - 1));
			_worldPos.x = localPos.x / (GetHRate() * (float)oldLvevel) + (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE;
			_worldPos.z = localPos.y / (GetVRate() * (float)oldLvevel) + (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE;
			yield return Utils.WaitForSeconds(0.01f);
			while (Mathf.Abs(finalSize - currentSize) >= 0.0001f)
			{
				currentSize = Mathf.Lerp(currentSize, finalSize, Time.deltaTime * 10f);
				float x2 = (_worldPos.x - localPos.x * currentSize / GetHRate()) / MAP_SIZE + 0.5f;
				float y2 = (_worldPos.z - localPos.y * currentSize / GetVRate()) / MAP_SIZE + 0.5f;
				x2 = Mathf.Clamp(x2 - currentSize / 2f, 0f, 1f - currentSize);
				y2 = Mathf.Clamp(y2 - currentSize / 2f, 0f, 1f - currentSize);
				_zoomrect.center = new Vector2(x2, y2);
				_zoomrect.size = new Vector2(currentSize, currentSize);
				_rawImage.uvRect = _zoomrect;
				UpdateMap();
				yield return Utils.WaitForSeconds(0.01f);
			}
		}

		private IEnumerator Zooms(Vector2 clickPoint, bool zoomIn = true)
		{
			doubleing = true;
			Vector2 localPos = _rawImage.transform.InverseTransformPoint(ViewMgr.Ins.UICamera.ScreenToWorldPoint(clickPoint));
			Vector3 worldPos = new Vector3
			{
				x = localPos.x / GetHRate() + (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE,
				z = localPos.y / GetVRate() + (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE
			};
			float endWidthfix = _widthfix + 2100f;
			if (endWidthfix > maxWid)
			{
				endWidthfix = maxWid;
			}
			while (Math.Abs(_widthfix - endWidthfix) > 10f && _canDouble)
			{
				_widthfix = Mathf.Lerp(_widthfix, endWidthfix, Time.deltaTime * 10f);
				UpdateMapRaw();
				Vector2 localPosnew = GetLocalPosOnMap(worldPos.x, worldPos.z);
				Vector2 temp = localPosnew - localPos;
				localPos = new Vector2(localPosnew.x, localPosnew.y);
				rect.anchoredPosition -= temp;
				UpdateMap();
				yield return Utils.WaitForSeconds(0.01f);
			}
			doubleing = false;
		}

		private void UpdateMap()
		{
			SetSelfPos();
			UpdateRebirthBuild();
		}

		private void UpdateRebirthBuild()
		{
			int i = 0;
			foreach (long rebirthBuild in _rebirthBuilds)
			{
				if (i >= _rebirthPosGameObjects.Count)
				{
					GDeathBirthPosItem gDeathBirthPosItem = UnityEngine.Object.Instantiate(m_birth_pos);
					gDeathBirthPosItem.transform.SetParent(m_birth_pos_root.transform, true);
					gDeathBirthPosItem.transform.localPosition = Vector3.zero;
					gDeathBirthPosItem.transform.localScale = Vector3.one;
					_rebirthPosGameObjects.Add(gDeathBirthPosItem);
				}
				_rebirthPosGameObjects[i].gameObject.SetActiveBetter(true);
				RebirthPos rebirthPos = Singleton<RebirthMgr>.Ins.RebirthPosDic[rebirthBuild];
				_birthPos = GetLocalPosOnMap(rebirthPos.pos.x, rebirthPos.pos.z);
				_rebirthPosGameObjects[i].transform.localPosition = _birthPos;
				ItemCfg itemCfg = ItemCfg.Get(rebirthPos.typeId);
				if (itemCfg.childType == 102)
				{
					_rebirthPosGameObjects[i].m_sleep_bed.SetActiveBetter(false);
					_rebirthPosGameObjects[i].m_sleep_cding.SetActiveBetter(false);
					_rebirthPosGameObjects[i].m_sleep_select.SetActiveBetter(false);
					float realTime = Singleton<RebirthMgr>.Ins.GetRealTime(rebirthBuild);
					float num = Time.realtimeSinceStartup - realTime;
					int num2 = rebirthPos.cdTime - (int)num;
					_rebirthPosGameObjects[i].m_bed_cding.SetActiveBetter(num2 > 0);
					_rebirthPosGameObjects[i].m_bed.SetActiveBetter(num2 <= 0 && rebirthBuild != _currentSelectBed);
					_rebirthPosGameObjects[i].m_bed_select.SetActiveBetter(num2 <= 0 && rebirthBuild == _currentSelectBed);
					if (num2 <= 0 && rebirthBuild == _currentSelectBed)
					{
						_rebirthPosGameObjects[i].transform.SetAsLastSibling();
					}
				}
				else if (itemCfg.childType == 103)
				{
					_rebirthPosGameObjects[i].m_bed_cding.SetActiveBetter(false);
					_rebirthPosGameObjects[i].m_bed.SetActiveBetter(false);
					_rebirthPosGameObjects[i].m_bed_select.SetActiveBetter(false);
					float realTime2 = Singleton<RebirthMgr>.Ins.GetRealTime(rebirthBuild);
					float num3 = Time.realtimeSinceStartup - realTime2;
					int num4 = rebirthPos.cdTime - (int)num3;
					_rebirthPosGameObjects[i].m_sleep_cding.SetActiveBetter(num4 > 0);
					_rebirthPosGameObjects[i].m_sleep_bed.SetActiveBetter(num4 <= 0 && rebirthBuild != _currentSelectBed);
					_rebirthPosGameObjects[i].m_sleep_select.SetActiveBetter(num4 <= 0 && rebirthBuild == _currentSelectBed);
					if (num4 <= 0 && rebirthBuild == _currentSelectBed)
					{
						_rebirthPosGameObjects[i].transform.SetAsLastSibling();
					}
				}
				i++;
			}
			for (; i < _rebirthPosGameObjects.Count; i++)
			{
				_rebirthPosGameObjects[i].gameObject.SetActiveBetter(false);
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(531);
			_rebirthBuilds = Singleton<RebirthMgr>.Ins.RebirthFacilityList;
			InitDefault();
			OnChangePage(RebirthPageType.Desc);
			RadioButton.ChooseBtn(btn_die_desc_tab);
			_zoomLevel = MIN_ZOOM_LEVEL;
			_imageWidth = _rawImage.rectTransform.rect.width;
			_imageHeight = _rawImage.rectTransform.rect.height;
			_hRate = _imageWidth / MAP_SIZE;
			_vRate = _imageHeight / MAP_SIZE;
			long roleId = Singleton<RoleMgr>.Ins.info.roleId;
			_roleIds.Clear();
			_roleIds.Add(roleId);
			foreach (TeamateInfo item in Singleton<TeamateMgr>.Ins.GetTeamateInfo())
			{
				_roleIds.Add(item.roleId);
			}
			_roleIds.Sort();
			rect.anchoredPosition = Vector2.zero;
			UpdateMapRaw();
			ShowMap();
			SetCenter();
			StartCoroutine(ZeroOneSecondTick());
			SetRebirthItemData();
			SetDeadDesc();
			UpdateMap();
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Combine(ins.UpdateBasicInfo, new Utils.LongDelegate(UpDateBasicInfo));
			SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
		}

		private void SRebirthHandle(SRebirth msg)
		{
			if (msg.playerInfo.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				Hide();
			}
		}

		protected override void onHide(string childView = null)
		{
			StopCoroutine(ZeroOneSecondTick());
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Remove(ins.UpdateBasicInfo, new Utils.LongDelegate(UpDateBasicInfo));
			SRebirth.handler = (SRebirth.Handler)Delegate.Remove(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
			ViewMgr.Ins.Destroy<DeathPanel>();
		}

		protected override void onDestroy()
		{
			Singleton<RebirthMgr>.Ins.ShowDropedItemsOffline();
		}

		private void UpDateBasicInfo(long roleId)
		{
			SetDeadDesc();
		}

		private Vector3 GetWorldPos(Vector2 vector2)
		{
			Vector3 result = default(Vector3);
			result.x = vector2.x / (GetHRate() * (float)_zoomLevel) + (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE;
			result.z = vector2.y / (GetVRate() * (float)_zoomLevel) + (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE;
			return result;
		}

		private Vector2 GetLocalPosOnMap(float worldPosX, float worldPosZ)
		{
			vector2.x = (worldPosX - (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE) * GetHRate() * (float)_zoomLevel;
			vector2.y = (worldPosZ - (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE) * GetVRate() * (float)_zoomLevel;
			return vector2;
		}

		private float GetHRate()
		{
			return _rawImage.rectTransform.rect.width / MAP_SIZE;
		}

		private float GetVRate()
		{
			return _rawImage.rectTransform.rect.height / MAP_SIZE;
		}

		private void SetPlayerPos(GameObject diePosGameObject, Vector3 selfDiePos)
		{
			Vector2 localPosOnMap = GetLocalPosOnMap(selfDiePos.x, selfDiePos.z);
			Vector3 localPosition = new Vector3(localPosOnMap.x, localPosOnMap.y, 0f);
			diePosGameObject.transform.localPosition = localPosition;
		}

		private void Update()
		{
			UpdatetouchCount();
			if (_rebirthRemainTime > 0f)
			{
				_rebirthRemainTime -= Time.deltaTime;
			}
			else
			{
				_rebirthRemainTime = 0f;
			}
			View.SetLabelText(txt_auto_rebirth_timeText, (int)_rebirthRemainTime);
		}

		private IEnumerator ZeroOneSecondTick()
		{
			while (true)
			{
				SetSelfPos();
				yield return Utils.WaitForSeconds(0.1f);
			}
		}

		private void SetSelfPos()
		{
			if (!(Battle.Ins == null))
			{
				SetPlayerPos(m_dead_pos, Singleton<MapMgr>.Ins.SelfDiePos);
			}
		}

		private void InitMap()
		{
			_widthfix = rect.sizeDelta.x;
			minWid = _widthfix;
		}

		private void SetCenter()
		{
			Vector2 localPosOnMap = GetLocalPosOnMap(Battle.Ins.SelfPlayer.Pos.x, Battle.Ins.SelfPlayer.Pos.z);
			rect.anchoredPosition -= localPosOnMap;
			if (rect.anchoredPosition.x > 0f)
			{
				float num = minWid / 2f + rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
				if (0f < num)
				{
					_offset.x = 0f - num;
				}
			}
			else
			{
				float num2 = minWid / 2f - rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
				if (0f < num2)
				{
					_offset.x = num2;
				}
			}
			if (rect.anchoredPosition.y > 0f)
			{
				float num3 = minWid / 2f + rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
				if (0f < num3)
				{
					_offset.y = 0f - num3;
				}
			}
			else
			{
				float num4 = minWid / 2f - rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
				if (0f < num4)
				{
					_offset.y = num4;
				}
			}
			rect.anchoredPosition += _offset;
		}

		private void UpdatetouchCount()
		{
			if (Input.touchCount > 1)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					Vector2 localPoint;
					if (!(_canvas != null) || !RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, touch.position, _canvas.worldCamera, out localPoint) || localPoint.x < -300f)
					{
						return;
					}
				}
				if (IsOpenSlider)
				{
					IsOpenSlider = false;
					_mapScroll.enabled = false;
				}
				if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
				{
					Vector2 position = Input.GetTouch(0).position;
					Vector2 position2 = Input.GetTouch(1).position;
					float num = GetDistance(position, position2) * 0.005f;
					_canDouble = false;
					Vector2 vector = _rawImage.transform.InverseTransformPoint(ViewMgr.Ins.UICamera.ScreenToWorldPoint(position));
					Vector3 vector2 = default(Vector3);
					vector2.x = vector.x / GetHRate() + (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE;
					vector2.z = vector.y / GetVRate() + (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE;
					_widthfix += num;
					_offset = Vector2.zero;
					UpdateMapRaw();
					Vector2 localPosOnMap = GetLocalPosOnMap(vector2.x, vector2.z);
					Vector2 vector3 = localPosOnMap - vector;
					vector = new Vector2(localPosOnMap.x, localPosOnMap.y);
					rect.anchoredPosition -= vector3;
					UpdateMap();
					if (rect.anchoredPosition.x > 0f)
					{
						float num2 = minWid / 2f + rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
						if (0f < num2)
						{
							_offset.x = 0f - num2;
						}
					}
					else
					{
						float num3 = minWid / 2f - rect.anchoredPosition.x - rect.sizeDelta.x / 2f;
						if (0f < num3)
						{
							_offset.x = num3;
						}
					}
					if (rect.anchoredPosition.y > 0f)
					{
						float num4 = minWid / 2f + rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
						if (0f < num4)
						{
							_offset.y = 0f - num4;
						}
					}
					else
					{
						float num5 = minWid / 2f - rect.anchoredPosition.y - rect.sizeDelta.y / 2f;
						if (0f < num5)
						{
							_offset.y = num5;
						}
					}
					rect.anchoredPosition += _offset;
					lastTouchPos1 = Input.GetTouch(0).position;
					lastTouchPos2 = Input.GetTouch(1).position;
				}
				else
				{
					lastTouchPos1 = Vector2.zero;
					lastTouchPos2 = Vector2.zero;
				}
			}
			else if (Input.touchCount == 0 && !IsOpenSlider)
			{
				IsOpenSlider = true;
				_mapScroll.enabled = true;
				lastTouchPos1 = Vector2.zero;
				lastTouchPos2 = Vector2.zero;
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				_widthfix += 200f;
				_canDouble = false;
				UpdateMap();
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				_widthfix -= 200f;
				_canDouble = false;
				UpdateMap();
			}
			if (_updateTime <= 0f)
			{
				UpdateMap();
				_updateTime = 0.1f;
			}
			_updateTime -= Time.deltaTime;
		}

		private void UpdateMapRaw()
		{
			if (_widthfix > maxWid)
			{
				_widthfix = maxWid;
			}
			else if (_widthfix < minWid)
			{
				_widthfix = minWid;
			}
			SetSlider();
			rect.sizeDelta = new Vector2(_widthfix, scale * _widthfix);
			show();
		}

		private float GetDistance(Vector2 pos1, Vector2 pos2)
		{
			if (lastTouchPos1 == Vector2.zero && lastTouchPos2 == Vector2.zero)
			{
				return 0f;
			}
			return (pos2 - pos1).sqrMagnitude - (lastTouchPos2 - lastTouchPos1).sqrMagnitude;
		}

		private IEnumerator MapScale(Vector2 localPos, float scaleWidth, bool zoomIn = true)
		{
			Vector3 worldPos = new Vector3
			{
				x = localPos.x / GetHRate() + (_rawImage.uvRect.center.x - 0.5f) * MAP_SIZE,
				z = localPos.y / GetVRate() + (_rawImage.uvRect.center.y - 0.5f) * MAP_SIZE
			};
			float endWidthfix = _widthfix + scaleWidth;
			if (endWidthfix > maxWid)
			{
				endWidthfix = maxWid;
			}
			while (Math.Abs(_widthfix - endWidthfix) > 10f && _canDouble)
			{
				_widthfix = Mathf.Lerp(_widthfix, endWidthfix, Time.deltaTime * 10f);
				UpdateMapRaw();
				Vector2 localPosnew = GetLocalPosOnMap(worldPos.x, worldPos.z);
				Vector2 temp = localPosnew - localPos;
				localPos = new Vector2(localPosnew.x, localPosnew.y);
				rect.anchoredPosition -= temp;
				UpdateMap();
				yield return Utils.WaitForSeconds(0.01f);
			}
		}

		public void SetCount()
		{
			if (_rawImage.rectTransform.rect.width > 7200f)
			{
				_lineCount = 60;
			}
			else if (_rawImage.rectTransform.rect.width > 2160f)
			{
				_lineCount = 30;
			}
			else
			{
				_lineCount = 6;
			}
		}

		public void show()
		{
			ShowColumn();
			ShowRow();
		}

		public void ShowColumn()
		{
			SetCount();
			float width = _rawImage.rectTransform.rect.width;
			float num = width / (float)_lineCount;
			int i = 0;
			for (int j = 0; j < _lineCount; j++)
			{
				Vector2 vector = new Vector2(num * (float)j - width / 2f, 0f);
				if (_lineColumnImages.Count <= j)
				{
					Image image = UnityEngine.Object.Instantiate(_image);
					image.transform.SetParent(m_map_line_root.transform);
					image.rectTransform.localScale = Vector3.one;
					image.transform.localEulerAngles = Vector3.zero;
					_lineColumnImages.Add(image);
				}
				_lineColumnImages[i].gameObject.SetActiveBetter(true);
				_lineColumnImages[i].transform.localPosition = vector;
				if (j % (_lineCount / 6) == 0)
				{
					_lineColumnImages[i].color = _outLineColor;
				}
				else
				{
					_lineColumnImages[i].color = _inLineColor;
				}
				i++;
			}
			for (; i < _lineColumnImages.Count; i++)
			{
				_lineColumnImages[i].gameObject.SetActiveBetter(false);
			}
		}

		public void ShowRow()
		{
			SetCount();
			float width = _rawImage.rectTransform.rect.width;
			float num = width / (float)_lineCount;
			int i = 0;
			for (int j = 0; j < _lineCount; j++)
			{
				Vector2 vector = new Vector2(0f, num * (float)j - width / 2f);
				if (_lineRowImages.Count <= j)
				{
					Image image = UnityEngine.Object.Instantiate(_image);
					image.transform.SetParent(m_map_line_root.transform);
					image.rectTransform.localScale = Vector3.one;
					image.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
					_lineRowImages.Add(image);
				}
				_lineRowImages[i].gameObject.SetActiveBetter(true);
				_lineRowImages[i].transform.localPosition = vector;
				if (j % (_lineCount / 6) == 0)
				{
					_lineRowImages[i].color = _outLineColor;
				}
				else
				{
					_lineRowImages[i].color = _inLineColor;
				}
				i++;
			}
			for (; i < _lineRowImages.Count; i++)
			{
				_lineRowImages[i].gameObject.SetActiveBetter(false);
			}
		}

		public void SetMapMax()
		{
			_mapSlide.value = 1f;
			rect.anchoredPosition = new Vector2(-3500f, 3500f);
		}

		public void SetMapMilitaryBase()
		{
			_mapSlide.value = 1f;
			rect.anchoredPosition = new Vector2(2672f, 441f);
		}

		private void SetRebirthItemData()
		{
			_rebirethScrollPanel.Clear();
			_rebirethScrollPanel.ResetNoPos(_rebirthBuilds.Count + 1, RebirthItemData);
		}

		private void RebirthItemData(GameObject go, int index)
		{
			_003CRebirthItemData_003Ec__AnonStorey4 _003CRebirthItemData_003Ec__AnonStorey = new _003CRebirthItemData_003Ec__AnonStorey4();
			_003CRebirthItemData_003Ec__AnonStorey._0024this = this;
			_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem = go.GetComponent<GDeathPanelRebirthItem>();
			if (index < _rebirthBuilds.Count)
			{
				_003CRebirthItemData_003Ec__AnonStorey5 _003CRebirthItemData_003Ec__AnonStorey2 = new _003CRebirthItemData_003Ec__AnonStorey5();
				_003CRebirthItemData_003Ec__AnonStorey2._003C_003Ef__ref_00244 = _003CRebirthItemData_003Ec__AnonStorey;
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_random_root.SetActiveBetter(false);
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_rebirth_pos.SetActiveBetter(true);
				_003CRebirthItemData_003Ec__AnonStorey2.instanceId = _rebirthBuilds[index];
				RebirthPos rebirthPos = Singleton<RebirthMgr>.Ins.RebirthPosDic[_003CRebirthItemData_003Ec__AnonStorey2.instanceId];
				ItemCfg itemCfg = ItemCfg.Get(rebirthPos.typeId);
				View.SetLabelText(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.txt_nameText, rebirthPos.name);
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_bed.SetActiveBetter(itemCfg.childType == 102);
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_sleeping_bag.SetActiveBetter(itemCfg.childType == 103);
				float realTime = Singleton<RebirthMgr>.Ins.GetRealTime(_003CRebirthItemData_003Ec__AnonStorey2.instanceId);
				float num = Time.realtimeSinceStartup - realTime;
				int num2 = rebirthPos.cdTime - (int)num;
				if (num2 > 0)
				{
					_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select.SetActiveBetter(false);
					_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_time.SetActiveBetter(true);
					CountDown.RegistTime(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.txt_time, num2, _003CRebirthItemData_003Ec__AnonStorey2._003C_003Em__0);
				}
				else
				{
					_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select.SetActiveBetter(true);
					_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_time.SetActiveBetter(false);
					CountDown.CancleTime(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.txt_time);
				}
				View.SetCheckbox(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select, _currentSelectBed == _003CRebirthItemData_003Ec__AnonStorey2.instanceId);
				ClickListener.Get(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select, string.Empty).onClick = _003CRebirthItemData_003Ec__AnonStorey2._003C_003Em__1;
			}
			else
			{
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_random_root.SetActiveBetter(true);
				_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.m_rebirth_pos.SetActiveBetter(false);
				View.SetCheckbox(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select_random, _currentSelectBed == -1);
				ClickListener.Get(_003CRebirthItemData_003Ec__AnonStorey.gDeathPanelRebirthItem.ckb_select_random, string.Empty).onClick = _003CRebirthItemData_003Ec__AnonStorey._003C_003Em__0;
			}
		}

		private void InitDefault()
		{
			for (int i = 0; i < _rebirthBuilds.Count; i++)
			{
				long num = _rebirthBuilds[i];
				RebirthPos rebirthPos = Singleton<RebirthMgr>.Ins.RebirthPosDic[num];
				float realTime = Singleton<RebirthMgr>.Ins.GetRealTime(num);
				float num2 = Time.realtimeSinceStartup - realTime;
				int num3 = rebirthPos.cdTime - (int)num2;
				if (num3 <= 0)
				{
					_currentSelectBed = num;
					break;
				}
			}
		}

		private void UpdateBirthData()
		{
			_rebirethScrollPanel.UpdateAllCell(UpdateBirthItemData);
		}

		private void UpdateBirthItemData(GameObject go, int index)
		{
			GDeathPanelRebirthItem component = go.GetComponent<GDeathPanelRebirthItem>();
			if (index < _rebirthBuilds.Count)
			{
				long num = _rebirthBuilds[index];
				View.SetCheckbox(component.ckb_select, _currentSelectBed == num);
			}
			else
			{
				View.SetCheckbox(component.ckb_select_random, _currentSelectBed == -1);
			}
		}

		private void SetDeadDesc()
		{
			Debug.LogError("RebirthMgr.Ins.KillPlayer.hitType:" + Singleton<RebirthMgr>.Ins.KillPlayer.hitType);
			switch (Singleton<RebirthMgr>.Ins.KillPlayer.hitType)
			{
			case 1:
				SetDeadByKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(220, Singleton<RebirthMgr>.Ins.KillPlayer.killer.name));
				break;
			case 2:
			{
				ItemCfg itemCfg = ItemCfg.Get(Singleton<RebirthMgr>.Ins.KillPlayer.itemId);
				if (itemCfg.childType == 106 || itemCfg.childType == 105)
				{
					NoKillPlayer();
					View.SetLabelText(txt_dead_descText, Utils.GetString(325, itemCfg.name));
				}
				else
				{
					SetDeadByKillPlayer();
					View.SetLabelText(txt_dead_descText, Utils.GetString(221, Singleton<RebirthMgr>.Ins.KillPlayer.killer.name, itemCfg.name));
				}
				break;
			}
			case 4:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(222));
				break;
			case 6:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(223));
				break;
			case 7:
			{
				NoKillPlayer();
				ItemCfg itemCfg = ItemCfg.Get(Singleton<RebirthMgr>.Ins.KillPlayer.itemId);
				View.SetLabelText(txt_dead_descText, Utils.GetString(287, itemCfg.name));
				break;
			}
			case 9:
				SetDeadByKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(224, Singleton<RebirthMgr>.Ins.KillPlayer.killer.name));
				break;
			case 10:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(225));
				break;
			case 11:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(289));
				break;
			case 12:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(290));
				break;
			case 13:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(291));
				break;
			case 14:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(292));
				break;
			case 15:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(293));
				break;
			case 16:
			{
				NoKillPlayer();
				ItemCfg itemCfg = ItemCfg.Get(Singleton<RebirthMgr>.Ins.KillPlayer.itemId);
				if (itemCfg != null && (itemCfg.childType == 106 || itemCfg.childType == 105))
				{
					View.SetLabelText(txt_dead_descText, Utils.GetString(325, itemCfg.name));
				}
				else
				{
					View.SetLabelText(txt_dead_descText, Utils.GetString(294));
				}
				break;
			}
			case 17:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(295));
				break;
			case 18:
				NoKillPlayer();
				View.SetLabelText(txt_dead_descText, Utils.GetString(321, Singleton<RebirthMgr>.Ins.KillPlayer.killer.name));
				break;
			default:
				View.SetLabelText(txt_dead_descText, Utils.GetString(288));
				break;
			case 5:
			case 8:
				break;
			}
			SetLossItems();
		}

		private void SetHeadIcon(GameObject go, int headId)
		{
			RoleHeadCfg roleHeadCfg = RoleHeadCfg.Get(headId);
			View.SetItemSprite(go, roleHeadCfg.icon);
		}

		private void NoKillPlayer()
		{
			m_killed_player.gameObject.SetActiveBetter(false);
		}

		private void SetDeadByKillPlayer()
		{
			m_killed_player.gameObject.SetActiveBetter(true);
			AllBasicInfo property = Singleton<BasicInfoScMgr>.Ins.GetProperty(Singleton<RebirthMgr>.Ins.KillerRoleId, int.MaxValue);
			SetHeadIcon(m_killed_player.m_icon, property.basicRoleInfo.headId);
			View.SetLabelText(m_killed_player.txt_nameText, property.basicRoleInfo.name);
			View.SetLabelText(m_killed_player.txt_levelText, Utils.GetString(149, property.basicRoleInfo.level));
		}

		private void SetLossItems()
		{
			_lossItemsScrollPanel.Clear();
			_lossItemsScrollPanel.ResetNoPos(Singleton<RebirthMgr>.Ins.DieDropItemList.Count, LossItemData);
		}

		private void LossItemData(GameObject go, int index)
		{
			GDeathPanelLossItem component = go.GetComponent<GDeathPanelLossItem>();
			ItemInfo itemInfo = Singleton<RebirthMgr>.Ins.DieDropItemList[index];
			ItemCfg itemCfg = ItemCfg.Get(itemInfo.itemId);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, itemInfo.number);
		}

		private void OnClickRebirth(GameObject go)
		{
			if (_currentSelectBed > 0)
			{
				Singleton<RebirthMgr>.Ins.Rebirth(3, _currentSelectBed);
			}
			else
			{
				Singleton<RebirthMgr>.Ins.Rebirth(1, -1L);
			}
		}

		private void OnClickRebirthRandom(GameObject go)
		{
			Singleton<RebirthMgr>.Ins.Rebirth(1, -1L);
		}

		private void OnClickCare(GameObject go)
		{
			Singleton<FriendScMgr>.Ins.AddCare(Singleton<RebirthMgr>.Ins.KillerRoleId);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_map_scroll = component.GameObjects[0].gameObject;
			m_Map = component.GameObjects[1].gameObject;
			m_map_line_root = component.GameObjects[2].gameObject;
			m_line = component.GameObjects[3].gameObject;
			m_dead_pos = component.GameObjects[4].gameObject;
			m_birth_pos_root = component.GameObjects[5].gameObject;
			m_birth_pos = View.AddComponentIfNotExist<GDeathBirthPosItem>(component.GameObjects[6].gameObject);
			btn_close = component.GameObjects[7].gameObject;
			m_map_slide_root = component.GameObjects[8].gameObject;
			m_map_slide = component.GameObjects[9].gameObject;
			btn_up = component.GameObjects[10].gameObject;
			btn_down = component.GameObjects[11].gameObject;
			txt_on = component.GameObjects[12].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[13].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_rebirth = component.GameObjects[14].gameObject;
			m_rebirth_page = component.GameObjects[15].gameObject;
			scp_rebirth = component.GameObjects[16].gameObject;
			m_cell = View.AddComponentIfNotExist<GDeathPanelRebirthItem>(component.GameObjects[17].gameObject);
			m_die_desc_page = component.GameObjects[18].gameObject;
			txt_dead_desc = component.GameObjects[19].gameObject;
			txt_dead_descText = txt_dead_desc.GetComponent<Text>();
			m_killed_player = View.AddComponentIfNotExist<GDeathPanelKilledPlayer>(component.GameObjects[20].gameObject);
			scp_loss = component.GameObjects[21].gameObject;
			btn_rebirth_rander = component.GameObjects[22].gameObject;
			m_cell_1 = View.AddComponentIfNotExist<GDeathPanelLossItem>(component.GameObjects[23].gameObject);
			btn_die_desc_tab = component.GameObjects[24].gameObject;
			btn_rebirth_tab = component.GameObjects[25].gameObject;
			txt_auto_rebirth_time = component.GameObjects[26].gameObject;
			txt_auto_rebirth_timeText = txt_auto_rebirth_time.GetComponent<Text>();
			txt_on_0 = component.GameObjects[27].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[28].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
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
			if (_mapSlide.value < 1f)
			{
				_mapSlide.value += 0.2f;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (_mapSlide.value > 0f)
			{
				_mapSlide.value -= 0.2f;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			OnChangePage(RebirthPageType.Desc);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			OnChangePage(RebirthPageType.Rebieth);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__5(GameObject go)
		{
			AlertBox.Show(296);
		}
	}
}
