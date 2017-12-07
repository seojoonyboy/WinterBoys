using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Skeleton_PlayerController : MonoBehaviour {
	[SerializeField] private Transform background;
	[SerializeField] private Rigidbody2D rigid;
	[SerializeField] private SkeletonAnimation character;
	private readonly string[] characterList = {"blue_stand","red_stand","yellow_stand"};
	
	private void Start() {
		setCharacter();
	}

	private void setCharacter() {
		int num = GameManager.Instance.character;
		character.AnimationState.SetAnimation(0, characterList[num], true);	
	}


	// Update is called once per frame
	private void Update () {
		rigid.AddForce(transform.right * Input.acceleration.x * 100f);
		var dir = background.transform.position - transform.position;
		var angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
