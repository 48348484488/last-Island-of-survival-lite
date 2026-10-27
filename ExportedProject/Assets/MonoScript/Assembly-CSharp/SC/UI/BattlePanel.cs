using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Socket;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using cfg;
using gs.bag.scmsg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;
using gs.chat.scmsg;
using gs.friends.scmsg;

namespace SC.UI
{
	public class BattlePanel : View
	{
		public class AirDropDistance
		{
			public float remainTime;

			public GameObject AirDropPosGameObject;

			public bool isDir = true;
		}

		private class SoundSource
		{
			public static readonly int FOOT_SOUND;

			public static readonly int GUN_SOUND = 1;

			public static readonly int VEHICLE_SOUND = 2;

			public static readonly int AIRDROP_SOUND = 3;

			public static readonly int GUN_SOUND_SILENCE = 4;

			public GameObject _mGameObject;

			public Transform _mTransform;

			public Vector3 _mTransformPos;

			public int _mSoundType;

			public float _mMaxDistance;

			public float _mTimeToVanish;

			public bool IsDir;
		}

		[CompilerGenerated]
		private sealed class _003CShowBullets_003Ec__AnonStorey7
		{
			internal int bulletId;

			internal BattlePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.m_bulletsSelectObj.SetActive(false);
				_0024this.m_longPress = false;
				Battle.Ins.HuanDan(bulletId);
			}
		}

		[CompilerGenerated]
		private sealed class _003COnAddExtraBtn_003Ec__AnonStorey8
		{
			internal int id;

			internal bool isShowNewbeeEffect;

			internal GExtraBtns gExtraBtns;

