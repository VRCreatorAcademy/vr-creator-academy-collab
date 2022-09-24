using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVControllerManager {
	public static bool leftControllerActive;
	public static bool rightControllerActive;

	public static SVGrabbable nearestGrabbableToRightController = null;
	public static SVGrabbable nearestGrabbableToLeftController = null;

	public static float distanceToRightController = 10000f;
	public static float distanceToLeftController = 10000f;
}
