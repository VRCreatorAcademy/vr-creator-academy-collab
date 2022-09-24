using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRevolvingBarrel : MonoBehaviour {
	//------------------------
	// Public Variables
	//------------------------

	/* Move these settings into SVRevolver */
	[HideInInspector]
	public int numberOfChambers = 6;

	[HideInInspector]
	public float rotationSpeed = 0.1f;
	[HideInInspector]
	public AudioClip revolverOpenClip;
	[HideInInspector]
	public AudioClip revolverClickClip;
	[HideInInspector]
	public AudioClip revolverCloseClip;
	[HideInInspector]
	public Vector3 openPosition;
	[HideInInspector]
	public Vector3 closedPosition;
	[HideInInspector]
	public float openSpeed = 1000f;
	[HideInInspector]
	public GameObject bulletPrefab;

	[HideInInspector]
	public float minRotationSpeedForReloading = 200f;
	[HideInInspector]
	public float openDurationReloadNeeded = 3f;
	[HideInInspector]
	public float openDurationReloadNotNeeded = 0.65f;
	[HideInInspector]
	public float minStayOpenDuration = 0.65f;

	[HideInInspector]
	public float shellEjectionForce = 1f;

	/* Keep these settings here */
	public Collider[] gunColliders;
	public float bulletPlacementRadius = 0.0102f;
	public float bulletForwardAxisOffset = 0.0092f;

	/* These are set by a script */
	[HideInInspector]
	public SVRevolver revolverParent;

	[HideInInspector]
	public bool isOpen = false;



	//------------------------
	// Private Variables
	//------------------------

	// Bullet Status
	private GameObject[] bullets;
	private bool[] bulletSpent;

	// Auto-Close Info
	private float openStartTime;
	private float stayOpenDuration;

	// Rotation while open
	private float revolverClickAngle = 0f;
	private int currentRevolvIndex = 0;
	private int shellEjectionIndex = -1;

	// Rotation while whooting
	private Quaternion rotationTarget;
	private bool isAnimating = false;

	// Sounds
	private Coroutine positionCoroutine;
	private AudioSource clickAS;
	private AudioSource openCloseAS;

	// States while open
	private bool shouldSpin = false;
	private bool canEjectShells = false;

	// Use this for initialization
	void Start () {
		this.closedPosition = this.transform.localPosition;
		bullets = new GameObject[numberOfChambers];
		bulletSpent = new bool[numberOfChambers];
		ReloadBarrel ();

		clickAS = gameObject.AddComponent<AudioSource> ();
		clickAS.clip = revolverClickClip;
		clickAS.volume = 0.2f;

		openCloseAS = gameObject.AddComponent<AudioSource> ();
	}

	void ReloadBarrel() {
		foreach (GameObject bullet in bullets) {
			if (bullet != null) {
				Destroy (bullet);
			}
		}

		for (int i = 0; i < numberOfChambers; i++) {
			GameObject bullet = Instantiate (bulletPrefab, this.transform);
			float degrees = AngleForIndex (i);
			float x = bulletPlacementRadius * Mathf.Cos (degrees * Mathf.Deg2Rad);  // we're rotated so x == z
			float y = bulletPlacementRadius * Mathf.Sin (degrees * Mathf.Deg2Rad);

			bullet.transform.localPosition = new Vector3 (bulletForwardAxisOffset, x, y);
			Collider col = bullet.GetComponent<Collider> ();
			foreach (Collider col2 in gunColliders) {
				Physics.IgnoreCollision (col, col2);
			}
			col.enabled = false;
			bullet.GetComponent<Collider>().enabled = false;

			bulletSpent [i] = false;
			bullets [i] = bullet;
		}
	}

	// Update is called once per frame
	void Update () {
		if (this.shouldSpin) {
			transform.localRotation = transform.localRotation * Quaternion.AngleAxis (Time.deltaTime * -Mathf.Max(openSpeed, minRotationSpeedForReloading), Vector3.right);

			int index = (int)Mathf.Floor ((Time.time - this.openStartTime) / 0.135f);
			index = index % this.numberOfChambers;
			if (shellEjectionIndex != index && canEjectShells) {
				EjectShell (index);
			}
			shellEjectionIndex = index;

			if (canEjectShells) {
				float angle = BarrelAngle ();
				if (Mathf.Abs(angle - this.revolverClickAngle) > 25) {
					this.revolverClickAngle = angle;
					clickAS.Play ();
				}
			}

			openSpeed *= 0.995f;  // friction
		} else if (this.isAnimating) {
			transform.localRotation = Quaternion.Lerp (transform.localRotation, this.rotationTarget, rotationSpeed);
			float curAngle = Quaternion.Angle (this.transform.localRotation, this.rotationTarget);
			if (curAngle < 1) {
				this.isAnimating = false;
			}
		}

		if (this.isOpen && Time.time > (this.openStartTime + this.stayOpenDuration)) {
			this.CloseFromReload ();
		}
	}

	public void Revolve() {
		bulletSpent [currentRevolvIndex] = true;
		this.rotationTarget = RotationForIndex(++currentRevolvIndex);
		this.isAnimating = true;

		if (currentRevolvIndex > numberOfChambers - 1) {
			currentRevolvIndex = 0;
		}
	}

	public void OpenForReload(bool isRight) {
		if (isOpen) {
			return;
		}

		this.openStartTime = Time.time;
		this.stayOpenDuration = HasShotsToReload () ? openDurationReloadNeeded : openDurationReloadNotNeeded;
		if (this.positionCoroutine != null) {
			StopCoroutine (this.positionCoroutine);
		}
		this.positionCoroutine = StartCoroutine(AnimateBarrelTo (isRight ? this.openPosition : -this.openPosition));

		openSpeed = 800f;
		shouldSpin = true;
		isOpen = true;

		openCloseAS.clip = this.revolverOpenClip;
		openCloseAS.Play ();

	}

	public bool CloseFromReload() {
		if (!isOpen || Time.time - this.openStartTime < this.minStayOpenDuration) {
			return false;
		}

		// Reload our shots
		revolverParent.Reload();

		// Close the barrel
		if (this.positionCoroutine != null) {
			StopCoroutine (this.positionCoroutine);
		}
		this.positionCoroutine = StartCoroutine(AnimateBarrelTo (this.closedPosition));
		isOpen = false;
		canEjectShells = false;
		ReloadBarrel ();

		openCloseAS.clip = this.revolverCloseClip;
		openCloseAS.Play ();

		return true;
	}

	private void EjectShell(int shellIndex) {
		if ( bullets[shellIndex] != null && bulletSpent [shellIndex] == true) {
			GameObject bullet = bullets [shellIndex];
			bullet.transform.parent = null;
			bullet.GetComponent<Collider> ().enabled = true;

			Rigidbody rb = bullet.GetComponent<Rigidbody> ();
			rb.isKinematic = false;
			rb.AddForce (shellEjectionForce * bullet.transform.right, ForceMode.Force);

			bullet.GetComponent<SVDestroyAfterTime> ().StartTimer ();

			bullets [shellIndex] = null;
		}
	}

	// Animation
	private IEnumerator AnimateBarrelTo(Vector3 position)
	{
		while ( Mathf.Abs((this.transform.localPosition - position).magnitude) > 0.001f)
		{
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition, position, 0.175f);
			yield return null;
		}
		this.transform.localPosition = position;
		if (isOpen) {
			canEjectShells = true;
		} else {
			shouldSpin = false;
			this.isAnimating = true; // forces us to spin back to the correct location
		}
	}

	// Helpers
	bool HasShotsToReload() {
		foreach (bool spent in this.bulletSpent) {
			if (spent == true) {
				return true;
			}
		}
		return false;
	}
	// Math Helpers
	Quaternion RotationForIndex(int curIndex) {
		float angle = AngleForIndex (curIndex);
		return Quaternion.AngleAxis (angle, Vector3.left);
	}

	float AngleForIndex(int curIndex) {
		return 360.0f * ((float)curIndex / (float)numberOfChambers);
	}

	int IndexForAngle(float angle) {
		return (int)Mathf.Floor((angle / 360.0f) * (float)numberOfChambers);
	}

	float BarrelAngle() {
		Vector3 forwardVector = transform.localRotation * Vector3.forward;
		float angle = Mathf.Atan2 (forwardVector.y, forwardVector.z) * Mathf.Rad2Deg;
		if (angle < 0) {
			angle += 360.0f;
		}
		return angle;
	}
}
