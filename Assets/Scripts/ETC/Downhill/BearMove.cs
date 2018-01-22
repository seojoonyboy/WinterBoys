using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class BearMove : MonoBehaviour {
    //minTime ~ maxTime 초마다 상태 변경 (상태 : 광폭, 일반)
    public float minTime = 5;
    public float maxTime = 10;

    private float time;
    private float randTime;

    private SkeletonAnimation anim;
    private int[] marks = { -1, 1 };
    private int mark = 1;
    private float speed = 0.005f;
    private bool isLeftDir = false;

    private void Start() {
        makeRandTime();
        anim = GetComponent<SkeletonAnimation>();
        mark = marks.Random();

        setRandDir();
    }

    private void setRandDir() {
        int[] arr = { -1, 1 };
        int randNum = arr.Random();

        if (randNum == -1) {
            anim.initialFlipX = true;
            anim.Initialize(true);
            isLeftDir = false;
        }
        else {
            isLeftDir = true;
        }
    }

    private void FixedUpdate() {
        if (isLeftDir) {
            transform.Translate(Vector2.left * speed);
        }
        else {
            transform.Translate(Vector2.right * speed);
        }

        time -= Time.deltaTime;
        if(time < 0) {
            makeRandTime();
            changeAnim();
        }
    }

    private void makeRandTime() {
        randTime = Random.Range(minTime, maxTime);
        time = randTime;
    }

    private void changeAnim() {
        if(anim.AnimationName == "run") {
            anim.AnimationName = "walk";
            speed = 0.005f;
        }
        else if(anim.AnimationName == "walk") {
            anim.AnimationName = "run";
            speed = 0.01f;
        }
    }
}