			internal void _003C_003Em__0(GameObject go)
			{
				Utils.TriggerEvent(EventHandlers.OnClickExtraBtn, id);
				if (isShowNewbeeEffect)
				{
					gExtraBtns.m_newbee_effect.SetActiveBetter(false);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBagBuild_003Ec__AnonStorey9
		{
			internal int id;

			internal bool isShowNewbieEffect;

			internal GBagBuild gBagBuild;

			internal BattlePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_0024this.m_curActiveBuildPartBtn == go)
				{
					SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
					_0024this.m_curActiveBuildPartBtn = null;
				}
				else
				{
					_0024this.m_curActiveBuildPartBtn = go;
					SingletonMono<BuilderBehaviour>.Ins.ChangePrefab(id, -1, -1L);
				}
				if (isShowNewbieEffect)
				{
					gBagBuild.m_newbee_effect.SetActiveBetter(false);
					Utils.TriggerEvent(GuideEvent.StepCompleteAction);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CRefreshQuickUseItems_003Ec__AnonStoreyB
		{
			internal BagItem bagItem;

			internal GBuildBagItem gItem;

			internal BattlePanel _0024this;
		}

		[CompilerGenerated]
		private sealed class _003CRefreshQuickUseItems_003Ec__AnonStoreyA
		{
			internal ItemCfg itemCfg;

			internal bool isShowNewbieEffect;

			internal _003CRefreshQuickUseItems_003Ec__AnonStoreyB _003C_003Ef__ref_002411;

			internal void _003C_003Em__0(GameObject go)
			{
				if (itemCfg.type == 13 || itemCfg.type == 17 || itemCfg.type == 82 || itemCfg.type == 25)
				{
					if (_003C_003Ef__ref_002411._0024this.m_Self.Swiming)
					{
						return;
					}
					if (Battle.Ins.SelfPlayer.CurrentWeaponInsId == _003C_003Ef__ref_002411.bagItem.instanceId)
					{
						Battle.Ins.SelfPlayer.PlayChangeWeapen(-1);
					}
					else if (itemCfg.type == 25)
					{
						Battle.Ins.SelfPlayer.PlayChangeWeapen(_003C_003Ef__ref_002411.bagItem.instanceId);
					}
					else if (_003C_003Ef__ref_002411.bagItem.duration > 0)
					{
						Battle.Ins.SelfPlayer.PlayChangeWeapen(_003C_003Ef__ref_002411.bagItem.instanceId);
					}
					else
					{
						AlertBox.Show(Utils.GetString(279));
					}
				}
				else
				{
					Singleton<BagMgr>.Ins.OnClickUseItem(_003C_003Ef__ref_002411.bagItem.instanceId);
				}
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview != null)
				{
					if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.Id == _003C_003Ef__ref_002411.bagItem.itemId)
					{
						Utils.TriggerEvent(BattleEvent.OnWantExitBuildState, true);
						SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
					}
					else if (itemCfg.type == 78)
					{
						if (_003C_003Ef__ref_002411._0024this.m_Self.CheckCanEnterBuildState())
						{
							if (!_003C_003Ef__ref_002411._0024this.m_Self.InBuildState)
							{
								Utils.TriggerEvent(BattleEvent.OnWantEnterBuildState, true);
							}
							SingletonMono<BuilderBehaviour>.Ins.ChangePrefab(_003C_003Ef__ref_002411.bagItem.itemId, _003C_003Ef__ref_002411.bagItem.instanceId, -1L);
						}
						else
						{
							AlertBox.Show(Utils.GetString(137));
						}
					}
					else
					{
						Utils.TriggerEvent(BattleEvent.OnWantExitBuildState, false);
						SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
					}
				}
				else if (itemCfg.type == 78)
				{
					if (_003C_003Ef__ref_002411._0024this.m_Self.CheckCanEnterBuildState())
					{
						if (!_003C_003Ef__ref_002411._0024this.m_Self.InBuildState)
						{
							Utils.TriggerEvent(BattleEvent.OnWantEnterBuildState, true);
						}
						SingletonMono<BuilderBehaviour>.Ins.ChangePrefab(_003C_003Ef__ref_002411.bagItem.itemId, _003C_003Ef__ref_002411.bagItem.instanceId, -1L);
					}
					else
					{
						AlertBox.Show(Utils.GetString(137));
					}
				}
				else if (itemCfg.type == 112)
				{
					SingletonMono<PlantMgr>.Ins.CreatePreviewPrefab(_003C_003Ef__ref_002411.bagItem.itemId);
				}
				if (isShowNewbieEffect)
				{
					_003C_003Ef__ref_002411.gItem.m_newbee_effect.SetActiveBetter(false);
					Utils.TriggerEvent(GuideEvent.StepCompleteAction);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillBuildChildsIcons_003Ec__AnonStoreyC
		{
			internal BuildPart p;

			internal BattlePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_0024this.m_curActiveBuildPartBtn == go)
				{
					SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
					_0024this.m_curActiveBuildPartBtn = null;
				}
				else
				{
					_0024this.m_curActiveBuildPartBtn = go;
					SingletonMono<BuilderBehaviour>.Ins.ChangePrefab(p.id, -1, -1L);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003COnOpenKaiJing_003Ec__AnonStoreyD
		{
			internal ItemCfg aim;

			internal BattlePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (_0024this.m_JingDic.ContainsKey(aim.id) && _0024this.m_JingDic[aim.id] != null)
				{
					UnityEngine.Object.Destroy(_0024this.m_JingDic[aim.id]);
				}
				if (_0024this.m_Self.CurGun == null || !_0024this.m_Self.CurGun.IsKaijing)
				{
					o.gameObject.SetActiveBetter(false);
				}
				o.transform.SetParent(_0024this.m_jing.transform, false);
				_0024this.m_JingDic.Add(aim.id, o);
			}
		}

		private PlayerController m_Self;

		private RectTransform m_randRect;

		private bool m_PressShoot;

		private Image m_BloodFillImage;

		private Image m_myBloodFillImage;

		private Image m_BloodFill2Image;

		private Image _hungerFillImage;

		private Image _waterFillImage;

		private Color selfSeatColor = Color.yellow;

		private Color otherSeatColor = Color.white;

		private Image[] carSeatImages;

		private Image[] planeSeatImages;

		private Image[] shipSeatImages;

		private Image[] moto2SeatImages;

		private Image m_bulletSlider1Image;

		private Image m_bulletSlider2Image;

		private Image m_bulletSlider3Image;

		private Image m_bulletSlider4Image;

		private Image m_bulletSlider1diImage;

		private Image m_bulletSlider2diImage;

		private Image m_bulletSlider3diImage;

		private Image m_bulletSlider4diImage;

		private Slider m_CarHpSlider;

		private Image m_CarChetiImage;

		private Image m_PlaneChetiImage;

		private Image m_ShipChetiImage;

		private Image m_Moto2ChetiImage;

		private GameObject m_curActiveBuildTypeBtn;

		private GameObject m_curActiveBuildPartBtn;

		private bool m_longPress;

		private bool m_InAutoRunBtn;

		private List<int> m_bagBuildsList;

		private ScrollRect m_bagBuildsScrollRect;

		private int LastCurrentPreviewId = -1;

		private int LastFramWeaponInsId = -1;

		private WaitForSeconds WaitSeconds5;

		private int m_SmartUseHpItemId = -1;

		private List<int> Hp50 = new List<int>
		{
			322, 302, 321, 301, 320, 300, 325, 305, 324, 304,
			323, 303
		};

		private List<int> Hp74 = new List<int>
		{
			320, 300, 321, 301, 323, 303, 324, 304, 325, 305,
			322, 302
		};

		private List<int> Hp100 = new List<int> { 323, 303, 324, 304, 325, 305, 322, 302 };

		private Image m_PulmonaryIamge;

		private RectTransform m_FireImageRect;

		private RectTransform m_FireBtnRect;

		private RectTransform m_QuickLookImageRect;

		private RectTransform m_QuickLookBtnRect;

		private int TouchId;

		private int m_IsFirstDrag;

		private int m_CountDownTime;

		private float m_ReciveCountDownTime;

		private Color32 m_equipBlue = new Color32(60, 153, 225, byte.MaxValue);

		private Color32 m_equipCheng = new Color32(byte.MaxValue, 169, 105, byte.MaxValue);

		private Color32 m_equipRed = new Color32(248, 67, 62, byte.MaxValue);

		private float m_KillItemShowTime = 3f;

		private Coroutine m_DequeueKillItemCoroutine;

		private Queue<SKillPlayer> m_killItemQueue = new Queue<SKillPlayer>();

		private Color32 killedRedColor = new Color32(byte.MaxValue, 108, 43, byte.MaxValue);

		private Color32 killerBlueColor = new Color32(138, 223, byte.MaxValue, byte.MaxValue);

		private Dictionary<int, GameObject> m_JingDic = new Dictionary<int, GameObject>();

		private Color32 cheng = new Color32(byte.MaxValue, 183, 107, byte.MaxValue);

		private Color32 hong = new Color32(byte.MaxValue, 55, 21, byte.MaxValue);

		private Color32 _color49 = new Color32(252, 221, 153, byte.MaxValue);

		private Color32 _color14 = new Color32(253, 154, 110, byte.MaxValue);

		private Color32 _color1 = new Color32(228, 50, 50, byte.MaxValue);

		private float _time10s = 10f;

		private Vector2 m_LastTouchPos;

		private GZhunxing m_GZhunxing;

		private WaitForSeconds m_zeroOneTime = new WaitForSeconds(0.1f);

		private WaitForSeconds m_zeroThreeTime = new WaitForSeconds(0.3f);

		private WaitForSeconds m_zeroFiveTime = new WaitForSeconds(0.5f);

		private WaitForSeconds m_OneSecondTime = new WaitForSeconds(1f);

		private Transform[] m_AllChildTrans;

		private readonly List<MsgBean> _msgList = new List<MsgBean>();

		private Toggle _ckbTrusteeship;

		private List<long> _mRoleIds = new List<long>();

		private long _mCurrentRoleId;

		private float _mCurrentRotation;

		private readonly float _mCompassDegreeOffset = 85f;

		private readonly float _mDegreeScale = 5f / 6f;

		private readonly float _mMinIconScale = 0.5f;

		private readonly float _mMaxIconScale = 1f;

		private readonly float _mDefaultVanishTime = 3f;

		private readonly float _mFootVanishTime = 1f;

		private readonly float _mVehicleSoundVanishTime = 3f;

		private readonly float _mAirdropSoundVanishTime = 4f;

		private Vector3 _mCenterWorldPos = default(Vector3);

		private Vector3 _mSharedVector3 = default(Vector3);

		private Vector2 _mSharedVector2 = default(Vector2);

		private HashSet<long> _mCurrentVehicleIds = new HashSet<long>();

		private List<SoundSource> _mSoundToTrack = new List<SoundSource>();

		private Dictionary<int, GameObject> _mSound2IconInMap = new Dictionary<int, GameObject>();

		private Dictionary<int, GameObject> _mSound2IconInMapDir = new Dictionary<int, GameObject>();

		private List<SoundSource> _mSoundSourceToRemove = new List<SoundSource>();

		private Dictionary<int, ObjectPool<GameObject>> _mSoundType2IconInMapPool = new Dictionary<int, ObjectPool<GameObject>>();

		private Dictionary<int, ObjectPool<GameObject>> _mSoundSource2IconInMapDirPool = new Dictionary<int, ObjectPool<GameObject>>();

		private ObjectPool<SoundSource> _mSoundSourcePool;

		private Dictionary<SoundSource, GameObject> _mSoundSource2IconInMap;

		private Dictionary<SoundSource, GameObject> _mSoundSource2IconInMapDir;

		private Transform _mSoundIconInMapParent;

		private readonly float _mZoomLevel;

		private readonly float MAP_SIZE;

		private Color _mSelfColor;

		private Color _mOtherColor;

		private RawImage _mBattleImage;

		private float _mWidthRate;

		private float _mHeightRate;

		private GPlayerMapMarks[] _mGPlayerMapMarksArray;

		private Vector2 _mMapUVCenter;

		private Image[] _mPosImageInMap;

		private Image[] _mDriveImageInMap;

		private Image[] _mJumpImageInMap;

		private Rect _mBattleImageRect;

		private int _mLineNumber;

		private float _mMapImageWidth;

		private readonly float GUIDE_LINE_LENGTH;

		private readonly int ENTER_TOXIC_ZONE_SOUNDID;

		private bool _mInToxicZone;

		private bool _mInSafeZone;

		private float _mDistanceToSafeZone;

		private float _mDistanceGasToSafe;

		private int _mGasTimeSliderWidth;

		private float _mPlayInToxicSoundTimeGap;

		private float _mPlayInToxicSoundTime;

		private RectTransform _mGasManRectTransform;

		private Image _image;

		private int _tag;

		private int m_selfIndex;

		private float _mapRate;

		private List<Image> _lineRowImages;

		private Color _outLineColor;

		private Color _inLineColor;

		private Vector2 vector2;

		private Vector2 _markDir;

		private Vector3 _markPos;

		private Vector2 _soundIconInMapPos;

		private Vector3 _mTransformPos;

		private Vector3 _airdropPos;

		private Vector3 _posOnMapV3;

		private Vector2 _lastCar;

		private Vector3 _dropItemDiePos;

		private Vector2 _dropItemDiePosLocal;

		private List<GameObject> _toolBoxPosGameObjects;

		private Vector3 _toolBoxPosVector3;

		private Vector3 _secondPos;

		private List<GameObject> _secondBattleGameObjects;

		private Vector3 _mineVector3;

		private List<GameObject> _minesGameObjects;

		private Dictionary<int, List<GameObject>> _minesGameObjectsDic;

		private Dictionary<int, int> _minesGameObjectsIndexDic;

		private int _smallIconDistance;

		private Dictionary<int, List<GameObject>> _plantsGameObjectsDic;

		private Dictionary<int, int> _plantsGameObjectsIndexDic;

		private List<GameObject> _treesGameObjects;

		private List<GameObject> _lajitongGameObjects;

		private Vector3 tep;

		private List<GameObject> _xiangziGameObjects;

		public Dictionary<int, AirDropDistance> AirDropDistanceDic;

		private WaitForSeconds _threeTime;

		private GameObject m_TouchPad;

		private GameObject m_zuo_shang_jiao;

		private GameObject m_map;

		private GameObject m_lefttop;

		private GameObject m_last_drive;

		private GameObject m_map_line_root;

		private GameObject m_map_line;

		private GameObject m_resource_tags;

		private GameObject m_tool_box_root;

		private GameObject m_tool_box;

		private GameObject m_second_battle_root;

		private GameObject m_second_battle_tag;

		private GameObject m_main_tags_root;

		private GameObject m_main_tag;

		private GameObject m_plant_tags_root;

		private GameObject m_plant_tag;

		private GameObject m_tree_tags_root;

		private GameObject m_tree_tag;

		private GameObject m_footSoundInMap;

		private GameObject m_vehicleSoundInMap;

		private GameObject m_gunSoundInMap;

		private GameObject m_gun_sound_pos_map;

		private GameObject m_gun_silence_sound_pos;

		private GameObject m_airdrop_pos_map;

		private GameObject m_airdrop_dir_map;

		private List<GPlayerMapMarks> m_team_memberslist;

		private GameObject[] m_team_members;

		private GameObject m_team_membersObj;

		private GameObject txt_coordinate;

		private Text txt_coordinateText;

		private GameObject btn_setting;

		private GameObject btn_louderSpeaker;

		private GameObject m_louderSpeakerDisabled;

		private GameObject btn_microPhone;

		private GameObject m_microphoneDisabled;

		private GameObject m_notWatch;

		private GameObject m_jing;

		private GameObject m_jingAim;

		private GJoystackType2 m_JoystackTypeCar;

		private GJoystackType2 m_JoystackType2;

		private GameObject m_InLand;

		private GameObject m_NearCar;

		private GameObject btn_NearCarDrive;

		private GameObject btn_NearCarUpCar;

		private GameObject m_swimPanel;

		private GameObject btn_SwimUp;

		private GameObject btn_SwimDown;

		private GameObject m_shootPanel;

		private GameObject m_OperationBtns;

		private GameObject btn_kaijing;

		private GameObject btn_leftFire;

		private GameObject btn_fire;

		private GameObject m_fireImage;

		private GameObject m_fireRealIcon;

		private GameObject btn_build;

		private List<GBulletSelectInfo> m_bulletsSelectlist;

		private GameObject[] m_bulletsSelect;

		private GameObject m_bulletsSelectObj;

		private GameObject btn_changeBullet;

		private GameObject m_moshi;

		private GameObject m_ShootModelImage;

		private GameObject btn_Auto2;

		private GameObject btn_SanLianfa2;

		private GameObject btn_Danfa2;

		private GameObject m_ChangeType2;

		private GameObject btn_up2;

		private GameObject m_JumpImage;

		private GameObject m_PatiziImage;

		private GameObject btn_crouch2;

		private GameObject btn_crouching2;

		private GameObject btn_pa2;

		private GameObject btn_paing2;

		private GameObject btn_autoRun;

		private GameObject btn_HelpOther;

		private GameObject m_car;

		private GameObject m_zaijiuInfo;

		private GameObject m_zaijvinfo;

		private GameObject m_carInfo;

		private GameObject[] m_carSeats;

		private GameObject m_carSeatsObj;

		private GameObject m_carcheti;

		private GameObject m_shipInfo;

		private GameObject[] m_shipSeats;

		private GameObject m_shipSeatsObj;

		private GameObject m_shipcheti;

		private GameObject m_planeInfo;

		private GameObject[] m_planeSeats;

		private GameObject m_planeSeatsObj;

		private GameObject m_planecheti;

		private GameObject m_moto2;

		private GameObject[] m_moto2Seats;

		private GameObject m_moto2SeatsObj;

		private GameObject m_moto2cheti;

		private GameObject m_moto3;

		private GameObject[] m_moto3Seats;

		private GameObject m_moto3SeatsObj;

		private GameObject m_moto3cheti;

		private GameObject txt_car_speed;

		private Text txt_car_speedText;

		private GameObject m_car_you;

		private GameObject m_car_you_slider;

		private GameObject m_car_blood;

		private GameObject m_car_blood_slider;

		private GameObject m_planeController;

		private GJoystackType2 m_JoystackType3;

		private GJoystackType2 m_JoystackType4;

		private GameObject m_carController;

		private GameObject btn_horn;

		private GameObject btn_addSpeed;

		private GameObject m_carButtons;

		private GameObject btn_carGasInput;

		private GameObject btn_carBrakeInput;

		private GameObject btn_carSteerLeftInput;

		private GameObject btn_carSteerRightInput;

		private GameObject m_CarBtns;

		private GameObject btn_downCar;

		private GameObject btn_addoil;

		private GameObject btn_tanshen;

		private GameObject btn_InCarDrive;

		private GameObject btn_huanzuo;

		private GameObject txt_TeamPlayerName;

		private Text txt_TeamPlayerNameText;

		private GameObject btn_quicklook;

		private GameObject m_quicklook;

		private List<GZhunxing> m_zhunxinglist;

		private GameObject[] m_zhunxing;

		private GameObject m_zhunxingObj;

		private GameObject m_zhunxingPoint;

		private GZhunxing m_zhunxing1;

		private GZhunxing m_zhunxing2;

		private GZhunxing m_zhunxing3;

		private GameObject btn_Gm;

		private GameObject m_PulmonaryPanel;

		private GameObject txt_use_item_name;

		private Text txt_use_item_nameText;

		private GameObject m_use_time;

		private GameObject txt_use_time;

		private Text txt_use_timeText;

		private GameObject txt_PlayerPos;

		private Text txt_PlayerPosText;

		private GameObject m_killTypeIcon;

		private GameObject m_killNumIcon;

		private GameObject m_xueDir;

		private GameObject m_ReduceHpImage;

		private GameObject m_shanghaizhunxing;

		private GameObject m_zhu;

		private GameObject m_zsj;

		private GameObject btn_shop;

		private GameObject btn_email;

		private GameObject m_mail_red_dot;

		private GameObject btn_ladder_task;

		private GameObject m_ladder_task_red;

		private GameObject btn_friend;

		private GameObject m_friend_red_dot;

		private GameObject btn_info;

		private GameObject m_info_red;

		private GameObject btn_welfare;

		private GameObject m_welfare_red_dot;

		private GameObject btn_union;

		private GameObject m_;

		private GameObject btn_active;

		private GameObject m_red_active;

		private GameObject btn_firstcharge;

		private GameObject m_first_charge_red;

		private GameObject btn_active1;

		private GameObject btn_;

		private GameObject m_shop_no_oversea;

		private List<GBuildPartItem> m_buildChildslist;

		private GameObject[] m_buildChilds;

		private GameObject m_buildChildsObj;

		private GameObject scp_bagBuilds;

		private GBagBuild m_cell;

		private GameObject btn_pack;

		private GameObject m_buildPartTypes;

		private GameObject btn_null;

		private GameObject btn_obstacle;

		private GameObject btn_floor;

		private GameObject btn_stair;

		private GameObject btn_window;

		private GameObject btn_wall;

		private GameObject btn_fundation;

		private GameObject btn_changeButton;

		private GameObject m_changeButtonLight;

		private List<GBuildBagItem> m_bagItemslist;

		private GameObject[] m_bagItems;

		private GameObject m_bagItemsObj;

		private GameObject m_headInfo;

		private GameObject m_player_head_icon;

		private GameObject m_player_frame;

		private GameObject btn_head;

		private GameObject txt_player_name;

		private Text txt_player_nameText;

		private GameObject m_BloodSlider2;

		private GameObject m_BloodFill2;

		private GameObject m_ysj;

		private GameObject m_mybloodSlilder;

		private GameObject m_mybloodFillImage;

		private GameObject txt_mybloodNum;

		private Text txt_mybloodNumText;

		private GameObject m_sld_hunger;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject m_sld_water;

		private GameObject txt_water;

		private Text txt_waterText;

		private GameObject btn_chat;

		private GameObject[] m_chats;

		private GameObject m_chatsObj;

		private GameObject txt_chat;

		private Text txt_chatText;

		private GameObject txt_chat_1;

		private Text txt_chat_1Text;

		private GameObject m_operatesPanel;

		private GameObject btn_operate;

		private GameObject[] m_buildBaseBtns;

		private GameObject m_buildBaseBtnsObj;

		private GameObject btn_opUp;

		private GameObject btn_opMove;

		private GameObject btn_opRotate;

		private GameObject btn_opRepair;

		private GameObject btn_opDestory;

		private GameObject m_plantFinishTimeSlider;

		private GameObject txt_plantFinishTime;

		private Text txt_plantFinishTimeText;

		private GameObject m_buildoperatesbtns;

		private GameObject txt_aimedBtnsTopName;

		private Text txt_aimedBtnsTopNameText;

		private List<GExtraBtns> m_extraBtnslist;

		private GameObject[] m_extraBtns;

		private GameObject m_extraBtnsObj;

		private List<GKillItem> m_killslist;

		private GameObject[] m_kills;

		private GameObject m_killsObj;

		private GameObject m_saveSelfPanel;

		private GameObject btn_saveSelf;

		private GameObject btn_giveUp;

		private GameObject m_player_head_bg;

		private GameObject m_sld_hunder_image;

		private GameObject m_sld_water_image;

		private GameObject m_die_dropitem_pos;

		private GameObject m_autoRunChoosed;

		private GameObject m_autoRunCircle;

		private GameObject m_image_no_saveitem;

		private GameObject ckb_trusteeship;

		private GameObject m_trusteeship;

		private GameObject txt_set_trusteeship;

		private Text txt_set_trusteeshipText;

		private GameObject m_saveBtnEffect;

		private GameObject m_lajitong_tags_root;

		private GameObject m_lajitong_tag;

		private GameObject m_xiangzi_tags_root;

		private GameObject m_xiangzi_tag;

		private GameObject txt_gameTime;

		private Text txt_gameTimeText;

		private GameObject m_reduceHp10;

		private GameObject m_reduceHp;

		private GameObject m_addHp;

		private GameObject m_jieEffect;

		private GameObject m_reduceHpFull;

		private GameObject m_nightDi;

		private GameObject m_huanghunDi;

		private GameObject m_dayDi;

		private GameObject m_qingchenDi;

		private GameObject m_nightIcon;

		private GameObject m_dayIcon;

		private GameObject m_red_active_0;

		private GameObject m_trusteeship_circle;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache2;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache3;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache4;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache5;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache6;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache7;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache8;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cache9;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cacheA;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cacheB;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cacheC;

		[CompilerGenerated]
		private static DownUpListener.VoidDelegate _003C_003Ef__am_0024cacheD;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cacheE;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cacheF;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache10;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache11;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache12;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache13;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache14;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache15;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache16;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache17;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache18;

		[CompilerGenerated]
		private static ObjectPool<SoundSource>.CreateObject<SoundSource> _003C_003Ef__am_0024cache19;

		public BattlePanel()
		{
			if (_003C_003Ef__am_0024cache19 == null)
			{
				_003C_003Ef__am_0024cache19 = _003C_mSoundSourcePool_003Em__2F;
			}
			_mSoundSourcePool = new ObjectPool<SoundSource>(15, _003C_003Ef__am_0024cache19);
			_mSoundSource2IconInMap = new Dictionary<SoundSource, GameObject>();
			_mSoundSource2IconInMapDir = new Dictionary<SoundSource, GameObject>();
			_mZoomLevel = 15f;
			MAP_SIZE = 6000f;
			_mSelfColor = Color.yellow;
			_mOtherColor = Color.white;
			_mMapUVCenter = default(Vector2);
			_mBattleImageRect = default(Rect);
			_mLineNumber = 1000;
			GUIDE_LINE_LENGTH = 10f;
			ENTER_TOXIC_ZONE_SOUNDID = 358;
			_mInSafeZone = true;
			_mPlayInToxicSoundTimeGap = 5f;
			_tag = 1;
			m_selfIndex = 1;
			_lineRowImages = new List<Image>();
			_outLineColor = new Color(0.95686275f, 37f / 51f, 0.38039216f);
			_inLineColor = new Color(1f, 1f, 1f, 0.3f);
			vector2 = Vector2.zero;
			_markDir = Vector2.zero;
			_markPos = Vector3.zero;
			_soundIconInMapPos = Vector3.zero;
			_mTransformPos = Vector3.zero;
			_airdropPos = new Vector3(-1764.4f, 180.9f, -323f);
			_posOnMapV3 = default(Vector3);
			_dropItemDiePos = Vector3.zero;
			_dropItemDiePosLocal = Vector2.zero;
			_toolBoxPosGameObjects = new List<GameObject>();
			_toolBoxPosVector3 = Vector3.zero;
			_secondPos = Vector3.zero;
			_secondBattleGameObjects = new List<GameObject>();
			_mineVector3 = Vector3.zero;
			_minesGameObjects = new List<GameObject>();
			_minesGameObjectsDic = new Dictionary<int, List<GameObject>>();
			_minesGameObjectsIndexDic = new Dictionary<int, int>();
			_smallIconDistance = 150;
			_plantsGameObjectsDic = new Dictionary<int, List<GameObject>>();
			_plantsGameObjectsIndexDic = new Dictionary<int, int>();
			_treesGameObjects = new List<GameObject>();
			_lajitongGameObjects = new List<GameObject>();
			tep = default(Vector3);
			_xiangziGameObjects = new List<GameObject>();
			AirDropDistanceDic = new Dictionary<int, AirDropDistance>();
			_threeTime = new WaitForSeconds(3f);
			m_team_memberslist = new List<GPlayerMapMarks>();
			m_bulletsSelectlist = new List<GBulletSelectInfo>();
			m_zhunxinglist = new List<GZhunxing>();
			m_buildChildslist = new List<GBuildPartItem>();
			m_bagItemslist = new List<GBuildBagItem>();
			m_extraBtnslist = new List<GExtraBtns>();
			m_killslist = new List<GKillItem>();
			base._002Ector();
		}

		protected override void onInit()
		{
			Battle.Ins.MyBattlePanel = this;
			Main.SetResolution();
			SingletonMono<BuildManager>.Ins.SelfRoleId = Singleton<RoleMgr>.Ins.info.roleId;
			SingletonMono<BuildManager>.Ins.SelfColliders = Battle.Ins.SelfPlayer.MyColliders;
			View.SetTexture(m_lefttop, "texture/map_map");
			carSeatImages = new Image[m_carSeats.Length];
			for (int i = 0; i < m_carSeats.Length; i++)
			{
				carSeatImages[i] = m_carSeats[i].GetComponent<Image>();
			}
			moto2SeatImages = new Image[m_moto2Seats.Length];
			for (int j = 0; j < m_moto2Seats.Length; j++)
			{
				moto2SeatImages[j] = m_moto2Seats[j].GetComponent<Image>();
			}
			planeSeatImages = new Image[m_planeSeats.Length];
			for (int k = 0; k < m_planeSeats.Length; k++)
			{
				planeSeatImages[k] = m_planeSeats[k].GetComponent<Image>();
			}
			shipSeatImages = new Image[m_shipSeats.Length];
			for (int l = 0; l < m_shipSeats.Length; l++)
			{
				shipSeatImages[l] = m_shipSeats[l].GetComponent<Image>();
			}
			DownUpListener downUpListener = DownUpListener.Get(btn_Gm);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__0;
			}
			downUpListener.onDown = _003C_003Ef__am_0024cache0;
			ClickListener clickListener = ClickListener.Get(btn_horn, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003ConInit_003Em__1;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache1;
			ClickListener.Get(btn_crouch2, "no").onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_pa2, "no").onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_up2, "no").onClick = OnClickUp2;
			ClickListener clickListener2 = ClickListener.Get(m_map, string.Empty);
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003ConInit_003Em__4;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache2;
			DownUpListener.Get(m_JoystackType2.m_yaoganTrigger).onDown = _003ConInit_003Em__5;
			DownUpListener.Get(m_JoystackType2.m_yaoganTrigger).onUp = _003ConInit_003Em__6;
			DragListener.Get(m_JoystackType2.m_yaoganTrigger).onDrag = _003ConInit_003Em__7;
			DownUpListener.Get(btn_kaijing).onDown = OnClickKaiJing;
			DownUpListener.Get(btn_Danfa2).onDown = OnDownDanfa;
			DownUpListener.Get(btn_SanLianfa2).onDown = OnDownLianfa;
			DownUpListener.Get(btn_Auto2).onDown = OnDownAuto;
			PressedListener.Get(btn_SwimUp).onPressed = _003ConInit_003Em__8;
			PressedListener.Get(btn_SwimDown).onPressed = _003ConInit_003Em__9;
			ScreenDownUpListener.Get(m_TouchPad).onDown = OnDownTouchPad;
			DragListener.Get(m_TouchPad).onDrag = OnDragTouchPad;
			ScreenDownUpListener.Get(m_TouchPad).onUp = OnUpTouchPad;
			DragListener.Get(btn_fire).onDrag = OnDragFire;
			DownUpListener.Get(btn_fire).onDown = OnDownFire;
			DownUpListener.Get(btn_leftFire).onDown = OnDownFire;
			DownUpListener.Get(btn_fire).onUp = OnUpFire;
			DownUpListener.Get(btn_leftFire).onUp = OnUpFire;
			DownUpListener downUpListener2 = DownUpListener.Get(btn_info);
			if (_003C_003Ef__am_0024cache3 == null)
			{
				_003C_003Ef__am_0024cache3 = _003ConInit_003Em__A;
			}
			downUpListener2.onUp = _003C_003Ef__am_0024cache3;
			m_info_red.SetActiveBetter(false);
			m_swimPanel.SetActiveBetter(false);
			m_PulmonaryPanel.SetActiveBetter(false);
			foreach (GExtraBtns item in m_extraBtnslist)
			{
				item.gameObject.SetActiveBetter(false);
			}
			m_myBloodFillImage = m_mybloodFillImage.GetComponent<Image>();
			m_BloodFill2Image = m_BloodFill2.GetComponent<Image>();
			_hungerFillImage = m_sld_hunder_image.GetComponent<Image>();
			_waterFillImage = m_sld_water_image.GetComponent<Image>();
			m_FireImageRect = m_fireImage.GetComponent<RectTransform>();
			m_FireBtnRect = btn_fire.GetComponent<RectTransform>();
			m_QuickLookImageRect = m_quicklook.GetComponent<RectTransform>();
			m_QuickLookBtnRect = btn_quicklook.GetComponent<RectTransform>();
			m_PulmonaryIamge = m_use_time.GetComponent<Image>();
			m_bulletsSelectObj.SetActive(false);
			m_CarHpSlider = m_car_blood_slider.GetComponent<Slider>();
			m_CarChetiImage = m_carcheti.GetComponent<Image>();
			m_PlaneChetiImage = m_planecheti.GetComponent<Image>();
			m_ShipChetiImage = m_shipcheti.GetComponent<Image>();
			m_Moto2ChetiImage = m_moto2cheti.GetComponent<Image>();
			ClickListener.Get(btn_downCar, string.Empty).onClick = OnClickDownCar;
			ClickListener.Get(btn_NearCarDrive, string.Empty).onClick = OnClickDriveCar;
			ClickListener.Get(btn_InCarDrive, string.Empty).onClick = OnClickInCarDrive;
			ClickListener.Get(btn_NearCarUpCar, string.Empty).onClick = OnClickUpCar;
			ClickListener.Get(btn_huanzuo, string.Empty).onClickLimit05s = OnClickHuanZuo;
			ClickListener.Get(btn_tanshen, string.Empty).onClickLimit05s = OnClickCarTanShen;
			DownUpListener downUpListener3 = DownUpListener.Get(btn_addSpeed);
			if (_003C_003Ef__am_0024cache4 == null)
			{
				_003C_003Ef__am_0024cache4 = _003ConInit_003Em__B;
			}
			downUpListener3.onDown = _003C_003Ef__am_0024cache4;
			DownUpListener downUpListener4 = DownUpListener.Get(btn_addSpeed);
			if (_003C_003Ef__am_0024cache5 == null)
			{
				_003C_003Ef__am_0024cache5 = _003ConInit_003Em__C;
			}
			downUpListener4.onUp = _003C_003Ef__am_0024cache5;
			DownUpListener downUpListener5 = DownUpListener.Get(btn_carGasInput);
			if (_003C_003Ef__am_0024cache6 == null)
			{
				_003C_003Ef__am_0024cache6 = _003ConInit_003Em__D;
			}
			downUpListener5.onDown = _003C_003Ef__am_0024cache6;
			DownUpListener downUpListener6 = DownUpListener.Get(btn_carGasInput);
			if (_003C_003Ef__am_0024cache7 == null)
			{
				_003C_003Ef__am_0024cache7 = _003ConInit_003Em__E;
			}
			downUpListener6.onUp = _003C_003Ef__am_0024cache7;
			DownUpListener downUpListener7 = DownUpListener.Get(btn_carBrakeInput);
			if (_003C_003Ef__am_0024cache8 == null)
			{
				_003C_003Ef__am_0024cache8 = _003ConInit_003Em__F;
			}
			downUpListener7.onDown = _003C_003Ef__am_0024cache8;
			DownUpListener downUpListener8 = DownUpListener.Get(btn_carBrakeInput);
			if (_003C_003Ef__am_0024cache9 == null)
			{
				_003C_003Ef__am_0024cache9 = _003ConInit_003Em__10;
			}
			downUpListener8.onUp = _003C_003Ef__am_0024cache9;
			DownUpListener downUpListener9 = DownUpListener.Get(btn_carSteerRightInput);
			if (_003C_003Ef__am_0024cacheA == null)
			{
				_003C_003Ef__am_0024cacheA = _003ConInit_003Em__11;
			}
			downUpListener9.onDown = _003C_003Ef__am_0024cacheA;
			DownUpListener downUpListener10 = DownUpListener.Get(btn_carSteerRightInput);
			if (_003C_003Ef__am_0024cacheB == null)
			{
				_003C_003Ef__am_0024cacheB = _003ConInit_003Em__12;
			}
			downUpListener10.onUp = _003C_003Ef__am_0024cacheB;
			DownUpListener downUpListener11 = DownUpListener.Get(btn_carSteerLeftInput);
			if (_003C_003Ef__am_0024cacheC == null)
			{
				_003C_003Ef__am_0024cacheC = _003ConInit_003Em__13;
			}
			downUpListener11.onDown = _003C_003Ef__am_0024cacheC;
			DownUpListener downUpListener12 = DownUpListener.Get(btn_carSteerLeftInput);
			if (_003C_003Ef__am_0024cacheD == null)
			{
				_003C_003Ef__am_0024cacheD = _003ConInit_003Em__14;
			}
			downUpListener12.onUp = _003C_003Ef__am_0024cacheD;
			ClickListener.Get(btn_HelpOther, string.Empty).onClick = _003ConInit_003Em__15;
			ClickListener clickListener3 = ClickListener.Get(btn_setting, string.Empty);
			if (_003C_003Ef__am_0024cacheE == null)
			{
				_003C_003Ef__am_0024cacheE = _003ConInit_003Em__16;
			}
			clickListener3.onClick = _003C_003Ef__am_0024cacheE;
			ClickListener clickListener4 = ClickListener.Get(btn_saveSelf, string.Empty);
			if (_003C_003Ef__am_0024cacheF == null)
			{
				_003C_003Ef__am_0024cacheF = _003ConInit_003Em__17;
			}
			clickListener4.onClick = _003C_003Ef__am_0024cacheF;
			ClickListener clickListener5 = ClickListener.Get(btn_giveUp, string.Empty);
			if (_003C_003Ef__am_0024cache10 == null)
			{
				_003C_003Ef__am_0024cache10 = _003ConInit_003Em__18;
			}
			clickListener5.onClick = _003C_003Ef__am_0024cache10;
			EnterExitListener.Get(btn_autoRun).onEnter = _003ConInit_003Em__19;
			EnterExitListener.Get(btn_autoRun).onExit = _003ConInit_003Em__1A;
			DragListener.Get(btn_quicklook).onDrag = _003ConInit_003Em__1B;
			DragListener.Get(btn_quicklook).onEndDrag = _003ConInit_003Em__1C;
			Battle.Ins.JingUI = m_jing;
			txt_TeamPlayerName.SetActiveBetter(false);
			btn_louderSpeaker.SetActiveBetter(ConstsBs.OpenVoiceRoom);
			btn_microPhone.SetActiveBetter(ConstsBs.OpenVoiceRoom);
			EventHandlers.OnBuildModeChanged += OnBuildModeChanged;
			BattleEvent.OnExitedBuildState = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnExitedBuildState, new Utils.VoidDelegate(OnExitedBuildState));
			BattleEvent.OnExitedBuildStateFromQuickUse = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnExitedBuildStateFromQuickUse, new Utils.VoidDelegate(OnExitedBuildStateFromQuickUse));
			BattleEvent.OnEnteredBuildState = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnteredBuildState, new Utils.VoidDelegate(OnEnteredBuildState));
			BattleEvent.OnEnteredBuildStateFromQuickUse = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnteredBuildStateFromQuickUse, new Utils.VoidDelegate(OnEnteredBuildStateFromQuickUse));
			ClickListener.Get(btn_build, string.Empty).onClick = OnClickBuild;
			ClickListener.Get(btn_fundation.gameObject, string.Empty).onClick = OnClickFundation;
			btn_fundation.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_wall.gameObject, string.Empty).onClick = OnClickWall;
			btn_wall.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_stair.gameObject, string.Empty).onClick = OnClickStair;
			btn_stair.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_floor.gameObject, string.Empty).onClick = OnClickFloor;
			btn_floor.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_window.gameObject, string.Empty).onClick = OnClickWindow;
			btn_window.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_obstacle.gameObject, string.Empty).onClick = OnClickObstacle;
			btn_obstacle.transform.Find("Text").gameObject.SetActive(false);
			ClickListener.Get(btn_changeButton, string.Empty).onClick = OnClickChangeButton;
			ClickListener.Get(btn_opDestory, string.Empty).onClick = OnClickBuildDestory;
			ClickListener.Get(btn_opUp, string.Empty).onClick = OnClickOpUp;
			ClickListener.Get(btn_opRotate, string.Empty).onClick = OnClickOpRotate;
			ClickListener.Get(btn_opMove, string.Empty).onClick = OnClickOpMove;
			ClickListener.Get(btn_opRepair, string.Empty).onClick = OnClickOpRepair;
			ClickListener.Get(btn_null, string.Empty).onClick = OnClickBagAdd;
			ClickListener.Get(btn_chat, string.Empty).onClick = OnClickChat;
			ClickListener clickListener6 = ClickListener.Get(btn_ladder_task, string.Empty);
			if (_003C_003Ef__am_0024cache11 == null)
			{
				_003C_003Ef__am_0024cache11 = _003ConInit_003Em__1D;
			}
			clickListener6.onClick = _003C_003Ef__am_0024cache11;
			ClickListener.Get(btn_changeBullet, string.Empty).onClick = OnClickChangeBullet;
			PressListener.Get(btn_changeBullet).onPress = _003ConInit_003Em__1E;
			BagEvent.RefreshQuickUse = (Utils.VoidDelegate)Delegate.Combine(BagEvent.RefreshQuickUse, new Utils.VoidDelegate(OnQuickUseItemChanged));
			BagEvent.AddOrRemoveBullet = (Utils.Int2Delegate)Delegate.Combine(BagEvent.AddOrRemoveBullet, new Utils.Int2Delegate(OnBulletChange));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			BagEvent.CloseBagPanel = (Utils.VoidDelegate)Delegate.Combine(BagEvent.CloseBagPanel, new Utils.VoidDelegate(_003ConInit_003Em__1F));
			EventHandlers.OnAddExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnAddExtraBtn, new Utils.IntDelegate(OnAddExtraBtn));
			EventHandlers.OnRemoveExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnRemoveExtraBtn, new Utils.IntDelegate(OnRemoveExtraBtn));
			EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimedPart));
			EventHandlers.OnAimedGo = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedGo, new Utils.LongDelegate(OnAimedPart));
			SRequestTimeToGrow.handler = (SRequestTimeToGrow.Handler)Delegate.Combine(SRequestTimeToGrow.handler, new SRequestTimeToGrow.Handler(OnRequestTimeToGrow));
			BagEvent.DurabilityChange = (Utils.IntLongDelegate)Delegate.Combine(BagEvent.DurabilityChange, new Utils.IntLongDelegate(OnNaiJiuChange));
			BattleEvent.OnChangeDay = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangeDay, new Utils.VoidDelegate(_003ConInit_003Em__20));
			BattleEvent.OnChangeNight = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangeNight, new Utils.VoidDelegate(_003ConInit_003Em__21));
			BattleEvent.OnChangeHuanghun = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangeHuanghun, new Utils.VoidDelegate(_003ConInit_003Em__22));
			BattleEvent.OnChangeQingchen = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangeQingchen, new Utils.VoidDelegate(_003ConInit_003Em__23));
			ResetBuildsBts();
			m_bagItemsObj.SetActive(true);
			m_buildPartTypes.SetActive(false);
			btn_build.SetActive(false);
			RefreshQuickUseItems();
			m_operatesPanel.SetActiveBetter(false);
			m_changeButtonLight.SetActiveBetter(false);
			m_headInfo.SetActive(false);
			OnAimedPart(-1L);
			AttachNonCombatBtnEvent();
			scp_bagBuilds.gameObject.SetActive(false);
			btn_null.transform.Find("Text").gameObject.SetActive(false);
			WaitSeconds5 = new WaitForSeconds(1f);
			OnInitMap();
			if (Battle.Ins.SelfInfo.isInBuildState)
			{
				OnEnteredBuildState();
			}
		}

		private void OnClickHeadInfo(GameObject go)
		{
			ViewMgr.Ins.ShowView<BattleInfoPanel>((long)UIContext.Get(go));
		}

		private void OnNaiJiuChange(int itemId, long insId)
		{
			for (int i = 0; i < m_bagItemslist.Count; i++)
			{
				GBuildBagItem gBuildBagItem = m_bagItemslist[i];
				ItemCfg itemCfg = ItemCfg.Get(itemId);
				BagItem value;
				if (Singleton<BagMgr>.Ins.QuickUseItems.TryGetValue(i, out value) && value.instanceId == insId)
				{
					View.SetSlider(gBuildBagItem.m_durationSlider, (float)value.duration / (float)itemCfg.durability);
				}
			}
		}

		private void ShowBullets()
		{
			DelayInvoker.DelayInvoke("cancelLongPress", 0.5f, _003CShowBullets_003Em__24);
			if (m_Self.CurGun != null)
			{
				m_bulletsSelectObj.SetActive(true);
				BagMgr.GunData gunDate = Singleton<BagMgr>.Ins.GetGunDate(m_Self.CurGun.InsId);
				for (int i = 0; i < gunDate.GunCfg.bulletIds.Count; i++)
				{
					_003CShowBullets_003Ec__AnonStorey7 _003CShowBullets_003Ec__AnonStorey = new _003CShowBullets_003Ec__AnonStorey7();
					_003CShowBullets_003Ec__AnonStorey._0024this = this;
					GBulletSelectInfo gBulletSelectInfo = m_bulletsSelectlist[i];
					gBulletSelectInfo.m_highLight.SetActiveBetter(false);
					View.SetLabelText(gBulletSelectInfo.txt_numText, Singleton<BagMgr>.Ins.GetItemNum(gunDate.GunCfg.bulletIds[i]));
					View.SetItemSprite(gBulletSelectInfo.m_icon, ItemCfg.Get(gunDate.GunCfg.bulletIds[i]).icon);
					_003CShowBullets_003Ec__AnonStorey.bulletId = gunDate.GunCfg.bulletIds[i];
					ClickListener.Get(gBulletSelectInfo.gameObject, string.Empty).onClick = _003CShowBullets_003Ec__AnonStorey._003C_003Em__0;
				}
				int lastGunBulletIndex = Singleton<BagMgr>.Ins.GetLastGunBulletIndex(m_Self.CurGun.InsId);
				if (lastGunBulletIndex >= 0)
				{
					GBulletSelectInfo gBulletSelectInfo2 = m_bulletsSelectlist[lastGunBulletIndex];
					gBulletSelectInfo2.m_highLight.SetActiveBetter(true);
				}
				for (int j = gunDate.GunCfg.bulletIds.Count; j < m_bulletsSelectlist.Count; j++)
				{
					m_bulletsSelectlist[j].gameObject.SetActiveBetter(false);
				}
			}
		}

		private void OnClickOpMove(GameObject go)
		{
			if ((bool)SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart)
			{
				SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.gameObject.SetActive(false);
				SingletonMono<BuilderBehaviour>.Ins.ChangePrefab(SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.Id, -1, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId);
			}
		}

		private void OnClickOpRotate(GameObject go)
		{
			if (SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingBaseOp())
			{
				SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.ChangeAreaState(State.Free);
				SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.RemoveSelfSocketBusySpace();
				SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.transform.SetEulerAnglesY(SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.transform.eulerAngles.y + (float)SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyCfg.OnceAngle);
				CRotateBuilding cRotateBuilding = new CRotateBuilding();
				cRotateBuilding.buildingId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
				cRotateBuilding.outmap = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.GetOutDic().AsDictionary();
				Client2Gs.Ins.Send(cRotateBuilding);
				SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.ChangeAreaState(State.Busy);
			}
			else
			{
				SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.SelfRotateAngle += (short)SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.MyCfg.OnceAngle;
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.SelfRotateAngle > 360)
				{
					SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.SelfRotateAngle -= 360;
				}
			}
		}

		private void OnClickOpRepair(GameObject go)
		{
			if (SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingBaseOp() && SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.NeedFix())
			{
				CFixBuilding cFixBuilding = new CFixBuilding();
				cFixBuilding.instanceId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
				Client2Gs.Ins.Send(cFixBuilding);
			}
		}

		private void OnAimedPart(long arg)
		{
			txt_aimedBtnsTopName.transform.parent.gameObject.SetActiveBetter(false);
			foreach (GExtraBtns item in m_extraBtnslist)
			{
				item.gameObject.SetActiveBetter(false);
			}
		}

		private void OnRemoveExtraBtn(int id)
		{
			if (m_operatesPanel.activeSelf)
			{
				Transform transform = m_buildoperatesbtns.transform.Find(id.ToString());
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		private void OnRequestTimeToGrow(SRequestTimeToGrow msg)
		{
			Battle.Ins.PlantDic[msg.instanceId].GrowFinishTime = (int)Time.time + msg.time;
			if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject != null)
			{
				PlantInfo plantInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as PlantInfo;
				if (plantInfo != null)
				{
					m_plantFinishTimeSlider.SetActiveBetter(true);
					SetAimedPlantGrowFinishTime(plantInfo);
				}
			}
		}

		private void OnAddExtraBtn(int id)
		{
			_003COnAddExtraBtn_003Ec__AnonStorey8 _003COnAddExtraBtn_003Ec__AnonStorey = new _003COnAddExtraBtn_003Ec__AnonStorey8();
			_003COnAddExtraBtn_003Ec__AnonStorey.id = id;
			if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart == null && SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject == null)
			{
				return;
			}
			if (_003COnAddExtraBtn_003Ec__AnonStorey.id == 110)
			{
				PlantInfo plantInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as PlantInfo;
				if (plantInfo != null && plantInfo.MySPlantInfo != null && !plantInfo.MySPlantInfo.isGrownUp)
				{
					if (plantInfo.GrowFinishTime == 0)
					{
						CRequestTimeToGrow cRequestTimeToGrow = new CRequestTimeToGrow();
						cRequestTimeToGrow.instanceId = plantInfo.MySPlantInfo.instanceId;
						Client2Gs.Ins.Send(cRequestTimeToGrow);
					}
					else
					{
						m_plantFinishTimeSlider.SetActiveBetter(true);
						SetAimedPlantGrowFinishTime(plantInfo);
					}
				}
				else
				{
					m_plantFinishTimeSlider.SetActiveBetter(false);
				}
			}
			m_buildoperatesbtns.SetActiveBetter(true);
			m_operatesPanel.SetActiveBetter(true);
			_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns = GetOneExtraBtn();
			_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns.gameObject.SetActive(true);
			_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns.gameObject.name = _003COnAddExtraBtn_003Ec__AnonStorey.id.ToString();
			DoButtonCfg doButtonCfg = DoButtonCfg.Get(_003COnAddExtraBtn_003Ec__AnonStorey.id);
			View.SetLabelText(_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns.txt_btnFontsText, doButtonCfg.name);
			txt_aimedBtnsTopName.transform.parent.gameObject.SetActiveBetter(true);
			if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null)
			{
				View.SetLabelText(txt_aimedBtnsTopNameText, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MapObjectName);
			}
			else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject != null)
			{
				MapObject component = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.GetComponent<MapObject>();
				if (CutPlantCfg.Get(component.CfgId) != null)
				{
					View.SetLabelText(txt_aimedBtnsTopNameText, CutPlantCfg.Get(component.CfgId).name);
				}
			}
			_003COnAddExtraBtn_003Ec__AnonStorey.isShowNewbeeEffect = Singleton<GuideMgr>.Ins.IsExtraBtnEffectShow(_003COnAddExtraBtn_003Ec__AnonStorey.id);
			_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns.m_newbee_effect.SetActiveBetter(_003COnAddExtraBtn_003Ec__AnonStorey.isShowNewbeeEffect);
			ClickListener.Get(_003COnAddExtraBtn_003Ec__AnonStorey.gExtraBtns.gameObject, string.Empty).onClick = _003COnAddExtraBtn_003Ec__AnonStorey._003C_003Em__0;
		}

		public void SetAimedPlantGrowFinishTime(PlantInfo p)
		{
			if (m_plantFinishTimeSlider.activeSelf)
			{
				CutPlantCfg cutPlantCfg = CutPlantCfg.Get(p.MySPlantInfo.plantId);
				int num = (int)((float)p.GrowFinishTime - Time.time);
				if (num <= 0)
				{
					m_plantFinishTimeSlider.SetActiveBetter(false);
					return;
				}
				View.SetLabelText(txt_plantFinishTimeText, Utils.GetString(262) + Utils.GetCountDownTime(num));
				View.SetSlider(m_plantFinishTimeSlider, (float)(cutPlantCfg.grownUpTime - num) / (float)cutPlantCfg.grownUpTime);
			}
		}

		public GExtraBtns GetOneExtraBtn()
		{
			foreach (GExtraBtns item in m_extraBtnslist)
			{
				if (!item.gameObject.activeSelf)
				{
					return item;
				}
			}
			Debug.LogError("no extra btn to use");
			return null;
		}

		private void OnEnteredBuildStateFromQuickUse()
		{
			ResetBuildsBts();
			ResetScpBagBuilds();
			btn_build.SetActiveBetter(true);
			btn_fire.SetActiveBetter(false);
		}

		private void OnExitedBuildStateFromQuickUse()
		{
			btn_build.SetActiveBetter(false);
			btn_fire.SetActiveBetter(true);
			UpdateGunImages();
			RefreshQuickUseItems();
		}

		private void OnClickChangeBullet(GameObject go)
		{
			if (!m_longPress)
			{
				m_bulletsSelectObj.SetActive(false);
				m_longPress = false;
				Battle.Ins.HuanDan(Singleton<BagMgr>.Ins.ChangeBulletItemId(m_Self.CurGun.InsId));
			}
		}

		private void UpdateAimInfo()
		{
			try
			{
				m_player_head_icon.SetActiveBetter(true);
				m_player_head_bg.SetActiveBetter(true);
				m_player_frame.SetActiveBetter(false);
				ClickListener.Get(btn_head, string.Empty).onClick = null;
				if ((bool)SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart && !SingletonMono<BuilderBehaviour>.Ins.CurrentPreview)
				{
					m_headInfo.SetActiveBetter(true);
					View.SetLabelText(txt_player_nameText, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MapObjectName);
					View.SetItemSprite(m_player_head_icon, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyCfg.icon);
					m_player_head_icon.SetActiveBetter(true);
					m_player_head_bg.SetActiveBetter(false);
					SetAimSlider(m_BloodSlider2, m_BloodFill2Image, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.Hp, SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MaxHp);
				}
				else if ((bool)SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject && !SingletonMono<BuilderBehaviour>.Ins.CurrentPreview)
				{
					m_headInfo.SetActiveBetter(true);
					if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is MonsterController)
					{
						MonsterController monsterController = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as MonsterController;
						View.SetLabelText(txt_player_nameText, monsterController.MyCfg.name);
						SetAimSlider(m_BloodSlider2, m_BloodFill2Image, monsterController.Hp, monsterController.MyCfg.hp);
						View.SetItemSprite(m_player_head_icon, monsterController.MyCfg.headIcon);
						View.SetItemSprite(m_player_head_bg, monsterController.MyCfg.headIconBg);
					}
					else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is OtherPlayerController)
					{
						OtherPlayerController otherPlayerController = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as OtherPlayerController;
						UIContext.Attach(btn_head, otherPlayerController.RoleId);
						if (otherPlayerController.NewPlayer)
						{
							View.SetLabelText(txt_player_nameText, otherPlayerController.Name + Utils.GetString(347));
						}
						else
						{
							View.SetLabelText(txt_player_nameText, otherPlayerController.Name);
						}
						SetAimSlider(m_BloodSlider2, m_BloodFill2Image, otherPlayerController.GetHp, otherPlayerController.MaxHp, otherPlayerController.IsDownWaitSave);
						View.SetItemSprite(m_player_head_icon, RoleHeadCfg.Get(otherPlayerController.PlayerInfo.headId).icon);
						RoleHeadFrameCfg roleHeadFrameCfg = RoleHeadFrameCfg.Get(otherPlayerController.PlayerInfo.headFrameId);
						if (roleHeadFrameCfg != null)
						{
							View.SetItemSprite(m_player_frame, roleHeadFrameCfg.frame);
						}
						View.SetItemSprite(m_player_head_bg, "head/head_ren_di_zd");
						ClickListener.Get(btn_head, string.Empty).onClick = OnClickHeadInfo;
					}
					else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is TreeInfo)
					{
						TreeInfo treeInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as TreeInfo;
						View.SetLabelText(txt_player_nameText, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MapObjectName);
						SetAimSlider(m_BloodSlider2, m_BloodFill2Image, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.Hp, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MaxHp);
						if (treeInfo.MyCfg != null)
						{
							View.SetItemSprite(m_player_head_icon, treeInfo.MyCfg.headIcon);
							View.SetItemSprite(m_player_head_bg, treeInfo.MyCfg.headIconBg);
						}
					}
					else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is Mine)
					{
						Mine mine = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as Mine;
						View.SetLabelText(txt_player_nameText, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MapObjectName);
						SetAimSlider(m_BloodSlider2, m_BloodFill2Image, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.Hp, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MaxHp);
						View.SetItemSprite(m_player_head_icon, mine.MyCfg.headIcon);
						View.SetItemSprite(m_player_head_bg, mine.MyCfg.headIconBg);
					}
					else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is TrashcanCfgInfo)
					{
						TrashcanCfgInfo trashcanCfgInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as TrashcanCfgInfo;
						View.SetLabelText(txt_player_nameText, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MapObjectName);
						SetAimSlider(m_BloodSlider2, m_BloodFill2Image, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.Hp, SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject.MaxHp);
						View.SetItemSprite(m_player_head_icon, trashcanCfgInfo.MyCfg.headIcon);
						View.SetItemSprite(m_player_head_bg, trashcanCfgInfo.MyCfg.headIconBg);
					}
					else
					{
						m_headInfo.SetActiveBetter(false);
					}
				}
				else
				{
					m_headInfo.SetActiveBetter(false);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private void OnClickOpUp(GameObject go)
		{
			if (!SingletonMono<BuildManager>.Ins.Online || !SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingBaseOp())
			{
				return;
			}
			if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.CanUpdate())
			{
				CUpgradeBuilding cUpgradeBuilding = new CUpgradeBuilding();
				cUpgradeBuilding.instanceId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
				Client2Gs.Ins.Send(cUpgradeBuilding);
			}
			else if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyNextLvCfg != null)
			{
				int lessMaterilInfo = GetLessMaterilInfo(SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyNextLvCfg.id);
				if (lessMaterilInfo < 0)
				{
					AlertBox.Show(Utils.GetString(135) + Math.Abs(lessMaterilInfo) + Utils.GetString(ItemCfg.Get(SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyNextLvCfg.materialInfo.itemId).name));
				}
			}
		}

		private void OnEnteredBuildState()
		{
			m_bagItemsObj.SetActive(false);
			m_buildPartTypes.SetActive(true);
			m_changeButtonLight.SetActiveBetter(true);
			btn_build.SetActiveBetter(true);
			btn_fire.SetActiveBetter(false);
			ResetBuildsBts();
			ResetScpBagBuilds();
		}

		private void OnExitedBuildState()
		{
			ResetBuildsBts();
			m_bagItemsObj.SetActive(true);
			RefreshQuickUseItems();
			m_buildPartTypes.SetActive(false);
			m_changeButtonLight.SetActiveBetter(false);
			btn_build.SetActiveBetter(false);
			btn_fire.SetActiveBetter(true);
			UpdateGunImages();
		}

		private void OnClickBuildDestory(GameObject go)
		{
			if (SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingBaseOp())
			{
				SingletonMono<BuilderBehaviour>.Ins.WantRemovePrefab();
			}
		}

		private void OnItemChange(int itemId, int num)
		{
			if (scp_bagBuilds.activeSelf)
			{
				ShowBagBuilds();
			}
			RefreshQuickUseItems();
		}

		private void UpdateOperatesPanel()
		{
			if (SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingBaseOp())
			{
				SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.Aim);
				m_operatesPanel.SetActiveBetter(true);
				m_buildBaseBtnsObj.SetActiveBetter(true);
				m_buildoperatesbtns.SetActiveBetter(false);
				m_plantFinishTimeSlider.SetActiveBetter(false);
				UpdateBuildBasebtn();
			}
			else if (SingletonMono<BuilderBehaviour>.Ins.CanShowBuildingOtherOp())
			{
				SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.Aim);
				m_operatesPanel.SetActiveBetter(true);
				m_buildBaseBtnsObj.SetActiveBetter(false);
				m_buildoperatesbtns.SetActiveBetter(true);
				m_plantFinishTimeSlider.SetActiveBetter(false);
			}
			else if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview != null)
			{
				m_operatesPanel.SetActiveBetter(true);
				m_buildBaseBtnsObj.SetActiveBetter(true);
				m_buildoperatesbtns.SetActiveBetter(false);
				m_plantFinishTimeSlider.SetActiveBetter(false);
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.MyCfg.OnceAngle > 0)
				{
					for (int i = 0; i < m_buildBaseBtns.Length; i++)
					{
						if (i != 4)
						{
							m_buildBaseBtns[i].SetActiveBetter(false);
						}
					}
					m_buildBaseBtns[4].SetActiveBetter(true);
				}
				else
				{
					HideAllBuildBaseBtns();
				}
			}
			else if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject != null)
			{
				if (m_Self.FSMUpBody.CurrentState.ID != StateID.Build)
				{
					if (!(SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject is PlantInfo))
					{
						m_plantFinishTimeSlider.SetActiveBetter(false);
					}
					SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.Aim);
				}
				m_buildBaseBtnsObj.SetActiveBetter(false);
			}
			else
			{
				m_buildBaseBtnsObj.SetActiveBetter(false);
				m_buildoperatesbtns.SetActiveBetter(false);
				m_plantFinishTimeSlider.SetActiveBetter(false);
				m_operatesPanel.SetActiveBetter(false);
				SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.Aim);
			}
		}

		private void UpdateBuildBasebtn()
		{
			if (!m_buildBaseBtnsObj.gameObject.activeSelf)
			{
				return;
			}
			if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.AllowOp)
			{
				List<int> list = new List<int>(SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.MyCfg.btns);
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.NoOwner || !SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.HavePermit)
				{
					list.Remove(2);
					list.Remove(4);
				}
				if (!SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.NeedFix())
				{
					list.Remove(6);
				}
				for (int i = 0; i < m_buildBaseBtns.Length; i++)
				{
					if (list.Contains(i + 1))
					{
						m_buildBaseBtns[i].SetActiveBetter(true);
					}
					else
					{
						m_buildBaseBtns[i].SetActiveBetter(false);
					}
				}
			}
			else
			{
				HideAllBuildBaseBtns();
			}
		}

		private void HideAllBuildBaseBtns()
		{
			for (int i = 0; i < m_buildBaseBtns.Length; i++)
			{
				m_buildBaseBtns[i].SetActiveBetter(false);
			}
		}

		private void OnBulletChange(int insId, int bulletChangeNum)
		{
		}

		private void UpdateChatPos(bool reset)
		{
			RectTransform component = btn_chat.GetComponent<RectTransform>();
			if (reset)
			{
				component.anchoredPosition = new Vector2(component.anchoredPosition.x, 0f);
			}
			else
			{
				component.anchoredPosition = new Vector2(component.anchoredPosition.x, 132f);
			}
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_mybloodNumText, Singleton<RoleMgr>.Ins.Blood);
			if (Singleton<RoleMgr>.Ins.Blood <= 0)
			{
				m_ReduceHpImage.SetActiveBetter(true);
			}
			SetHpSlider(m_mybloodSlilder, m_myBloodFillImage, Singleton<RoleMgr>.Ins.Blood, Singleton<RoleMgr>.Ins.BloodMax, Battle.Ins.SelfPlayer.IsDownWaitSave);
			View.SetLabelText(txt_mybloodNumText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			if ((float)Singleton<RoleMgr>.Ins.Hunger * 1f / (float)ConstsBs.MAX_HUNGER > 0.1f)
			{
				m_jieEffect.SetActiveBetter(false);
			}
			else
			{
				m_jieEffect.SetActiveBetter(true);
			}
			View.SetSlider(m_sld_hunger, (float)Singleton<RoleMgr>.Ins.Hunger * 1f / (float)ConstsBs.MAX_HUNGER);
			SetSliderValueColor(_hungerFillImage, (float)Singleton<RoleMgr>.Ins.Hunger * 1f / (float)ConstsBs.MAX_HUNGER, false);
			View.SetLabelText(txt_waterText, Singleton<RoleMgr>.Ins.Water);
			View.SetSlider(m_sld_water, (float)Singleton<RoleMgr>.Ins.Water * 1f / (float)ConstsBs.MAX_STRENGTH);
			SetSliderValueColor(_waterFillImage, (float)Singleton<RoleMgr>.Ins.Water * 1f / (float)ConstsBs.MAX_STRENGTH, false);
		}

		private void HideCurActiveBuildPartBtnToggle()
		{
			if (m_curActiveBuildPartBtn != null)
			{
				Toggle component = m_curActiveBuildPartBtn.GetComponent<Toggle>();
				if (component != null)
				{
					component.isOn = false;
				}
			}
		}

		private void OnClickBagAdd(GameObject go)
		{
			SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
			m_buildChildsObj.SetActiveBetter(false);
			HideCurActiveBuildPartBtnToggle();
			if (scp_bagBuilds.gameObject.activeSelf)
			{
				scp_bagBuilds.gameObject.SetActive(false);
				btn_null.transform.Find("Text").gameObject.SetActive(false);
				UpdateChatPos(true);
				ResetScpBagBuilds();
				return;
			}
			scp_bagBuilds.gameObject.SetActive(true);
			btn_null.transform.Find("Text").gameObject.SetActive(true);
			if (m_curActiveBuildTypeBtn != null)
			{
				m_curActiveBuildTypeBtn.transform.Find("Text").gameObject.SetActiveBetter(false);
				m_curActiveBuildTypeBtn = null;
			}
			ShowBagBuilds();
			UpdateChatPos(false);
		}

		private void OnClickChat(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<ChatPanel>();
		}

		private void ShowBagBuilds()
		{
			UIScrollPanel component = scp_bagBuilds.GetComponent<UIScrollPanel>();
			m_bagBuildsList = Singleton<BagMgr>.Ins.GetBuildIds();
			if (m_bagBuildsList.Count > 7)
			{
				m_bagBuildsScrollRect.enabled = true;
			}
			else
			{
				m_bagBuildsScrollRect.enabled = false;
			}
			component.Reset(m_bagBuildsList.Count, FillBagBuild);
		}

		private void FillBagBuild(GameObject cell, int index)
		{
			_003CFillBagBuild_003Ec__AnonStorey9 _003CFillBagBuild_003Ec__AnonStorey = new _003CFillBagBuild_003Ec__AnonStorey9();
			_003CFillBagBuild_003Ec__AnonStorey._0024this = this;
			_003CFillBagBuild_003Ec__AnonStorey.gBagBuild = cell.GetComponent<GBagBuild>();
			_003CFillBagBuild_003Ec__AnonStorey.id = m_bagBuildsList[index];
			ItemCfg itemCfg = ItemCfg.Get(_003CFillBagBuild_003Ec__AnonStorey.id);
			View.SetItemSprite(_003CFillBagBuild_003Ec__AnonStorey.gBagBuild.m_icon, itemCfg.icon);
			View.SetLabelText(_003CFillBagBuild_003Ec__AnonStorey.gBagBuild.txt_nameText, Singleton<BagMgr>.Ins.GetItemNum(_003CFillBagBuild_003Ec__AnonStorey.id));
			_003CFillBagBuild_003Ec__AnonStorey.gBagBuild.GetComponent<Toggle>().isOn = false;
			m_curActiveBuildPartBtn = null;
			_003CFillBagBuild_003Ec__AnonStorey.isShowNewbieEffect = Singleton<GuideMgr>.Ins.IsBuildModeBagBuildEffectShow(_003CFillBagBuild_003Ec__AnonStorey.id);
			_003CFillBagBuild_003Ec__AnonStorey.gBagBuild.m_newbee_effect.SetActiveBetter(_003CFillBagBuild_003Ec__AnonStorey.isShowNewbieEffect);
			ClickListener.Get(_003CFillBagBuild_003Ec__AnonStorey.gBagBuild.gameObject, string.Empty).onClick = _003CFillBagBuild_003Ec__AnonStorey._003C_003Em__0;
		}

		private void OnQuickUseItemChanged()
		{
			RefreshQuickUseItems();
		}

		private void ResetBuildsBts()
		{
			if (m_curActiveBuildTypeBtn != null)
			{
				Toggle component = m_curActiveBuildTypeBtn.GetComponent<Toggle>();
				if (component != null)
				{
					component.isOn = false;
				}
			}
			if (m_curActiveBuildPartBtn != null)
			{
				Toggle component2 = m_curActiveBuildPartBtn.GetComponent<Toggle>();
				if (component2 != null)
				{
					component2.isOn = false;
				}
			}
			m_curActiveBuildPartBtn = null;
			CheckBuildChildsClose(null);
		}

		private void ResetScpBagBuilds()
		{
			btn_null.GetComponent<Toggle>().isOn = false;
			if (m_curActiveBuildPartBtn != null && m_curActiveBuildPartBtn.GetComponent<Toggle>() != null)
			{
				m_curActiveBuildPartBtn.GetComponent<Toggle>().isOn = false;
			}
		}

		private void RefreshQuickUseItems()
		{
			for (int i = 0; i < m_bagItemslist.Count; i++)
			{
				GBuildBagItem gBuildBagItem = m_bagItemslist[i];
				ClickListener.Get(gBuildBagItem.gameObject, string.Empty).onClick = null;
				gBuildBagItem.m_heightLignt.SetActiveBetter(false);
				gBuildBagItem.m_gunInfo.SetActiveBetter(false);
			}
			for (int j = 0; j < m_bagItemslist.Count; j++)
			{
				_003CRefreshQuickUseItems_003Ec__AnonStoreyB _003CRefreshQuickUseItems_003Ec__AnonStoreyB = new _003CRefreshQuickUseItems_003Ec__AnonStoreyB();
				_003CRefreshQuickUseItems_003Ec__AnonStoreyB._0024this = this;
				_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem = m_bagItemslist[j];
				if (Singleton<BagMgr>.Ins.QuickUseItems.TryGetValue(j, out _003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem))
				{
					_003CRefreshQuickUseItems_003Ec__AnonStoreyA _003CRefreshQuickUseItems_003Ec__AnonStoreyA = new _003CRefreshQuickUseItems_003Ec__AnonStoreyA();
					_003CRefreshQuickUseItems_003Ec__AnonStoreyA._003C_003Ef__ref_002411 = _003CRefreshQuickUseItems_003Ec__AnonStoreyB;
					_003CRefreshQuickUseItems_003Ec__AnonStoreyA.itemCfg = ItemCfg.Get(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.itemId);
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_s.SetActive(true);
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_bind.SetActiveBetter(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.isBind);
					View.SetItemSprite(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_icon, _003CRefreshQuickUseItems_003Ec__AnonStoreyA.itemCfg.icon);
					View.SetLabelText(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.txt_numText, _003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.number);
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_durationSlider.SetActiveBetter(_003CRefreshQuickUseItems_003Ec__AnonStoreyA.itemCfg.durability > 0);
					View.SetSlider(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_durationSlider, (float)_003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.duration / (float)_003CRefreshQuickUseItems_003Ec__AnonStoreyA.itemCfg.durability);
					if (_003CRefreshQuickUseItems_003Ec__AnonStoreyA.itemCfg.type == 13)
					{
						BagMgr.GunData gunDate = Singleton<BagMgr>.Ins.GetGunDate(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.instanceId);
						if (gunDate != null)
						{
							gunDate.Index = j;
							View.SetLabelText(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.txt_numText, gunDate.CurBulletNum);
							_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_gunInfo.SetActiveBetter(true);
							int num = 0;
							if (gunDate.GunCfg.muzzleParts.Count > 0)
							{
								num++;
							}
							if (gunDate.GunCfg.aimParts.Count > 0)
							{
								num++;
							}
							if (gunDate.GunCfg.clipParts.Count > 0)
							{
								num++;
							}
							if (gunDate.GunCfg.qiangbaParts.Count > 0)
							{
								num++;
							}
							for (int k = 0; k < _003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_di.Length; k++)
							{
								_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_di[k].SetActiveBetter(k < num);
							}
							for (int l = 0; l < _003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_bluePoint.Length; l++)
							{
								_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_bluePoint[l].SetActiveBetter(l < gunDate.PartsId.Count);
							}
						}
					}
					else
					{
						_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_gunInfo.SetActiveBetter(false);
					}
					_003CRefreshQuickUseItems_003Ec__AnonStoreyA.isShowNewbieEffect = Singleton<GuideMgr>.Ins.IsShortcutEffectShow(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.bagItem.itemId);
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_newbee_effect.SetActiveBetter(_003CRefreshQuickUseItems_003Ec__AnonStoreyA.isShowNewbieEffect);
					ClickListener.Get(_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.gameObject, string.Empty).onClick = _003CRefreshQuickUseItems_003Ec__AnonStoreyA._003C_003Em__0;
				}
				else
				{
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_s.SetActive(false);
					_003CRefreshQuickUseItems_003Ec__AnonStoreyB.gItem.m_newbee_effect.SetActiveBetter(false);
				}
			}
			UpdateQuickUseItemHightLignt(true);
		}

		private void UpdateQuickUseItemHightLignt(bool force = false)
		{
			if (m_Self == null)
			{
				return;
			}
			try
			{
				foreach (GBuildBagItem item in m_bagItemslist)
				{
					if (!item.m_s.activeSelf)
					{
						item.m_heightLignt.SetActiveBetter(false);
					}
				}
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview != null && (force || LastCurrentPreviewId != (int)SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.InsId))
				{
					int quickUseIndexByInstanceId = Singleton<BagMgr>.Ins.GetQuickUseIndexByInstanceId(LastCurrentPreviewId);
					if (quickUseIndexByInstanceId != -1 && m_bagItemslist[quickUseIndexByInstanceId] != null)
					{
						m_bagItemslist[quickUseIndexByInstanceId].m_heightLignt.SetActiveBetter(false);
					}
					LastCurrentPreviewId = (int)SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.InsId;
					quickUseIndexByInstanceId = Singleton<BagMgr>.Ins.GetQuickUseIndexByInstanceId((int)SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.InsId);
					if (quickUseIndexByInstanceId != -1)
					{
						m_bagItemslist[quickUseIndexByInstanceId].m_heightLignt.SetActiveBetter(true);
					}
				}
				if (LastCurrentPreviewId != -1 && SingletonMono<BuilderBehaviour>.Ins.CurrentPreview == null)
				{
					int quickUseIndexByInstanceId2 = Singleton<BagMgr>.Ins.GetQuickUseIndexByInstanceId(LastCurrentPreviewId);
					if (quickUseIndexByInstanceId2 != -1 && m_bagItemslist[quickUseIndexByInstanceId2] != null)
					{
						m_bagItemslist[quickUseIndexByInstanceId2].m_heightLignt.SetActiveBetter(false);
					}
					LastCurrentPreviewId = -1;
				}
				if (force || m_Self.CurrentWeaponInsId != LastFramWeaponInsId)
				{
					int quickUseIndexByInstanceId3 = Singleton<BagMgr>.Ins.GetQuickUseIndexByInstanceId(LastFramWeaponInsId);
					if (quickUseIndexByInstanceId3 != -1 && m_bagItemslist[quickUseIndexByInstanceId3] != null)
					{
						m_bagItemslist[quickUseIndexByInstanceId3].m_heightLignt.SetActiveBetter(false);
					}
					LastFramWeaponInsId = m_Self.CurrentWeaponInsId;
					quickUseIndexByInstanceId3 = Singleton<BagMgr>.Ins.GetQuickUseIndexByInstanceId(m_Self.CurrentWeaponInsId);
					if (quickUseIndexByInstanceId3 != -1)
					{
						m_bagItemslist[quickUseIndexByInstanceId3].m_heightLignt.SetActiveBetter(true);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void OnClickChangeButton(GameObject go)
		{
			if (m_buildPartTypes.activeSelf)
			{
				Utils.TriggerEvent(BattleEvent.OnWantExitBuildState, false);
			}
			else if (m_Self.InBuildState)
			{
				m_buildPartTypes.SetActiveBetter(true);
				m_bagItemsObj.SetActiveBetter(false);
				SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
			}
			else
			{
				Utils.TriggerEvent(BattleEvent.OnWantEnterBuildState, false);
			}
		}

		public bool BuildBaseLabelIsShow()
		{
			if (m_buildPartTypes.activeSelf)
			{
				return true;
			}
			return false;
		}

		private void OnBuildModeChanged(BuildMode mode)
		{
			if (m_Self.FSMUpBody.CurrentState.ID == StateID.Build)
			{
				btn_build.SetActiveBetter(true);
				btn_fire.SetActiveBetter(false);
			}
			else
			{
				btn_build.SetActiveBetter(false);
				btn_fire.SetActiveBetter(true);
			}
		}

		private void OnClickFundation(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(2);
		}

		private void OnClickWall(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(3);
		}

		private void OnClickStair(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(4);
		}

		private void OnClickFloor(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(8);
		}

		private void OnClickWindow(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(6);
		}

		private void OnClickDoor(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(5);
		}

		private void OnClickObstacle(GameObject go)
		{
			CheckBuildChildsClose(go);
			FillBuildChildsIcons(7);
		}

		private void CheckBuildChildsClose(GameObject go)
		{
			SingletonMono<BuilderBehaviour>.Ins.ChangeMode(BuildMode.None);
			scp_bagBuilds.gameObject.SetActiveBetter(false);
			btn_null.transform.Find("Text").gameObject.SetActiveBetter(false);
			if (m_curActiveBuildTypeBtn == go)
			{
				if (m_curActiveBuildTypeBtn != null)
				{
					m_curActiveBuildTypeBtn.transform.Find("Text").gameObject.SetActiveBetter(false);
				}
				UpdateChatPos(true);
				m_buildChildsObj.SetActive(false);
				m_curActiveBuildTypeBtn = null;
			}
			else
			{
				UpdateChatPos(false);
				m_buildChildsObj.SetActive(true);
				if (m_curActiveBuildTypeBtn != null)
				{
					m_curActiveBuildTypeBtn.transform.Find("Text").gameObject.SetActiveBetter(false);
				}
				m_curActiveBuildTypeBtn = go;
				if (m_curActiveBuildTypeBtn != null)
				{
					m_curActiveBuildTypeBtn.transform.Find("Text").gameObject.SetActiveBetter(true);
				}
			}
			if (go == null)
			{
				UpdateChatPos(true);
				m_buildChildsObj.SetActive(false);
				m_curActiveBuildTypeBtn = null;
			}
		}

		private void FillBuildChildsIcons(int type)
		{
			List<BuildPart> basicPartByType = SingletonMono<BuildManager>.Ins.GetBasicPartByType(type);
			for (int i = 0; i < m_buildChildslist.Count; i++)
			{
				GBuildPartItem gBuildPartItem = m_buildChildslist[i];
				if (i < basicPartByType.Count)
				{
					_003CFillBuildChildsIcons_003Ec__AnonStoreyC _003CFillBuildChildsIcons_003Ec__AnonStoreyC = new _003CFillBuildChildsIcons_003Ec__AnonStoreyC();
					_003CFillBuildChildsIcons_003Ec__AnonStoreyC._0024this = this;
					_003CFillBuildChildsIcons_003Ec__AnonStoreyC.p = basicPartByType[i];
					if (_003CFillBuildChildsIcons_003Ec__AnonStoreyC.p != null)
					{
						gBuildPartItem.gameObject.SetActive(true);
						View.SetItemSprite(gBuildPartItem.m_icon, _003CFillBuildChildsIcons_003Ec__AnonStoreyC.p.icon);
						View.SetLabelText(gBuildPartItem.txt_nameText, _003CFillBuildChildsIcons_003Ec__AnonStoreyC.p.name);
						gBuildPartItem.gameObject.GetComponent<Toggle>().isOn = false;
						m_curActiveBuildPartBtn = null;
						ClickListener.Get(gBuildPartItem.gameObject, string.Empty).onClick = _003CFillBuildChildsIcons_003Ec__AnonStoreyC._003C_003Em__0;
					}
				}
				else
				{
					gBuildPartItem.gameObject.SetActive(false);
				}
			}
		}

		private void MailRedDotCountChange(int arg)
		{
			m_mail_red_dot.SetActiveBetter(arg > 0);
		}

		private int GetBasePartCanBuildCout(int itemId)
		{
			BuildPart buildPart = BuildPart.Get(itemId);
			return Singleton<BagMgr>.Ins.GetItemNum(buildPart.materialInfo.itemId) / buildPart.materialInfo.needNum;
		}

		private int GetLessMaterilInfo(int itemId)
		{
			BuildPart buildPart = BuildPart.Get(itemId);
			return Singleton<BagMgr>.Ins.GetItemNum(buildPart.materialInfo.itemId) - buildPart.materialInfo.needNum;
		}

		private void OnClickBuild(GameObject go)
		{
			if (m_Self.InBuildState)
			{
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview == null)
				{
					AlertBox.Show(138);
					return;
				}
				ShowWhyCantBuild();
				int lessMaterilInfo = GetLessMaterilInfo(SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.Id);
				if (lessMaterilInfo < 0 && SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.ProtoInsid == -1)
				{
					AlertBox.Show(Utils.GetString(135) + Math.Abs(lessMaterilInfo) + ItemCfg.Get(SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.MyCfg.materialInfo.itemId).name);
					return;
				}
				if (SingletonMono<BuilderBehaviour>.Ins.WantPlacePrefab())
				{
					Battle.Ins.PlayEffectAtWorldPos(SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.MyCfg.buildEffectId, SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.Center, SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.transform.forward);
					SingletonMono<AudioManager>.Ins.Play(SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.MyCfg.buildSoundId, SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.transform.position);
				}
			}
			Utils.TriggerEvent(BattleEvent.OnClickBuildBtn);
		}

		private void ShowWhyCantBuild()
		{
			if (!SingletonMono<BuilderBehaviour>.Ins.AllowPlacement)
			{
				switch (SingletonMono<BuilderBehaviour>.Ins.WhyCantBuild)
				{
				case BuilderBehaviour.CantBuildReson.NoEnoughSupport:
					AlertBox.Show(Utils.GetString(176));
					break;
				case BuilderBehaviour.CantBuildReson.TooNearToPart:
					AlertBox.Show(Utils.GetString(179));
					break;
				case BuilderBehaviour.CantBuildReson.TooHigh:
					AlertBox.Show(Utils.GetString(177));
					break;
				case BuilderBehaviour.CantBuildReson.HaveOtherBuildNear:
					AlertBox.Show(Utils.GetString(134));
					break;
				case BuilderBehaviour.CantBuildReson.UpPartTooNear:
					AlertBox.Show(Utils.GetString(178));
					break;
				case BuilderBehaviour.CantBuildReson.Common:
					AlertBox.Show(Utils.GetString(136));
					break;
				case BuilderBehaviour.CantBuildReson.notallow:
					AlertBox.Show(Utils.GetString(245));
					break;
				case BuilderBehaviour.CantBuildReson.Null:
					break;
				}
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			if (Singleton<RoleMgr>.Ins.IsGm)
			{
				ShowGm();
			}
			else
			{
				CloseGm();
			}
			m_Self = Battle.Ins.SelfPlayer;
			HideAllKillItem();
			btn_autoRun.SetActiveBetter(false);
			m_car.SetActiveBetter(false);
			m_jing.SetActiveBetter(false);
			HideSaveSelfPanel();
			m_autoRunChoosed.gameObject.SetActiveBetter(false);
			btn_crouching2.SetActiveBetter(false);
			btn_paing2.SetActiveBetter(false);
			btn_addoil.SetActiveBetter(false);
			m_killNumIcon.SetActiveBetter(false);
			m_killTypeIcon.SetActiveBetter(false);
			m_xueDir.SetActiveBetter(false);
			m_shanghaizhunxing.SetActive(false);
			m_bagBuildsScrollRect = scp_bagBuilds.GetComponent<ScrollRect>();
			BattleEvent.onSelfDie = (Utils.Vector3Delegate)Delegate.Combine(BattleEvent.onSelfDie, new Utils.Vector3Delegate(OnSelfDie));
			BattleEvent.OnSelfKilledSelf = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnSelfKilledSelf, new Utils.VoidDelegate(OnSelfKilledSelf));
			BattleEvent.OnCurLoadBulletNumChange = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnCurLoadBulletNumChange, new Utils.IntDelegate(OnCurLoadBulletNumChange));
			BattleEvent.OnNearCar = (BattleEvent.NearCar)Delegate.Combine(BattleEvent.OnNearCar, new BattleEvent.NearCar(OnNearCar));
			BattleEvent.OnGetOutVehicle = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnGetOutVehicle, new Utils.VoidDelegate(OnGetoutVehicle));
			BattleEvent.OnFarCar = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnFarCar, new Utils.VoidDelegate(OnFarCar));
			BattleEvent.OnSelfHpChange = (BattleEvent.OnHpChangeDelegate)Delegate.Combine(BattleEvent.OnSelfHpChange, new BattleEvent.OnHpChangeDelegate(OnSelfHpChange));
			BattleEvent.OnSelfDown = (Utils.BoolDelegate)Delegate.Combine(BattleEvent.OnSelfDown, new Utils.BoolDelegate(OnSelfDown));
			BattleEvent.OnEpChange = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnEpChange, new Utils.IntDelegate(OnEpChange));
			MailMgr ins = MailMgr.Ins;
			ins.RedDotCountChange = (Utils.IntDelegate)Delegate.Combine(ins.RedDotCountChange, new Utils.IntDelegate(MailRedDotCountChange));
			BattleEvent.OnChangeHandGun = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangeHandGun, new Utils.VoidDelegate(OnChangeHandGun));
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(OnPlayerDie));
			SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(OnPlayerReBirth));
			BattleEvent.OnOpenKaiJing = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnOpenKaiJing, new Utils.VoidDelegate(OnOpenKaiJing));
			BattleEvent.OnCloseKaiJing = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnCloseKaiJing, new Utils.VoidDelegate(OnCloseKaiJing));
			BattleEvent.OnOpenJiMiao = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnOpenJiMiao, new Utils.VoidDelegate(OnOpenJiMiao));
			BattleEvent.OnCloseJiMiao = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnCloseJiMiao, new Utils.VoidDelegate(OnCloseJiMiao));
			BattlePackEvent.PickItemDelegate = (Utils.VoidDelegate)Delegate.Combine(BattlePackEvent.PickItemDelegate, new Utils.VoidDelegate(OnPickItem));
			BattleEvent.OnChangePart = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnChangePart, new Utils.VoidDelegate(OnChangePart));
			BattleEvent.OnHited = (Utils.Vector3Delegate)Delegate.Combine(BattleEvent.OnHited, new Utils.Vector3Delegate(OnHited));
			BattleEvent.OnHitOtherPlayer = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnHitOtherPlayer, new Utils.VoidDelegate(OnHitOtherPlayer));
			SPublicMsg.handler = (SPublicMsg.Handler)Delegate.Combine(SPublicMsg.handler, new SPublicMsg.Handler(SPublicMsgHandle));
			SKillPlayer.handler = (SKillPlayer.Handler)Delegate.Combine(SKillPlayer.handler, new SKillPlayer.Handler(OnSKillPlayer));
			SOwnDiePlayer.handler = (SOwnDiePlayer.Handler)Delegate.Combine(SOwnDiePlayer.handler, new SOwnDiePlayer.Handler(OnSOwnDiePlayer));
			BattlePackEvent.DropItemDelegate = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.DropItemDelegate, new Utils.Int2Delegate(OnDropItem));
			BattlePackEvent.PickEquip = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.PickEquip, new Utils.Int2Delegate(OnPickEquip));
			BattlePackEvent.DropEquip = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.DropEquip, new Utils.Int2Delegate(OnDropEquip));
			BattlePackEvent.EquipHpDelegate = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.EquipHpDelegate, new Utils.Int2Delegate(OnEquipHpChange));
			BattleEvent.OnEnterCrouch = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnterCrouch, new Utils.VoidDelegate(OnEnterCrouch));
			BattleEvent.OnEnterPa = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnterPa, new Utils.VoidDelegate(OnEnterPa));
			BattleEvent.OnEnterStand = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnEnterStand, new Utils.VoidDelegate(OnEnterStand));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			TaskEvent.RefreshTaskRed = (Utils.VoidDelegate)Delegate.Combine(TaskEvent.RefreshTaskRed, new Utils.VoidDelegate(RefreshLadderTaskRed));
			LadderEvent.RefreshLadderRed = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLadderRed, new Utils.VoidDelegate(RefreshLadderTaskRed));
			OnMoneyChange();
			UpdateGunImages();
			try
			{
				StartCoroutine(ZeroOneSecondTick());
				StartCoroutine(ZeroThreeSecondTick());
				StartCoroutine(ZeroFiveSecondTick());
				StartCoroutine(OneSecondTick());
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			m_reduceHpFull.SetActiveBetter(false);
			m_reduceHp10.SetActiveBetter(false);
			m_addHp.SetActiveBetter(false);
			m_jieEffect.SetActiveBetter(false);
			OnSelfHpChange(m_Self.HP, 0);
			OnEpChange(m_Self.EP);
			m_ReduceHpImage.SetActiveBetter(false);
			btn_HelpOther.SetActiveBetter(false);
			if (Battle.Ins.SelfInfo.isSecondHp)
			{
				ShowSaveSelfPanel();
			}
			m_PatiziImage.SetActiveBetter(false);
			m_plantFinishTimeSlider.SetActiveBetter(false);
			OnChangeHandGun();
			InitEquipInfo();
			UpdateBtnPosBySetting();
			Singleton<BattleScMgr>.Ins.SendLoadFinish();
			m_cell.gameObject.SetActive(false);
			StartCoroutine(Tick5());
			AttachRedDotEvent();
			OnShowMap();
			RefreshLadderTaskRed();
			MailMgr.Ins.updateRedDotCount();
			BattleEvent.OnTrusteeshipDownFireAction += OnDownFire;
		}

		private void OnSelfDown(bool down)
		{
			if (!down)
			{
				m_notWatch.SetActiveBetter(true);
				HideSaveSelfPanel();
			}
		}

		private void RefreshLadderTaskRed()
		{
			m_ladder_task_red.SetActiveBetter(Singleton<LadderMgr>.Ins.IsShowRedDot() || Singleton<TaskMgr>.Ins.IsShowRedDot() || Singleton<LadderMgr>.Ins.IsShowBoxRedDot());
		}

		private IEnumerator Tick5()
		{
			while (true)
			{
				SetMainChat();
				yield return WaitSeconds5;
			}
		}

		private void SPublicMsgHandle(SPublicMsg msg)
		{
			if (msg.info.msgType == 1 || msg.info.msgType == 5)
			{
				if (_msgList.Count > 1)
				{
					_msgList.RemoveAt(0);
				}
				_msgList.Add(msg.info);
			}
		}

		private void SetChat(string str)
		{
			View.SetLabelText(txt_chatText, str);
		}

		private string GetSmallcircleTeamString(int index)
		{
			switch (index)
			{
			case 0:
				return Utils.GetString(250);
			case 1:
				return Utils.GetString(251);
			case 2:
				return Utils.GetString(252);
			case 3:
				return Utils.GetString(253);
			case 4:
				return Utils.GetString(254);
			default:
				Debug.LogError("team id wrong!!!");
				return Utils.GetString(250);
			}
		}

		private void OnClickExitGame(GameObject go)
		{
		}

		private void OnExitCustomSettingPanel()
		{
			UpdateBtnPosBySetting();
		}

		public void OnUpFire(GameObject go)
		{
			m_FireImageRect.anchoredPosition = Vector2.zero;
			if (m_Self.HaveThrowWeaponInHand)
			{
				Utils.TriggerEvent(BattleEvent.OnUpLei);
			}
			else if (Battle.Ins.SelfPlayer.CurGun != null)
			{
				Battle.Ins.SelfPlayer.CurGun.StopShoot();
			}
		}

		public void OnDownFire(GameObject go)
		{
			if (!m_Self.CheckCanFire())
			{
				return;
			}
			m_Self.WaitForRush = 1f;
			m_Self.CloseAutoRun();
			Battle.Ins.QuickLooking = false;
			m_Self.SetRotation(Battle.Ins.MainCamera.SelfTransform);
			if (Battle.Ins.SelfPlayer.CurGun != null)
			{
				Battle.Ins.SelfPlayer.CurGun.StartShoot();
				Battle.Ins.SelfPlayer.FreeType = false;
			}
			if (m_Self.HaveNearWeaponInHand)
			{
				if (m_Self.FSM.CurrentState.ID == StateID.Jump)
				{
					m_Self.FSMUpBody.SwitchState(StateID.Attack, true);
				}
				if (m_Self.FSM.CurrentState.ID != StateID.Pa)
				{
					m_Self.FSMUpBody.SwitchState(StateID.Attack, false);
				}
			}
			if (m_Self.HaveThrowWeaponInHand)
			{
				Utils.TriggerEvent(BattleEvent.OnDownThrowLei, m_Self.GetCurrentWeapon().WeaponCfg.id);
			}
			if (m_Self.IsKongshou)
			{
				OnDownQuanTou();
			}
			if ((bool)go)
			{
				BattleEvent.StopTrusteeship();
			}
		}

		private void HideOtherPanel()
		{
		}

		private void OnScrollPageChangedSame(int pageIndex)
		{
			Battle.Ins.HuanDan(Singleton<BagMgr>.Ins.ChangeBulletItemId(Singleton<BagMgr>.Ins.GetCurGunDate().InsId));
		}

		private void ShowGm()
		{
			btn_Gm.SetActiveBetter(true);
			txt_PlayerPos.SetActiveBetter(true);
			GameObject gameObject = GameObject.Find("FPS");
			if (gameObject != null)
			{
				gameObject.GetComponent<Text>().enabled = true;
			}
		}

		private void CloseGm()
		{
			btn_Gm.SetActiveBetter(false);
			txt_PlayerPos.SetActiveBetter(false);
			GameObject gameObject = GameObject.Find("FPS");
			if (gameObject != null)
			{
				gameObject.GetComponent<Text>().enabled = false;
			}
		}

		private void OnClickBag(GameObject go)
		{
			TweenTime.Begin(go, 0.5f, _003COnClickBag_003Em__25);
		}

		private void OnDownAuto(GameObject go)
		{
			if (m_Self.CurGun != null)
			{
				m_Self.CurGun.SetFireType(FireType.Danfa);
				UpdateGunFireType();
			}
		}

		private void OnDownLianfa(GameObject go)
		{
			if (m_Self.CurGun != null)
			{
				if (m_Self.CurGun.GunCfg.canAuto)
				{
					m_Self.CurGun.SetFireType(FireType.Auto);
					UpdateGunFireType();
				}
				else
				{
					m_Self.CurGun.SetFireType(FireType.Danfa);
					UpdateGunFireType();
				}
			}
		}

		private void OnDownDanfa(GameObject go)
		{
			if (m_Self.CurGun != null)
			{
				if (m_Self.CurGun.GunCfg.canLianfa)
				{
					m_Self.CurGun.SetFireType(FireType.LianFa);
					UpdateGunFireType();
				}
				else if (m_Self.CurGun.GunCfg.canAuto)
				{
					m_Self.CurGun.SetFireType(FireType.Auto);
					UpdateGunFireType();
				}
			}
		}

		private void GetBestHpItem()
		{
			Dictionary<int, int> hpItemDic = new Dictionary<int, int>();
			if ((float)m_Self.HP <= (float)ConstsBs.HpPlayer * 0.5f)
			{
				m_SmartUseHpItemId = GetBestHpItemId(hpItemDic, Hp50);
			}
			else if ((float)m_Self.HP <= (float)ConstsBs.HpPlayer * 0.74f)
			{
				m_SmartUseHpItemId = GetBestHpItemId(hpItemDic, Hp74);
			}
			else if (m_Self.HP < ConstsBs.HpPlayer)
			{
				m_SmartUseHpItemId = GetBestHpItemId(hpItemDic, Hp100);
			}
			else
			{
				m_SmartUseHpItemId = -1;
			}
		}

		private int GetBestHpItemId(Dictionary<int, int> hpItemDic, List<int> hpItemList)
		{
			foreach (int hpItem in hpItemList)
			{
				if (hpItemDic.ContainsKey(hpItem))
				{
					return hpItem;
				}
			}
			return -1;
		}

		public void UpdatePulmonaryPanel(float num, float time)
		{
			m_PulmonaryPanel.SetActiveBetter(true);
			m_PulmonaryIamge.fillAmount = num;
			View.SetLabelText(txt_use_timeText, time);
		}

		public void HidePulmonaryPanel()
		{
			m_PulmonaryPanel.SetActiveBetter(false);
		}

		private void UpdateJingBtn()
		{
			if ((bool)m_Self && m_Self.CurGun != null && m_Self.FSM.CurrentState.ID != StateID.Pa)
			{
				btn_kaijing.SetActiveBetter(m_Self.CheckCanFire());
			}
			else
			{
				btn_kaijing.SetActiveBetter(false);
			}
		}

		private void UpdateFireBtn()
		{
			btn_fire.SetActiveBetter(m_Self.CheckCanFire());
			if (SettingMgr.LeftShootMode == 0)
			{
				btn_leftFire.SetActiveBetter(m_Self.CheckCanFire() && m_Self.CurGun != null);
			}
			else if (SettingMgr.LeftShootMode == 1)
			{
				btn_leftFire.SetActiveBetter(m_Self.CheckCanFire() && m_Self.CurGun != null && (m_Self.CurGun.IsKaijing || m_Self.CurGun.IsJiMiao));
			}
			else
			{
				btn_leftFire.SetActiveBetter(false);
			}
		}

		private void UpdateBuildBtn()
		{
			if ((bool)m_Self && (m_Self.InBuildState || m_Self.InPlantState))
			{
				btn_build.SetActiveBetter(true);
			}
			else
			{
				btn_build.SetActiveBetter(false);
			}
		}

		private void CheckAutoRun()
		{
			btn_autoRun.SetActiveBetter(!m_Self.InCar);
		}

		private void OnClickSmartHpItem(GameObject go)
		{
			Utils.TriggerEvent(BattlePackEvent.UseItemDelegate, m_SmartUseHpItemId);
		}

		private void OnClickCarTanShen(GameObject go)
		{
			if (m_Self.Tanshen)
			{
				Utils.TriggerEvent(BattleEvent.OnNeedBackSeat);
			}
			else
			{
				Utils.TriggerEvent(BattleEvent.OnNeedTanshen);
			}
		}

		private void OnDownQuanTou()
		{
			if (Battle.Ins.SelfPlayer.CurGun == null)
			{
				Utils.TriggerEvent(BattleEvent.OnAttack);
				if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Jump)
				{
					Battle.Ins.SelfPlayer.FSMUpBody.SwitchState(StateID.Attack, true);
				}
				else if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID != StateID.Pa)
				{
					Battle.Ins.SelfPlayer.FSMUpBody.SwitchState(StateID.Attack, false);
				}
			}
		}

		private void OnDragFire(GameObject go)
		{
			SetFireCirclePos();
			if ((double)Math.Abs(DragListener.pointEventData.delta.x) > 0.1 || (double)Math.Abs(DragListener.pointEventData.delta.y) > 0.1)
			{
				Battle.Ins.MainCamera.DragCamera(DragListener.pointEventData.delta.x, DragListener.pointEventData.delta.y);
				if (!m_Self.IsDie)
				{
					Battle.Ins.AimHelp.TryLockTarget();
				}
			}
		}

		private void SetFireCirclePos()
		{
			SetDragCirclePos(m_FireBtnRect, m_FireImageRect);
		}

		private void SetQuickLookCirclePos()
		{
			SetDragCirclePos(m_QuickLookBtnRect, m_QuickLookImageRect);
		}

		private void SetDragCirclePos(RectTransform parentRect, RectTransform circleImageRect)
		{
			Vector2 localPoint = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, DragListener.pointEventData.position, Battle.Ins.UICamrea, out localPoint);
			float num = Vector2.Distance(Vector2.zero, localPoint);
			float num2 = parentRect.sizeDelta.x * 0.8f;
			if (num > num2)
			{
				num = num2;
				localPoint = localPoint.normalized * num2;
			}
			circleImageRect.anchoredPosition = localPoint;
		}

		private void OnUpTouchPad(GameObject go)
		{
			if (ScreenDownUpListener.pointEventData.pointerId == TouchId || ScreenDownUpListener.ClickNum == 0 || Input.touchCount == 0)
			{
				TouchId = -1;
				Battle.Ins.MainCamera.StopDragCamera();
				Battle.Ins.AimHelp.TryLockTarget();
			}
		}

		private void OnDownTouchPad(GameObject go)
		{
			if (ScreenDownUpListener.ClickNum > 0 || TouchId > 0)
			{
				return;
			}
			TouchId = ScreenDownUpListener.pointEventData.pointerId;
			m_IsFirstDrag = 0;
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.GetTouch(i).fingerId == TouchId)
				{
					m_LastTouchPos = Input.GetTouch(i).position;
				}
			}
			Battle.Ins.MainCamera.StartDragCamera();
			if (!m_Self.IsDie)
			{
				Battle.Ins.AimHelp.TryLockTarget();
				SetFireBtnPos();
			}
			if (Singleton<BattleScMgr>.Ins.NeedNewPlayerboot)
			{
				Utils.TriggerEvent(BattleEvent.OnDownTouchPad);
			}
			BattleEvent.StopTrusteeship();
		}

		private void OnDragTouchPad(GameObject go)
		{
			if (!m_Self.IsDie)
			{
				SetFireBtnPos();
			}
		}

		private void OnCarControllTypeChange(int driveMode)
		{
			if (m_Self.IsDriver)
			{
				bool flag = driveMode == 1;
				m_JoystackTypeCar.gameObject.SetActiveBetter(flag);
				m_carButtons.SetActiveBetter(!flag);
			}
		}

		private void SetFireBtnPos()
		{
			if (!SettingMgr.FireBtnFix)
			{
				Vector3 worldPoint = Vector3.zero;
				RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TouchPad.transform as RectTransform, DragListener.pointEventData.position, Battle.Ins.UICamrea, out worldPoint);
				m_FireBtnRect.position = worldPoint;
			}
		}

		private void UpdateKillMePanel(OtherPlayerController c)
		{
		}

		public void UpdateKillMeHp(OtherPlayerController c)
		{
		}

		private void SetVipLevel(GameObject go, int vipLevel)
		{
		}

		private void OnSelfDie(Vector3 vector3)
		{
		}

		public void HideWatch()
		{
		}

		public void HideBulletClothIcon()
		{
		}

		private void OnHitOtherPlayer()
		{
			m_shanghaizhunxing.SetActiveBetter(true);
		}

		private void OnHited(Vector3 vector3)
		{
			m_xueDir.SetActive(true);
			float num = MathUtils.Angle_360(to: new Vector3(0f - vector3.x, 0f, 0f - vector3.z), from: m_Self.PlayerTransform.forward);
			m_xueDir.transform.localEulerAngles = new Vector3(0f, 0f, 0f - num);
		}

		private void UpdateBtnPosBySetting()
		{
		}

		private void OnDropItem(int arg1, int arg2)
		{
			UpdateGunImages();
		}

		private void OnGetoutVehicle()
		{
			m_ChangeType2.SetActiveBetter(true);
		}

		private void OnDestroy()
		{
			SettingEvent.ExitCustomSaveSettingPanelDelegate = (Utils.VoidDelegate)Delegate.Remove(SettingEvent.ExitCustomSaveSettingPanelDelegate, new Utils.VoidDelegate(OnExitCustomSettingPanel));
			StopAllCoroutines();
		}

		private void OnEnterCrouch()
		{
			btn_crouching2.SetActiveBetter(true);
			btn_paing2.SetActiveBetter(false);
		}

		private void OnEnterPa()
		{
			btn_crouching2.SetActiveBetter(false);
			btn_paing2.SetActiveBetter(true);
		}

		private void OnEnterStand()
		{
			btn_crouching2.SetActiveBetter(false);
			btn_paing2.SetActiveBetter(false);
		}

		private void OnChangePart()
		{
			UpdateGunImages();
		}

		private void OnPickItem()
		{
			UpdateGunImages();
		}

		private void OnCloseJiMiao()
		{
		}

		private void OnOpenJiMiao()
		{
		}

		protected override void onHide(string childView = null)
		{
			BattleEvent.onSelfDie = (Utils.Vector3Delegate)Delegate.Remove(BattleEvent.onSelfDie, new Utils.Vector3Delegate(OnSelfDie));
			BattleEvent.OnSelfKilledSelf = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnSelfKilledSelf, new Utils.VoidDelegate(OnSelfKilledSelf));
			SettingEvent.DriveDelegate = (Utils.IntDelegate)Delegate.Remove(SettingEvent.DriveDelegate, new Utils.IntDelegate(OnCarControllTypeChange));
			BattleEvent.OnCurLoadBulletNumChange = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnCurLoadBulletNumChange, new Utils.IntDelegate(OnCurLoadBulletNumChange));
			BattleEvent.OnNearCar = (BattleEvent.NearCar)Delegate.Remove(BattleEvent.OnNearCar, new BattleEvent.NearCar(OnNearCar));
			BattleEvent.OnFarCar = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnFarCar, new Utils.VoidDelegate(OnFarCar));
			BattleEvent.OnSelfHpChange = (BattleEvent.OnHpChangeDelegate)Delegate.Remove(BattleEvent.OnSelfHpChange, new BattleEvent.OnHpChangeDelegate(OnSelfHpChange));
			BattleEvent.OnEpChange = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnEpChange, new Utils.IntDelegate(OnEpChange));
			MailMgr ins = MailMgr.Ins;
			ins.RedDotCountChange = (Utils.IntDelegate)Delegate.Remove(ins.RedDotCountChange, new Utils.IntDelegate(MailRedDotCountChange));
			BattleEvent.OnChangeHandGun = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnChangeHandGun, new Utils.VoidDelegate(OnChangeHandGun));
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Remove(SPlayerDie.handler, new SPlayerDie.Handler(OnPlayerDie));
			SRebirth.handler = (SRebirth.Handler)Delegate.Remove(SRebirth.handler, new SRebirth.Handler(OnPlayerReBirth));
			BattleEvent.OnOpenKaiJing = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnOpenKaiJing, new Utils.VoidDelegate(OnOpenKaiJing));
			BattleEvent.OnCloseKaiJing = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCloseKaiJing, new Utils.VoidDelegate(OnCloseKaiJing));
			BattleEvent.OnOpenJiMiao = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnOpenJiMiao, new Utils.VoidDelegate(OnOpenJiMiao));
			BattleEvent.OnCloseJiMiao = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCloseJiMiao, new Utils.VoidDelegate(OnCloseJiMiao));
			BattlePackEvent.PickItemDelegate = (Utils.VoidDelegate)Delegate.Remove(BattlePackEvent.PickItemDelegate, new Utils.VoidDelegate(OnPickItem));
			BattleEvent.OnChangePart = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnChangePart, new Utils.VoidDelegate(OnChangePart));
			BattleEvent.OnHited = (Utils.Vector3Delegate)Delegate.Remove(BattleEvent.OnHited, new Utils.Vector3Delegate(OnHited));
			BattleEvent.OnHitOtherPlayer = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnHitOtherPlayer, new Utils.VoidDelegate(OnHitOtherPlayer));
			SKillPlayer.handler = (SKillPlayer.Handler)Delegate.Remove(SKillPlayer.handler, new SKillPlayer.Handler(OnSKillPlayer));
			SOwnDiePlayer.handler = (SOwnDiePlayer.Handler)Delegate.Remove(SOwnDiePlayer.handler, new SOwnDiePlayer.Handler(OnSOwnDiePlayer));
			SettingEvent.OperationDelegate = (Utils.IntDelegate)Delegate.Remove(SettingEvent.OperationDelegate, new Utils.IntDelegate(OnOperationModelChange));
			BattlePackEvent.PickEquip = (Utils.Int2Delegate)Delegate.Remove(BattlePackEvent.PickEquip, new Utils.Int2Delegate(OnPickEquip));
			BattlePackEvent.DropEquip = (Utils.Int2Delegate)Delegate.Remove(BattlePackEvent.DropEquip, new Utils.Int2Delegate(OnDropEquip));
			BattlePackEvent.EquipHpDelegate = (Utils.Int2Delegate)Delegate.Remove(BattlePackEvent.EquipHpDelegate, new Utils.Int2Delegate(OnEquipHpChange));
			BattleEvent.OnEnterCrouch = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnEnterCrouch, new Utils.VoidDelegate(OnEnterCrouch));
			BattleEvent.OnEnterPa = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnEnterPa, new Utils.VoidDelegate(OnEnterPa));
			BattleEvent.OnEnterStand = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnEnterStand, new Utils.VoidDelegate(OnEnterStand));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			TaskEvent.RefreshTaskRed = (Utils.VoidDelegate)Delegate.Remove(TaskEvent.RefreshTaskRed, new Utils.VoidDelegate(RefreshLadderTaskRed));
			LadderEvent.RefreshLadderRed = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLadderRed, new Utils.VoidDelegate(RefreshLadderTaskRed));
			BattleEvent.OnGetOutVehicle = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnGetOutVehicle, new Utils.VoidDelegate(OnGetoutVehicle));
			BattlePackEvent.DropItemDelegate = (Utils.Int2Delegate)Delegate.Remove(BattlePackEvent.DropItemDelegate, new Utils.Int2Delegate(OnDropItem));
			EventHandlers.OnBuildModeChanged -= OnBuildModeChanged;
			SPublicMsg.handler = (SPublicMsg.Handler)Delegate.Remove(SPublicMsg.handler, new SPublicMsg.Handler(SPublicMsgHandle));
			StopAllCoroutines();
			DetachRedDotEvent();
			OnHideMap();
			BattleEvent.OnTrusteeshipDownFireAction -= OnDownFire;
		}

		public void InitEquipInfo()
		{
		}

		private void OnEquipHpChange(int id, int hp)
		{
			OnPickEquip(id, hp);
		}

		private void OnDropEquip(int id, int hp)
		{
			ItemCfg itemCfg = ItemCfg.Get(id);
			if (itemCfg != null && itemCfg.type != 33 && itemCfg.type != 22)
			{
			}
		}

		private void OnPickEquip(int id, int hp)
		{
		}

		private void UpdateEquipInfoIcon(int id, int hp, GHelmetInfo gHelmetInfo, GBulletCloth gBulletClothInfo)
		{
			ItemCfg itemCfg = ItemCfg.Get(id);
			if (itemCfg.type == 33)
			{
				gHelmetInfo.gameObject.SetActiveBetter(true);
				View.SetItemSprite(gHelmetInfo.gameObject, itemCfg.extrasstring[0]);
				SetEquipInfoImageColor(gHelmetInfo.m_helmetInfoHp, hp);
				if (itemCfg.itemLevel == 1)
				{
					gHelmetInfo.m_helmetlv1.SetActiveBetter(true);
					gHelmetInfo.m_helmetlv2.SetActiveBetter(false);
					gHelmetInfo.m_helmetlv3.SetActiveBetter(false);
				}
				else if (itemCfg.itemLevel == 2)
				{
					gHelmetInfo.m_helmetlv1.SetActiveBetter(true);
					gHelmetInfo.m_helmetlv2.SetActiveBetter(true);
					gHelmetInfo.m_helmetlv3.SetActiveBetter(false);
				}
				else
				{
					gHelmetInfo.m_helmetlv1.SetActiveBetter(true);
					gHelmetInfo.m_helmetlv2.SetActiveBetter(true);
					gHelmetInfo.m_helmetlv3.SetActiveBetter(true);
				}
			}
			else if (itemCfg.type == 22)
			{
				gBulletClothInfo.gameObject.SetActiveBetter(true);
				View.SetItemSprite(gBulletClothInfo.gameObject, itemCfg.extrasstring[0]);
				SetEquipInfoImageColor(gBulletClothInfo.m_bulletclothhp, hp);
				if (itemCfg.itemLevel == 1)
				{
					gBulletClothInfo.m_bulletclothlv1.SetActiveBetter(true);
					gBulletClothInfo.m_bulletclothlv2.SetActiveBetter(false);
					gBulletClothInfo.m_bulletclothlv3.SetActiveBetter(false);
				}
				else if (itemCfg.itemLevel == 2)
				{
					gBulletClothInfo.m_bulletclothlv1.SetActiveBetter(true);
					gBulletClothInfo.m_bulletclothlv2.SetActiveBetter(true);
					gBulletClothInfo.m_bulletclothlv3.SetActiveBetter(false);
				}
				else
				{
					gBulletClothInfo.m_bulletclothlv1.SetActiveBetter(true);
					gBulletClothInfo.m_bulletclothlv2.SetActiveBetter(true);
					gBulletClothInfo.m_bulletclothlv3.SetActiveBetter(true);
				}
			}
		}

		private void SetEquipInfoImageColor(GameObject go, int hp)
		{
			Image component = go.GetComponent<Image>();
			if (component != null)
			{
				component.fillAmount = (float)hp / 10000f;
			}
			if (hp > 9000)
			{
				go.SetActiveBetter(false);
				View.SetImageColor(go, Color.white);
			}
			else if (hp > 5000)
			{
				go.SetActiveBetter(true);
				View.SetImageColor(go, m_equipCheng);
			}
			else
			{
				go.SetActiveBetter(true);
				View.SetImageColor(go, m_equipRed);
			}
		}

		private void OnOperationModelChange(int model)
		{
			UpdateOperationModel(model);
		}

		private void UpdateOperationModel(int model)
		{
			UpdateGunImages();
		}

		private void OnSKillPlayer(SKillPlayer msg)
		{
			if (m_killItemQueue.Count <= 0)
			{
				Utils.StopConroutine(m_DequeueKillItemCoroutine);
				m_DequeueKillItemCoroutine = Utils.StartConroutine(DequeueKillItem());
			}
			m_killItemQueue.Enqueue(msg);
			if (m_killItemQueue.Count > 3)
			{
				m_killItemQueue.Dequeue();
			}
			FillKillPlayerOnlyOnce(msg);
			UpdateKillItem();
		}

		private IEnumerator DequeueKillItem()
		{
			while (true)
			{
				yield return new WaitForSeconds(m_KillItemShowTime);
				if (m_killItemQueue.Count > 0)
				{
					m_killItemQueue.Dequeue();
				}
			}
		}

		private void FillKillPlayer(SKillPlayer msg, GKillItem m_kill)
		{
			View.SetLabelColor(m_kill.txt_killerNameText, Color.white);
			View.SetLabelColor(m_kill.txt_killTxtText, Color.white);
			View.SetLabelColor(m_kill.txt_killedNameText, Color.white);
			if (msg.killer.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				View.SetLabelColor(m_kill.txt_killerNameText, killerBlueColor);
				View.SetLabelColor(m_kill.txt_killTxtText, killerBlueColor);
				View.SetLabelColor(m_kill.txt_killedNameText, killerBlueColor);
			}
			if (msg.die.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				OtherPlayerController playerById = Battle.Ins.GetPlayerById(msg.killer.roleId);
				if (playerById != null)
				{
					playerById.VipLevel = msg.killerVipLevel;
				}
				View.SetLabelColor(m_kill.txt_killerNameText, killedRedColor);
				View.SetLabelColor(m_kill.txt_killTxtText, killedRedColor);
				View.SetLabelColor(m_kill.txt_killedNameText, killedRedColor);
			}
			if (msg.msgType == 1)
			{
				if (msg.bodyPart == 9)
				{
					View.SetLabelText(m_kill.txt_killTxtText, Utils.GetString(218));
				}
				else
				{
					View.SetLabelText(m_kill.txt_killTxtText, Utils.GetString(219));
				}
			}
			else
			{
				View.SetLabelText(m_kill.txt_killTxtText, Utils.GetString(217));
			}
			m_kill.gameObject.SetActiveBetter(true);
			m_kill.m_killIcon.SetActiveBetter(true);
			View.SetLabelText(m_kill.txt_killerNameText, msg.killer.name);
			View.SetLabelText(m_kill.txt_killedNameText, msg.die.name);
			if (msg.hitType == 2 || msg.hitType == 11 || msg.hitType == 16 || msg.hitType == 13 || msg.hitType == 15)
			{
				ItemCfg itemCfg = ItemCfg.Get(msg.itemId);
				if (itemCfg != null)
				{
					View.SetItemSprite(m_kill.m_killIcon, itemCfg.extrasstring[2], true);
				}
			}
			else if (msg.hitType == 1)
			{
				VehicleCfg vehicleCfg = VehicleCfg.Get(msg.itemId);
				if (vehicleCfg != null)
				{
					View.SetItemSprite(m_kill.m_killIcon, vehicleCfg.icon);
				}
			}
			else if (msg.hitType == 9)
			{
				View.SetBattleGunSprite(m_kill.m_killIcon, "gun/quantou_xiao");
			}
			else
			{
				m_kill.m_killIcon.SetActiveBetter(false);
			}
		}

		private void UpdateKillItem()
		{
			if (m_killItemQueue.Count > 0)
			{
				Queue<SKillPlayer> queue = new Queue<SKillPlayer>(m_killItemQueue);
				for (int num = queue.Count - 1; num >= 0; num--)
				{
					GKillItem gKillItem = m_killslist[num];
					gKillItem.gameObject.SetActiveBetter(false);
					SKillPlayer msg = queue.Dequeue();
					FillKillPlayer(msg, gKillItem);
				}
			}
			for (int i = m_killItemQueue.Count; i < m_killslist.Count; i++)
			{
				m_killslist[i].gameObject.SetActiveBetter(false);
			}
		}

		private void FillKillPlayerOnlyOnce(SKillPlayer msg)
		{
			if (msg.killer.roleId == Singleton<RoleMgr>.Ins.info.roleId && ((m_Self != null) & (m_Self.ExtraAddHp > 0f)))
			{
				CAddHp cAddHp = new CAddHp();
				cAddHp.hp = (short)((float)ConstsBs.HpPlayer * m_Self.ExtraAddHp);
				Client2Gs.Ins.Send(cAddHp);
			}
			if (msg.die.roleId == Singleton<RoleMgr>.Ins.info.roleId && msg.msgType == 1)
			{
				Utils.TriggerEvent(BattleEvent.OnSelfKilled, msg.killer.roleId);
			}
			if (msg.msgType == 1)
			{
				if (msg.killer.roleId == Singleton<RoleMgr>.Ins.info.roleId)
				{
					m_killNumIcon.transform.localScale = new Vector3(5f, 5f, 5f);
					m_killNumIcon.transform.DOScale(2f, 0.5f);
					if (msg.killInRow == 1)
					{
						View.SetBattleGunSprite(m_killNumIcon, "gun/jisha_yisha");
					}
					else if (msg.killInRow == 2)
					{
						View.SetBattleGunSprite(m_killNumIcon, "gun/jisha_ersha");
					}
					else if (msg.killInRow == 3 || msg.killInRow == 4)
					{
						View.SetBattleGunSprite(m_killNumIcon, "gun/jisha_3sha");
					}
					else if (msg.killInRow > 4)
					{
						View.SetBattleGunSprite(m_killNumIcon, "gun/jisha_wusha");
					}
					if (msg.bodyPart == 9)
					{
						m_killTypeIcon.SetActiveBetter(true);
						View.SetBattleGunSprite(m_killTypeIcon, "gun/jisha_baotou");
						SingletonMono<AudioManager>.Ins.Play2D(109);
					}
					if (msg.hitType == 1)
					{
						m_killTypeIcon.SetActiveBetter(true);
						View.SetBattleGunSprite(m_killTypeIcon, "gun/jisha_kaiche");
					}
					if (msg.hitType == 2 && ItemCfg.Get(msg.itemId).type == 17)
					{
						m_killTypeIcon.SetActiveBetter(true);
						View.SetBattleGunSprite(m_killTypeIcon, "gun/jisha_jinzhan");
					}
					SingletonMono<AudioManager>.Ins.Play2D(359);
				}
			}
			else if (msg.killer.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				SingletonMono<AudioManager>.Ins.Play2D(360);
			}
		}

		private void HideAllKillItem()
		{
			foreach (GKillItem item in m_killslist)
			{
				item.gameObject.SetActiveBetter(false);
			}
		}

		private void OnSOwnDiePlayer(SOwnDiePlayer msg)
		{
		}

		private void OnSelfKilledSelf()
		{
		}

		private void OnOpenKaiJing()
		{
			_003COnOpenKaiJing_003Ec__AnonStoreyD _003COnOpenKaiJing_003Ec__AnonStoreyD = new _003COnOpenKaiJing_003Ec__AnonStoreyD();
			_003COnOpenKaiJing_003Ec__AnonStoreyD._0024this = this;
			m_jing.SetActiveBetter(true);
			m_zhunxingObj.SetActiveBetter(false);
			_003COnOpenKaiJing_003Ec__AnonStoreyD.aim = m_Self.CurGun.GetPartCfg(14);
			if (_003COnOpenKaiJing_003Ec__AnonStoreyD.aim == null)
			{
				_003COnOpenKaiJing_003Ec__AnonStoreyD.aim = ItemCfg.Get(53600);
			}
			if (_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id == 53602)
			{
				m_jingAim.SetActiveBetter(true);
				View.SetItemSprite(m_jingAim, m_Self.CurGun.WeaponCfg.extrasstring[0], true);
				m_jingAim.transform.SetLocalPositionY(m_Self.CurGun.WeaponCfg.extras[0]);
			}
			else if (_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id == 12031)
			{
				m_jingAim.SetActiveBetter(true);
				View.SetItemSprite(m_jingAim, m_Self.CurGun.WeaponCfg.extrasstring[3], true);
				m_jingAim.transform.SetLocalPositionY(m_Self.CurGun.WeaponCfg.extras[2]);
			}
			else if (_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id == 12040)
			{
				m_jingAim.SetActiveBetter(true);
				View.SetItemSprite(m_jingAim, m_Self.CurGun.WeaponCfg.extrasstring[1], true);
				m_jingAim.transform.SetLocalPositionY(m_Self.CurGun.WeaponCfg.extras[1]);
			}
			else if (_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id == 12041)
			{
				m_jingAim.SetActiveBetter(true);
				View.SetItemSprite(m_jingAim, m_Self.CurGun.WeaponCfg.extrasstring[4], true);
				m_jingAim.transform.SetLocalPositionY(m_Self.CurGun.WeaponCfg.extras[3]);
			}
			else
			{
				m_jingAim.SetActiveBetter(false);
			}
			if (m_JingDic.ContainsKey(_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id))
			{
				if (m_JingDic[_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id] != null)
				{
					m_JingDic[_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.id].SetActiveBetter(true);
				}
			}
			else
			{
				ResMgr.Ins.CreateFromAB("model/jing" + (int)_003COnOpenKaiJing_003Ec__AnonStoreyD.aim.extras[0] + ".ab", null, _003COnOpenKaiJing_003Ec__AnonStoreyD._003C_003Em__0);
			}
		}

		private void OnCloseKaiJing()
		{
			m_zhunxingObj.SetActiveBetter(true);
			m_jing.SetActiveBetter(false);
			Utils.HideAllChild(m_jing.transform);
		}

		private void OnPlayerDie(SPlayerDie msg)
		{
			if (msg.roleId == m_Self.RoleId)
			{
				m_ReduceHpImage.SetActiveBetter(false);
				m_xueDir.SetActiveBetter(false);
				m_notWatch.SetActiveBetter(false);
			}
		}

		private void OnClickKillOk(GameObject go)
		{
			if (!m_Self.IsDie)
			{
			}
		}

		private void OnPlayerReBirth(SRebirth msg)
		{
			if (msg.playerInfo.roleId == Battle.Ins.SelfInfo.roleId)
			{
				m_notWatch.SetActiveBetter(true);
				HideSaveSelfPanel();
			}
		}

		public void ShowSaveSelfPanel()
		{
			m_saveSelfPanel.SetActiveBetter(true);
			m_image_no_saveitem.SetActiveBetter(Singleton<BagMgr>.Ins.GetItemNum(8) == 0);
			m_saveBtnEffect.SetActiveBetter(Singleton<BagMgr>.Ins.GetItemNum(8) != 0);
		}

		public void HideSaveSelfPanel()
		{
			m_saveSelfPanel.SetActiveBetter(false);
		}

		private void OnClickHuanZuo(GameObject go)
		{
			if (m_Self != null && m_Self.NearCar != null)
			{
				m_Self.NearCar.SwitchSeat(m_Self.gameObject);
				if (m_Self.CurGun != null)
				{
					m_Self.CurGun.CloseJiMiao();
				}
			}
		}

		private void OnClickInCarDrive(GameObject go)
		{
			if (m_Self != null && m_Self.NearCar != null)
			{
				m_Self.NearCar.SwitchToDrive(m_Self.gameObject);
			}
		}

		private void OnClickUpCar(GameObject go)
		{
			if (m_Self != null && m_Self.NearCar != null && m_Self.CheckCommonDoCondition())
			{
				m_Self.NearCar.GetInVehicle(m_Self, false);
			}
		}

		private void OnClickDriveCar(GameObject go)
		{
			if (m_Self != null && m_Self.NearCar != null && m_Self.CheckCommonDoCondition())
			{
				m_Self.NearCar.GetInVehicle(m_Self, true);
			}
		}

		public void OnClickDownCar(GameObject go)
		{
			if (m_Self != null && m_Self.NearCar != null && m_Self.NearCar.GetOutVehicle(m_Self, true))
			{
				m_car.SetActiveBetter(false);
				m_OperationBtns.SetActiveBetter(true);
				m_JoystackTypeCar.gameObject.SetActiveBetter(false);
				m_JoystackType2.gameObject.SetActiveBetter(true);
			}
		}

		private void OnChangeHandGun()
		{
			for (int i = 0; i < m_zhunxing.Length; i++)
			{
				m_zhunxing[i].SetActiveBetter(false);
			}
			if (m_Self.CurGun != null)
			{
				m_GZhunxing = m_zhunxing[m_Self.CurGun.GunCfg.aimPointType].GetComponent<GZhunxing>();
				m_GZhunxing.gameObject.SetActiveBetter(true);
			}
		}

		private void OnSelfHpChange(int hp, int changedHp)
		{
			if ((float)hp / (float)m_Self.MaxHp < 0.3f)
			{
				m_reduceHp10.SetActiveBetter(true);
			}
			else
			{
				m_reduceHp10.SetActiveBetter(false);
			}
			if ((float)hp / (float)m_Self.MaxHp < 0.1f)
			{
				m_reduceHpFull.SetActiveBetter(true);
				UnityEngine.Object.Destroy(m_reduceHpFull.GetComponent<AutoHide>());
			}
			else if (changedHp < 0)
			{
				m_reduceHpFull.SetActiveBetter(true);
				m_reduceHp.SetActiveBetter(true);
				AutoHide component = m_reduceHpFull.GetComponent<AutoHide>();
				if ((bool)component)
				{
					component.Delay = 1.5f;
				}
				else
				{
					m_reduceHpFull.AddComponent<AutoHide>().Delay = 1.5f;
				}
			}
			else
			{
				m_reduceHpFull.SetActiveBetter(false);
			}
			if (changedHp > 0)
			{
			}
			if (hp == 0)
			{
				m_reduceHpFull.SetActiveBetter(false);
				m_reduceHp10.SetActiveBetter(false);
				m_addHp.SetActiveBetter(false);
			}
		}

		private void SetHpColor(Image image, float hpRate, bool isSecondHp)
		{
			if (isSecondHp)
			{
				image.color = hong;
			}
			else if (hpRate >= 0.8f)
			{
				image.color = Color.white;
			}
			else if (hpRate >= 0.4f)
			{
				image.color = cheng;
			}
			else
			{
				image.color = hong;
			}
		}

		private void SetSliderValueColor(Image image, float value, bool isSecondHp)
		{
			if (isSecondHp)
			{
				image.color = _color1;
			}
			else if (value >= 0.9f)
			{
				image.color = Color.white;
			}
			else if (value >= 0.4f)
			{
				image.color = _color49;
			}
			else if (value >= 0.1f)
			{
				image.color = _color1;
			}
		}

		private void SetHpSlider(GameObject slider, Image image, int hp, int hpMax, bool isSecondHp)
		{
			View.SetSlider(slider, (float)hp * 1f / (float)hpMax);
			SetHpColor(image, (float)hp * 1f / (float)hpMax, isSecondHp);
		}

		private void SetAimSlider(GameObject slider, Image image, float hp, int maxHp, bool isSencond = false)
		{
			image.fillAmount = hp / (float)maxHp;
			SetHpColor(image, hp / (float)maxHp, isSencond);
		}

		private void OnEpChange(int ep)
		{
			UpdateEpIcon(ep);
		}

		private void UpdateEpIcon(int ep)
		{
		}

		private void OnFarCar()
		{
			m_NearCar.SetActiveBetter(false);
		}

		private void OnNearCar(VehicleMonitor car)
		{
			m_NearCar.SetActiveBetter(true);
		}

		public void OnClickKaiJing(GameObject go)
		{
			if (m_Self.IsJiMiao)
			{
				if (m_Self.CurGun != null)
				{
					m_Self.CurGun.CloseJiMiao();
					SingletonMono<AudioManager>.Ins.Play2D(154);
				}
				return;
			}
			m_Self.CloseAutoRun();
			if (m_Self.CurGun != null)
			{
				m_Self.CurGun.OpenJiMiao();
				SingletonMono<AudioManager>.Ins.Play2D(155);
			}
		}

		private void OnClickHuanZidan(GameObject go)
		{
			Battle.Ins.HuanDan(Singleton<BagMgr>.Ins.ChangeBulletItemId(Singleton<BagMgr>.Ins.GetCurGunDate().InsId));
		}

		private void OnClickUp2(GameObject go)
		{
			if (m_Self.FSMUpBody.CurrentState.ID == StateID.Attack)
			{
				return;
			}
			if (m_Self.FSM.CurrentState.ID == StateID.Climb)
			{
				m_Self.FSM.SwitchState(StateID.Fall);
			}
			else if (m_Self.FSM.CurrentState.ID == StateID.Pa && m_Self.CheckCanPaToStand())
			{
				m_Self.FSM.SwitchState(StateID.PaUp, true);
				m_Self.FSMUpBody.SwitchState(StateID.NullStateID);
				SingletonMono<AudioManager>.Ins.Play2D(341);
			}
			else if (m_Self.FSM.CurrentState.ID == StateID.Crouch && m_Self.CheckCanCrouchUp())
			{
				m_Self.FSM.SwitchState(StateID.Stand);
				SingletonMono<AudioManager>.Ins.Play2D(435);
			}
			else
			{
				if (m_Self.FSM.CurrentState.ID != StateID.Stand)
				{
					return;
				}
				CrossState.CheckCrossHeight(m_Self);
				RaycastHit raycastHit = m_Self.CheckNearLowToTopTiZi();
				Collider collider = raycastHit.collider;
				RaycastHit raycastHit2 = m_Self.CheckNearTopToLowTiZi();
				Collider collider2 = raycastHit2.collider;
				if (CrossState.CanPlayHighCross || CrossState.CanPlayStandCross)
				{
					m_Self.FSM.SwitchState(StateID.Cross);
				}
				else if (collider != null && collider.CompareTag("Tizi") && !m_Self.InBuildState)
				{
					if (m_Self.CheckCommonDoCondition() && (m_Self.FSMUpBody.CurrentState.ID == StateID.NullStateID || m_Self.FSMUpBody.CurrentState.ID == StateID.Aim || m_Self.FSMUpBody.CurrentState.ID == StateID.HoldNearWeaponState))
					{
						m_Self.FSM.SwitchState(StateID.Climb, raycastHit, false);
					}
				}
				else if (collider2 != null && collider2.CompareTag("Tizi") && !m_Self.InBuildState)
				{
					if (m_Self.CheckCommonDoCondition() && (m_Self.FSMUpBody.CurrentState.ID == StateID.NullStateID || m_Self.FSMUpBody.CurrentState.ID == StateID.Aim || m_Self.FSMUpBody.CurrentState.ID == StateID.HoldNearWeaponState) && raycastHit2.collider.transform.position.y < m_Self.Pos.y)
					{
						m_Self.FSM.SwitchState(StateID.Climb, raycastHit2, true);
					}
				}
				else
				{
					m_Self.FSM.SwitchState(StateID.Jump);
				}
			}
		}

		private void OnNaGunFinish(Gun gun)
		{
			UpdateGunImages();
		}

		private void OnCurLoadBulletNumChange(int num)
		{
			if (m_Self.CurGun != null)
			{
				BagMgr.GunData gunDate = Singleton<BagMgr>.Ins.GetGunDate(m_Self.CurGun.InsId);
				GBuildBagItem component = m_bagItemslist[gunDate.Index].GetComponent<GBuildBagItem>();
				View.SetLabelText(component.txt_numText, num);
			}
			if (m_bulletsSelectObj.activeSelf && m_Self.CurGun != null)
			{
				BagMgr.GunData gunDate2 = Singleton<BagMgr>.Ins.GetGunDate(m_Self.CurGun.InsId);
				for (int i = 0; i < gunDate2.GunCfg.bulletIds.Count; i++)
				{
					GBulletSelectInfo gBulletSelectInfo = m_bulletsSelectlist[i];
					View.SetLabelText(gBulletSelectInfo.txt_numText, Singleton<BagMgr>.Ins.GetItemNum(gunDate2.GunCfg.bulletIds[i]));
					Debug.LogError(Singleton<BagMgr>.Ins.GetItemNum(gunDate2.GunCfg.bulletIds[i]) + "****");
				}
			}
		}

		protected override void onDestroy()
		{
			CancelInvoke();
		}

		private void Update()
		{
			UpdateAimPoint();
			UpdateJingPos();
			UpdateOperatesPanel();
			if (Singleton<RoleMgr>.Ins.IsGm)
			{
				View.SetLabelText(txt_PlayerPos, string.Concat(m_Self.Pos, "地：", m_Self.GroundName));
			}
			OnUpdateMap();
			MoveCamera();
			RefreshNameLabelPos();
		}

		private void MoveCamera()
		{
			if (!m_Self || TouchId < 0 || ScreenDownUpListener.ClickNum <= 0)
			{
				return;
			}
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.fingerId != TouchId)
				{
					continue;
				}
				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
				{
					m_LastTouchPos = touch.position;
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					Vector2 vector = touch.position - m_LastTouchPos;
					m_LastTouchPos = touch.position;
					if (m_IsFirstDrag++ < 2 && vector.sqrMagnitude > 1f && (double)Time.deltaTime < 0.05)
					{
						vector *= 0.1f;
					}
					Battle.Ins.MainCamera.DragCamera(vector.x, vector.y);
					if (!m_Self.IsDie)
					{
						Battle.Ins.AimHelp.TryLockTarget();
					}
				}
				break;
			}
		}

		private void KeyBoard()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				OnClickUp2(null);
			}
		}

		private void UpdateAimPoint()
		{
			Gun curGun = Battle.Ins.SelfPlayer.CurGun;
			if (curGun == null)
			{
				m_zhunxingPoint.SetActiveBetter(true);
				if ((bool)m_GZhunxing)
				{
					m_GZhunxing.gameObject.SetActiveBetter(false);
				}
				return;
			}
			m_zhunxingPoint.SetActiveBetter(false);
			if ((bool)m_GZhunxing)
			{
				m_GZhunxing.gameObject.SetActiveBetter(true);
				if (m_Self.IsJiMiaoHavejing)
				{
					m_GZhunxing.m_AimLeft.SetActiveBetter(false);
					m_GZhunxing.m_AimRight.SetActiveBetter(false);
					m_GZhunxing.m_AimUp.SetActiveBetter(false);
					m_GZhunxing.m_AimDown.SetActiveBetter(false);
					return;
				}
				m_GZhunxing.m_AimLeft.SetActiveBetter(true);
				m_GZhunxing.m_AimRight.SetActiveBetter(true);
				m_GZhunxing.m_AimUp.SetActiveBetter(true);
				m_GZhunxing.m_AimDown.SetActiveBetter(true);
				m_GZhunxing.m_AimLeft.transform.SetLocalPositionX(0f - curGun.AimRadius);
				m_GZhunxing.m_AimRight.transform.SetLocalPositionX(curGun.AimRadius);
				m_GZhunxing.m_AimUp.transform.SetLocalPositionY(curGun.AimRadius);
				m_GZhunxing.m_AimDown.transform.SetLocalPositionY(0f - curGun.AimRadius);
			}
		}

		private void OnClickLying(GameObject go)
		{
			if (m_Self.FSM.CurrentState.ID == StateID.Pa)
			{
				m_Self.FSM.SwitchState(StateID.PaUp);
			}
		}

		private void OnClickCrouch(GameObject go)
		{
			if (m_Self.FSM.CurrentState.ID == StateID.Crouch)
			{
				Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.CrouchDown, 0.5f);
			}
		}

		private void OnClickStand(GameObject go)
		{
			if (m_Self.FSM.CurrentState.ID == StateID.Stand)
			{
				SingletonMono<AudioManager>.Ins.Play2D(339);
				Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.Crouch);
			}
		}

		private IEnumerator ZeroOneSecondTick()
		{
			while (true)
			{
				UpdateAutoRunImage();
				UpdateJingBtn();
				UpdateFireBtn();
				UpdateBuildBtn();
				CheckShowSavePeopleBtn();
				UpdateQuickUseItemHightLignt();
				OnUpdate01Map();
				UpdateAimInfo();
				yield return m_zeroOneTime;
			}
		}

		private IEnumerator ZeroThreeSecondTick()
		{
			while (true)
			{
				UpdateCarInfo();
				yield return m_zeroThreeTime;
			}
		}

		private IEnumerator ZeroFiveSecondTick()
		{
			while (true)
			{
				UpdateJumpImageOrPatiziImage();
				if (m_Self.FSM.CurrentState.ID != StateID.Swim)
				{
					Battle.Ins.MyBattlePanel.HideSwimPanel();
					Battle.Ins.MyBattlePanel.HidePulmonaryPanel();
				}
				Battle.Ins.AutoHuanDan();
				yield return m_zeroFiveTime;
			}
		}

		private IEnumerator OneSecondTick()
		{
			while (true)
			{
				View.SetLabelText(txt_gameTimeText, Utils.GetClockTimeNoSecond((int)(SingletonMono<DayNightSystem>.Ins.GetGameTime() * 1000.0)));
				SingletonMono<DayNightSystem>.Ins.CheckQDHN();
				yield return m_OneSecondTime;
			}
		}

		private void UpdateJumpImageOrPatiziImage()
		{
			if (m_Self.InBuildState)
			{
				m_JumpImage.SetActiveBetter(true);
				m_PatiziImage.SetActiveBetter(false);
				return;
			}
			RaycastHit raycastHit = m_Self.CheckNearLowToTopTiZi();
			RaycastHit raycastHit2 = m_Self.CheckNearTopToLowTiZi();
			if ((bool)m_Self && (bool)raycastHit.collider && raycastHit.collider.CompareTag("Tizi"))
			{
				m_JumpImage.SetActiveBetter(false);
				m_PatiziImage.SetActiveBetter(true);
				if (m_Self.FSM.CurrentState.ID != StateID.Climb && m_Self.Input.y > 0.5f)
				{
					OnClickUp2(null);
				}
			}
			else if ((bool)m_Self && (bool)raycastHit2.collider && raycastHit2.collider.CompareTag("Tizi"))
			{
				if (raycastHit2.collider.transform.position.y < m_Self.Pos.y)
				{
					m_JumpImage.SetActiveBetter(false);
					m_PatiziImage.SetActiveBetter(true);
				}
			}
			else
			{
				m_JumpImage.SetActiveBetter(true);
				m_PatiziImage.SetActiveBetter(false);
			}
		}

		private void UpdateAutoRunImage()
		{
			if (m_Self.AutoRun)
			{
				m_autoRunCircle.SetActiveBetter(true);
				m_JoystackType2.m_YaoganCircle.SetActiveBetter(false);
			}
			else
			{
				m_autoRunCircle.SetActiveBetter(false);
				m_JoystackType2.m_YaoganCircle.SetActiveBetter(true);
			}
		}

		private void UpdateCarInfo()
		{
			if (m_JoystackTypeCar.isActiveAndEnabled)
			{
				m_JoystackTypeCar.gameObject.SetActiveBetter(!(m_Self == null) && m_Self.IsDriver);
			}
			if (!m_Self || !m_Self.NearCar || !m_Self.InCar)
			{
				return;
			}
			m_JoystackType2.gameObject.SetActiveBetter(false);
			View.SetLabelText(txt_car_speedText, (int)m_Self.NearCar.GetSpeed() + "KM/H");
			View.SetSlider(m_car_you_slider, m_Self.NearCar.GetFuel() / m_Self.NearCar.GetMaxFuel());
			float num = (float)m_Self.NearCar.GetHp() / (float)m_Self.NearCar.GetMaxHp();
			View.SetSlider(m_CarHpSlider, num);
			if (num >= 0.9f)
			{
				View.SetImageColor(m_CarHpSlider.fillRect.gameObject, Color.white);
			}
			else if (num >= 0.5f)
			{
				View.SetImageColor(m_CarHpSlider.fillRect.gameObject, m_equipCheng);
			}
			else
			{
				View.SetImageColor(m_CarHpSlider.fillRect.gameObject, m_equipRed);
			}
			m_ChangeType2.SetActiveBetter(false);
			m_car.SetActiveBetter(true);
			m_NearCar.SetActiveBetter(false);
			int vehicleType = m_Self.NearCar.GetVehicleType();
			if (m_Self.HaveGun && !m_Self.IsDriver && vehicleType != 3)
			{
				btn_tanshen.SetActiveBetter(true);
			}
			else
			{
				btn_tanshen.SetActiveBetter(false);
			}
			if (m_Self.IsDriver)
			{
				btn_InCarDrive.SetActiveBetter(false);
			}
			else
			{
				btn_InCarDrive.SetActiveBetter(true);
			}
			m_planeController.SetActiveBetter(vehicleType == 2 && m_Self.IsDriver);
			m_carController.SetActiveBetter(vehicleType == 0 || vehicleType == 3);
			GameObject[] array = null;
			Image[] array2 = null;
			switch (vehicleType)
			{
			case 0:
				SetCheTiImageColor(m_CarChetiImage, num);
				array = m_carSeats;
				array2 = carSeatImages;
				InCarShow();
				break;
			case 3:
				SetCheTiImageColor(m_Moto2ChetiImage, num);
				array = m_moto2Seats;
				array2 = moto2SeatImages;
				InCarShow();
				break;
			case 2:
				SetCheTiImageColor(m_PlaneChetiImage, num);
				array = m_planeSeats;
				array2 = planeSeatImages;
				m_JoystackTypeCar.gameObject.SetActiveBetter(false);
				break;
			case 1:
				SetCheTiImageColor(m_ShipChetiImage, num);
				m_JoystackTypeCar.gameObject.SetActiveBetter(m_Self.IsDriver);
				array = m_shipSeats;
				array2 = shipSeatImages;
				break;
			}
			m_carInfo.SetActiveBetter(vehicleType == 0);
			m_shipInfo.SetActiveBetter(vehicleType == 1);
			m_planeInfo.SetActiveBetter(vehicleType == 2);
			m_moto2.SetActiveBetter(vehicleType == 3);
			m_moto3.SetActiveBetter(false);
			long[] roleIds = m_Self.NearCar.GetRoleIds();
			for (int i = 0; i < roleIds.Length; i++)
			{
				long num2 = roleIds[i];
				array[i].SetActiveBetter(num2 > 0);
				if (num2 == Singleton<RoleMgr>.Ins.info.roleId)
				{
					array2[i].color = selfSeatColor;
				}
				else
				{
					array2[i].color = otherSeatColor;
				}
			}
		}

		private void InCarShow()
		{
			if (m_Self.IsDriver)
			{
				btn_addSpeed.SetActiveBetter(true);
				btn_horn.SetActiveBetter(true);
				bool flag = SettingMgr.DriveMode == 1;
				m_JoystackTypeCar.gameObject.SetActiveBetter(flag);
				m_carButtons.SetActiveBetter(!flag);
			}
			else
			{
				btn_horn.SetActiveBetter(false);
				btn_addSpeed.SetActiveBetter(false);
				m_JoystackTypeCar.gameObject.SetActiveBetter(false);
				m_carButtons.SetActiveBetter(false);
			}
		}

		private void SetCheTiImageColor(Image image, float sliderNum)
		{
			if (sliderNum >= 0.9f)
			{
				View.SetImageColor(image, Color.white);
			}
			else if (sliderNum >= 0.5f)
			{
				View.SetImageColor(image, m_equipCheng);
			}
			else
			{
				View.SetImageColor(image, m_equipRed);
			}
		}

		public void UpdateGunImages()
		{
			if (m_Self.IsKongshou)
			{
				View.SetItemSprite(m_fireRealIcon, "gun/quantou_sc");
			}
			else
			{
				View.SetItemSprite(m_fireRealIcon, "gun/fire");
			}
			UpdateChangeBulletBtn();
		}

		private void UpdateChangeBulletBtn()
		{
			if (m_Self.CurGun != null)
			{
				btn_changeBullet.SetActiveBetter(true);
				return;
			}
			btn_changeBullet.SetActiveBetter(false);
			m_bulletsSelectObj.SetActiveBetter(false);
		}

		private void UpdateGunFireType()
		{
			if (m_Self.CurGun != null)
			{
				m_ShootModelImage.SetActiveBetter(true);
				if (m_Self.CurGun.CurFireType == FireType.Danfa)
				{
					btn_Danfa2.SetActiveBetter(true);
					btn_Auto2.SetActiveBetter(false);
					btn_SanLianfa2.SetActiveBetter(false);
				}
				else if (m_Self.CurGun.CurFireType == FireType.Auto)
				{
					btn_Auto2.SetActiveBetter(true);
					btn_Danfa2.SetActiveBetter(false);
					btn_SanLianfa2.SetActiveBetter(false);
				}
				else if (m_Self.CurGun.CurFireType == FireType.LianFa)
				{
					btn_Auto2.SetActiveBetter(false);
					btn_Danfa2.SetActiveBetter(false);
					btn_SanLianfa2.SetActiveBetter(true);
				}
			}
			else
			{
				m_ShootModelImage.SetActiveBetter(false);
				btn_Auto2.SetActiveBetter(false);
				btn_Danfa2.SetActiveBetter(false);
				btn_SanLianfa2.SetActiveBetter(false);
			}
		}

		private void UpdateJingPos()
		{
			if (m_Self.CurGun == null)
			{
				m_jing.SetActiveBetter(false);
			}
			else if (m_jing.activeSelf)
			{
				(m_jing.transform as RectTransform).SetLocalPositionX(m_Self.CurGun.JingPos.x);
				(m_jing.transform as RectTransform).SetLocalPositionY(m_Self.CurGun.JingPos.y);
			}
		}

		private void RefreshNameLabelPos()
		{
			foreach (KeyValuePair<long, OtherPlayerController> item in Battle.Ins.TeamPlayerDic)
			{
				if (!item.Value.NameLabel)
				{
					item.Value.NameLabel = UnityEngine.Object.Instantiate(txt_TeamPlayerName).GetComponent<RectTransform>();
					item.Value.NameLabel.SetParent(base.transform);
					item.Value.NameLabel.localScale = Vector3.one;
					View.SetLabelText(item.Value.NameLabel.transform.Find("name").GetComponent<Text>(), item.Value.Name, false);
					item.Value.NameLabel.gameObject.SetActiveBetter(true);
					item.Value.NameLabel.transform.Find("teamIcon").gameObject.SetActive(false);
				}
				if (item.Value.RoleId == Singleton<RoleMgr>.Ins.info.roleId || item.Value.IsDie)
				{
					item.Value.NameLabel.gameObject.SetActiveBetter(false);
				}
				item.Value.NameLabel.position = Battle.Ins.UICamrea.ScreenToWorldPoint(item.Value.NameLabelPos);
			}
		}

		public void ShowSwimPanel()
		{
			m_swimPanel.SetActiveBetter(true);
			m_OperationBtns.SetActiveBetter(false);
			m_ChangeType2.SetActiveBetter(false);
		}

		public void HideSwimPanel()
		{
			m_OperationBtns.SetActiveBetter(true);
			m_swimPanel.SetActiveBetter(false);
			m_ChangeType2.SetActiveBetter(true);
		}

		private void CheckShowSavePeopleBtn()
		{
			if (m_Self.CanSavedPeople != null && !m_Self.IsDie && !m_Self.IsDownWaitSave)
			{
				if (m_Self.FSM.CurrentState.ID == StateID.Crouch || m_Self.FSM.CurrentState.ID == StateID.Stand || m_Self.FSM.CurrentState.ID == StateID.Pa)
				{
					btn_HelpOther.SetActiveBetter(true);
				}
			}
			else
			{
				btn_HelpOther.SetActiveBetter(false);
			}
		}

		public void OnlyShowSomething(GameObject parent)
		{
			Transform[] componentsInParent = parent.GetComponentsInParent<Transform>(true);
			Transform[] array = componentsInParent;
			foreach (Transform transform in array)
			{
				transform.gameObject.SetActiveBetter(true);
			}
			Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(true);
			Transform[] array2 = componentsInChildren;
			foreach (Transform transform2 in array2)
			{
				transform2.gameObject.SetActiveBetter(true);
			}
		}

		public void HideAllChild()
		{
			m_AllChildTrans = GetComponentsInChildren<Transform>();
			Transform[] allChildTrans = m_AllChildTrans;
			foreach (Transform transform in allChildTrans)
			{
				if (!(transform.name == "BattlePanel"))
				{
					transform.gameObject.SetActiveBetter(false);
				}
			}
		}

		public void ShowMap()
		{
			m_zuo_shang_jiao.SetActive(true);
		}

		private void SetMainChat()
		{
			for (int i = 0; i < m_chats.Length; i++)
			{
				m_chats[i].SetActiveBetter(i < _msgList.Count);
				if (i < _msgList.Count)
				{
					FillRoleChatCell(m_chats[i], i);
				}
			}
		}

		private void FillRoleChatCell(GameObject go, int index)
		{
			if (index < _msgList.Count)
			{
				MsgBean msgBean = _msgList[index];
				View.SetLabelText(go, Utils.GetString(100, msgBean.role.name, msgBean.text));
			}
		}

		public void ShowAllUi()
		{
			m_zuo_shang_jiao.SetActiveBetter(true);
			m_notWatch.SetActiveBetter(true);
			m_zhu.SetActiveBetter(true);
			m_killsObj.SetActiveBetter(true);
			m_zsj.SetActiveBetter(true);
			m_ysj.SetActiveBetter(true);
			m_zuo_shang_jiao.SetActiveBetter(true);
			btn_Gm.SetActive(true);
			btn_quicklook.SetActiveBetter(true);
			GuidePanel view = ViewMgr.Ins.GetView<GuidePanel>();
			if ((bool)view)
			{
				view.gameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			}
			ViewMgr.Ins.ShowTopView<TeamTaskPanel>();
			base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));
		}

		public void HideAllUi()
		{
			GuidePanel view = ViewMgr.Ins.GetView<GuidePanel>();
			if ((bool)view)
			{
				view.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
			}
			ViewMgr.Ins.HideView<TeamTaskPanel>();
			base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		}

		public void HideUi()
		{
			m_zsj.SetActiveBetter(false);
			m_ysj.SetActiveBetter(false);
			m_zuo_shang_jiao.SetActiveBetter(false);
			btn_Gm.SetActive(false);
			btn_quicklook.SetActiveBetter(false);
			ViewMgr.Ins.HideView<TeamTaskPanel>();
			GuidePanel view = ViewMgr.Ins.GetView<GuidePanel>();
			if ((bool)view)
			{
				view.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
			}
		}

		private void AttachNonCombatBtnEvent()
		{
			_ckbTrusteeship = ckb_trusteeship.GetComponent<Toggle>();
			_ckbTrusteeship.isOn = false;
			_ckbTrusteeship.onValueChanged.AddListener(_003CAttachNonCombatBtnEvent_003Em__26);
			ClickListener clickListener = ClickListener.Get(btn_friend, string.Empty);
			if (_003C_003Ef__am_0024cache12 == null)
			{
				_003C_003Ef__am_0024cache12 = _003CAttachNonCombatBtnEvent_003Em__27;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache12;
			ClickListener clickListener2 = ClickListener.Get(btn_email, string.Empty);
			if (_003C_003Ef__am_0024cache13 == null)
			{
				_003C_003Ef__am_0024cache13 = _003CAttachNonCombatBtnEvent_003Em__28;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache13;
			ClickListener clickListener3 = ClickListener.Get(btn_pack, string.Empty);
			if (_003C_003Ef__am_0024cache14 == null)
			{
				_003C_003Ef__am_0024cache14 = _003CAttachNonCombatBtnEvent_003Em__29;
			}
			clickListener3.onClick = _003C_003Ef__am_0024cache14;
			ClickListener clickListener4 = ClickListener.Get(btn_Gm, string.Empty);
			if (_003C_003Ef__am_0024cache15 == null)
			{
				_003C_003Ef__am_0024cache15 = _003CAttachNonCombatBtnEvent_003Em__2A;
			}
			clickListener4.onClick = _003C_003Ef__am_0024cache15;
			ClickListener clickListener5 = ClickListener.Get(btn_shop, string.Empty);
			if (_003C_003Ef__am_0024cache16 == null)
			{
				_003C_003Ef__am_0024cache16 = _003CAttachNonCombatBtnEvent_003Em__2B;
			}
			clickListener5.onClick = _003C_003Ef__am_0024cache16;
			ClickListener clickListener6 = ClickListener.Get(btn_welfare, string.Empty);
			if (_003C_003Ef__am_0024cache17 == null)
			{
				_003C_003Ef__am_0024cache17 = _003CAttachNonCombatBtnEvent_003Em__2C;
			}
			clickListener6.onClick = _003C_003Ef__am_0024cache17;
			ClickListener.Get(ckb_trusteeship, string.Empty).onClick = _003CAttachNonCombatBtnEvent_003Em__2D;
			ClickListener clickListener7 = ClickListener.Get(m_trusteeship, string.Empty);
			if (_003C_003Ef__am_0024cache18 == null)
			{
				_003C_003Ef__am_0024cache18 = _003CAttachNonCombatBtnEvent_003Em__2E;
			}
			clickListener7.onClick = _003C_003Ef__am_0024cache18;
		}

		private void AttachRedDotEvent()
		{
			_ckbTrusteeship.isOn = SingletonMono<TrusteeshipMgr>.Ins.IsInTrusteeship;
			m_trusteeship_circle.SetActiveBetter(_ckbTrusteeship.isOn);
			m_trusteeship.SetActiveBetter(SingletonMono<TrusteeshipMgr>.Ins.IsInTrusteeship);
			UpdateFriendRedDotAct();
			UpdateActivityRedDotAct();
			FriendEventSc.UpdateFriendRedDot = (Utils.VoidDelegate)Delegate.Combine(FriendEventSc.UpdateFriendRedDot, new Utils.VoidDelegate(UpdateFriendRedDotAct));
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Combine(ActivityEvent.UpdateActivityRedDot, new Action(UpdateActivityRedDotAct));
			TrusteeshipEvent.TrusteeshipOnOffDelegate = (Action<bool>)Delegate.Combine(TrusteeshipEvent.TrusteeshipOnOffDelegate, new Action<bool>(OnTrusteeshipOnOff));
		}

		private void DetachRedDotEvent()
		{
			FriendEventSc.UpdateFriendRedDot = (Utils.VoidDelegate)Delegate.Remove(FriendEventSc.UpdateFriendRedDot, new Utils.VoidDelegate(UpdateFriendRedDotAct));
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Remove(ActivityEvent.UpdateActivityRedDot, new Action(UpdateActivityRedDotAct));
			TrusteeshipEvent.TrusteeshipOnOffDelegate = (Action<bool>)Delegate.Remove(TrusteeshipEvent.TrusteeshipOnOffDelegate, new Action<bool>(OnTrusteeshipOnOff));
		}

		private void OnTrusteeshipOnOff(bool b)
		{
			_ckbTrusteeship.isOn = b;
			m_trusteeship.SetActiveBetter(b);
		}

		private void UpdateFriendRedDotAct()
		{
			m_friend_red_dot.SetActiveBetter(Singleton<FriendScMgr>.Ins.IsShowRedDot());
		}

		private void UpdateActivityRedDotAct()
		{
			m_welfare_red_dot.SetActiveBetter(Singleton<ActivityMgr>.Ins.ShowActivityRedDot());
		}

		public void OnInitMap()
		{
			_minesGameObjects.Add(m_main_tag);
			_treesGameObjects.Add(m_tree_tag);
			_xiangziGameObjects.Add(m_xiangzi_tag);
			_lajitongGameObjects.Add(m_lajitong_tag);
			m_main_tag.SetActiveBetter(false);
			m_plant_tag.SetActiveBetter(false);
			m_xiangzi_tag.SetActiveBetter(false);
			m_lajitong_tag.SetActiveBetter(false);
			_secondBattleGameObjects.Add(m_second_battle_tag);
			_toolBoxPosGameObjects.Add(m_tool_box);
			InitCompassAndMinimap();
		}

		public void OnShowMap()
		{
			OnShowCompassAndMinimap();
		}

		public void OnHideMap()
		{
			OnHideCompassAndMinimap();
		}

		public void OnUpdateMap()
		{
			ShowLine();
		}

		public void OnUpdate01Map()
		{
			UpdateCompassAndMinimap();
			if (_tag == 1)
			{
				UpdateMainsPos();
			}
			else if (_tag == 2)
			{
				UpdateTreesPos();
			}
			else if (_tag == 3)
			{
				UpdatePlantsBattlePos();
			}
			else if (_tag == 4)
			{
				UpdateLajitongPos();
				UpdateXiangziPos();
			}
			_tag++;
			if (_tag > 4)
			{
				_tag = 1;
			}
		}

		private GameObject CreateGunSoundIconInMap()
		{
			return CreateSoundIconInMap(SoundSource.GUN_SOUND);
		}

		private GameObject CreateAirDropSoundIconInMap()
		{
			return CreateSoundIconInMap(SoundSource.AIRDROP_SOUND);
		}

		private GameObject CreateSoundIconInMap(int soundType)
		{
			GameObject original = _mSound2IconInMap[soundType];
			return UnityEngine.Object.Instantiate(original);
		}

		private GameObject CreateFootSoundIconInMapDir()
		{
			return CreateSoundIconInMapDir(SoundSource.FOOT_SOUND);
		}

		private GameObject CreateVehicleSoundIconInMapDir()
		{
			return CreateSoundIconInMapDir(SoundSource.VEHICLE_SOUND);
		}

		private GameObject CreateGunSoundIconInMapDir()
		{
			return CreateSoundIconInMapDir(SoundSource.GUN_SOUND);
		}

		private GameObject CreateAirDropSoundIconInMapDir()
		{
			return CreateSoundIconInMapDir(SoundSource.AIRDROP_SOUND);
		}

		private GameObject CreateSilenceSoundIconInMapDir()
		{
			return CreateSoundIconInMapDir(SoundSource.GUN_SOUND_SILENCE);
		}

		private GameObject CreateSoundIconInMapDir(int soundType)
		{
			GameObject original = _mSound2IconInMapDir[soundType];
			return UnityEngine.Object.Instantiate(original);
		}

		private void RecycleSoundIcon(GameObject go, int soundType, bool isDir = false)
		{
			if (go != null)
			{
				go.SetActiveBetter(false);
				ObjectPool<GameObject> objectPool = ((!isDir) ? _mSoundType2IconInMapPool[soundType] : _mSoundSource2IconInMapDirPool[soundType]);
				objectPool.Recycle(go);
			}
		}

		private void DestroySoundIcon(GameObject go)
		{
			UnityEngine.Object.Destroy(go);
		}

		private GameObject GetSoundIconFromPool(int soundType, Transform parent, bool isDir = false)
		{
			GameObject gameObject = ((!isDir) ? _mSoundType2IconInMapPool[soundType].Get() : _mSoundSource2IconInMapDirPool[soundType].Get());
			if (gameObject == null)
			{
				Debug.LogError("SoundType=" + soundType + " isDir=" + isDir);
			}
			gameObject.transform.SetParent(parent);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		private SoundSource CreateSoundSource(GameObject go, int soundType, int soundId, bool isDir)
		{
			SoundSource soundSource = _mSoundSourcePool.Get();
			soundSource._mGameObject = go;
			soundSource._mTransform = go.transform;
			soundSource._mTransformPos = go.transform.position;
			soundSource._mSoundType = soundType;
			soundSource.IsDir = isDir;
			SoundCfg soundCfg = SoundCfg.Get(soundId);
			if (soundCfg != null)
			{
				soundSource._mMaxDistance = soundCfg.radius;
			}
			else
			{
				soundSource._mMaxDistance = 20f;
			}
			return soundSource;
		}

		private void Recyecle(SoundSource soundSource)
		{
			GameObject value = null;
			if (_mSoundSource2IconInMap.TryGetValue(soundSource, out value))
			{
				RecycleSoundIcon(value, soundSource._mSoundType);
				_mSoundSource2IconInMap.Remove(soundSource);
			}
			if (_mSoundSource2IconInMapDir.TryGetValue(soundSource, out value))
			{
				RecycleSoundIcon(value, soundSource._mSoundType, true);
				_mSoundSource2IconInMapDir.Remove(soundSource);
			}
			_mSoundSourcePool.Recycle(soundSource);
		}

		private void OnShowCompassAndMinimap()
		{
			_mCurrentRoleId = Singleton<RoleMgr>.Ins.info.roleId;
			OnRefreshTeamateInfo();
			OnShowPos();
			_mSoundToTrack.Clear();
			float num = _mBattleImage.rectTransform.rect.height * _mZoomLevel;
			float num2 = num / 4096f * 204f;
			_mapRate = (num - num2) / num / MAP_SIZE;
			num -= num2;
			_mWidthRate = num / MAP_SIZE;
			_mHeightRate = num / MAP_SIZE;
			BattleEvent.roleGunSoundEvent = (BattleEvent.RoleGunSoundEvent)Delegate.Combine(BattleEvent.roleGunSoundEvent, new BattleEvent.RoleGunSoundEvent(OnTriggerGunSound));
			BattleEvent.vehicleSoundEvent = (BattleEvent.VehicleSoundEvent)Delegate.Combine(BattleEvent.vehicleSoundEvent, new BattleEvent.VehicleSoundEvent(OnTriggerVehicleSound));
			BattleEvent.airdropSoundEvent = (BattleEvent.AirdropSoundEvent)Delegate.Combine(BattleEvent.airdropSoundEvent, new BattleEvent.AirdropSoundEvent(OnTriggerAirDropSound));
			MapEvent.OnSTeamInfoDelegate = (Utils.VoidDelegate)Delegate.Combine(MapEvent.OnSTeamInfoDelegate, new Utils.VoidDelegate(OnSTeamInfoDelegate));
			BattleEvent.OnRefreshTeamateInfo = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnRefreshTeamateInfo, new Utils.VoidDelegate(OnRefreshTeamateInfo));
		}

		private void OnSTeamInfoDelegate()
		{
			OnRefreshTeamateInfo();
		}

		private void OnRefreshTeamateInfo()
		{
			_mRoleIds.Clear();
			if (Battle.Ins.SelfPlayer != null && !_mRoleIds.Contains(Singleton<RoleMgr>.Ins.info.roleId))
			{
				_mRoleIds.Add(Singleton<RoleMgr>.Ins.info.roleId);
			}
			foreach (TeamateInfo item in Singleton<TeamateMgr>.Ins.GetTeamateInfo())
			{
				if (!_mRoleIds.Contains(item.roleId))
				{
					_mRoleIds.Add(item.roleId);
				}
			}
			_mRoleIds.Sort(Singleton<TeamScMgr>.Ins.SortPlayersList);
			m_selfIndex = _mRoleIds.IndexOf(Singleton<RoleMgr>.Ins.info.roleId);
		}

		private void OnHideCompassAndMinimap()
		{
			_mRoleIds.Clear();
			_mSoundToTrack.Clear();
			_mSoundSourceToRemove.Clear();
			_mCurrentVehicleIds.Clear();
			BattleEvent.roleGunSoundEvent = (BattleEvent.RoleGunSoundEvent)Delegate.Remove(BattleEvent.roleGunSoundEvent, new BattleEvent.RoleGunSoundEvent(OnTriggerGunSound));
			BattleEvent.vehicleSoundEvent = (BattleEvent.VehicleSoundEvent)Delegate.Remove(BattleEvent.vehicleSoundEvent, new BattleEvent.VehicleSoundEvent(OnTriggerVehicleSound));
			BattleEvent.airdropSoundEvent = (BattleEvent.AirdropSoundEvent)Delegate.Remove(BattleEvent.airdropSoundEvent, new BattleEvent.AirdropSoundEvent(OnTriggerAirDropSound));
			MapEvent.OnSTeamInfoDelegate = (Utils.VoidDelegate)Delegate.Remove(MapEvent.OnSTeamInfoDelegate, new Utils.VoidDelegate(OnSTeamInfoDelegate));
			BattleEvent.OnRefreshTeamateInfo = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnRefreshTeamateInfo, new Utils.VoidDelegate(OnRefreshTeamateInfo));
		}

		private void InitCompassAndMinimap()
		{
			_image = m_map_line.GetComponent<Image>();
			_image.rectTransform.sizeDelta = new Vector2(5f, 305f);
			m_map_line.SetActiveBetter(false);
			_mSoundIconInMapParent = m_footSoundInMap.transform.parent;
			_mSound2IconInMap[SoundSource.GUN_SOUND] = m_gun_sound_pos_map;
			_mSound2IconInMap[SoundSource.AIRDROP_SOUND] = m_airdrop_pos_map;
			foreach (GameObject value in _mSound2IconInMap.Values)
			{
				value.SetActiveBetter(false);
			}
			_mSoundType2IconInMapPool[SoundSource.GUN_SOUND] = new ObjectPool<GameObject>(5, CreateGunSoundIconInMap, DestroySoundIcon);
			_mSoundType2IconInMapPool[SoundSource.AIRDROP_SOUND] = new ObjectPool<GameObject>(5, CreateAirDropSoundIconInMap, DestroySoundIcon);
			_mSound2IconInMapDir[SoundSource.FOOT_SOUND] = m_footSoundInMap;
			_mSound2IconInMapDir[SoundSource.GUN_SOUND] = m_gunSoundInMap;
			_mSound2IconInMapDir[SoundSource.VEHICLE_SOUND] = m_vehicleSoundInMap;
			_mSound2IconInMapDir[SoundSource.AIRDROP_SOUND] = m_airdrop_dir_map;
			_mSound2IconInMapDir[SoundSource.GUN_SOUND_SILENCE] = m_gun_silence_sound_pos;
			foreach (GameObject value2 in _mSound2IconInMapDir.Values)
			{
				value2.SetActiveBetter(false);
			}
			_mSoundSource2IconInMapDirPool[SoundSource.FOOT_SOUND] = new ObjectPool<GameObject>(8, CreateFootSoundIconInMapDir, DestroySoundIcon);
			_mSoundSource2IconInMapDirPool[SoundSource.GUN_SOUND] = new ObjectPool<GameObject>(8, CreateGunSoundIconInMapDir, DestroySoundIcon);
			_mSoundSource2IconInMapDirPool[SoundSource.VEHICLE_SOUND] = new ObjectPool<GameObject>(5, CreateVehicleSoundIconInMapDir, DestroySoundIcon);
			_mSoundSource2IconInMapDirPool[SoundSource.AIRDROP_SOUND] = new ObjectPool<GameObject>(5, CreateAirDropSoundIconInMapDir, DestroySoundIcon);
			_mSoundSource2IconInMapDirPool[SoundSource.GUN_SOUND_SILENCE] = new ObjectPool<GameObject>(5, CreateSilenceSoundIconInMapDir, DestroySoundIcon);
			_mBattleImage = m_lefttop.GetComponent<RawImage>();
			_mBattleImageRect.x = 0f;
			_mBattleImageRect.y = 0f;
			_mSharedVector2.Set(1f / _mZoomLevel, 1f / _mZoomLevel);
			_mBattleImageRect.size = _mSharedVector2;
			_mBattleImage.uvRect = _mBattleImageRect;
			_mMapImageWidth = _mBattleImage.rectTransform.rect.width;
			_mGPlayerMapMarksArray = new GPlayerMapMarks[m_team_members.Length];
			_mPosImageInMap = new Image[m_team_members.Length];
			_mDriveImageInMap = new Image[m_team_members.Length];
			_mJumpImageInMap = new Image[m_team_members.Length];
			for (int i = 0; i < m_team_members.Length; i++)
			{
				_mGPlayerMapMarksArray[i] = m_team_members[i].GetComponent<GPlayerMapMarks>();
				_mPosImageInMap[i] = _mGPlayerMapMarksArray[i].m_pos.GetComponent<Image>();
				_mDriveImageInMap[i] = _mGPlayerMapMarksArray[i].m_drive.GetComponent<Image>();
				_mGPlayerMapMarksArray[i].m_dead.SetActiveBetter(false);
			}
		}

		private void DrawZone(DuCircle circle, Zone zone)
		{
			Vector2 anchoredPosition = GetPosOnMap(zone.centerX, zone.centerY);
			circle.rectTransform.anchoredPosition = anchoredPosition;
			circle.Radius = zone.radius * _mBattleImage.rectTransform.rect.width * _mZoomLevel / MAP_SIZE;
		}

		private void ShowLine()
		{
			if (!((double)_mWidthRate <= 0.001))
			{
				int j = 0;
				j = ShowLine(j, true);
				for (j = ShowLine(j, false); j < _lineRowImages.Count; j++)
				{
					_lineRowImages[j].gameObject.SetActiveBetter(false);
				}
			}
		}

		private int ShowLine(int j, bool isCol)
		{
			for (int i = 0; i < 60; i++)
			{
				vector2 = ((!isCol) ? GetPosOnMap(0f, (float)(100 * i) - MAP_SIZE / 2f) : GetPosOnMap((float)(100 * i) - MAP_SIZE / 2f, 0f));
				if (isCol)
				{
					vector2.y = 0f;
				}
				else
				{
					vector2.x = 0f;
				}
				if (!(vector2.x < -150f) && !(vector2.x > 150f) && !(vector2.y < -150f) && !(vector2.y > 150f))
				{
					if (_lineRowImages.Count <= j)
					{
						Image image = UnityEngine.Object.Instantiate(_image);
						image.transform.SetParent(m_map_line_root.transform);
						image.rectTransform.localScale = Vector3.one;
						_lineRowImages.Add(image);
					}
					_lineRowImages[j].transform.localEulerAngles = ((!isCol) ? new Vector3(0f, 0f, 90f) : Vector3.zero);
					_lineRowImages[j].gameObject.SetActiveBetter(true);
					_lineRowImages[j].transform.localPosition = vector2;
					if (i % 10 == 0)
					{
						_lineRowImages[j].color = _outLineColor;
					}
					else
					{
						_lineRowImages[j].color = _inLineColor;
					}
					j++;
				}
			}
			return j;
		}

		private void UpdateRotationAndWorldCenter()
		{
			GameObject playerByRoleId = Battle.Ins.GetPlayerByRoleId(_mCurrentRoleId);
			if (playerByRoleId != null)
			{
				_mCurrentRotation = Battle.Ins.MainCamera.transform.localEulerAngles.y;
				_mCenterWorldPos.Set(playerByRoleId.transform.position.x, playerByRoleId.transform.position.y, playerByRoleId.transform.position.z);
			}
		}

		private Vec3 GetMark(long roleId)
		{
			if (roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				return Battle.Ins.GetMark();
			}
			foreach (TeamateInfo item in Singleton<TeamateMgr>.Ins.GetTeamateInfo())
			{
				if (item.roleId == roleId)
				{
					if (item.markList.Count > 0)
					{
						return (!item.isDead) ? item.markList[0] : null;
					}
					break;
				}
			}
			return null;
		}

		private float VectorAngle(Vector2 from, Vector2 to)
		{
			Vector3 vector = Vector3.Cross(from, to);
			float num = Vector2.Angle(from, to);
			return (!(vector.z > 0f)) ? (0f - num + 360f) : num;
		}

		private void UpdateMarks()
		{
			int i;
			for (i = 0; i < _mRoleIds.Count; i++)
			{
				long roleId = _mRoleIds[i];
				Vec3 mark = GetMark(roleId);
				GPlayerMapMarks gPlayerMapMarks = _mGPlayerMapMarksArray[i];
				if (mark != null)
				{
					_markPos.Set(mark.x, 0f, mark.z);
					_mSharedVector3 = GetPosOnMap(mark.x, mark.z);
					Vector3 vector = _markPos - _mCenterWorldPos;
					if (vector.x < -200f || vector.x > 200f || vector.z < -200f || vector.z > 200f)
					{
						gPlayerMapMarks.m_mark.SetActiveBetter(false);
						gPlayerMapMarks.m_mark_dir.gameObject.SetActiveBetter(true);
						_markDir = _mSharedVector3;
						Vector2 vector2 = GetPosOnMap(_mCenterWorldPos.x, _mCenterWorldPos.z);
						Vector2 vector3 = _markDir - vector2;
						gPlayerMapMarks.m_mark_dir.transform.up = vector3;
					}
					else
					{
						gPlayerMapMarks.m_mark.SetActiveBetter(true);
						gPlayerMapMarks.m_mark_dir.gameObject.SetActiveBetter(false);
						gPlayerMapMarks.m_mark.transform.localPosition = _mSharedVector3;
					}
				}
				else
				{
					gPlayerMapMarks.m_mark.SetActiveBetter(false);
					gPlayerMapMarks.m_mark_dir.gameObject.SetActiveBetter(false);
				}
				gPlayerMapMarks.gameObject.SetActiveBetter(true);
			}
			for (int j = i; j < _mRoleIds.Count; j++)
			{
				_mGPlayerMapMarksArray[j].gameObject.SetActiveBetter(false);
			}
		}

		private long GetVehicleId(long roleId)
		{
			if (roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				return m_Self.VehicleId;
			}
			TeamateInfo teamInfo = Singleton<TeamateMgr>.Ins.GetTeamInfo(roleId);
			if (teamInfo != null)
			{
				return teamInfo.vehicleId;
			}
			return 0L;
		}

		private float GetVehicleRotationY(long vehicleId)
		{
			VehicleMonitor vehicle = Battle.Ins.GetVehicle((int)vehicleId);
			if (vehicle != null)
			{
				return vehicle.gameObject.transform.localEulerAngles.y;
			}
			return 0f;
		}

		private void OnShowPos()
		{
			for (int i = 0; i < _mRoleIds.Count && i < 4; i++)
			{
				GPlayerMapMarks gPlayerMapMarks = _mGPlayerMapMarksArray[i];
				gPlayerMapMarks.gameObject.SetActiveBetter(true);
				long num = _mRoleIds[i];
				if (num == Singleton<RoleMgr>.Ins.info.roleId)
				{
					_mGPlayerMapMarksArray[i].transform.SetAsLastSibling();
				}
				View.SetLabelText(gPlayerMapMarks.txt_role_index, i + 1);
			}
		}

		private void UpdatePosOnMap()
		{
			int num = 0;
			long vehicleId = GetVehicleId(_mCurrentRoleId);
			while (num < _mRoleIds.Count)
			{
				long num2 = _mRoleIds[num];
				BasePlayerController player = Battle.Ins.GetPlayer(num2);
				GPlayerMapMarks gPlayerMapMarks = _mGPlayerMapMarksArray[num];
				gPlayerMapMarks.gameObject.SetActiveBetter(true);
				if (player != null)
				{
					bool flag = num2 == Singleton<RoleMgr>.Ins.info.roleId;
					bool flag2 = false;
					bool flag3 = false;
					bool isDie = player.IsDie;
					_mSharedVector3 = GetPosOnMap(player.transform.position.x, player.transform.position.z);
					if (flag)
					{
						flag2 = Battle.Ins.SelfPlayer.InStartPlane;
						flag3 = Battle.Ins.SelfPlayer.Skydiving || Battle.Ins.SelfPlayer.SanOpened;
					}
					else
					{
						TeamateInfo teamInfo = Singleton<TeamateMgr>.Ins.GetTeamInfo(num2);
						if (teamInfo.status < 0)
						{
							gPlayerMapMarks.m_pos.SetActiveBetter(false);
							gPlayerMapMarks.m_pos_light.SetActiveBetter(false);
							gPlayerMapMarks.m_drive.SetActiveBetter(false);
							gPlayerMapMarks.m_mark.SetActiveBetter(false);
							gPlayerMapMarks.txt_role_index.SetActiveBetter(false);
							num++;
							continue;
						}
						flag2 = teamInfo.status == 2;
						flag3 = teamInfo.status == 4 || teamInfo.status == 3;
						if (flag2 && Battle.Ins.StartPlane != null)
						{
							_mSharedVector3 = Battle.Ins.StartPlane.transform.position;
						}
					}
					gPlayerMapMarks.m_pos.transform.localPosition = _mSharedVector3;
					gPlayerMapMarks.txt_role_index.transform.localPosition = _mSharedVector3;
					gPlayerMapMarks.m_drive.transform.localPosition = _mSharedVector3;
					gPlayerMapMarks.m_pos_light.transform.localPosition = _mSharedVector3;
					long vehicleId2 = GetVehicleId(num2);
					if (vehicleId2 <= 0)
					{
						float num3 = ((num2 != _mCurrentRoleId) ? player.transform.eulerAngles.y : Battle.Ins.SelfPlayer.transform.eulerAngles.y);
						_mSharedVector3.Set(0f, 0f, 0f - num3);
					}
					else
					{
						_mSharedVector3.Set(0f, 0f, 0f - GetVehicleRotationY(vehicleId2));
					}
					gPlayerMapMarks.m_pos.transform.localEulerAngles = _mSharedVector3;
					gPlayerMapMarks.m_drive.transform.localEulerAngles = _mSharedVector3;
					gPlayerMapMarks.m_pos.SetActiveBetter(!isDie);
					if (flag)
					{
						float y = Battle.Ins.MainCamera.transform.eulerAngles.y;
						_mSharedVector3.Set(0f, 0f, 0f - y);
						gPlayerMapMarks.m_pos_light.transform.localEulerAngles = _mSharedVector3;
						gPlayerMapMarks.m_pos_light.SetActiveBetter(!isDie);
						if (!isDie)
						{
							gPlayerMapMarks.txt_role_index.SetActiveBetter(true);
							View.SetLabelText(gPlayerMapMarks.txt_role_indexText, num + 1);
						}
						else
						{
							gPlayerMapMarks.txt_role_index.SetActiveBetter(false);
						}
						gPlayerMapMarks.m_drive.SetActiveBetter(false);
					}
					else
					{
						gPlayerMapMarks.m_pos_light.SetActiveBetter(false);
						bool flag4 = flag2 || (vehicleId2 > 0 && vehicleId2 != vehicleId);
						gPlayerMapMarks.m_drive.SetActiveBetter(flag4 && !isDie);
						if (!flag3 && !flag4 && !isDie)
						{
							gPlayerMapMarks.txt_role_index.SetActiveBetter(true);
							View.SetLabelText(gPlayerMapMarks.txt_role_indexText, num + 1);
						}
						else
						{
							gPlayerMapMarks.txt_role_index.SetActiveBetter(false);
						}
					}
				}
				else
				{
					gPlayerMapMarks.m_pos.SetActiveBetter(false);
					gPlayerMapMarks.m_pos_light.SetActiveBetter(false);
					gPlayerMapMarks.m_drive.SetActiveBetter(false);
					gPlayerMapMarks.m_mark.SetActiveBetter(false);
					gPlayerMapMarks.txt_role_index.SetActiveBetter(false);
				}
				num++;
			}
			for (int i = num; i < m_team_members.Length; i++)
			{
				_mGPlayerMapMarksArray[i].gameObject.SetActiveBetter(false);
			}
		}

		private void UpdateTeamDieMap()
		{
			if (!Battle.Ins)
			{
				return;
			}
			for (int i = 0; i < _mRoleIds.Count; i++)
			{
				GPlayerMapMarks gPlayerMapMarks = _mGPlayerMapMarksArray[i];
				if (Singleton<TeamateMgr>.Ins.PlayerDieDic.ContainsKey(_mRoleIds[i]))
				{
					SetTeamDie(gPlayerMapMarks, Singleton<TeamateMgr>.Ins.PlayDiePos(_mRoleIds[i]), true);
				}
				else if (_mRoleIds[i] == Singleton<RoleMgr>.Ins.info.roleId && Battle.Ins.SelfPlayer.IsDie)
				{
					SetTeamDie(gPlayerMapMarks, Singleton<MapMgr>.Ins.SelfDiePos, true);
				}
				else
				{
					gPlayerMapMarks.m_dead.SetActiveBetter(false);
				}
			}
		}

		private void SetTeamDie(GPlayerMapMarks gPlayerMapMarks, Vector3 worldPos, bool isDead)
		{
			Vector2 vector = GetPosOnMap(worldPos.x, worldPos.z);
			gPlayerMapMarks.m_dead.transform.localPosition = vector;
			gPlayerMapMarks.m_dead.SetActiveBetter(isDead);
		}

		private float GetSoundIconScale(float maxDistance, float distance)
		{
			float value = _mMaxIconScale;
			if (Mathf.Abs(distance) > 0f)
			{
				value = maxDistance / distance;
			}
			return Mathf.Clamp(value, _mMinIconScale, _mMaxIconScale);
		}

		private void UpdateSoundIconInMap(SoundSource soundSource, float distance)
		{
			if (soundSource._mSoundType != SoundSource.VEHICLE_SOUND)
			{
				float soundIconScale = GetSoundIconScale(soundSource._mMaxDistance, distance);
				GameObject value;
				if (_mSoundSource2IconInMap.TryGetValue(soundSource, out value) && value != null)
				{
					value.SetActiveBetter(true);
					value.transform.localPosition = GetPosOnMap(_mTransformPos.x, _mTransformPos.z);
				}
			}
		}

		private void UpdateSoundIconInMapDir(SoundSource soundSource, float distance)
		{
			GameObject value;
			if (_mSoundSource2IconInMapDir.TryGetValue(soundSource, out value) && value != null)
			{
				value.SetActiveBetter(true);
				_soundIconInMapPos = GetPosOnMap(_mTransformPos.x, _mTransformPos.z);
				Vector2 vector = GetPosOnMap(_mCenterWorldPos.x, _mCenterWorldPos.z);
				Vector2 vector2 = _soundIconInMapPos - vector;
				value.transform.up = vector2;
			}
		}

		private void UpdateSoundIcons()
		{
			float time = Time.time;
			float num = 0f;
			foreach (SoundSource item in _mSoundToTrack)
			{
				if ((item._mSoundType != SoundSource.AIRDROP_SOUND && item._mGameObject == null) || item._mTimeToVanish < time)
				{
					_mSoundSourceToRemove.Add(item);
					continue;
				}
				if (item._mSoundType == SoundSource.AIRDROP_SOUND)
				{
					_mTransformPos = item._mTransformPos;
				}
				else
				{
					_mTransformPos = item._mTransform.position;
				}
				num = Vector3.Distance(_mTransformPos, _mCenterWorldPos);
				UpdateSoundIconInMap(item, num);
				UpdateSoundIconInMapDir(item, num);
			}
			foreach (SoundSource item2 in _mSoundSourceToRemove)
			{
				_mSoundToTrack.Remove(item2);
				Recyecle(item2);
			}
			_mSoundSourceToRemove.Clear();
		}

		private void UpdateMapCenter()
		{
			_mMapUVCenter.Set(Mathf.Clamp(_mCenterWorldPos.x * _mapRate + 0.5f, 0f, 1f), Mathf.Clamp(_mCenterWorldPos.z * _mapRate + 0.5f, 0f, 1f));
			_mBattleImageRect.center = _mMapUVCenter;
			_mBattleImage.uvRect = _mBattleImageRect;
		}

		private Vector3 GetPosOnMap(float worldPosX, float worldPosZ)
		{
			float newX = (worldPosX - (_mBattleImage.uvRect.center.x - 0.5f) / _mapRate) * _mWidthRate;
			float newY = (worldPosZ - (_mBattleImage.uvRect.center.y - 0.5f) / _mapRate) * _mHeightRate;
			float newZ = 0f;
			_posOnMapV3.Set(newX, newY, newZ);
			return _posOnMapV3;
		}

		private void LastCarPos()
		{
			if (Singleton<MapMgr>.Ins.IsShowLastCarPos)
			{
				m_last_drive.SetActiveBetter(true);
				_lastCar = GetPosOnMap(Singleton<MapMgr>.Ins.LastCarPos.x, Singleton<MapMgr>.Ins.LastCarPos.z);
				m_last_drive.transform.localPosition = _lastCar;
			}
			else
			{
				m_last_drive.SetActiveBetter(false);
			}
		}

		private void DropItemDiePos()
		{
			if (Singleton<MapMgr>.Ins.TryGetDropedItemsVector3(out _dropItemDiePos))
			{
				_dropItemDiePosLocal = GetPosOnMap(_dropItemDiePos.x, _dropItemDiePos.z);
				m_die_dropitem_pos.SetActiveBetter(true);
				m_die_dropitem_pos.transform.localPosition = _dropItemDiePosLocal;
			}
			else
			{
				m_die_dropitem_pos.SetActiveBetter(false);
			}
		}

		private void UpdateCoordinate()
		{
			if ((bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer)
			{
				int num = 3000;
				View.SetLabelText(txt_coordinateText, Utils.GetString(263, (int)Battle.Ins.SelfPlayer.Pos.x + num, (int)Battle.Ins.SelfPlayer.Pos.z + num));
			}
		}

		private void UpdateTooBoxPos()
		{
			int i = 0;
			if (MapMgr.ShowToolBoxInLittleMap)
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
					_toolBoxPosVector3 = GetPosOnMap(item.Value.posX, item.Value.posZ);
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
			if (MapMgr.SecondBattleLittleMap)
			{
				foreach (Vector3 secondPo in Singleton<MapMgr>.Ins.SecondPos)
				{
					if (i >= _secondBattleGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_second_battle_tag);
						gameObject.transform.SetParent(m_second_battle_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_secondBattleGameObjects.Add(gameObject);
					}
					_secondBattleGameObjects[i].SetActiveBetter(true);
					_secondPos = GetPosOnMap(secondPo.x, secondPo.z);
					_secondBattleGameObjects[i].transform.localPosition = _secondPos;
					i++;
				}
			}
			for (; i < _secondBattleGameObjects.Count; i++)
			{
				_secondBattleGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateMainsPos()
		{
			foreach (KeyValuePair<long, Mine> item in Battle.Ins.MineDic)
			{
				Mine value = item.Value;
				if (!value.isActiveAndEnabled)
				{
					continue;
				}
				Vector3 vector = value.transform.position - _mCenterWorldPos;
				if (!(vector.x < (float)(-_smallIconDistance)) && !(vector.x > (float)_smallIconDistance) && !(vector.z < (float)(-_smallIconDistance)) && !(vector.z > (float)_smallIconDistance) && Singleton<MapMgr>.Ins.Mains.Contains(value.ShowMineInfo.mineTypeId))
				{
					if (!_minesGameObjectsDic.ContainsKey(value.ShowMineInfo.mineTypeId))
					{
						_minesGameObjectsDic.Add(value.ShowMineInfo.mineTypeId, new List<GameObject>());
					}
					List<GameObject> list = _minesGameObjectsDic[value.ShowMineInfo.mineTypeId];
					if (!_minesGameObjectsIndexDic.ContainsKey(value.ShowMineInfo.mineTypeId))
					{
						_minesGameObjectsIndexDic.Add(value.ShowMineInfo.mineTypeId, 0);
					}
					if (_minesGameObjectsIndexDic[value.ShowMineInfo.mineTypeId] >= list.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_main_tag);
						gameObject.transform.SetParent(m_main_tags_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						list.Add(gameObject);
					}
					list[_minesGameObjectsIndexDic[value.ShowMineInfo.mineTypeId]].SetActiveBetter(true);
					_mineVector3 = GetPosOnMap(value.transform.position.x, value.transform.position.z);
					View.SetItemSprite(list[_minesGameObjectsIndexDic[value.ShowMineInfo.mineTypeId]], Singleton<MapMgr>.Ins.GetMineIconName(value.ShowMineInfo.mineTypeId));
					list[_minesGameObjectsIndexDic[value.ShowMineInfo.mineTypeId]].transform.localPosition = _mineVector3;
					_minesGameObjectsIndexDic[value.ShowMineInfo.mineTypeId]++;
				}
			}
			foreach (KeyValuePair<int, List<GameObject>> item2 in _minesGameObjectsDic)
			{
				for (int i = _minesGameObjectsIndexDic[item2.Key]; i < item2.Value.Count; i++)
				{
					item2.Value[i].SetActiveBetter(false);
				}
				_minesGameObjectsIndexDic[item2.Key] = 0;
			}
		}

		private void UpdatePlantsBattlePos()
		{
			foreach (KeyValuePair<long, PlantInfo> item in Battle.Ins.PlantDic)
			{
				PlantInfo value = item.Value;
				if (!value.isActiveAndEnabled)
				{
					continue;
				}
				Vector3 vector = value.transform.position - _mCenterWorldPos;
				if (!(vector.x < (float)(-_smallIconDistance)) && !(vector.x > (float)_smallIconDistance) && !(vector.z < (float)(-_smallIconDistance)) && !(vector.z > (float)_smallIconDistance) && Singleton<MapMgr>.Ins.Plant.Contains(value.MySPlantInfo.plantId))
				{
					if (!_plantsGameObjectsDic.ContainsKey(value.MySPlantInfo.plantId))
					{
						_plantsGameObjectsDic.Add(value.MySPlantInfo.plantId, new List<GameObject>());
					}
					List<GameObject> list = _plantsGameObjectsDic[value.MySPlantInfo.plantId];
					if (!_plantsGameObjectsIndexDic.ContainsKey(value.MySPlantInfo.plantId))
					{
						_plantsGameObjectsIndexDic.Add(value.MySPlantInfo.plantId, 0);
					}
					if (_plantsGameObjectsIndexDic[value.MySPlantInfo.plantId] >= list.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_plant_tag);
						gameObject.transform.SetParent(m_plant_tags_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						list.Add(gameObject);
					}
					list[_plantsGameObjectsIndexDic[value.MySPlantInfo.plantId]].SetActiveBetter(true);
					_mineVector3 = GetPosOnMap(value.transform.position.x, value.transform.position.z);
					View.SetItemSprite(list[_plantsGameObjectsIndexDic[value.MySPlantInfo.plantId]], Singleton<MapMgr>.Ins.GetPlantIconName(value.MySPlantInfo.plantId));
					list[_plantsGameObjectsIndexDic[value.MySPlantInfo.plantId]].transform.localPosition = _mineVector3;
					_plantsGameObjectsIndexDic[value.MySPlantInfo.plantId]++;
				}
			}
			foreach (KeyValuePair<int, List<GameObject>> item2 in _plantsGameObjectsDic)
			{
				for (int i = _plantsGameObjectsIndexDic[item2.Key]; i < item2.Value.Count; i++)
				{
					item2.Value[i].SetActiveBetter(false);
				}
				_plantsGameObjectsIndexDic[item2.Key] = 0;
			}
		}

		private void UpdateTreesPos()
		{
			int i = 0;
			foreach (TreeInfo value in Battle.Ins.TreesDic.Values)
			{
				if (!value.isActiveAndEnabled)
				{
					continue;
				}
				Vector3 vector = value.transform.position - _mCenterWorldPos;
				if (vector.x < (float)(-_smallIconDistance) || vector.x > (float)_smallIconDistance || vector.z < (float)(-_smallIconDistance) || vector.z > (float)_smallIconDistance)
				{
					continue;
				}
				if (value.treeType == 0)
				{
					value.treeType = 1;
				}
				if (Singleton<MapMgr>.Ins.Trees.Contains(value.treeType))
				{
					if (i >= _treesGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_tree_tag);
						gameObject.transform.SetParent(m_tree_tags_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_treesGameObjects.Add(gameObject);
					}
					_treesGameObjects[i].SetActiveBetter(true);
					_mineVector3 = GetPosOnMap(value.transform.position.x, value.transform.position.z);
					View.SetItemSprite(_treesGameObjects[i], Singleton<MapMgr>.Ins.GetTreeIconName(value.treeType));
					_treesGameObjects[i].transform.localPosition = _mineVector3;
					i++;
				}
			}
			for (; i < _treesGameObjects.Count; i++)
			{
				_treesGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateLajitongPos()
		{
			int i = 0;
			foreach (STrashCan value in Battle.Ins.LajitongDic.Values)
			{
				tep.x = value.pos.x;
				tep.y = value.pos.y;
				tep.z = value.pos.z;
				Vector3 vector = tep - _mCenterWorldPos;
				if (!(vector.x < (float)(-_smallIconDistance)) && !(vector.x > (float)_smallIconDistance) && !(vector.z < (float)(-_smallIconDistance)) && !(vector.z > (float)_smallIconDistance) && Singleton<MapMgr>.Ins.Lajitong.Contains(value.type))
				{
					if (i >= _lajitongGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_lajitong_tag);
						gameObject.transform.SetParent(m_lajitong_tags_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_lajitongGameObjects.Add(gameObject);
					}
					_lajitongGameObjects[i].SetActiveBetter(true);
					_mineVector3 = GetPosOnMap(tep.x, tep.z);
					View.SetItemSprite(_lajitongGameObjects[i], Singleton<MapMgr>.Ins.GetLajitongIconName(value.type - 20000));
					_lajitongGameObjects[i].transform.localPosition = _mineVector3;
					i++;
				}
			}
			for (; i < _lajitongGameObjects.Count; i++)
			{
				_lajitongGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateXiangziPos()
		{
			int i = 0;
			foreach (STrashCan value in Battle.Ins.XiangziDic.Values)
			{
				tep.x = value.pos.x;
				tep.y = value.pos.y;
				tep.z = value.pos.z;
				Vector3 vector = tep - _mCenterWorldPos;
				if (!(vector.x < (float)(-_smallIconDistance)) && !(vector.x > (float)_smallIconDistance) && !(vector.z < (float)(-_smallIconDistance)) && !(vector.z > (float)_smallIconDistance) && Singleton<MapMgr>.Ins.Xiangzi.Contains(value.type))
				{
					if (i >= _xiangziGameObjects.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_xiangzi_tag);
						gameObject.transform.SetParent(m_xiangzi_tags_root.transform, true);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						_xiangziGameObjects.Add(gameObject);
					}
					_xiangziGameObjects[i].SetActiveBetter(true);
					_mineVector3 = GetPosOnMap(tep.x, tep.z);
					View.SetItemSprite(_xiangziGameObjects[i], Singleton<MapMgr>.Ins.GetXiangziIconName(value.type - 20002));
					_xiangziGameObjects[i].transform.localPosition = _mineVector3;
					i++;
				}
			}
			for (; i < _xiangziGameObjects.Count; i++)
			{
				_xiangziGameObjects[i].SetActiveBetter(false);
			}
		}

		private void UpdateCompassAndMinimap()
		{
			UpdateRotationAndWorldCenter();
			UpdateMapCenter();
			UpdateMarks();
			UpdatePosOnMap();
			UpdateTeamDieMap();
			UpdateSoundIcons();
			LastCarPos();
			DropItemDiePos();
			UpdateTooBoxPos();
			UpdateCoordinate();
			UpdateSecondBattlePos();
		}

		private SoundSource FindSameSoundSource(GameObject go, int soundType, bool isDir)
		{
			foreach (SoundSource item in _mSoundToTrack)
			{
				if (item._mGameObject == go && item._mSoundType == soundType && isDir == item.IsDir)
				{
					return item;
				}
			}
			return null;
		}

		private void OnTriggerFootSound(GameObject go, long roleId, int soundId)
		{
			if (!Battle.Ins.SelfPlayer.Landed && !_mRoleIds.Contains(roleId))
			{
				OnTriggerSound(go, SoundSource.FOOT_SOUND, soundId);
			}
		}

		private void OnTriggerGunSound(GameObject go, long roleId, int soundId, bool isSilencer = false)
		{
			if (!Battle.Ins.SelfPlayer.Landed && !_mRoleIds.Contains(roleId))
			{
				if (isSilencer)
				{
					OnTriggerSound(go, SoundSource.GUN_SOUND_SILENCE, cfg.Consts.GUN_SOUND_DISTANCE);
				}
				else
				{
					OnTriggerSound(go, SoundSource.GUN_SOUND, cfg.Consts.GUN_SOUND_DISTANCE);
				}
			}
		}

		private void OnTriggerAirDropSound(GameObject go, int airdropId, int soundId)
		{
			if (!Battle.Ins.SelfPlayer.Landed)
			{
				OnTriggerSound(go, SoundSource.AIRDROP_SOUND, cfg.Consts.AIRDROP_SOUND_DISTANCE);
			}
		}

		public void CalcAirDropDistance()
		{
			foreach (KeyValuePair<int, AirDropDistance> item in AirDropDistanceDic)
			{
				item.Value.remainTime -= 1f;
				if (item.Value.remainTime <= 0f)
				{
					AirDropDistanceDic.Remove(item.Key);
					continue;
				}
				Vector3 vector = item.Value.AirDropPosGameObject.transform.position - _mCenterWorldPos;
				if (item.Value.isDir)
				{
					if (vector.x > -200f && vector.x < 200f && vector.z > -200f && vector.z < 200f)
					{
						item.Value.isDir = false;
						OnTriggerSound(item.Value.AirDropPosGameObject, SoundSource.AIRDROP_SOUND, cfg.Consts.AIRDROP_SOUND_DISTANCE);
					}
				}
				else if (vector.x < -200f || vector.x > 200f || vector.z < -200f || vector.z > 200f)
				{
					OnTriggerSound(item.Value.AirDropPosGameObject, SoundSource.AIRDROP_SOUND, cfg.Consts.AIRDROP_SOUND_DISTANCE);
				}
			}
		}

		private void OnTriggerVehicleSound(GameObject go, int vehicleId, int soundId)
		{
			if (Battle.Ins.SelfPlayer.Landed)
			{
				return;
			}
			_mCurrentVehicleIds.Clear();
			if (m_Self != null && m_Self.VehicleId > 0)
			{
				_mCurrentVehicleIds.Add(m_Self.VehicleId);
			}
			foreach (TeamateInfo item in Singleton<TeamateMgr>.Ins.GetTeamateInfo())
			{
				if (item.vehicleId > 0)
				{
					_mCurrentVehicleIds.Add(item.vehicleId);
				}
			}
			if (!_mCurrentVehicleIds.Contains(vehicleId))
			{
				OnTriggerSound(go, SoundSource.VEHICLE_SOUND, soundId);
			}
		}

		private float GetMaxDistance(int soundId)
		{
			SoundCfg soundCfg = SoundCfg.Get(soundId);
			if (soundCfg != null)
			{
				return soundCfg.radius;
			}
			return 20f;
		}

		private void OnTriggerSound(GameObject go, int soundType, int soundId)
		{
			Vector3 vector = go.transform.position - _mCenterWorldPos;
			float num = Vector3.Distance(go.transform.position, _mCenterWorldPos);
			float maxDistance = GetMaxDistance(soundId);
			if (!(num <= maxDistance))
			{
				return;
			}
			bool flag = false;
			if (soundType == SoundSource.GUN_SOUND || soundType == SoundSource.AIRDROP_SOUND)
			{
				if (vector.x < -200f || vector.x > 200f || vector.z < -200f || vector.z > 200f)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			SoundSource soundSource = FindSameSoundSource(go, soundType, flag);
			if (soundSource == null)
			{
				soundSource = CreateSoundSource(go, soundType, soundId, flag);
				if (flag)
				{
					_mSoundSource2IconInMapDir[soundSource] = GetSoundIconFromPool(soundType, _mSoundIconInMapParent, true);
				}
				else
				{
					_mSoundSource2IconInMap[soundSource] = GetSoundIconFromPool(soundType, _mSoundIconInMapParent);
				}
				_mSoundToTrack.Add(soundSource);
			}
			if (soundType == SoundSource.VEHICLE_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mVehicleSoundVanishTime;
			}
			else if (soundType == SoundSource.AIRDROP_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mAirdropSoundVanishTime;
			}
			else if (soundType == SoundSource.FOOT_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mFootVanishTime;
			}
			else if (soundType == SoundSource.GUN_SOUND_SILENCE)
			{
				soundSource._mTimeToVanish = Time.time + _mDefaultVanishTime;
			}
			else
			{
				soundSource._mTimeToVanish = Time.time + _mDefaultVanishTime;
			}
		}

		private void OnTriggerSound(GameObject go, int soundType, float maxDistance)
		{
			if (!go)
			{
				return;
			}
			Vector3 vector = go.transform.position - _mCenterWorldPos;
			float num = Vector3.Distance(go.transform.position, _mCenterWorldPos);
			if (!(num <= maxDistance))
			{
				return;
			}
			bool flag = false;
			if (soundType == SoundSource.GUN_SOUND || soundType == SoundSource.AIRDROP_SOUND)
			{
				if (vector.x < -200f || vector.x > 200f || vector.z < -200f || vector.z > 200f)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			SoundSource soundSource = FindSameSoundSource(go, soundType, flag);
			if (soundSource == null)
			{
				soundSource = CreateSoundSource(go, soundType, 1, flag);
				if (flag)
				{
					_mSoundSource2IconInMapDir[soundSource] = GetSoundIconFromPool(soundType, _mSoundIconInMapParent, flag);
				}
				else
				{
					_mSoundSource2IconInMap[soundSource] = GetSoundIconFromPool(soundType, _mSoundIconInMapParent);
				}
				_mSoundToTrack.Add(soundSource);
			}
			if (soundType == SoundSource.VEHICLE_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mVehicleSoundVanishTime;
			}
			else if (soundType == SoundSource.AIRDROP_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mAirdropSoundVanishTime;
			}
			else if (soundType == SoundSource.FOOT_SOUND)
			{
				soundSource._mTimeToVanish = Time.time + _mFootVanishTime;
			}
			else
			{
				soundSource._mTimeToVanish = Time.time + _mDefaultVanishTime;
			}
		}

		private IEnumerator ThreeSecondTick()
		{
			while (true)
			{
				yield return _threeTime;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_TouchPad = component.GameObjects[0].gameObject;
			m_zuo_shang_jiao = component.GameObjects[1].gameObject;
			m_map = component.GameObjects[2].gameObject;
			m_lefttop = component.GameObjects[3].gameObject;
			m_last_drive = component.GameObjects[4].gameObject;
			m_map_line_root = component.GameObjects[5].gameObject;
			m_map_line = component.GameObjects[6].gameObject;
			m_resource_tags = component.GameObjects[7].gameObject;
			m_tool_box_root = component.GameObjects[8].gameObject;
			m_tool_box = component.GameObjects[9].gameObject;
			m_second_battle_root = component.GameObjects[10].gameObject;
			m_second_battle_tag = component.GameObjects[11].gameObject;
			m_main_tags_root = component.GameObjects[12].gameObject;
			m_main_tag = component.GameObjects[13].gameObject;
			m_plant_tags_root = component.GameObjects[14].gameObject;
			m_plant_tag = component.GameObjects[15].gameObject;
			m_tree_tags_root = component.GameObjects[16].gameObject;
			m_tree_tag = component.GameObjects[17].gameObject;
			m_footSoundInMap = component.GameObjects[18].gameObject;
			m_vehicleSoundInMap = component.GameObjects[19].gameObject;
			m_gunSoundInMap = component.GameObjects[20].gameObject;
			m_gun_sound_pos_map = component.GameObjects[21].gameObject;
			m_gun_silence_sound_pos = component.GameObjects[22].gameObject;
			m_airdrop_pos_map = component.GameObjects[23].gameObject;
			m_airdrop_dir_map = component.GameObjects[24].gameObject;
			m_team_members = component.GameObjects[25].gameObject.GetComponent<UIGameObjectList>().objects;
			m_team_membersObj = component.GameObjects[25].gameObject;
			if (m_team_memberslist.Count <= 0)
			{
				for (int i = 0; i < m_team_members.Length; i++)
				{
					m_team_memberslist.Add(View.AddComponentIfNotExist<GPlayerMapMarks>(m_team_members[i].gameObject));
				}
			}
			txt_coordinate = component.GameObjects[26].gameObject;
			txt_coordinateText = txt_coordinate.GetComponent<Text>();
			btn_setting = component.GameObjects[27].gameObject;
			btn_louderSpeaker = component.GameObjects[28].gameObject;
			m_louderSpeakerDisabled = component.GameObjects[29].gameObject;
			btn_microPhone = component.GameObjects[30].gameObject;
			m_microphoneDisabled = component.GameObjects[31].gameObject;
			m_notWatch = component.GameObjects[32].gameObject;
			m_jing = component.GameObjects[33].gameObject;
			m_jingAim = component.GameObjects[34].gameObject;
			m_JoystackTypeCar = View.AddComponentIfNotExist<GJoystackType2>(component.GameObjects[35].gameObject);
			m_JoystackType2 = View.AddComponentIfNotExist<GJoystackType2>(component.GameObjects[36].gameObject);
			m_InLand = component.GameObjects[37].gameObject;
			m_NearCar = component.GameObjects[38].gameObject;
			btn_NearCarDrive = component.GameObjects[39].gameObject;
			btn_NearCarUpCar = component.GameObjects[40].gameObject;
			m_swimPanel = component.GameObjects[41].gameObject;
			btn_SwimUp = component.GameObjects[42].gameObject;
			btn_SwimDown = component.GameObjects[43].gameObject;
			m_shootPanel = component.GameObjects[44].gameObject;
			m_OperationBtns = component.GameObjects[45].gameObject;
			btn_kaijing = component.GameObjects[46].gameObject;
			btn_leftFire = component.GameObjects[47].gameObject;
			btn_fire = component.GameObjects[48].gameObject;
			m_fireImage = component.GameObjects[49].gameObject;
			m_fireRealIcon = component.GameObjects[50].gameObject;
			btn_build = component.GameObjects[51].gameObject;
			m_bulletsSelect = component.GameObjects[52].gameObject.GetComponent<UIGameObjectList>().objects;
			m_bulletsSelectObj = component.GameObjects[52].gameObject;
			if (m_bulletsSelectlist.Count <= 0)
			{
				for (int j = 0; j < m_bulletsSelect.Length; j++)
				{
					m_bulletsSelectlist.Add(View.AddComponentIfNotExist<GBulletSelectInfo>(m_bulletsSelect[j].gameObject));
				}
			}
			btn_changeBullet = component.GameObjects[53].gameObject;
			m_moshi = component.GameObjects[54].gameObject;
			m_ShootModelImage = component.GameObjects[55].gameObject;
			btn_Auto2 = component.GameObjects[56].gameObject;
			btn_SanLianfa2 = component.GameObjects[57].gameObject;
			btn_Danfa2 = component.GameObjects[58].gameObject;
			m_ChangeType2 = component.GameObjects[59].gameObject;
			btn_up2 = component.GameObjects[60].gameObject;
			m_JumpImage = component.GameObjects[61].gameObject;
			m_PatiziImage = component.GameObjects[62].gameObject;
			btn_crouch2 = component.GameObjects[63].gameObject;
			btn_crouching2 = component.GameObjects[64].gameObject;
			btn_pa2 = component.GameObjects[65].gameObject;
			btn_paing2 = component.GameObjects[66].gameObject;
			btn_autoRun = component.GameObjects[67].gameObject;
			btn_HelpOther = component.GameObjects[68].gameObject;
			m_car = component.GameObjects[69].gameObject;
			m_zaijiuInfo = component.GameObjects[70].gameObject;
			m_zaijvinfo = component.GameObjects[71].gameObject;
			m_carInfo = component.GameObjects[72].gameObject;
			m_carSeats = component.GameObjects[73].gameObject.GetComponent<UIGameObjectList>().objects;
			m_carSeatsObj = component.GameObjects[73].gameObject;
			m_carcheti = component.GameObjects[74].gameObject;
			m_shipInfo = component.GameObjects[75].gameObject;
			m_shipSeats = component.GameObjects[76].gameObject.GetComponent<UIGameObjectList>().objects;
			m_shipSeatsObj = component.GameObjects[76].gameObject;
			m_shipcheti = component.GameObjects[77].gameObject;
			m_planeInfo = component.GameObjects[78].gameObject;
			m_planeSeats = component.GameObjects[79].gameObject.GetComponent<UIGameObjectList>().objects;
			m_planeSeatsObj = component.GameObjects[79].gameObject;
			m_planecheti = component.GameObjects[80].gameObject;
			m_moto2 = component.GameObjects[81].gameObject;
			m_moto2Seats = component.GameObjects[82].gameObject.GetComponent<UIGameObjectList>().objects;
			m_moto2SeatsObj = component.GameObjects[82].gameObject;
			m_moto2cheti = component.GameObjects[83].gameObject;
			m_moto3 = component.GameObjects[84].gameObject;
			m_moto3Seats = component.GameObjects[85].gameObject.GetComponent<UIGameObjectList>().objects;
			m_moto3SeatsObj = component.GameObjects[85].gameObject;
			m_moto3cheti = component.GameObjects[86].gameObject;
			txt_car_speed = component.GameObjects[87].gameObject;
			txt_car_speedText = txt_car_speed.GetComponent<Text>();
			m_car_you = component.GameObjects[88].gameObject;
			m_car_you_slider = component.GameObjects[89].gameObject;
			m_car_blood = component.GameObjects[90].gameObject;
			m_car_blood_slider = component.GameObjects[91].gameObject;
			m_planeController = component.GameObjects[92].gameObject;
			m_JoystackType3 = View.AddComponentIfNotExist<GJoystackType2>(component.GameObjects[93].gameObject);
			m_JoystackType4 = View.AddComponentIfNotExist<GJoystackType2>(component.GameObjects[94].gameObject);
			m_carController = component.GameObjects[95].gameObject;
			btn_horn = component.GameObjects[96].gameObject;
			btn_addSpeed = component.GameObjects[97].gameObject;
			m_carButtons = component.GameObjects[98].gameObject;
			btn_carGasInput = component.GameObjects[99].gameObject;
			btn_carBrakeInput = component.GameObjects[100].gameObject;
			btn_carSteerLeftInput = component.GameObjects[101].gameObject;
			btn_carSteerRightInput = component.GameObjects[102].gameObject;
			m_CarBtns = component.GameObjects[103].gameObject;
			btn_downCar = component.GameObjects[104].gameObject;
			btn_addoil = component.GameObjects[105].gameObject;
			btn_tanshen = component.GameObjects[106].gameObject;
			btn_InCarDrive = component.GameObjects[107].gameObject;
			btn_huanzuo = component.GameObjects[108].gameObject;
			txt_TeamPlayerName = component.GameObjects[109].gameObject;
			txt_TeamPlayerNameText = txt_TeamPlayerName.GetComponent<Text>();
			btn_quicklook = component.GameObjects[110].gameObject;
			m_quicklook = component.GameObjects[111].gameObject;
			m_zhunxing = component.GameObjects[112].gameObject.GetComponent<UIGameObjectList>().objects;
			m_zhunxingObj = component.GameObjects[112].gameObject;
			if (m_zhunxinglist.Count <= 0)
			{
				for (int k = 0; k < m_zhunxing.Length; k++)
				{
					m_zhunxinglist.Add(View.AddComponentIfNotExist<GZhunxing>(m_zhunxing[k].gameObject));
				}
			}
			m_zhunxingPoint = component.GameObjects[113].gameObject;
			m_zhunxing1 = View.AddComponentIfNotExist<GZhunxing>(component.GameObjects[114].gameObject);
			m_zhunxing2 = View.AddComponentIfNotExist<GZhunxing>(component.GameObjects[115].gameObject);
			m_zhunxing3 = View.AddComponentIfNotExist<GZhunxing>(component.GameObjects[116].gameObject);
			btn_Gm = component.GameObjects[117].gameObject;
			m_PulmonaryPanel = component.GameObjects[118].gameObject;
			txt_use_item_name = component.GameObjects[119].gameObject;
			txt_use_item_nameText = txt_use_item_name.GetComponent<Text>();
			m_use_time = component.GameObjects[120].gameObject;
			txt_use_time = component.GameObjects[121].gameObject;
			txt_use_timeText = txt_use_time.GetComponent<Text>();
			txt_PlayerPos = component.GameObjects[122].gameObject;
			txt_PlayerPosText = txt_PlayerPos.GetComponent<Text>();
			m_killTypeIcon = component.GameObjects[123].gameObject;
			m_killNumIcon = component.GameObjects[124].gameObject;
			m_xueDir = component.GameObjects[125].gameObject;
			m_ReduceHpImage = component.GameObjects[126].gameObject;
			m_shanghaizhunxing = component.GameObjects[127].gameObject;
			m_zhu = component.GameObjects[128].gameObject;
			m_zsj = component.GameObjects[129].gameObject;
			btn_shop = component.GameObjects[130].gameObject;
			btn_email = component.GameObjects[131].gameObject;
			m_mail_red_dot = component.GameObjects[132].gameObject;
			btn_ladder_task = component.GameObjects[133].gameObject;
			m_ladder_task_red = component.GameObjects[134].gameObject;
			btn_friend = component.GameObjects[135].gameObject;
			m_friend_red_dot = component.GameObjects[136].gameObject;
			btn_info = component.GameObjects[137].gameObject;
			m_info_red = component.GameObjects[138].gameObject;
			btn_welfare = component.GameObjects[139].gameObject;
			m_welfare_red_dot = component.GameObjects[140].gameObject;
			btn_union = component.GameObjects[141].gameObject;
			m_ = component.GameObjects[142].gameObject;
			btn_active = component.GameObjects[143].gameObject;
			m_red_active = component.GameObjects[144].gameObject;
			btn_firstcharge = component.GameObjects[145].gameObject;
			m_first_charge_red = component.GameObjects[146].gameObject;
			btn_active1 = component.GameObjects[147].gameObject;
			btn_ = component.GameObjects[148].gameObject;
			m_shop_no_oversea = component.GameObjects[149].gameObject;
			m_buildChilds = component.GameObjects[150].gameObject.GetComponent<UIGameObjectList>().objects;
			m_buildChildsObj = component.GameObjects[150].gameObject;
			if (m_buildChildslist.Count <= 0)
			{
				for (int l = 0; l < m_buildChilds.Length; l++)
				{
					m_buildChildslist.Add(View.AddComponentIfNotExist<GBuildPartItem>(m_buildChilds[l].gameObject));
				}
			}
			scp_bagBuilds = component.GameObjects[151].gameObject;
			m_cell = View.AddComponentIfNotExist<GBagBuild>(component.GameObjects[152].gameObject);
			btn_pack = component.GameObjects[153].gameObject;
			m_buildPartTypes = component.GameObjects[154].gameObject;
			btn_null = component.GameObjects[155].gameObject;
			btn_obstacle = component.GameObjects[156].gameObject;
			btn_floor = component.GameObjects[157].gameObject;
			btn_stair = component.GameObjects[158].gameObject;
			btn_window = component.GameObjects[159].gameObject;
			btn_wall = component.GameObjects[160].gameObject;
			btn_fundation = component.GameObjects[161].gameObject;
			btn_changeButton = component.GameObjects[162].gameObject;
			m_changeButtonLight = component.GameObjects[163].gameObject;
			m_bagItems = component.GameObjects[164].gameObject.GetComponent<UIGameObjectList>().objects;
			m_bagItemsObj = component.GameObjects[164].gameObject;
			if (m_bagItemslist.Count <= 0)
			{
				for (int m = 0; m < m_bagItems.Length; m++)
				{
					m_bagItemslist.Add(View.AddComponentIfNotExist<GBuildBagItem>(m_bagItems[m].gameObject));
				}
			}
			m_headInfo = component.GameObjects[165].gameObject;
			m_player_head_icon = component.GameObjects[166].gameObject;
			m_player_frame = component.GameObjects[167].gameObject;
			btn_head = component.GameObjects[168].gameObject;
			txt_player_name = component.GameObjects[169].gameObject;
			txt_player_nameText = txt_player_name.GetComponent<Text>();
			m_BloodSlider2 = component.GameObjects[170].gameObject;
			m_BloodFill2 = component.GameObjects[171].gameObject;
			m_ysj = component.GameObjects[172].gameObject;
			m_mybloodSlilder = component.GameObjects[173].gameObject;
			m_mybloodFillImage = component.GameObjects[174].gameObject;
			txt_mybloodNum = component.GameObjects[175].gameObject;
			txt_mybloodNumText = txt_mybloodNum.GetComponent<Text>();
			m_sld_hunger = component.GameObjects[176].gameObject;
			txt_hunger = component.GameObjects[177].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			m_sld_water = component.GameObjects[178].gameObject;
			txt_water = component.GameObjects[179].gameObject;
			txt_waterText = txt_water.GetComponent<Text>();
			btn_chat = component.GameObjects[180].gameObject;
			m_chats = component.GameObjects[181].gameObject.GetComponent<UIGameObjectList>().objects;
			m_chatsObj = component.GameObjects[181].gameObject;
			txt_chat = component.GameObjects[182].gameObject;
			txt_chatText = txt_chat.GetComponent<Text>();
			txt_chat_1 = component.GameObjects[183].gameObject;
			txt_chat_1Text = txt_chat_1.GetComponent<Text>();
			m_operatesPanel = component.GameObjects[184].gameObject;
			btn_operate = component.GameObjects[185].gameObject;
			m_buildBaseBtns = component.GameObjects[186].gameObject.GetComponent<UIGameObjectList>().objects;
			m_buildBaseBtnsObj = component.GameObjects[186].gameObject;
			btn_opUp = component.GameObjects[187].gameObject;
			btn_opMove = component.GameObjects[188].gameObject;
			btn_opRotate = component.GameObjects[189].gameObject;
			btn_opRepair = component.GameObjects[190].gameObject;
			btn_opDestory = component.GameObjects[191].gameObject;
			m_plantFinishTimeSlider = component.GameObjects[192].gameObject;
			txt_plantFinishTime = component.GameObjects[193].gameObject;
			txt_plantFinishTimeText = txt_plantFinishTime.GetComponent<Text>();
			m_buildoperatesbtns = component.GameObjects[194].gameObject;
			txt_aimedBtnsTopName = component.GameObjects[195].gameObject;
			txt_aimedBtnsTopNameText = txt_aimedBtnsTopName.GetComponent<Text>();
			m_extraBtns = component.GameObjects[196].gameObject.GetComponent<UIGameObjectList>().objects;
			m_extraBtnsObj = component.GameObjects[196].gameObject;
			if (m_extraBtnslist.Count <= 0)
			{
				for (int n = 0; n < m_extraBtns.Length; n++)
				{
					m_extraBtnslist.Add(View.AddComponentIfNotExist<GExtraBtns>(m_extraBtns[n].gameObject));
				}
			}
			m_kills = component.GameObjects[197].gameObject.GetComponent<UIGameObjectList>().objects;
			m_killsObj = component.GameObjects[197].gameObject;
			if (m_killslist.Count <= 0)
			{
				for (int num = 0; num < m_kills.Length; num++)
				{
					m_killslist.Add(View.AddComponentIfNotExist<GKillItem>(m_kills[num].gameObject));
				}
			}
			m_saveSelfPanel = component.GameObjects[198].gameObject;
			btn_saveSelf = component.GameObjects[199].gameObject;
			btn_giveUp = component.GameObjects[200].gameObject;
			m_player_head_bg = component.GameObjects[201].gameObject;
			m_sld_hunder_image = component.GameObjects[202].gameObject;
			m_sld_water_image = component.GameObjects[203].gameObject;
			m_die_dropitem_pos = component.GameObjects[204].gameObject;
			m_autoRunChoosed = component.GameObjects[205].gameObject;
			m_autoRunCircle = component.GameObjects[206].gameObject;
			m_image_no_saveitem = component.GameObjects[207].gameObject;
			ckb_trusteeship = component.GameObjects[208].gameObject;
			m_trusteeship = component.GameObjects[209].gameObject;
			txt_set_trusteeship = component.GameObjects[210].gameObject;
			txt_set_trusteeshipText = txt_set_trusteeship.GetComponent<Text>();
			m_saveBtnEffect = component.GameObjects[211].gameObject;
			m_lajitong_tags_root = component.GameObjects[212].gameObject;
			m_lajitong_tag = component.GameObjects[213].gameObject;
			m_xiangzi_tags_root = component.GameObjects[214].gameObject;
			m_xiangzi_tag = component.GameObjects[215].gameObject;
			txt_gameTime = component.GameObjects[216].gameObject;
			txt_gameTimeText = txt_gameTime.GetComponent<Text>();
			m_reduceHp10 = component.GameObjects[217].gameObject;
			m_reduceHp = component.GameObjects[218].gameObject;
			m_addHp = component.GameObjects[219].gameObject;
			m_jieEffect = component.GameObjects[220].gameObject;
			m_reduceHpFull = component.GameObjects[221].gameObject;
			m_nightDi = component.GameObjects[222].gameObject;
			m_huanghunDi = component.GameObjects[223].gameObject;
			m_dayDi = component.GameObjects[224].gameObject;
			m_qingchenDi = component.GameObjects[225].gameObject;
			m_nightIcon = component.GameObjects[226].gameObject;
			m_dayIcon = component.GameObjects[227].gameObject;
			m_red_active_0 = component.GameObjects[228].gameObject;
			m_trusteeship_circle = component.GameObjects[229].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__0(GameObject go)
		{
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__1(GameObject go)
		{
			if (Battle.Ins.SelfPlayer != null && Battle.Ins.SelfPlayer.NearCar != null && Battle.Ins.SelfPlayer.IsDriver)
			{
				Battle.Ins.SelfPlayer.NearCar.PlayHornSound();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (m_Self.FSMUpBody.CurrentState.ID != StateID.Attack)
			{
				if (m_Self.FSM.CurrentState.ID == StateID.Pa && m_Self.CheckCanPaUp())
				{
					m_Self.FSM.SwitchState(StateID.PaUp);
				}
				else if (m_Self.FSM.CurrentState.ID == StateID.Stand)
				{
					SingletonMono<AudioManager>.Ins.Play2D(339);
					Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.Crouch);
				}
				else if (m_Self.FSM.CurrentState.ID == StateID.Crouch && m_Self.CheckCanCrouchUp())
				{
					m_Self.FSM.SwitchState(StateID.Stand);
					SingletonMono<AudioManager>.Ins.Play2D(435);
				}
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (m_Self.FSMUpBody.CurrentState.ID != StateID.Attack)
			{
				if (m_Self.FSM.CurrentState.ID == StateID.Crouch || m_Self.FSM.CurrentState.ID == StateID.Stand)
				{
					Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.CrouchDown);
				}
				if (m_Self.FSM.CurrentState.ID == StateID.Pa && m_Self.CheckCanPaToStand())
				{
					m_Self.FSM.SwitchState(StateID.PaUp, true);
					SingletonMono<AudioManager>.Ins.Play2D(341);
				}
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__4(GameObject go)
		{
			ViewMgr.Ins.ShowView<BattleMapPanel>();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			m_InAutoRunBtn = false;
			m_autoRunChoosed.SetActiveBetter(false);
			m_Self.CloseAutoRun();
			BattleEvent.StopTrusteeship();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			btn_autoRun.SetActiveBetter(false);
			if (m_InAutoRunBtn)
			{
				m_Self.SetAutoRun();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			if (!m_Self.AutoRun)
			{
				m_Self.YaoGanAngle = MathUtils.GetOrientation(m_Self.Input);
				if ((m_Self.YaoGanAngle >= 0f && m_Self.YaoGanAngle <= 50f) || (m_Self.YaoGanAngle > 310f && m_Self.YaoGanAngle <= 360f))
				{
					btn_autoRun.SetActiveBetter(true);
				}
				else
				{
					btn_autoRun.SetActiveBetter(false);
				}
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			m_Self.ChangeYPos(0.05f);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__9(GameObject go)
		{
			m_Self.ChangeYPos(-0.05f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__A(GameObject go)
		{
			Singleton<RoleMgr>.Ins.GetRoleMoreInformation(Singleton<RoleMgr>.Ins.info.roleId);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__B(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.VirtualNitrogenButton, 1f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__C(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.VirtualNitrogenButton, 0f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__D(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Vertical, 1f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__E(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Vertical, 0f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__F(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Vertical, -1f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__10(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Vertical, 0f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__11(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Horizontal, 1f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__12(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Horizontal, 0f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__13(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Horizontal, -1f);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__14(GameObject go)
		{
			CrossPlatformInputManager.SetAxis(KeyName.Horizontal, 0f);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__15(GameObject go)
		{
			if (m_Self.CheckCommonDoCondition() && !m_Self.InCar)
			{
				m_Self.FSM.SwitchState(StateID.SavePeople, m_Self.FSM.CurrentState.ID);
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__16(GameObject go)
		{
			ViewMgr.Ins.ShowView<SettingPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__17(GameObject go)
		{
			if (Singleton<BagMgr>.Ins.IsCanRescue())
			{
				CRebirth msg = new CRebirth
				{
					type = 2
				};
				Client2Gs.Ins.Send(msg);
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__18(GameObject go)
		{
			CDie msg = new CDie();
			Client2Gs.Ins.Send(msg);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__19(GameObject go)
		{
			m_InAutoRunBtn = true;
			m_autoRunChoosed.SetActiveBetter(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1A(GameObject go)
		{
			m_InAutoRunBtn = false;
			m_autoRunChoosed.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1B(GameObject go)
		{
			if ((double)Math.Abs(DragListener.pointEventData.delta.x) > 0.1 || (double)Math.Abs(DragListener.pointEventData.delta.y) > 0.1)
			{
				SetQuickLookCirclePos();
				Battle.Ins.MainCamera.DragCamera(DragListener.pointEventData.delta.x * 5f, DragListener.pointEventData.delta.y * 5f);
				Battle.Ins.QuickLooking = true;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1C(GameObject go)
		{
			Battle.Ins.QuickLooking = false;
			Battle.Ins.MainCamera.SetCameraTargetDirection(m_Self.PlayerTransform.forward);
			m_QuickLookImageRect.anchoredPosition = Vector2.zero;
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__1D(GameObject go)
		{
			if (!Singleton<GuideMgr>.Ins.IsAllNoviceTaskFinish())
			{
				AlertBox.Show(339);
			}
			else
			{
				ViewMgr.Ins.ShowView<LadderTaskPanel>();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1E(GameObject go)
		{
			m_longPress = true;
			ShowBullets();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1F()
		{
			RefreshQuickUseItems();
			UpdateQuickUseItemHightLignt(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__20()
		{
			m_dayIcon.SetActiveBetter(true);
			m_nightIcon.SetActiveBetter(false);
			m_qingchenDi.SetActiveBetter(false);
			m_dayDi.SetActiveBetter(true);
			m_nightDi.SetActiveBetter(false);
			m_huanghunDi.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__21()
		{
			SingletonMono<AudioManager>.Ins.Play2D(566);
			m_dayIcon.SetActiveBetter(false);
			m_nightIcon.SetActiveBetter(true);
			m_qingchenDi.SetActiveBetter(false);
			m_dayDi.SetActiveBetter(false);
			m_nightDi.SetActiveBetter(true);
			m_huanghunDi.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__22()
		{
			m_dayIcon.SetActiveBetter(true);
			m_nightIcon.SetActiveBetter(false);
			m_qingchenDi.SetActiveBetter(false);
			m_dayDi.SetActiveBetter(false);
			m_nightDi.SetActiveBetter(false);
			m_huanghunDi.SetActiveBetter(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__23()
		{
			m_Self.DayNightLight.enabled = false;
			SingletonMono<AudioManager>.Ins.Play2D(567);
			m_dayIcon.SetActiveBetter(true);
			m_nightIcon.SetActiveBetter(false);
			m_qingchenDi.SetActiveBetter(true);
			m_dayDi.SetActiveBetter(false);
			m_nightDi.SetActiveBetter(false);
			m_huanghunDi.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003CShowBullets_003Em__24(object[] o)
		{
			m_longPress = false;
		}

		[CompilerGenerated]
		private void _003COnClickBag_003Em__25()
		{
			if (m_Self.IsJiMiao && m_Self.CurGun != null)
			{
				m_Self.CurGun.CloseJiMiao();
			}
			HideOtherPanel();
		}

		[CompilerGenerated]
		private void _003CAttachNonCombatBtnEvent_003Em__26(bool b)
		{
			m_trusteeship_circle.SetActiveBetter(b);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__27(GameObject go)
		{
			ViewMgr.Ins.ShowView<FriendPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__28(GameObject go)
		{
			ViewMgr.Ins.ShowView<MailPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__29(GameObject go)
		{
			ViewMgr.Ins.ShowView<BagAndBuildPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__2A(GameObject go)
		{
			ViewMgr.Ins.ShowView<GMPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__2B(GameObject go)
		{
			ViewMgr.Ins.ShowView<ShoppingPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__2C(GameObject go)
		{
			ViewMgr.Ins.ShowView<ActivityPanel>(null, false);
		}

		[CompilerGenerated]
		private void _003CAttachNonCombatBtnEvent_003Em__2D(GameObject go)
		{
			if (_ckbTrusteeship.isOn)
			{
				SingletonMono<TrusteeshipMgr>.Ins.StartTrusteeship();
			}
			else
			{
				SingletonMono<TrusteeshipMgr>.Ins.StopTrusteeship();
			}
		}

		[CompilerGenerated]
		private static void _003CAttachNonCombatBtnEvent_003Em__2E(GameObject go)
		{
			ViewMgr.Ins.ShowView<SettingPanel>(SettingPanel.SettingPage.Trusteeship);
		}

		[CompilerGenerated]
		private static SoundSource _003C_mSoundSourcePool_003Em__2F()
		{
			return new SoundSource();
		}
	}
}
