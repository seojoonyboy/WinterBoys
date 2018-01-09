﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using GameEvents;
using System;

public class SkiJumpPlayerController : MonoBehaviour {
    private EventManager _eventManger;
    private SoundManager soundManager;

    private GameManager gm;
    private SaveManager pm;

    public SkiJumpManager sm;
    public Transform arrow;
    public Transform startPos;

    public float forceAmount;

    [HideInInspector]public Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;
    private float rotateAmount = 35;        //회전력
    private float statBasedRotAmount;       //Stat을 적용한 회전력

    private bool
        isAscending = false,
        isLanding = false,
        isDescending = false,
        isFirstAsc = false,
        tmp = false,
        tmp2 = true,
        canButtonPress = true;

    private int ascendingCnt = 0;
    private int characterIndex = 0;
    private string Slopetag;

    public GameObject[] characters;
    private SkeletonAnimation anim;
    private ItemCoolTime itemCooltimes;

    private float 
        whiteBirdCoolTime,
        blackBirdCoolTime,
        balloonCoolTime,
        reverseCoolTime,
        thunderCoolTime,
        buttonCoolTime;

    private Vector2 beforeVelOfWhiteBird;
    public PlayerState playerState;
    private float preGravityScale;

    public AudioSource extraAudioSource;
    private void Awake() {
        _eventManger = EventManager.Instance;
        soundManager = SoundManager.Instance;

        gm = GameManager.Instance;
        pm = SaveManager.Instance;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        statBasedRotAmount = rotateAmount * pm.getControlPercent();

        playerState = PlayerState.NORMAL;
        preGravityScale = rb.gravityScale;

        rb.centerOfMass = new Vector2(0, -0.6f);

        itemCooltimes = sm.GetComponent<ItemCoolTime>();

        removeListener();

        _eventManger.AddListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.AddListener<SkiJump_Resume>(resume);
    }

    private void OnEnable() {
        characterIndex = gm.character;

        initChar(characterIndex);                   //Spine Character 설정
        anim = characters[characterIndex].GetComponent<SkeletonAnimation>();
        SkelAnimChange("starting", false);

        isDescending = false;
        isAscending = false;
        isLanding = false;
        canButtonPress = true;

        tmp = false;

        ascendingCnt = 0;

        Time.fixedDeltaTime = 0.02f;                //슬로우모션 제거

        soundManager.Play(SoundManager.SoundType.BGM, "sj");
    }

    private void Update() {
        Vector2 nextBgStandardPos = sm.boardHolder.nextSetPos;
        if(transform.position.x >= nextBgStandardPos.x) {
            _eventManger.TriggerEvent(new SkjJump_NextBgGenerate());
        }

        if (!canButtonPress) {
            buttonCoolTime -= Time.deltaTime;
            if(buttonCoolTime < 0) {
                canButtonPress = true;
                buttonCoolTime = 0.3f;
            }
        }
    }

