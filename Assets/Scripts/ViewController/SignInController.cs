using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInController : MonoBehaviour {
    public GameObject signupPanel;
    private GameManager gm;
    public InputField nickname;
    public Text msg;

    private void Awake() {
        //PlayerPrefs.DeleteAll();
    }

    private void Start() {
        gm = GameManager.Instance;
        getInfo();
    }

    public void getInfo() {
        gm.init();
        string str = "보너스 시간 정보 : " + gm.bonus_times[0] + ", " + gm.bonus_times[1];
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb
            .Append(str)
            .Append("\n")
            .Append("폴 사이 정보 : ");
        for (int i = 0; i <= gm.poll_intervals.Length - 1; i++) {
            sb.Append(" " + gm.poll_intervals[i]);
        }
        sb.Append("\n");
        sb.Append("행간 정보 : ");
        for (int i = 0; i <= gm.vertical_intervals.Length - 1; i++) {
            sb.Append(" " + gm.vertical_intervals[i]);
        }
        sb.Append("\n");
        sb.Append("평행이동 관련 정보 : ");
        for (int i = 0; i <= gm.pararell_intervals.Length - 1; i++) {
            sb.Append(" " + gm.pararell_intervals[i]);
        }
        msg.text = sb.ToString();
    }

    public void signIn() {
        string nickname = PlayerPrefs.GetString("nickname");
        if (string.IsNullOrEmpty(nickname)) {
            signUp();
            //google play 연동 시도
            //GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
            //GooglePlayConnection.Instance.Connect();
        }
        else {
            gm.nickname = nickname;

            int character = PlayerPrefs.GetInt("character");
            gm.character = character;
            gm.nickname = PlayerPrefs.GetString("nickname");

            SceneManager.LoadScene("Main");
        }
    }

    private void signUp() {
        signupPanel.SetActive(true);
    }

    //최종 회원가입 버튼 처리
    public void submit() {
        PlayerPrefs.SetString("nickname", nickname.text);
        gm.nickname = nickname.text;
        PlayerPrefs.SetInt("character", gm.character);

        signupPanel.SetActive(false);
        SceneManager.LoadScene("Main");
    }

    private void ActionConnectionResultReceived(GooglePlayConnectionResult result) {
        if (result.IsSuccess) {
            signUp();
        }
        else {
            msg.text = result.code.ToString();
        }
    }
}
