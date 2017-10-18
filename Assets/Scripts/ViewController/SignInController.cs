using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInController : MonoBehaviour {
    public GameObject signupPanel;
    private GameManager gm;
    public InputField nickname;

    private void Awake() {
        gm = GameManager.Instance;
        //PlayerPrefs.DeleteAll();
    }

    public void signIn() {
        string nickname = PlayerPrefs.GetString("nickname");
        if (string.IsNullOrEmpty(nickname)) {
            signUp();
        }
        else {
            gm.nickname = nickname;

            int character = PlayerPrefs.GetInt("character");
            gm.character = character;

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
}
