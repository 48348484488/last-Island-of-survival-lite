using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.LargeScene;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.scmsg;
using gs.friends.scmsg;

namespace SC.UI
{
	public class BattleMapPanel : View
	{
		private GPlayerBattleMapMarks _mMyMapMarks;

		private Dropdown _dropdown;

		private GPlayerBattleMapMarks[] _GPlayerBattleMapMarkses;

		private Sprite[] _mSprites;

		private Sprite[] _mRoleSprites;

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

		private Vec3 _myPos = new Vec3();

		private Vec3 _myRotation = new Vec3();

		private Sprite[] _sprites = new Sprite[4];

		private List<long> _roleIds = new List<long>();

		private readonly int MAKE_MARK_SOUND_ID = 353;

		private float _duCircleRadio;

		private ScrollRect _mapScroll;

		private readonly int REMOVE_MARK_SOUND_ID = 354;

		private Vec3 _mTeamatePos = new Vec3();

		private Vec3 _mTeamateRotation = new Vec3();

		private Canvas _canvas;

		private Slider _mapSlide;

		private Image _image;

		private List<Image> _lineColumnImages = new List<Image>();

		private List<Image> _lineRowImages = new List<Image>();

		private int _lineCount = 6;

		private float _intervalWidth = 108f;

		private Transform _showFlyLineTransform;

		private Toggle _ckbToolBoxPosToggle;

		private Toggle _ckbSecondBattlePosToggle;

		private Toggle _ckbTeamPosToggle;

		private Toggle _ckbAirdropPostoToggle;

		private Toggle _ckbDieToggle;

		public Dictionary<int, BattlePanel.AirDropDistance> AirDropDistanceDic = new Dictionary<int, BattlePanel.AirDropDistance>();

		private List<GameObject> _airdropPosGameObjects = new List<GameObject>();

		private Vec3 _myMark = new Vec3();

		private Vector3 offset;

		private Rect _zoomrect = Rect.zero;

		private Vector3 _worldPos = Vector3.zero;

		private bool _canDouble;

		private bool doubleing;

		private Vector2 vector2 = default(Vector2);

		private Vector3 markV3 = Vector3.one;

		private Vector3 _battleInAirportPos = Vector3.zero;

		private bool _isUpdateTeamDieMapError = true;

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

		private float _nwidthRate;

		private float _nheightRate;

		private Vector2 _lastCar;

		private Vector2 _airDropPos = Vector2.zero;

		private List<GameObject> _toolBoxPosGameObjects = new List<GameObject>();

		private Vector3 _toolBoxPosVector3 = Vector3.zero;

		private Vector3 _secondPos = Vector3.zero;

		private List<GameObject> _secondBattleGameObjects = new List<GameObject>();

		private Vector3 _dropItemDiePos = Vector3.zero;

		private Vector2 _dropItemDiePosLocal = Vector2.zero;

		private GameObject m_map_scroll;

		private GameObject m_Map;

		private GameObject m_map_line_root;

		private GameObject m_line;

		private GameObject m_last_drive;

		private List<GPlayerBattleMapMarks> m_mapMarkslist = new List<GPlayerBattleMapMarks>();

		private GameObject[] m_mapMarks;

		private GameObject m_mapMarksObj;

		private GPlayerBattleMapMarks m_selfMapInfo;

		private GameObject btn_close;

		private GameObject btn_removeMark;

		private GameObject[] m_marks;

		private GameObject m_marksObj;

		private GameObject[] m_role_sprite;

		private GameObject m_role_spriteObj;

		private GameObject m_map_slide_root;

		private GameObject m_map_slide;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_latter_map_setting;

		private GameObject ckb_tool_box_pos;

		private GameObject ckb_second_battle_pos;

		private GameObject ckb_team_pos;

		private GameObject ckb_airdrop_pos;

		private GameObject ckb_die_pos;

		private GameObject m_second_battle_pos;

		private GameObject m_airdrop_pos_root;

		private GameObject m_airdrop_pos;

		private GameObject m_tool_box;

		private GameObject m_coordinate_root;

		private GameObject txt_coordinate;

		private Text txt_coordinateText;

		private GameObject m_tool_box_root;

		private GameObject btn_transfer;

		private GameObject m_die_dropitem_pos;

		private GameObject m_tip;

		private GameObject txt_guide;

