using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour {
    private GameManager gm;
    public Text 
        nickname,
        characterName;
    public string[] charNames;

    private void Awake() {
        gm = GameManager.Instance;

        string str = RemoteSettings.GetString("Characters_name");
        string[] spl_str = str.Split(',');
        for (int i = 0; i < charNames.Length; i++) {
            charNames[i] = spl_str[i];
        }
    }

    private void Start() {
        nickname.text = gm.nickname;
        characterName.text = charNames[gm.character];
    }

    public void LoadGame(int type) {
        switch (type) {
            case 0:
                SceneManager.LoadScene("DownHill");
                break;
            case 1:
                SceneManager.LoadScene("Skeleton");
                break;
            case 2:
                SceneManager.LoadScene("SkiJump");
                break;
        }
    }

    public void rankingShow() {
        UM_GameServiceManager.Instance.ShowLeaderBoardsUI();
    }


}
