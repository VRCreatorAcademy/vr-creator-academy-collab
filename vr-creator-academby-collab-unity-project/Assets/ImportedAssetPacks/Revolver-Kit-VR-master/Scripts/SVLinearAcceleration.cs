using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVLinearAcceleration {
	
	private Vector3[] positionRegister;
	private float[] posTimeRegister;
	private int positionSamplesTaken = 0;

	public Vector3 LinearAcceleration(out Vector3 vector, Vector3 position, int samples){
		Vector3 averageSpeedChange = Vector3.zero;
		Vector3 averageVelocity = Vector3.zero;
		vector = Vector3.zero;
		Vector3 deltaDistance;
		float deltaTime;
		Vector3 speedA = Vector3.zero;
		Vector3 speedB = Vector3.zero;

		//Clamp sample amount. In order to calculate acceleration we need at least 2 changes
		//in speed, so we need at least 3 position samples.
		if(samples < 3){

			samples = 3;
		}

		//Initialize
		if(positionRegister == null) {
			positionRegister = new Vector3[samples];
			posTimeRegister = new float[samples];
		}

		//Fill the position and time sample array and shift the location in the array to the left
		//each time a new sample is taken. This way index 0 will always hold the oldest sample and the
		//highest index will always hold the newest sample. 
		for(int i = 0; i < positionRegister.Length - 1; i++){

			positionRegister[i] = positionRegister[i+1];
			posTimeRegister[i] = posTimeRegister[i+1];
		}
		positionRegister[positionRegister.Length - 1] = position;
		posTimeRegister[posTimeRegister.Length - 1] = Time.time;

		positionSamplesTaken++;

		//The output acceleration can only be calculated if enough samples are taken.
		if(positionSamplesTaken >= samples){

			//Calculate average speed change.
			for(int i = 0; i < positionRegister.Length - 2; i++){

				deltaDistance = positionRegister[i+1] - positionRegister[i];
				deltaTime = posTimeRegister[i+1] - posTimeRegister[i];

				//If deltaTime is 0, the output is invalid.
				if(deltaTime == 0){

					return Vector3.zero;
				}

				speedA = deltaDistance / deltaTime;
				deltaDistance = positionRegister[i+2] - positionRegister[i+1];
				deltaTime = posTimeRegister[i+2] - posTimeRegister[i+1];

				if(deltaTime == 0){

					return  Vector3.zero;
				}

				speedB = deltaDistance / deltaTime;

				//This is the accumulated speed change at this stage, not the average yet.
				averageSpeedChange += speedB - speedA;
				averageVelocity += speedB;

			}

			//Now this is the average speed change.
			averageSpeedChange /= positionRegister.Length - 2; 
			averageVelocity /= positionRegister.Length - 2; 


			//Get the total time difference.
			float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];			

			//Now calculate the acceleration, which is an average over the amount of samples taken.
			vector = averageSpeedChange / deltaTimeTotal;

			//Vector3 curVelocity = (speedA + speedB) / 2.0f;

			return averageVelocity;		
		}
		else {
			return Vector3.zero;
		}
	}
}
