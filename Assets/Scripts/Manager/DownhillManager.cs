using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using GameEvents;
using System;

public class DownhillManager : MonoBehaviour {
    public ResultModalController modal;
    private GameManager gm;
    private SaveManager pm;
    public int remainTime;

    [HideInInspector] public float playTime;

    [SerializeField] private float _timeScale = 1;

    public Text 
        remainTimeTxt,
        effectTxt,
        distanceTxt,
        speedTxt;

    public int passNum = 0;
    public int comboNum = 0;

    private int maxCombo = 0;
    private int score = 0;

    private double 
        distOfMeter = 0,
        preDistOfMeter = 0,
        additionalScore = 0;

    public Ski_PlayerController playerController;
    public EventManager _eventManager;

    public delegate void gameOverHandler(GameoverReason reason);
    public static event gameOverHandler OngameOver;

    public SoundManager soundManager;
    private GameoverReason gameoverReason;
    private void Awake() {
        gm = GameManager.Instance;
        pm = SaveManager.Instance;
        soundManager = SoundManager.Instance;

        _eventManager = EventManager.Instance;
    }

    private void Start() {
        init();
        InvokeRepeating("timeDec", 1.0f, 1.0f);
    }

    private void OnDisable() {
        removeListener();
    }

    private void Update() {
        setUIText();
    }

    public float getTimeScale {
        get {
            return _timeScale;
        }
    }

    public float setTimeScale {
        set {
            _timeScale = value;
        }
    }

    private void setUIText() {
        effectTxt.text = "아이템 효과 : " + playerController.playerState.ToString();
        playTime += Time.deltaTime;
        distOfMeter = System.Math.Truncate(playerController.virtualPlayerPosOfY);
        distanceTxt.text = distOfMeter + " M";
        float speed = playerController.GetComponent<Rigidbody2D>().velocity.magnitude;
        speedTxt.text = System.Math.Truncate(speed) + "KM/S";
    }

    private void init() {
        initEventHandler();

        remainTime = gm.startTime;

        score = 0;
        comboNum = 0;
        maxCombo = 0;

        playTime = 0;
        distOfMeter = 0;
        preDistOfMeter = 0;
        additionalScore = 0;

        soundManager.Play(SoundManager.SoundType.BGM, "dh");
    }

    private void initEventHandler() {
        _eventManager.AddListener<Downhill_RepositionCharToResume>(resetCharPosReq);
        _eventManager.AddListener<Downhill_RepositionCharToResumeFinished>(finishResetCharPosReq);

        _eventManager.TriggerEvent(new Downhill_RepositionCharToResume());

        OngameOver += OnGameOver;
    }

    void timeDec() {
        if(_timeScale == 0) { return; }

        remainTime -= 1;
        remainTimeTxt.text = remainTime + " 초";

        if(remainTime <= 0) {
            remainTime = 0;
            OnGameOver(GameoverReason.TIMEEND);
        }
    }

    public void passNumInc() {
        passNum++;
        if(passNum >= gm.bonus_times[0]) {
            remainTime += (int)gm.bonus_times[1];
            passNum = 0;
            Debug.Log("시간 증가");
            soundManager.Play(SoundManager.SoundType.EFX, "timeAdd");
        }
        Debug.Log("통과 갯수 : " + passNum);
        scoreInc(5);
    }

    //콤보 처리(성공, 실패)
    public void setCombo(int isPass) {
        if (isPass == 0) {
            comboNum = 0;
        }
        else if(isPass == 1) {
            comboNum++;
        }

        if(comboNum > maxCombo) {
            maxCombo = comboNum;
        }

        Debug.Log("콤보횟수 : " + comboNum);
    }

    public void scoreInc(int amount) {
        score += amount;
    }

    public void OnGameOver(GameoverReason reason) {
        setTimeScale = 0;

        modal.gameObject.SetActive(true);

        Transform innerModal = modal.transform.Find("Panel");

        Vector3 playerEndPos = playerController.playerPos;

        score += (int)((distOfMeter - preDistOfMeter) / gm.points[0]);
        preDistOfMeter = distOfMeter;
        additionalScore = System.Math.Truncate(score * (maxCombo * 0.02f));

        modal.setGame(gameObject, SportType.DOWNHILL);
        modal.setData(playTime, (float)distOfMeter, score, (int)additionalScore, maxCombo, null);

        gameoverReason = reason;
    }

    //이어하기 버튼 클릭
    public void resume() {
        setTimeScale = 1;

        remainTime = 30;

        //캐릭터를 이동 가는한 영역의 중앙으로 강제이동시킴(Side Tile 충돌하여 게임 종료되었을 시 재개해도 바로 다시 충돌함)
        //회전전부 초기화
        if(gameoverReason == GameoverReason.SIDETILE) {
            playerController.resetQuarternion();
            _eventManager.TriggerEvent(new Downhill_RepositionCharToResume());
        }
    }

    public void addCrystal(int amount) {
        pm.addCrystal(amount);
    }

    private void resetCharPosReq(Downhill_RepositionCharToResume e) {
        setTimeScale = 0;
        if (playerController != null) {
            playerController.GetComponent<CharResumePosFilter>().enabled = true;
        }
    }

    private void finishResetCharPosReq(Downhill_RepositionCharToResumeFinished e) {
        setTimeScale = 1;
    }

    private void removeListener() {
        _eventManager.RemoveListener<Downhill_RepositionCharToResume>(resetCharPosReq);
        _eventManager.RemoveListener<Downhill_RepositionCharToResumeFinished>(finishResetCharPosReq);
    }

    public enum GameoverReason {
        SIDETILE,
        TIMEEND
    }
}
