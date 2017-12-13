using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_ItemController : MonoBehaviour {
	private Animator ani;
	private Skeleton_TrackController st;
	private void Start() {
		ani = GetComponent<Animator>();
		ani.SetInteger("show", Random.Range(0,5));
		st = GetComponentInParent<Skeleton_TrackController>();
		st.itemSpeed += setSpeed;
	}

	public void destroy() {
		st.itemSpeed -= setSpeed;
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		GameObject.Find("Canvas").SendMessage("getItem", GetComponent<Item>().item_st, SendMessageOptions.DontRequireReceiver);
		destroy();
	}

	private void setSpeed(float speed) {
		ani.speed = speed * 0.5f;
	}
}
