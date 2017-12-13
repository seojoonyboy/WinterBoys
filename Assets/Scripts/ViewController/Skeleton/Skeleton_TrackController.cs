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
	private float gravityTime = 0f;
	private float rand = 45f;
	public delegate void speed(float speed);
	public speed itemSpeed;

	public void setSpeed(float speed) {
		cloud.speed = speed * 0.5f;
		track.speed = speed;
		if(itemSpeed == null) return;
		itemSpeed(speed);
	}

	public void triggerTurn() {
		int rand = Random.Range(0,3);
		skeletonManager.direction = (SkeletonManager.arrow)rand;
		track.SetInteger("direction", rand);
	}

	public void riseUpdate(float time) {
		if(skeletonManager.direction == SkeletonManager.arrow.FRONT) {
			effector2D.forceAngle = direction;
			return;
		}

		if(skeletonManager.direction == SkeletonManager.arrow.RIGHT) {
			effector2D.forceAngle = direction + rand;
		}
		else if(skeletonManager.direction == SkeletonManager.arrow.LEFT) {
			effector2D.forceAngle = direction - rand;
		}
		gravityTime += time;
		if(gravityTime < 1.0f) return;
		gravityTime -= 1.0f;
		rand = Random.Range(45f, 90f);
	}

	public void fallUpdate(float time) {
		if(skeletonManager.direction == SkeletonManager.arrow.FRONT) return;
		float rand = 45f;
		if(skeletonManager.direction == SkeletonManager.arrow.RIGHT) {
			effector2D.forceAngle = direction + rand;
			return;
		}
		if(skeletonManager.direction == SkeletonManager.arrow.LEFT) {
			effector2D.forceAngle = direction - rand;
			return;
		}
	}
}
