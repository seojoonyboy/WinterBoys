using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class reloadTest : MonoBehaviour {
    SkeletonAnimation anim;
    private void Start() {
        anim = GetComponent<SkeletonAnimation>();
    }

    public void FlipX() {
        anim.initialFlipX = true;
        anim.Initialize(true);
    }
}
