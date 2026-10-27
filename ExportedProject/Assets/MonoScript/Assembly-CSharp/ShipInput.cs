using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ShipInput : MonoBehaviour
{
	private ShipControll shipControll;

	private void Awake()
	{
		shipControll = GetComponent<ShipControll>();
		shipControll._mHasInput = true;
	}

	private void Update()
	{
		if (shipControll != null)
		{
			float axis = CrossPlatformInputManager.GetAxis(KeyName.Horizontal);
			float axis2 = CrossPlatformInputManager.GetAxis(KeyName.Vertical);
			float num = ((axis2 > 0f) ? 1 : (-1));
			axis2 = Mathf.Sqrt(axis2 * axis2 + axis * axis);
			axis2 *= num;
			shipControll._mHorizontalInput = axis;
			shipControll._mVerticalInput = axis2;
		}
	}

	private void OnDestroy()
	{
		shipControll._mHorizontalInput = 0f;
		shipControll._mVerticalInput = 0f;
		shipControll._mHasInput = false;
	}
}
