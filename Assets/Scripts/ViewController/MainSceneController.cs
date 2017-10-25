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

    public void downhillGameLoad() {
        SceneManager.LoadScene("Ski");
    }

    public void skeletonGameLoad() {
        SceneManager.LoadScene("Skeleton");
    }

    public void rankingShow() {
        UM_GameServiceManager.Instance.ShowLeaderBoardsUI();
    }


}
