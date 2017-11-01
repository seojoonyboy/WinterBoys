using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpPlayerController : MonoBehaviour {
    public Transform arrow;
    public float forceAmount;

    private Rigidbody2D rb;

    private void Update() {
        if (arrow.gameObject.activeSelf) {
            transform.rotation = arrow.rotation;
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        ArrowRotate.OnRotatingEnd += RotatingEnd;
    }

    private void RotatingEnd() {
        rb.AddForce(transform.right * forceAmount, ForceMode2D.Impulse);
    }
}
