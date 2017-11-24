using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameEvents;

public class SkiJumpManager : Singleton<SkiJumpManager> {
    protected SkiJumpManager() { }

    private EventManager _eventManger;
    private PointManager pm;

    public SkiJumpPlayerController playerController;
    public ArrowRotate arrowController;
    public SkiJumpCM_controller CM_controller;

    public GameObject
        modal,
        character,
        forceButton,                //가속 버튼
        angleUI,                    //각도기 UI
        jumpButton,                 //점프하기 버튼
        speedText;
    public GameObject[] upAndDownButtons;

    public float forceAmount;           //가속 정도
    public float statBasedSpeedForce;  //Stat을 적용한 가속 정도
    public float 
        slowdownFactor,     //슬로우 모션 정도
        frictionFactor;     //마찰 계수

    private Rigidbody2D charRb;
    private bool 
        isLanded = false,
        isUnstableLand = false,
        tmp = true;
    public double 
        score = 0,
        bonusScore = 0;

    private void Awake() {
        _eventManger = EventManager.Instance;
        pm = PointManager.Instance;
    }

    private void OnEnable() {
        _eventManger.AddListenerOnce<SkiJump_JumpEvent>(_OnJumpArea);
        _eventManger.AddListenerOnce<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.AddListenerOnce<SkiJump_UnstableLandingEvent>(_UnstableLanding);
        _eventManger.AddListenerOnce<SkiJump_ArrowRotEndEvent>(_OffZooming);

        Time.timeScale = 1;
        bonusScore = 0;
    }

    private void OnDisable() {
        isLanded = false;
    }

    private void _OnJumpArea(SkiJump_JumpEvent e) {
        forceButton.SetActive(false);
        angleUI.SetActive(true);
        jumpButton.SetActive(true);
    }

    private void Start() {
        Screen.orientation = ScreenOrientation.LandscapeRight;

        initGroundEnv();

        charRb = character.GetComponent<Rigidbody2D>();

        //statBasedSpeedForce = forceAmount * pm.getSpeedPercent();

        //최저치
        statBasedSpeedForce = forceAmount;
        //최대치
        //statBasedSpeedForce = forceAmount * 1.5f;
    }

    private void FixedUpdate() {
        if (isLanded) {
            if(charRb.velocity.x <= 0) {
                modal.SetActive(true);

                //착지 위치 기반 점수 계산
                score = System.Math.Round(character.transform.position.x / 6.0f);
                if (isUnstableLand) {
                    Debug.Log("불안정 착지로 인한 감점");
                    score = System.Math.Round(score * 0.75f);
                }
                double totalScore = score + bonusScore;
                modal.transform.Find("InnerModal/Score").GetComponent<Text>().text = "최종 점수 : " + totalScore + " 점 획득";
                isLanded = false;

                pm.setRecord(character.transform.position.x, SportType.SKIJUMP);
                pm.addPoint((int)totalScore);
            }
        }
    }

    //마찰 계수 설정
    //...
    private void initGroundEnv() {

    }

    public void mainLoad() {
        //UM_GameServiceManager.ActionScoreSubmitted -= HandleActionScoreSubmitted;
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;

        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void restart() {
        SceneManager.LoadScene("SkiJump");
    }

    //가속 버튼
    public void AddForce() {
        CM_controller.playableDirectors[0].gameObject.SetActive(false);
        if (tmp) {
            playerController.SkelAnimChange("run", true);
            tmp = false;
        }
    }

    //점프 버튼 클릭
    public void jumping() {
        arrowController.stopRotating();
        charRb.AddForce(angleUI.transform.up * 10, ForceMode2D.Impulse);
    }

    private void _UnstableLanding(SkiJump_UnstableLandingEvent e) {
        charRb.velocity = new Vector2(charRb.velocity.x, 0);
        isUnstableLand = true;
    }

    private void _OnLanding(SkiJump_LandingEvent e) {
        isLanded = true;
    }

    private void _OffZooming(SkiJump_ArrowRotEndEvent e) {
        Time.timeScale = 1.0f;

        jumpButton.SetActive(false);
        angleUI.SetActive(false);

        foreach (GameObject obj in upAndDownButtons) {
            obj.SetActive(true);
        }
    }
}
