using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using cfg;

public class TwoWheelMotoInput : MonoBehaviour
{
	private TwoWheelMotoController _twoWheelMotoController;

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
		_twoWheelMotoController = GetComponent<TwoWheelMotoController>();
	}

	private void OnEnable()
	{
		_twoWheelMotoController.hasInput = true;
		CrossPlatformInputManager.SetAxis(KeyName.Vertical, 0f);
		CrossPlatformInputManager.SetAxis(KeyName.Horizontal, 0f);
	}

	private void Update()
	{
		if (_twoWheelMotoController != null)
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
			if (lastNitrogenInput <= 0f && nitrogenInput > 0f)
			{
				_twoWheelMotoController.PlayNitrogenSound();
			}
			lastNitrogenInput = nitrogenInput;
			_twoWheelMotoController.gasInput = gasInput;
			_twoWheelMotoController.brakeInput = brakeInput;
			_twoWheelMotoController.steerInput = (lastSteerInput = Mathf.Lerp(lastSteerInput, steerInput, Time.deltaTime * ConstsBs.CARTURNSPEED));
			_twoWheelMotoController._nitrogenInput = nitrogenInput;
		}
	}

	private void OnDestroy()
	{
		_twoWheelMotoController.gasInput = 0f;
		_twoWheelMotoController.steerInput = 0f;
		_twoWheelMotoController.brakeInput = 0f;
		_twoWheelMotoController.hasInput = false;
		lastSteerInput = 0f;
	}
}
