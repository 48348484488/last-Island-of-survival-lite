using System.Collections.Generic;
using UnityEngine;

public class StopCar : MonoBehaviour
{
	private CarInfo carInfo;

	private List<WheelCollider> wheelColliders = new List<WheelCollider>();

	private void Start()
	{
		carInfo = GetComponent<CarInfo>();
		if (carInfo != null)
		{
			if (carInfo.frontLeftWheelCollider != null)
			{
				wheelColliders.Add(carInfo.frontLeftWheelCollider);
			}
			if (carInfo.frontRightWheelCollider != null)
			{
				wheelColliders.Add(carInfo.frontRightWheelCollider);
			}
			if (carInfo.rearLeftWheelCollider != null)
			{
				wheelColliders.Add(carInfo.rearLeftWheelCollider);
			}
			if (carInfo.rearRightWheelCollider != null)
			{
				wheelColliders.Add(carInfo.rearRightWheelCollider);
			}
		}
	}

	private void Update()
	{
		foreach (WheelCollider wheelCollider in wheelColliders)
		{
			Debug.LogError("ApplyBrakeTorque");
			wheelCollider.brakeTorque = 100000f;
		}
	}
}
