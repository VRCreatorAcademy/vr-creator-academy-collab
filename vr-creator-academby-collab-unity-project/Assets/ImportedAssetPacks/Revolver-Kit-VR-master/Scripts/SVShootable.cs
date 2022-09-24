using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVShootable : MonoBehaviour {
	public AudioClip bulletHitSoundEffect;
	public float minPitch = 0.8f;
	public float maxPitch = 1.2f;
	public float volume = 1f;

	private AudioSource audioSource;

	public virtual void Hit(RaycastHit hit, SVBullet bullet, Vector3 rayDirection) {
		if (audioSource == null) {
			if (!GetComponent<AudioSource> ()) {
				audioSource = gameObject.AddComponent<AudioSource> ();
			} else {
				audioSource = GetComponent<AudioSource> ();
			}
			audioSource.clip = bulletHitSoundEffect;
		}

		if (GetComponent<Rigidbody> ()) {
			Rigidbody rb = GetComponent<Rigidbody> ();
			Vector3 impactForce = bullet.bulletMass * Mathf.Pow (bullet.bulletVelocity, 2) * rayDirection;
			rb.AddForceAtPosition (impactForce, hit.point);
		}

		if ( bulletHitSoundEffect != null ) {
			audioSource.pitch = Random.Range (minPitch, maxPitch);
			audioSource.volume = volume;
			audioSource.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
