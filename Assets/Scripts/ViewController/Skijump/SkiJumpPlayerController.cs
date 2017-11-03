using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpPlayerController : MonoBehaviour {
    public Transform arrow;
    public float forceAmount;

    private Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;

    public float height = 30;
    private bool
        isAscending = false,
        isLanding = false,
        isDescending = false;

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
        if (isLanding) return;

        //상승 버튼을 누르는 경우
        if (isAscending) {
            //45도 이상 뒤로 기울지 않게 고정
            if (transform.eulerAngles.z < 180 && transform.eulerAngles.z > 35) {
                rb.angularVelocity = 0f;
            }
            else {
                rb.angularVelocity = 25f;
            }
            if (transform.position.y <= height) {
                Debug.Log(rb.velocity.x);
                if(rb.velocity.x > 0) {
                    rb.AddForce(Vector2.up * 30);
                    //회전 저항 부여
                    rb.angularDrag = 0.0001f;
                    //일반 저항 부여
                    rb.drag = 0.0001f;
                }
            }
        }
        else {
            //하강 버튼을 누르는 경우
            if (isDescending) {
                rb.angularVelocity = -25f;
                rb.AddForce(-Vector2.up * 15);
            }
            //자연 하강을 하는 경우
            else {
                //저항 제거
                rb.drag = 0f;
                rb.angularDrag = 0f;

                rb.angularVelocity = -15f;
            }
        }

        //기본적인 바람 저항
        float dot = Vector2.Dot(transform.up, Vector2.left);
        //Debug.Log(dot);
        if(dot >= 0 && rb.velocity.x >= 0) {
            Vector2 airResist = new Vector2(-dot * 10, 0);
            rb.AddForce(airResist);
        }

        if (isLanding) {
            //불안정 자세로 착지시 
            rb.angularVelocity = -1 * rb.velocity.x * 30f;
        }
    }

    private void RotatingEnd() {
        rb.AddForce(arrow.transform.right * forceAmount, ForceMode2D.Force);
    }

    public void Ascending() {
        //rb.AddForce(Vector2.up * rb.velocity.x, ForceMode2D.Impulse);
        //rb.velocity = new Vector2(rb.velocity.x * 0.85f, rb.velocity.y);
        height *= 0.975f;
        isAscending = true;
        //Debug.Log("최대 상승할 수 있는 높이 : " + height);
    }

    public void EndAscending() {
        isAscending = false;
    }

    public void Descending() {
        isDescending = true;
    }

    public void EndDescending() {
        isDescending = false;
    }

    //착지
    private void _OnLanding() {
        isLanding = true;
    }
}
