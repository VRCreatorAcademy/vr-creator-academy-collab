using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVBullet : MonoBehaviour {
	//------------------------
	// Variables
	//------------------------
	public float bulletVelocity;
	public float bulletDamage = 1;
	public float bulletLifetime = 3;
	public float bulletMass = .015f;

	public LayerMask hitLayers = -1;
	private float startTime;
	private bool hasHit = false;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.hasHit) {
			Destroy (gameObject);
			return;
		}

		float distanceTraveled = Time.fixedDeltaTime * bulletVelocity;

		RaycastHit hitOut; 
		bool hit = Physics.Raycast (this.transform.position, this.transform.TransformDirection (Vector3.forward), out hitOut, distanceTraveled, hitLayers);

		if (hit) {
			if (hitOut.rigidbody != null) {
				HitWithObject (hitOut.rigidbody.gameObject, hitOut);
			} else if (hitOut.collider != null) {
				HitWithObject (hitOut.collider.gameObject, hitOut);
			}

			// Wait til the next frame to destroy to ensure the trail shows up
			this.transform.position += this.transform.TransformDirection (Vector3.forward) * hitOut.distance;
			this.hasHit = true;
		} else {
			this.transform.position += this.transform.TransformDirection (Vector3.forward) * distanceTraveled;
			if (Time.time - this.startTime > bulletLifetime) {
				Destroy (gameObject);
			}
		}
	}

	private void HitWithObject(GameObject hitObject, RaycastHit hit) {
		if (hitObject.GetComponent<SVShootable> ()) {
			SVShootable shootable = hitObject.GetComponent<SVShootable> ();
			shootable.Hit (hit, this, this.transform.TransformDirection (Vector3.forward));
		}
	}
}
