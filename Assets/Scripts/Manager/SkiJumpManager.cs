using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameEvents;
using UnityEngine.Advertisements;
using Spine.Unity;

public class SkiJumpManager : Singleton<SkiJumpManager> {
    protected SkiJumpManager() { }

    private List<IconList> iconList;

    private EventManager _eventManger;
    private SaveManager pm;

    public SkiJumpPlayerController playerController;
    public ArrowRotate arrowController;
    public SkiJumpCM_controller CM_controller;
    public SkiJumpBoardHolder boardHolder;
    public ResultModalController modal;
    public GameObject 
        pauseModal,
        timeWarningModal;

    public GameObject
        meterSign,                  //미터 표시기
        freezingSign,
        countdown,                  //시작전 카운트다운
        countdownBg,
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
        isQTETriggerOccured = false,
        isEndQTE = false,
        isGameStart = false,
        canPlayTimeLessEfx = true;

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

    public float 
        playTime,
        lastTime,
        preFixedDeltaTime,
        canClickArrowBtnTime;         //점프 화살표 클릭이 가능한 시간

    private SoundManager soundManager;
    private float lastTimeIncInterval = 1000;
    private float nextMeterSignInterval = 500;  //미터 표시 간격

    public IEnumerator WaitForRealSeconds(float time) {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time) {
            yield return null;
        }
    }

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

        iconList = new List<IconList>();
    }

    private void startQTE(SkiJump_QTE_start e) {
        if (isQTETriggerOccured) {
            return;
        }

        isQTETriggerOccured = true;
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
        timeText.text = System.Math.Truncate(lastTime) + " " + I2.Loc.LocalizationManager.GetTranslation("InGameCommon/sec");
    }

    private void FixedUpdate() {
        if (isGameStart) {
            playTime += Time.deltaTime;
            lastTime -= Time.deltaTime;
        }

        if(nextMeterSignInterval < charRb.transform.position.x) {
            OnMeterSign(nextMeterSignInterval);
            nextMeterSignInterval += 500;
        }

        if (angleUI.activeSelf) {
            canClickArrowBtnTime -= Time.realtimeSinceStartup;
            //Debug.Log(canClickArrowBtnTime);
            if(canClickArrowBtnTime <= 0) {
                canClickArrowBtn = true;
            }
        }

        if(lastTime <= 5 && lastTime > 0) {
            timeWarningModal.SetActive(true);
            if (canPlayTimeLessEfx) {
                soundManager.Play(SoundManager.SoundType.EFX, "timeless");
                canPlayTimeLessEfx = false;
            }
        }
        else {
            timeWarningModal.SetActive(false);
            canPlayTimeLessEfx = true;
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
        countdown.SetActive(true);
        countdownBg.SetActive(true);

        forceButton.SetActive(false);

        var skeletonGraphic = countdown.GetComponent<SkeletonGraphic>();
        Spine.AnimationState state = skeletonGraphic.AnimationState;
        Invoke("CountAnimEnd", state.Tracks.Items[0].AnimationEnd);
    }

    private void CountAnimEnd() {
        countdown.SetActive(false);
        countdownBg.SetActive(false);

        Time.timeScale = 1;
        charRb.constraints = RigidbodyConstraints2D.None;
        CM_controller.playableDirectors[0].gameObject.SetActive(false);
        playerController.SkelAnimChange("run", true);

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
        GameManager.Instance.LoadSceneFromIngame("Main", SportType.SKIJUMP);
    }

    private void _UnstableLanding(SkiJump_UnstableLandingEvent e) {
        charRb.velocity = new Vector2(charRb.velocity.x, 0);
        isUnstableLand = true;
    }

    private void _OnLanding(SkiJump_LandingEvent e) {
        playerController.extraAudioSource.gameObject.SetActive(true);
        if(!GameManager.Instance.optionData.efx) return;
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
        IconList preItem = iconList.Find(x => x.id == index);
        if (preItem != null) {
            Destroy(preItem.obj);
            iconList.Remove(preItem);
        }

        GameObject icon = Instantiate(effectIcons[index]);
        icon.transform.SetParent(effectIconPanel.transform);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one;

        var cooltimeComp = icon.transform.Find("BlackBg").gameObject.AddComponent<Icon>();
        cooltimeComp.cooltime = cooltime;

        IconList item = new IconList();
        item.id = index;
        item.obj = icon;
        iconList.Add(item);
    }

    public void gameOver() {
        playerController.extraAudioSource.gameObject.SetActive(false);

        //착지 위치 기반 점수 계산
        score = System.Math.Round(character.transform.position.x / 5.0f);
        //1000m 간격 보너스 배율
        double bonusPrecentage = 1 + (50 * System.Math.Round(character.transform.position.x / 1000) / 100);

        score *= (1 + qte_magnification + bonusPrecentage);

        if (isUnstableLand) {
            Debug.Log("불안정 착지로 인한 감점");
            score = System.Math.Round(score * 0.75f);
        }

        modal.setGame(gameObject, SportType.SKIJUMP);
        modal.setData(playTime, character.transform.position.x, (int)score, (int)bonusScore, null, qte_magnification);

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

        charRb.AddForce(dir * 20f, ForceMode2D.Impulse);

        isGameEnd = false;
        lastTime = 40f;
        isEndQTE = false;

        Time.timeScale = 1;

        charRb.centerOfMass = new Vector2(0, 0);
        isQTETriggerOccured = false;
    }

    public void addCrystal(int amount) {
        pm.addCrystal(amount);
    }

    private void OnMeterSign(float value) {
        meterSign.SetActive(true);
        meterSign.transform.Find("Text").GetComponent<Text>().text = value + " M";
        Invoke("OffMeterSign", 2.0f);
    }

    private void OffMeterSign() {
        meterSign.SetActive(false);
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

    public void efxPlay(string name) {
        soundManager.Play(SoundManager.SoundType.EFX, name);
    }

    public class IconList {
        public int id;
        public GameObject obj;
    }
}
