using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
	[SerializeField] XRController		leftController;
	[SerializeField] XRController		rightController;
	private Animator					animator;

	List<UnityEngine.XR.InputDevice> leftHandDevices = new List<UnityEngine.XR.InputDevice>();
	List<UnityEngine.XR.InputDevice> rightHandDevices = new List<UnityEngine.XR.InputDevice>();


	void Start()
	{
		animator = GetComponentInChildren<Animator>();

	}


	// Update is called once per frame
	void Update()
	{
		GetDevices();
		LeftGrip();
		RightGrip();
	}

	void GetDevices()
	{
		if(rightHandDevices.Count == 0)
		{
			var leftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
			InputDevices.GetDevicesWithCharacteristics(leftCharacteristics, leftHandDevices);

			if(leftHandDevices.Count == 1)
			{
				UnityEngine.XR.InputDevice device = leftHandDevices[0];
				Debug.Log(string.Format("Left Hand Device name '{0}' with role '{1}'", device.name, device.characteristics.ToString()));
			}
		}

		if(rightHandDevices.Count == 0)
		{
			var rightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right| InputDeviceCharacteristics.Controller;
			InputDevices.GetDevicesWithCharacteristics(rightCharacteristics, rightHandDevices);

			if(rightHandDevices.Count == 1)
			{
				UnityEngine.XR.InputDevice device = rightHandDevices[0];
				Debug.Log(string.Format("Right Hand Device name '{0}' with role '{1}'", device.name, device.characteristics.ToString()));
			}
		}
	}


public void LeftGrip()
	{
		if(leftHandDevices.Count!=0)
		{
			float gripValue;
			leftHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out gripValue);
			Debug.Log($"Grip = {gripValue}");
			animator.SetFloat("Left Grip", gripValue);
		}
	}


	public void RightGrip()
	{
		if(rightHandDevices.Count != 0)
		{
			float gripValue;
			rightHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out gripValue);
			Debug.Log($"Grip = {gripValue}");
			animator.SetFloat("Right Grip", gripValue);
		}
	}
}
