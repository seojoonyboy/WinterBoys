using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour {
    private GameManager gm;

    public ReadyController ready;
    public Text 
        nickname,
        characterName;
    public string[] charNames;
    public Sprite[] characters;
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
        //characterName.text = charNames[gm.character];

        SoundManager.Instance.Play(SoundManager.SoundType.BGM, "selGame");
    }

    public void LoadGame(int type) {
        switch (type) {
            case 0:
                ready.open(SportType.DOWNHILL);
                break;
            case 1:
                ready.open(SportType.SKELETON);
                break;
            case 2:
                ready.open(SportType.SKIJUMP);
                break;
        }
    }

    public void rankingShow() {
        UM_GameServiceManager.Instance.ShowLeaderBoardsUI();

        //SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 7);
    }

    public void efxPlay(string name) {
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, name);
    }
}
