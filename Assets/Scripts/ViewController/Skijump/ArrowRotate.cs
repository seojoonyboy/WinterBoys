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

    private void Awake() {
        sm = SkiJumpManager.Instance;
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        //transform.Rotate(Vector3.forward);
        if (canRotate) {
            transform.Rotate(Vector3.forward);

            if (transform.eulerAngles.z >= 80) {
                stopRotating();
            }
        }
    }

    public void stopRotating() {
        OnRotatingEnd();
        canRotate = false;
    }
}
