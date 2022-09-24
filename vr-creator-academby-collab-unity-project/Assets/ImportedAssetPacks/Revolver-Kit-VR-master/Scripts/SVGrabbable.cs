using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class SVGrabbable : MonoBehaviour {

    //------------------------
    // Variables
    //------------------------
    public float grabDistance = 1;
	public float grabFlyTime = 2f;
	public bool shouldFly = true;

	[HideInInspector]
	public bool inHand = false;

    private SVOutline outlineComponent;

	private SVControllerInput input;
	private float grabStartTime;
	private Vector3 grabStartPosition;

	private float kickStartTime;
	private float maxKickDuration = 1f;
	private float minKickAngle = .25f;
	private bool isKicking = false;
	private Quaternion kickOffset;

	private Quaternion grabStartRotation;
		
    //------------------------
    // Init
    //------------------------
    void Start () {
        outlineComponent = this.gameObject.GetComponent<SVOutline>();
		this.input = this.gameObject.GetComponent<SVControllerInput> ();
    }

    //------------------------
    // Update
    //------------------------
    void Update() {
		if (this.input.activeController == SVControllerType.SVController_None) {
            this.UngrabbedUpdate();
        } else {
            this.GrabbedUpdate();
        }
    }

    private void UngrabbedUpdate() {
		this.inHand = false;

        float distanceToLeftHand = 1000;
		if (input.LeftControllerIsConnected) {
			distanceToLeftHand = (this.transform.position - input.LeftControllerPosition).magnitude;
        }

		float distanceToRightHand = 1000;
		if (input.RightControllerIsConnected) {
			distanceToRightHand = (this.transform.position - input.RightControllerPosition).magnitude;
		}

        if (grabDistance > distanceToLeftHand ||
            grabDistance > distanceToRightHand) {
            float distance = Mathf.Min(distanceToLeftHand, distanceToRightHand);
            if (this.outlineComponent) {
                float distanceForHighlight = grabDistance / 4f;
                float highlight = Mathf.Max(0, Mathf.Min(1, (grabDistance - distance) / distanceForHighlight));
                outlineComponent.outlineActive = highlight;
            }

			// order them based on distance
			SVControllerType firstController = SVControllerType.SVController_None;
			SVControllerType secondController = SVControllerType.SVController_None;

			if (distanceToLeftHand < distanceToRightHand) {
				if (SVControllerManager.nearestGrabbableToLeftController == this)
					firstController = SVControllerType.SVController_Left;
				
				if (SVControllerManager.nearestGrabbableToRightController == this)
					secondController = SVControllerType.SVController_Right;	
			} else {
				if (SVControllerManager.nearestGrabbableToRightController == this)
					firstController = SVControllerType.SVController_Right;	

				if (SVControllerManager.nearestGrabbableToLeftController == this)
					secondController = SVControllerType.SVController_Left;	
			}

			TrySetActiveController (firstController);
			TrySetActiveController (secondController);

			// Update grabbable distance so we always grab the nearest revolver
			if (distanceToLeftHand < SVControllerManager.distanceToLeftController || 
				SVControllerManager.nearestGrabbableToLeftController == null ||
				SVControllerManager.nearestGrabbableToLeftController == this) {
				SVControllerManager.nearestGrabbableToLeftController = this;
				SVControllerManager.distanceToLeftController = distanceToLeftHand;
			}

			if (distanceToRightHand < SVControllerManager.distanceToRightController || 
				SVControllerManager.nearestGrabbableToRightController == null ||
				SVControllerManager.nearestGrabbableToRightController == this) {
				SVControllerManager.nearestGrabbableToRightController = this;
				SVControllerManager.distanceToRightController = distanceToRightHand;
			}
				
        } else {
            outlineComponent.outlineActive = 0;
        }
    }

    private void GrabbedUpdate() {
		if (input.gripAutoHolds) {
			if (input.GetReleaseGripButtonPressed (input.activeController)) {
				this.ClearActiveController ();
				return;
			}
		} else if (!input.GetGripButtonDown(input.activeController)) {
			this.ClearActiveController ();
			return;
        }

		float percComplete = (Time.time - this.grabStartTime) / this.grabFlyTime;
		if (percComplete < 1 && this.shouldFly) {
			this.inHand = false;
			transform.position = Vector3.Lerp (this.grabStartPosition, this.input.PositionForController(this.input.activeController), percComplete);
			transform.rotation = Quaternion.Lerp (this.grabStartRotation, this.input.RotationForController(this.input.activeController), percComplete);
		} else if (isKicking) {
			this.kickOffset = Quaternion.Lerp (this.kickOffset, Quaternion.identity, 0.05f);
			this.transform.SetPositionAndRotation (this.input.PositionForController(this.input.activeController), this.input.RotationForController(this.input.activeController) * this.kickOffset);

			float curAngle = Quaternion.Angle (this.kickOffset, Quaternion.identity);
			if (curAngle < minKickAngle || Time.time - this.kickStartTime > maxKickDuration) {
				this.isKicking = false;
			}
		} else {
			this.inHand = true;
			this.transform.SetPositionAndRotation(this.input.PositionForController(this.input.activeController), this.input.RotationForController(this.input.activeController));
		}
    }

	//------------------------
	// Kick
	//------------------------
	public void EditGripForKick(float kickForce) {
		kickStartTime = Time.time;
		isKicking = true;

		Quaternion upRotation = Quaternion.AngleAxis (-90, Vector3.right);
		this.kickOffset = Quaternion.RotateTowards (Quaternion.identity, upRotation, kickForce);
	}

    //------------------------
    // State Changes
    //------------------------
	private void TrySetActiveController(SVControllerType controller) {
		if (this.input.activeController != SVControllerType.SVController_None ||
			controller == SVControllerType.SVController_None)
			return;	

		if (input.gripAutoHolds) {
			if (!input.GetGripButtonPressed(controller)) {
				return;
			}
		} else {
			if (!input.GetGripButtonDown (controller)) {
				return;
			}
		}

		if (this.input.SetActiveController (controller)) {
			this.grabStartTime = Time.time;
			this.grabStartPosition = this.gameObject.transform.position;
			this.grabStartRotation = this.gameObject.transform.rotation;
			outlineComponent.outlineActive = 0;
		
			Rigidbody rigidbody = this.GetComponent<Rigidbody> ();
			rigidbody.isKinematic = true;

			// hide the controller model
			this.input.HideActiveModel();
		}
    }

	private void ClearActiveController() {
		Rigidbody rigidbody = this.GetComponent<Rigidbody> ();
		rigidbody.isKinematic = false;
		rigidbody.velocity = this.input.ActiveControllerVelocity ();
		rigidbody.angularVelocity = this.input.ActiveControllerAngularVelocity ();

		// Show the render model
		this.input.ShowActiveModel();
		this.input.ClearActiveController ();
	}
}
