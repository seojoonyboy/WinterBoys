﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkiJumpManager : Singleton<SkiJumpManager> {
    protected SkiJumpManager() { }
    private GameManager gm;
    public SkiJumpCameraController cameraController;
    public SkiJumpPlayerController playerController;
    public ArrowRotate arrowController;

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
    private bool isLanded = false;
    private void OnEnable() {
        SlowMotion.OnJumpArea += _OnJumpArea;
        SkiJumpCameraController.OffZooming += _OffZooming;
        Landing.OnLanding += _OnLanding;

        Time.timeScale = 1;
    }

    private void OnDisable() {
        SlowMotion.OnJumpArea -= _OnJumpArea;
        SkiJumpCameraController.OffZooming -= _OffZooming;
        Landing.OnLanding -= _OnLanding;

        isLanded = false;
    }

    private void Start() {
        //gm = GameManager.Instance;
        forceButton.SetActive(true);
        angleUI.SetActive(false);
        jumpButton.SetActive(false);
        Screen.orientation = ScreenOrientation.LandscapeRight;

        initGroundEnv();

        charRb = character.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (isLanded && charRb.velocity.x <= 0) {
            modal.SetActive(true);
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

    public void AddForce() {
        charRb.AddForce(character.transform.right * forceAmount);
    }

    private void _OnJumpArea() {
        //Debug.Log("점프대 도착");
        cameraController.zoomIn(5);

        forceButton.SetActive(false);
        angleUI.SetActive(true);
        jumpButton.SetActive(true);
    }

    //점프 버튼 클릭
    public void jumping() {
        arrowController.stopRotating();
        cameraController.zoomOut();

        charRb.AddForce(angleUI.transform.up * 10, ForceMode2D.Impulse);
    }

    private void _OffZooming() {
        Time.timeScale = 1.0f;

        jumpButton.SetActive(false);
        angleUI.SetActive(false);

        foreach(GameObject obj in upAndDownButtons) {
            obj.SetActive(true);
        }
    }

    private void _OnLanding() {
        Debug.Log("착지");
        isLanded = true;
    }
}
