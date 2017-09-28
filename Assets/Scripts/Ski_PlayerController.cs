using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ski_PlayerController : MonoBehaviour {
    public float speedForce = 0;
    public float torqueForce = 1.0f;
    public float driftFactorSticky = 0.9f;
    public float driftFactorSlippy = 1f;
    public float maxStickyVelocity = 2.5f;
    public float minStickyVelocity = 1.5f;
    public float coneringLossVelocity = 1.0f;

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float driftFactor = driftFactorSticky;
        //Debug.Log(RightVelocity().magnitude);
        if (RightVelocity().magnitude > maxStickyVelocity) {
            driftFactor = driftFactorSlippy;
        }
        rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;
        //회전력
        rb.angularVelocity = Input.GetAxis("Horizontal") * torqueForce;
        //Debug.Log(transform.eulerAngles.z);
        //rb.freezeRotation = true;
        Debug.Log(transform.eulerAngles);
        if(transform.eulerAngles.z > 270) {
            if(rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 269.5f);
            }
            else {
                rb.freezeRotation = true;
            }
        }

        if(transform.eulerAngles.z < 90) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 91.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        if (rb.angularVelocity != 0) {
            var val = transform.up * speedForce / rb.angularVelocity * coneringLossVelocity;
            rb.AddForce(val);
        }
        else {
            rb.AddForce(transform.up * speedForce);
        }
        checkPlayerPos();
    }

    Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

    void checkPlayerPos() {
        Ray2D ray = new Ray2D(transform.position, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null) {
            Vector2 pos = hit.collider.transform.position;
            var bM = GameManager.Instance.bM;
            if (pos == bM.lastTilePos) {
                if (!bM.isMade) {
                    bM.addToBoard(pos.y);
                }
            }
        }
        //Debug.DrawRay(transform.position, Vector3.forward, Color.red);
    }
}