		private Text txt_guideText;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			_airdropPosGameObjects.Add(m_airdrop_pos);
			_secondBattleGameObjects.Add(m_second_battle_pos);
			_toolBoxPosGameObjects.Add(m_tool_box);
			_ckbToolBoxPosToggle = ckb_tool_box_pos.GetComponent<Toggle>();
			_ckbSecondBattlePosToggle = ckb_second_battle_pos.GetComponent<Toggle>();
			_ckbTeamPosToggle = ckb_team_pos.GetComponent<Toggle>();
			_ckbAirdropPostoToggle = ckb_airdrop_pos.GetComponent<Toggle>();
			_ckbDieToggle = ckb_die_pos.GetComponent<Toggle>();
			_ckbToolBoxPosToggle.onValueChanged.AddListener(OnToolBoxPos);
			_ckbSecondBattlePosToggle.onValueChanged.AddListener(OnSecondBattlePos);
			_ckbTeamPosToggle.onValueChanged.AddListener(OnTeamPos);
			_ckbAirdropPostoToggle.onValueChanged.AddListener(OnAirdropPos);
			_ckbDieToggle.onValueChanged.AddListener(OnDiePos);
			_image = m_line.GetComponent<Image>();
			_image.rectTransform.sizeDelta = new Vector2(4f, maxWid);
			m_line.SetActiveBetter(false);
			_mapSlide = m_map_slide.GetComponent<Slider>();
			_mapSlide.onValueChanged.AddListener(OnMapSlide);
			View.SetTexture(m_Map, "texture/map_map");
			_canvas = GameObject.Find("UIRootCanvas").GetComponent<Canvas>();
			_mapScroll = m_map_scroll.GetComponent<ScrollRect>();
			rect = m_Map.GetComponent<RectTransform>();
			_mSprites = new Sprite[m_marks.Length];
			_mRoleSprites = new Sprite[m_role_sprite.Length];
			for (int i = 0; i < m_marks.Length; i++)
			{
				Image component = m_marks[i].GetComponent<Image>();
				_mSprites[i] = component.sprite;
				m_marks[i].SetActiveBetter(false);
				Image component2 = m_role_sprite[i].GetComponent<Image>();
				_mRoleSprites[i] = component2.sprite;
				m_role_sprite[i].SetActiveBetter(false);
			}
			_GPlayerBattleMapMarkses = new GPlayerBattleMapMarks[m_mapMarks.Length];
			for (int j = 0; j < m_mapMarks.Length; j++)
			{
				_GPlayerBattleMapMarkses[j] = m_mapMarks[j].GetComponent<GPlayerBattleMapMarks>();
			}
			_rawImage = m_Map.GetComponent<RawImage>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			btn_transfer.SetActiveBetter(Singleton<RoleMgr>.Ins.IsGm);
			ClickListener clickListener = ClickListener.Get(btn_transfer, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__1;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener.Get(btn_removeMark, string.Empty).onClick = _003ConInit_003Em__2;
			BindMapClickListener();
			_mMyMapMarks = m_selfMapInfo.GetComponent<GPlayerBattleMapMarks>();
			_widthfix = minWid;
			_intervalWidth = minWid / (float)_lineCount;
			ClickListener.Get(btn_removeMark, string.Empty).SoundName = SoundCfg.Get(REMOVE_MARK_SOUND_ID).path;
			DoubleClickListener.Get(_rawImage.gameObject).SoundName = SoundCfg.Get(MAKE_MARK_SOUND_ID).path;
			GameObject[] mapMarks = m_mapMarks;
			foreach (GameObject go in mapMarks)
			{
				go.SetActiveBetter(false);
			}
			ClickListener.Get(btn_up, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_down, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_latter_map_setting, string.Empty).onClick = _003ConInit_003Em__5;
			InitSetting();
		}

		private void OnToolBoxPos(bool value)
		{
			MapMgr.ShowToolBoxInMap = value;
		}

		private void OnSecondBattlePos(bool value)
		{
			MapMgr.SecondBattle = value;
		}

		private void OnTeamPos(bool value)
		{
			MapMgr.TeamMarkPos = value;
		}

		private void OnAirdropPos(bool value)
		{
			MapMgr.AirDropPos = value;
		}

		private void OnDiePos(bool value)
		{
			MapMgr.DiePos = value;
		}

		private void OnMapSlide(float arg)
		{
			_widthfix = minWid + arg * (maxWid - minWid);
			UpdateMapRaw();
			Vector2 vector = m_selfMapInfo.m_pos.transform.localPosition;
			Vector2 localPosOnMap = GetLocalPosOnMap(_myPos.x, _myPos.z);
			Vector2 vector2 = localPosOnMap - vector;
			rect.anchoredPosition -= vector2;
			UpdateOtherMapInfo();
			SetSelfPos();
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
			UpdateOtherMapInfo();
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
			DoubleClickListener.Get(_rawImage.gameObject).onSingleClick = OnClickMap;
			DoubleClickListener.Get(_rawImage.gameObject).onDoubleClick = OnDoubleClickMap;
		}

		private void RemoveMapClickListener()
		{
			DoubleClickListener.Get(_rawImage.gameObject).onSingleClick = null;
			DoubleClickListener.Get(_rawImage.gameObject).onDoubleClick = null;
		}

