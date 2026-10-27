using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using cfg;

public class CarInput : MonoBehaviour
{
	private CarController carController;

	private float gasInput;

	private float brakeInput;

	private float steerInput;

	private float nitrogenInput;

	private static float minimalGasDegree;

	private static float minimalSteerDegree;

	private float minimalGasInput = Mathf.Sin(minimalGasDegree * ((float)Math.PI / 180f));

	private float minimalSteerInput = Mathf.Sin(minimalSteerDegree * ((float)Math.PI / 180f));

	private float lastSteerInput;

	private float lastNitrogenInput;

	private void Awake()
	{
		carController = GetComponent<CarController>();
	}

	private void OnEnable()
	{
		carController.hasInput = true;
		CrossPlatformInputManager.SetAxis(KeyName.Vertical, 0f);
		CrossPlatformInputManager.SetAxis(KeyName.Horizontal, 0f);
		CrossPlatformInputManager.SetAxis(KeyName.VirtualNitrogenButton, 0f);
	}

	private void Update()
	{
		if (carController != null)
		{
			gasInput = CrossPlatformInputManager.GetAxis(KeyName.Vertical);
			brakeInput = Mathf.Clamp01(0f - CrossPlatformInputManager.GetAxis(KeyName.Vertical));
			steerInput = CrossPlatformInputManager.GetAxis(KeyName.Horizontal);
			nitrogenInput = CrossPlatformInputManager.GetAxis(KeyName.VirtualNitrogenButton);
			if (Mathf.Abs(gasInput) < minimalGasInput)
			{
				gasInput = 0f;
			}
			else
			{
				float num = ((gasInput > 0f) ? 1 : (-1));
				gasInput = Mathf.Sqrt(gasInput * gasInput + steerInput * steerInput);
				gasInput *= num;
			}
			if (Mathf.Abs(steerInput) < minimalSteerInput)
			{
				steerInput = 0f;
			}
			else
			{
				steerInput -= minimalSteerInput;
			}
			if (Input.GetKey(KeyCode.LeftShift))
			{
				nitrogenInput = 1f;
			}
			if (Input.GetKeyUp(KeyCode.LeftShift))
			{
				nitrogenInput = 0f;
			}
			if (lastNitrogenInput <= 0f && nitrogenInput > 0f)
			{
				carController.PlayNitrogenSound();
			}
			lastNitrogenInput = nitrogenInput;
			carController.gasInput = gasInput;
			carController.brakeInput = brakeInput;
			carController._nitrogenInput = nitrogenInput;
			carController.steerInput = (lastSteerInput = Mathf.Lerp(lastSteerInput, steerInput, Time.deltaTime * ConstsBs.CARTURNSPEED));
		}
	}

	private void OnDestroy()
	{
		carController.gasInput = 0f;
		carController.steerInput = 0f;
		carController.brakeInput = 0f;
		carController._nitrogenInput = 0f;
		carController.hasInput = false;
		lastSteerInput = 0f;
	}
}
