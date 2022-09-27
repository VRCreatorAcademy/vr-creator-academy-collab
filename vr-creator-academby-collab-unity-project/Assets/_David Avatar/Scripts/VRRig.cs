using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VRMap
{
	[SerializeField] Transform  vrTarget;
	[SerializeField] Transform  rigTarget;
	[SerializeField] Vector3    trackingPositionOffset;
	[SerializeField] Vector3	trackingRotationOffset;

	public void Map()
	{
		rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
		rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
	}
}


public class VRRig : MonoBehaviour
{
	[SerializeField] protected VRMap	head;
	[SerializeField] protected VRMap    rightHand;
	[SerializeField] protected VRMap    leftHand;

	[SerializeField] Transform	headConstraint;
	[SerializeField] Vector3	headBodyOffset;


	void Start()
	{
		headBodyOffset = transform.position - headConstraint.position;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		transform.position = headConstraint.position + headBodyOffset;

		head.Map();
		leftHand.Map();
		rightHand.Map();
	}
}
