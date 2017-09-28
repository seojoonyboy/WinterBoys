using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speedForce = 0;
    public float torqueForce = 1.0f;
    public float driftFactorSticky = 0.9f;
    public float driftFactorSlippy = 1f;
    public float maxStickyVelocity = 2.5f;
    public float minStickyVelocity = 1.5f;

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float driftFactor = driftFactorSticky;
        //Debug.Log(RightVelocity().magnitude);
        if(RightVelocity().magnitude > maxStickyVelocity) {
            driftFactor = driftFactorSlippy;
        }

        if (Input.GetKey(KeyCode.W)) {
            rb.AddForce(transform.up * speedForce);
        }
        rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;

        //rb.AddTorque(Input.GetAxis("Horizontal") * torqueForce);
        rb.angularVelocity = Input.GetAxis("Horizontal") * torqueForce;

        Debug.Log("Rig Vel : " + rb.velocity);
    }

    Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }
}
