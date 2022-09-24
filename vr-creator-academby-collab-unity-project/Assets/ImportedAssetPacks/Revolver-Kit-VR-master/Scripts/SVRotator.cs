using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRotator : MonoBehaviour {
	public Vector3 axis;
	public float startRotation;
	public float endRotation;

	private bool animating;
	private bool isReturningToStart;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (this.animating) {
			float targetAngle = this.isReturningToStart ? startRotation : endRotation;
			Quaternion target = Quaternion.AngleAxis (targetAngle, axis);

			transform.localRotation = Quaternion.Lerp (transform.localRotation, target, isReturningToStart ? 0.1f : 0.25f);
			float curAngle = Quaternion.Angle (this.transform.localRotation, target);

			if (curAngle < 1) {
				if (this.isReturningToStart) {
					this.animating = false;
				} else {
					this.isReturningToStart = true;
				}
			}
		}
	}

	public void Rotate(float percent) {
		this.animating = true;
		this.isReturningToStart = false;
	}
}
