using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp_addForce : MonoBehaviour {
    public float forceAmnt = 1.0f;
    public float torqueForce = 1.0f;
    public float driftFactor = 1.0f;
    Rigidbody rb;
    GameManager gm;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        gm = GameManager.Instance;
    }

    private void FixedUpdate() {
        rb.AddForce(transform.forward * forceAmnt);
        rb.angularVelocity = new Vector3(0, Input.GetAxis("Horizontal") * torqueForce, 0);

        rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;

        //Debug.Log(rb.velocity);

        checkPlayerPos();
    }

    //전방으로 얼마나 추가적으로 힘을 가할지 (현재 속도 기준)
    Vector3 ForwardVelocity() {
        Vector3 val = transform.forward * Vector3.Dot(transform.forward, rb.velocity);
        return val;
    }

    //코너링시 코너링 방향으로 밀리는 힘의 크기 (현재 속도 기준)
    Vector3 RightVelocity() {
        Vector3 val = transform.right * Vector3.Dot(transform.right, rb.velocity);
        return val;
    }

    void checkPlayerPos() {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f)) {
            if(hit.transform.position.z <= GameManager.Instance.tmp_tH.lastTilePos.z) {
                if (!GameManager.Instance.tmp_tH.isMade) {
                    GameManager.Instance.tmp_tH.addToBoard();
                }
            }
        }
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }
}
