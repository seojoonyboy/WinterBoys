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
    public Text 
        remainTimeTxt,
        effectTxt;
    public int passNum = 0;
    public int comboNum = 0;

    private int maxCombo = 0;
    private int score = 0;

    public Ski_PlayerController playerController;
    public delegate void gameOverHandler();
    public static event gameOverHandler OngameOver;
    private void Awake() {
        gm = GameManager.Instance;
        umgm = UM_GameServiceManager.Instance;
        pm = PointManager.Instance;
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

        initEventHandler();
    }

    private void Update() {
        effectTxt.text = "아이템 효과 : " + playerController.playerState.ToString();
    }

    private void initEventHandler() {
        OngameOver += OnGameOver;
    }

    public void mainLoad() {
        UM_GameServiceManager.ActionScoreSubmitted -= HandleActionScoreSubmitted;
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
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

        Debug.Log("콤보횟수 : " + comboNum);
    }

    public void scoreInc(int amount) {
        score += amount;
    }

    public void OnGameOver() {
        Time.timeScale = 0;

        modal.SetActive(true);

        Transform innerModal = modal.transform.Find("InnerModal");

        Vector3 playerEndPos = playerController.playerPos;
        var distOfMeter = System.Math.Truncate(playerEndPos.y);

        score += (int)(-1 * distOfMeter / gm.points[0]);
        umgm.SubmitScore("DownHill", (long)score);

        Transform values = innerModal.Find("DataPanel/Values");
        values.Find("Dist").GetComponent<Text>().text = -1 * distOfMeter + " M";
        values.Find("Combo").GetComponent<Text>().text = maxCombo.ToString();

        float additionalScore = score * (maxCombo * 0.02f);
        values.Find("Point").GetComponent<Text>().text = score + " + " + additionalScore;

        innerModal.Find("TotalScorePanel/Value").GetComponent<Text>().text = (score + additionalScore).ToString();

        pm.setRecord((float)distOfMeter * -1f, SportType.DOWNHILL);
        pm.addPoint(score);

        Debug.Log("최대 콤보 : " + maxCombo);
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
