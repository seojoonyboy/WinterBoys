using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour {
    private GameManager gm;

    public GameObject charSelPanel;

    public ReadyController ready;
    public Text 
        nickname,
        characterName;
    public string[] charNames;
    private int charIndex = 0;
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
        characterName.text = charNames[gm.character];

        charIndex = gm.character;
        changeTexts(charIndex);

        SoundManager.Instance.Play(SoundManager.SoundType.BGM, 2);
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

    public void selchange(bool isLeft) {
        if(charNames.Length == 0) { return; }

        int dir = 1;

        if (isLeft) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        charIndex += dir;
        
        if(charIndex <= -1) {
            charIndex = charNames.Length - 1;
        }
        if(charIndex >= charNames.Length) {
            charIndex = 0;
        }

        changeTexts(charIndex);
    }

    public void rankingShow() {
        UM_GameServiceManager.Instance.ShowLeaderBoardsUI();

        //SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 7);
    }

    private void changeTexts(int index) {
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 7);

        Text charName = charSelPanel.transform.Find("CharName").GetComponent<Text>();
        Text bonusStat = charSelPanel.transform.Find("BonusStat").GetComponent<Text>();
        Text changeToCompete = charSelPanel.transform.Find("ChanceToCompete").GetComponent<Text>();
        Text btn_message = charSelPanel.transform.Find("Button/Message").GetComponent<Text>();

        Image image = charSelPanel.transform.Find("SelectPanel/Character").GetComponent<Image>();
        image.sprite = characters[index];

        charName.text = charNames[index];
        if(index == gm.character) {
            btn_message.text = "출전중...";
        }
        else {
            btn_message.text = "Hire\n 500 D / 6,000 P";
        }
    }

    public void efxPlay(int index) {
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, index);
    }
}
