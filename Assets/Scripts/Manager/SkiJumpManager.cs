using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameEvents;
using UnityEngine.Advertisements;

public class SkiJumpManager : Singleton<SkiJumpManager> {
    protected SkiJumpManager() { }

    private EventManager _eventManger;
    private SaveManager pm;

    public SkiJumpPlayerController playerController;
    public ArrowRotate arrowController;
    public SkiJumpCM_controller CM_controller;
    public SkiJumpBoardHolder boardHolder;

    public GameObject
        modal,
        character,
        forceButton,                //가속 버튼
        angleUI,                    //각도기 UI
        jumpButton,                 //점프하기 버튼
        resumeBtn,                  //이어하기 버튼
        adsBtn;                     //광고보기 버튼

    public Text 
        speedText,
        distanceText;

    public GameObject[] upAndDownButtons;

    public float forceAmount;           //가속 정도
    public float statBasedSpeedForce;  //Stat을 적용한 가속 정도
    public float 
        slowdownFactor,     //슬로우 모션 정도
        frictionFactor;     //마찰 계수
    public float qte_magnification = 0;
    private Rigidbody2D charRb;
    private bool 
        isLanded = false,
        isUnstableLand = false,
        tmp = true;
    public double 
        score = 0,
        bonusScore = 0,
        totalScore = 0;

    public Text 
        height,
        itemEffect;

    public Slider heightSlider;
    public GameObject qteButton;

    public bool isQTE_occured = false;

    private float 
        playTime,
        preFixedDeltaTime;
    private GameObject resumeButton;

    private SoundManager soundManager;

    private void Awake() {
        _eventManger = EventManager.Instance;
        pm = SaveManager.Instance;

        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        Time.timeScale = 1;
        bonusScore = 0;

        playTime = 0;

        qte_magnification = 0;
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
        Screen.orientation = ScreenOrientation.Landscape;
        
        charRb = character.GetComponent<Rigidbody2D>();

        statBasedSpeedForce = forceAmount * pm.getSpeedPercent();
        resumeButton = modal.transform.Find("InnerModal/Buttons/Resume").gameObject;

        _eventManger.AddListener<SkiJump_JumpEvent>(_OnJumpArea);
        _eventManger.AddListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.AddListener<SkiJump_UnstableLandingEvent>(_UnstableLanding);
        _eventManger.AddListener<SkiJump_ArrowRotEndEvent>(_OffZooming);
        _eventManger.AddListener<SkiJump_Resume>(resume);
        //최저치
        //statBasedSpeedForce = forceAmount;
        //최대치
        //statBasedSpeedForce = forceAmount * 1.8f;

        preFixedDeltaTime = Time.fixedDeltaTime;

        if (!resumeBtn.activeSelf) {
            resumeBtn.SetActive(true);
        }

        connectUnityAdsButton();
    }

    private void Update() {
        float speed = playerController.rb.velocity.magnitude;
        float dist = playerController.transform.position.x;

        speedText.text = System.Math.Round(speed * 3, 2) + "KM/h";
        if(dist <= 0) {
            dist = 0;
        }
        distanceText.text = System.Math.Truncate(dist) + "M";
        itemEffect.text = playerController.playerState + " 효과 적용중";
    }

    private void FixedUpdate() {
        playTime += Time.deltaTime;
        if (isLanded) {
            charRb.velocity = new Vector2(charRb.velocity.x * 0.999995f, charRb.velocity.y * 0.995f);
            if(charRb.velocity.x <= 0) {
                gameOver();
            }
        }
        double value = System.Math.Round(charRb.transform.position.y * 3f);
        height.text = value + " M";
        heightSlider.value = (float)value;
    }

    public void mainLoad() {
        //UM_GameServiceManager.ActionScoreSubmitted -= HandleActionScoreSubmitted;
        SceneManager.LoadScene("Main");

        Time.timeScale = 1;

        Screen.orientation = ScreenOrientation.Portrait;

        removeListener();

        soundManager.Play(SoundManager.SoundType.EFX, "returnMain");

        resumeButton.SetActive(true);

        pm.setRecord(character.transform.position.x, SportType.SKIJUMP);
        pm.addPoint((int)totalScore);
    }

