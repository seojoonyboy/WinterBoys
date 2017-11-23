﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using GameEvents;
using System;

public class SkiJumpPlayerController : MonoBehaviour {
    private EventManager _eventManger;
    private GameManager gm;
    private PointManager pm;

    public SkiJumpManager sm;
    public Transform arrow;
    public Transform startPos;

    public float forceAmount;

    private Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;
    private float rotateAmount = 35;        //회전력
    private float statBasedRotAmount;       //Stat을 적용한 회전력
    //최대 상승 가능 높이
    public float MaxHeight = 100;

    private bool
        isAscending = false,
        isLanding = false,
        isDescending = false,
        isFirstAsc = false,
        tmp = false,
        tmp2 = true;

    private int ascendingCnt = 0;
    private int characterIndex = 0;
    private string Slopetag;

    public GameObject[] characters;
    private SkeletonAnimation anim;

    private float whiteBirdCoolTime;
    private PlayerState playerState;
    private float preGravityScale;
    private void Awake() {
        _eventManger = EventManager.Instance;

        gm = GameManager.Instance;
        pm = PointManager.Instance;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        statBasedRotAmount = rotateAmount * pm.getControlPercent();
        whiteBirdCoolTime = 3.0f;

        playerState = PlayerState.NORMAL;
        preGravityScale = rb.gravityScale;
    }

    private void OnEnable() {
        characterIndex = gm.character;

        initChar(characterIndex);                   //Spine Character 설정
        anim = characters[characterIndex].GetComponent<SkeletonAnimation>();
        SkelAnimChange("starting", false);

        _eventManger.AddListenerOnce<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.AddListenerOnce<SkiJump_ArrowRotEndEvent>(RotatingEnd);

        isDescending = false;
        isAscending = false;
        isLanding = false;
        tmp = false;

        ascendingCnt = 0;

        Time.fixedDeltaTime = 0.02f;                //슬로우모션 제거
    }

    private void Update() {
        sm.speedText.GetComponent<Text>().text = System.Math.Round(rb.velocity.magnitude * 3, 2) + " km/h";
    }

    private void FixedUpdate() {
        if (isLanding) return;

        if (isFirstAsc && rb.velocity.y < 0) {
            MaxHeight = transform.position.y * 0.7f;
            isFirstAsc = false;
        }

        //하얀 새 효과
        if(playerState == PlayerState.IMMORTAL) {
            whiteBirdCoolTime -= Time.deltaTime;
            if(whiteBirdCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                rb.gravityScale = preGravityScale;
                whiteBirdCoolTime = 3.0f;
            }
            else {
                rb.angularVelocity = 0;
                rb.AddForce(-transform.right * 0.01f);
                rb.gravityScale = 0;
            }
            return;
        }

        float angle = transform.eulerAngles.z;

        if(rb.velocity.magnitude >= 20) {
            if(anim.AnimationName == "run") {
                SkelAnimChange("run_loop", true);
            }
        }

        if (tmp) {
            //반시계방향 회전중
            if (rb.angularVelocity > 0) {
                //Debug.Log("반시계 방향 회전중");
                if(angle > 45 && angle <= 180) {
                    transform.eulerAngles = new Vector3(0, 0, angle - 0.1f);
                    rb.angularVelocity = 0;
                }
            }
            //시계방향 회전중
            else {
                //Debug.Log("시계 방향 회전중");
                if(angle < 305 && angle >= 180) {
                    transform.eulerAngles = new Vector3(0, 0, angle + 0.1f);
                    rb.angularVelocity = 0;
                }
            }
        }

        if (isAscending) {
            //45도 이상 뒤로 기울지 않게 고정
            if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                rb.angularVelocity = statBasedRotAmount;
            }

            Vector2 force = new Vector2(rb.velocity.x * 0.01f, 20f);
            rb.AddForce(force);

            if (rb.velocity.y < 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.9995f);
            }
            else {
                if(transform.position.y > MaxHeight * 0.9f) {
                    MaxHeight = transform.position.y * 0.7f;
                    isAscending = false;
                }
            }
        }

        else {
            //하강 버튼을 누르는 경우
            if (isDescending) {
                if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                    rb.angularVelocity = -statBasedRotAmount;
                }

                Vector2 val = new Vector2(rb.velocity.x * 0.1f, -0.01f);
                rb.AddForce(val);
            }
            //자연 하강을 하는 경우
        }
    }

    private void RotatingEnd(SkiJump_ArrowRotEndEvent e) {
        Vector2 forceDir = new Vector2(arrow.transform.right.x * forceAmount * 5f * pm.getSpeedPercent(), arrow.transform.right.y * forceAmount * 10f * pm.getSpeedPercent());
        rb.AddForce(forceDir);
        tmp = true;
        isFirstAsc = true;
    }

    //가속 버튼
    public void AddForce() {
        if(Slopetag == "StartSlope") {
            //최대치
            //rb.AddForce(transform.right * sm.statBasedSpeedForce * 0.6f);

            //최저치
            rb.AddForce(transform.right * sm.statBasedSpeedForce);
        }
        else {
            rb.AddForce(transform.right * sm.statBasedSpeedForce * 2f);
        }
    }

    public void Ascending() {
        isAscending = true;

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

    private void initChar(int index) {
        for(int i=0; i<characters.Length; i++) {
            if(i == index) {
                characters[i].SetActive(true);
            }
            else {
                characters[i].SetActive(false);
            }
        }
    }

    public void SkelAnimChange(string name, bool needLoop = false) {
        anim.loop = needLoop;
        anim.AnimationName = name;
    }

    private void _OnLanding(SkiJump_LandingEvent e) {
        isLanding = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Slopetag = collision.transform.tag;
        //Debug.Log(collision.transform.tag);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        itemCheck(obj);
    }

    private void itemCheck(GameObject obj) {
        if (obj.tag == "Item") {
            ItemType type = obj.GetComponent<ItemType>();
            switch (type.type) {
                case itemType.BLACK_BIRD:
                    //감속 효과
                    rb.velocity = new Vector2(rb.velocity.x * 0.85f, rb.velocity.y * 0.85f);
                    break;
                case itemType.WHITE_BIRD:
                    //무적효과, 다른 아이템 무시
                    //3초간 캐릭터 정면으로 이동 (중력 3초간 제거)
                    playerState = PlayerState.IMMORTAL;
                    break;
            }
            Destroy(obj);
        }
    }

    //버프 효과 부여
    private void Buff(float sec) {

    }
}

public enum PlayerState {
    NORMAL,
    IMMORTAL,
    REVERSE_ROTATE,
    GRAVITY_CHANGE,
    BALLOON
}