using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Gyroscope : MonoBehaviour {
	public Vector3 gyroscope_rotation;
	
	void Awake() {
		Input.gyro.enabled = true;
	}
	
	void Update() {
		//gyroscope_rotation.x += -Input.gyro.rotationRate.x;
		//gyroscope_rotation.y += -Input.gyro.rotationRate.y;
		gyroscope_rotation.z += Input.gyro.rotationRate.z;
		Debug.Log(gyroscope_rotation);
	}
}
