using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_PlayerController : MonoBehaviour {
	[SerializeField] private Transform background;
	[SerializeField] private Rigidbody2D rigid;
	
	// Update is called once per frame
	void Update () {
		rigid.AddForce(transform.right * Input.acceleration.x * 100f);
		var dir = background.transform.position - transform.position;
		var angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
