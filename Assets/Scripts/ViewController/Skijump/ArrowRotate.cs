using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotate : MonoBehaviour {
    public delegate void EndRotating();
    public static event EndRotating OnRotatingEnd;

    private Rigidbody2D rb;
    private SkiJumpManager sm;
    public float rotateAmount = 1;
    public bool canRotate = true;

    private Vector3 rotDir = Vector3.forward;
    private float time = 0;
    private int rotCnt = 0;
    private void Awake() {
        sm = SkiJumpManager.Instance;
    }

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
        OnRotatingEnd();
        canRotate = false;
    }
}
