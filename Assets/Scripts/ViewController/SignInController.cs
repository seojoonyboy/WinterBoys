using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine;
using Spine.Unity;

public class SignInController : MonoBehaviour {
    public GameObject 
        signupPanel,
        topInputBg;
    private GameManager gm;
    public InputField nickname;
    public Text msg;
    private bool isInputMove = false;

    public Transform inputPos;
    private Vector3 inputOriginPos;
    public SkeletonGraphic chara;
    private void Awake() {
        //PlayerPrefs.DeleteAll();
        inputOriginPos = nickname.transform.position;
    }

    private void Start() {
        gm = GameManager.Instance;
        getInfo();
    }

    private void Update() {
        if (!nickname.isFocused) {
            nickname.transform.position = inputOriginPos;
            isInputMove = false;
            topInputBg.SetActive(false);
        }
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
        UM_GameServiceManager.OnPlayerConnected += OnPlayerConnected;
        UM_GameServiceManager.OnPlayerDisconnected += OnPlayerDisconnected;
        UM_GameServiceManager.Instance.Connect();

        string nickname = PlayerPrefs.GetString("nickname");
        if (string.IsNullOrEmpty(nickname)) {
            signUp();
        }
        else {
            gm.nickname = nickname;

            int character = PlayerPrefs.GetInt("character");
            gm.character = character;
            gm.nickname = PlayerPrefs.GetString("nickname");

            SceneManager.LoadScene("Main");
        }

        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 0);
    }

    private void OnPlayerConnected() {
        Debug.Log("Player Connected");
    }

    private void OnPlayerDisconnected() {
        Debug.Log("Player Disconnected");
    }

    private void signUp() {
        signupPanel.SetActive(true);

        SoundManager.Instance.Play(SoundManager.SoundType.BGM, 1);
    }

    //최종 회원가입 버튼 처리
    public void submit() {
        PlayerPrefs.SetString("nickname", nickname.text);
        gm.nickname = nickname.text;
        PlayerPrefs.SetInt("character", gm.character);

        TrackEntry track = chara.AnimationState.SetAnimation(0, "approval", false);
        Invoke("changeScene", track.AnimationEnd + 0.5f);

        SoundManager.Instance.Play(SoundManager.SoundType.CHARCHANGE_SCENE, 1);
    }

    private void changeScene() {
        SceneManager.LoadScene("Main");
        //signupPanel.SetActive(false);
    }

    private void ActionConnectionResultReceived(GooglePlayConnectionResult result) {
        if (result.IsSuccess) {
            signUp();
        }
        else {
            msg.text = result.code.ToString();
        }
    }
    public void onClickInput() {
        if (!isInputMove) {
            nickname.transform.position = inputPos.transform.position;
            topInputBg.SetActive(true);
        }
        else {
            nickname.transform.position = inputOriginPos;
            topInputBg.SetActive(false);
        }
        isInputMove = !isInputMove;
        //Invoke("GetKeyboardSize", 3.0f);
    }

    //private void GetKeyboardSize() {
    //    using(AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
    //        AndroidJavaObject View = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

    //        using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect")) {
    //            View.Call("getWindowVisibleDisplayFrame", Rct);
    //            height.text = (Screen.height - Rct.Call<int>("height")).ToString();
    //        }
    //    }
    //}
}
