using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FlagController : MonoBehaviour {
    public enum type { LEFT, RIGHT };
    public type rayDir;
    public float distance;

    private bool isSend = false;
    private bool isPassSend = false;

    private DownhillManager dm;
    private BoardManager bm;

    private SkeletonAnimation anim;

    void Start() {
        gameObject.tag = "Flag";
    }

    private void Awake() {
        dm = GameObject.Find("Manager").GetComponent<DownhillManager>();
        bm = GameObject.Find("BoardHolder").GetComponent<BoardManager>();

        anim = GetComponent<SkeletonAnimation>();
        anim.loop = false;
    }

    private void FixedUpdate() {
        IsFail();
        IsPass();
    }

    void IsFail() {
        Vector3 dir = Vector3.zero;
        if (rayDir == type.LEFT) {
            dir = Vector3.left;
        }
        else if (rayDir == type.RIGHT) {
            dir = Vector3.right;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit)) {
            if (hit.collider.tag == "Player") {
                if(dm.playerController.playerState == Ski_PlayerController.PlayerState.BOOSTING) {
                    return;
                }

                if (!isSend) {
                    //Debug.Log("실패");
                    dm.remainTime -= (int)GameManager.Instance.panelty_time;
                    dm.setCombo(0);
                    //if (bm.centers.Count != 0) {
                    //    bm.centers.RemoveAt(0);
                    //}
                }
                isSend = true;
            }
        }
    }
    void IsPass() {
        RaycastHit hit;
        Vector3 dir = Vector3.zero;
        if (rayDir == type.LEFT) {
            dir = Vector3.right;
            if (Physics.Raycast(transform.position, dir, out hit, distance)) {
                if (hit.collider.tag == "Player") {
                    if (!isPassSend) {
                        //Debug.Log("성공");
                        dm.passNumInc();
                        dm.setCombo(1);
                        //if (bm.centers.Count != 0) {
                        //    bm.centers.RemoveAt(0);
                        //}
                    }
                    isPassSend = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            if (rayDir == type.LEFT) {
                anim.AnimationName = "broken_left";
            }
            else if (rayDir == type.RIGHT) {
                anim.AnimationName = "broken_right";
            }
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x * 0.8f, rb.velocity.y * 0.8f);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_flagCrash");
        }
    }
}
