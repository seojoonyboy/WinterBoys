using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class DownhillManager : MonoBehaviour {
    public GameObject 
        modal,
        resumeBtn,
        adShowBtn;

    private GameManager gm;
    private SaveManager pm;
    private UM_GameServiceManager umgm;
    public int remainTime;

    [HideInInspector] public float playTime;

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
    public delegate void gameOverHandler(GameoverReason reason);
    public static event gameOverHandler OngameOver;

    public SoundManager soundManager;
    private GameoverReason gameoverReason;
    private void Awake() {
        gm = GameManager.Instance;
        umgm = UM_GameServiceManager.Instance;
        pm = SaveManager.Instance;
        soundManager = SoundManager.Instance;
        UM_GameServiceManager.ActionScoreSubmitted += HandleActionScoreSubmitted;
    }

    private void Start() {
        Time.timeScale = 1;
        remainTime = gm.startTime;
        InvokeRepeating("timeDec", 1.0f, 1.0f);

        score = 0;
        comboNum = 0;
        maxCombo = 0;

        playTime = 0;
        distOfMeter = 0;
        preDistOfMeter = 0;
        additionalScore = 0;

        initEventHandler();

        soundManager.Play(SoundManager.SoundType.BGM, "dh");

        connectUnityAdsButton();

        if (!resumeBtn.activeSelf) {
            resumeBtn.SetActive(true);
        }
    }

    private void Update() {
        effectTxt.text = "아이템 효과 : " + playerController.playerState.ToString();
        playTime += Time.deltaTime;
        distOfMeter = System.Math.Truncate(playerController.virtualPlayerPosOfY);
        distanceTxt.text = distOfMeter + " M";
        float speed = playerController.GetComponent<Rigidbody2D>().velocity.magnitude;
        speedTxt.text = System.Math.Truncate(speed) + "KM/S";
    }

    private void initEventHandler() {
        OngameOver += OnGameOver;
    }

    public void mainLoad() {
        pm.setRecord((float)distOfMeter * -1f, SportType.DOWNHILL);
        pm.addPoint(score);

        UM_GameServiceManager.ActionScoreSubmitted -= HandleActionScoreSubmitted;
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;

        resumeBtn.SetActive(true);

        soundManager.Play(SoundManager.SoundType.EFX, "returnMain");
    }

    void timeDec() {
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
        //Debug.Log("통과 갯수 : " + passNum);
        //scoreInc(5);
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

        //Debug.Log("콤보횟수 : " + comboNum);
    }

    public void scoreInc(int amount) {
        score += amount;
    }

    public void OnGameOver(GameoverReason reason) {
        Time.timeScale = 0;

        modal.SetActive(true);

        Transform innerModal = modal.transform.Find("Panel");

        Vector3 playerEndPos = playerController.playerPos;

        score += (int)((distOfMeter - preDistOfMeter) / gm.points[0]);
        preDistOfMeter = distOfMeter;

        umgm.SubmitScore("DownHill", (long)score);

        Transform labels = innerModal.Find("Labels");
        labels.Find("Distance/Data").GetComponent<Text>().text = distOfMeter + " M";
        labels.Find("Combo/Data").GetComponent<Text>().text = maxCombo.ToString();

        additionalScore = System.Math.Truncate(score * (maxCombo * 0.02f));
        labels.Find("Point/Data").GetComponent<Text>().text = score + " + " + additionalScore;

        //innerModal.Find("TotalScorePanel/Value").GetComponent<Text>().text = (score + additionalScore).ToString();

        int playTime = (int)this.playTime;
        int minute = 0;
        if (playTime != 0) {
            minute = playTime / 60;
        }
        int second = playTime - (60 * minute);

        labels.Find("Time/Data").GetComponent<Text>().text = minute + " : " + second;
        gameoverReason = reason;

        int randNum = Random.Range(0, 100);
        if(randNum < 30) {
            adShowBtn.SetActive(true);
        }
        else {
            adShowBtn.SetActive(false);
        }

        soundManager.Play(SoundManager.SoundType.EFX, "gameOver");
    }

    //이어하기 버튼 클릭
    public void resume() {
        Time.timeScale = 1;
        remainTime = 30;

        modal.SetActive(false);

        //캐릭터를 중앙으로 강제이동시킴(Side Tile 충돌하여 게임 종료되었을 시 재개해도 바로 다시 충돌함)
        //회전전부 초기화
        if(gameoverReason == GameoverReason.SIDETILE) {
            playerController.resetQuarternion();
            playerController.transform.position = new Vector3(
                0,
                playerController.transform.position.y,
                playerController.transform.position.z);
        }
        resumeBtn.SetActive(false);
    }

    public void addCrystal(int amount) {
        pm.addCrystal(amount);
    }

    private void connectUnityAdsButton() {
        Button button = modal.transform.Find("Panel/Labels/Point/Advertise").GetComponent<Button>();
        button.onClick.AddListener(AdButtonClicked);

        UnityAdsHelper.Instance.onResultCallback += onResultCallback;
    }

    private void AdButtonClicked() {
        UnityAdsHelper.Instance.ShowRewardedAd();
    }

    private void onResultCallback(ShowResult result) {
        switch (result) {
            case ShowResult.Finished: {
                    Debug.Log("The ad was successfully shown.");

                    //획득 포인트를 2배 증가시킨다.
                    //이어하기 버튼 비활성화
                    resumeBtn.SetActive(false);
                    score *= 2;

                    Transform labels = modal.transform.Find("Panel/Labels");
                    labels.Find("Point/Data").GetComponent<Text>().text = score + " + " + additionalScore;
                    
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

    private void HandleActionScoreSubmitted(UM_LeaderboardResult res) {
        if (res.IsSucceeded) {
            UM_Score playerScore = res.Leaderboard.GetCurrentPlayerScore(UM_TimeSpan.ALL_TIME, UM_CollectionType.GLOBAL);
            Debug.Log("Score submitted, new player high score: " + playerScore.LongScore);
        }
        else {
            Debug.Log("Score submission failed: " + res.Error.Code + " / " + res.Error.Description);
        }
    }

    public enum GameoverReason {
        SIDETILE,
        TIMEEND
    }
}
