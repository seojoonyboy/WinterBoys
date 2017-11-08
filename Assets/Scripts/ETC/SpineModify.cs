using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
public class SpineModify : MonoBehaviour {
    private SkeletonAnimation skeletonAnim;
    private Skeleton skel;
    private Bone bone;
    private void Awake() {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        skel = skeletonAnim.Skeleton;

        bone = skel.FindBone("character");

        skeletonAnim.UpdateLocal += UpdateBones;
    }

    void UpdateBones(ISkeletonAnimation s) {
        bone.rotation = 90f;
    }
}
