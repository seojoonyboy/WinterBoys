using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using UnityEngine.UI;

public class DownhillManager : MonoBehaviour {
    public GameObject modal;
    private GameManager gm;
    public int remainTime;
    public Text remainTimeTxt;
    public int passNum = 0;
    private int score = 0;

    public Ski_PlayerController playerController;
    public delegate void gameOverHandler();
    public static event gameOverHandler OngameOver;
    private void Awake() {
        gm = GameManager.Instance;
    }

    private void Start() {
        Time.timeScale = 1;
        remainTime = gm.startTime;
        remainTimeTxt.text = "남은 시간 : " + gm.startTime + " 초";
        InvokeRepeating("timeDec", 1.0f, 1.0f);

        initEventHandler();
    }

    private void initEventHandler() {
        OngameOver += OnGameOver;
    }

    public void mainLoad() {
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
        Debug.Log("통과 갯수 : " + passNum);
        scoreInc(5);
    }

    public void scoreInc(int amount) {
        score += amount;
    }

    public void OnGameOver() {
        Time.timeScale = 0;

        modal.SetActive(true);
        Text dist = modal.transform.Find("InnerModal/Dist").GetComponent<Text>();

        Vector3 playerEndPos = playerController.playerPos;
        string str = -1 * System.Math.Truncate(playerEndPos.y) + " M 이동";
        dist.text = str;
    }
}
