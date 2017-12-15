using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using UnityEngine.UI;

public class DownhillManager : MonoBehaviour {
    public GameObject modal;

    private GameManager gm;
    private PointManager pm;
    private UM_GameServiceManager umgm;
    public int remainTime;

    [HideInInspector] public float playTime;

    public Text 
        remainTimeTxt,
        effectTxt,
        distanceTxt;

    public int passNum = 0;
    public int comboNum = 0;

    private int maxCombo = 0;
    private int score = 0;

    private double distOfMeter = 0;

    public Ski_PlayerController playerController;
    public delegate void gameOverHandler();
    public static event gameOverHandler OngameOver;

    public SoundManager soundManager;
    private void Awake() {
        gm = GameManager.Instance;
        umgm = UM_GameServiceManager.Instance;
        pm = PointManager.Instance;
        soundManager = SoundManager.Instance;
        UM_GameServiceManager.ActionScoreSubmitted += HandleActionScoreSubmitted;
    }

    private void Start() {
        Time.timeScale = 1;
        remainTime = gm.startTime;
        remainTimeTxt.text = "남은 시간 : " + gm.startTime + " 초";
        InvokeRepeating("timeDec", 1.0f, 1.0f);

        score = 0;
        comboNum = 0;
        maxCombo = 0;

        playTime = 0;
        distOfMeter = 0;

        initEventHandler();

        soundManager.Play(SoundManager.SoundType.BGM, 4);
    }

    private void Update() {
        effectTxt.text = "아이템 효과 : " + playerController.playerState.ToString();
        playTime += Time.deltaTime;
        distOfMeter = System.Math.Truncate(playerController.virtualPlayerPosOfY);
        distanceTxt.text = distOfMeter + " M";
    }

    private void initEventHandler() {
        OngameOver += OnGameOver;
    }

    public void mainLoad() {
        UM_GameServiceManager.ActionScoreSubmitted -= HandleActionScoreSubmitted;
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;

        soundManager.Play(SoundManager.SoundType.DOWNHILL, 6);
    }

    void timeDec() {
        remainTime -= 1;
        remainTimeTxt.text = "남은 시간 : " + remainTime + " 초";

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
            soundManager.Play(SoundManager.SoundType.DOWNHILL, 3);
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

    public void OnGameOver() {
        Time.timeScale = 0;

        modal.SetActive(true);

        Transform innerModal = modal.transform.Find("InnerModal");

        Vector3 playerEndPos = playerController.playerPos;

        score += (int)(distOfMeter / gm.points[0]);
        umgm.SubmitScore("DownHill", (long)score);

        Transform values = innerModal.Find("DataPanel/Values");
        values.Find("Dist").GetComponent<Text>().text = distOfMeter + " M";
        values.Find("Combo").GetComponent<Text>().text = maxCombo.ToString();

        double additionalScore = System.Math.Truncate(score * (maxCombo * 0.02f));
        values.Find("Point").GetComponent<Text>().text = score + " + " + additionalScore;

        innerModal.Find("TotalScorePanel/Value").GetComponent<Text>().text = (score + additionalScore).ToString();

        int playTime = (int)this.playTime;
        int minute = 0;
        if (playTime != 0) {
            minute = playTime / 60;
        }
        int second = playTime - (60 * minute);

        values.Find("Time").GetComponent<Text>().text = minute + " : " + second;

        pm.setRecord((float)distOfMeter * -1f, SportType.DOWNHILL);
        pm.addPoint(score);

        GameObject resumeBtn = innerModal.Find("Buttons/ResumeButton").gameObject;
        int randNum = Random.Range(0, 100);
        if(randNum < 15) {
            resumeBtn.SetActive(true);
        }
        else {
            resumeBtn.SetActive(false);
        }

        soundManager.Play(SoundManager.SoundType.DOWNHILL, 4);
    }

    //이어하기 버튼 클릭
    public void resume() {
        Time.timeScale = 1;
        remainTime = 30;

        modal.SetActive(false);
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
}
