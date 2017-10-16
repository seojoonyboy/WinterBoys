using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_PlayerController : MonoBehaviour {
	public Skeleton_Gyroscope gyro;
	
	// Update is called once per frame
	void Update () {
		transform.localRotation = Quaternion.Euler(gyro.gyroscope_rotation);
	}
}
