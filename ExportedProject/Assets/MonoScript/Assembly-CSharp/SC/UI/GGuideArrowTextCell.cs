using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GGuideArrowTextCell : MonoBehaviour
	{
		public GameObject txt_guide;

		public Text txt_guideText;

		public object context;

		private void Awake()
		{
			txt_guide = base.transform.Find("Image/txt_guide").gameObject;
			txt_guideText = txt_guide.GetComponent<Text>();
		}
	}
}
