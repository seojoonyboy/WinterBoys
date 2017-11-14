using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class ArrowRotate : MonoBehaviour {
    private Rigidbody2D rb;
    public float rotateAmount = 1;
    public bool canRotate = true;

    private Vector3 rotDir = Vector3.forward;
    private int rotCnt = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (canRotate) {
            if(transform.eulerAngles.z >= 80) {
                rotDir = Vector3.forward * -1;
            }
            if (transform.eulerAngles.z < 10) {
                rotDir = Vector3.forward;
                rotCnt++;
            }
            transform.Rotate(rotDir * 5f);

            if(rotCnt > 4) {
                stopRotating();
            }
        }
    }

    public void stopRotating() {
        EventManager.Instance.TriggerEvent(new SkiJump_ArrowRotEndEvent());
        canRotate = false;
    }
}
