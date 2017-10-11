using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ski_PlayerController : MonoBehaviour {
    public float speedForce = 1.0f;
    public float torqueForce = 1.0f;
    public float driftFactor = 1.0f;
    public float angleV = 45.0f;

    public GameObject playerImage;

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        //rb.velocity = ForwardVelocity() + RightVelocity() * driftFactorSlippy;
        rb.AddForce(__ForwardVelocity());

        rb.angularVelocity = Input.GetAxis("Horizontal") * torqueForce;

        checkPlayerPos();
        changePlayerImage();

        if (transform.eulerAngles.z > 260) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 259.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        //최대회전 각도 지정 (좌측)
        if (transform.eulerAngles.z < 100) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 100.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        float angle = Vector3.Angle(transform.up, -Vector3.up);

        if (angle >= angleV) {
            //Debug.Log(transform.up.x);
            Vector3 val;
            if (transform.up.x < 0) {
                val = new Vector3(-1, 0.6f, 0) * (angle / 150.0f);
            } else {
                val = new Vector3(1, 0.6f, 0) * (angle / 150.0f);
            }
            rb.AddForce(val);
        }
        //rb.AddForce(ForwardVelocity() * 2f);
        else {
            rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;
        }
    }

    //전방으로 얼마나 추가적으로 힘을 가할지 (현재 속도 기준)
    Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector3 __ForwardVelocity() {
        return transform.up * Vector3.Dot(-Vector3.up, transform.up) * speedForce;
    }
    
    //코너링시 코너링 방향으로 밀리는 힘의 크기 (현재 속도 기준)
    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
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

    void changePlayerImage() {
        var eularAngle = transform.eulerAngles;
        SpriteRenderer sR = playerImage.GetComponent<SpriteRenderer>();
        GameManager gm = GameManager.Instance;

        Debug.Log(eularAngle);

        if(eularAngle.z >= 135 && eularAngle.z <= 225) {
            sR.sprite = gm.players[0];
        }
        if (eularAngle.z < 135) {
            if (sR.flipX) {
                sR.flipX = false;
            }
            if (eularAngle.z > 105) {
                sR.sprite = gm.players[1];
            }
            else {
                sR.sprite = gm.players[2];
            }
        }
        else if(eularAngle.z > 225) {
            sR.flipX = true;
            sR.sprite = gm.players[1];
            if(eularAngle.z > 250) {
                sR.sprite = gm.players[2];
            }
            else {
                sR.sprite = gm.players[1];
            }
        }
    }
}
