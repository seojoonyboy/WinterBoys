using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_PlayerController : MonoBehaviour {
	[SerializeField] Skeleton_Gyroscope gyro;
	[SerializeField] private Transform background;
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D>().AddForce(transform.right * -gyro.gyroscope_rotation.z);
		var dir = background.transform.position - transform.position;
		var angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
