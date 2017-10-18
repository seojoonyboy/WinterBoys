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

    private void Awake() {
        gm = GameManager.Instance;
    }

    private void Start() {
        Time.timeScale = 1;
        remainTime = gm.startTime;
        remainTimeTxt.text = "남은 시간 : " + gm.startTime + " 초";
        InvokeRepeating("timeDec", 1.0f, 1.0f);
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
            modal.SetActive(true);
            gm.gameOver();
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
    }
}
