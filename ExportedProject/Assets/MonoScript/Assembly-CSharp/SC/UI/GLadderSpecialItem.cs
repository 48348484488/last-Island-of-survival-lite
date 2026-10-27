using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLadderSpecialItem : MonoBehaviour
	{
		public GameObject txt_award;

		public Text txt_awardText;

		public object context;

		private void Awake()
		{
			txt_award = base.transform.Find("txt_award").gameObject;
			txt_awardText = txt_award.GetComponent<Text>();
		}
	}
}
