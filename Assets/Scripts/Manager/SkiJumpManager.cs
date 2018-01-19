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
    public ResultModalController modal;
    public GameObject pauseModal;

    public GameObject
        freezingSign,
        character,
        forceButton,                //가속 버튼
        angleUI,                    //각도기 UI
        jumpButton;                //점프하기 버튼

    public Text 
        speedText,
        distanceText,
        timeText;

    public GameObject[] upAndDownButtons;

    public float forceAmount;           //가속 정도
    public float statBasedSpeedForce;  //Stat을 적용한 가속 정도
    public float 
        slowdownFactor,     //슬로우 모션 정도
        frictionFactor;     //마찰 계수
    public float qte_magnification = 0;
    private Rigidbody2D charRb;
    private bool
        isUnstableLand = false,
        canClickArrowBtn = false,
        isEndQTE = false,
        isGameStart = false;

    public bool isGameEnd = false;

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

    public float 
        playTime,
        lastTime,
        preFixedDeltaTime,
        canClickArrowBtnTime;         //점프 화살표 클릭이 가능한 시간

    private SoundManager soundManager;
    private float lastTimeIncInterval = 1000;
    private void Awake() {
        _eventManger = EventManager.Instance;
        pm = SaveManager.Instance;

        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        bonusScore = 0;

        playTime = 0;
        lastTime = 40;

        isGameStart = false;
        qte_magnification = 0;
    }

    private void OnDisable() {
        Time.timeScale = 1;
        Screen.orientation = ScreenOrientation.Portrait;
        removeListener();
    }

    private void _OnJumpArea(SkiJump_JumpEvent e) {
        angleUI.SetActive(true);
        jumpButton.SetActive(true);

        playerController.isSliding = false;
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
        _eventManger.AddListener<SkiJump_QTE_start>(startQTE);
        _eventManger.AddListener<SkiJump_QTE_end>(endQTE);

        preFixedDeltaTime = Time.fixedDeltaTime;
        GameManager.Instance.setExitModal(pauseModal);
    }

    private void startQTE(SkiJump_QTE_start e) {
        Debug.Log("QTE 시작");
        qteButton.SetActive(true);
        Time.timeScale = 0;
    }

    private void endQTE(SkiJump_QTE_end e) {
        Time.timeScale = 1;
        isEndQTE = true;
        qteButton.SetActive(false);
    }

    private void Update() {
        float speed = playerController.rb.velocity.magnitude;
        float dist = playerController.transform.position.x;
        if (playerController.isSliding) {
            speedText.text = playerController.virtualSpeed + ".00KM/h";
        }
        else {
            speedText.text = System.Math.Round(speed * 3, 2) + "KM/h";
        }

        if(dist <= 0) {
            dist = 0;
        }
        distanceText.text = System.Math.Truncate(dist) + "M";
        timeText.text = System.Math.Truncate(lastTime) + " 초";
    }

    private void FixedUpdate() {
        if (isGameStart) {
            playTime += Time.deltaTime;
            lastTime -= Time.deltaTime;
        }

        if (angleUI.activeSelf) {
            canClickArrowBtnTime -= Time.realtimeSinceStartup;
            //Debug.Log(canClickArrowBtnTime);
            if(canClickArrowBtnTime <= 0) {
                canClickArrowBtn = true;
            }
        }

        if(lastTime <= 0) {
            lastTime = 0;
            isGameEnd = true;
        }

        if (isEndQTE) {
            charRb.velocity = new Vector2(charRb.velocity.x * 0.995f, charRb.velocity.y * 0.995f);
            if (charRb.velocity.x <= 0) {
                gameOver();
            }
        }
        double value = System.Math.Round(charRb.transform.position.y * 3f);
        height.text = value + " M";
        heightSlider.value = (float)value;

        if(charRb.transform.position.x >= lastTimeIncInterval) {
            lastTime += 20.0f;
            lastTimeIncInterval += 1000;
        }
    }

    public void startButtonPressed() {
        Time.timeScale = 1;
        charRb.constraints = RigidbodyConstraints2D.None;
        CM_controller.playableDirectors[0].gameObject.SetActive(false);
        playerController.SkelAnimChange("run", true);
        forceButton.SetActive(false);

        playerController.AddForce();
    }

    //점프 버튼 클릭
    public void jumping() {
        if (canClickArrowBtn) {
            //Debug.Log("Arrow 클릭!");
            arrowController.OnPointerUp();
            charRb.AddForce(angleUI.transform.up * 10, ForceMode2D.Impulse);
        }
        isGameStart = true;
    }

    public void Onpause() {
        Time.timeScale = 0;
    }

    public void OnResume() {
        Time.timeScale = 1.0f;
    }

    public void OnQuit() {
        SceneManager.LoadScene("main");
    }

    private void _UnstableLanding(SkiJump_UnstableLandingEvent e) {
        charRb.velocity = new Vector2(charRb.velocity.x, 0);
        isUnstableLand = true;
    }

    private void _OnLanding(SkiJump_LandingEvent e) {
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

    //이어하기 버튼 클릭
    public void resumneButtonPressed() {
        _eventManger.TriggerEvent(new SkiJump_Resume());
    }

    private void resume(SkiJump_Resume e) {
        Vector2 dir = new Vector2(1, 1);
        charRb.transform.position = new Vector3(charRb.transform.position.x, 2f);
        charRb.velocity = Vector3.zero;
        charRb.transform.rotation = Quaternion.identity;

        charRb.AddForce(dir * 20f, ForceMode2D.Impulse);
        isQTE_occured = false;

        isGameEnd = false;
        lastTime = 40f;
        isEndQTE = false;

        Time.timeScale = 1;
    }

    public void addCrystal(int amount) {
        pm.addCrystal(amount);
    }

    private void removeListener() {
        _eventManger.RemoveListener<SkiJump_JumpEvent>(_OnJumpArea);
        _eventManger.RemoveListener<SkiJump_LandingEvent>(_OnLanding);
        _eventManger.RemoveListener<SkiJump_UnstableLandingEvent>(_UnstableLanding);
        _eventManger.RemoveListener<SkiJump_ArrowRotEndEvent>(_OffZooming);
        _eventManger.RemoveListener<SkiJump_Resume>(resume);
        _eventManger.AddListener<SkiJump_QTE_start>(startQTE);
        _eventManger.RemoveListener<SkiJump_QTE_end>(endQTE);
    }
}
