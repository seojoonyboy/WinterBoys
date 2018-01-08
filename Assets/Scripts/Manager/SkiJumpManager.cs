﻿using System;
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
    public ResultModalController modal;

    public GameObject
        character,
        forceButton,                //가속 버튼
        angleUI,                    //각도기 UI
        jumpButton;                //점프하기 버튼

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

    public Text height;

    public Slider heightSlider;
    public GameObject qteButton;
    public GameObject effectIconPanel;
    public GameObject[] effectIcons;

    public bool isQTE_occured = false;

    private float 
        playTime,
        preFixedDeltaTime;

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
        Time.timeScale = 1;
        Screen.orientation = ScreenOrientation.Portrait;
        removeListener();
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
    }

    private void Update() {
        float speed = playerController.rb.velocity.magnitude;
        float dist = playerController.transform.position.x;

        speedText.text = System.Math.Round(speed * 3, 2) + "KM/h";
        if(dist <= 0) {
            dist = 0;
        }
        distanceText.text = System.Math.Truncate(dist) + "M";
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

    public void addEffectIcon(int index, float cooltime) {
        GameObject icon = Instantiate(effectIcons[index]);
        icon.transform.SetParent(effectIconPanel.transform);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one;

        var cooltimeComp = icon.transform.Find("BlackBg").gameObject.AddComponent<Icon>();
        cooltimeComp.cooltime = cooltime;
    }

    public void gameOver() {
        playerController.extraAudioSource.gameObject.SetActive(false);

        //착지 위치 기반 점수 계산
        score = System.Math.Round(character.transform.position.x / 6.0f);
        if (isUnstableLand) {
            Debug.Log("불안정 착지로 인한 감점");
            score = System.Math.Round(score * 0.75f);
        }
        score *= (1 + qte_magnification);
        modal.setGame(gameObject, SportType.SKIJUMP);
        modal.setData(playTime, character.transform.position.x, (int)score, (int)bonusScore, null, qte_magnification);

        Debug.Log("추가 배율 : " + qte_magnification);

        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = preFixedDeltaTime;
    }

    public void resumneButtonPressed() {
        _eventManger.TriggerEvent(new SkiJump_Resume());

        Time.timeScale = 1;
    }

    private void resume(SkiJump_Resume e) {
        Vector2 dir = new Vector2(1, 1);
        charRb.transform.position = new Vector3(transform.position.x, 1.35f);
        charRb.velocity = Vector3.zero;

        charRb.AddForce(dir * 20f, ForceMode2D.Impulse);

        isLanded = false;
        isQTE_occured = false;

    }

    private void removeListener() {
        _eventManger.RemoveListener<SkiJump_JumpEvent>(_OnJumpArea);
        _eventManger.RemoveListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.RemoveListener<SkiJump_UnstableLandingEvent>(_UnstableLanding);
        _eventManger.RemoveListener<SkiJump_ArrowRotEndEvent>(_OffZooming);
        _eventManger.RemoveListener<SkiJump_Resume>(resume);
    }
}
