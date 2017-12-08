using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_TrackController : MonoBehaviour {
	[SerializeField] private Animator cloud;
	[SerializeField] private Animator track;
	[SerializeField] private SkeletonManager skeletonManager;
	[SerializeField] private AreaEffector2D effector2D;
	[HideInInspector] public bool trigger = false;
	private readonly float direction = -90f;

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
				skeletonManager.direction = SkeletonManager.arrow.RIGHT;
			}
			else {
				GetComponent<SpriteRenderer>().flipX = false;
				skeletonManager.direction = SkeletonManager.arrow.LEFT;
			}
		}
		else {
			effector2D.forceAngle = direction;
			skeletonManager.direction = SkeletonManager.arrow.FRONT;
		}
	}

	private void FixedUpdate() {
		if(skeletonManager.direction == SkeletonManager.arrow.FRONT) return;
		float rand = Random.Range(45f, 90f);
		if(skeletonManager.direction == SkeletonManager.arrow.LEFT) {
			effector2D.forceAngle = direction + rand;
			return;
		}
		if(skeletonManager.direction == SkeletonManager.arrow.RIGHT) {
			effector2D.forceAngle = direction - rand;
			return;
		}
	}
}
