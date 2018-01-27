using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FlagController : MonoBehaviour {
    public enum type { LEFT, RIGHT };
    public type flagType;

    private bool isHit;

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

        isHit = false;
    }

    private void FixedUpdate() {
        if (isHit) { return; }

        Vector3 dir = Vector3.left;
        ray(dir);

        dir = Vector3.right;
        ray(dir);
    }

    private void ray(Vector3 dir) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit)) {
            if (hit.collider.gameObject.CompareTag("Player")) {
                isHit = true;
                checkSuccessOrFail(dir);
                GetComponent<FlagController>().enabled = false;
            }
        }
    }

    private void checkSuccessOrFail(Vector3 dir) {
        if(flagType == type.LEFT && dir == Vector3.left) {
            //성공
            dm.passNumInc();
            dm.setCombo(1);
            //Debug.Log("Ray로 성공");
        }
        else if(flagType == type.LEFT && dir == Vector3.right) {
            //실패
            dm.setCombo(0);
            dm.decreaseTime(5);
            //Debug.Log("Ray로 실패");
        }

        if(flagType == type.RIGHT && dir == Vector3.right) {
            //성공
            dm.passNumInc();
            dm.setCombo(1);
            //Debug.Log("Ray로 성공");
        }
        else if (flagType == type.RIGHT && dir == Vector3.left) {
            //실패
            dm.setCombo(0);
            dm.decreaseTime(5);
            //Debug.Log("Ray로 실패");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isHit) { return; }

        if (collision.gameObject.CompareTag("Player")) {
            //성공
            isHit = true;

            dm.passNumInc();
            dm.setCombo(1);
            //Debug.Log("충돌로 성공");

            if(flagType == type.LEFT) {
                anim.loop = false;
                anim.AnimationName = "left_crash";
            }
            else if(flagType == type.RIGHT) {
                anim.loop = false;
                anim.AnimationName = "right_crash";
            }
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x * 0.8f, rb.velocity.y * 0.8f);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_flagCrash");

            GetComponent<FlagController>().enabled = false;
        }
    }
}