		private void OnClickMap(GameObject go, Vector2 data)
		{
			Vector3 vector = _rawImage.transform.InverseTransformPoint(ViewMgr.Ins.UICamera.ScreenToWorldPoint(data));
			_mMyMapMarks.m_mark.SetActiveBetter(true);
			_mMyMapMarks.m_mark.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			Vector3 worldPos = GetWorldPos(vector);
			Battle.Ins.OnMakeMark(worldPos);
			_myMark.x = worldPos.x;
			_myMark.y = worldPos.y;
			_myMark.z = worldPos.z;
			SelfMarkCoordinate(_myMark);
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
			SetSelfMark();
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
				UpdateOtherMapInfo();
				SetSelfPos();
				yield return Utils.WaitForSeconds(0.01f);
			}
			doubleing = false;
		}

		private void UpdateMap()
		{
			SetSelfMark();
			ShowTeamateInfo();
			SetSelfPos();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			if (SettingMgr.FirstOpenMap)
			{
				m_tip.SetActiveBetter(true);
				SettingMgr.FirstOpenMap = false;
			}
			else
			{
				m_tip.SetActiveBetter(false);
			}
			m_mapMarksObj.SetActiveBetter(true);
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
			_roleIds.Sort(Singleton<TeamScMgr>.Ins.SortPlayersList);
			int num = _roleIds.IndexOf(roleId);
			for (int i = 0; i < _roleIds.Count; i++)
			{
				if (_roleIds[i] == roleId)
				{
					_mMyMapMarks.m_mark.GetComponent<Image>().sprite = _mSprites[i];
					_mMyMapMarks.m_pos.GetComponent<Image>().sprite = _mRoleSprites[i];
					View.SetLabelText(_mMyMapMarks.txt_role_indexText, i + 1);
				}
				else
				{
					int num2 = ((i <= num) ? i : (i - 1));
					_GPlayerBattleMapMarkses[num2].m_mark.GetComponent<Image>().sprite = _mSprites[i];
					_GPlayerBattleMapMarkses[num2].m_pos.GetComponent<Image>().sprite = _mRoleSprites[i];
					View.SetLabelText(_GPlayerBattleMapMarkses[num2].txt_role_indexText, i + 1);
					_GPlayerBattleMapMarkses[num2].gameObject.SetActiveBetter(true);
				}
			}
			for (int j = _roleIds.Count; j < _GPlayerBattleMapMarkses.Length; j++)
			{
				_GPlayerBattleMapMarkses[j].gameObject.SetActiveBetter(false);
			}
			rect.anchoredPosition = Vector2.zero;
			UpdateMapRaw();
			ShowMap();
			ShowTeamateInfo();
			UpdateTeamDieMap();
			UpdateAirDrop();
			UpdateTooBoxPos();
			UpdateSecondBattlePos();
			DropItemDiePos();
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(SPlayerDieHandle));
			MapEvent.OnShowLastCarPos = (Utils.VoidDelegate)Delegate.Combine(MapEvent.OnShowLastCarPos, new Utils.VoidDelegate(LastCarPos));
			MapEvent.OnHideLastCarPos = (Utils.VoidDelegate)Delegate.Combine(MapEvent.OnHideLastCarPos, new Utils.VoidDelegate(LastCarPos));
			MapEvent.ToolBoxDelegate = (Utils.BoolDelegate)Delegate.Combine(MapEvent.ToolBoxDelegate, new Utils.BoolDelegate(ToolBoxDelegate));
			MapEvent.SecondBattlePosDelegate = (Utils.BoolDelegate)Delegate.Combine(MapEvent.SecondBattlePosDelegate, new Utils.BoolDelegate(SecondBattlePosDelegate));
			MapEvent.TeamPosDelegate = (Utils.BoolDelegate)Delegate.Combine(MapEvent.TeamPosDelegate, new Utils.BoolDelegate(TeamPosDelegate));
			MapEvent.AirDropPosDelegate = (Utils.BoolDelegate)Delegate.Combine(MapEvent.AirDropPosDelegate, new Utils.BoolDelegate(AirDropPosDelegate));
			MapEvent.DiePosDelegate = (Utils.BoolDelegate)Delegate.Combine(MapEvent.DiePosDelegate, new Utils.BoolDelegate(DiePosDelegate));
			MapEvent.OnDropedItemsDuringOfflineDelegate = (Utils.VoidDelegate)Delegate.Combine(MapEvent.OnDropedItemsDuringOfflineDelegate, new Utils.VoidDelegate(DropItemDiePos));
			SetCenter();
			StartCoroutine(ZeroOneSecondTick());
			LastCarPos();
			OnMoneyChange();
		}

		private void DiePosDelegate(bool arg)
		{
			UpdateTeamDieMap();
		}

		private void AirDropPosDelegate(bool arg)
		{
			UpdateAirDrop();
		}

		private void TeamPosDelegate(bool arg)
		{
		}

		private void SecondBattlePosDelegate(bool arg)
		{
			UpdateSecondBattlePos();
		}

		private void ToolBoxDelegate(bool arg)
		{
			UpdateTooBoxPos();
		}

		private void SPlayerDieHandle(SPlayerDie msg)
		{
			UpdateTeamDieMap();
		}

		protected override void onHide(string childView = null)
		{
			StopCoroutine(ZeroOneSecondTick());
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Remove(SPlayerDie.handler, new SPlayerDie.Handler(SPlayerDieHandle));
			MapEvent.OnShowLastCarPos = (Utils.VoidDelegate)Delegate.Remove(MapEvent.OnShowLastCarPos, new Utils.VoidDelegate(LastCarPos));
			MapEvent.OnHideLastCarPos = (Utils.VoidDelegate)Delegate.Remove(MapEvent.OnHideLastCarPos, new Utils.VoidDelegate(LastCarPos));
			MapEvent.ToolBoxDelegate = (Utils.BoolDelegate)Delegate.Remove(MapEvent.ToolBoxDelegate, new Utils.BoolDelegate(ToolBoxDelegate));
			MapEvent.SecondBattlePosDelegate = (Utils.BoolDelegate)Delegate.Remove(MapEvent.SecondBattlePosDelegate, new Utils.BoolDelegate(SecondBattlePosDelegate));
			MapEvent.TeamPosDelegate = (Utils.BoolDelegate)Delegate.Remove(MapEvent.TeamPosDelegate, new Utils.BoolDelegate(TeamPosDelegate));
			MapEvent.AirDropPosDelegate = (Utils.BoolDelegate)Delegate.Remove(MapEvent.AirDropPosDelegate, new Utils.BoolDelegate(AirDropPosDelegate));
			MapEvent.DiePosDelegate = (Utils.BoolDelegate)Delegate.Remove(MapEvent.DiePosDelegate, new Utils.BoolDelegate(DiePosDelegate));
			MapEvent.OnDropedItemsDuringOfflineDelegate = (Utils.VoidDelegate)Delegate.Remove(MapEvent.OnDropedItemsDuringOfflineDelegate, new Utils.VoidDelegate(DropItemDiePos));
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
			return _nwidthRate;
		}

		private float GetVRate()
		{
			return _nheightRate;
		}

		private void SetSelfMark()
		{
			Vec3 mark = Battle.Ins.GetMark();
			SelfMarkCoordinate(mark);
			if (mark == null)
			{
				_mMyMapMarks.m_mark.SetActiveBetter(false);
				return;
			}
			Vector2 localPosOnMap = GetLocalPosOnMap(mark.x, mark.z);
			_mMyMapMarks.m_mark.SetActiveBetter(true);
			_mMyMapMarks.m_mark.transform.localPosition = new Vector3(localPosOnMap.x, localPosOnMap.y, 0f);
		}

		private void SetTeamDie(GPlayerBattleMapMarks GPlayerBattleMapMarks, Vector3 worldPos, bool isDead)
		{
			Vector2 localPosOnMap = GetLocalPosOnMap(worldPos.x, worldPos.z);
			Vector3 localPosition = new Vector3(localPosOnMap.x, localPosOnMap.y, 0f);
			GPlayerBattleMapMarks.m_dead.transform.localPosition = localPosition;
			GPlayerBattleMapMarks.m_dead.SetActiveBetter(isDead);
		}

		private void SetPlayerPos(GPlayerBattleMapMarks GPlayerBattleMapMarks, Vec3 worldPos, Vec3 rotation, int status, long vehicleId, bool isSelf = false, bool isDead = false)
		{
			Vector2 localPosOnMap = GetLocalPosOnMap(worldPos.x, worldPos.z);
			Vector3 localPosition = new Vector3(localPosOnMap.x, localPosOnMap.y, 0f);
			Vector3 localEulerAngles = new Vector3(0f, 0f, 0f - rotation.y);
			GPlayerBattleMapMarks.m_pos.transform.localPosition = localPosition;
			GPlayerBattleMapMarks.m_drive.transform.localPosition = localPosition;
			GPlayerBattleMapMarks.txt_role_index.transform.localPosition = localPosition;
			GPlayerBattleMapMarks.m_pos_light.transform.localPosition = localPosition;
			GPlayerBattleMapMarks.m_pos.transform.localEulerAngles = localEulerAngles;
			GPlayerBattleMapMarks.m_drive.transform.localEulerAngles = localEulerAngles;
			bool flag = status == 2 || vehicleId > 0;
			bool flag2 = status == 4 || status == 3;
			GPlayerBattleMapMarks.m_pos.SetActiveBetter(!isDead);
			GPlayerBattleMapMarks.m_dead.SetActiveBetter(isDead);
			if (!isSelf)
			{
				bool flag3 = vehicleId > 0 && vehicleId == Battle.Ins.SelfPlayer.VehicleId;
				GPlayerBattleMapMarks.txt_role_index.SetActiveBetter(!flag3 && !flag && !flag2 && !isDead);
				GPlayerBattleMapMarks.m_drive.SetActiveBetter(!flag3 && flag && !flag2 && !isDead);
				GPlayerBattleMapMarks.m_pos_light.SetActiveBetter(false);
			}
			else
			{
				GPlayerBattleMapMarks.m_drive.SetActiveBetter(false);
				GPlayerBattleMapMarks.txt_role_index.SetActiveBetter(!isDead);
				GPlayerBattleMapMarks.m_pos_light.SetActiveBetter(true);
				float y = Battle.Ins.MainCamera.transform.eulerAngles.y;
				localEulerAngles.Set(0f, 0f, 0f - y);
				GPlayerBattleMapMarks.m_pos_light.transform.localEulerAngles = localEulerAngles;
			}
		}

		private void SetPlayerPos(GPlayerBattleMapMarks GPlayerBattleMapMarks, TeamateInfo teamateInfo)
		{
			OtherPlayerController playerById = Battle.Ins.GetPlayerById(teamateInfo.roleId);
			BasePlayerController basePlayerController = playerById;
			if (basePlayerController == null || teamateInfo.status < 0)
			{
				GPlayerBattleMapMarks.m_drive.SetActiveBetter(false);
				GPlayerBattleMapMarks.m_mark.SetActiveBetter(false);
				GPlayerBattleMapMarks.m_pos.SetActiveBetter(false);
				GPlayerBattleMapMarks.txt_role_index.SetActiveBetter(false);
				GPlayerBattleMapMarks.m_pos_light.SetActiveBetter(false);
				return;
			}
			if (teamateInfo.status == 2 && Battle.Ins.StartPlane != null)
			{
				_mTeamatePos.x = Battle.Ins.StartPlane.transform.position.x;
				_mTeamatePos.y = Battle.Ins.StartPlane.transform.position.y;
				_mTeamatePos.z = Battle.Ins.StartPlane.transform.position.z;
			}
			else
			{
				_mTeamatePos.x = basePlayerController.PlayerTransform.position.x;
				_mTeamatePos.y = basePlayerController.PlayerTransform.position.y;
				_mTeamatePos.z = basePlayerController.PlayerTransform.position.z;
			}
			_mTeamateRotation.x = 0f;
			_mTeamateRotation.y = basePlayerController.PlayerTransform.transform.eulerAngles.y;
			_mTeamateRotation.z = 0f;
			SetPlayerPos(GPlayerBattleMapMarks, _mTeamatePos, _mTeamateRotation, teamateInfo.status, teamateInfo.vehicleId, false, teamateInfo.isDead);
			if (teamateInfo.markList.Count == 0)
			{
				GPlayerBattleMapMarks.m_mark.SetActiveBetter(false);
				return;
			}
			Vec3 vec = teamateInfo.markList[0];
			Vector2 localPosOnMap = GetLocalPosOnMap(vec.x, vec.z);
			if (teamateInfo.isDead)
			{
				GPlayerBattleMapMarks.m_mark.SetActiveBetter(false);
				return;
			}
			GPlayerBattleMapMarks.m_mark.SetActiveBetter(MapMgr.TeamMarkPos);
			markV3.Set(localPosOnMap.x, localPosOnMap.y, 0f);
			GPlayerBattleMapMarks.m_mark.transform.localPosition = markV3;
		}

		private void Update()
		{
			UpdatetouchCount();
		}

		private IEnumerator ZeroOneSecondTick()
		{
			while (true)
			{
				ShowTeamateInfo();
				SetSelfPos();
				yield return Utils.WaitForSeconds(0.1f);
			}
		}

		private void SetSelfPos()
		{
			if (Battle.Ins == null)
			{
				return;
			}
			if (Battle.Ins.SelfPlayer != null && !Battle.Ins.SelfPlayer.IsDie)
			{
				_myPos.x = Battle.Ins.SelfPlayer.Pos.x;
				_myPos.z = Battle.Ins.SelfPlayer.Pos.z;
				_myRotation.y = Battle.Ins.SelfPlayer.transform.eulerAngles.y;
				int status = 5;
				if (Battle.Ins.SelfPlayer.Skydiving)
				{
					status = 4;
				}
				if (Battle.Ins.SelfPlayer.InStartPlane)
				{
					status = 2;
				}
				SetPlayerPos(m_selfMapInfo, _myPos, _myRotation, status, Battle.Ins.SelfPlayer.VehicleId, true);
			}
			else
			{
				m_selfMapInfo.m_drive.SetActiveBetter(false);
				m_selfMapInfo.m_mark.SetActiveBetter(false);
				m_selfMapInfo.m_pos.SetActiveBetter(false);
				m_selfMapInfo.txt_role_index.SetActiveBetter(false);
				m_selfMapInfo.m_pos_light.SetActiveBetter(false);
			}
		}

		private void ShowTeamateInfo()
		{
			List<TeamateInfo> teamateInfo = Singleton<TeamateMgr>.Ins.GetTeamateInfo();
			if (teamateInfo.Count == 0)
			{
				GameObject[] mapMarks = m_mapMarks;
				foreach (GameObject go in mapMarks)
				{
					go.SetActiveBetter(false);
				}
				return;
			}
			for (int j = 0; j < teamateInfo.Count; j++)
			{
				TeamateInfo teamateInfo2 = teamateInfo[j];
				if (teamateInfo2.isDead)
				{
				}
				if (teamateInfo2.status < 0)
				{
					m_mapMarks[j].gameObject.SetActiveBetter(false);
					continue;
				}
				m_mapMarks[j].gameObject.SetActiveBetter(true);
				GPlayerBattleMapMarks gPlayerBattleMapMarks = _GPlayerBattleMapMarkses[j];
				SetPlayerPos(gPlayerBattleMapMarks, teamateInfo2);
			}
		}

		private void UpdateTeamDieMap()
		{
			List<TeamateInfo> teamateInfo = Singleton<TeamateMgr>.Ins.GetTeamateInfo();
			for (int i = 0; i < teamateInfo.Count; i++)
			{
				TeamateInfo teamateInfo2 = teamateInfo[i];
				if (i > 4)
				{
					if (_isUpdateTeamDieMapError)
					{
						_isUpdateTeamDieMapError = false;
					}
					continue;
				}
				GPlayerBattleMapMarks gPlayerBattleMapMarks = _GPlayerBattleMapMarkses[i];
				if ((teamateInfo2.status == 11 || teamateInfo2.isDead) && MapMgr.DiePos)
				{
					SetTeamDie(gPlayerBattleMapMarks, Singleton<TeamateMgr>.Ins.PlayDiePos(teamateInfo2.roleId), teamateInfo2.isDead);
				}
				else
				{
					gPlayerBattleMapMarks.m_dead.SetActiveBetter(false);
				}
			}
			if (MapMgr.DiePos && Battle.Ins.SelfPlayer.IsDie)
			{
				SetTeamDie(m_selfMapInfo, Singleton<MapMgr>.Ins.SelfDiePos, Battle.Ins.SelfPlayer.IsDie);
			}
			else
			{
				m_selfMapInfo.m_dead.SetActiveBetter(false);
			}
		}

		private void UpdateTeamMap()
		{
			List<TeamateInfo> teamateInfo = Singleton<TeamateMgr>.Ins.GetTeamateInfo();
			if (teamateInfo.Count == 0)
			{
				GameObject[] mapMarks = m_mapMarks;
				foreach (GameObject go in mapMarks)
				{
					go.SetActiveBetter(false);
				}
				return;
			}
			for (int j = 0; j < teamateInfo.Count && j < 4; j++)
			{
				TeamateInfo teamateInfo2 = teamateInfo[j];
				if (teamateInfo2.status < 0)
				{
					m_mapMarks[j].gameObject.SetActiveBetter(false);
					continue;
				}
				m_mapMarks[j].gameObject.SetActiveBetter(true);
				GPlayerBattleMapMarks gPlayerBattleMapMarks = _GPlayerBattleMapMarkses[j];
				SetPlayerPos(gPlayerBattleMapMarks, teamateInfo2);
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
					UpdateOtherMapInfo();
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
				UpdateAllMapInfo();
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				_widthfix -= 200f;
				_canDouble = false;
				UpdateAllMapInfo();
			}
			if (_updateTime <= 0f)
			{
				SetSelfPos();
				UpdateTeamMap();
				_updateTime = 0.1f;
			}
			_updateTime -= Time.deltaTime;
		}

		private void UpdateAllMapInfo()
		{
			UpdateMapRaw();
			SetSelfMark();
			UpdateTeamMap();
			SetSelfPos();
			UpdateAirDrop();
			UpdateTooBoxPos();
			UpdateSecondBattlePos();
			DropItemDiePos();
		}

		private void UpdateOtherMapInfo()
		{
			SetSelfMark();
			UpdateTeamMap();
			SetSelfPos();
			UpdateTeamDieMap();
			UpdateAirDrop();
			UpdateTooBoxPos();
			UpdateSecondBattlePos();
			DropItemDiePos();
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
			LastCarPos();
		}

		private float GetDistance(Vector2 pos1, Vector2 pos2)
		{
			if (lastTouchPos1 == Vector2.zero && lastTouchPos2 == Vector2.zero)
			{
				return 0f;
			}
			return (pos2 - pos1).sqrMagnitude - (lastTouchPos2 - lastTouchPos1).sqrMagnitude;
		}

		private void OnClickMapMax(GameObject go)
		{
			StartCoroutine(MapScale(m_selfMapInfo.m_pos.transform.localPosition, maxWid));
		}

		private void OnClickMapResert(GameObject go)
		{
			StartCoroutine(MapScale(m_selfMapInfo.m_pos.transform.localPosition, 0f - maxWid));
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
				UpdateOtherMapInfo();
				SetSelfPos();
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
			float num = width / 4096f * 102f;
			width = width - num - num;
			_nwidthRate = width / MAP_SIZE;
			float num2 = width / (float)_lineCount;
			int i = 0;
			for (int j = 1; j < _lineCount; j++)
			{
				Vector2 vector = new Vector2(num2 * (float)j - width / 2f, 0f);
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
			float height = _rawImage.rectTransform.rect.height;
			float num = height / 4096f * 102f;
			height = height - num - num;
			_nheightRate = height / MAP_SIZE;
			float num2 = height / (float)_lineCount;
			int i = 0;
			for (int j = 1; j < _lineCount; j++)
			{
				Vector2 vector = new Vector2(0f, num2 * (float)j - height / 2f);
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

		private void LastCarPos()
		{
			if (Singleton<MapMgr>.Ins.IsShowLastCarPos)
			{
				m_last_drive.SetActive(true);
				_lastCar = GetLocalPosOnMap(Singleton<MapMgr>.Ins.LastCarPos.x, Singleton<MapMgr>.Ins.LastCarPos.z);
				m_last_drive.transform.localPosition = _lastCar;
			}
			else
			{
				m_last_drive.SetActive(false);
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

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void UpdateAirDrop()
		{
			if (!MapMgr.AirDropPos)
			{
				m_airdrop_pos_root.SetActiveBetter(false);
				return;
			}
			m_airdrop_pos_root.SetActiveBetter(true);
			int i = 0;
			foreach (KeyValuePair<int, BattlePanel.AirDropDistance> item in AirDropDistanceDic)
			{
				if (i >= _airdropPosGameObjects.Count)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(m_airdrop_pos);
					gameObject.transform.SetParent(m_airdrop_pos_root.transform, true);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					_airdropPosGameObjects.Add(gameObject);
				}
				_airdropPosGameObjects[i].SetActiveBetter(true);
				_airDropPos = GetLocalPosOnMap(item.Value.AirDropPosGameObject.transform.position.x, item.Value.AirDropPosGameObject.transform.position.z);
				_airdropPosGameObjects[i].transform.localPosition = _airDropPos;
				i++;
			}
			for (; i < _airdropPosGameObjects.Count; i++)
			{
				_airdropPosGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateTooBoxPos()
		{
			int i = 0;
			if (MapMgr.ShowToolBoxInMap)
			{
				foreach (KeyValuePair<long, OneChestPermit> item in Singleton<FriendPermitMgr>.Ins.DicInstanceId2ChestInfo)
				{
					if (i >= _toolBoxPosGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_tool_box);
						gameObject.transform.SetParent(m_tool_box_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_toolBoxPosGameObjects.Add(gameObject);
					}
					_toolBoxPosGameObjects[i].SetActiveBetter(true);
					_toolBoxPosVector3 = GetLocalPosOnMap(item.Value.posX, item.Value.posZ);
					_toolBoxPosGameObjects[i].transform.localPosition = _toolBoxPosVector3;
					i++;
				}
			}
			for (; i < _toolBoxPosGameObjects.Count; i++)
			{
				_toolBoxPosGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateSecondBattlePos()
		{
			int i = 0;
			if (MapMgr.SecondBattle)
			{
				foreach (Vector3 secondPo in Singleton<MapMgr>.Ins.SecondPos)
				{
					if (i >= _secondBattleGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_second_battle_pos);
						gameObject.transform.SetParent(m_second_battle_pos.transform.parent, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_secondBattleGameObjects.Add(gameObject);
					}
					_secondBattleGameObjects[i].SetActiveBetter(true);
					_secondPos = GetLocalPosOnMap(secondPo.x, secondPo.z);
					_secondBattleGameObjects[i].transform.localPosition = _secondPos;
					i++;
				}
			}
			for (; i < _secondBattleGameObjects.Count; i++)
			{
				_secondBattleGameObjects[i].SetActiveBetter(false);
			}
		}

		private void InitSetting()
		{
			_ckbToolBoxPosToggle.isOn = MapMgr.ShowToolBoxInMap;
			_ckbSecondBattlePosToggle.isOn = MapMgr.SecondBattle;
			_ckbTeamPosToggle.isOn = MapMgr.TeamMarkPos;
			_ckbAirdropPostoToggle.isOn = MapMgr.AirDropPos;
			_ckbDieToggle.isOn = MapMgr.DiePos;
		}

		private void SelfMarkCoordinate(Vec3 mark)
		{
			if (mark == null)
			{
				m_coordinate_root.SetActiveBetter(false);
				return;
			}
			m_coordinate_root.SetActiveBetter(true);
			int num = 3000;
			View.SetLabelText(txt_coordinateText, Utils.GetString(263, (int)mark.x + num, (int)mark.z + num));
		}

		private void DropItemDiePos()
		{
			if (Singleton<MapMgr>.Ins.TryGetDropedItemsVector3(out _dropItemDiePos))
			{
				_dropItemDiePosLocal = GetLocalPosOnMap(_dropItemDiePos.x, _dropItemDiePos.z);
				m_die_dropitem_pos.SetActiveBetter(true);
				m_die_dropitem_pos.transform.localPosition = _dropItemDiePosLocal;
			}
			else
			{
				m_die_dropitem_pos.SetActiveBetter(false);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_map_scroll = component.GameObjects[0].gameObject;
			m_Map = component.GameObjects[1].gameObject;
			m_map_line_root = component.GameObjects[2].gameObject;
			m_line = component.GameObjects[3].gameObject;
			m_last_drive = component.GameObjects[4].gameObject;
			m_mapMarks = component.GameObjects[5].gameObject.GetComponent<UIGameObjectList>().objects;
			m_mapMarksObj = component.GameObjects[5].gameObject;
			if (m_mapMarkslist.Count <= 0)
			{
				for (int i = 0; i < m_mapMarks.Length; i++)
				{
					m_mapMarkslist.Add(View.AddComponentIfNotExist<GPlayerBattleMapMarks>(m_mapMarks[i].gameObject));
				}
			}
			m_selfMapInfo = View.AddComponentIfNotExist<GPlayerBattleMapMarks>(component.GameObjects[6].gameObject);
			btn_close = component.GameObjects[7].gameObject;
			btn_removeMark = component.GameObjects[8].gameObject;
			m_marks = component.GameObjects[9].gameObject.GetComponent<UIGameObjectList>().objects;
			m_marksObj = component.GameObjects[9].gameObject;
			m_role_sprite = component.GameObjects[10].gameObject.GetComponent<UIGameObjectList>().objects;
			m_role_spriteObj = component.GameObjects[10].gameObject;
			m_map_slide_root = component.GameObjects[11].gameObject;
			m_map_slide = component.GameObjects[12].gameObject;
			btn_up = component.GameObjects[13].gameObject;
			btn_down = component.GameObjects[14].gameObject;
			txt_blood = component.GameObjects[15].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[16].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[17].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			btn_latter_map_setting = component.GameObjects[18].gameObject;
			ckb_tool_box_pos = component.GameObjects[19].gameObject;
			ckb_second_battle_pos = component.GameObjects[20].gameObject;
			ckb_team_pos = component.GameObjects[21].gameObject;
			ckb_airdrop_pos = component.GameObjects[22].gameObject;
			ckb_die_pos = component.GameObjects[23].gameObject;
			m_second_battle_pos = component.GameObjects[24].gameObject;
			m_airdrop_pos_root = component.GameObjects[25].gameObject;
			m_airdrop_pos = component.GameObjects[26].gameObject;
			m_tool_box = component.GameObjects[27].gameObject;
			m_coordinate_root = component.GameObjects[28].gameObject;
			txt_coordinate = component.GameObjects[29].gameObject;
			txt_coordinateText = txt_coordinate.GetComponent<Text>();
			m_tool_box_root = component.GameObjects[30].gameObject;
			btn_transfer = component.GameObjects[31].gameObject;
			m_die_dropitem_pos = component.GameObjects[32].gameObject;
			m_tip = component.GameObjects[33].gameObject;
			txt_guide = component.GameObjects[34].gameObject;
			txt_guideText = txt_guide.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__1(GameObject go)
		{
			Vec3 mark = Battle.Ins.GetMark();
			if (mark != null)
			{
				LargeSceneManager.Ins.ToSceneCell(mark.x, 100f, mark.z);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Battle.Ins.OnRemoveMark();
			SetSelfMark();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (_mapSlide.value < 1f)
			{
				_mapSlide.value += 0.2f;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			if (_mapSlide.value > 0f)
			{
				_mapSlide.value -= 0.2f;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			ViewMgr.Ins.ShowView<BattleMapSettingPanel>(null, false);
			m_tip.SetActiveBetter(false);
		}
	}
}
