using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SkiJumpPlayerController : MonoBehaviour {
    public Transform arrow;
    public float forceAmount;

    private Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;
    private SkeletonAnimation anim;

    public float height = 30;
    private bool
        isAscending = false,
        isLanding = false;

    public GameObject 
        endSlope,
        ground;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        ArrowRotate.OnRotatingEnd += RotatingEnd;
        Landing.OnLanding += _OnLanding;
    }

    private void FixedUpdate() {
        //Debug.Log("속력 : " + rb.velocity.magnitude);
        if (isAscending) {
            if(transform.position.y <= height) {
                rb.AddForce(Vector2.up * 30);
                //회전 저항 부여
                rb.angularDrag = 0.0001f;
                //일반 저항 부여
                rb.drag = 0.0001f;
            }

            rb.angularVelocity = 25f;
        }
        else {
            //저항 제거
            rb.drag = 0f;
            rb.angularDrag = 0f;

            rb.angularVelocity = -15f;
        }

        if (isLanding) {
            //불안정 자세로 착지시 
            rb.angularVelocity = -1 * rb.velocity.x * 30f;
        }
    }

    private void RotatingEnd() {
        rb.AddForce(transform.up * forceAmount, ForceMode2D.Force);
    }

    public void Ascending() {
        //rb.AddForce(Vector2.up * rb.velocity.x, ForceMode2D.Impulse);
        //rb.velocity = new Vector2(rb.velocity.x * 0.85f, rb.velocity.y);
        height *= 0.975f;
        isAscending = true;
        Debug.Log("최대 상승할 수 있는 높이 : " + height);
    }

    public void EndAscending() {
        isAscending = false;
    }

    //착지
    private void _OnLanding() {
        //Debug.Log("착지");
        isLanding = true;
    }
}