    private void FixedUpdate() {
        if (isLanding) return;

        if (sm.isGameEnd) {
            transform.eulerAngles = Vector3.zero;
            return;
        }

        if (isFirstAsc && rb.velocity.y < 0) {
            isFirstAsc = false;
        }

        if(transform.position.y > 35.0f) {
            sm.gameOver();
        }

        effectCheck();

        float angle = transform.eulerAngles.z;

        if(rb.velocity.magnitude >= 20) {
            if(anim.AnimationName == "run") {
                SkelAnimChange("run_loop", true);
            }
        }

        if (tmp) {
            if (angle > 45 && angle <= 180) {
                transform.eulerAngles = new Vector3(0, 0, angle - 0.1f);
                rb.angularVelocity = 0;
            }

            if (angle < 305 && angle >= 180) {
                transform.eulerAngles = new Vector3(0, 0, angle + 0.1f);
                rb.angularVelocity = 0;
            }
        }

        if (isAscending) {
            buttonCoolTime -= Time.deltaTime;
            if(buttonCoolTime < 0) {
                buttonCoolTime = 0.8f;
                canButtonPress = true;
            }
            int mark = 1;
            if(playerState == PlayerState.REVERSE_ROTATE) {
                mark *= -1;
            }

            if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                rb.angularVelocity = mark * statBasedRotAmount;
            }

            if(playerState == PlayerState.REVERSE_ROTATE) {
                Vector2 val = new Vector2(rb.velocity.x * 0.1f, -0.01f);
                rb.AddForce(val);
            }

            else {
                float forceX = -1;
                if (rb.velocity.x < 1) {
                    forceX = 0;
                }
                Vector2 force = new Vector2(forceX, 20f);
                rb.AddForce(force);

                if (rb.velocity.y < 0) {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.99999f);
                }
            }
        }

        else {
            //하강 버튼을 누르는 경우
            if (isDescending) {
                int mark = -1;
                if(playerState == PlayerState.REVERSE_ROTATE) {
                    mark *= -1;
                }

                if(playerState == PlayerState.REVERSE_ROTATE) {
                    float forceX = -1;
                    if (rb.velocity.x < 1) {
                        forceX = 0;
                    }
                    Vector2 force = new Vector2(forceX, 20f);
                    rb.AddForce(force);

                    if (rb.velocity.y < 0) {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.999995f);
                    }
                }
                else {
                    if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                        rb.angularVelocity = mark * statBasedRotAmount;
                    }
                    //Debug.Log(rb.velocity.x);
                    float forceX = 10;
                    if (rb.velocity.x > 40) {
                        forceX = 0;
                    }
                    Vector2 val = new Vector2(forceX, -0.01f);
                    rb.AddForce(val);
                }
            }
        }
    }

    public void RotatingEnd() {
        Vector2 forceDir = new Vector2(arrow.transform.right.x * forceAmount * 5f * pm.getSpeedPercent(), arrow.transform.right.y * forceAmount * 10f * pm.getSpeedPercent());
        rb.AddForce(forceDir);
        tmp = true;
        isFirstAsc = true;

        sm.CM_controller.off(1);

        extraAudioSource.gameObject.SetActive(false);

        soundManager.Play(SoundManager.SoundType.EFX, "sj_arrowRot");
        soundManager.Play(SoundManager.SoundType.BGM, "sj_fly");
    }

    //가속 버튼
    public void AddForce() {
        if(Slopetag == "StartSlope") {
            rb.AddForce(transform.right * 20f);
        }

        else if(Slopetag == "MainSlope") {
            rb.AddForce(transform.right * sm.statBasedSpeedForce * 2.5f);

            extraAudioSource.clip = soundManager.searchResource(SoundManager.SoundType.EFX, "sj_slideMove").clip;
            if (!extraAudioSource.isPlaying) {
                extraAudioSource.Play();
            }
        }

        sm.CM_controller.Play(1);
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "sj_addForceBtn");
    }

    public void Ascending() {
        if (canButtonPress) {
            isAscending = true;
        }
    }

    public void EndAscending() {
        isAscending = false;
        canButtonPress = false;
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

    private void resume(SkiJump_Resume e) {
        isLanding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(Slopetag == "MainSlope") {
            return;
        }
        Slopetag = collision.gameObject.tag;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        itemCheck(obj);
    }

    private float whiteBirdEffect() {
        return 0;
    }

    private void effectCheck() {
        //검은새 효과
        if(playerState == PlayerState.BLACK_BIRD) {
            blackBirdCoolTime -= Time.deltaTime;
            Vector2 dir = new Vector2(0.3f, -1.0f);
            rb.MovePosition(rb.position + dir * 0.01f);

            if (blackBirdCoolTime < 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                playerState = PlayerState.NORMAL;
            }
        }

        //풍선 효과
        if(playerState == PlayerState.BALLOON) {
            balloonCoolTime -= Time.deltaTime;
            Vector2 dir = new Vector2(0.3f, 1.0f);
            rb.MovePosition(rb.position + dir * 0.01f);

            if (balloonCoolTime < 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                playerState = PlayerState.NORMAL;
            }
        }

        //무적효과, 다른 아이템 무시
        //오른쪽으로 1000m/h 속도로 강제 이동 (제어 불가)
        //3초간 캐릭터 정면으로 이동 (중력 3초간 제거)
        if (playerState == PlayerState.WHITE_BIRD) {
            whiteBirdCoolTime -= Time.deltaTime;
            if (whiteBirdCoolTime < 0) {
                rb.velocity = beforeVelOfWhiteBird;
                rb.AddForce(Vector2.up, ForceMode2D.Impulse);
                playerState = PlayerState.NORMAL;
            }
            else {
                rb.velocity = new Vector2(34f, 0);
            }
            return;
        }

        //먹구름 효과
        if (playerState == PlayerState.REVERSE_ROTATE) {
            reverseCoolTime -= Time.deltaTime;
            if (reverseCoolTime < 0) {
                playerState = PlayerState.NORMAL;
            }
        }

        //번개 먹구름 효과
        if (playerState == PlayerState.GRAVITY_CHANGE) {
            thunderCoolTime -= Time.deltaTime;
            if (thunderCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                rb.gravityScale = 0.8f;
            }
            else {
                rb.gravityScale = 1.0f;
            }
        }
    }

    public void itemCheck(GameObject obj) {
        if(playerState == PlayerState.WHITE_BIRD) {
            Destroy(obj);
            return;
        }

        if (obj.tag == "Item") {
            Item item = obj.GetComponent<Item>();
            switch (item.item_sj) {
                case ItemType.SJ.BL_BIRD:
                    //1초간 하강 (약 15%)
                    blackBirdCoolTime = itemCooltimes.blackBird_cooltime;
                    sm.addEffectIcon(1, blackBirdCoolTime);

                    playerState = PlayerState.BLACK_BIRD;
                    break;
                case ItemType.SJ.WH_BIRD:
                    whiteBirdCoolTime = itemCooltimes.whiteBird_cooltime;
                    beforeVelOfWhiteBird = rb.velocity;

                    sm.addEffectIcon(0, whiteBirdCoolTime);

                    playerState = PlayerState.WHITE_BIRD;
                    break;
                case ItemType.SJ.BALLOON:
                    balloonCoolTime = itemCooltimes.balloon_cooltime;
                    sm.addEffectIcon(3, balloonCoolTime);

                    playerState = PlayerState.BALLOON;
                    break;
                case ItemType.SJ.DK_CLOUD:
                    reverseCoolTime = itemCooltimes.reverseRot_cooltime;
                    sm.addEffectIcon(2, reverseCoolTime);

                    playerState = PlayerState.REVERSE_ROTATE;
                    break;
                case ItemType.SJ.POINT:
                    sm.bonusScore += 50;
                    break;
                case ItemType.SJ.THUNDER_CLOUD:
                    thunderCoolTime = itemCooltimes.thunderCloud_cooltime;
                    sm.addEffectIcon(4, thunderCoolTime);

                    playerState = PlayerState.GRAVITY_CHANGE;
                    break;
                case ItemType.SJ.MONEY:
                    sm.addCrystal(5);
                    break;
                case ItemType.SJ.TIME:
                    sm.lastTime += 15f;
                    break;
            }
            Destroy(obj);
        }
    }

    public enum PlayerState {
        NORMAL,
        WHITE_BIRD,
        BLACK_BIRD,
        REVERSE_ROTATE,
        GRAVITY_CHANGE,
        BALLOON
    }

    private void removeListener() {
        _eventManger.RemoveListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.RemoveListener<SkiJump_Resume>(resume);
    }
}

