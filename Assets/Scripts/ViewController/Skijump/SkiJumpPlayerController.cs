using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkiJumpPlayerController : MonoBehaviour {
    private SkiJumpManager sm;
    public Transform arrow;
    public float forceAmount;

    private Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;

    //최대 상승 가능 높이
    private float MaxHeight = 100;
    private bool
        isAscending = false,
        isLanding = false,
        isDescending = false;

    public GameObject 
        endSlope,
        ground;
    private int ascendingCnt = 0;

    private void Awake() {
        sm = SkiJumpManager.Instance;
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        ArrowRotate.OnRotatingEnd += RotatingEnd;
        Landing.OnLanding += _OnLanding;
    }

    private void Update() {
        sm.speedText.GetComponent<Text>().text = System.Math.Round(rb.velocity.magnitude * 3, 2) + " km/h";
    }

    private void FixedUpdate() {
        if (isLanding) return;

        //Debug.Log(rb.velocity);
        if (isAscending) {
            //45도 이상 뒤로 기울지 않게 고정
            if (transform.eulerAngles.z < 180 && transform.eulerAngles.z > 35) {
                rb.angularVelocity = 0f;
            }
            else {
                rb.angularVelocity = 25f;
            }


            //if (rb.velocity.y < 0 || (rb.velocity.y > 0 && transform.position.y < MaxHeight)) {
            //    rb.AddForce(Vector2.up * 20f);
            //}
            //else {
            //    isAscending = false;
            //}
            
            if (rb.velocity.y <= 0) {
                //Debug.Log("하강중");
                rb.AddForce(Vector2.up * rb.velocity.magnitude);
                //Debug.Log(Vector2.up * -rb.velocity.y * 10f);
            }
            else {
                Debug.Log(transform.position.y);
                Debug.Log("최대 상승할 수 있는 높이 : " + MaxHeight);
                if (transform.position.y <= MaxHeight) {
                    rb.AddForce(Vector2.up * 20f);
                }
                else {
                    isAscending = false;
                }
            }
        }

        else {
            //하강 버튼을 누르는 경우
            if (isDescending) {
                rb.angularVelocity = -25f;
                Vector2 val = new Vector2(rb.velocity.x * 0.1f, -0.01f);
                rb.AddForce(val);
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
        //float dot = Vector2.Dot(transform.up, Vector2.left);
        ////Debug.Log(dot);
        //if(dot >= 0 && rb.velocity.x >= 0) {
        //    Vector2 airResist = new Vector2(-dot * 10, 0);
        //    rb.AddForce(airResist);
        //}

        if (isLanding) {
            //불안정 자세로 착지시
            rb.angularVelocity = -1 * rb.velocity.x * 30f;
        }
    }

    private void RotatingEnd() {
        rb.AddForce(arrow.transform.right * forceAmount, ForceMode2D.Force);
    }

    public void Ascending() {
        isAscending = true;

        if (ascendingCnt > 1) {
            MaxHeight = transform.position.y * 0.65f;
        }
        else {
            MaxHeight = transform.position.y * 0.8f;
        }
        ascendingCnt++;
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
