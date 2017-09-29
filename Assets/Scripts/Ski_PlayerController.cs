﻿using System.Collections;
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
        //rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;

        //회전속도
        rb.angularVelocity = Input.GetAxis("Horizontal") * torqueForce;

        //최대회전 각도 지정 (우측)
        if (transform.eulerAngles.z > 270) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 269.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        //최대회전 각도 지정 (좌측)
        if (transform.eulerAngles.z < 90) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 91.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        //항상 Player 전방으로 Force 부여
        //rb.AddForce(transform.up * speedForce);

        //커브한 정도에 따라 감속
        //...
        //...


        Debug.Log(rb.angularVelocity);
        if (rb.angularVelocity != 0) {
            var val = transform.up * speedForce / rb.angularVelocity * coneringLossVelocity;
            rb.AddForce(val);
        } else {
            rb.AddForce(transform.up * speedForce);
        }
        checkPlayerPos();
    }

    //전방으로 얼마나 추가적으로 힘을 가할지 (현재 속도 기준)
    Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }
    
    //코너링시 코너링 방향으로 밀리는 힘의 크기 (현재 속도 기준)
    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right * 2);
    }

    void checkPlayerPos() {
        Ray2D ray = new Ray2D(transform.position, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null) {
            Vector2 pos = hit.collider.transform.position;
            var bM = GameManager.Instance.bM;
            if (pos.y <= bM.lastTilePos.y) {
                if (!bM.isMade) {
                    bM.addToBoard();
                }
            }
        }
        Debug.DrawRay(transform.position, Vector3.forward, Color.red);
    }
}
