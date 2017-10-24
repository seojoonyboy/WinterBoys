using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class downhill_player_coll : MonoBehaviour {
    private DownhillManager dM;
    private void Awake() {
        dM = GameObject.Find("Manager").GetComponent<DownhillManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Flag") {
            var flagComp = other.GetComponent<FlagController>();
            var anim = other.GetComponent<SkeletonAnimation>();
            anim.loop = false;

            if (flagComp.rayDir == FlagController.type.LEFT) {
                anim.AnimationName = "broken_left";
            }
            else if(flagComp.rayDir == FlagController.type.RIGHT) {
                anim.AnimationName = "broken_right";
            }
        }
    }
}
