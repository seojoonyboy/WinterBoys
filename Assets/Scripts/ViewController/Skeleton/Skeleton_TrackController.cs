using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_TrackController : MonoBehaviour {
	[SerializeField] private Animator cloud;
	[SerializeField] private Animator track;
	[SerializeField] private SkeletonManager skeletonManager;
	[SerializeField] private AreaEffector2D effector2D;
	[HideInInspector] public bool trigger = false;

	public void setSpeed(float speed) {
		cloud.speed = speed * 0.5f;
		track.speed = speed;
	}

	public void triggerTurn() {
		trigger = !trigger;
		track.SetBool("turn", trigger);
		if(trigger) {
			int rand = Random.Range(0,2);
			if(rand == 1) {
				GetComponent<SpriteRenderer>().flipX = true;
				effector2D.forceAngle = -10f;
			}
			else {
				GetComponent<SpriteRenderer>().flipX = false;
				effector2D.forceAngle = -170f;
			}
		}
		else {
			effector2D.forceAngle = -90f;
		}
	}
}
