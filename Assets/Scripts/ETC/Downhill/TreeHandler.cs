using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TreeHandler : MonoBehaviour {
    public enum TreeType { GREEN, BLUE }
    public TreeType type;
    private SkeletonAnimation anim;

    private DownhillManager dm;
    private BoardManager bm;

    private void Awake() {
        dm = GameObject.Find("Manager").GetComponent<DownhillManager>();
        bm = GameObject.Find("BoardHolder").GetComponent<BoardManager>();
    }

    private void Start() {
        anim = GetComponent<SkeletonAnimation>();
    }

    private void destroyItself() {
        Destroy(transform.parent.gameObject);
    }

    public void Play() {
        anim.loop = false;
        if (type == TreeType.BLUE) {
            anim.AnimationName = "blue_crash";
        }
        else if (type == TreeType.GREEN) {
            anim.AnimationName = "green_crash";
        }

        float time = anim.Skeleton.Data.FindAnimation("blue_crash").duration;
        Invoke("destroyItself", time);
    }
}