    public void restart() {
        SceneManager.LoadScene("SkiJump");

        Time.timeScale = 1;

        removeListener();
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
        Debug.Log("착지");

        playerController.extraAudioSource.gameObject.SetActive(true);

        playerController.extraAudioSource.clip = soundManager.searchResource(SoundManager.SoundType.EFX, "sj_landingAndSlide").clip;
        playerController.extraAudioSource.Play();

        soundManager.Play(SoundManager.SoundType.EFX, "sj_landing");
    }

    private void _OffZooming(SkiJump_ArrowRotEndEvent e) {
        Time.timeScale = 1.0f;

        jumpButton.SetActive(false);
        angleUI.SetActive(false);

        foreach (GameObject obj in upAndDownButtons) {
            obj.SetActive(true);
        }

        playerController.RotatingEnd();
    }

    private void gameOver() {
        playerController.extraAudioSource.gameObject.SetActive(false);
        soundManager.Play(SoundManager.SoundType.EFX, "gameOver");

        modal.SetActive(true);

        //착지 위치 기반 점수 계산
        score = System.Math.Round(character.transform.position.x / 6.0f);
        if (isUnstableLand) {
            Debug.Log("불안정 착지로 인한 감점");
            score = System.Math.Round(score * 0.75f);
        }
        score *= (1 + qte_magnification);
        //totalScore = score + bonusScore;

        Transform innerModal = modal.transform.Find("InnerModal");

        innerModal.Find("Labels/Point/Data").GetComponent<Text>().text = 
            System.Math.Truncate(score)
            + " + " + System.Math.Truncate(bonusScore)
            + "(배율 : x" + qte_magnification + ")";

        innerModal.Find("Labels/Distance/Data").GetComponent<Text>().text = System.Math.Truncate(character.transform.position.x) + " M";
        innerModal.Find("Labels/Time/Data").GetComponent<Text>().text = System.Math.Truncate(playTime) + "초";

        Debug.Log("추가 배율 : " + qte_magnification);

        int randNum = UnityEngine.Random.Range(0, 100);
        if(randNum < 30) {
            adsBtn.SetActive(true);
        }
        else {
            adsBtn.SetActive(false);
        }

        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = preFixedDeltaTime;
    }

    public void resumneButtonPressed() {
        _eventManger.TriggerEvent(new SkiJump_Resume());

        Time.timeScale = 1;

        soundManager.Play(SoundManager.SoundType.EFX, "resumeBtn");
    }

    private void resume(SkiJump_Resume e) {
        Vector2 dir = new Vector2(1, 1);
        charRb.AddForce(dir * 40f, ForceMode2D.Impulse);

        isLanded = false;
        isQTE_occured = false;

        resumeBtn.SetActive(false);
    }

    private void onResultCallback(ShowResult result) {
        switch (result) {
            case ShowResult.Finished: {
                    Debug.Log("The ad was successfully shown.");
                    //획득 포인트를 2배 증가시킨다.
                    //이어하기 버튼 비활성화
                    resumeBtn.SetActive(false);
                    adsBtn.SetActive(false);

                    score *= 2;
                    bonusScore *= 2;

                    Transform innerModal = modal.transform.Find("InnerModal");

                    innerModal.Find("Labels/Point/Data").GetComponent<Text>().text = 
                        System.Math.Truncate(score)
                        + " + " + System.Math.Truncate(bonusScore)
                        + "(배율 : x" + qte_magnification + ")";
                    break;
                }
            case ShowResult.Skipped: {
                    Debug.Log("The ad was skipped before reaching the end.");

                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed: {
                    Debug.LogError("The ad failed to be shown.");

                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }

    private void connectUnityAdsButton() {
        Button button = adsBtn.GetComponent<Button>();
        button.onClick.AddListener(AdButtonClicked);

        UnityAdsHelper.Instance.onResultCallback += onResultCallback;
    }

    private void AdButtonClicked() {
        UnityAdsHelper.Instance.ShowRewardedAd();
    }

    private void removeListener() {
        _eventManger.RemoveListener<SkiJump_JumpEvent>(_OnJumpArea);
        _eventManger.RemoveListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.RemoveListener<SkiJump_UnstableLandingEvent>(_UnstableLanding);
        _eventManger.RemoveListener<SkiJump_ArrowRotEndEvent>(_OffZooming);
        _eventManger.RemoveListener<SkiJump_Resume>(resume);
    }
}
