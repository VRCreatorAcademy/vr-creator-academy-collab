using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVTargetDemo : SVShootable {
	private bool isRotating;

	private Quaternion upRotation = Quaternion.AngleAxis (-90, Vector3.right);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Hit(RaycastHit hit, SVBullet bullet, Vector3 rayDirection) {
		base.Hit (hit, bullet, rayDirection);

		if (!isRotating) {
			StartCoroutine (AnimateTargetTo (this.transform.localRotation * upRotation));
		}
	}

	private IEnumerator AnimateTargetTo(Quaternion targetRotation)
	{
		isRotating = true;
		Quaternion originalRotation = this.transform.localRotation;

		while ( Quaternion.Angle(this.transform.localRotation, targetRotation) > 0.01f) {
			
			this.transform.localRotation = Quaternion.Lerp (this.transform.localRotation, targetRotation, 0.15f);
			yield return null;
		}
		yield return new WaitForSeconds (5);

		while ( Quaternion.Angle(this.transform.localRotation, originalRotation) > 0.01f) {

			this.transform.localRotation = Quaternion.Lerp (this.transform.localRotation, originalRotation, 0.15f);
			yield return null;
		}
		isRotating = false;
	}
}
