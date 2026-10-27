using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HelicopterInput : MonoBehaviour
{
	private HelicopterController helicopterController;

	private float leftHorizontalInput;

	private float leftVerticalInput;

	private float rightHorizontalInput;

	private float rightVeticalInput;

	private void Awake()
	{
		helicopterController = GetComponent<HelicopterController>();
		helicopterController.hasInput = true;
	}

	private void Update()
	{
		if (helicopterController != null)
		{
			leftHorizontalInput = CrossPlatformInputManager.GetAxis(KeyName.LeftJoystickHorizotal);
			leftVerticalInput = CrossPlatformInputManager.GetAxis(KeyName.LeftJoystickVertical);
			rightHorizontalInput = CrossPlatformInputManager.GetAxis(KeyName.RightJoystickHorizotal);
			rightVeticalInput = CrossPlatformInputManager.GetAxis(KeyName.RightJoystickVertical);
			helicopterController.leftHorizontalInput = leftHorizontalInput;
			helicopterController.leftVerticalInput = leftVerticalInput;
			helicopterController.rightHorizontalInput = rightHorizontalInput;
			helicopterController.rightVeticalInput = rightVeticalInput;
		}
	}

	private void OnDestroy()
	{
		helicopterController.leftHorizontalInput = 0f;
		helicopterController.leftVerticalInput = 0f;
		helicopterController.rightHorizontalInput = 0f;
		helicopterController.rightVeticalInput = 0f;
		helicopterController.hasInput = false;
	}
}
