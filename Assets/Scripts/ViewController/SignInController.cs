using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine;
using Spine.Unity;

public class SignInController : MonoBehaviour {
    public GameObject 
        signupPanel;
    private GameManager gm;
    public Text msg;
    public Text nickName;
    public SkeletonGraphic chara;
    public GameObject noNickNameModal;

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
        msg.text = sb.ToString();
    }

    public void signIn() {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("nickname"))) {
            signUp();
        }
        else {
            int character = PlayerPrefs.GetInt("character");
            CharacterManager.Instance.currentCharacter = character;

            SceneManager.LoadScene("Main");
        }

        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "main_startBtn");
    }

    private void signUp() {
        signupPanel.SetActive(true);
        SoundManager.Instance.Play(SoundManager.SoundType.BGM, "regist");
    }

    //최종 회원가입 버튼 처리
    public void submit() {
        if (string.IsNullOrEmpty(nickName.text)) {
            noNickNameModal.SetActive(true);
            return;
        }
        int character = CharacterManager.Instance.currentCharacter;
        PlayerPrefs.SetString("nickname", nickName.text);
        PlayerPrefs.SetInt("character", character);
        CharacterManager.Instance.sold(character);
        TrackEntry track = chara.AnimationState.SetAnimation(0, "approval", false);
        Invoke("changeScene", track.AnimationEnd + 0.5f);
        Invoke("submitSound", track.AnimationEnd - 0.8f);

        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "registBtn");
    }

    private void submitSound() {
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "stamp");
    }

    private void changeScene() {
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
