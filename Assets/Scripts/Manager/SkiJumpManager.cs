using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkiJumpManager : Singleton<SkiJumpManager> {
    protected SkiJumpManager() { }

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

    public float forceAmount;
    public float 
        slowdownFactor,     //슬로우 모션 정도
        frictionFactor;     //마찰 계수

    private Rigidbody2D charRb;
    private bool 
        isLanded = false,
        isUnstableLand = false,
        tmp = true;
    private double score = 0;

    private void OnEnable() {
        SlowMotion.OnJumpArea += _OnJumpArea;
        Landing.OnLanding += _OnLanding;
        Landing.UnstableLanding += _UnstableLanding;
        ArrowRotate.OnRotatingEnd += _OffZooming;

        Time.timeScale = 1;
    }

    private void OnDisable() {
        SlowMotion.OnJumpArea -= _OnJumpArea;
        Landing.OnLanding -= _OnLanding;
        ArrowRotate.OnRotatingEnd -= _OffZooming;

        isLanded = false;
    }

    private void Awake() {
        pm = PointManager.Instance;
    }

    private void Start() {
        Screen.orientation = ScreenOrientation.LandscapeRight;

        initGroundEnv();

        charRb = character.GetComponent<Rigidbody2D>();
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
                modal.transform.Find("InnerModal/Score").GetComponent<Text>().text = "최종 점수 : " + score + " 점 획득";
                isLanded = false;

                pm.setRecord((float)score, SportType.SKIJUMP);
                pm.addPoint((int)score, SportType.SKIJUMP);
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
        charRb.AddForce(character.transform.right * forceAmount);
        CM_controller.Play(1);
        if (tmp) {
            playerController.SkelAnimChange("run", true);
            tmp = false;
        }
    }

    private void _OnJumpArea() {
        forceButton.SetActive(false);
        angleUI.SetActive(true);
        jumpButton.SetActive(true);

        CM_controller.Play(2);
    }

    //점프 버튼 클릭
    public void jumping() {
        arrowController.stopRotating();
        _OffZooming();

        charRb.AddForce(angleUI.transform.up * 10, ForceMode2D.Impulse);
    }

    private void _OffZooming() {
        Time.timeScale = 1.0f;

        jumpButton.SetActive(false);
        angleUI.SetActive(false);

        foreach(GameObject obj in upAndDownButtons) {
            obj.SetActive(true);
        }

        CM_controller.Play(3);
    }

    private void _OnLanding() {
        isLanded = true;
    }

    private void _UnstableLanding() {
        isUnstableLand = true;
    }
}
