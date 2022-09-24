using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRevolver : MonoBehaviour {
	
	//------------------------
	// Variables
	//------------------------

	[Space(15)]
	[Header("Firing Settings")]
	public GameObject bulletPrefab;
	public float muzzleVelocity = 200f;
	public LayerMask shotHitLayers = -1; 
	public int maxBullets = 6;
	public float kickForce = 30;
	public int numberOfChambers = 6;
	public float barrelRotationSpeed = 0.1f;

	[Space(15)]
	[Header("Reload Settings")]

	public GameObject shellPrefab;
	public float barrelOpenSpeed = 1000f;
	public float minRotationSpeedForReloading = 200f;
	public float openDurationReloadNeeded = 3f;
	public float openDurationReloadNotNeeded = 0.65f;
	public float minStayOpenDuration = 0.65f;
	public float shellEjectionForce = 1f;

	public Vector3 barrelOpenPosition;
	public Vector3 barrelClosedPosition;

	//[Space(15)]
	//[Header("Grab Settings")]
	//public float grabDistance = 1;
	//public float grabFlyTime = .1f;
	//public bool shouldFly = true;

	[Tooltip("The thickness of the outline effect")]
	public float outlineThickness = 1f;
	public Color outlineColor;

	[Space(15)]
	[Header("Sounds and FX")]
	public AudioClip revolverOpenClip;
	public AudioClip revolverClickClip;
	public AudioClip revolverCloseClip;
	public GameObject muzzleFlashPrefab;
	public GameObject dryFirePrefab;

	[Space(15)]
	[Header("Gun Components")]
	public SVRotator trigger;
	public SVRotator hammer;
	public SVRevolvingBarrel revolvingBarrel;

	//------------------------
	// Private Variables
	//------------------------

	private SVGrabbable grabComponent;
	private SVFireBullet fireBulletComponent;
	private SVControllerInput input;
	private int curBullets = 6;

	private SVLinearAcceleration linearAccelerationTracker;

	// Use this for initialization
	void Start () {
		this.grabComponent = GetComponent<SVGrabbable> ();
		this.fireBulletComponent = GetComponent<SVFireBullet> ();
		this.input = this.gameObject.GetComponent<SVControllerInput> ();

		linearAccelerationTracker = new SVLinearAcceleration ();
		revolvingBarrel.revolverParent = this;

		this.SetupComponents ();
    }

    // Update is called once per frame
    void Update()
    {
		//FIRE is now performed via XR INTERACTABLE ON ACTIVATE EVENT (See Revolver Parent XR Grab Interactable)
		
		//Add Input Action Property for controller button press to trigger ToggleBarrel();

        
		//ORIGINAL SV CODE
        //if (input.activeController != SVControllerType.SVController_None) {
        //	if (input.GetTriggerButtonPressed(input.activeController)) {
        //		Fire();
        //	}
        //	if (!this.revolvingBarrel.isOpen && input.GetOpenBarrelPressed (input.activeController)) {
        //		ToggleBarrel ();
        //	} else if (this.revolvingBarrel.isOpen && input.GetCloseBarrelPressed (input.activeController)) {
        //		ToggleBarrel ();
        //	}
        //}
    }

    void FixedUpdate() 
	{
		// ORIGINAL SV CODE Addressing Weapon Physics:  UnComment to learn more!

		//if (input.activeController != SVControllerType.SVController_None && input.openWithPhysics && grabComponent.inHand) {
		//	Vector3 gunAcceleration;
		//	Vector3 gunVelocity = linearAccelerationTracker.LinearAcceleration (out gunAcceleration, this.transform.localPosition, 8);
		//	if (gunVelocity == Vector3.zero)
		//		return;
			
		//	gunAcceleration = this.transform.InverseTransformDirection (gunAcceleration);
		//	gunVelocity = this.transform.InverseTransformDirection (gunVelocity);
		//	float openAcceleration = (this.curBullets == 0) ? input.openEmptyAcceleration : input.openAcceleration;
		//	bool isDecelerating = !Sign(gunAcceleration.x, gunVelocity.x);
		//	bool controllerInRightHand = input.activeController == SVControllerType.SVController_Right;
		//	if (isDecelerating) {
		//		if (Sign (gunAcceleration.x, (controllerInRightHand ? openAcceleration : -openAcceleration)) && Mathf.Abs (gunAcceleration.x) > Mathf.Abs (openAcceleration)) {
		//			revolvingBarrel.OpenForReload (controllerInRightHand);
		//		} else if (Sign (gunAcceleration.x, (controllerInRightHand ? input.closeAcceleration : -input.closeAcceleration)) && Mathf.Abs (gunAcceleration.x) > Mathf.Abs (input.closeAcceleration)) {
		//			revolvingBarrel.CloseFromReload ();
		//		}
		//	}
		//}
	}

	// Actions
	private void Fire() {
		if (revolvingBarrel.isOpen || !grabComponent.inHand) {
			return;
		}

		if (curBullets > 0) {
			curBullets--;
			fireBulletComponent.Fire ();
			grabComponent.EditGripForKick (kickForce);
			input.RumbleActiveController (0.25f);
			trigger.Rotate (1.0f);
			hammer.Rotate (1.0f);
			revolvingBarrel.Revolve ();
		} else {
			fireBulletComponent.DryFire ();
			input.RumbleActiveController (0.05f);
			trigger.Rotate (1.0f);
			hammer.Rotate (1.0f);
			revolvingBarrel.Revolve ();
		}

	}

	private void ToggleBarrel() {
		if (!grabComponent.inHand)
			return;
		
		if (revolvingBarrel.isOpen) {
			revolvingBarrel.CloseFromReload ();
		} else {
			revolvingBarrel.OpenForReload (input.activeController == SVControllerType.SVController_Right);
		}
	}

	public void Reload() {
		curBullets = maxBullets;
	}	

	// Helpers
	bool Sign(float a, float b) {
		return (a > 0 && b > 0 ||
		    a <= 0 && b <= 0);
	}

	void OnValidate()
	{
		if (this.grabComponent == null) {
			this.grabComponent = GetComponent<SVGrabbable> ();
		}

		if (this.fireBulletComponent == null) {
			this.fireBulletComponent = GetComponent<SVFireBullet> ();
		}

		SetupComponents ();
	}

	private void SetupComponents() {
		/* Fire Bullet Component */
		this.fireBulletComponent.bulletPrefab = this.bulletPrefab;
		this.fireBulletComponent.muzzleFlashPrefab = this.muzzleFlashPrefab;
		this.fireBulletComponent.dryFirePrefab = this.dryFirePrefab;
		this.fireBulletComponent.muzzleVelocity = this.muzzleVelocity;
		this.fireBulletComponent.hitLayers = this.shotHitLayers;

		/* Grab Component */
		//this.grabComponent.grabDistance = this.grabDistance;
		//this.grabComponent.grabFlyTime = this.grabFlyTime;
		//this.grabComponent.shouldFly = this.shouldFly;

		/* Outline */
		//if (this.GetComponent<SVOutline> ()) {
		//	SVOutline outline = this.GetComponent<SVOutline> ();
		//	outline.outlineThickness = this.outlineThickness;
		//	outline.outlineColor = this.outlineColor;
		//}

		/* Revolver Barrel */
		this.revolvingBarrel.numberOfChambers = this.numberOfChambers;
		this.revolvingBarrel.rotationSpeed = this.barrelRotationSpeed;
		this.revolvingBarrel.revolverOpenClip = this.revolverCloseClip;
		this.revolvingBarrel.revolverClickClip = this.revolverClickClip;
		this.revolvingBarrel.revolverCloseClip = this.revolverCloseClip;
		this.revolvingBarrel.openPosition = this.barrelOpenPosition;
		this.revolvingBarrel.closedPosition = this.barrelClosedPosition;
		this.revolvingBarrel.openSpeed = this.barrelOpenSpeed;
		this.revolvingBarrel.bulletPrefab = this.shellPrefab;
		this.revolvingBarrel.minRotationSpeedForReloading = this.minRotationSpeedForReloading;
		this.revolvingBarrel.openDurationReloadNeeded = this.openDurationReloadNeeded;
		this.revolvingBarrel.openDurationReloadNotNeeded = this.openDurationReloadNotNeeded;
		this.revolvingBarrel.minStayOpenDuration = this.minStayOpenDuration;
		this.revolvingBarrel.shellEjectionForce = this.shellEjectionForce;
	}
}
