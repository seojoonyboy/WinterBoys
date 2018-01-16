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
    public GameObject[] icons;
    public Transform iconPanel;

    [HideInInspector] public float playTime;

    [SerializeField] private float _timeScale = 1;

    public Text 
        remainTimeTxt,
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
    public GameObject pauseModal;
    private void Awake() {
        gm = GameManager.Instance;
        pm = SaveManager.Instance;
        soundManager = SoundManager.Instance;

        _eventManager = EventManager.Instance;
    }

    private void Start() {
        init();
        InvokeRepeating("timeDec", 1.0f, 1.0f);
        gm.setExitModal(pauseModal);
    }

    private void Update() {
        setUIText();
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
        playTime += Time.deltaTime;
        distOfMeter = System.Math.Truncate(playerController.virtualPlayerPosOfY);
        distanceTxt.text = distOfMeter + " M";
        float speed = playerController.GetComponent<Rigidbody2D>().velocity.magnitude;
        speedTxt.text = System.Math.Truncate(speed) + "KM/S";
    }

    private void init() {
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

    void timeDec() {
        if(_timeScale == 0) { return; }

        remainTime -= 1;
        remainTimeTxt.text = remainTime + " 초";

        if(remainTime <= 0) {
            remainTime = 0;
            OnGameOver();
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

    public void OnGameOver() {
        setTimeScale = 0;

        modal.gameObject.SetActive(true);

        Transform innerModal = modal.transform.Find("Panel");

        Vector3 playerEndPos = playerController.playerPos;

        score += (int)((distOfMeter - preDistOfMeter) / gm.points[0]);
        preDistOfMeter = distOfMeter;
        additionalScore = System.Math.Truncate(score * (maxCombo * gm.points[1]));

        modal.setGame(gameObject, SportType.DOWNHILL);
        modal.setData(playTime, (float)distOfMeter, score, (int)additionalScore, maxCombo, null);
    }

    //이어하기 버튼 클릭
    public void resume() {
        setTimeScale = 1;

        remainTime = 30;
    }

    public void addCrystal(int amount) {
        pm.addCrystal(amount);
    }

    //현재 받고 있는 아이템 효과 아이콘
    public void setItemEffectIcon(float coolTime, ItemType.DH type) {
        int index = -1;
        switch (type) {
            case ItemType.DH.BOOSTING_HILL:
                index = 0;
                break;
            case ItemType.DH.ANTI_SPEED_HILL:
                index = 1;
                break;
            case ItemType.DH.ENEMY_BUGS:
                index = 2;
                break;
            case ItemType.DH.OBSTACLE_POLL:
                index = 3;
                break;
            case ItemType.DH.OBSTACLE_OIL:
                index = 4;
                break;
            case ItemType.DH.ENEMY_BEAR:
                index = 5;
                break;
            case ItemType.DH.TREE:
                index = 6;
                break;
        }
        
        if(index == -1) { return; }

        GameObject icon = Instantiate(icons[index]);
        icon.transform.SetParent(iconPanel);
        icon.transform.localScale = Vector3.one;
        icon.transform.localPosition = Vector3.zero;

        var iconComp = icon.transform.Find("BlackBg").gameObject.AddComponent<Icon>();
        iconComp.cooltime = coolTime;
    }

    public enum GameoverReason {
        SIDETILE,
        TIMEEND
    }
}
