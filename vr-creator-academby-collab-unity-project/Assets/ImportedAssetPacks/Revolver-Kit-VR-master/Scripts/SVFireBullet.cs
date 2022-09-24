using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVFireBullet : MonoBehaviour {
	[HideInInspector]
	public GameObject bulletPrefab;
	[HideInInspector]
	public GameObject muzzleFlashPrefab;
	[HideInInspector]
	public GameObject dryFirePrefab;
	[HideInInspector]
	public LayerMask hitLayers = -1;
	[HideInInspector]
	public float muzzleVelocity = 1000f;  // meters per second

	public Transform bulletSpawnPoint;

	public void Fire() {
		GameObject muzzleFlash = Instantiate (muzzleFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
		muzzleFlash.transform.localScale = this.gameObject.transform.lossyScale;

		GameObject bullet = Instantiate (bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
		SVBullet bulletScript = bullet.GetComponent<SVBullet> ();
		bulletScript.bulletVelocity = muzzleVelocity;
		bulletScript.hitLayers = hitLayers;
		bullet.transform.localScale = this.gameObject.transform.lossyScale;
	}

	public void DryFire() {
		GameObject muzzleFlash = Instantiate (dryFirePrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
		muzzleFlash.transform.localScale = this.gameObject.transform.lossyScale;
	}
}
