using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class MainPanel : View
	{
		private GameObject m_zuo_shang_jiao;

		private GameObject m_map;

		private GameObject m_lefttop;

		private GameObject m_last_drive;

		private GameObject m_footSoundInMap;

		private GameObject m_vehicleSoundInMap;

		private GameObject m_gunSoundInMap;

		private GameObject m_airdrop_dir_map;

		private GameObject m_airdrop_pos_map;

		private GameObject m_gun_sound_pos_map;

		private GameObject m_mapInAirport;

		private GameObject[] m_mapInNewModeMarks;

		private GameObject m_mapInNewModeMarksObj;

		private GPlayerMapMarks m_self_in_new_mode;

		private List<GPlayerMapMarks> m_team_memberslist = new List<GPlayerMapMarks>();

		private GameObject[] m_team_members;

		private GameObject m_team_membersObj;

		private GameObject btn_louderSpeaker;

		private GameObject m_louderSpeakerDisabled;

		private GameObject btn_microPhone;

		private GameObject m_microphoneDisabled;

		private GameObject btn_setting;

		private GameObject m_gasTimeSlider;

		private GameObject m_gasHuman;

		private GameObject txt_gasTime;

		private Text txt_gasTimeText;

		private GameObject m_smallcircle;

		private GameObject txt_reliveTimeSmallCircle;

		private Text txt_reliveTimeSmallCircleText;

		private GameObject txt_remainTimeSmallCircle;

		private Text txt_remainTimeSmallCircleText;

		private GameObject m_myTeamColorSmallCircle;

		private GameObject txt_myScoreSmallCircle;

		private Text txt_myScoreSmallCircleText;

		private GameObject txt_myRankSmallCircle;

		private Text txt_myRankSmallCircleText;

		private GameObject m_firstScoreTeamPanel;

		private GameObject m_firstScoreTeamColorSmallCircle;

		private GameObject txt_firstScoreSmallCircle;

		private Text txt_firstScoreSmallCircleText;

		private GameObject txt_firstTeamSmallCircle;

		private Text txt_firstTeamSmallCircleText;

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_zuo_shang_jiao = component.GameObjects[0].gameObject;
			m_map = component.GameObjects[1].gameObject;
			m_lefttop = component.GameObjects[2].gameObject;
			m_last_drive = component.GameObjects[3].gameObject;
			m_footSoundInMap = component.GameObjects[4].gameObject;
			m_vehicleSoundInMap = component.GameObjects[5].gameObject;
			m_gunSoundInMap = component.GameObjects[6].gameObject;
			m_airdrop_dir_map = component.GameObjects[7].gameObject;
			m_airdrop_pos_map = component.GameObjects[8].gameObject;
			m_gun_sound_pos_map = component.GameObjects[9].gameObject;
			m_mapInAirport = component.GameObjects[10].gameObject;
			m_mapInNewModeMarks = component.GameObjects[11].gameObject.GetComponent<UIGameObjectList>().objects;
			m_mapInNewModeMarksObj = component.GameObjects[11].gameObject;
			m_self_in_new_mode = View.AddComponentIfNotExist<GPlayerMapMarks>(component.GameObjects[12].gameObject);
			m_team_members = component.GameObjects[13].gameObject.GetComponent<UIGameObjectList>().objects;
			m_team_membersObj = component.GameObjects[13].gameObject;
			for (int i = 0; i < m_team_members.Length; i++)
			{
				m_team_memberslist.Add(View.AddComponentIfNotExist<GPlayerMapMarks>(m_team_members[i].gameObject));
			}
			btn_louderSpeaker = component.GameObjects[14].gameObject;
			m_louderSpeakerDisabled = component.GameObjects[15].gameObject;
			btn_microPhone = component.GameObjects[16].gameObject;
			m_microphoneDisabled = component.GameObjects[17].gameObject;
			btn_setting = component.GameObjects[18].gameObject;
			m_gasTimeSlider = component.GameObjects[19].gameObject;
			m_gasHuman = component.GameObjects[20].gameObject;
			txt_gasTime = component.GameObjects[21].gameObject;
			txt_gasTimeText = txt_gasTime.GetComponent<Text>();
			m_smallcircle = component.GameObjects[22].gameObject;
			txt_reliveTimeSmallCircle = component.GameObjects[23].gameObject;
			txt_reliveTimeSmallCircleText = txt_reliveTimeSmallCircle.GetComponent<Text>();
			txt_remainTimeSmallCircle = component.GameObjects[24].gameObject;
			txt_remainTimeSmallCircleText = txt_remainTimeSmallCircle.GetComponent<Text>();
			m_myTeamColorSmallCircle = component.GameObjects[25].gameObject;
			txt_myScoreSmallCircle = component.GameObjects[26].gameObject;
			txt_myScoreSmallCircleText = txt_myScoreSmallCircle.GetComponent<Text>();
			txt_myRankSmallCircle = component.GameObjects[27].gameObject;
			txt_myRankSmallCircleText = txt_myRankSmallCircle.GetComponent<Text>();
			m_firstScoreTeamPanel = component.GameObjects[28].gameObject;
			m_firstScoreTeamColorSmallCircle = component.GameObjects[29].gameObject;
			txt_firstScoreSmallCircle = component.GameObjects[30].gameObject;
			txt_firstScoreSmallCircleText = txt_firstScoreSmallCircle.GetComponent<Text>();
			txt_firstTeamSmallCircle = component.GameObjects[31].gameObject;
			txt_firstTeamSmallCircleText = txt_firstTeamSmallCircle.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}
	}
}
